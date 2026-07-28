using LessonRunner.Application.Courses;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Participants;
using LessonRunner.Domain.Billing;
using LessonRunner.Domain.Groups;

namespace LessonRunner.Application.Billing;

public sealed class BillingService(
    IBillingRepository billingRepository,
    IParticipantRepository participantRepository,
    IGroupRepository groupRepository,
    ICourseRepository courseRepository,
    IPaymentProvider paymentProvider) : IBillingService
{
    public async Task<BillingOverviewDto> GetOverviewAsync(CancellationToken cancellationToken)
    {
        var pricePlans = await billingRepository.ListPricePlansAsync(cancellationToken);
        var enrollments = await billingRepository.ListEnrollmentsAsync(cancellationToken);
        var invoices = await billingRepository.ListInvoicesAsync(cancellationToken);
        var payments = await billingRepository.ListPaymentsAsync(cancellationToken);
        var credits = await ExpireOutdatedCreditsAsync(cancellationToken);
        var context = await LoadContextAsync(cancellationToken);

        foreach (var invoice in invoices.Where(IsOverdue))
        {
            invoice.Status = InvoiceStatus.Overdue;
            await billingRepository.UpdateInvoiceAsync(invoice, cancellationToken);
        }

        var mappedInvoices = invoices.Select(invoice => ToDto(invoice, context)).ToList();
        var kpis = new BillingKpiDto(
            mappedInvoices.Where(invoice => invoice.Status is "open").Sum(invoice => invoice.AmountCents),
            mappedInvoices.Where(invoice => invoice.Status is "overdue").Sum(invoice => invoice.AmountCents),
            mappedInvoices.Where(invoice => invoice.Status is "paid").Sum(invoice => invoice.AmountCents),
            enrollments.Count(enrollment => enrollment.Status == BillingEnrollmentStatus.Trial),
            enrollments.Count(enrollment => enrollment.Status == BillingEnrollmentStatus.Active),
            credits.Count(credit => credit.Status == LessonCreditStatus.Available));

        return new BillingOverviewDto(
            pricePlans.Select(plan => ToDto(plan, context)).ToList(),
            enrollments.Select(enrollment => ToDto(enrollment, pricePlans, context)).ToList(),
            mappedInvoices,
            payments.Select(ToDto).ToList(),
            kpis,
            credits.Select(credit => ToDto(credit, context)).ToList());
    }

    public async Task<PricePlanDto> CreatePricePlanAsync(UpsertPricePlanDto dto, CancellationToken cancellationToken)
    {
        var context = await LoadContextAsync(cancellationToken);
        var pricePlan = BuildPricePlan(new PricePlan { Name = "", Currency = "PLN" }, dto, context);
        await billingRepository.AddPricePlanAsync(pricePlan, cancellationToken);
        return ToDto(pricePlan, context);
    }

    public async Task<PricePlanDto?> UpdatePricePlanAsync(Guid id, UpsertPricePlanDto dto, CancellationToken cancellationToken)
    {
        var pricePlan = await billingRepository.GetPricePlanAsync(id, cancellationToken);
        if (pricePlan is null)
        {
            return null;
        }

        var context = await LoadContextAsync(cancellationToken);
        BuildPricePlan(pricePlan, dto, context);
        pricePlan.UpdatedAt = DateTimeOffset.UtcNow;
        await billingRepository.UpdatePricePlanAsync(pricePlan, cancellationToken);
        return ToDto(pricePlan, context);
    }

    public Task<bool> DeletePricePlanAsync(Guid id, CancellationToken cancellationToken) =>
        billingRepository.DeletePricePlanAsync(id, cancellationToken);

    public async Task<BillingEnrollmentDto> CreateEnrollmentAsync(CreateBillingEnrollmentDto dto, CancellationToken cancellationToken)
    {
        var participant = await participantRepository.GetByIdAsync(dto.ParticipantId, cancellationToken)
            ?? throw new ArgumentException("Wybrany uczestnik nie istnieje.");
        var group = await groupRepository.GetByIdAsync(dto.GroupId, cancellationToken)
            ?? throw new ArgumentException("Wybrana grupa nie istnieje.");

        if (!group.Enrollments.Any(enrollment => enrollment.ParticipantId == participant.Id && enrollment.Status == EnrollmentStatus.Enrolled))
        {
            throw new ArgumentException("Uczestnik musi być aktywnie zapisany do grupy.");
        }

        var pricePlan = dto.PricePlanId is Guid pricePlanId
            ? await billingRepository.GetPricePlanAsync(pricePlanId, cancellationToken)
                ?? throw new ArgumentException("Wybrany cennik nie istnieje.")
            : null;

        if (pricePlan is not null && !pricePlan.IsActive)
        {
            throw new ArgumentException("Wybrany cennik jest nieaktywny.");
        }

        var enrollment = new BillingEnrollment
        {
            ParticipantId = dto.ParticipantId,
            GroupId = dto.GroupId,
            CourseId = group.CourseId,
            PricePlanId = pricePlan?.Id,
            Status = dto.Trial ? BillingEnrollmentStatus.Trial : BillingEnrollmentStatus.Active,
            TrialEndsAt = dto.TrialEndsAt
        };

        await billingRepository.AddEnrollmentAsync(enrollment, cancellationToken);
        var context = await LoadContextAsync(cancellationToken);
        return ToDto(enrollment, await billingRepository.ListPricePlansAsync(cancellationToken), context);
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken)
    {
        var enrollment = await billingRepository.GetEnrollmentAsync(dto.BillingEnrollmentId, cancellationToken)
            ?? throw new ArgumentException("Wybrany zapis rozliczeniowy nie istnieje.");

        if (enrollment.Status == BillingEnrollmentStatus.Cancelled)
        {
            throw new ArgumentException("Nie można wystawić faktury dla anulowanego zapisu.");
        }

        var pricePlan = await ResolvePricePlanAsync(enrollment, cancellationToken)
            ?? throw new ArgumentException("Brak cennika dla tego zapisu.");
        var amountCents = dto.AmountCents ?? pricePlan.AmountCents;

        if (amountCents <= 0)
        {
            throw new ArgumentException("Kwota faktury musi być większa od zera.");
        }

        var dueDate = dto.DueDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

        // Numer wyznaczamy z aktualnego stanu bazy, więc dwa równoległe zapisy mogłyby trafić
        // na ten sam numer i odbić się od unikalnego indeksu. Zamiast zwracać 500, ponawiamy
        // próbę z przeliczonym numerem.
        const int maxAttempts = 3;

        for (var attempt = 1; ; attempt++)
        {
            var invoice = new Invoice
            {
                BillingEnrollmentId = enrollment.Id,
                ParticipantId = enrollment.ParticipantId,
                GroupId = enrollment.GroupId,
                CourseId = enrollment.CourseId,
                Number = await NextInvoiceNumberAsync(cancellationToken),
                AmountCents = amountCents,
                Currency = pricePlan.Currency,
                DueDate = dueDate,
                Status = InvoiceStatus.Open
            };

            try
            {
                await billingRepository.AddInvoiceAsync(invoice, cancellationToken);
                return ToDto(invoice, await LoadContextAsync(cancellationToken));
            }
            catch (Exception) when (attempt < maxAttempts)
            {
                // Kolizja numeru - następne podejście policzy go od nowa.
            }
        }
    }

    public async Task<InvoiceDto?> MarkInvoicePaidAsync(Guid invoiceId, RecordManualPaymentDto dto, CancellationToken cancellationToken)
    {
        var invoice = await billingRepository.GetInvoiceAsync(invoiceId, cancellationToken);
        if (invoice is null)
        {
            return null;
        }

        if (invoice.Status == InvoiceStatus.Cancelled)
        {
            throw new ArgumentException("Nie można opłacić anulowanej faktury.");
        }

        var amountCents = dto.AmountCents ?? invoice.AmountCents;
        if (amountCents <= 0)
        {
            throw new ArgumentException("Kwota płatności musi być większa od zera.");
        }

        var paidAt = dto.PaidAt ?? DateTimeOffset.UtcNow;
        var payment = await paymentProvider.RecordManualPaymentAsync(invoice, amountCents, paidAt, dto.ExternalId, cancellationToken);
        await billingRepository.AddPaymentAsync(payment, cancellationToken);

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = paidAt;
        await billingRepository.UpdateInvoiceAsync(invoice, cancellationToken);
        return ToDto(invoice, await LoadContextAsync(cancellationToken));
    }

    public async Task<InvoiceDto?> CancelInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await billingRepository.GetInvoiceAsync(invoiceId, cancellationToken);
        if (invoice is null)
        {
            return null;
        }

        if (invoice.Status == InvoiceStatus.Paid)
        {
            throw new ArgumentException("Nie można anulować opłaconej faktury.");
        }

        invoice.Status = InvoiceStatus.Cancelled;
        await billingRepository.UpdateInvoiceAsync(invoice, cancellationToken);
        return ToDto(invoice, await LoadContextAsync(cancellationToken));
    }

    private async Task<PricePlan?> ResolvePricePlanAsync(BillingEnrollment enrollment, CancellationToken cancellationToken)
    {
        var pricePlans = await billingRepository.ListPricePlansAsync(cancellationToken);
        if (enrollment.PricePlanId is Guid pricePlanId)
        {
            return pricePlans.FirstOrDefault(plan => plan.Id == pricePlanId && plan.IsActive);
        }

        return pricePlans.FirstOrDefault(plan => plan.IsActive && plan.GroupId == enrollment.GroupId)
            ?? pricePlans.FirstOrDefault(plan => plan.IsActive && enrollment.CourseId is not null && plan.CourseId == enrollment.CourseId);
    }

    private static PricePlan BuildPricePlan(PricePlan pricePlan, UpsertPricePlanDto dto, BillingContext context)
    {
        var name = (dto.Name ?? string.Empty).Trim();
        if (name.Length == 0)
        {
            throw new ArgumentException("Nazwa cennika jest wymagana.");
        }

        if (dto.CourseId is null && dto.GroupId is null)
        {
            throw new ArgumentException("Cennik musi być przypisany do kursu albo grupy.");
        }

        if (dto.CourseId is Guid courseId && !context.CourseNames.ContainsKey(courseId))
        {
            throw new ArgumentException("Wybrany kurs nie istnieje.");
        }

        if (dto.GroupId is Guid groupId && !context.GroupNames.ContainsKey(groupId))
        {
            throw new ArgumentException("Wybrana grupa nie istnieje.");
        }

        if (dto.AmountCents <= 0)
        {
            throw new ArgumentException("Cena musi być większa od zera.");
        }

        pricePlan.Name = name;
        pricePlan.CourseId = dto.CourseId;
        pricePlan.GroupId = dto.GroupId;
        pricePlan.AmountCents = dto.AmountCents;
        pricePlan.Currency = NormalizeCurrency(dto.Currency);
        pricePlan.IsActive = dto.IsActive;
        return pricePlan;
    }

    // --- Kredyty zajęciowe ---

    public async Task<IReadOnlyList<LessonCreditDto>> ListCreditsAsync(Guid? participantId, CancellationToken cancellationToken)
    {
        var credits = await ExpireOutdatedCreditsAsync(cancellationToken);
        var context = await LoadContextAsync(cancellationToken);

        return credits
            .Where(credit => participantId is null || credit.ParticipantId == participantId)
            .Select(credit => ToDto(credit, context))
            .ToList();
    }

    public async Task<LessonCreditDto> IssueCreditAsync(
        IssueLessonCreditDto dto,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var reason = (dto.Reason ?? string.Empty).Trim();

        if (reason.Length == 0)
        {
            throw new ArgumentException("Podaj powód przyznania kredytu.");
        }

        if (await participantRepository.GetByIdAsync(dto.ParticipantId, cancellationToken) is null)
        {
            throw new ArgumentException("Wybrany uczestnik nie istnieje.");
        }

        if (dto.AmountCents is <= 0)
        {
            throw new ArgumentException("Wartość kredytu musi być większa od zera.");
        }

        var credit = new LessonCredit
        {
            ParticipantId = dto.ParticipantId,
            GroupId = dto.GroupId,
            SourceSessionId = dto.SourceSessionId,
            Reason = reason[..Math.Min(reason.Length, 1000)],
            AmountCents = dto.AmountCents,
            Currency = dto.AmountCents is null ? null : NormalizeCurrency(dto.Currency),
            ExpiresAt = dto.ExpiresAt,
            IssuedByUserId = actingUserId
        };

        await billingRepository.AddCreditAsync(credit, cancellationToken);
        return ToDto(credit, await LoadContextAsync(cancellationToken));
    }

    public async Task<LessonCreditDto?> UseCreditAsync(
        Guid creditId,
        UseLessonCreditDto dto,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var credit = await billingRepository.GetCreditAsync(creditId, cancellationToken);

        if (credit is null)
        {
            return null;
        }

        if (!credit.IsUsable(DateOnly.FromDateTime(DateTime.UtcNow)))
        {
            throw new ArgumentException($"Kredyt jest w stanie „{credit.Status.Label()}” i nie da się go wykorzystać.");
        }

        if (!Enum.TryParse<LessonCreditUsage>(dto.Usage, ignoreCase: true, out var usage) || usage == LessonCreditUsage.None)
        {
            throw new ArgumentException("Wskaż, na co kredyt został wykorzystany.");
        }

        credit.Status = LessonCreditStatus.Used;
        credit.Usage = usage;
        credit.UsedAt = DateTimeOffset.UtcNow;
        credit.UsedByUserId = actingUserId;
        credit.UsedForSessionId = dto.UsedForSessionId;
        credit.UsedForInvoiceId = dto.UsedForInvoiceId;
        credit.UsageNote = Shorten(dto.UsageNote, 1000);

        await billingRepository.UpdateCreditAsync(credit, cancellationToken);
        return ToDto(credit, await LoadContextAsync(cancellationToken));
    }

    public async Task<LessonCreditDto?> RevokeCreditAsync(
        Guid creditId,
        string? reason,
        Guid? actingUserId,
        CancellationToken cancellationToken)
    {
        var credit = await billingRepository.GetCreditAsync(creditId, cancellationToken);

        if (credit is null)
        {
            return null;
        }

        if (credit.Status == LessonCreditStatus.Used)
        {
            throw new ArgumentException("Wykorzystanego kredytu nie da się wycofać.");
        }

        credit.Status = LessonCreditStatus.Revoked;
        credit.UsedByUserId = actingUserId;
        credit.UsedAt = DateTimeOffset.UtcNow;
        credit.UsageNote = Shorten(reason, 1000);

        await billingRepository.UpdateCreditAsync(credit, cancellationToken);
        return ToDto(credit, await LoadContextAsync(cancellationToken));
    }

    /// <summary>Kredyty po terminie ważności przestawiamy przy odczycie - nie ma workera,
    /// a przeterminowany kredyt pokazywany jako „do wykorzystania" wprowadza w błąd.</summary>
    private async Task<IReadOnlyList<LessonCredit>> ExpireOutdatedCreditsAsync(CancellationToken cancellationToken)
    {
        var credits = await billingRepository.ListCreditsAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var credit in credits.Where(credit =>
            credit.Status == LessonCreditStatus.Available && credit.ExpiresAt is not null && credit.ExpiresAt < today))
        {
            credit.Status = LessonCreditStatus.Expired;
            await billingRepository.UpdateCreditAsync(credit, cancellationToken);
        }

        return credits;
    }

    private static LessonCreditDto ToDto(LessonCredit credit, BillingContext context) => new(
        credit.Id,
        credit.ParticipantId,
        context.ParticipantNames.GetValueOrDefault(credit.ParticipantId, "(nieznany)"),
        credit.GroupId,
        credit.GroupId is Guid groupId ? context.GroupNames.GetValueOrDefault(groupId) : null,
        credit.SourceSessionId,
        credit.Reason,
        credit.AmountCents,
        credit.Currency,
        credit.Status.Name(),
        credit.Status.Label(),
        credit.IssuedAt,
        credit.IssuedByUserId,
        credit.ExpiresAt,
        credit.Usage.Name(),
        credit.Usage.Label(),
        credit.UsedAt,
        credit.UsedForSessionId,
        credit.UsedForInvoiceId,
        credit.UsageNote);

    private static string? Shorten(string? value, int maxLength)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed[..Math.Min(trimmed.Length, maxLength)];
    }

    private async Task<BillingContext> LoadContextAsync(CancellationToken cancellationToken)
    {
        var participants = await participantRepository.ListAsync(cancellationToken);
        var groups = await groupRepository.ListAsync(cancellationToken);
        var courses = await courseRepository.ListAsync(cancellationToken);

        return new BillingContext(
            participants.ToDictionary(participant => participant.Id, participant => $"{participant.FirstName} {participant.LastName}"),
            groups.ToDictionary(group => group.Id, group => group.Name),
            groups.ToDictionary(group => group.Id, group => group.CourseId),
            courses.ToDictionary(course => course.Id, course => course.Name));
    }

    private static PricePlanDto ToDto(PricePlan plan, BillingContext context) =>
        new(
            plan.Id,
            plan.Name,
            plan.CourseId,
            plan.CourseId is Guid courseId ? context.CourseNames.GetValueOrDefault(courseId) : null,
            plan.GroupId,
            plan.GroupId is Guid groupId ? context.GroupNames.GetValueOrDefault(groupId) : null,
            plan.AmountCents,
            plan.Currency,
            plan.IsActive);

    private static BillingEnrollmentDto ToDto(
        BillingEnrollment enrollment,
        IReadOnlyList<PricePlan> pricePlans,
        BillingContext context)
    {
        var pricePlan = enrollment.PricePlanId is Guid pricePlanId
            ? pricePlans.FirstOrDefault(plan => plan.Id == pricePlanId)
            : pricePlans.FirstOrDefault(plan => plan.IsActive && plan.GroupId == enrollment.GroupId)
                ?? pricePlans.FirstOrDefault(plan => plan.IsActive && enrollment.CourseId is not null && plan.CourseId == enrollment.CourseId);

        return new BillingEnrollmentDto(
            enrollment.Id,
            enrollment.ParticipantId,
            context.ParticipantNames.GetValueOrDefault(enrollment.ParticipantId, "(nieznany)"),
            enrollment.GroupId,
            context.GroupNames.GetValueOrDefault(enrollment.GroupId, "(nieznana)"),
            enrollment.CourseId,
            enrollment.CourseId is Guid courseId ? context.CourseNames.GetValueOrDefault(courseId) : null,
            enrollment.PricePlanId,
            StatusName(enrollment.Status),
            StatusLabel(enrollment.Status),
            enrollment.TrialEndsAt,
            pricePlan?.AmountCents,
            pricePlan?.Currency);
    }

    private static InvoiceDto ToDto(Invoice invoice, BillingContext context) =>
        new(
            invoice.Id,
            invoice.BillingEnrollmentId,
            invoice.ParticipantId,
            context.ParticipantNames.GetValueOrDefault(invoice.ParticipantId, "(nieznany)"),
            invoice.GroupId,
            context.GroupNames.GetValueOrDefault(invoice.GroupId, "(nieznana)"),
            invoice.Number,
            invoice.AmountCents,
            invoice.Currency,
            StatusName(invoice.Status),
            StatusLabel(invoice.Status),
            invoice.DueDate,
            invoice.IssuedAt,
            invoice.PaidAt);

    private static PaymentDto ToDto(Payment payment) =>
        new(
            payment.Id,
            payment.InvoiceId,
            payment.Provider,
            payment.ExternalId,
            payment.AmountCents,
            payment.Currency,
            StatusName(payment.Status),
            StatusLabel(payment.Status),
            payment.PaidAt,
            payment.CreatedAt);

    private static bool IsOverdue(Invoice invoice) =>
        invoice.Status == InvoiceStatus.Open && invoice.DueDate < DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>Numer faktury w formacie FV/{rok}/{kolejny w tym roku}.
    /// Licznik jest per rok (a nie globalny), więc 1 stycznia numeracja startuje od nowa,
    /// zgodnie z zasadą ciągłej numeracji w obrębie okresu rozliczeniowego.
    /// Kolejny numer wyznaczamy z najwyższego już istniejącego w danym roku - dzięki temu
    /// usunięcie/anulowanie faktury nie powoduje ponownego użycia zajętego numeru.</summary>
    private async Task<string> NextInvoiceNumberAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"FV/{year}/";
        var invoices = await billingRepository.ListInvoicesAsync(cancellationToken);

        var highest = invoices
            .Select(invoice => invoice.Number)
            .Where(number => number.StartsWith(prefix, StringComparison.Ordinal))
            .Select(number => int.TryParse(number[prefix.Length..], out var parsed) ? parsed : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"{prefix}{highest + 1:D5}";
    }

    private static string NormalizeCurrency(string? currency)
    {
        var normalized = (currency ?? "PLN").Trim().ToUpperInvariant();
        return normalized.Length == 3 ? normalized : throw new ArgumentException("Waluta musi być kodem ISO, np. PLN.");
    }

    private static string StatusName(BillingEnrollmentStatus status) => status.ToString().ToLowerInvariant();
    private static string StatusName(InvoiceStatus status) => status.ToString().ToLowerInvariant();
    private static string StatusName(PaymentStatus status) => status.ToString().ToLowerInvariant();

    private static string StatusLabel(BillingEnrollmentStatus status) => status switch
    {
        BillingEnrollmentStatus.Trial => "Trial",
        BillingEnrollmentStatus.Active => "Aktywny",
        BillingEnrollmentStatus.Cancelled => "Anulowany",
        _ => status.ToString()
    };

    private static string StatusLabel(InvoiceStatus status) => status switch
    {
        InvoiceStatus.Draft => "Szkic",
        InvoiceStatus.Open => "Do zapłaty",
        InvoiceStatus.Paid => "Opłacona",
        InvoiceStatus.Overdue => "Zaległa",
        InvoiceStatus.Cancelled => "Anulowana",
        _ => status.ToString()
    };

    private static string StatusLabel(PaymentStatus status) => status switch
    {
        PaymentStatus.Pending => "Oczekuje",
        PaymentStatus.Succeeded => "Udana",
        PaymentStatus.Failed => "Nieudana",
        _ => status.ToString()
    };

    private sealed record BillingContext(
        IReadOnlyDictionary<Guid, string> ParticipantNames,
        IReadOnlyDictionary<Guid, string> GroupNames,
        IReadOnlyDictionary<Guid, Guid?> GroupCourseIds,
        IReadOnlyDictionary<Guid, string> CourseNames);
}

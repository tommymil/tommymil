using LessonRunner.Application.Billing;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Parents;
using LessonRunner.Application.Participants;
using LessonRunner.Application.Scheduling;
using LessonRunner.Domain.Billing;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Notifications;
using LessonRunner.Domain.Participants;

namespace LessonRunner.Application.Notifications;

public sealed class NotificationService(
    INotificationRepository notificationRepository,
    IEmailSender emailSender,
    IGroupRepository groupRepository,
    IParticipantRepository participantRepository,
    ILessonRepository lessonRepository,
    IGuardianDirectory? guardianDirectory = null,
    // Opcjonalne, żeby starsze testy budujące serwis ręcznie nadal się kompilowały.
    // Brak repozytorium oznacza brak przypomnień o płatnościach, a nie wywrócony serwis.
    IBillingRepository? billingRepository = null) : INotificationService
{
    public async Task<NotificationSettingsDto> GetSettingsAsync(CancellationToken cancellationToken) =>
        ToDto(await notificationRepository.GetSettingsAsync(cancellationToken));

    public async Task<NotificationSettingsDto> UpdateSettingsAsync(NotificationSettingsDto dto, CancellationToken cancellationToken)
    {
        var settings = new NotificationSettings
        {
            RemindersEnabled = dto.RemindersEnabled,
            AbsenceEnabled = dto.AbsenceEnabled,
            ReminderLeadHours = Math.Clamp(dto.ReminderLeadHours, 1, 168),
            FromName = Required(dto.FromName, "Nazwa nadawcy jest wymagana."),
            FromEmail = RequiredEmail(dto.FromEmail),
            ReminderSubject = Required(dto.ReminderSubject, "Temat przypomnienia jest wymagany."),
            ReminderBody = Required(dto.ReminderBody, "Treść przypomnienia jest wymagana."),
            AbsenceSubject = Required(dto.AbsenceSubject, "Temat nieobecności jest wymagany."),
            AbsenceBody = Required(dto.AbsenceBody, "Treść nieobecności jest wymagana.")
        };

        await notificationRepository.SaveSettingsAsync(settings, cancellationToken);
        return ToDto(settings);
    }

    public async Task<IReadOnlyList<NotificationLogDto>> ListLogsAsync(int limit, CancellationToken cancellationToken)
    {
        var logs = await notificationRepository.ListLogsAsync(Math.Clamp(limit <= 0 ? 100 : limit, 1, 500), cancellationToken);
        return logs.Select(ToDto).ToList();
    }

    public async Task<NotificationPreviewDto> PreviewAsync(string type, CancellationToken cancellationToken)
    {
        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);
        var context = new TemplateContext(
            "Jan Kowalski",
            "Scratch A",
            "Pierwsze kroki w Scratch",
            // Podgląd musi używać tego samego formatera co wysyłka. Wcześniej brał czas lokalny
            // serwera, więc w panelu wyglądał poprawnie także wtedy, gdy realny e-mail podawał UTC.
            SchoolTime.FormatDateTime(DateTimeOffset.UtcNow.AddDays(1)),
            "Opiekun",
            "https://meet.google.com/przyklad-linku");

        var normalized = (type ?? string.Empty).Trim().ToLowerInvariant();
        return normalized == "absence"
            ? new NotificationPreviewDto("absence", Render(settings.AbsenceSubject, context), Render(settings.AbsenceBody, context))
            : new NotificationPreviewDto("reminder", Render(settings.ReminderSubject, context), Render(settings.ReminderBody, context));
    }

    public async Task NotifyAbsencesAsync(Guid sessionId, IReadOnlyList<Guid> absentParticipantIds, CancellationToken cancellationToken)
    {
        if (absentParticipantIds.Count == 0)
        {
            return;
        }

        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);

        if (!settings.AbsenceEnabled)
        {
            return;
        }

        var group = await groupRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return;
        }

        var participants = await participantRepository.GetByIdsAsync(absentParticipantIds.Distinct().ToList(), cancellationToken);
        var lessonTitle = await LessonTitleAsync(session.LessonId, cancellationToken);

        var participantsById = participants.ToDictionary(participant => participant.Id);

        foreach (var contact in await ResolveGuardiansAsync(participants, cancellationToken))
        {
            var participant = participantsById[contact.ParticipantId];

            // Deduplikacja po adresie, nie po dziecku - przy dwojgu opiekunów każde ma dostać
            // swoją wiadomość, ale tylko jedną.
            var dedupeKey = DedupeKey("absence", sessionId, contact.ParticipantId, contact.Email);
            if (await notificationRepository.HasLogAsync(dedupeKey, cancellationToken))
            {
                continue;
            }

            var context = CreateContext(participant, group, session, lessonTitle, contact.Name);
            await SendLoggedAsync(
                "absence",
                dedupeKey,
                contact.Email,
                Render(settings.AbsenceSubject, context),
                Render(settings.AbsenceBody, context),
                settings,
                cancellationToken);
        }
    }

    public async Task SendUpcomingSessionRemindersAsync(CancellationToken cancellationToken)
    {
        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);

        if (!settings.RemindersEnabled)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var until = now.AddHours(settings.ReminderLeadHours);
        var groups = await groupRepository.ListAsync(cancellationToken);
        var lessonTitles = await lessonRepository.ListTitlesAsync(cancellationToken);

        foreach (var group in groups)
        {
            var participants = await participantRepository.GetByIdsAsync(
                group.Enrollments
                    .Where(enrollment => enrollment.Status == EnrollmentStatus.Enrolled)
                    .Select(enrollment => enrollment.ParticipantId)
                    .ToList(),
                cancellationToken);

            foreach (var session in group.Sessions.Where(session =>
                session.Status.IsUpcoming()
                && session.ScheduledAt >= now
                && session.ScheduledAt <= until))
            {
                var lessonTitle = session.LessonId is Guid lessonId && lessonTitles.TryGetValue(lessonId, out var title)
                    ? title
                    : "Zajęcia";

                var participantsById = participants.ToDictionary(participant => participant.Id);

                foreach (var contact in await ResolveGuardiansAsync(participants, cancellationToken))
                {
                    var dedupeKey = DedupeKey("reminder", session.Id, contact.ParticipantId, contact.Email);
                    if (await notificationRepository.HasLogAsync(dedupeKey, cancellationToken))
                    {
                        continue;
                    }

                    var context = CreateContext(
                        participantsById[contact.ParticipantId], group, session, lessonTitle, contact.Name);
                    await SendLoggedAsync(
                        "reminder",
                        dedupeKey,
                        contact.Email,
                        Render(settings.ReminderSubject, context),
                        Render(settings.ReminderBody, context),
                        settings,
                        cancellationToken);
                }
            }
        }
    }

    /// <summary>
    /// Zmiana terminu: przełożenie albo odwołanie.
    ///
    /// Rozdział 12 dokumentu koncepcyjnego wymienia „nie dostaliśmy informacji o zmianie”
    /// wśród typowych reklamacji, a dotąd znacznik „powiadomiono opiekunów” zaznaczało się
    /// w historii **ręcznie** — system prosił człowieka o zadeklarowanie faktu, którego sam
    /// nie sprawdzał, i zapisywał tę deklarację jako dowód.
    ///
    /// Zwracana liczba to liczba wiadomości, które faktycznie wyszły.
    /// </summary>
    public async Task<int> NotifySessionRescheduledAsync(
        Guid sessionId,
        DateTimeOffset? previousScheduledAt,
        string? reason,
        bool cancelled,
        CancellationToken cancellationToken)
    {
        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);

        // Przełożenie dzieli przełącznik z przypomnieniami: to ta sama kategoria wiadomości,
        // czyli informacja o tym, kiedy odbywają się zajęcia.
        //
        // **Odwołanie idzie zawsze**, niezależnie od ustawień. Wyłączenie przypomnień znaczy
        // „nie przypominaj mi co tydzień”, a nie „nie mów mi, że zajęcia się nie odbędą” —
        // rodzic, który przywiezie dziecko na odwołane zajęcia, ma pełne prawo do pretensji.
        // Ta sama zasada rządzi e-mailami o dostępie do konta.
        if (!cancelled && !settings.RemindersEnabled)
        {
            return 0;
        }

        var group = await groupRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null)
        {
            return 0;
        }

        var participants = await EnrolledParticipantsAsync(group, cancellationToken);
        var lessonTitle = await LessonTitleAsync(session.LessonId, cancellationToken);
        var participantsById = participants.ToDictionary(participant => participant.Id);
        var sent = 0;

        foreach (var contact in await ResolveGuardiansAsync(participants, cancellationToken))
        {
            // Klucz niesie moment zmiany, a nie sam termin: ten sam termin bywa przekładany
            // kilka razy, a rodzic ma dostać informację o każdej z tych zmian. `UtcTicks`
            // zamiast sformatowanej daty - równie jednoznaczne, o połowę krótsze.
            var dedupeKey = DedupeKey(
                cancelled ? "cancellation" : "reschedule",
                sessionId,
                session.ScheduledAt.UtcTicks,
                contact.Email);

            if (await notificationRepository.HasLogAsync(dedupeKey, cancellationToken))
            {
                continue;
            }

            var context = CreateContext(participantsById[contact.ParticipantId], group, session, lessonTitle, contact.Name)
                with
            {
                PreviousAt = previousScheduledAt is DateTimeOffset previous
                    ? SchoolTime.FormatDateTime(previous)
                    : "poprzedni termin",
                Reason = string.IsNullOrWhiteSpace(reason) ? "nie podano" : reason.Trim(),
            };

            var subject = cancelled ? NotificationTemplates.CancelSubject : NotificationTemplates.RescheduleSubject;
            var body = cancelled ? NotificationTemplates.CancelBody : NotificationTemplates.RescheduleBody;

            await SendLoggedAsync(
                cancelled ? "cancellation" : "reschedule",
                dedupeKey,
                contact.Email,
                Render(subject, context),
                Render(body, context),
                settings,
                cancellationToken);

            sent++;
        }

        return sent;
    }

    /// <summary>
    /// Podsumowanie zajęć.
    ///
    /// Wysyłamy wyłącznie `ScheduledSession.ParentSummary` — pole napisane z myślą o rodzicu.
    /// Notatka wewnętrzna terminu nie przechodzi tędy w ogóle: to była najprostsza droga,
    /// żeby uwaga dla zespołu wylądowała w skrzynce opiekuna.
    /// </summary>
    public async Task<int> NotifySessionSummaryAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);

        // Podsumowanie dzieli przełącznik z przypomnieniami: obie wiadomości to „informacja
        // o zajęciach”, obie są miłe, ale żadna nie jest krytyczna. Szkoła, która wyłączyła
        // pocztę o zajęciach, nie spodziewa się, że i tak coś wyjdzie.
        //
        // Osobny przełącznik wymaga kolumn w ustawieniach - patrz komentarz przy
        // `NotificationTemplates`.
        if (!settings.RemindersEnabled)
        {
            return 0;
        }

        var group = await groupRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        var session = group?.Sessions.FirstOrDefault(item => item.Id == sessionId);

        if (group is null || session is null || string.IsNullOrWhiteSpace(session.ParentSummary))
        {
            return 0;
        }

        var participants = await EnrolledParticipantsAsync(group, cancellationToken);
        var lessonTitle = await LessonTitleAsync(session.LessonId, cancellationToken);
        var participantsById = participants.ToDictionary(participant => participant.Id);
        var sent = 0;

        foreach (var contact in await ResolveGuardiansAsync(participants, cancellationToken))
        {
            var dedupeKey = DedupeKey("summary", sessionId, contact.Email);

            if (await notificationRepository.HasLogAsync(dedupeKey, cancellationToken))
            {
                continue;
            }

            var context = CreateContext(participantsById[contact.ParticipantId], group, session, lessonTitle, contact.Name)
                with
            {
                Summary = session.ParentSummary!.Trim(),
            };

            await SendLoggedAsync(
                "summary",
                dedupeKey,
                contact.Email,
                Render(NotificationTemplates.SummarySubject, context),
                Render(NotificationTemplates.SummaryBody, context),
                settings,
                cancellationToken);

            sent++;
        }

        return sent;
    }

    /// <summary>
    /// Przypomnienia o zaległych płatnościach.
    ///
    /// Świadomie **bez automatu i bez przełącznika w ustawieniach**: upominanie się
    /// o pieniądze to decyzja biznesowa i wizerunkowa. Uruchamia ją administrator, klikając
    /// przycisk — i widzi w odpowiedzi, ile wiadomości wyszło.
    ///
    /// Zaległość liczymy z daty, a nie ze statusu w bazie: status „Overdue” zmienia się
    /// dopiero przy jakiejś operacji na fakturze, więc dokument po terminie potrafiłby
    /// tygodniami udawać „Do zapłaty”.
    /// </summary>
    public async Task<int> SendPaymentRemindersAsync(CancellationToken cancellationToken)
    {
        if (billingRepository is null)
        {
            return 0;
        }

        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);

        var overdue = (await billingRepository.ListInvoicesAsync(cancellationToken))
            .Where(invoice => invoice.Status is InvoiceStatus.Open or InvoiceStatus.Overdue)
            .Where(invoice => invoice.DueDate < today)
            .ToList();

        if (overdue.Count == 0)
        {
            return 0;
        }

        var groups = await groupRepository.ListAsync(cancellationToken);
        var participants = await participantRepository.GetByIdsAsync(
            overdue.Select(invoice => invoice.ParticipantId).Distinct().ToList(),
            cancellationToken);
        var participantsById = participants.ToDictionary(participant => participant.Id);
        var contacts = await ResolveGuardiansAsync(participants, cancellationToken);
        var sent = 0;

        foreach (var invoice in overdue)
        {
            if (!participantsById.TryGetValue(invoice.ParticipantId, out var participant))
            {
                continue;
            }

            var groupName = groups.FirstOrDefault(group => group.Id == invoice.GroupId)?.Name ?? "zajęcia";

            foreach (var contact in contacts.Where(item => item.ParticipantId == invoice.ParticipantId))
            {
                // Klucz per faktura, nie per dziecko: druga zaległa faktura to druga sprawa.
                var dedupeKey = DedupeKey("payment", invoice.Id, contact.Email);

                if (await notificationRepository.HasLogAsync(dedupeKey, cancellationToken))
                {
                    continue;
                }

                var context = new TemplateContext(
                    $"{participant.FirstName} {participant.LastName}",
                    groupName,
                    "Zajęcia",
                    "",
                    contact.Name,
                    "")
                {
                    Amount = $"{invoice.AmountCents / 100m:0.00} {invoice.Currency}",
                    DueDate = invoice.DueDate.ToString("yyyy-MM-dd"),
                };

                await SendLoggedAsync(
                    "payment",
                    dedupeKey,
                    contact.Email,
                    Render(NotificationTemplates.PaymentSubject, context),
                    Render(NotificationTemplates.PaymentBody, context),
                    settings,
                    cancellationToken);

                sent++;
            }
        }

        return sent;
    }

    /// <summary>Dzieci aktywnie zapisane do grupy — adresaci wiadomości o jej terminach.</summary>
    private async Task<IReadOnlyList<Participant>> EnrolledParticipantsAsync(
        Group group,
        CancellationToken cancellationToken) =>
        await participantRepository.GetByIdsAsync(
            group.Enrollments
                .Where(enrollment => enrollment.Status == EnrollmentStatus.Enrolled)
                .Select(enrollment => enrollment.ParticipantId)
                .Distinct()
                .ToList(),
            cancellationToken);

    /// <summary>Ile znaków mieści kolumna `NotificationLogs.DedupeKey`.</summary>
    private const int DedupeKeyMaxLength = 220;

    /// <summary>
    /// Klucz deduplikacji o gwarantowanej długości.
    ///
    /// Klucz zawiera adres e-mail, a adres może mieć do 254 znaków — czyli więcej, niż mieści
    /// cała kolumna. Przy dłuższych adresach klucz był po cichu za długi: SQLite tego nie
    /// pilnuje, więc problem nie dawał znaku o sobie, ale na PostgreSQL zapis by się wywrócił,
    /// a przy obcięciu dwa różne adresy mogłyby dać ten sam klucz i **wygasić drugą wiadomość**.
    /// Dotyczyło to również kluczy sprzed 03.08.2026 (`absence`, `reminder`).
    ///
    /// Klucz zostaje czytelny w typowym przypadku; dopiero po przekroczeniu limitu ogon
    /// zastępuje deterministyczny skrót, żeby jednoznaczność była zachowana zawsze.
    /// </summary>
    private static string DedupeKey(string type, params object[] parts)
    {
        var key = $"{type}:{string.Join(':', parts)}";

        if (key.Length <= DedupeKeyMaxLength)
        {
            return key;
        }

        var digest = Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(key)));

        // Zostawiamy początek klucza (typ i identyfikator), żeby dziennik dało się czytać,
        // a resztę zastępujemy skrótem.
        return key[..(DedupeKeyMaxLength - 65)] + ":" + digest[..64];
    }

    private async Task SendLoggedAsync(
        string type,
        string dedupeKey,
        string recipient,
        string subject,
        string body,
        NotificationSettings settings,
        CancellationToken cancellationToken)
    {
        var log = new NotificationLog
        {
            Type = type,
            Channel = "email",
            Recipient = recipient,
            Subject = subject,
            Status = "pending",
            DedupeKey = dedupeKey
        };

        try
        {
            await emailSender.SendAsync(new EmailMessage(settings.FromEmail, settings.FromName, recipient, subject, body), cancellationToken);
            log.Status = "sent";
            log.SentAt = DateTimeOffset.UtcNow;
        }
        catch (Exception ex)
        {
            log.Status = "failed";
            log.Error = ex.Message;
        }

        await notificationRepository.AddLogAsync(log, cancellationToken);
    }

    private async Task<string> LessonTitleAsync(Guid? lessonId, CancellationToken cancellationToken)
    {
        if (lessonId is null)
        {
            return "Zajęcia";
        }

        var lesson = await lessonRepository.GetByIdAsync(lessonId.Value, cancellationToken);
        return lesson?.Title ?? "Zajęcia";
    }

    private static TemplateContext CreateContext(
        Participant participant,
        Group group,
        ScheduledSession session,
        string lessonTitle,
        string guardianName) =>
        new(
            $"{participant.FirstName} {participant.LastName}",
            group.Name,
            lessonTitle,
            SchoolTime.FormatDateTime(session.ScheduledAt),
            guardianName,
            // Link terminu wygrywa z linkiem grupy - tak samo jak w portalu rodzica.
            string.IsNullOrWhiteSpace(session.MeetingUrl) ? group.MeetingUrl ?? "" : session.MeetingUrl);

    /// <summary>
    /// Kto ma dostać wiadomość w sprawie tych dzieci.
    ///
    /// Docelowo rozstrzyga to `GuardianDirectory` (konta opiekunów, z zapasem w danych przy
    /// dziecku). Gdy nie został wstrzyknięty - w starszych testach budujących serwis ręcznie -
    /// wracamy do samych danych przy dziecku, żeby zachowanie pozostało to samo.
    /// </summary>
    private async Task<IReadOnlyList<GuardianContact>> ResolveGuardiansAsync(
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken)
    {
        if (guardianDirectory is not null)
        {
            return await guardianDirectory.ResolveAsync(participants, cancellationToken);
        }

        return participants
            .Where(participant => participant.DataProcessingConsentAt is not null
                && !string.IsNullOrWhiteSpace(participant.GuardianEmail)
                && participant.GuardianEmail.Contains('@'))
            .Select(participant => new GuardianContact(
                participant.Id,
                participant.GuardianEmail!,
                participant.GuardianName ?? "Opiekun",
                participant.GuardianRelation,
                IsPrimaryContact: true,
                FromAccount: false))
            .ToList();
    }

    private static string Render(string template, TemplateContext context) =>
        template
            .Replace("{{participant}}", context.Participant, StringComparison.OrdinalIgnoreCase)
            .Replace("{{group}}", context.Group, StringComparison.OrdinalIgnoreCase)
            .Replace("{{lesson}}", context.Lesson, StringComparison.OrdinalIgnoreCase)
            .Replace("{{sessionAt}}", context.SessionAt, StringComparison.OrdinalIgnoreCase)
            .Replace("{{guardian}}", context.Guardian, StringComparison.OrdinalIgnoreCase)
            .Replace("{{link}}", context.Link, StringComparison.OrdinalIgnoreCase)
            .Replace("{{previousAt}}", context.PreviousAt, StringComparison.OrdinalIgnoreCase)
            .Replace("{{reason}}", context.Reason, StringComparison.OrdinalIgnoreCase)
            .Replace("{{summary}}", context.Summary, StringComparison.OrdinalIgnoreCase)
            .Replace("{{amount}}", context.Amount, StringComparison.OrdinalIgnoreCase)
            .Replace("{{dueDate}}", context.DueDate, StringComparison.OrdinalIgnoreCase);

    private static NotificationSettingsDto ToDto(NotificationSettings settings) =>
        new(
            settings.RemindersEnabled,
            settings.AbsenceEnabled,
            settings.ReminderLeadHours,
            settings.FromName,
            settings.FromEmail,
            settings.ReminderSubject,
            settings.ReminderBody,
            settings.AbsenceSubject,
            settings.AbsenceBody);

    private static NotificationLogDto ToDto(NotificationLog log) =>
        new(log.Id, log.CreatedAt, log.SentAt, log.Type, log.Channel, log.Recipient, log.Subject, log.Status, log.DedupeKey, log.Error);

    private static string Required(string? value, string error)
    {
        var normalized = (value ?? string.Empty).Trim();
        return normalized.Length == 0 ? throw new ArgumentException(error) : normalized;
    }

    private static string RequiredEmail(string? value)
    {
        var normalized = Required(value, "Adres e-mail nadawcy jest wymagany.");
        return normalized.Contains('@') ? normalized : throw new ArgumentException("Adres e-mail nadawcy jest nieprawidłowy.");
    }

    /// <summary>
    /// Kontekst szablonu.
    ///
    /// Pola pozycyjne występują we wszystkich typach wiadomości; te z inicjalizatorem
    /// wyłącznie w części z nich. Nieużyta zmienna renderuje się jako pusty łańcuch,
    /// a nie jako surowe `{{amount}}` w mailu do rodzica.
    /// </summary>
    private sealed record TemplateContext(
        string Participant,
        string Group,
        string Lesson,
        string SessionAt,
        string Guardian,
        /// <summary>Link do spotkania - najczęstsze pytanie rodzica przed zajęciami.</summary>
        string Link)
    {
        /// <summary>Poprzedni termin - tylko w wiadomości o przełożeniu zajęć.</summary>
        public string PreviousAt { get; init; } = "";

        /// <summary>Powód zmiany albo odwołania.</summary>
        public string Reason { get; init; } = "";

        /// <summary>Podsumowanie zajęć napisane przez instruktora dla rodzica.</summary>
        public string Summary { get; init; } = "";

        /// <summary>Kwota i waluta - tylko w przypomnieniu o płatności.</summary>
        public string Amount { get; init; } = "";

        public string DueDate { get; init; } = "";
    }
}

using LessonRunner.Application.Billing;
using LessonRunner.Domain.Billing;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Billing;

internal sealed class EfBillingRepository(AppDbContext dbContext) : IBillingRepository
{
    public async Task<IReadOnlyList<PricePlan>> ListPricePlansAsync(CancellationToken cancellationToken) =>
        (await dbContext.PricePlans.AsNoTracking().OrderBy(plan => plan.Name).ToListAsync(cancellationToken))
            .Select(ToDomain)
            .ToList();

    public async Task<PricePlan?> GetPricePlanAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.PricePlans.AsNoTracking().FirstOrDefaultAsync(plan => plan.Id == id, cancellationToken);
        return document is null ? null : ToDomain(document);
    }

    public async Task AddPricePlanAsync(PricePlan pricePlan, CancellationToken cancellationToken)
    {
        dbContext.PricePlans.Add(ToDocument(pricePlan));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdatePricePlanAsync(PricePlan pricePlan, CancellationToken cancellationToken)
    {
        var document = await dbContext.PricePlans.FirstOrDefaultAsync(plan => plan.Id == pricePlan.Id, cancellationToken);
        if (document is null)
        {
            return false;
        }

        document.Name = pricePlan.Name;
        document.CourseId = pricePlan.CourseId;
        document.GroupId = pricePlan.GroupId;
        document.AmountCents = pricePlan.AmountCents;
        document.Currency = pricePlan.Currency;
        document.IsActive = pricePlan.IsActive;
        document.UpdatedAt = pricePlan.UpdatedAt;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeletePricePlanAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.PricePlans.FirstOrDefaultAsync(plan => plan.Id == id, cancellationToken);
        if (document is null)
        {
            return false;
        }

        dbContext.PricePlans.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<BillingEnrollment>> ListEnrollmentsAsync(CancellationToken cancellationToken) =>
        // SQLite nie sortuje po DateTimeOffset w SQL - materializujemy i porządkujemy po stronie klienta.
        (await dbContext.BillingEnrollments.AsNoTracking().ToListAsync(cancellationToken))
            .OrderBy(enrollment => enrollment.CreatedAt)
            .Select(ToDomain)
            .ToList();

    public async Task<BillingEnrollment?> GetEnrollmentAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.BillingEnrollments.AsNoTracking().FirstOrDefaultAsync(enrollment => enrollment.Id == id, cancellationToken);
        return document is null ? null : ToDomain(document);
    }

    public async Task AddEnrollmentAsync(BillingEnrollment enrollment, CancellationToken cancellationToken)
    {
        dbContext.BillingEnrollments.Add(ToDocument(enrollment));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateEnrollmentAsync(BillingEnrollment enrollment, CancellationToken cancellationToken)
    {
        var document = await dbContext.BillingEnrollments.FirstOrDefaultAsync(item => item.Id == enrollment.Id, cancellationToken);
        if (document is null)
        {
            return false;
        }

        document.PricePlanId = enrollment.PricePlanId;
        document.Status = enrollment.Status.ToString();
        document.TrialEndsAt = enrollment.TrialEndsAt;
        document.UpdatedAt = enrollment.UpdatedAt;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Invoice>> ListInvoicesAsync(CancellationToken cancellationToken) =>
        (await dbContext.Invoices.AsNoTracking().ToListAsync(cancellationToken))
            .OrderByDescending(invoice => invoice.IssuedAt)
            .Select(ToDomain)
            .ToList();

    public async Task<Invoice?> GetInvoiceAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Invoices.AsNoTracking().FirstOrDefaultAsync(invoice => invoice.Id == id, cancellationToken);
        return document is null ? null : ToDomain(document);
    }

    public async Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        dbContext.Invoices.Add(ToDocument(invoice));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        var document = await dbContext.Invoices.FirstOrDefaultAsync(item => item.Id == invoice.Id, cancellationToken);
        if (document is null)
        {
            return false;
        }

        document.Status = invoice.Status.ToString();
        document.PaidAt = invoice.PaidAt;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Payment>> ListPaymentsAsync(CancellationToken cancellationToken) =>
        (await dbContext.Payments.AsNoTracking().ToListAsync(cancellationToken))
            .OrderByDescending(payment => payment.CreatedAt)
            .Select(ToDomain)
            .ToList();

    public async Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        dbContext.Payments.Add(ToDocument(payment));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static PricePlan ToDomain(PricePlanDocument document) => new()
    {
        Id = document.Id,
        Name = document.Name,
        CourseId = document.CourseId,
        GroupId = document.GroupId,
        AmountCents = document.AmountCents,
        Currency = document.Currency,
        IsActive = document.IsActive,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt
    };

    private static BillingEnrollment ToDomain(BillingEnrollmentDocument document) => new()
    {
        Id = document.Id,
        ParticipantId = document.ParticipantId,
        GroupId = document.GroupId,
        CourseId = document.CourseId,
        PricePlanId = document.PricePlanId,
        Status = ParseEnum(document.Status, BillingEnrollmentStatus.Active),
        TrialEndsAt = document.TrialEndsAt,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt
    };

    private static Invoice ToDomain(InvoiceDocument document) => new()
    {
        Id = document.Id,
        BillingEnrollmentId = document.BillingEnrollmentId,
        ParticipantId = document.ParticipantId,
        GroupId = document.GroupId,
        CourseId = document.CourseId,
        Number = document.Number,
        AmountCents = document.AmountCents,
        Currency = document.Currency,
        Status = ParseEnum(document.Status, InvoiceStatus.Open),
        DueDate = document.DueDate,
        IssuedAt = document.IssuedAt,
        PaidAt = document.PaidAt
    };

    private static Payment ToDomain(PaymentDocument document) => new()
    {
        Id = document.Id,
        InvoiceId = document.InvoiceId,
        Provider = document.Provider,
        ExternalId = document.ExternalId,
        AmountCents = document.AmountCents,
        Currency = document.Currency,
        Status = ParseEnum(document.Status, PaymentStatus.Pending),
        PaidAt = document.PaidAt,
        CreatedAt = document.CreatedAt
    };

    private static PricePlanDocument ToDocument(PricePlan pricePlan) => new()
    {
        Id = pricePlan.Id,
        Name = pricePlan.Name,
        CourseId = pricePlan.CourseId,
        GroupId = pricePlan.GroupId,
        AmountCents = pricePlan.AmountCents,
        Currency = pricePlan.Currency,
        IsActive = pricePlan.IsActive,
        CreatedAt = pricePlan.CreatedAt,
        UpdatedAt = pricePlan.UpdatedAt
    };

    private static BillingEnrollmentDocument ToDocument(BillingEnrollment enrollment) => new()
    {
        Id = enrollment.Id,
        ParticipantId = enrollment.ParticipantId,
        GroupId = enrollment.GroupId,
        CourseId = enrollment.CourseId,
        PricePlanId = enrollment.PricePlanId,
        Status = enrollment.Status.ToString(),
        TrialEndsAt = enrollment.TrialEndsAt,
        CreatedAt = enrollment.CreatedAt,
        UpdatedAt = enrollment.UpdatedAt
    };

    private static InvoiceDocument ToDocument(Invoice invoice) => new()
    {
        Id = invoice.Id,
        BillingEnrollmentId = invoice.BillingEnrollmentId,
        ParticipantId = invoice.ParticipantId,
        GroupId = invoice.GroupId,
        CourseId = invoice.CourseId,
        Number = invoice.Number,
        AmountCents = invoice.AmountCents,
        Currency = invoice.Currency,
        Status = invoice.Status.ToString(),
        DueDate = invoice.DueDate,
        IssuedAt = invoice.IssuedAt,
        PaidAt = invoice.PaidAt
    };

    private static PaymentDocument ToDocument(Payment payment) => new()
    {
        Id = payment.Id,
        InvoiceId = payment.InvoiceId,
        Provider = payment.Provider,
        ExternalId = payment.ExternalId,
        AmountCents = payment.AmountCents,
        Currency = payment.Currency,
        Status = payment.Status.ToString(),
        PaidAt = payment.PaidAt,
        CreatedAt = payment.CreatedAt
    };

    private static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct =>
        Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : fallback;

    public async Task<IReadOnlyList<LessonCredit>> ListCreditsAsync(CancellationToken cancellationToken)
    {
        var documents = await dbContext.LessonCredits
            .AsNoTracking()
            .OrderByDescending(credit => credit.IssuedAt)
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<LessonCredit?> GetCreditAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.LessonCredits.AsNoTracking().FirstOrDefaultAsync(credit => credit.Id == id, cancellationToken);
        return document is null ? null : ToDomain(document);
    }

    public async Task AddCreditAsync(LessonCredit credit, CancellationToken cancellationToken)
    {
        dbContext.LessonCredits.Add(ToDocument(credit));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateCreditAsync(LessonCredit credit, CancellationToken cancellationToken)
    {
        var document = await dbContext.LessonCredits.FirstOrDefaultAsync(item => item.Id == credit.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Status = credit.Status.ToString();
        document.Usage = credit.Usage.ToString();
        document.UsedAt = credit.UsedAt;
        document.UsedByUserId = credit.UsedByUserId;
        document.UsedForSessionId = credit.UsedForSessionId;
        document.UsedForInvoiceId = credit.UsedForInvoiceId;
        document.UsageNote = credit.UsageNote;
        document.ExpiresAt = credit.ExpiresAt;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static LessonCredit ToDomain(LessonCreditDocument document) => new()
    {
        Id = document.Id,
        ParticipantId = document.ParticipantId,
        GroupId = document.GroupId,
        SourceSessionId = document.SourceSessionId,
        Reason = document.Reason,
        AmountCents = document.AmountCents,
        Currency = document.Currency,
        Status = ParseEnum(document.Status, LessonCreditStatus.Available),
        IssuedAt = document.IssuedAt,
        IssuedByUserId = document.IssuedByUserId,
        ExpiresAt = document.ExpiresAt,
        Usage = ParseEnum(document.Usage, LessonCreditUsage.None),
        UsedAt = document.UsedAt,
        UsedByUserId = document.UsedByUserId,
        UsedForSessionId = document.UsedForSessionId,
        UsedForInvoiceId = document.UsedForInvoiceId,
        UsageNote = document.UsageNote
    };

    private static LessonCreditDocument ToDocument(LessonCredit credit) => new()
    {
        Id = credit.Id,
        ParticipantId = credit.ParticipantId,
        GroupId = credit.GroupId,
        SourceSessionId = credit.SourceSessionId,
        Reason = credit.Reason,
        AmountCents = credit.AmountCents,
        Currency = credit.Currency,
        Status = credit.Status.ToString(),
        IssuedAt = credit.IssuedAt,
        IssuedByUserId = credit.IssuedByUserId,
        ExpiresAt = credit.ExpiresAt,
        Usage = credit.Usage.ToString(),
        UsedAt = credit.UsedAt,
        UsedByUserId = credit.UsedByUserId,
        UsedForSessionId = credit.UsedForSessionId,
        UsedForInvoiceId = credit.UsedForInvoiceId,
        UsageNote = credit.UsageNote
    };
}

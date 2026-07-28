namespace LessonRunner.Application.Billing;

public sealed record PricePlanDto(
    Guid Id,
    string Name,
    Guid? CourseId,
    string? CourseName,
    Guid? GroupId,
    string? GroupName,
    long AmountCents,
    string Currency,
    bool IsActive);

public sealed record UpsertPricePlanDto(
    string Name,
    Guid? CourseId,
    Guid? GroupId,
    long AmountCents,
    string? Currency,
    bool IsActive = true);

public sealed record BillingEnrollmentDto(
    Guid Id,
    Guid ParticipantId,
    string ParticipantName,
    Guid GroupId,
    string GroupName,
    Guid? CourseId,
    string? CourseName,
    Guid? PricePlanId,
    string Status,
    string StatusLabel,
    DateOnly? TrialEndsAt,
    long? CurrentPriceCents,
    string? Currency);

public sealed record CreateBillingEnrollmentDto(
    Guid ParticipantId,
    Guid GroupId,
    Guid? PricePlanId,
    bool Trial = false,
    DateOnly? TrialEndsAt = null);

public sealed record InvoiceDto(
    Guid Id,
    Guid BillingEnrollmentId,
    Guid ParticipantId,
    string ParticipantName,
    Guid GroupId,
    string GroupName,
    string Number,
    long AmountCents,
    string Currency,
    string Status,
    string StatusLabel,
    DateOnly DueDate,
    DateTimeOffset IssuedAt,
    DateTimeOffset? PaidAt);

public sealed record CreateInvoiceDto(
    Guid BillingEnrollmentId,
    DateOnly? DueDate,
    long? AmountCents = null);

public sealed record PaymentDto(
    Guid Id,
    Guid InvoiceId,
    string Provider,
    string? ExternalId,
    long AmountCents,
    string Currency,
    string Status,
    string StatusLabel,
    DateTimeOffset? PaidAt,
    DateTimeOffset CreatedAt);

public sealed record RecordManualPaymentDto(
    long? AmountCents = null,
    DateTimeOffset? PaidAt = null,
    string? ExternalId = null);

public sealed record LessonCreditDto(
    Guid Id,
    Guid ParticipantId,
    string ParticipantName,
    Guid? GroupId,
    string? GroupName,
    Guid? SourceSessionId,
    string Reason,
    long? AmountCents,
    string? Currency,
    string Status,
    string StatusLabel,
    DateTimeOffset IssuedAt,
    Guid? IssuedByUserId,
    DateOnly? ExpiresAt,
    string Usage,
    string UsageLabel,
    DateTimeOffset? UsedAt,
    Guid? UsedForSessionId,
    Guid? UsedForInvoiceId,
    string? UsageNote);

/// <summary>Przyznanie kredytu. Powód jest wymagany - bez niego kredyt jest nie do obronienia
/// przy rozmowie z rodzicem ani przy kontroli.</summary>
public sealed record IssueLessonCreditDto(
    Guid ParticipantId,
    string Reason,
    Guid? GroupId = null,
    Guid? SourceSessionId = null,
    long? AmountCents = null,
    string? Currency = null,
    DateOnly? ExpiresAt = null);

public sealed record RevokeLessonCreditDto(string? Reason = null);

/// <summary>Wykorzystanie kredytu: na co i przy jakim terminie albo fakturze.</summary>
public sealed record UseLessonCreditDto(
    string Usage,
    Guid? UsedForSessionId = null,
    Guid? UsedForInvoiceId = null,
    string? UsageNote = null);

public sealed record BillingOverviewDto(
    IReadOnlyList<PricePlanDto> PricePlans,
    IReadOnlyList<BillingEnrollmentDto> Enrollments,
    IReadOnlyList<InvoiceDto> Invoices,
    IReadOnlyList<PaymentDto> Payments,
    BillingKpiDto Kpis,
    IReadOnlyList<LessonCreditDto> Credits);

public sealed record BillingKpiDto(
    long OpenAmountCents,
    long OverdueAmountCents,
    long PaidAmountCents,
    int TrialEnrollments,
    int ActiveEnrollments,
    /// <summary>Kredyty czekające na wykorzystanie - zobowiązanie wobec rodziców.</summary>
    int AvailableCredits = 0);

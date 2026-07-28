namespace LessonRunner.Infrastructure.Billing;

internal sealed class PricePlanDocument
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? GroupId { get; set; }
    public long AmountCents { get; set; }
    public required string Currency { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

internal sealed class BillingEnrollmentDocument
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid GroupId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? PricePlanId { get; set; }
    public required string Status { get; set; }
    public DateOnly? TrialEndsAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

internal sealed class InvoiceDocument
{
    public Guid Id { get; set; }
    public Guid BillingEnrollmentId { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid GroupId { get; set; }
    public Guid? CourseId { get; set; }
    public required string Number { get; set; }
    public long AmountCents { get; set; }
    public required string Currency { get; set; }
    public required string Status { get; set; }
    public DateOnly DueDate { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
}

internal sealed class PaymentDocument
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public required string Provider { get; set; }
    public string? ExternalId { get; set; }
    public long AmountCents { get; set; }
    public required string Currency { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

internal sealed class LessonCreditDocument
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? SourceSessionId { get; set; }
    public required string Reason { get; set; }
    public long? AmountCents { get; set; }
    public string? Currency { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public Guid? IssuedByUserId { get; set; }
    public DateOnly? ExpiresAt { get; set; }
    public required string Usage { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public Guid? UsedByUserId { get; set; }
    public Guid? UsedForSessionId { get; set; }
    public Guid? UsedForInvoiceId { get; set; }
    public string? UsageNote { get; set; }
}

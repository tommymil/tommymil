using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Billing;

public sealed class Payment : Entity
{
    public Guid InvoiceId { get; set; }
    public required string Provider { get; set; }
    public string? ExternalId { get; set; }
    public long AmountCents { get; set; }
    public required string Currency { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}

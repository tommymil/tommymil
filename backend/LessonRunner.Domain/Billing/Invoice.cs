using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Billing;

public sealed class Invoice : Entity
{
    public Guid BillingEnrollmentId { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid GroupId { get; set; }
    public Guid? CourseId { get; set; }
    public required string Number { get; set; }
    public long AmountCents { get; set; }
    public required string Currency { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Open;
    public DateOnly DueDate { get; set; }
    public DateTimeOffset IssuedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? PaidAt { get; set; }
}

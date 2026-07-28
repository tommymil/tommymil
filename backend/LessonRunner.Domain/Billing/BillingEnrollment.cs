using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Billing;

public sealed class BillingEnrollment : Entity
{
    public Guid ParticipantId { get; set; }
    public Guid GroupId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? PricePlanId { get; set; }
    public BillingEnrollmentStatus Status { get; set; } = BillingEnrollmentStatus.Active;
    public DateOnly? TrialEndsAt { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

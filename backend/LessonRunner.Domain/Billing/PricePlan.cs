using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Billing;

public sealed class PricePlan : Entity
{
    public required string Name { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? GroupId { get; set; }
    public long AmountCents { get; set; }
    public required string Currency { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

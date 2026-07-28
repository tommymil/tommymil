using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Audit;

public sealed class AuditLog : Entity
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid? ActorUserId { get; set; }
    public required string Action { get; set; }
    public required string EntityType { get; set; }
    public string? EntityId { get; set; }
    public bool Success { get; set; } = true;
    public string? Details { get; set; }
}

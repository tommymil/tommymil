namespace LessonRunner.Infrastructure.Audit;

internal sealed class AuditLogDocument
{
    public Guid Id { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public Guid? ActorUserId { get; set; }
    public required string Action { get; set; }
    public required string EntityType { get; set; }
    public string? EntityId { get; set; }
    public bool Success { get; set; }
    public string? Details { get; set; }
}

namespace LessonRunner.Infrastructure.Parents;

internal sealed class ParentParticipantLinkDocument
{
    public Guid Id { get; set; }
    public Guid ParentUserId { get; set; }
    public Guid ParticipantId { get; set; }
    public string? Relation { get; set; }
    public bool IsPrimaryContact { get; set; }
    public bool ReceivesNotifications { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
}

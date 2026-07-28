namespace LessonRunner.Infrastructure.Parents;

internal sealed class ParentParticipantLinkDocument
{
    public Guid Id { get; set; }
    public Guid ParentUserId { get; set; }
    public Guid ParticipantId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

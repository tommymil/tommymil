using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Parents;

public sealed class ParentParticipantLink : Entity
{
    public Guid ParentUserId { get; set; }
    public Guid ParticipantId { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}

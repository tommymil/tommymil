using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Groups;

/// <summary>Przypisanie uczestnika (Domain.Participants.Participant) do grupy - relacja wiele-do-wielu.</summary>
public sealed class GroupEnrollment : Entity
{
    public Guid GroupId { get; set; }
    public Guid ParticipantId { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;
    public DateTimeOffset EnrolledAt { get; init; } = DateTimeOffset.UtcNow;
}

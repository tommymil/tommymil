using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Groups;

public sealed class Group : Entity
{
    public required string Name { get; set; }
    public Guid InstructorId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? LocationId { get; set; }
    public int? Capacity { get; set; }

    /// <summary>Stały link do spotkania online (Zoom/Meet) dzielony przez wszystkie terminy grupy.</summary>
    public string? MeetingUrl { get; set; }
    public GroupStatus Status { get; set; } = GroupStatus.Active;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<GroupEnrollment> Enrollments { get; set; } = [];
    public List<ScheduledSession> Sessions { get; set; } = [];
}

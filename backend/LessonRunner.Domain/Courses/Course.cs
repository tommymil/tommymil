using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Courses;

public sealed class Course : Entity
{
    public required string Name { get; set; }
    public required string Subject { get; set; }
    public required string Level { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<CourseLesson> Lessons { get; set; } = [];
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

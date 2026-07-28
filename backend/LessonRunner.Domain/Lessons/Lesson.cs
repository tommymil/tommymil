using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Lessons;

public sealed class Lesson : Entity
{
    public required string Title { get; set; }
    public required string Subject { get; set; }
    public required string Level { get; set; }
    public required string Description { get; set; }
    public int Order { get; set; }
    public LessonStatus Status { get; set; } = LessonStatus.Draft;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<string> Tags { get; set; } = [];
    public List<LessonStep> Steps { get; set; } = [];
    public LessonProjectFiles ProjectFiles { get; set; } = new();
}

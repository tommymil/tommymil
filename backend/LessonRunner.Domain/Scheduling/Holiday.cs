using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Scheduling;

public sealed class Holiday : Entity
{
    public DateOnly Date { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}

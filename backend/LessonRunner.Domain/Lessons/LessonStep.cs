using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Lessons;

public sealed class LessonStep : Entity
{
    public int Order { get; set; }
    public LessonStepType Type { get; set; }
    public required string Title { get; set; }
    public int DurationMinutes { get; set; }
    public List<string> Script { get; set; } = [];
    public List<StudentItem> StudentItems { get; set; } = [];
    public List<LessonResource> Resources { get; set; } = [];
    public List<LessonNote> Notes { get; set; } = [];
}

using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Lessons;

public sealed class LessonResource : Entity
{
    public LessonResourceKind Kind { get; set; }
    public string? Label { get; set; }
    public string? Url { get; set; }
    public string? Code { get; set; }
    public string? Language { get; set; }
}

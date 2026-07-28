using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Lessons;

public sealed class StudentItem : Entity
{
    public StudentItemKind Kind { get; set; }
    public string? Text { get; set; }
    public string? Caption { get; set; }
    public string? Url { get; set; }
}

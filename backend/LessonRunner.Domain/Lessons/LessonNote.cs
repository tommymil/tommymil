using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Lessons;

public sealed class LessonNote : Entity
{
    public LessonNoteKind Kind { get; set; }
    public required string Text { get; set; }
}

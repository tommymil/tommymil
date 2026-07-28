using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Courses;

public sealed class CourseLesson : Entity
{
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public int Order { get; set; }
}

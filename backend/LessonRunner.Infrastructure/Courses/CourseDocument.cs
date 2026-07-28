namespace LessonRunner.Infrastructure.Courses;

internal sealed class CourseDocument
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Subject { get; set; }
    public required string Level { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<CourseLessonDocument> Lessons { get; set; } = [];
}

internal sealed class CourseLessonDocument
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public int Order { get; set; }
}

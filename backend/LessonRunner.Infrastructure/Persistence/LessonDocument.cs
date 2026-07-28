namespace LessonRunner.Infrastructure.Persistence;

internal sealed class LessonDocument
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Subject { get; set; }
    public required string Status { get; set; }
    public required string DocumentJson { get; set; }
}

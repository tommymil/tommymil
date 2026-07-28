namespace LessonRunner.Infrastructure.Scheduling;

internal sealed class LocationDocument
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

internal sealed class HolidayDocument
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

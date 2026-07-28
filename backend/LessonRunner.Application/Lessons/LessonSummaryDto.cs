namespace LessonRunner.Application.Lessons;

public sealed record LessonSummaryDto(
    Guid Id,
    string Title,
    string Subject,
    string Level,
    string Description,
    int Order,
    string Status,
    string StatusLabel,
    int StepCount,
    int DurationMinutes);

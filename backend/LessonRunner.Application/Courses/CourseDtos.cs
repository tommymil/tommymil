namespace LessonRunner.Application.Courses;

public sealed record CourseLessonDto(
    Guid LessonId,
    string LessonTitle,
    string Subject,
    string Level,
    int Order);

public sealed record CourseSummaryDto(
    Guid Id,
    string Name,
    string Subject,
    string Level,
    string Description,
    int LessonCount);

public sealed record CourseDetailsDto(
    Guid Id,
    string Name,
    string Subject,
    string Level,
    string Description,
    IReadOnlyList<CourseLessonDto> Lessons);

public sealed record UpsertCourseDto(
    string Name,
    string Subject,
    string Level,
    string Description,
    IReadOnlyList<Guid> LessonIds);

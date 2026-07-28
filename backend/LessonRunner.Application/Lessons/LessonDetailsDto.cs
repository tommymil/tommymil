namespace LessonRunner.Application.Lessons;

public sealed record LessonDetailsDto(
    Guid Id,
    string Title,
    string Subject,
    string Level,
    string Description,
    int Order,
    string Status,
    string StatusLabel,
    int StepCount,
    int DurationMinutes,
    IReadOnlyList<string> Tags,
    LessonProjectFilesDto ProjectFiles,
    IReadOnlyList<LessonStepDto> Steps);

public sealed record LessonProjectFilesDto(
    LessonProjectFileDto? Starter,
    LessonProjectFileDto? Final);

public sealed record LessonProjectFileDto(
    string Label,
    string Url,
    string FileName,
    string ContentType,
    long SizeBytes,
    string DownloadToken,
    string DownloadUrl);

public sealed record LessonStepDto(
    Guid Id,
    int Order,
    string Type,
    string Title,
    int DurationMinutes,
    IReadOnlyList<string> Script,
    IReadOnlyList<StudentItemDto> StudentItems,
    IReadOnlyList<LessonResourceDto> Resources,
    IReadOnlyList<LessonNoteDto> Notes);

public sealed record StudentItemDto(
    string Kind,
    string? Text,
    string? Caption,
    string? Url);

public sealed record LessonResourceDto(
    string Kind,
    string? Label,
    string? Url,
    string? Code,
    string? Language);

public sealed record LessonNoteDto(
    string Kind,
    string Text);

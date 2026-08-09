namespace LessonRunner.Application.Progress;

public sealed record AutonomyOptionDto(string Value, string Label, int Rank);

public sealed record ProgressEntryDto(
    Guid Id,
    Guid ParticipantId,
    Guid? SessionId,
    Guid? GroupId,
    Guid? LessonId,
    string Autonomy,
    string AutonomyLabel,
    int AutonomyRank,
    bool LessonCompleted,
    string? NoteForParent,
    string? NextStep,
    DateTimeOffset UpdatedAt);

public sealed record SaveProgressEntryDto(
    Guid ParticipantId,
    string Autonomy,
    bool LessonCompleted = false,
    string? NoteForParent = null,
    string? NextStep = null);

/// <summary>Zbiorczy zapis z kokpitu: cała lista dzieci na terminie za jednym razem.</summary>
public sealed record SaveSessionProgressDto(IReadOnlyList<SaveProgressEntryDto> Entries);

public sealed record SessionProgressDto(
    Guid SessionId,
    IReadOnlyList<ProgressEntryDto> Entries,
    IReadOnlyList<AutonomyOptionDto> AutonomyOptions);

public sealed record ProjectSubmissionDto(
    Guid Id,
    int Version,
    string? Url,
    string? FileName,
    long? SizeBytes,
    string? DownloadUrl,
    DateTimeOffset SubmittedAt,
    string? InstructorComment);

public sealed record ProjectDto(
    Guid Id,
    Guid ParticipantId,
    string Title,
    string? Description,
    Guid? GroupId,
    DateTimeOffset CreatedAt,
    IReadOnlyList<ProjectSubmissionDto> Submissions);

public sealed record CreateProjectDto(
    Guid ParticipantId,
    string Title,
    string? Description = null,
    Guid? GroupId = null,
    Guid? LessonId = null);

/// <summary>Nowa wersja projektu: link **albo** plik. Podanie obu jest błędem — wtedy nie
/// wiadomo, co jest właściwą wersją.</summary>
public sealed record AddSubmissionDto(
    string? Url = null,
    string? FileUrl = null,
    string? FileName = null,
    string? ContentType = null,
    long? SizeBytes = null,
    string? InstructorComment = null);

public sealed record SetSubmissionCommentDto(string? Comment);

/// <summary>Dorobek jednego dziecka: wpisy o postępach i projekty.</summary>
public sealed record ParticipantProgressDto(
    Guid ParticipantId,
    IReadOnlyList<ProgressEntryDto> Entries,
    IReadOnlyList<ProjectDto> Projects);

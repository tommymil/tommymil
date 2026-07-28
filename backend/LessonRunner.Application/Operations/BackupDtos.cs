namespace LessonRunner.Application.Operations;

public sealed record BackupResultDto(
    string FileName,
    string Path,
    long SizeBytes,
    DateTimeOffset CreatedAt);

public sealed record BackupFileDto(
    string FileName,
    long SizeBytes,
    DateTimeOffset CreatedAt);

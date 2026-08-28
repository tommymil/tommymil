namespace LessonRunner.Application.Materials;

public sealed record MaterialDto(
    Guid Id,
    string Title,
    string Description,
    string ResourceUrl,
    string? FileName,
    string? ContentType,
    long? SizeBytes,
    string Visibility,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record UpsertMaterialDto(
    string Title,
    string? Description,
    string ResourceUrl,
    string? FileName,
    string? ContentType,
    long? SizeBytes,
    string Visibility);

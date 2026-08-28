using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Materials;

public enum MaterialVisibility
{
    Admin,
    Staff
}

public sealed class Material : Entity
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public required string ResourceUrl { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }
    public MaterialVisibility Visibility { get; set; } = MaterialVisibility.Staff;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

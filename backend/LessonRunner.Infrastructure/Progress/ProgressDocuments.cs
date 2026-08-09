namespace LessonRunner.Infrastructure.Progress;

internal sealed class ProgressEntryDocument
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? LessonId { get; set; }
    public required string Autonomy { get; set; }
    public bool LessonCompleted { get; set; }
    public string? NoteForParent { get; set; }
    public string? NextStep { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid? AuthorUserId { get; set; }
}

internal sealed class ProjectDocument
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? LessonId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }

    public List<ProjectSubmissionDocument> Submissions { get; set; } = [];
}

internal sealed class ProjectSubmissionDocument
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public int Version { get; set; }
    public string? Url { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }
    public string? DownloadToken { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
    public Guid? SubmittedByUserId { get; set; }
    public string? InstructorComment { get; set; }

    public ProjectDocument? Project { get; set; }
}

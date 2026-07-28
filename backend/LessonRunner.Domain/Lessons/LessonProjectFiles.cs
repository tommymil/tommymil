namespace LessonRunner.Domain.Lessons;

public sealed class LessonProjectFiles
{
    public LessonProjectFile? Starter { get; set; }
    public LessonProjectFile? Final { get; set; }
}

public sealed class LessonProjectFile
{
    public required string Label { get; set; }
    public required string Url { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public required string DownloadToken { get; set; }
}

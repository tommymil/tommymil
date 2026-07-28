namespace LessonRunner.Infrastructure.Operations;

public sealed class BackupOptions
{
    public const string SectionName = "Backup";

    public string RootPath { get; set; } = "backups";
}

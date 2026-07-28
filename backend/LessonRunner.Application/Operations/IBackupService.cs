namespace LessonRunner.Application.Operations;

public interface IBackupService
{
    Task<BackupResultDto> CreateAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<BackupFileDto>> ListAsync(CancellationToken cancellationToken);
}

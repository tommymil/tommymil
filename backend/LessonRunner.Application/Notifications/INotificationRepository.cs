using LessonRunner.Domain.Notifications;

namespace LessonRunner.Application.Notifications;

public interface INotificationRepository
{
    Task<NotificationSettings> GetSettingsAsync(CancellationToken cancellationToken);
    Task SaveSettingsAsync(NotificationSettings settings, CancellationToken cancellationToken);
    Task<bool> HasLogAsync(string dedupeKey, CancellationToken cancellationToken);
    Task AddLogAsync(NotificationLog log, CancellationToken cancellationToken);
    Task<IReadOnlyList<NotificationLog>> ListLogsAsync(int limit, CancellationToken cancellationToken);
}

using LessonRunner.Application.Notifications;
using LessonRunner.Domain.Notifications;

namespace LessonRunner.Tests;

internal sealed class InMemoryNotificationRepository : INotificationRepository
{
    private NotificationSettings _settings = new();
    private readonly List<NotificationLog> _logs = [];

    public Task<NotificationSettings> GetSettingsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(_settings);

    public Task SaveSettingsAsync(NotificationSettings settings, CancellationToken cancellationToken)
    {
        _settings = settings;
        return Task.CompletedTask;
    }

    public Task<bool> HasLogAsync(string dedupeKey, CancellationToken cancellationToken) =>
        Task.FromResult(_logs.Any(log => log.DedupeKey == dedupeKey));

    public Task AddLogAsync(NotificationLog log, CancellationToken cancellationToken)
    {
        _logs.Add(log);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<NotificationLog>> ListLogsAsync(int limit, CancellationToken cancellationToken)
    {
        IReadOnlyList<NotificationLog> result = _logs.OrderByDescending(log => log.CreatedAt).Take(limit).ToList();
        return Task.FromResult(result);
    }
}

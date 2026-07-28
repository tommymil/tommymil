namespace LessonRunner.Application.Notifications;

public interface INotificationService
{
    Task<NotificationSettingsDto> GetSettingsAsync(CancellationToken cancellationToken);
    Task<NotificationSettingsDto> UpdateSettingsAsync(NotificationSettingsDto dto, CancellationToken cancellationToken);
    Task<IReadOnlyList<NotificationLogDto>> ListLogsAsync(int limit, CancellationToken cancellationToken);
    Task<NotificationPreviewDto> PreviewAsync(string type, CancellationToken cancellationToken);
    Task NotifyAbsencesAsync(Guid sessionId, IReadOnlyList<Guid> absentParticipantIds, CancellationToken cancellationToken);
    Task SendUpcomingSessionRemindersAsync(CancellationToken cancellationToken);
}

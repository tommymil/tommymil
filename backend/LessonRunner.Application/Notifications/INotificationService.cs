namespace LessonRunner.Application.Notifications;

public interface INotificationService
{
    Task<NotificationSettingsDto> GetSettingsAsync(CancellationToken cancellationToken);
    Task<NotificationSettingsDto> UpdateSettingsAsync(NotificationSettingsDto dto, CancellationToken cancellationToken);
    Task<IReadOnlyList<NotificationLogDto>> ListLogsAsync(int limit, CancellationToken cancellationToken);
    Task<NotificationPreviewDto> PreviewAsync(string type, CancellationToken cancellationToken);
    Task NotifyAbsencesAsync(Guid sessionId, IReadOnlyList<Guid> absentParticipantIds, CancellationToken cancellationToken);
    Task SendUpcomingSessionRemindersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Informacja o przełożeniu albo odwołaniu terminu. Zwraca liczbę wysłanych wiadomości —
    /// to ona, a nie deklaracja człowieka, wypełnia znacznik „powiadomiono opiekunów”
    /// w historii zmian terminów.
    /// </summary>
    Task<int> NotifySessionRescheduledAsync(
        Guid sessionId,
        DateTimeOffset? previousScheduledAt,
        string? reason,
        bool cancelled,
        CancellationToken cancellationToken);

    /// <summary>Podsumowanie zajęć napisane przez instruktora z myślą o rodzicu.
    /// Bez takiego podsumowania nie wysyłamy nic.</summary>
    Task<int> NotifySessionSummaryAsync(Guid sessionId, CancellationToken cancellationToken);

    /// <summary>Przypomnienia o zaległych płatnościach. Uruchamiane wyłącznie świadomym
    /// działaniem administratora — nie ma tu żadnego automatu.</summary>
    Task<int> SendPaymentRemindersAsync(CancellationToken cancellationToken);
}

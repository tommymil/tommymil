namespace LessonRunner.Application.Groups;

/// <summary>
/// Operacje instruktora na terminach. Każda metoda sprawdza właściciela (instruktora grupy);
/// dla cudzego/nieistniejącego terminu zwraca null (mapowane na 404).
/// </summary>
public interface ISessionService
{
    Task<IReadOnlyList<ScheduledSessionDto>> GetScheduleAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>Grafik instruktora jako plik iCalendar do zaimportowania w swoim kalendarzu.</summary>
    Task<byte[]> ExportScheduleIcsAsync(Guid userId, CancellationToken cancellationToken);
    Task<ScheduledSessionDto?> GetSessionAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken);

    Task<SessionAttendanceDto?> StartAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken);
    Task<SessionAttendanceDto?> GetAttendanceAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken);
    Task<SessionAttendanceDto?> SaveAttendanceAsync(Guid sessionId, Guid userId, SaveAttendanceDto dto, CancellationToken cancellationToken);

    /// <summary>Znacznik pracy jednego dziecka w trakcie trwających zajęć.</summary>
    Task<SessionAttendanceDto?> SetLiveStatusAsync(Guid sessionId, Guid userId, SetLiveStatusDto dto, CancellationToken cancellationToken);

    Task<ScheduledSessionDto?> FinishAsync(Guid sessionId, Guid userId, FinishSessionDto dto, CancellationToken cancellationToken);
}

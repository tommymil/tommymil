namespace LessonRunner.Application.Groups;

/// <summary>Operacje administracyjne na grupach (tworzenie, uczestnicy, odwoływanie terminów).</summary>
public interface IGroupService
{
    Task<IReadOnlyList<GroupSummaryDto>> GetSummariesAsync(CancellationToken cancellationToken);
    Task<GroupDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<GroupDetailsDto> CreateAsync(CreateGroupDto dto, CancellationToken cancellationToken);
    Task<GroupDetailsDto?> UpdateAsync(Guid id, UpdateGroupDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<ScheduledSessionDto?> AddSessionAsync(Guid groupId, AddSessionDto dto, Guid? actingUserId, CancellationToken cancellationToken);

    Task<ScheduledSessionDto?> CancelSessionAsync(Guid groupId, Guid sessionId, CancelSessionDto? dto, Guid? actingUserId, CancellationToken cancellationToken);
    Task<ScheduledSessionDto?> RescheduleSessionAsync(Guid groupId, Guid sessionId, RescheduleSessionDto dto, Guid? actingUserId, CancellationToken cancellationToken);
    Task<ScheduledSessionDto?> SetSubstituteInstructorAsync(Guid groupId, Guid sessionId, SetSubstituteInstructorDto dto, Guid? actingUserId, CancellationToken cancellationToken);

    /// <summary>Link do spotkania i nagranie dla pojedynczego terminu (nadpisują ustawienia grupy).</summary>
    Task<ScheduledSessionDto?> SetSessionLinksAsync(Guid groupId, Guid sessionId, UpdateSessionLinksDto dto, Guid? actingUserId, CancellationToken cancellationToken);

    /// <summary>Ręczna zmiana statusu terminu (potwierdzenie, awaria techniczna, niezrealizowane).</summary>
    Task<ScheduledSessionDto?> SetSessionStatusAsync(Guid groupId, Guid sessionId, SetSessionStatusDto dto, Guid? actingUserId, CancellationToken cancellationToken);

    /// <summary>Dostępne statusy terminu wraz z etykietami - słownik dla panelu.</summary>
    IReadOnlyList<SessionStatusOptionDto> GetSessionStatusOptions();

    /// <summary>Historia zmian terminów grupy - kto, kiedy, dlaczego i czy powiadomiono opiekunów.</summary>
    Task<IReadOnlyList<SessionChangeDto>?> GetSessionHistoryAsync(Guid groupId, CancellationToken cancellationToken);

    Task<GroupAttendanceSummaryDto?> GetAttendanceSummaryAsync(Guid groupId, CancellationToken cancellationToken);
    Task<AttendanceExportDto?> ExportAttendanceSummaryAsync(Guid groupId, string? format, CancellationToken cancellationToken);
    Task<AttendanceExportDto?> ExportSessionAttendanceAsync(Guid groupId, Guid sessionId, string? format, CancellationToken cancellationToken);

    Task<IReadOnlyList<InstructorDto>> GetInstructorsAsync(CancellationToken cancellationToken);
}

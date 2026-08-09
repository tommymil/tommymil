namespace LessonRunner.Application.Parents;

public interface IParentPortalService
{
    Task<ParentPortalDto> GetPortalAsync(Guid parentUserId, CancellationToken cancellationToken);
    /// <summary>Harmonogram dziecka jako plik iCalendar.</summary>
    Task<byte[]> ExportScheduleIcsAsync(Guid parentUserId, CancellationToken cancellationToken);

    /// <summary>Zgłoszenie nieobecności dziecka na nadchodzącym terminie. Zwraca `false`,
    /// gdy rodzic nie jest powiązany z dzieckiem albo termin już się odbył.</summary>
    Task<bool> ReportAbsenceAsync(Guid parentUserId, Guid sessionId, ReportAbsenceDto dto, CancellationToken cancellationToken);

    /// <summary>Zmiana zgody na wizerunek przez opiekuna. Zwraca `false`, gdy rodzic
    /// nie jest powiązany z tym dzieckiem.</summary>
    Task<bool> UpdateConsentAsync(Guid parentUserId, UpdateParentConsentDto dto, CancellationToken cancellationToken);

    Task<IReadOnlyList<ParentParticipantLinkDto>> ListLinksAsync(CancellationToken cancellationToken);
    Task<ParentParticipantLinkDto> LinkAsync(
        Guid parentUserId,
        Guid participantId,
        string? relation,
        bool isPrimaryContact,
        bool receivesNotifications,
        CancellationToken cancellationToken);
    Task<bool> UnlinkAsync(Guid parentUserId, Guid participantId, CancellationToken cancellationToken);
}

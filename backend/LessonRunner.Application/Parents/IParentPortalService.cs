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

    /// <summary>
    /// Zakłada opiekunowi konto na podstawie danych zapisanych przy dziecku i od razu je wiąże.
    ///
    /// Dane opiekuna (imię, e-mail, relacja) leżą już na uczestniku, a mimo to założenie
    /// dostępu wymagało przepisania adresu do panelu użytkowników i osobnego powiązania na
    /// trzecim ekranie. Zwraca `null`, gdy dziecka nie ma.
    /// </summary>
    Task<GuardianAccountResultDto?> CreateGuardianAccountAsync(
        Guid participantId,
        Guid? actingUserId,
        CancellationToken cancellationToken);
}

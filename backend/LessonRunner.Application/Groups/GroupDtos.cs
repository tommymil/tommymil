namespace LessonRunner.Application.Groups;

public sealed record CreateGroupDto(
    string Name,
    Guid InstructorId,
    IReadOnlyList<Guid> LessonIds,
    DateTimeOffset FirstSessionAt,
    IReadOnlyList<Guid> ParticipantIds,
    Guid? CourseId = null,
    Guid? LocationId = null,
    int? Capacity = null,
    string? MeetingUrl = null);

public sealed record ParticipantDto(Guid Id, string FirstName, string LastName, string? Phone, string? Email, string EnrollmentStatus);

public sealed record InstructorDto(Guid Id, string Email, string DisplayName);

public sealed record GroupSummaryDto(
    Guid Id,
    string Name,
    Guid InstructorId,
    string InstructorEmail,
    string InstructorName,
    Guid? CourseId,
    Guid? LocationId,
    string? LocationName,
    int? Capacity,
    string? MeetingUrl,
    int ParticipantCount,
    int WaitlistedCount,
    int SessionCount,
    DateTimeOffset? NextSessionAt);

public sealed record ScheduledSessionDto(
    Guid Id,
    Guid GroupId,
    string GroupName,
    Guid? LessonId,
    string? LessonTitle,
    DateTimeOffset ScheduledAt,
    int SequenceNumber,
    string Status,
    string StatusLabel,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    string? InstructorNote,
    Guid? LocationId,
    string? LocationName,
    Guid? SubstituteInstructorId,
    string? SubstituteInstructorName,
    /// <summary>Link obowiązujący dla tego terminu: własny link terminu, a gdy go nie ma - link grupy.</summary>
    string? MeetingUrl,
    /// <summary>Link ustawiony wprost na terminie (bez podstawienia z grupy) - do formularza edycji.</summary>
    string? SessionMeetingUrl = null,
    string? RecordingUrl = null,
    /// <summary>Czego nie zdążyliśmy - podpowiadane na kolejnym terminie tej grupy.</summary>
    string? UnfinishedNote = null,
    /// <summary>Podsumowanie dla rodzica - jedyny fragment debriefu widoczny w portalu.</summary>
    string? ParentSummary = null);

/// <summary>Ustawienie linku do spotkania i nagrania dla pojedynczego terminu.
/// Puste wartości oznaczają wyczyszczenie - link grupy wraca wtedy do gry.</summary>
public sealed record UpdateSessionLinksDto(string? MeetingUrl, string? RecordingUrl);

public sealed record UpdateGroupDto(
    string Name,
    Guid InstructorId,
    Guid? LocationId = null,
    int? Capacity = null,
    string? MeetingUrl = null);

public sealed record AddSessionDto(
    Guid LessonId,
    DateTimeOffset ScheduledAt,
    Guid? LocationId = null,
    Guid? SubstituteInstructorId = null);

public sealed record RescheduleSessionDto(
    DateTimeOffset ScheduledAt,
    Guid? LocationId = null,
    Guid? SubstituteInstructorId = null,
    /// <summary>Powód przełożenia - trafia do historii zmian terminu.</summary>
    string? Reason = null,
    /// <summary>Czy przy tej zmianie poinformowano opiekunów.</summary>
    bool GuardiansNotified = false);

/// <summary>Ręczna zmiana statusu terminu (potwierdzenie, awaria, niezrealizowane, odwołanie).
/// Powód i autor trafiają do historii zmian.</summary>
public sealed record SetSessionStatusDto(string Status, string? Reason = null, bool GuardiansNotified = false);

public sealed record SessionStatusOptionDto(string Value, string Label, bool CountsAsHeld);

/// <summary>Odwołanie terminu z powodem. Powód trafia do historii zmian.</summary>
public sealed record CancelSessionDto(
    string? Reason = null,
    bool GuardiansNotified = false,
    /// <summary>Kto odwołał: `instructor` albo `parent`. Domyślnie instruktor.</summary>
    string? CancelledBy = null,
    /// <summary>Decyzja o rekompensacie, świadomie oddzielona od samego odwołania:
    /// `none` (domyślnie), `credit`, `makeup`, `refund`. Dokument koncepcyjny wprost odradza
    /// sklejanie zmiany terminu z rozliczeniem - to dwie różne decyzje, często podejmowane
    /// przez różne osoby i w różnym czasie.</summary>
    string? Compensation = null,
    /// <summary>Termin ważności kredytu, gdy rekompensatą jest kredyt.</summary>
    DateOnly? CreditExpiresAt = null,
    /// <summary>Przesunąć materiał odwołanych zajęć na kolejne terminy? Lekcja z odwołanego
    /// terminu wchodzi na najbliższy, reszta przesuwa się o jeden, a kurs wydłuża się
    /// o jeden termin na końcu. Domyślnie nie - czasem zajęcia po prostu przepadają.</summary>
    bool ShiftFollowingLessons = false);

/// <summary>Wpis historii zmian terminu.</summary>
public sealed record SessionChangeDto(
    Guid Id,
    Guid SessionId,
    int SequenceNumber,
    string ChangeType,
    string ChangeTypeLabel,
    DateTimeOffset? PreviousScheduledAt,
    DateTimeOffset? NewScheduledAt,
    string? Reason,
    string? Details,
    Guid? ChangedByUserId,
    string? ChangedByName,
    bool GuardiansNotified,
    DateTimeOffset ChangedAt);

public sealed record SetSubstituteInstructorDto(Guid? SubstituteInstructorId);

public sealed record ParticipantAttendanceDto(
    Guid ParticipantId,
    string FirstName,
    string LastName,
    int PresentCount,
    int HeldCount,
    int RatePercent);

public sealed record GroupAttendanceSummaryDto(
    int HeldSessions,
    IReadOnlyList<ParticipantAttendanceDto> Participants);

public sealed record AttendanceExportDto(
    string FileName,
    string ContentType,
    byte[] Content);

public sealed record GroupDetailsDto(
    Guid Id,
    string Name,
    Guid InstructorId,
    string InstructorEmail,
    string InstructorName,
    string Status,
    Guid? CourseId,
    Guid? LocationId,
    string? LocationName,
    int? Capacity,
    string? MeetingUrl,
    IReadOnlyList<ParticipantDto> Participants,
    IReadOnlyList<ScheduledSessionDto> Sessions);

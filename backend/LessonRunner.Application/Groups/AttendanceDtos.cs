namespace LessonRunner.Application.Groups;

public sealed record AttendanceEntryDto(
    Guid ParticipantId,
    string FirstName,
    string LastName,
    /// <summary>Skrót „liczy się jako obecność" - do frekwencji i szybkiego podglądu.</summary>
    bool Present,
    bool MakeupRequired,
    Guid? MakeupSessionId,
    /// <summary>Pełny status, np. `late`, `technicalissues`, `excusedabsence`.</summary>
    string Status = "unexcusedabsence",
    string StatusLabel = "Nieobecność niezgłoszona",
    string? Note = null,
    DateTimeOffset? JoinedAt = null,
    DateTimeOffset? LeftAt = null);

/// <summary>Lista statusów do wyboru w kokpicie - backend jest źródłem etykiet,
/// żeby frontend nie utrzymywał własnej kopii słownika.</summary>
public sealed record AttendanceStatusOptionDto(string Value, string Label, bool CountsAsPresent);

public sealed record SessionAttendanceDto(
    Guid SessionId,
    string Status,
    string StatusLabel,
    IReadOnlyList<AttendanceEntryDto> Entries,
    IReadOnlyList<MakeupSessionOptionDto> MakeupOptions,
    IReadOnlyList<AttendanceStatusOptionDto> StatusOptions);

public sealed record MakeupSessionOptionDto(
    Guid SessionId,
    Guid GroupId,
    string GroupName,
    DateTimeOffset ScheduledAt,
    string? LessonTitle);

public sealed record SaveAttendanceEntryDto(
    Guid ParticipantId,
    bool Present,
    bool MakeupRequired = false,
    Guid? MakeupSessionId = null,
    /// <summary>Pełny status. Gdy nie podany, bierzemy pod uwagę samo <paramref name="Present"/>
    /// - stary kontrakt nadal działa.</summary>
    string? Status = null,
    string? Note = null,
    DateTimeOffset? JoinedAt = null,
    DateTimeOffset? LeftAt = null);

public sealed record SaveAttendanceDto(IReadOnlyList<SaveAttendanceEntryDto> Entries);

public sealed record FinishSessionDto(string? Note);

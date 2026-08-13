namespace LessonRunner.Application.Groups;

public sealed record AttendanceEntryDto(
    Guid ParticipantId,
    string FirstName,
    string LastName,
    /// <summary>Skrót „liczy się jako obecność” - do frekwencji i szybkiego podglądu.</summary>
    bool Present,
    bool MakeupRequired,
    Guid? MakeupSessionId,
    /// <summary>Pełny status, np. `late`, `technicalissues`, `excusedabsence`.</summary>
    string Status = "unexcusedabsence",
    string StatusLabel = "Nieobecność niezgłoszona",
    string? Note = null,
    DateTimeOffset? JoinedAt = null,
    DateTimeOffset? LeftAt = null,
    /// <summary>Znacznik pracy na żywo: `working`, `needshelp`, `finished`, `blocked`.
    /// Żyje tylko na czas trwających zajęć i nie trafia do portalu rodzica.</summary>
    string LiveStatus = "working",
    string LiveStatusLabel = "Pracuje");

/// <summary>Lista statusów do wyboru w kokpicie - backend jest źródłem etykiet,
/// żeby frontend nie utrzymywał własnej kopii słownika.</summary>
public sealed record AttendanceStatusOptionDto(string Value, string Label, bool CountsAsPresent);

public sealed record SessionAttendanceDto(
    Guid SessionId,
    string Status,
    string StatusLabel,
    IReadOnlyList<AttendanceEntryDto> Entries,
    IReadOnlyList<MakeupSessionOptionDto> MakeupOptions,
    IReadOnlyList<AttendanceStatusOptionDto> StatusOptions,
    IReadOnlyList<LiveStatusOptionDto>? LiveStatusOptions = null);

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
    DateTimeOffset? LeftAt = null,
    /// <summary>Znacznik pracy na żywo. Pominięty = bez zmiany (autozapis wysyła
    /// tylko to, co użytkownik faktycznie zmienił).</summary>
    string? LiveStatus = null);

public sealed record SaveAttendanceDto(IReadOnlyList<SaveAttendanceEntryDto> Entries);

/// <summary>
/// Zakończenie zajęć w trzech polach zamiast jednego.
///
/// Rozdział 5 dokumentu koncepcyjnego oczekuje po lekcji trzech różnych informacji, a nie
/// jednego bloku tekstu: uwag wewnętrznych, tego czego nie zdążyliśmy (potrzebne na kolejnym
/// terminie i przy zastępstwie) oraz podsumowania dla rodzica. Wcześniej wszystko lądowało
/// w `Note`, z którego nic nie dało się odczytać automatycznie ani pokazać w portalu.
///
/// `Note` zostaje pierwszym parametrem, więc stary kontrakt (`{ "note": "..." }`) nadal działa.
/// </summary>
public sealed record FinishSessionDto(
    string? Note,
    string? UnfinishedNote = null,
    string? ParentSummary = null);

/// <summary>Zmiana znacznika pracy dziecka w trakcie trwających zajęć.</summary>
public sealed record SetLiveStatusDto(Guid ParticipantId, string LiveStatus);

/// <summary>Lista znaczników pracy na żywo - tak jak przy statusach obecności,
/// etykiety przychodzą z domeny, a nie z kopii we froncie.</summary>
public sealed record LiveStatusOptionDto(string Value, string Label);

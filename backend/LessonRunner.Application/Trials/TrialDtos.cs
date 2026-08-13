namespace LessonRunner.Application.Trials;

/// <summary>Słownik do list rozwijanych — etykiety pochodzą z domeny, nie z kopii we froncie.</summary>
public sealed record TrialOptionDto(string Value, string Label);

public sealed record TrialLessonDto(
    Guid Id,
    string ChildFirstName,
    string ChildLastName,
    DateOnly? ChildBirthDate,
    int? ChildAge,
    string? GuardianName,
    string? GuardianEmail,
    string? GuardianPhone,
    string? Source,
    string? RequestNote,
    Guid? InstructorId,
    string? InstructorName,
    DateTimeOffset? ScheduledAt,
    string? MeetingUrl,
    Guid? LessonId,
    string? LessonTitle,
    string Reading,
    string ReadingLabel,
    string Computer,
    string ComputerLabel,
    string Programming,
    string ProgrammingLabel,
    string Recommendation,
    string RecommendationLabel,
    string? RecommendedLevel,
    string? DiagnosisNote,
    DateTimeOffset? DiagnosedAt,
    string Status,
    string StatusLabel,
    Guid? ParticipantId,
    string? DeclineReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ClosedAt,
    /// <summary>Diagnoza wypełniona w komplecie — administracja ma na czym oprzeć decyzję.</summary>
    bool HasDiagnosis,
    /// <summary>Czeka na ruch: świeże zgłoszenie bez terminu albo dziecko po lekcji.</summary>
    bool NeedsAttention);

/// <summary>Zgłoszenie z telefonu albo formularza. Wymagamy wyłącznie imienia i nazwiska
/// dziecka — reszta dosypuje się w trakcie rozmowy.</summary>
public sealed record CreateTrialDto(
    string ChildFirstName,
    string ChildLastName,
    DateOnly? ChildBirthDate = null,
    string? GuardianName = null,
    string? GuardianEmail = null,
    string? GuardianPhone = null,
    string? Source = null,
    string? RequestNote = null);

/// <summary>Umówienie terminu. Puste `ScheduledAt` cofa zgłoszenie do stanu „Zgłoszenie”.</summary>
public sealed record ScheduleTrialDto(
    Guid? InstructorId,
    DateTimeOffset? ScheduledAt,
    string? MeetingUrl = null,
    Guid? LessonId = null);

/// <summary>Diagnoza wypełniana przez instruktora po lekcji.</summary>
public sealed record SaveTrialDiagnosisDto(
    string Reading,
    string Computer,
    string Programming,
    string Recommendation,
    string? RecommendedLevel = null,
    string? DiagnosisNote = null);

/// <summary>
/// Zapis dziecka do systemu.
///
/// `CreateGuardianAccount` domyślnie włączone: rodzina, która przeszła lekcję próbną,
/// ma od razu dostać dostęp do portalu — inaczej pierwsze, co widzi po decyzji, to cisza.
/// </summary>
public sealed record EnrollTrialDto(bool CreateGuardianAccount = true);

public sealed record DeclineTrialDto(string? Reason = null, bool NoShow = false);

/// <summary>Wynik zapisu — mówi, co dokładnie powstało, żeby ekran nie musiał zgadywać.</summary>
public sealed record TrialEnrollmentResultDto(
    Guid ParticipantId,
    string ParticipantName,
    bool GuardianAccountCreated,
    bool InvitationSent,
    string? GuardianEmail,
    string? Error);

public sealed record TrialBoardDto(
    IReadOnlyList<TrialLessonDto> Trials,
    IReadOnlyList<TrialOptionDto> ReadingOptions,
    IReadOnlyList<TrialOptionDto> ComputerOptions,
    IReadOnlyList<TrialOptionDto> ProgrammingOptions,
    IReadOnlyList<TrialOptionDto> RecommendationOptions,
    IReadOnlyList<TrialOptionDto> StatusOptions);

namespace LessonRunner.Application.Safety;

public sealed record IncidentParticipantDto(Guid ParticipantId, string Name);

public sealed record IncidentDto(
    Guid Id,
    string Kind,
    string KindLabel,
    string Severity,
    string SeverityLabel,
    string Status,
    string StatusLabel,
    DateTimeOffset OccurredAt,
    Guid? GroupId,
    string? GroupName,
    Guid? SessionId,
    IReadOnlyList<IncidentParticipantDto> Participants,
    string Description,
    string? ActionsTaken,
    string? Resolution,
    Guid ReportedByUserId,
    string ReportedByName,
    Guid? AssignedToUserId,
    string? AssignedToName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ResolvedAt,
    /// <summary>Sprawa wymagająca reakcji dziś — wysoka waga albo rodzaj z definicji poważny.</summary>
    bool NeedsImmediateAttention);

public sealed record CreateIncidentDto(
    string Kind,
    string Severity,
    DateTimeOffset? OccurredAt,
    Guid? GroupId,
    Guid? SessionId,
    IReadOnlyList<Guid>? ParticipantIds,
    string Description);

/// <summary>
/// Prowadzenie sprawy. Wszystkie pola opcjonalne poza statusem — administrator uzupełnia
/// je w miarę, jak sprawa się rozwija, a nie jednym ruchem.
/// </summary>
public sealed record UpdateIncidentDto(
    string Status,
    string? Severity = null,
    string? ActionsTaken = null,
    string? Resolution = null,
    Guid? AssignedToUserId = null);

/// <summary>Słownik do list rozwijanych — etykiety pochodzą z domeny, nie z kopii we froncie.</summary>
public sealed record SafetyOptionDto(string Value, string Label);

public sealed record IncidentBoardDto(
    IReadOnlyList<IncidentDto> Incidents,
    IReadOnlyList<SafetyOptionDto> KindOptions,
    IReadOnlyList<SafetyOptionDto> SeverityOptions,
    IReadOnlyList<SafetyOptionDto> StatusOptions);

public sealed record SupportTicketDto(
    Guid Id,
    Guid? ParticipantId,
    string? ParticipantName,
    Guid? SessionId,
    Guid? GroupId,
    string? GroupName,
    string Category,
    string CategoryLabel,
    string Status,
    string StatusLabel,
    string Description,
    string? Resolution,
    bool CostLessonTime,
    Guid ReportedByUserId,
    string ReportedByName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ResolvedAt);

public sealed record CreateSupportTicketDto(
    string Category,
    string Description,
    Guid? ParticipantId,
    Guid? SessionId,
    Guid? GroupId,
    bool CostLessonTime = false);

public sealed record UpdateSupportTicketDto(
    string Status,
    string? Category = null,
    string? Resolution = null,
    bool? CostLessonTime = null);

public sealed record SupportBoardDto(
    IReadOnlyList<SupportTicketDto> Tickets,
    IReadOnlyList<SafetyOptionDto> CategoryOptions,
    IReadOnlyList<SafetyOptionDto> StatusOptions);

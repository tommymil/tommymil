namespace LessonRunner.Infrastructure.Safety;

/// <summary>
/// Incydent w bazie.
///
/// Bez kluczy obcych do grup, terminów i dzieci — incydent ma przetrwać usunięcie grupy
/// i anonimizację dziecka. To rejestr zdarzeń, a nie część dziennika zajęć.
/// </summary>
internal sealed class IncidentDocument
{
    public Guid Id { get; set; }
    public required string Kind { get; set; }
    public required string Severity { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? SessionId { get; set; }

    /// <summary>Identyfikatory dzieci rozdzielone przecinkami — patrz komentarz przy encji.</summary>
    public string ParticipantIds { get; set; } = string.Empty;

    public required string Description { get; set; }
    public string? ActionsTaken { get; set; }
    public string? Resolution { get; set; }
    public Guid ReportedByUserId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
}

/// <summary>Zgłoszenie techniczne w bazie. Powiązania jak wyżej — luźne, bez kluczy obcych.</summary>
internal sealed class SupportTicketDocument
{
    public Guid Id { get; set; }
    public Guid? ParticipantId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? GroupId { get; set; }
    public required string Category { get; set; }
    public required string Status { get; set; }
    public required string Description { get; set; }
    public string? Resolution { get; set; }
    public bool CostLessonTime { get; set; }
    public Guid ReportedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
}

namespace LessonRunner.Infrastructure.Trials;

/// <summary>
/// Zgłoszenie na lekcję próbną w bazie.
///
/// Bez klucza obcego do uczestnika: `ParticipantId` wypełnia się dopiero przy zapisie,
/// a rekord zgłoszenia ma przetrwać późniejsze usunięcie albo anonimizację dziecka.
/// To rejestr pozyskania klienta, a nie część kartoteki uczestnika.
/// </summary>
internal sealed class TrialLessonDocument
{
    public Guid Id { get; set; }

    public required string ChildFirstName { get; set; }
    public required string ChildLastName { get; set; }
    public DateOnly? ChildBirthDate { get; set; }

    public string? GuardianName { get; set; }
    public string? GuardianEmail { get; set; }
    public string? GuardianPhone { get; set; }
    public string? Source { get; set; }
    public string? RequestNote { get; set; }

    public Guid? InstructorId { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }
    public string? MeetingUrl { get; set; }
    public Guid? LessonId { get; set; }

    public required string Reading { get; set; }
    public required string Computer { get; set; }
    public required string Programming { get; set; }
    public required string Recommendation { get; set; }
    public string? RecommendedLevel { get; set; }
    public string? DiagnosisNote { get; set; }
    public DateTimeOffset? DiagnosedAt { get; set; }
    public Guid? DiagnosedByUserId { get; set; }

    public required string Status { get; set; }
    public Guid? ParticipantId { get; set; }
    public string? DeclineReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
}

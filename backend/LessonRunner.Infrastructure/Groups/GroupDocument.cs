namespace LessonRunner.Infrastructure.Groups;

internal sealed class GroupDocument
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid InstructorId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? LocationId { get; set; }
    public int? Capacity { get; set; }
    public string? MeetingUrl { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<GroupEnrollmentDocument> Enrollments { get; set; } = [];
    public List<ScheduledSessionDocument> Sessions { get; set; } = [];
}

internal sealed class GroupEnrollmentDocument
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid ParticipantId { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
}

internal sealed class ScheduledSessionDocument
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid? LessonId { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? SubstituteInstructorId { get; set; }
    public int SequenceNumber { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? InstructorNote { get; set; }

    /// <summary>Czego nie zdążyliśmy - osobne pole, żeby dało się to podpowiedzieć
    /// na kolejnym terminie zamiast szukać zdania w notatce.</summary>
    public string? UnfinishedNote { get; set; }

    /// <summary>Podsumowanie dla rodzica - jedyny fragment debriefu widoczny w portalu.</summary>
    public string? ParentSummary { get; set; }

    public string? MeetingUrl { get; set; }
    public string? RecordingUrl { get; set; }

    public List<AttendanceRecordDocument> Attendance { get; set; } = [];
}

internal sealed class SessionChangeLogDocument
{
    public Guid Id { get; set; }
    public Guid ScheduledSessionId { get; set; }
    public Guid GroupId { get; set; }
    public required string ChangeType { get; set; }
    public DateTimeOffset? PreviousScheduledAt { get; set; }
    public DateTimeOffset? NewScheduledAt { get; set; }
    public string? Reason { get; set; }
    public string? Details { get; set; }
    public Guid? ChangedByUserId { get; set; }
    public bool GuardiansNotified { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
}

internal sealed class AttendanceRecordDocument
{
    public Guid Id { get; set; }
    public Guid ScheduledSessionId { get; set; }
    public Guid ParticipantId { get; set; }

    /// <summary>Zdenormalizowany skrót statusu. Zostaje jako osobna kolumna, bo liczą z niej
    /// frekwencję zapytania zbiorcze i KPI - taniej niż parsowanie statusu w każdym miejscu.</summary>
    public bool Present { get; set; }

    /// <summary>Pełny status. Puste = rekord sprzed migracji `AddAttendanceStatus`;
    /// przy odczycie podstawiamy wtedy status wyprowadzony z <see cref="Present"/>.</summary>
    public string Status { get; set; } = string.Empty;

    public string? Note { get; set; }
    public DateTimeOffset? JoinedAt { get; set; }
    public DateTimeOffset? LeftAt { get; set; }

    /// <summary>Znacznik pracy na żywo. Puste = rekord sprzed migracji
    /// `AddSessionDebriefAndLiveStatus`; przy odczycie podstawiamy „Working".</summary>
    public string LiveStatus { get; set; } = string.Empty;

    public bool MakeupRequired { get; set; }
    public Guid? MakeupSessionId { get; set; }
    public DateTimeOffset MarkedAt { get; set; }
}

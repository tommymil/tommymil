using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.LessonRuns;

public sealed class LessonRunSession : Entity
{
    public required Guid LessonId { get; init; }
    public required Guid UserId { get; init; }

    /// <summary>Termin z grafiku, którego dotyczy prowadzenie; Guid.Empty dla prowadzenia ad-hoc.</summary>
    public Guid ScheduledSessionId { get; init; }
    public int StepIndex { get; set; }
    public int ElapsedTotalSeconds { get; set; }
    public int ElapsedStepSeconds { get; set; }
    public bool Running { get; set; }
    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

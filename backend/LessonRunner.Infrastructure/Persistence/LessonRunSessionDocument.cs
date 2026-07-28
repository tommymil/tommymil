namespace LessonRunner.Infrastructure.Persistence;

internal sealed class LessonRunSessionDocument
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public Guid UserId { get; set; }
    public Guid ScheduledSessionId { get; set; }
    public int StepIndex { get; set; }
    public int ElapsedTotalSeconds { get; set; }
    public int ElapsedStepSeconds { get; set; }
    public bool Running { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

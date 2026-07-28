namespace LessonRunner.Application.LessonRuns;

public sealed record LessonRunSessionDto(
    Guid Id,
    Guid LessonId,
    Guid UserId,
    int StepIndex,
    int ElapsedTotalSeconds,
    int ElapsedStepSeconds,
    bool Running,
    DateTimeOffset StartedAt,
    DateTimeOffset UpdatedAt);

public sealed record UpdateLessonRunSessionDto(
    int StepIndex,
    int ElapsedTotalSeconds,
    int ElapsedStepSeconds,
    bool Running);

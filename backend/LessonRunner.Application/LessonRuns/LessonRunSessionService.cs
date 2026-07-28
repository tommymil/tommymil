using LessonRunner.Application.Lessons;
using LessonRunner.Domain.LessonRuns;

namespace LessonRunner.Application.LessonRuns;

public sealed class LessonRunSessionService(
    ILessonRepository lessonRepository,
    ILessonRunSessionRepository sessionRepository) : ILessonRunSessionService
{
    public async Task<LessonRunSessionDto?> GetOrCreateAsync(Guid lessonId, Guid userId, Guid scheduledSessionId, CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(lessonId, cancellationToken);

        if (lesson is null)
        {
            return null;
        }

        var existing = await sessionRepository.GetByLessonAndUserAsync(lessonId, userId, scheduledSessionId, cancellationToken);

        if (existing is not null)
        {
            Normalize(existing, lesson.Steps.Count);
            return ToDto(existing);
        }

        var created = await sessionRepository.AddAsync(
            new LessonRunSession { LessonId = lessonId, UserId = userId, ScheduledSessionId = scheduledSessionId },
            cancellationToken);

        Normalize(created, lesson.Steps.Count);
        return ToDto(created);
    }

    public async Task<LessonRunSessionDto?> UpdateAsync(
        Guid lessonId,
        Guid userId,
        Guid scheduledSessionId,
        UpdateLessonRunSessionDto dto,
        CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(lessonId, cancellationToken);

        if (lesson is null)
        {
            return null;
        }

        var session = await sessionRepository.GetByLessonAndUserAsync(lessonId, userId, scheduledSessionId, cancellationToken)
            ?? await sessionRepository.AddAsync(
                new LessonRunSession { LessonId = lessonId, UserId = userId, ScheduledSessionId = scheduledSessionId },
                cancellationToken);

        Apply(session, dto, lesson.Steps.Count);
        await sessionRepository.UpdateAsync(session, cancellationToken);

        return ToDto(session);
    }

    private static void Apply(LessonRunSession session, UpdateLessonRunSessionDto dto, int stepCount)
    {
        session.StepIndex = ClampStepIndex(dto.StepIndex, stepCount);
        session.ElapsedTotalSeconds = Math.Max(0, dto.ElapsedTotalSeconds);
        session.ElapsedStepSeconds = Math.Max(0, dto.ElapsedStepSeconds);
        session.Running = dto.Running;
        session.UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void Normalize(LessonRunSession session, int stepCount)
    {
        session.StepIndex = ClampStepIndex(session.StepIndex, stepCount);
        session.ElapsedTotalSeconds = Math.Max(0, session.ElapsedTotalSeconds);
        session.ElapsedStepSeconds = Math.Max(0, session.ElapsedStepSeconds);
    }

    private static int ClampStepIndex(int stepIndex, int stepCount)
    {
        return Math.Min(Math.Max(stepIndex, 0), Math.Max(stepCount - 1, 0));
    }

    private static LessonRunSessionDto ToDto(LessonRunSession session)
    {
        return new LessonRunSessionDto(
            session.Id,
            session.LessonId,
            session.UserId,
            session.StepIndex,
            session.ElapsedTotalSeconds,
            session.ElapsedStepSeconds,
            session.Running,
            session.StartedAt,
            session.UpdatedAt);
    }
}

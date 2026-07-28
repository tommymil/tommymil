namespace LessonRunner.Application.LessonRuns;

public interface ILessonRunSessionService
{
    Task<LessonRunSessionDto?> GetOrCreateAsync(Guid lessonId, Guid userId, Guid scheduledSessionId, CancellationToken cancellationToken);
    Task<LessonRunSessionDto?> UpdateAsync(
        Guid lessonId,
        Guid userId,
        Guid scheduledSessionId,
        UpdateLessonRunSessionDto dto,
        CancellationToken cancellationToken);
}

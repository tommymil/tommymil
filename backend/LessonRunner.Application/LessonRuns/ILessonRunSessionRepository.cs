using LessonRunner.Domain.LessonRuns;

namespace LessonRunner.Application.LessonRuns;

public interface ILessonRunSessionRepository
{
    Task<LessonRunSession?> GetByLessonAndUserAsync(Guid lessonId, Guid userId, Guid scheduledSessionId, CancellationToken cancellationToken);

    /// <summary>
    /// Wstawia sesję lub - gdy równoległe żądanie utworzyło już sesję dla tej pary
    /// (LessonId, UserId) - zwraca istniejący wiersz. Operacja jest idempotentna.
    /// </summary>
    Task<LessonRunSession> AddAsync(LessonRunSession session, CancellationToken cancellationToken);
    Task UpdateAsync(LessonRunSession session, CancellationToken cancellationToken);
}

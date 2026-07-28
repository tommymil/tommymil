using LessonRunner.Application.LessonRuns;
using LessonRunner.Domain.LessonRuns;

namespace LessonRunner.Tests;

internal sealed class InMemoryLessonRunSessionRepository : ILessonRunSessionRepository
{
    private readonly List<LessonRunSession> _sessions = [];

    public Task<LessonRunSession?> GetByLessonAndUserAsync(Guid lessonId, Guid userId, Guid scheduledSessionId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_sessions.FirstOrDefault(session =>
            session.LessonId == lessonId && session.UserId == userId && session.ScheduledSessionId == scheduledSessionId));
    }

    public Task<LessonRunSession> AddAsync(LessonRunSession session, CancellationToken cancellationToken)
    {
        var existing = _sessions.FirstOrDefault(item =>
            item.LessonId == session.LessonId && item.UserId == session.UserId && item.ScheduledSessionId == session.ScheduledSessionId);

        if (existing is not null)
        {
            return Task.FromResult(existing);
        }

        _sessions.Add(session);
        return Task.FromResult(session);
    }

    public Task UpdateAsync(LessonRunSession session, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

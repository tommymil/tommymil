using LessonRunner.Application.LessonRuns;
using LessonRunner.Domain.LessonRuns;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.LessonRuns;

internal sealed class EfLessonRunSessionRepository(AppDbContext dbContext) : ILessonRunSessionRepository
{
    public async Task<LessonRunSession?> GetByLessonAndUserAsync(Guid lessonId, Guid userId, Guid scheduledSessionId, CancellationToken cancellationToken)
    {
        var document = await dbContext.LessonRunSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                session => session.LessonId == lessonId && session.UserId == userId && session.ScheduledSessionId == scheduledSessionId,
                cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task<LessonRunSession> AddAsync(LessonRunSession session, CancellationToken cancellationToken)
    {
        var document = ToDocument(session);
        dbContext.LessonRunSessions.Add(document);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return ToDomain(document);
        }
        catch (DbUpdateException)
        {
            // Wyścig: równoległe żądanie utworzyło już sesję dla tej pary (LessonId, UserId)
            // i naruszyliśmy unikalny indeks. Porzucamy nieudany wpis i zwracamy istniejący
            // wiersz, dzięki czemu "get or create" pozostaje idempotentne.
            dbContext.Entry(document).State = EntityState.Detached;

            var existing = await dbContext.LessonRunSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.LessonId == session.LessonId
                        && item.UserId == session.UserId
                        && item.ScheduledSessionId == session.ScheduledSessionId,
                    cancellationToken);

            if (existing is null)
            {
                throw;
            }

            return ToDomain(existing);
        }
    }

    public async Task UpdateAsync(LessonRunSession session, CancellationToken cancellationToken)
    {
        var document = await dbContext.LessonRunSessions
            .FirstAsync(item => item.Id == session.Id, cancellationToken);

        document.StepIndex = session.StepIndex;
        document.ElapsedTotalSeconds = session.ElapsedTotalSeconds;
        document.ElapsedStepSeconds = session.ElapsedStepSeconds;
        document.Running = session.Running;
        document.UpdatedAt = session.UpdatedAt;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static LessonRunSession ToDomain(LessonRunSessionDocument document)
    {
        return new LessonRunSession
        {
            Id = document.Id,
            LessonId = document.LessonId,
            UserId = document.UserId,
            ScheduledSessionId = document.ScheduledSessionId,
            StepIndex = document.StepIndex,
            ElapsedTotalSeconds = document.ElapsedTotalSeconds,
            ElapsedStepSeconds = document.ElapsedStepSeconds,
            Running = document.Running,
            StartedAt = document.StartedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    private static LessonRunSessionDocument ToDocument(LessonRunSession session)
    {
        return new LessonRunSessionDocument
        {
            Id = session.Id,
            LessonId = session.LessonId,
            UserId = session.UserId,
            ScheduledSessionId = session.ScheduledSessionId,
            StepIndex = session.StepIndex,
            ElapsedTotalSeconds = session.ElapsedTotalSeconds,
            ElapsedStepSeconds = session.ElapsedStepSeconds,
            Running = session.Running,
            StartedAt = session.StartedAt,
            UpdatedAt = session.UpdatedAt
        };
    }
}

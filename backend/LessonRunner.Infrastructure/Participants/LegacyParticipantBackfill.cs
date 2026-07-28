using System.Data;
using LessonRunner.Infrastructure.Groups;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Participants;

/// <summary>
/// Jednorazowy, idempotentny backfill: przepisuje wiersze ze starej tabeli "GroupParticipants"
/// (uczestnik zaszyty 1:1 w grupie) do nowego modelu Participants + GroupEnrollments, zachowując
/// identyczne Id uczestnika - dzięki temu AttendanceRecords.ParticipantId pozostaje poprawny bez
/// żadnych zmian. Bezpieczny do wielokrotnego wywołania i bezpieczny, gdy stara tabela już nie istnieje
/// (po przyszłej migracji porządkującej).
/// </summary>
internal static class LegacyParticipantBackfill
{
    public static async Task RunAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Participants.AnyAsync(cancellationToken))
        {
            return;
        }

        if (!await LegacyTableExistsAsync(dbContext, cancellationToken))
        {
            return;
        }

        var legacyOptions = new DbContextOptionsBuilder<LegacyParticipantsDbContext>()
            .UseSqlite(dbContext.Database.GetDbConnection().ConnectionString)
            .Options;

        List<LegacyGroupParticipantDocument> legacyParticipants;
        await using (var legacyContext = new LegacyParticipantsDbContext(legacyOptions))
        {
            legacyParticipants = await legacyContext.GroupParticipants.AsNoTracking().ToListAsync(cancellationToken);
        }

        if (legacyParticipants.Count == 0)
        {
            return;
        }

        foreach (var legacy in legacyParticipants)
        {
            dbContext.Participants.Add(new ParticipantDocument
            {
                Id = legacy.Id,
                FirstName = legacy.FirstName,
                LastName = legacy.LastName,
                Phone = legacy.Phone,
                Email = legacy.Email,
                CreatedAt = legacy.CreatedAt,
                UpdatedAt = legacy.CreatedAt
            });

            dbContext.GroupEnrollments.Add(new GroupEnrollmentDocument
            {
                Id = Guid.NewGuid(),
                GroupId = legacy.GroupId,
                ParticipantId = legacy.Id,
                Status = Domain.Groups.EnrollmentStatus.Enrolled.ToString(),
                EnrolledAt = legacy.CreatedAt
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<bool> LegacyTableExistsAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();
        var wasClosed = connection.State != ConnectionState.Open;

        if (wasClosed)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'GroupParticipants'";
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is not null;
        }
        finally
        {
            if (wasClosed)
            {
                await connection.CloseAsync();
            }
        }
    }
}

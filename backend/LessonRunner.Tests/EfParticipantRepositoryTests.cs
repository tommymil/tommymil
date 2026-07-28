using LessonRunner.Domain.Participants;
using LessonRunner.Infrastructure.Groups;
using LessonRunner.Infrastructure.Participants;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>Round-trip repozytorium uczestników na SQLite in-memory + weryfikacja backfillu ze starej tabeli GroupParticipants.</summary>
public sealed class EfParticipantRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public EfParticipantRepositoryTests()
    {
        // Cache=Shared, bo backfill otwiera dodatkowe połączenie do tej samej bazy in-memory
        // (LegacyParticipantsDbContext) - zwykłe ":memory:" tworzyłoby dla niego osobną, pustą bazę.
        _connection = new SqliteConnection($"Data Source={Guid.NewGuid()};Mode=Memory;Cache=Shared");
        _connection.Open();
        _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        using var context = new AppDbContext(_options);
        context.Database.EnsureCreated();
    }

    private AppDbContext CreateContext() => new(_options);

    [Fact]
    public async Task AddThenGetById_RoundTripsParticipant()
    {
        var participant = new Participant { FirstName = "Jan", LastName = "Kowalski", Phone = "123456789", Email = "jan@x.pl" };

        await using (var context = CreateContext())
        {
            await new EfParticipantRepository(context).AddAsync(participant, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var loaded = await new EfParticipantRepository(context).GetByIdAsync(participant.Id, CancellationToken.None);

            Assert.NotNull(loaded);
            Assert.Equal("Jan", loaded!.FirstName);
            Assert.Equal("Kowalski", loaded.LastName);
            Assert.Equal("123456789", loaded.Phone);
            Assert.Equal("jan@x.pl", loaded.Email);
        }
    }

    [Fact]
    public async Task GetByIdsAsync_ReturnsOnlyRequestedParticipants()
    {
        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        var ola = new Participant { FirstName = "Ola", LastName = "Nowak" };

        await using (var context = CreateContext())
        {
            var repository = new EfParticipantRepository(context);
            await repository.AddAsync(jan, CancellationToken.None);
            await repository.AddAsync(ola, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var loaded = await new EfParticipantRepository(context).GetByIdsAsync([jan.Id], CancellationToken.None);

            Assert.Single(loaded);
            Assert.Equal(jan.Id, loaded[0].Id);
        }
    }

    [Fact]
    public async Task UpdateAsync_PersistsContactData()
    {
        var participant = new Participant { FirstName = "Jan", LastName = "Kowalski" };

        await using (var context = CreateContext())
        {
            await new EfParticipantRepository(context).AddAsync(participant, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            participant.Phone = "999888777";
            await new EfParticipantRepository(context).UpdateAsync(participant, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var loaded = await new EfParticipantRepository(context).GetByIdAsync(participant.Id, CancellationToken.None);
            Assert.Equal("999888777", loaded!.Phone);
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesParticipant()
    {
        var participant = new Participant { FirstName = "Jan", LastName = "Kowalski" };

        await using (var context = CreateContext())
        {
            await new EfParticipantRepository(context).AddAsync(participant, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var deleted = await new EfParticipantRepository(context).DeleteAsync(participant.Id, CancellationToken.None);
            Assert.True(deleted);
        }

        await using (var context = CreateContext())
        {
            Assert.Null(await new EfParticipantRepository(context).GetByIdAsync(participant.Id, CancellationToken.None));
        }
    }

    [Fact]
    public async Task LegacyParticipantBackfill_MovesLegacyRowsIntoParticipantsAndEnrollments()
    {
        var groupId = Guid.NewGuid();
        var legacyId = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2026, 1, 10, 9, 0, 0, TimeSpan.Zero);

        await using (var context = CreateContext())
        {
            context.Groups.Add(new GroupDocument
            {
                Id = groupId,
                Name = "Grupa testowa",
                InstructorId = Guid.NewGuid(),
                Status = "active",
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            });
            await context.SaveChangesAsync(CancellationToken.None);

            await context.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE GroupParticipants (
                    Id TEXT NOT NULL PRIMARY KEY,
                    GroupId TEXT NOT NULL,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    Phone TEXT NULL,
                    Email TEXT NULL,
                    CreatedAt TEXT NOT NULL
                )
                """);
            await context.Database.ExecuteSqlRawAsync(
                "INSERT INTO GroupParticipants (Id, GroupId, FirstName, LastName, Phone, Email, CreatedAt) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                legacyId, groupId, "Jan", "Kowalski", "123456789", "jan@x.pl", createdAt);
        }

        await using (var context = CreateContext())
        {
            await LegacyParticipantBackfill.RunAsync(context, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var participant = await new EfParticipantRepository(context).GetByIdAsync(legacyId, CancellationToken.None);
            Assert.NotNull(participant);
            Assert.Equal("Jan", participant!.FirstName);
            Assert.Equal(createdAt, participant.CreatedAt);

            var enrollment = await context.GroupEnrollments
                .FirstOrDefaultAsync(item => item.ParticipantId == legacyId && item.GroupId == groupId, CancellationToken.None);
            Assert.NotNull(enrollment);
            Assert.Equal(createdAt, enrollment!.EnrolledAt);
        }
    }

    [Fact]
    public async Task LegacyParticipantBackfill_IsNoopWhenLegacyTableIsAbsent()
    {
        await using var context = CreateContext();

        await LegacyParticipantBackfill.RunAsync(context, CancellationToken.None);

        Assert.Empty(await context.Participants.ToListAsync(CancellationToken.None));
    }

    public void Dispose() => _connection.Dispose();
}

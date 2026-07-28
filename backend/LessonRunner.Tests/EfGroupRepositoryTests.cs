using LessonRunner.Domain.Groups;
using LessonRunner.Infrastructure.Groups;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>Round-trip repozytorium grup na SQLite in-memory (agregat + obecność).</summary>
public sealed class EfGroupRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public EfGroupRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        using var context = new AppDbContext(_options);
        context.Database.EnsureCreated();
    }

    private AppDbContext CreateContext() => new(_options);

    private static Group BuildGroup(Guid instructorId)
    {
        var group = new Group { Name = "Grupa testowa", InstructorId = instructorId };
        group.Enrollments =
        [
            new GroupEnrollment { GroupId = group.Id, ParticipantId = Guid.NewGuid() },
            new GroupEnrollment { GroupId = group.Id, ParticipantId = Guid.NewGuid() }
        ];
        group.Sessions =
        [
            new ScheduledSession { GroupId = group.Id, LessonId = Guid.NewGuid(), ScheduledAt = DateTimeOffset.UtcNow, SequenceNumber = 1 },
            new ScheduledSession { GroupId = group.Id, LessonId = Guid.NewGuid(), ScheduledAt = DateTimeOffset.UtcNow.AddDays(7), SequenceNumber = 2 }
        ];
        return group;
    }

    [Fact]
    public async Task AddThenGetById_RoundTripsAggregate()
    {
        var group = BuildGroup(Guid.NewGuid());

        await using (var context = CreateContext())
        {
            await new EfGroupRepository(context).AddAsync(group, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var loaded = await new EfGroupRepository(context).GetByIdAsync(group.Id, CancellationToken.None);

            Assert.NotNull(loaded);
            Assert.Equal("Grupa testowa", loaded!.Name);
            Assert.Equal(2, loaded.Enrollments.Count);
            Assert.Equal(2, loaded.Sessions.Count);
            Assert.Equal([1, 2], loaded.Sessions.OrderBy(session => session.SequenceNumber).Select(session => session.SequenceNumber));
        }
    }

    [Fact]
    public async Task SaveAttendanceAndUpdateSession_Persist()
    {
        var group = BuildGroup(Guid.NewGuid());
        var sessionId = group.Sessions[0].Id;
        var participantId = group.Enrollments[0].ParticipantId;

        await using (var context = CreateContext())
        {
            await new EfGroupRepository(context).AddAsync(group, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var repository = new EfGroupRepository(context);
            await repository.SaveAttendanceAsync(
                sessionId,
                [new AttendanceRecord { ScheduledSessionId = sessionId, ParticipantId = participantId, Present = true }],
                CancellationToken.None);

            var session = group.Sessions[0];
            session.Status = ScheduledSessionStatus.Completed;
            session.InstructorNote = "Notatka";
            await repository.UpdateSessionAsync(session, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var loaded = await new EfGroupRepository(context).GetBySessionIdAsync(sessionId, CancellationToken.None);
            var session = loaded!.Sessions.Single(item => item.Id == sessionId);

            Assert.Equal(ScheduledSessionStatus.Completed, session.Status);
            Assert.Equal("Notatka", session.InstructorNote);
            Assert.Single(session.Attendance);
            Assert.True(session.Attendance[0].Present);
        }
    }

    [Fact]
    public async Task UpdateGroup_PersistsMeetingUrl()
    {
        var group = BuildGroup(Guid.NewGuid());

        await using (var context = CreateContext())
        {
            await new EfGroupRepository(context).AddAsync(group, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            group.MeetingUrl = "https://zoom.us/j/987654321";
            group.UpdatedAt = DateTimeOffset.UtcNow;
            await new EfGroupRepository(context).UpdateGroupAsync(group, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var loaded = await new EfGroupRepository(context).GetByIdAsync(group.Id, CancellationToken.None);
            Assert.Equal("https://zoom.us/j/987654321", loaded!.MeetingUrl);
        }
    }

    public void Dispose() => _connection.Dispose();
}

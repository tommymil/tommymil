using LessonRunner.Application.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Users;
using Xunit;

namespace LessonRunner.Tests;

public sealed class SessionServiceTests
{
    private static async Task<(SessionService Sessions, GroupService Groups, GroupDetailsDto Group, Guid InstructorId)> BuildAsync()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = new Lesson { Title = "L1", Subject = "Scratch", Level = "P1", Description = "d", Status = LessonStatus.Ready };
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var participantRepository = new InMemoryParticipantRepository();
        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        var ola = new Participant { FirstName = "Ola", LastName = "Nowak" };
        await participantRepository.AddAsync(jan, CancellationToken.None);
        await participantRepository.AddAsync(ola, CancellationToken.None);

        var groupRepository = new InMemoryGroupRepository();
        var groupService = new GroupService(groupRepository, lessons, users, participantRepository);
        var details = await groupService.CreateAsync(
            new CreateGroupDto(
                "Grupa",
                instructor.Id,
                [lesson.Id],
                new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero),
                [jan.Id, ola.Id]),
            CancellationToken.None);

        var sessionService = new SessionService(groupRepository, lessons, participantRepository);
        return (sessionService, groupService, details, instructor.Id);
    }

    [Fact]
    public async Task StartAsync_MarksInProgressAndCreatesUnmarkedAttendance()
    {
        var (sessions, _, group, instructorId) = await BuildAsync();
        var sessionId = group.Sessions[0].Id;

        var attendance = await sessions.StartAsync(sessionId, instructorId, CancellationToken.None);

        Assert.NotNull(attendance);
        Assert.Equal("inprogress", attendance!.Status);
        Assert.Equal(2, attendance.Entries.Count);
        Assert.All(attendance.Entries, entry => Assert.False(entry.Present));
    }

    [Fact]
    public async Task SaveAttendanceAsync_UpdatesPresentFlag()
    {
        var (sessions, _, group, instructorId) = await BuildAsync();
        var sessionId = group.Sessions[0].Id;
        var attendance = await sessions.StartAsync(sessionId, instructorId, CancellationToken.None);
        var participantId = attendance!.Entries[0].ParticipantId;

        var saved = await sessions.SaveAttendanceAsync(
            sessionId,
            instructorId,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(participantId, true)]),
            CancellationToken.None);

        Assert.NotNull(saved);
        Assert.True(saved!.Entries.Single(entry => entry.ParticipantId == participantId).Present);
        Assert.False(saved.Entries.Single(entry => entry.ParticipantId != participantId).Present);
    }

    [Fact]
    public async Task SaveAttendanceAsync_StoresRichStatusAndNote()
    {
        var (sessions, _, group, instructorId) = await BuildAsync();
        var sessionId = group.Sessions[0].Id;
        var attendance = await sessions.StartAsync(sessionId, instructorId, CancellationToken.None);
        var spozniony = attendance!.Entries[0].ParticipantId;
        var awaria = attendance.Entries[1].ParticipantId;

        var saved = await sessions.SaveAttendanceAsync(
            sessionId,
            instructorId,
            new SaveAttendanceDto([
                new SaveAttendanceEntryDto(spozniony, true, Status: "late", Note: "  dołączył 15 minut później  "),
                new SaveAttendanceEntryDto(awaria, false, Status: "technicalissues", Note: "zerwane łącze")
            ]),
            CancellationToken.None);

        var pierwszy = saved!.Entries.Single(entry => entry.ParticipantId == spozniony);
        Assert.Equal("late", pierwszy.Status);
        Assert.Equal("Spóźniony", pierwszy.StatusLabel);
        Assert.True(pierwszy.Present);
        Assert.Equal("dołączył 15 minut później", pierwszy.Note);

        // Problemy techniczne to nieudana próba uczestnictwa - do frekwencji liczymy je jako obecność.
        var drugi = saved.Entries.Single(entry => entry.ParticipantId == awaria);
        Assert.Equal("technicalissues", drugi.Status);
        Assert.True(drugi.Present);
    }

    [Fact]
    public async Task SaveAttendanceAsync_KeepsOldContractWhenStatusNotSent()
    {
        var (sessions, _, group, instructorId) = await BuildAsync();
        var sessionId = group.Sessions[0].Id;
        var attendance = await sessions.StartAsync(sessionId, instructorId, CancellationToken.None);
        var participantId = attendance!.Entries[0].ParticipantId;

        var saved = await sessions.SaveAttendanceAsync(
            sessionId,
            instructorId,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(participantId, true)]),
            CancellationToken.None);

        var entry = saved!.Entries.Single(item => item.ParticipantId == participantId);
        Assert.Equal("present", entry.Status);
        Assert.True(entry.Present);
    }

    [Fact]
    public async Task SaveAttendanceAsync_ExposesStatusDictionary()
    {
        var (sessions, _, group, instructorId) = await BuildAsync();
        var attendance = await sessions.StartAsync(group.Sessions[0].Id, instructorId, CancellationToken.None);

        // Frontend nie utrzymuje własnej kopii słownika - etykiety przychodzą z backendu.
        Assert.Contains(attendance!.StatusOptions, option => option.Value == "late" && option.CountsAsPresent);
        Assert.Contains(attendance.StatusOptions, option => option.Value == "excusedabsence" && !option.CountsAsPresent);
    }

    [Fact]
    public async Task FinishAsync_SetsCompletedWithNote()
    {
        var (sessions, _, group, instructorId) = await BuildAsync();
        var sessionId = group.Sessions[0].Id;
        await sessions.StartAsync(sessionId, instructorId, CancellationToken.None);

        var finished = await sessions.FinishAsync(
            sessionId,
            instructorId,
            new FinishSessionDto("Świetne zajęcia, wszyscy aktywni."),
            CancellationToken.None);

        Assert.NotNull(finished);
        Assert.Equal("completed", finished!.Status);
        Assert.Equal("Świetne zajęcia, wszyscy aktywni.", finished.InstructorNote);
        Assert.NotNull(finished.CompletedAt);
    }

    [Fact]
    public async Task ForeignInstructor_CannotSeeOrStartSession()
    {
        var (sessions, _, group, _) = await BuildAsync();
        var sessionId = group.Sessions[0].Id;
        var foreignInstructor = Guid.NewGuid();

        Assert.Null(await sessions.GetSessionAsync(sessionId, foreignInstructor, CancellationToken.None));
        Assert.Null(await sessions.StartAsync(sessionId, foreignInstructor, CancellationToken.None));
    }

    [Fact]
    public async Task GetScheduleAsync_ReturnsOnlyOwnSessions()
    {
        var (sessions, _, _, instructorId) = await BuildAsync();

        var mine = await sessions.GetScheduleAsync(instructorId, CancellationToken.None);
        var others = await sessions.GetScheduleAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Single(mine);
        Assert.Empty(others);
    }

    [Fact]
    public async Task StartAsync_IncludesParticipantsAssignedForMakeup()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = new Lesson { Title = "L1", Subject = "Scratch", Level = "P1", Description = "d", Status = LessonStatus.Ready };
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var participants = new InMemoryParticipantRepository();
        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        var ola = new Participant { FirstName = "Ola", LastName = "Nowak" };
        await participants.AddAsync(jan, CancellationToken.None);
        await participants.AddAsync(ola, CancellationToken.None);

        var repository = new InMemoryGroupRepository();
        var groupService = new GroupService(repository, lessons, users, participants);
        var sessionService = new SessionService(repository, lessons, participants);

        var source = await groupService.CreateAsync(
            new CreateGroupDto("Zrodlowa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow.AddDays(7), [jan.Id]),
            CancellationToken.None);
        var target = await groupService.CreateAsync(
            new CreateGroupDto("Docelowa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow.AddDays(14), [ola.Id]),
            CancellationToken.None);

        await sessionService.StartAsync(source.Sessions[0].Id, instructor.Id, CancellationToken.None);
        await sessionService.SaveAttendanceAsync(
            source.Sessions[0].Id,
            instructor.Id,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(jan.Id, false, true, target.Sessions[0].Id)]),
            CancellationToken.None);

        var targetAttendance = await sessionService.StartAsync(target.Sessions[0].Id, instructor.Id, CancellationToken.None);

        Assert.NotNull(targetAttendance);
        Assert.Contains(targetAttendance!.Entries, entry => entry.ParticipantId == jan.Id);
        Assert.Contains(targetAttendance.Entries, entry => entry.ParticipantId == ola.Id);
    }
}

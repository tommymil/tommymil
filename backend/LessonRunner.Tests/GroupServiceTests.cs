using LessonRunner.Application.Groups;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Scheduling;
using LessonRunner.Domain.Users;
using System.Text;
using Xunit;

namespace LessonRunner.Tests;

public sealed class GroupServiceTests
{
    private static Lesson ReadyLesson(string title) => new()
    {
        Title = title,
        Subject = "Scratch",
        Level = "Poziom 1",
        Description = "Opis",
        Status = LessonStatus.Ready
    };

    private static Participant SomeParticipant(string firstName = "Jan", string lastName = "Kowalski") => new()
    {
        FirstName = firstName,
        LastName = lastName
    };

    private static async Task<(GroupService Service, InMemoryLessonRepository Lessons, InMemoryParticipantRepository Participants, User Instructor)> BuildAsync()
    {
        var lessons = new InMemoryLessonRepository();
        var users = new InMemoryUserRepository();
        var participants = new InMemoryParticipantRepository();
        var instructor = new User { Email = "instruktor@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var service = new GroupService(new InMemoryGroupRepository(), lessons, users, participants);
        return (service, lessons, participants, instructor);
    }

    [Fact]
    public async Task SetSessionLinksAsync_OverridesGroupLink_AndFallsBackWhenCleared()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var group = await service.CreateAsync(
            new CreateGroupDto(
                "Grupa",
                instructor.Id,
                [lesson.Id],
                DateTimeOffset.UtcNow,
                [],
                MeetingUrl: "https://meet.google.com/grupowy-link"),
            CancellationToken.None);
        var sessionId = group.Sessions[0].Id;

        Assert.Equal("https://meet.google.com/grupowy-link", group.Sessions[0].MeetingUrl);
        Assert.Null(group.Sessions[0].SessionMeetingUrl);

        var withOwnLink = await service.SetSessionLinksAsync(
            group.Id,
            sessionId,
            new UpdateSessionLinksDto("https://zoom.us/j/zastepstwo", "https://nagrania.example.com/1"),
            null,
            CancellationToken.None);

        Assert.NotNull(withOwnLink);
        Assert.Equal("https://zoom.us/j/zastepstwo", withOwnLink!.MeetingUrl);
        Assert.Equal("https://zoom.us/j/zastepstwo", withOwnLink.SessionMeetingUrl);
        Assert.Equal("https://nagrania.example.com/1", withOwnLink.RecordingUrl);

        // Wyczyszczenie linku terminu ma przywrócić link grupy, a nie zostawić pustkę.
        var cleared = await service.SetSessionLinksAsync(
            group.Id,
            sessionId,
            new UpdateSessionLinksDto(null, null),
            null,
            CancellationToken.None);

        Assert.NotNull(cleared);
        Assert.Equal("https://meet.google.com/grupowy-link", cleared!.MeetingUrl);
        Assert.Null(cleared.SessionMeetingUrl);
        Assert.Null(cleared.RecordingUrl);
    }

    [Fact]
    public async Task SetSessionLinksAsync_RejectsLinkThatIsNotHttpAddress()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var group = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow, []),
            CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(() => service.SetSessionLinksAsync(
            group.Id,
            group.Sessions[0].Id,
            new UpdateSessionLinksDto("meet.google.com/bez-schematu", null),
            null,
            CancellationToken.None));
    }

    /// <summary>
    /// Regresja pod „nie dostaliśmy informacji o zmianie": przesunięcie terminu musi zostawić
    /// ślad z poprzednią i nową datą, powodem, autorem i informacją o powiadomieniu opiekunów.
    /// </summary>
    [Fact]
    public async Task RescheduleSessionAsync_WritesChangeHistory()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var pierwotny = new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero);
        var group = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], pierwotny, []),
            CancellationToken.None);

        var nowy = new DateTimeOffset(2026, 6, 17, 18, 0, 0, TimeSpan.Zero);
        await service.RescheduleSessionAsync(
            group.Id,
            group.Sessions[0].Id,
            new RescheduleSessionDto(nowy, Reason: "Instruktor chory", GuardiansNotified: true),
            instructor.Id,
            CancellationToken.None);

        var history = await service.GetSessionHistoryAsync(group.Id, CancellationToken.None);

        Assert.NotNull(history);
        var wpis = Assert.Single(history!, item => item.ChangeType == "rescheduled");
        Assert.Equal(pierwotny, wpis.PreviousScheduledAt);
        Assert.Equal(nowy, wpis.NewScheduledAt);
        Assert.Equal("Instruktor chory", wpis.Reason);
        Assert.Equal(instructor.Id, wpis.ChangedByUserId);
        Assert.Equal(instructor.DisplayName, wpis.ChangedByName);
        Assert.True(wpis.GuardiansNotified);
        Assert.Equal(1, wpis.SequenceNumber);
    }

    [Fact]
    public async Task SetSessionStatusAsync_RecordsTransitionAndKeepsAttendanceOutOfFrequency()
    {
        var (service, lessons, participants, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);
        var jan = SomeParticipant();
        await participants.AddAsync(jan, CancellationToken.None);

        var group = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow, [jan.Id]),
            CancellationToken.None);

        var updated = await service.SetSessionStatusAsync(
            group.Id,
            group.Sessions[0].Id,
            new SetSessionStatusDto("technicalfailure", "Padł internet u prowadzącego"),
            instructor.Id,
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("technicalfailure", updated!.Status);
        Assert.Equal("Przerwane technicznie", updated.StatusLabel);

        // Awaria po naszej stronie nie może obniżać frekwencji dziecka.
        var summary = await service.GetAttendanceSummaryAsync(group.Id, CancellationToken.None);
        Assert.Equal(0, summary!.HeldSessions);

        var history = await service.GetSessionHistoryAsync(group.Id, CancellationToken.None);
        var wpis = Assert.Single(history!, item => item.ChangeType == "statuschanged");
        Assert.Equal("Zaplanowane → Przerwane technicznie", wpis.Details);
        Assert.Equal("Padł internet u prowadzącego", wpis.Reason);
    }

    [Fact]
    public async Task SetSessionStatusAsync_RejectsInProgressAndUnknownStatus()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var group = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow, []),
            CancellationToken.None);
        var sessionId = group.Sessions[0].Id;

        // „W toku" ustawia się wyłącznie przez rozpoczęcie zajęć - inaczej rozjechałyby się timery.
        await Assert.ThrowsAsync<ArgumentException>(() => service.SetSessionStatusAsync(
            group.Id, sessionId, new SetSessionStatusDto("inprogress"), null, CancellationToken.None));

        await Assert.ThrowsAsync<ArgumentException>(() => service.SetSessionStatusAsync(
            group.Id, sessionId, new SetSessionStatusDto("cos-czego-nie-ma"), null, CancellationToken.None));
    }

    [Fact]
    public async Task CancelSessionAsync_DistinguishesWhoCancelled()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var group = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow, []),
            CancellationToken.None);

        var byParent = await service.CancelSessionAsync(
            group.Id,
            group.Sessions[0].Id,
            new CancelSessionDto("Dziecko chore", CancelledBy: "parent"),
            instructor.Id,
            CancellationToken.None);

        Assert.Equal("cancelledbyparent", byParent!.Status);
        Assert.Equal("Odwołane przez rodzica", byParent.StatusLabel);
    }

    [Fact]
    public async Task CancelSessionAsync_WritesReasonToHistory()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var group = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow, []),
            CancellationToken.None);

        await service.CancelSessionAsync(
            group.Id,
            group.Sessions[0].Id,
            new CancelSessionDto("Awaria prądu w szkole", GuardiansNotified: true),
            instructor.Id,
            CancellationToken.None);

        var history = await service.GetSessionHistoryAsync(group.Id, CancellationToken.None);
        var wpis = Assert.Single(history!, item => item.ChangeType == "cancelled");

        Assert.Equal("Awaria prądu w szkole", wpis.Reason);
        Assert.True(wpis.GuardiansNotified);
    }

    [Fact]
    public async Task CreateAsync_GeneratesWeeklySessionsInLessonOrder()
    {
        var (service, lessons, participants, instructor) = await BuildAsync();
        var l1 = ReadyLesson("L1");
        var l2 = ReadyLesson("L2");
        var l3 = ReadyLesson("L3");
        await lessons.AddAsync(l1, CancellationToken.None);
        await lessons.AddAsync(l2, CancellationToken.None);
        await lessons.AddAsync(l3, CancellationToken.None);

        var participant = SomeParticipant();
        await participants.AddAsync(participant, CancellationToken.None);

        var first = new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero);
        var dto = new CreateGroupDto(
            "Grupa A",
            instructor.Id,
            [l1.Id, l2.Id, l3.Id],
            first,
            [participant.Id]);

        var details = await service.CreateAsync(dto, CancellationToken.None);

        Assert.Equal(3, details.Sessions.Count);
        Assert.Equal([1, 2, 3], details.Sessions.Select(session => session.SequenceNumber));
        Assert.Equal(first, details.Sessions[0].ScheduledAt);
        Assert.Equal(first.AddDays(7), details.Sessions[1].ScheduledAt);
        Assert.Equal(first.AddDays(14), details.Sessions[2].ScheduledAt);
        Assert.Equal([l1.Id, l2.Id, l3.Id], details.Sessions.Select(session => session.LessonId));
        Assert.All(details.Sessions, session => Assert.Equal("planned", session.Status));
        Assert.Single(details.Participants);
        Assert.Equal(instructor.Email, details.InstructorEmail);
    }

    [Fact]
    public async Task CreateAsync_KeepsWallClockTimeAcrossDaylightSavingChange()
    {
        var warsaw = TryGetWarsawTimeZone();
        if (warsaw is null)
        {
            return; // środowisko bez bazy stref czasowych - test nie ma czego weryfikować.
        }

        var (service, lessons, _, instructor) = await BuildAsync();
        var l1 = ReadyLesson("L1");
        var l2 = ReadyLesson("L2");
        await lessons.AddAsync(l1, CancellationToken.None);
        await lessons.AddAsync(l2, CancellationToken.None);

        // Pierwszy termin: czwartek 26.03.2026, 18:00 czasu warszawskiego (przed zmianą czasu 29.03).
        var firstLocal = new DateTime(2026, 3, 26, 18, 0, 0);
        var first = new DateTimeOffset(firstLocal, warsaw.GetUtcOffset(firstLocal));

        var dto = new CreateGroupDto("Grupa", instructor.Id, [l1.Id, l2.Id], first, []);
        var details = await service.CreateAsync(dto, CancellationToken.None);

        // Oba terminy muszą wypaść o 18:00 lokalnie, mimo że drugi jest już po zmianie czasu.
        Assert.All(details.Sessions, session =>
        {
            var local = TimeZoneInfo.ConvertTime(session.ScheduledAt, warsaw);
            Assert.Equal(18, local.Hour);
            Assert.Equal(0, local.Minute);
        });

        // Odstęp w czasie UTC to 6 dni 23 h (a nie równe 7 dni), bo zegar cofnął się o godzinę.
        Assert.Equal(
            TimeSpan.FromDays(7) - TimeSpan.FromHours(1),
            details.Sessions[1].ScheduledAt - details.Sessions[0].ScheduledAt);
    }

    [Fact]
    public async Task GetAttendanceSummaryAsync_DoesNotPenalizeParticipantJoiningMidCourse()
    {
        var lessons = new InMemoryLessonRepository();
        var l1 = ReadyLesson("L1");
        var l2 = ReadyLesson("L2");
        await lessons.AddAsync(l1, CancellationToken.None);
        await lessons.AddAsync(l2, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var participantRepository = new InMemoryParticipantRepository();
        var jan = SomeParticipant("Jan", "Kowalski");
        var ola = SomeParticipant("Ola", "Nowak");
        await participantRepository.AddAsync(jan, CancellationToken.None);
        await participantRepository.AddAsync(ola, CancellationToken.None);

        var repository = new InMemoryGroupRepository();
        var groupService = new GroupService(repository, lessons, users, participantRepository);
        var sessionService = new SessionService(repository, lessons, participantRepository);

        // Na starcie kursu zapisany jest tylko Jan.
        var details = await groupService.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [l1.Id, l2.Id], new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero), [jan.Id]),
            CancellationToken.None);

        // Termin 1: obecny tylko Jan (Ola jeszcze nie dołączyła).
        var firstSession = details.Sessions[0].Id;
        await sessionService.StartAsync(firstSession, instructor.Id, CancellationToken.None);
        await sessionService.SaveAttendanceAsync(
            firstSession,
            instructor.Id,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(jan.Id, true)]),
            CancellationToken.None);
        await sessionService.FinishAsync(firstSession, instructor.Id, new FinishSessionDto(null), CancellationToken.None);

        // Ola dołącza dopiero teraz.
        await repository.AddEnrollmentAsync(new GroupEnrollment { GroupId = details.Id, ParticipantId = ola.Id }, CancellationToken.None);

        // Termin 2: oboje obecni.
        var secondSession = details.Sessions[1].Id;
        await sessionService.StartAsync(secondSession, instructor.Id, CancellationToken.None);
        await sessionService.SaveAttendanceAsync(
            secondSession,
            instructor.Id,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(jan.Id, true), new SaveAttendanceEntryDto(ola.Id, true)]),
            CancellationToken.None);
        await sessionService.FinishAsync(secondSession, instructor.Id, new FinishSessionDto(null), CancellationToken.None);

        var summary = await groupService.GetAttendanceSummaryAsync(details.Id, CancellationToken.None);

        Assert.NotNull(summary);
        Assert.Equal(2, summary!.HeldSessions);

        var janRow = summary.Participants.Single(item => item.ParticipantId == jan.Id);
        Assert.Equal(2, janRow.HeldCount);
        Assert.Equal(100, janRow.RatePercent);

        // Ola liczona tylko za termin, w którym była na liście - 1/1 = 100%, a nie 1/2 = 50%.
        var olaRow = summary.Participants.Single(item => item.ParticipantId == ola.Id);
        Assert.Equal(1, olaRow.HeldCount);
        Assert.Equal(1, olaRow.PresentCount);
        Assert.Equal(100, olaRow.RatePercent);
    }

    private static TimeZoneInfo? TryGetWarsawTimeZone()
    {
        foreach (var id in new[] { "Europe/Warsaw", "Central European Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return null;
    }

    [Fact]
    public async Task CreateAsync_RejectsLessonThatIsNotReady()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var draft = new Lesson { Title = "Szkic", Subject = "Scratch", Level = "P1", Description = "d", Status = LessonStatus.Draft };
        await lessons.AddAsync(draft, CancellationToken.None);

        var dto = new CreateGroupDto("Grupa", instructor.Id, [draft.Id], DateTimeOffset.UtcNow, []);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_RejectsUnknownInstructor()
    {
        var (service, lessons, _, _) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var dto = new CreateGroupDto("Grupa", Guid.NewGuid(), [lesson.Id], DateTimeOffset.UtcNow, []);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_RejectsUnknownParticipant()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var dto = new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow, [Guid.NewGuid()]);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ChangesName()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);
        var details = await service.CreateAsync(
            new CreateGroupDto("Stara nazwa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow, []),
            CancellationToken.None);

        var updated = await service.UpdateAsync(details.Id, new UpdateGroupDto("Nowa nazwa", instructor.Id), CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("Nowa nazwa", updated!.Name);
    }

    [Fact]
    public async Task AddSessionAsync_AppendsSessionWithNextSequence()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var l1 = ReadyLesson("L1");
        var l2 = ReadyLesson("L2");
        await lessons.AddAsync(l1, CancellationToken.None);
        await lessons.AddAsync(l2, CancellationToken.None);

        var details = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [l1.Id], new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero), []),
            CancellationToken.None);

        var added = await service.AddSessionAsync(
            details.Id,
            new AddSessionDto(l2.Id, new DateTimeOffset(2026, 6, 22, 16, 0, 0, TimeSpan.Zero)),
            null,
            CancellationToken.None);

        Assert.NotNull(added);
        Assert.Equal(2, added!.SequenceNumber);
        Assert.Equal(l2.Id, added.LessonId);

        var reloaded = await service.GetDetailsAsync(details.Id, CancellationToken.None);
        Assert.Equal(2, reloaded!.Sessions.Count);
    }

    [Fact]
    public async Task AddSessionAsync_RejectsNonReadyLesson()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var ready = ReadyLesson("L1");
        var draft = new Lesson { Title = "Szkic", Subject = "Scratch", Level = "P1", Description = "d", Status = LessonStatus.Draft };
        await lessons.AddAsync(ready, CancellationToken.None);
        await lessons.AddAsync(draft, CancellationToken.None);

        var details = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [ready.Id], DateTimeOffset.UtcNow, []),
            CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.AddSessionAsync(details.Id, new AddSessionDto(draft.Id, DateTimeOffset.UtcNow), null, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_RejectsInstructorCollision()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var groups = new InMemoryGroupRepository();
        var service = new GroupService(groups, lessons, users, new InMemoryParticipantRepository());
        var at = new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero);
        await service.CreateAsync(new CreateGroupDto("Pierwsza", instructor.Id, [lesson.Id], at, []), CancellationToken.None);

        await Assert.ThrowsAsync<SchedulingConflictException>(() =>
            service.CreateAsync(new CreateGroupDto("Druga", instructor.Id, [lesson.Id], at, []), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_RejectsLocationCollision()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var firstInstructor = new User { Email = "a@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        var secondInstructor = new User { Email = "b@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(firstInstructor, CancellationToken.None);
        await users.AddAsync(secondInstructor, CancellationToken.None);

        var scheduling = new InMemorySchedulingRepository();
        var room = new Location { Name = "Sala A" };
        await scheduling.AddLocationAsync(room, CancellationToken.None);

        var service = new GroupService(new InMemoryGroupRepository(), lessons, users, new InMemoryParticipantRepository(), null, scheduling);
        var at = new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero);
        await service.CreateAsync(new CreateGroupDto("Pierwsza", firstInstructor.Id, [lesson.Id], at, [], null, room.Id), CancellationToken.None);

        await Assert.ThrowsAsync<SchedulingConflictException>(() =>
            service.CreateAsync(new CreateGroupDto("Druga", secondInstructor.Id, [lesson.Id], at, [], null, room.Id), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_SkipsHolidayDuringWeeklyGeneration()
    {
        var lessons = new InMemoryLessonRepository();
        var l1 = ReadyLesson("L1");
        var l2 = ReadyLesson("L2");
        await lessons.AddAsync(l1, CancellationToken.None);
        await lessons.AddAsync(l2, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var scheduling = new InMemorySchedulingRepository();
        await scheduling.AddHolidayAsync(new Holiday { Date = new DateOnly(2026, 6, 22), Name = "Dzien wolny" }, CancellationToken.None);

        var service = new GroupService(new InMemoryGroupRepository(), lessons, users, new InMemoryParticipantRepository(), null, scheduling);
        var first = new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero);

        var details = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [l1.Id, l2.Id], first, []),
            CancellationToken.None);

        Assert.Equal(first, details.Sessions[0].ScheduledAt);
        Assert.Equal(new DateTimeOffset(2026, 6, 23, 18, 0, 0, TimeSpan.FromHours(2)), details.Sessions[1].ScheduledAt);
    }

    [Fact]
    public async Task CreateAsync_WaitlistsParticipantsOverCapacity()
    {
        var (service, lessons, participants, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        var jan = SomeParticipant("Jan", "Kowalski");
        var ola = SomeParticipant("Ola", "Nowak");
        await lessons.AddAsync(lesson, CancellationToken.None);
        await participants.AddAsync(jan, CancellationToken.None);
        await participants.AddAsync(ola, CancellationToken.None);

        var details = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow, [jan.Id, ola.Id], null, null, 1),
            CancellationToken.None);

        Assert.Equal(1, details.Capacity);
        Assert.Equal("enrolled", details.Participants.Single(participant => participant.Id == jan.Id).EnrollmentStatus);
        Assert.Equal("waitlisted", details.Participants.Single(participant => participant.Id == ola.Id).EnrollmentStatus);
    }

    [Fact]
    public async Task SubstituteInstructor_CanOpenAndStartSession()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var owner = new User { Email = "owner@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        var substitute = new User { Email = "sub@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(owner, CancellationToken.None);
        await users.AddAsync(substitute, CancellationToken.None);

        var participantRepository = new InMemoryParticipantRepository();
        var repository = new InMemoryGroupRepository();
        var groupService = new GroupService(repository, lessons, users, participantRepository);
        var sessionService = new SessionService(repository, lessons, participantRepository, users);
        var details = await groupService.CreateAsync(
            new CreateGroupDto("Grupa", owner.Id, [lesson.Id], DateTimeOffset.UtcNow, []),
            CancellationToken.None);

        await groupService.SetSubstituteInstructorAsync(
            details.Id,
            details.Sessions[0].Id,
            new SetSubstituteInstructorDto(substitute.Id),
            null,
            CancellationToken.None);

        var session = await sessionService.GetSessionAsync(details.Sessions[0].Id, substitute.Id, CancellationToken.None);
        var attendance = await sessionService.StartAsync(details.Sessions[0].Id, substitute.Id, CancellationToken.None);

        Assert.NotNull(session);
        Assert.NotNull(attendance);
    }

    [Fact]
    public async Task RescheduleSessionAsync_MovesPlannedTerm()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var details = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero), []),
            CancellationToken.None);

        var newDate = new DateTimeOffset(2026, 7, 1, 17, 0, 0, TimeSpan.Zero);
        var updated = await service.RescheduleSessionAsync(
            details.Id,
            details.Sessions[0].Id,
            new RescheduleSessionDto(newDate),
            null,
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(newDate, updated!.ScheduledAt);
    }

    [Fact]
    public async Task GetAttendanceSummaryAsync_CountsPresenceOverCompletedSessions()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var participantRepository = new InMemoryParticipantRepository();
        var jan = SomeParticipant("Jan", "Kowalski");
        var ola = SomeParticipant("Ola", "Nowak");
        await participantRepository.AddAsync(jan, CancellationToken.None);
        await participantRepository.AddAsync(ola, CancellationToken.None);

        var repository = new InMemoryGroupRepository();
        var groupService = new GroupService(repository, lessons, users, participantRepository);
        var sessionService = new SessionService(repository, lessons, participantRepository);

        var details = await groupService.CreateAsync(
            new CreateGroupDto(
                "Grupa",
                instructor.Id,
                [lesson.Id],
                new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero),
                [jan.Id, ola.Id]),
            CancellationToken.None);

        var sessionId = details.Sessions[0].Id;
        var attendance = await sessionService.StartAsync(sessionId, instructor.Id, CancellationToken.None);
        var presentParticipant = attendance!.Entries[0].ParticipantId;
        await sessionService.SaveAttendanceAsync(
            sessionId,
            instructor.Id,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(presentParticipant, true)]),
            CancellationToken.None);
        await sessionService.FinishAsync(sessionId, instructor.Id, new FinishSessionDto(null), CancellationToken.None);

        var summary = await groupService.GetAttendanceSummaryAsync(details.Id, CancellationToken.None);

        Assert.NotNull(summary);
        Assert.Equal(1, summary!.HeldSessions);
        Assert.Equal(1, summary.Participants.Single(item => item.ParticipantId == presentParticipant).PresentCount);
        Assert.Equal(100, summary.Participants.Single(item => item.ParticipantId == presentParticipant).RatePercent);
        Assert.Equal(0, summary.Participants.Single(item => item.ParticipantId != presentParticipant).PresentCount);
    }

    [Fact]
    public async Task GetAttendanceSummaryAsync_KeepsHistory_AfterParticipantUnenrolled()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var participantRepository = new InMemoryParticipantRepository();
        var jan = SomeParticipant("Jan", "Kowalski");
        await participantRepository.AddAsync(jan, CancellationToken.None);

        var repository = new InMemoryGroupRepository();
        var groupService = new GroupService(repository, lessons, users, participantRepository);
        var sessionService = new SessionService(repository, lessons, participantRepository);

        var details = await groupService.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero), [jan.Id]),
            CancellationToken.None);

        var sessionId = details.Sessions[0].Id;
        await sessionService.StartAsync(sessionId, instructor.Id, CancellationToken.None);
        await sessionService.SaveAttendanceAsync(
            sessionId,
            instructor.Id,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(jan.Id, true)]),
            CancellationToken.None);
        await sessionService.FinishAsync(sessionId, instructor.Id, new FinishSessionDto(null), CancellationToken.None);

        // Wypisanie z grupy - frekwencja powinna zostać widoczna mimo to.
        await repository.RemoveEnrollmentAsync(details.Id, jan.Id, CancellationToken.None);

        var summary = await groupService.GetAttendanceSummaryAsync(details.Id, CancellationToken.None);

        Assert.NotNull(summary);
        Assert.Single(summary!.Participants);
        Assert.Equal(1, summary.Participants[0].PresentCount);
    }

    [Fact]
    public async Task GetAttendanceSummaryAsync_CountsCompletedMakeupAsPresence()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var participantRepository = new InMemoryParticipantRepository();
        var jan = SomeParticipant("Jan", "Kowalski");
        var ola = SomeParticipant("Ola", "Nowak");
        await participantRepository.AddAsync(jan, CancellationToken.None);
        await participantRepository.AddAsync(ola, CancellationToken.None);

        var repository = new InMemoryGroupRepository();
        var groupService = new GroupService(repository, lessons, users, participantRepository);
        var sessionService = new SessionService(repository, lessons, participantRepository);

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
        await sessionService.FinishAsync(source.Sessions[0].Id, instructor.Id, new FinishSessionDto(null), CancellationToken.None);

        await sessionService.StartAsync(target.Sessions[0].Id, instructor.Id, CancellationToken.None);
        await sessionService.SaveAttendanceAsync(
            target.Sessions[0].Id,
            instructor.Id,
            new SaveAttendanceDto([
                new SaveAttendanceEntryDto(jan.Id, true),
                new SaveAttendanceEntryDto(ola.Id, true)
            ]),
            CancellationToken.None);
        await sessionService.FinishAsync(target.Sessions[0].Id, instructor.Id, new FinishSessionDto(null), CancellationToken.None);

        var summary = await groupService.GetAttendanceSummaryAsync(source.Id, CancellationToken.None);

        Assert.NotNull(summary);
        var janRow = summary!.Participants.Single(item => item.ParticipantId == jan.Id);
        Assert.Equal(1, janRow.HeldCount);
        Assert.Equal(1, janRow.PresentCount);
        Assert.Equal(100, janRow.RatePercent);
    }

    [Fact]
    public async Task ExportAttendanceSummaryAsync_ReturnsCsvWithBomAndSummaryRows()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var participantRepository = new InMemoryParticipantRepository();
        var jan = SomeParticipant("Jan", "Kowalski");
        await participantRepository.AddAsync(jan, CancellationToken.None);

        var repository = new InMemoryGroupRepository();
        var groupService = new GroupService(repository, lessons, users, participantRepository);
        var sessionService = new SessionService(repository, lessons, participantRepository);

        var details = await groupService.CreateAsync(
            new CreateGroupDto("Grupa CSV", instructor.Id, [lesson.Id], new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero), [jan.Id]),
            CancellationToken.None);
        var sessionId = details.Sessions[0].Id;
        await sessionService.StartAsync(sessionId, instructor.Id, CancellationToken.None);
        await sessionService.SaveAttendanceAsync(
            sessionId,
            instructor.Id,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(jan.Id, true)]),
            CancellationToken.None);
        await sessionService.FinishAsync(sessionId, instructor.Id, new FinishSessionDto(null), CancellationToken.None);

        var export = await groupService.ExportAttendanceSummaryAsync(details.Id, "csv", CancellationToken.None);

        Assert.NotNull(export);
        Assert.Equal([0xEF, 0xBB, 0xBF], export!.Content.Take(3).ToArray());
        var csv = Encoding.UTF8.GetString(export.Content.Skip(3).ToArray());
        Assert.Contains("Uczestnik;Obecności;Liczone zajęcia;Frekwencja (%)", csv);
        Assert.Contains("Jan Kowalski;1;1;100", csv);
    }

    [Fact]
    public async Task ExportSessionAttendanceAsync_ReturnsCsvListForSingleSession()
    {
        var lessons = new InMemoryLessonRepository();
        var lesson = ReadyLesson("Robotyka");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@x.pl", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);

        var participantRepository = new InMemoryParticipantRepository();
        var jan = SomeParticipant("Jan", "Kowalski");
        var ola = SomeParticipant("Ola", "Nowak");
        await participantRepository.AddAsync(jan, CancellationToken.None);
        await participantRepository.AddAsync(ola, CancellationToken.None);

        var repository = new InMemoryGroupRepository();
        var groupService = new GroupService(repository, lessons, users, participantRepository);
        var sessionService = new SessionService(repository, lessons, participantRepository);

        var details = await groupService.CreateAsync(
            new CreateGroupDto("Grupa CSV", instructor.Id, [lesson.Id], new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero), [jan.Id, ola.Id]),
            CancellationToken.None);
        var sessionId = details.Sessions[0].Id;
        await sessionService.StartAsync(sessionId, instructor.Id, CancellationToken.None);
        await sessionService.SaveAttendanceAsync(
            sessionId,
            instructor.Id,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(jan.Id, true), new SaveAttendanceEntryDto(ola.Id, false)]),
            CancellationToken.None);

        var export = await groupService.ExportSessionAttendanceAsync(details.Id, sessionId, "csv", CancellationToken.None);

        Assert.NotNull(export);
        var csv = Encoding.UTF8.GetString(export!.Content.Skip(3).ToArray());
        // Eksport niesie teraz również pełny status i notatkę instruktora, nie tylko „tak/nie".
        Assert.Contains("Lp.;Uczestnik;Obecny;Status;Notatka;Podpis", csv);
        Assert.Contains("1;Jan Kowalski;tak;Obecny;;", csv);
        Assert.Contains("2;Ola Nowak;nie;Nieobecność niezgłoszona;;", csv);
    }

    [Fact]
    public async Task GetInstructorsAsync_ReturnsInstructorAccounts()
    {
        var (service, _, _, instructor) = await BuildAsync();

        var instructors = await service.GetInstructorsAsync(CancellationToken.None);

        Assert.Contains(instructors, item => item.Id == instructor.Id && item.Email == instructor.Email);
    }

    [Fact]
    public async Task CreateAsync_PersistsMeetingUrl_AndExposesItOnSessions()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var details = await service.CreateAsync(
            new CreateGroupDto(
                "Grupa online",
                instructor.Id,
                [lesson.Id],
                new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.Zero),
                [],
                MeetingUrl: "https://zoom.us/j/123456789"),
            CancellationToken.None);

        Assert.Equal("https://zoom.us/j/123456789", details.MeetingUrl);
        Assert.All(details.Sessions, session => Assert.Equal("https://zoom.us/j/123456789", session.MeetingUrl));

        var reloaded = await service.GetDetailsAsync(details.Id, CancellationToken.None);
        Assert.Equal("https://zoom.us/j/123456789", reloaded!.MeetingUrl);
    }

    [Fact]
    public async Task UpdateAsync_ChangesMeetingUrl()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);
        var details = await service.CreateAsync(
            new CreateGroupDto("Grupa", instructor.Id, [lesson.Id], DateTimeOffset.UtcNow, []),
            CancellationToken.None);

        var updated = await service.UpdateAsync(
            details.Id,
            new UpdateGroupDto("Grupa", instructor.Id, MeetingUrl: "https://meet.google.com/abc-defg-hij"),
            CancellationToken.None);

        Assert.Equal("https://meet.google.com/abc-defg-hij", updated!.MeetingUrl);
    }

    [Fact]
    public async Task CreateAsync_RejectsInvalidMeetingUrl()
    {
        var (service, lessons, _, instructor) = await BuildAsync();
        var lesson = ReadyLesson("L1");
        await lessons.AddAsync(lesson, CancellationToken.None);

        var dto = new CreateGroupDto(
            "Grupa",
            instructor.Id,
            [lesson.Id],
            DateTimeOffset.UtcNow,
            [],
            MeetingUrl: "nie-adres");

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto, CancellationToken.None));
    }
}

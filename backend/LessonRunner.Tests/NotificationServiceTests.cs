using LessonRunner.Application.Groups;
using LessonRunner.Application.Notifications;
using LessonRunner.Application.Scheduling;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Participants;
using Xunit;

namespace LessonRunner.Tests;

public sealed class NotificationServiceTests
{
    /// <summary>
    /// Regresja: przeglądarka wysyła termin przez `toISOString()`, więc w bazie leży offset `Z`.
    /// Wcześniej treść przypomnienia powstawała z `ScheduledAt.ToString("HH:mm")`, czyli
    /// drukowała godzinę UTC — rodzic dostawał termin o dwie godziny wcześniejszy niż faktyczny,
    /// a w panelu i w kalendarzu widniała godzina poprawna.
    /// </summary>
    [Fact]
    public async Task SendUpcomingSessionRemindersAsync_UsesSchoolClock_NotTheStoredOffset()
    {
        if (SchoolTime.Zone.BaseUtcOffset == TimeSpan.Zero)
        {
            return; // środowisko bez bazy stref czasowych - test nie ma czego weryfikować.
        }

        var notifications = new InMemoryNotificationRepository();
        var sender = new FakeEmailSender();
        var groups = new InMemoryGroupRepository();
        var participants = new InMemoryParticipantRepository();
        var lessons = new InMemoryLessonRepository();

        var lesson = new Lesson
        {
            Title = "Scratch",
            Subject = "Scratch",
            Level = "P1",
            Description = "Opis",
            Status = LessonStatus.Ready
        };
        await lessons.AddAsync(lesson, CancellationToken.None);

        var participant = new Participant
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            GuardianEmail = "rodzic@example.com",
            DataProcessingConsentAt = DateTimeOffset.UtcNow
        };
        await participants.AddAsync(participant, CancellationToken.None);

        // Dokładnie tak, jak zapisuje to panel: moment w przyszłości z offsetem zerowym.
        // Dwie godziny naprzód mieszczą się w domyślnym oknie przypomnień (24 h).
        var scheduledAt = new DateTimeOffset(DateTime.UtcNow.AddHours(2), TimeSpan.Zero);

        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = Guid.NewGuid(),
            Enrollments =
            [
                new GroupEnrollment { ParticipantId = participant.Id, Status = EnrollmentStatus.Enrolled }
            ],
            Sessions =
            [
                new ScheduledSession { LessonId = lesson.Id, ScheduledAt = scheduledAt, SequenceNumber = 1 }
            ]
        };
        group.Enrollments[0].GroupId = group.Id;
        group.Sessions[0].GroupId = group.Id;
        await groups.AddAsync(group, CancellationToken.None);

        var service = new NotificationService(notifications, sender, groups, participants, lessons);
        await service.SendUpcomingSessionRemindersAsync(CancellationToken.None);

        var body = Assert.Single(sender.Messages).Body;
        var schoolClock = SchoolTime.FormatDateTime(scheduledAt);
        var storedOffsetClock = scheduledAt.ToString("yyyy-MM-dd HH:mm");

        Assert.NotEqual(storedOffsetClock, schoolClock);
        Assert.Contains(schoolClock, body);
        Assert.DoesNotContain(storedOffsetClock, body);
    }

    [Fact]
    public async Task NotifyAbsencesAsync_SendsOnlyWithGuardianEmailAndConsent_AndDeduplicates()
    {
        var notifications = new InMemoryNotificationRepository();
        var sender = new FakeEmailSender();
        var groups = new InMemoryGroupRepository();
        var participants = new InMemoryParticipantRepository();
        var lessons = new InMemoryLessonRepository();
        var lesson = new Lesson
        {
            Title = "Scratch",
            Subject = "Scratch",
            Level = "P1",
            Description = "Opis",
            Status = LessonStatus.Ready
        };
        await lessons.AddAsync(lesson, CancellationToken.None);
        var withConsent = new Participant
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            GuardianEmail = "rodzic@example.com",
            DataProcessingConsentAt = DateTimeOffset.UtcNow
        };
        var withoutConsent = new Participant
        {
            FirstName = "Ola",
            LastName = "Nowak",
            GuardianEmail = "brak@example.com"
        };
        await participants.AddAsync(withConsent, CancellationToken.None);
        await participants.AddAsync(withoutConsent, CancellationToken.None);
        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = Guid.NewGuid(),
            Enrollments =
            [
                new GroupEnrollment { ParticipantId = withConsent.Id, Status = EnrollmentStatus.Enrolled },
                new GroupEnrollment { ParticipantId = withoutConsent.Id, Status = EnrollmentStatus.Enrolled }
            ],
            Sessions =
            [
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow,
                    SequenceNumber = 1
                }
            ]
        };
        foreach (var enrollment in group.Enrollments)
        {
            enrollment.GroupId = group.Id;
        }
        group.Sessions[0].GroupId = group.Id;
        await groups.AddAsync(group, CancellationToken.None);
        var service = new NotificationService(notifications, sender, groups, participants, lessons);

        await service.NotifyAbsencesAsync(group.Sessions[0].Id, [withConsent.Id, withoutConsent.Id], CancellationToken.None);
        await service.NotifyAbsencesAsync(group.Sessions[0].Id, [withConsent.Id], CancellationToken.None);

        Assert.Single(sender.Messages);
        Assert.Equal("rodzic@example.com", sender.Messages[0].ToEmail);
        var logs = await notifications.ListLogsAsync(10, CancellationToken.None);
        Assert.Single(logs);
        Assert.Equal("sent", logs[0].Status);
    }

    /// <summary>
    /// Regresja: `StartAsync` tworzy listę obecności z wszystkimi niezaznaczonymi. Gdyby
    /// powiadomienia szły w tym momencie, każdy opiekun dostawałby e-mail "dziecko było
    /// nieobecne" na starcie zajęć - a deduplikacja blokowałaby późniejszą korektę.
    /// Wysyłka ma nastąpić dopiero przy zamknięciu terminu i objąć wyłącznie faktycznie
    /// nieobecnych.
    /// </summary>
    [Fact]
    public async Task Absences_AreNotSentOnStart_ButOnFinish_AndOnlyForAbsentChildren()
    {
        var notifications = new InMemoryNotificationRepository();
        var sender = new FakeEmailSender();
        var groups = new InMemoryGroupRepository();
        var participants = new InMemoryParticipantRepository();
        var lessons = new InMemoryLessonRepository();

        var lesson = new Lesson
        {
            Title = "Scratch",
            Subject = "Scratch",
            Level = "P1",
            Description = "Opis",
            Status = LessonStatus.Ready
        };
        await lessons.AddAsync(lesson, CancellationToken.None);

        var obecny = new Participant
        {
            FirstName = "Jan",
            LastName = "Obecny",
            GuardianEmail = "jan@example.com",
            DataProcessingConsentAt = DateTimeOffset.UtcNow
        };
        var nieobecny = new Participant
        {
            FirstName = "Ola",
            LastName = "Nieobecna",
            GuardianEmail = "ola@example.com",
            DataProcessingConsentAt = DateTimeOffset.UtcNow
        };
        await participants.AddAsync(obecny, CancellationToken.None);
        await participants.AddAsync(nieobecny, CancellationToken.None);

        var instructorId = Guid.NewGuid();
        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = instructorId,
            Enrollments =
            [
                new GroupEnrollment { ParticipantId = obecny.Id, Status = EnrollmentStatus.Enrolled },
                new GroupEnrollment { ParticipantId = nieobecny.Id, Status = EnrollmentStatus.Enrolled }
            ],
            Sessions =
            [
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow,
                    SequenceNumber = 1
                }
            ]
        };
        foreach (var enrollment in group.Enrollments)
        {
            enrollment.GroupId = group.Id;
        }
        group.Sessions[0].GroupId = group.Id;
        await groups.AddAsync(group, CancellationToken.None);

        var notificationService = new NotificationService(notifications, sender, groups, participants, lessons);
        var sessionService = new SessionService(groups, lessons, participants, null, notificationService);
        var sessionId = group.Sessions[0].Id;

        await sessionService.StartAsync(sessionId, instructorId, CancellationToken.None);
        Assert.Empty(sender.Messages);

        await sessionService.SaveAttendanceAsync(
            sessionId,
            instructorId,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(obecny.Id, true)]),
            CancellationToken.None);
        Assert.Empty(sender.Messages);

        await sessionService.FinishAsync(sessionId, instructorId, new FinishSessionDto(null), CancellationToken.None);

        Assert.Single(sender.Messages);
        Assert.Equal("ola@example.com", sender.Messages[0].ToEmail);
    }

    /// <summary>
    /// Gdy opiekun sam zgłosił, że dziecka nie będzie, odsyłanie mu informacji o nieobecności
    /// jest szumem. Powiadamiamy wyłącznie o nieobecności niezgłoszonej.
    /// </summary>
    [Fact]
    public async Task ExcusedAbsence_DoesNotTriggerNotification()
    {
        var notifications = new InMemoryNotificationRepository();
        var sender = new FakeEmailSender();
        var groups = new InMemoryGroupRepository();
        var participants = new InMemoryParticipantRepository();
        var lessons = new InMemoryLessonRepository();

        var lesson = new Lesson
        {
            Title = "Scratch",
            Subject = "Scratch",
            Level = "P1",
            Description = "Opis",
            Status = LessonStatus.Ready
        };
        await lessons.AddAsync(lesson, CancellationToken.None);

        var dziecko = new Participant
        {
            FirstName = "Ola",
            LastName = "Zgłoszona",
            GuardianEmail = "ola@example.com",
            DataProcessingConsentAt = DateTimeOffset.UtcNow
        };
        await participants.AddAsync(dziecko, CancellationToken.None);

        var instructorId = Guid.NewGuid();
        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = instructorId,
            Enrollments = [new GroupEnrollment { ParticipantId = dziecko.Id, Status = EnrollmentStatus.Enrolled }],
            Sessions =
            [
                new ScheduledSession { LessonId = lesson.Id, ScheduledAt = DateTimeOffset.UtcNow, SequenceNumber = 1 }
            ]
        };
        group.Enrollments[0].GroupId = group.Id;
        group.Sessions[0].GroupId = group.Id;
        await groups.AddAsync(group, CancellationToken.None);

        var notificationService = new NotificationService(notifications, sender, groups, participants, lessons);
        var sessionService = new SessionService(groups, lessons, participants, null, notificationService);
        var sessionId = group.Sessions[0].Id;

        await sessionService.StartAsync(sessionId, instructorId, CancellationToken.None);
        await sessionService.SaveAttendanceAsync(
            sessionId,
            instructorId,
            new SaveAttendanceDto([new SaveAttendanceEntryDto(dziecko.Id, false, Status: "excusedabsence")]),
            CancellationToken.None);
        await sessionService.FinishAsync(sessionId, instructorId, new FinishSessionDto(null), CancellationToken.None);

        Assert.Empty(sender.Messages);
    }
}

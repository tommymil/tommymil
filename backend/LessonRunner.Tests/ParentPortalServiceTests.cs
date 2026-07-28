using LessonRunner.Application.Billing;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Parents;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Users;
using Xunit;

namespace LessonRunner.Tests;

public sealed class ParentPortalServiceTests
{
    [Fact]
    public async Task GetPortalAsync_ReturnsOnlyLinkedParticipantData()
    {
        var parentLinks = new InMemoryParentPortalRepository();
        var users = new InMemoryUserRepository();
        var participants = new InMemoryParticipantRepository();
        var groups = new InMemoryGroupRepository();
        var lessons = new InMemoryLessonRepository();
        var billing = new InMemoryBillingRepository();
        var courses = new InMemoryCourseRepository();

        var parent = new User { Email = "parent@example.com", PasswordHash = "h", Role = UserRole.Parent };
        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(parent, CancellationToken.None);
        await users.AddAsync(instructor, CancellationToken.None);

        var lesson = new Lesson { Title = "Scratch", Subject = "Scratch", Level = "P1", Description = "Opis", Status = LessonStatus.Ready };
        await lessons.AddAsync(lesson, CancellationToken.None);
        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        var ola = new Participant { FirstName = "Ola", LastName = "Nowak" };
        await participants.AddAsync(jan, CancellationToken.None);
        await participants.AddAsync(ola, CancellationToken.None);

        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = instructor.Id,
            MeetingUrl = "https://meet.google.com/abc-defg-hij",
            Enrollments =
            [
                new GroupEnrollment { ParticipantId = jan.Id, Status = EnrollmentStatus.Enrolled },
                new GroupEnrollment { ParticipantId = ola.Id, Status = EnrollmentStatus.Enrolled }
            ],
            Sessions =
            [
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(2),
                    SequenceNumber = 1,
                    Status = ScheduledSessionStatus.Planned
                },
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(-2),
                    SequenceNumber = 0,
                    Status = ScheduledSessionStatus.Completed,
                    Attendance = [new AttendanceRecord { ParticipantId = jan.Id, Present = true }]
                }
            ]
        };
        foreach (var enrollment in group.Enrollments)
        {
            enrollment.GroupId = group.Id;
        }
        foreach (var session in group.Sessions)
        {
            session.GroupId = group.Id;
            foreach (var record in session.Attendance)
            {
                record.ScheduledSessionId = session.Id;
            }
        }
        await groups.AddAsync(group, CancellationToken.None);

        var service = new ParentPortalService(parentLinks, users, participants, groups, lessons, billing);
        await service.LinkAsync(parent.Id, jan.Id, CancellationToken.None);

        var portal = await service.GetPortalAsync(parent.Id, CancellationToken.None);

        Assert.Single(portal.Children);
        Assert.Equal(jan.Id, portal.Children[0].ParticipantId);
        Assert.DoesNotContain(portal.Children, child => child.ParticipantId == ola.Id);
        Assert.Single(portal.Schedule);
        Assert.Equal("https://meet.google.com/abc-defg-hij", portal.Schedule[0].MeetingUrl);
        Assert.Single(portal.Attendance);
        Assert.Equal(100, portal.Attendance[0].RatePercent);
    }

    [Fact]
    public async Task GetPortalAsync_PrefersSessionMeetingUrl_AndSharesMaterialsOnlyAfterCompletedSession()
    {
        var parentLinks = new InMemoryParentPortalRepository();
        var users = new InMemoryUserRepository();
        var participants = new InMemoryParticipantRepository();
        var groups = new InMemoryGroupRepository();
        var lessons = new InMemoryLessonRepository();
        var billing = new InMemoryBillingRepository();

        var parent = new User { Email = "parent@example.com", PasswordHash = "h", Role = UserRole.Parent };
        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(parent, CancellationToken.None);
        await users.AddAsync(instructor, CancellationToken.None);

        var lesson = new Lesson
        {
            Title = "Scratch",
            Subject = "Scratch",
            Level = "P1",
            Description = "Opis",
            Status = LessonStatus.Ready,
            ProjectFiles = new LessonProjectFiles
            {
                Final = new LessonProjectFile
                {
                    Label = "Projekt końcowy",
                    Url = "/uploads/abc.sb3",
                    FileName = "kotek.sb3",
                    ContentType = "application/zip",
                    SizeBytes = 2048,
                    DownloadToken = "token-do-pobrania-projektu-123"
                }
            }
        };
        await lessons.AddAsync(lesson, CancellationToken.None);

        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        await participants.AddAsync(jan, CancellationToken.None);

        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = instructor.Id,
            MeetingUrl = "https://meet.google.com/grupowy-link",
            Enrollments = [new GroupEnrollment { ParticipantId = jan.Id, Status = EnrollmentStatus.Enrolled }],
            Sessions =
            [
                // Zastępstwo prowadzi u siebie - link terminu ma wygrać z linkiem grupy.
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(2),
                    SequenceNumber = 2,
                    Status = ScheduledSessionStatus.Planned,
                    MeetingUrl = "https://zoom.us/j/zastepstwo"
                },
                new ScheduledSession
                {
                    LessonId = lesson.Id,
                    ScheduledAt = DateTimeOffset.UtcNow.AddDays(-2),
                    SequenceNumber = 1,
                    Status = ScheduledSessionStatus.Completed,
                    RecordingUrl = "https://nagrania.example.com/1",
                    Attendance = [new AttendanceRecord { ParticipantId = jan.Id, Present = true }]
                }
            ]
        };
        group.Enrollments[0].GroupId = group.Id;
        foreach (var session in group.Sessions)
        {
            session.GroupId = group.Id;
            foreach (var record in session.Attendance)
            {
                record.ScheduledSessionId = session.Id;
            }
        }
        await groups.AddAsync(group, CancellationToken.None);

        var service = new ParentPortalService(parentLinks, users, participants, groups, lessons, billing);
        await service.LinkAsync(parent.Id, jan.Id, CancellationToken.None);

        var portal = await service.GetPortalAsync(parent.Id, CancellationToken.None);

        Assert.Single(portal.Schedule);
        Assert.Equal("https://zoom.us/j/zastepstwo", portal.Schedule[0].MeetingUrl);

        // Materiały wyłącznie z terminu zakończonego - przed zajęciami nic nie udostępniamy.
        Assert.Single(portal.Materials);
        Assert.Equal("https://nagrania.example.com/1", portal.Materials[0].RecordingUrl);
        Assert.Single(portal.Materials[0].Files);
        Assert.Equal("kotek.sb3", portal.Materials[0].Files[0].FileName);
        Assert.Equal("/download/lesson-files/token-do-pobrania-projektu-123", portal.Materials[0].Files[0].DownloadUrl);
    }

    [Fact]
    public async Task LinkAsync_RejectsNonParentAccount()
    {
        var users = new InMemoryUserRepository();
        var participants = new InMemoryParticipantRepository();
        var admin = new User { Email = "admin@example.com", PasswordHash = "h", Role = UserRole.Admin };
        var child = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        await users.AddAsync(admin, CancellationToken.None);
        await participants.AddAsync(child, CancellationToken.None);

        var service = new ParentPortalService(
            new InMemoryParentPortalRepository(),
            users,
            participants,
            new InMemoryGroupRepository(),
            new InMemoryLessonRepository(),
            new InMemoryBillingRepository());

        await Assert.ThrowsAsync<ArgumentException>(() => service.LinkAsync(admin.Id, child.Id, CancellationToken.None));
    }
}

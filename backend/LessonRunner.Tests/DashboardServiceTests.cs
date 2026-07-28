using LessonRunner.Application.Dashboard;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Lessons;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Users;
using Xunit;

namespace LessonRunner.Tests;

public sealed class DashboardServiceTests
{
    [Fact]
    public async Task GetAsync_AggregatesAttendanceCapacityAndMakeups()
    {
        var groups = new InMemoryGroupRepository();
        var participants = new InMemoryParticipantRepository();
        var lessons = new InMemoryLessonRepository();
        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);
        var lesson = new Lesson { Title = "Scratch", Subject = "Scratch", Level = "P1", Description = "Opis", Status = LessonStatus.Ready };
        await lessons.AddAsync(lesson, CancellationToken.None);
        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        var ola = new Participant { FirstName = "Ola", LastName = "Nowak" };
        await participants.AddAsync(jan, CancellationToken.None);
        await participants.AddAsync(ola, CancellationToken.None);
        var completed = new ScheduledSession
        {
            LessonId = lesson.Id,
            ScheduledAt = DateTimeOffset.UtcNow.AddDays(-1),
            SequenceNumber = 1,
            Status = ScheduledSessionStatus.Completed,
            Attendance =
            [
                new AttendanceRecord { ParticipantId = jan.Id, Present = true },
                new AttendanceRecord { ParticipantId = ola.Id, Present = false, MakeupRequired = true }
            ]
        };
        var upcoming = new ScheduledSession
        {
            LessonId = lesson.Id,
            ScheduledAt = DateTimeOffset.UtcNow.AddDays(1),
            SequenceNumber = 2,
            Status = ScheduledSessionStatus.Planned
        };
        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = instructor.Id,
            Capacity = 1,
            Enrollments =
            [
                new GroupEnrollment { ParticipantId = jan.Id, Status = EnrollmentStatus.Enrolled },
                new GroupEnrollment { ParticipantId = ola.Id, Status = EnrollmentStatus.Waitlisted }
            ],
            Sessions = [completed, upcoming]
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
        var service = new DashboardService(groups, participants, lessons, users);

        var dashboard = await service.GetAsync(CancellationToken.None);

        Assert.Equal(2, dashboard.Kpis.ActiveParticipants);
        Assert.Equal(1, dashboard.Kpis.ActiveGroups);
        Assert.Equal(1, dashboard.Kpis.EnrolledParticipants);
        Assert.Equal(1, dashboard.Kpis.WaitlistedParticipants);
        Assert.Equal(50, dashboard.Kpis.AverageAttendancePercent);
        Assert.Equal(1, dashboard.Kpis.PendingMakeups);
        Assert.Single(dashboard.UpcomingSessions);
        Assert.Single(dashboard.GroupFill);
        Assert.Equal(100, dashboard.GroupFill[0].FillPercent);
    }

    [Fact]
    public async Task GetAsync_DoesNotCountCompletedMakeupAsPending()
    {
        var groups = new InMemoryGroupRepository();
        var participants = new InMemoryParticipantRepository();
        var lessons = new InMemoryLessonRepository();
        var users = new InMemoryUserRepository();
        var instructor = new User { Email = "i@example.com", PasswordHash = "h", Role = UserRole.Instructor };
        await users.AddAsync(instructor, CancellationToken.None);
        var lesson = new Lesson { Title = "Scratch", Subject = "Scratch", Level = "P1", Description = "Opis", Status = LessonStatus.Ready };
        await lessons.AddAsync(lesson, CancellationToken.None);
        var jan = new Participant { FirstName = "Jan", LastName = "Kowalski" };
        await participants.AddAsync(jan, CancellationToken.None);

        var makeupSession = new ScheduledSession
        {
            LessonId = lesson.Id,
            ScheduledAt = DateTimeOffset.UtcNow.AddDays(-1),
            SequenceNumber = 2,
            Status = ScheduledSessionStatus.Completed,
            Attendance = [new AttendanceRecord { ParticipantId = jan.Id, Present = true }]
        };
        var originalSession = new ScheduledSession
        {
            LessonId = lesson.Id,
            ScheduledAt = DateTimeOffset.UtcNow.AddDays(-2),
            SequenceNumber = 1,
            Status = ScheduledSessionStatus.Completed,
            Attendance =
            [
                new AttendanceRecord
                {
                    ParticipantId = jan.Id,
                    Present = false,
                    MakeupRequired = true,
                    MakeupSessionId = makeupSession.Id
                }
            ]
        };
        var group = new Group
        {
            Name = "Grupa A",
            InstructorId = instructor.Id,
            Enrollments = [new GroupEnrollment { ParticipantId = jan.Id, Status = EnrollmentStatus.Enrolled }],
            Sessions = [originalSession, makeupSession]
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
        var service = new DashboardService(groups, participants, lessons, users);

        var dashboard = await service.GetAsync(CancellationToken.None);

        Assert.Equal(0, dashboard.Kpis.PendingMakeups);
    }
}

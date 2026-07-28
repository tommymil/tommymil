using LessonRunner.Application.Auth;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Participants;
using LessonRunner.Application.Scheduling;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Dashboard;

public sealed class DashboardService(
    IGroupRepository groupRepository,
    IParticipantRepository participantRepository,
    ILessonRepository lessonRepository,
    IUserRepository userRepository,
    ISchedulingRepository? schedulingRepository = null) : IDashboardService
{
    public async Task<DashboardDto> GetAsync(CancellationToken cancellationToken)
    {
        var groups = await groupRepository.ListAsync(cancellationToken);
        var participants = await participantRepository.ListAsync(cancellationToken);
        var lessons = (await lessonRepository.ListAsync(cancellationToken)).ToDictionary(lesson => lesson.Id, lesson => lesson.Title);
        var users = await userRepository.ListAsync(cancellationToken);
        var instructorNames = users.ToDictionary(user => user.Id, user => user.DisplayName);
        var locationNames = schedulingRepository is null
            ? new Dictionary<Guid, string>()
            : (await schedulingRepository.ListLocationsAsync(cancellationToken)).ToDictionary(location => location.Id, location => location.Name);

        var activeGroups = groups.Where(group => group.Status == GroupStatus.Active).ToList();
        var completedSessions = activeGroups.SelectMany(group => group.Sessions).Where(session => session.Status.CountsAsHeld()).ToList();
        var completedAttendance = completedSessions.SelectMany(session => session.Attendance).ToList();
        var attendancePercent = completedAttendance.Count == 0
            ? 0
            : (int)Math.Round(100.0 * completedAttendance.Count(record => record.Present) / completedAttendance.Count);
        var now = DateTimeOffset.UtcNow;
        var upcoming = activeGroups
            .SelectMany(group => group.Sessions.Select(session => (group, session)))
            .Where(item => item.session.Status.IsActive())
            .Where(item => item.session.ScheduledAt >= now.AddHours(-2))
            .OrderBy(item => item.session.ScheduledAt)
            .ToList();
        var enrolledCount = activeGroups.Sum(group => group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Enrolled));
        var waitlistedCount = activeGroups.Sum(group => group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Waitlisted));
        var groupsAtCapacity = activeGroups.Count(group =>
            group.Capacity is int capacity
            && group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Enrolled) >= capacity);
        var pendingMakeups = activeGroups
            .SelectMany(group => group.Sessions)
            .SelectMany(session => session.Attendance)
            .Count(record => record.MakeupRequired && !IsMakeupCompleted(record, activeGroups));

        var kpis = new DashboardKpiDto(
            participants.Count(participant => !participant.IsArchived),
            activeGroups.Count,
            enrolledCount,
            waitlistedCount,
            groupsAtCapacity,
            upcoming.Count,
            completedSessions.Count,
            attendancePercent,
            pendingMakeups);

        var upcomingDtos = upcoming
            .Take(8)
            .Select(item =>
            {
                var locationId = item.session.LocationId ?? item.group.LocationId;
                return new DashboardUpcomingSessionDto(
                    item.session.Id,
                    item.group.Id,
                    item.group.Name,
                    item.session.LessonId is Guid lessonId && lessons.TryGetValue(lessonId, out var lessonTitle) ? lessonTitle : null,
                    item.session.ScheduledAt,
                    instructorNames.GetValueOrDefault(item.session.SubstituteInstructorId ?? item.group.InstructorId, "(nieznany)"),
                    locationId is Guid id ? locationNames.GetValueOrDefault(id) : null);
            })
            .ToList();

        var instructorLoads = users
            .Where(user => user.Role is UserRole.Instructor or UserRole.Admin)
            .Select(user => new DashboardInstructorLoadDto(
                user.Id,
                user.DisplayName,
                activeGroups.Count(group => group.InstructorId == user.Id),
                upcoming.Count(item => item.group.InstructorId == user.Id),
                upcoming.Count(item => item.session.SubstituteInstructorId == user.Id)))
            .Where(load => load.ActiveGroups > 0 || load.UpcomingSessions > 0 || load.SubstituteSessions > 0)
            .OrderByDescending(load => load.UpcomingSessions + load.SubstituteSessions)
            .ThenBy(load => load.InstructorName)
            .Take(8)
            .ToList();

        var groupFill = activeGroups
            .Select(group =>
            {
                var enrolled = group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Enrolled);
                var waitlisted = group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Waitlisted);
                var fillPercent = group.Capacity is int capacity && capacity > 0
                    ? Math.Min(100, (int)Math.Round(100.0 * enrolled / capacity))
                    : 0;

                return new DashboardGroupFillDto(group.Id, group.Name, enrolled, waitlisted, group.Capacity, fillPercent);
            })
            .OrderByDescending(group => group.Capacity is null ? -1 : group.FillPercent)
            .ThenByDescending(group => group.Waitlisted)
            .ThenBy(group => group.GroupName)
            .Take(10)
            .ToList();

        return new DashboardDto(kpis, upcomingDtos, instructorLoads, groupFill);
    }

    private static bool IsMakeupCompleted(AttendanceRecord record, IReadOnlyList<Group> groups)
    {
        if (!record.MakeupRequired || record.MakeupSessionId is not Guid makeupSessionId)
        {
            return false;
        }

        var makeupSession = groups
            .SelectMany(group => group.Sessions)
            .FirstOrDefault(session => session.Id == makeupSessionId);

        return makeupSession?.Status.CountsAsHeld() == true
            && makeupSession.Attendance.Any(item => item.ParticipantId == record.ParticipantId && item.Present);
    }
}

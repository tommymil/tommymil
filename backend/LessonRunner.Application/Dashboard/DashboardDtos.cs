namespace LessonRunner.Application.Dashboard;

public sealed record DashboardKpiDto(
    int ActiveParticipants,
    int ActiveGroups,
    int EnrolledParticipants,
    int WaitlistedParticipants,
    int GroupsAtCapacity,
    int UpcomingSessions,
    int CompletedSessions,
    int AverageAttendancePercent,
    int PendingMakeups);

public sealed record DashboardUpcomingSessionDto(
    Guid SessionId,
    Guid GroupId,
    string GroupName,
    string? LessonTitle,
    DateTimeOffset ScheduledAt,
    string InstructorName,
    string? LocationName);

public sealed record DashboardInstructorLoadDto(
    Guid InstructorId,
    string InstructorName,
    int ActiveGroups,
    int UpcomingSessions,
    int SubstituteSessions);

public sealed record DashboardGroupFillDto(
    Guid GroupId,
    string GroupName,
    int Enrolled,
    int Waitlisted,
    int? Capacity,
    int FillPercent);

public sealed record DashboardDto(
    DashboardKpiDto Kpis,
    IReadOnlyList<DashboardUpcomingSessionDto> UpcomingSessions,
    IReadOnlyList<DashboardInstructorLoadDto> InstructorLoads,
    IReadOnlyList<DashboardGroupFillDto> GroupFill);

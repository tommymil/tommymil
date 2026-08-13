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

/// <summary>
/// Pozycja listy „Wymaga uwagi”.
///
/// Administrator otwiera panel z pytaniem „czym się dziś zająć”, a dostawał tablicę
/// sześciu liczb bez progów i bez trendu — „Frekwencja 87%” nie mówi, czy to dobrze.
/// Każda pozycja tej listy jest konkretną sprawą z odnośnikiem do miejsca, w którym
/// da się ją załatwić.
/// </summary>
public sealed record DashboardAttentionDto(
    /// <summary>Rodzaj sprawy - front dobiera po nim ikonę: `overdue`, `nolesson`,
    /// `noinstructor`, `lowattendance`, `expiringcredit`, `waitlist`.</summary>
    string Kind,
    string Title,
    string Detail,
    /// <summary>`danger` albo `warning` - czy to już problem, czy dopiero ostrzeżenie.</summary>
    string Severity,
    /// <summary>Ścieżka we froncie prowadząca do miejsca załatwienia sprawy.</summary>
    string Path);

public sealed record DashboardDto(
    DashboardKpiDto Kpis,
    IReadOnlyList<DashboardUpcomingSessionDto> UpcomingSessions,
    IReadOnlyList<DashboardInstructorLoadDto> InstructorLoads,
    IReadOnlyList<DashboardGroupFillDto> GroupFill,
    IReadOnlyList<DashboardAttentionDto>? Attention = null);

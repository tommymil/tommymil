export type DashboardKpis = {
  activeParticipants: number;
  activeGroups: number;
  enrolledParticipants: number;
  waitlistedParticipants: number;
  groupsAtCapacity: number;
  upcomingSessions: number;
  completedSessions: number;
  averageAttendancePercent: number;
  pendingMakeups: number;
};

export type DashboardUpcomingSession = {
  sessionId: string;
  groupId: string;
  groupName: string;
  lessonTitle: string | null;
  scheduledAt: string;
  instructorName: string;
  locationName: string | null;
};

export type DashboardInstructorLoad = {
  instructorId: string;
  instructorName: string;
  activeGroups: number;
  upcomingSessions: number;
  substituteSessions: number;
};

export type DashboardGroupFill = {
  groupId: string;
  groupName: string;
  enrolled: number;
  waitlisted: number;
  capacity: number | null;
  fillPercent: number;
};

/**
 * Pozycja listy „Wymaga uwagi”.
 *
 * Pulpit pokazywał sześć liczb bez progów i bez trendu, a administrator otwiera go
 * z pytaniem „czym się dziś zająć”. Każda pozycja to konkretna sprawa z odnośnikiem
 * do miejsca, w którym da się ją załatwić.
 */
export type DashboardAttention = {
  kind: "overdue" | "nolesson" | "noinstructor" | "lowattendance" | "expiringcredit" | "waitlist";
  title: string;
  detail: string;
  severity: "danger" | "warning";
  path: string;
};

export type Dashboard = {
  kpis: DashboardKpis;
  upcomingSessions: DashboardUpcomingSession[];
  instructorLoads: DashboardInstructorLoad[];
  groupFill: DashboardGroupFill[];
  attention?: DashboardAttention[] | null;
};

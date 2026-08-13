import { apiDelete, apiDownload, apiGet, apiPost, apiPostEmpty, apiPut } from "./client";
import type { DownloadedFile } from "./client";
import type {
  AddSessionRequest,
  CreateGroupRequest,
  FinishSessionRequest,
  GroupAttendanceSummary,
  GroupDetails,
  GroupSummary,
  Instructor,
  CancelSessionRequest,
  RescheduleRequest,
  SaveAttendanceRequest,
  ScheduledSession,
  SessionChange,
  SessionStatusOption,
  SetLiveStatusRequest,
  SetSessionStatusRequest,
  SetSubstituteInstructorRequest,
  SessionAttendance,
  UpdateGroupRequest,
  UpdateSessionLinksRequest,
} from "../types/group";

// --- Administracja grupami ---

export function getGroups(): Promise<GroupSummary[]> {
  return apiGet<GroupSummary[]>("/api/groups");
}

export function getGroup(id: string): Promise<GroupDetails> {
  return apiGet<GroupDetails>(`/api/groups/${id}`);
}

export function createGroup(request: CreateGroupRequest): Promise<GroupDetails> {
  return apiPost<CreateGroupRequest, GroupDetails>("/api/groups", request);
}

export function updateGroup(id: string, request: UpdateGroupRequest): Promise<GroupDetails> {
  return apiPut<UpdateGroupRequest, GroupDetails>(`/api/groups/${id}`, request);
}

export function deleteGroup(id: string): Promise<void> {
  return apiDelete(`/api/groups/${id}`);
}

export function addSession(groupId: string, request: AddSessionRequest): Promise<ScheduledSession> {
  return apiPost<AddSessionRequest, ScheduledSession>(`/api/groups/${groupId}/sessions`, request);
}

export function getInstructors(): Promise<Instructor[]> {
  return apiGet<Instructor[]>("/api/users/instructors");
}

export function cancelSession(
  groupId: string,
  sessionId: string,
  request: CancelSessionRequest,
): Promise<ScheduledSession> {
  return apiPost<CancelSessionRequest, ScheduledSession>(`/api/groups/${groupId}/sessions/${sessionId}/cancel`, request);
}

/** Ręczna zmiana statusu terminu (potwierdzenie, awaria techniczna, niezrealizowane). */
export function setSessionStatus(
  groupId: string,
  sessionId: string,
  request: SetSessionStatusRequest,
): Promise<ScheduledSession> {
  return apiPut<SetSessionStatusRequest, ScheduledSession>(`/api/groups/${groupId}/sessions/${sessionId}/status`, request);
}

export function getSessionStatusOptions(): Promise<SessionStatusOption[]> {
  return apiGet<SessionStatusOption[]>("/api/groups/session-statuses");
}

/** Historia zmian terminów grupy, od najnowszej. */
export function getSessionHistory(groupId: string): Promise<SessionChange[]> {
  return apiGet<SessionChange[]>(`/api/groups/${groupId}/sessions/history`);
}

export function rescheduleSession(groupId: string, sessionId: string, request: RescheduleRequest): Promise<ScheduledSession> {
  return apiPost<RescheduleRequest, ScheduledSession>(`/api/groups/${groupId}/sessions/${sessionId}/reschedule`, request);
}

export function setSessionSubstitute(
  groupId: string,
  sessionId: string,
  request: SetSubstituteInstructorRequest,
): Promise<ScheduledSession> {
  return apiPost<SetSubstituteInstructorRequest, ScheduledSession>(`/api/groups/${groupId}/sessions/${sessionId}/substitute`, request);
}

/** Link do spotkania i nagranie dla pojedynczego terminu - nadpisują ustawienia grupy. */
export function setSessionLinks(
  groupId: string,
  sessionId: string,
  request: UpdateSessionLinksRequest,
): Promise<ScheduledSession> {
  return apiPut<UpdateSessionLinksRequest, ScheduledSession>(`/api/groups/${groupId}/sessions/${sessionId}/links`, request);
}

export function getGroupAttendance(groupId: string): Promise<GroupAttendanceSummary> {
  return apiGet<GroupAttendanceSummary>(`/api/groups/${groupId}/attendance`);
}

export function exportGroupAttendance(groupId: string): Promise<DownloadedFile> {
  return apiDownload(`/api/groups/${groupId}/attendance/export?format=csv`);
}

export function exportSessionAttendance(groupId: string, sessionId: string): Promise<DownloadedFile> {
  return apiDownload(`/api/groups/${groupId}/sessions/${sessionId}/attendance/export?format=csv`);
}

// --- Grafik instruktora ---

export function getSchedule(): Promise<ScheduledSession[]> {
  return apiGet<ScheduledSession[]>("/api/schedule");
}

export function getScheduledSession(sessionId: string): Promise<ScheduledSession> {
  return apiGet<ScheduledSession>(`/api/schedule/${sessionId}`);
}

export function startSession(sessionId: string): Promise<SessionAttendance> {
  return apiPostEmpty<SessionAttendance>(`/api/schedule/${sessionId}/start`);
}

export function getAttendance(sessionId: string): Promise<SessionAttendance> {
  return apiGet<SessionAttendance>(`/api/schedule/${sessionId}/attendance`);
}

export function saveAttendance(sessionId: string, request: SaveAttendanceRequest): Promise<SessionAttendance> {
  return apiPut<SaveAttendanceRequest, SessionAttendance>(`/api/schedule/${sessionId}/attendance`, request);
}

/**
 * Znacznik pracy jednego dziecka w trakcie zajęć.
 *
 * Osobna, wąska operacja zamiast przepychania całej listy obecności: instruktor klika to
 * co kilkadziesiąt sekund, a wysyłanie przy tym stanu całej grupy groziłoby nadpisaniem
 * świeżej zmiany danymi sprzed chwili.
 */
export function setLiveStatus(sessionId: string, request: SetLiveStatusRequest): Promise<SessionAttendance> {
  return apiPut<SetLiveStatusRequest, SessionAttendance>(`/api/schedule/${sessionId}/live-status`, request);
}

export function finishSession(sessionId: string, request: FinishSessionRequest): Promise<ScheduledSession> {
  return apiPost<FinishSessionRequest, ScheduledSession>(`/api/schedule/${sessionId}/finish`, request);
}

/** Grafik instruktora jako plik iCalendar do zaimportowania w swoim kalendarzu. */
export function exportScheduleIcs(): Promise<DownloadedFile> {
  return apiDownload("/api/schedule/export.ics");
}

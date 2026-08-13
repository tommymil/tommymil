export type ScheduledSessionStatus =
  | "planned"
  | "confirmed"
  | "inprogress"
  | "completed"
  | "cancelled"
  | "cancelledbyinstructor"
  | "cancelledbyparent"
  | "technicalfailure"
  | "notdelivered"
  | "awaitingreschedule";

/** Słownik statusów terminu przychodzi z backendu - bez własnej kopii etykiet we frontendzie. */
export type SessionStatusOption = {
  value: string;
  label: string;
  countsAsHeld: boolean;
};

/** Termin przed nami: zaplanowany albo potwierdzony. Po dołożeniu „potwierdzone”
 *  samo porównanie z „planned” przestało wystarczać. */
export function isUpcomingSession(status: ScheduledSessionStatus): boolean {
  return status === "planned" || status === "confirmed";
}

/** Termin aktywny: przed zajęciami albo w ich trakcie. */
export function isActiveSession(status: ScheduledSessionStatus): boolean {
  return isUpcomingSession(status) || status === "inprogress";
}

/** Każdy wariant odwołania - także oczekiwanie na nowy termin. */
export function isCancelledSession(status: ScheduledSessionStatus): boolean {
  return (
    status === "cancelled" ||
    status === "cancelledbyinstructor" ||
    status === "cancelledbyparent" ||
    status === "awaitingreschedule"
  );
}

export type SetSessionStatusRequest = {
  status: string;
  reason: string | null;
  guardiansNotified: boolean;
};

export type ScheduledSession = {
  id: string;
  groupId: string;
  groupName: string;
  lessonId: string | null;
  lessonTitle: string | null;
  scheduledAt: string;
  sequenceNumber: number;
  status: ScheduledSessionStatus;
  statusLabel: string;
  startedAt: string | null;
  completedAt: string | null;
  instructorNote: string | null;
  locationId?: string | null;
  locationName?: string | null;
  substituteInstructorId?: string | null;
  substituteInstructorName?: string | null;
  /** Link obowiązujący dla tego terminu: własny link terminu, a gdy go nie ma - link grupy. */
  meetingUrl?: string | null;
  /** Link ustawiony wprost na terminie (bez podstawienia z grupy) - do formularza edycji. */
  sessionMeetingUrl?: string | null;
  recordingUrl?: string | null;
  unfinishedNote?: string | null;
  parentSummary?: string | null;
};

export type CancelSessionRequest = {
  reason: string | null;
  guardiansNotified: boolean;
  /** `instructor` albo `parent` - przy rozliczeniu to nie jest ten sam przypadek. */
  cancelledBy?: string | null;
  /** `none` | `credit` | `makeup` | `refund` - decyzja osobna od samego odwołania. */
  compensation?: string | null;
  /** Przesunąć materiał na kolejne terminy i wydłużyć kurs o jedne zajęcia. */
  shiftFollowingLessons?: boolean;
};

/** Wpis historii zmian terminu - kto, kiedy, dlaczego i czy powiadomiono opiekunów. */
export type SessionChange = {
  id: string;
  sessionId: string;
  sequenceNumber: number;
  changeType: string;
  changeTypeLabel: string;
  previousScheduledAt: string | null;
  newScheduledAt: string | null;
  reason: string | null;
  details: string | null;
  changedByUserId: string | null;
  changedByName: string | null;
  guardiansNotified: boolean;
  changedAt: string;
};

export type UpdateSessionLinksRequest = {
  meetingUrl: string | null;
  recordingUrl: string | null;
};

export type Participant = {
  id: string;
  firstName: string;
  lastName: string;
  phone: string | null;
  email: string | null;
  enrollmentStatus?: "enrolled" | "waitlisted";
};

export type GroupSummary = {
  id: string;
  name: string;
  instructorId: string;
  instructorEmail: string;
  instructorName: string;
  courseId?: string | null;
  locationId?: string | null;
  locationName?: string | null;
  capacity?: number | null;
  meetingUrl?: string | null;
  participantCount: number;
  waitlistedCount?: number;
  sessionCount: number;
  nextSessionAt: string | null;
};

export type GroupDetails = {
  id: string;
  name: string;
  instructorId: string;
  instructorEmail: string;
  instructorName: string;
  status: string;
  courseId?: string | null;
  locationId?: string | null;
  locationName?: string | null;
  capacity?: number | null;
  meetingUrl?: string | null;
  participants: Participant[];
  sessions: ScheduledSession[];
};

export type Instructor = {
  id: string;
  email: string;
  displayName: string;
};

export type CreateGroupRequest = {
  name: string;
  instructorId: string;
  lessonIds: string[];
  firstSessionAt: string;
  participantIds: string[];
  courseId?: string | null;
  locationId?: string | null;
  capacity?: number | null;
  meetingUrl?: string | null;
};

/** Znacznik pracy dziecka w trakcie trwających zajęć. Nie trafia do portalu rodzica. */
export type LiveWorkStatus = "working" | "needshelp" | "finished" | "blocked";

export type LiveStatusOption = { value: string; label: string };

export type AttendanceEntry = {
  participantId: string;
  firstName: string;
  lastName: string;
  /** Skrót „liczy się jako obecność” - wyliczany na backendzie ze statusu. */
  present: boolean;
  liveStatus?: LiveWorkStatus;
  liveStatusLabel?: string;
  makeupRequired?: boolean;
  makeupSessionId?: string | null;
  status: string;
  statusLabel: string;
  note?: string | null;
  joinedAt?: string | null;
  leftAt?: string | null;
};

/** Słownik statusów przychodzi z backendu - frontend nie trzyma własnej kopii etykiet. */
export type AttendanceStatusOption = {
  value: string;
  label: string;
  countsAsPresent: boolean;
};

export type SessionAttendance = {
  sessionId: string;
  status: ScheduledSessionStatus;
  statusLabel: string;
  entries: AttendanceEntry[];
  makeupOptions: MakeupSessionOption[];
  statusOptions: AttendanceStatusOption[];
  liveStatusOptions?: LiveStatusOption[] | null;
};

export type MakeupSessionOption = {
  sessionId: string;
  groupId: string;
  groupName: string;
  scheduledAt: string;
  lessonTitle: string | null;
};

export type SaveAttendanceRequest = {
  entries: {
    participantId: string;
    present: boolean;
    makeupRequired?: boolean;
    makeupSessionId?: string | null;
    status?: string;
    note?: string | null;
    /** Pominięty = bez zmiany. Autozapis wysyła tylko to, co faktycznie zmieniono. */
    liveStatus?: string | null;
  }[];
};

/** Zmiana znacznika pracy jednego dziecka - wąska operacja zamiast całej listy. */
export type SetLiveStatusRequest = {
  participantId: string;
  liveStatus: string;
};

/**
 * Zakończenie zajęć w trzech polach zamiast jednego.
 *
 * Rozdział 5 dokumentu koncepcyjnego oczekuje po lekcji trzech różnych informacji.
 * Wcześniej wszystko szło do jednego `note`, z którego nic nie dało się odczytać
 * automatycznie ani pokazać rodzicowi.
 */
export type FinishSessionRequest = {
  /** Uwagi wewnętrzne - do portalu rodzica nie trafiają w ogóle. */
  note: string | null;
  /** Czego nie zdążyliśmy - podpowiadane na kolejnym terminie i przy zastępstwie. */
  unfinishedNote?: string | null;
  /** Podsumowanie dla rodzica - jedyny fragment debriefu widoczny w portalu. */
  parentSummary?: string | null;
};

export type UpdateGroupRequest = {
  name: string;
  instructorId: string;
  locationId?: string | null;
  capacity?: number | null;
  meetingUrl?: string | null;
};

export type AddSessionRequest = {
  lessonId: string;
  scheduledAt: string;
  locationId?: string | null;
  substituteInstructorId?: string | null;
};

export type RescheduleRequest = {
  scheduledAt: string;
  locationId?: string | null;
  substituteInstructorId?: string | null;
  /** Powód przełożenia - trafia do historii zmian terminu. */
  reason?: string | null;
  guardiansNotified?: boolean;
};

export type SetSubstituteInstructorRequest = {
  substituteInstructorId?: string | null;
};

export type ParticipantAttendance = {
  participantId: string;
  firstName: string;
  lastName: string;
  presentCount: number;
  heldCount: number;
  ratePercent: number;
};

export type GroupAttendanceSummary = {
  heldSessions: number;
  participants: ParticipantAttendance[];
};

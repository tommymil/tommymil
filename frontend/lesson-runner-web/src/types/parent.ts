export type ParentChild = {
  participantId: string;
  firstName: string;
  lastName: string;
  groups: { groupId: string; groupName: string }[];
};

export type ParentSessionChild = {
  participantId: string;
  firstName: string;
  lastName: string;
  /** Czy nieobecność została już zgłoszona. */
  absenceReported: boolean;
  absenceNote: string | null;
};

export type ParentScheduleItem = {
  sessionId: string;
  groupId: string;
  groupName: string;
  lessonTitle: string | null;
  scheduledAt: string;
  status: string;
  statusLabel: string;
  meetingUrl?: string | null;
  /** Dzieci tego rodzica zapisane na ten termin. */
  children?: ParentSessionChild[] | null;
  /** „lekcja 7 z 12” — rodzic pyta o postęp kursu częściej niż o pojedynczy termin. */
  sequenceNumber?: number;
  courseLength?: number;
  /** Kto faktycznie poprowadzi te zajęcia (zastępstwo wygrywa z instruktorem grupy). */
  instructorName?: string | null;
  durationMinutes?: number;
  earlyLeaveAfterMinutes?: number | null;
  lessonKind?: "standard" | "showcase";
  lessonKindLabel?: string;
};

export type ParentAttendanceItem = {
  groupId: string;
  groupName: string;
  presentCount: number;
  heldCount: number;
  ratePercent: number;
  participantId?: string;
};

export type ParentInvoice = {
  invoiceId: string;
  number: string;
  groupName: string;
  amountCents: number;
  currency: string;
  status: string;
  statusLabel: string;
  dueDate: string;
  paidAt: string | null;
  participantId?: string;
  /** Po terminie płatności — liczone przy odczycie, nie ze statusu w bazie. */
  isOverdue?: boolean;
};

/** Kredyt zajęciowy w wersji dla rodzica: „należą się jedne zajęcia”. */
export type ParentCredit = {
  creditId: string;
  participantId: string;
  childName: string;
  groupName: string | null;
  reason: string;
  issuedAt: string;
  expiresAt: string | null;
};

export type ParentCourseProgress = {
  participantId: string;
  groupId: string;
  groupName: string;
  completedLessons: number;
  totalLessons: number;
  nextLessonTitle: string | null;
  nextSessionAt: string | null;
};

export type ParentConsent = {
  participantId: string;
  childName: string;
  dataProcessing: boolean;
  dataProcessingAt: string | null;
  image: boolean;
  imageAt: string | null;
};

/** Cztery liczby, po które rodzic wchodzi do portalu. */
export type ParentSummary = {
  outstandingCents: number;
  overdueCents: number;
  currency: string;
  nextDueDate: string | null;
  availableCredits: number;
  attendancePercent: number;
  nextSession: ParentScheduleItem | null;
};

export type ParentMaterialFile = {
  label: string;
  fileName: string;
  sizeBytes: number;
  downloadUrl: string;
};

export type ParentMaterial = {
  sessionId: string;
  groupId: string;
  groupName: string;
  lessonTitle: string | null;
  scheduledAt: string;
  files: ParentMaterialFile[];
  recordingUrl: string | null;
  /** Których dzieci dotyczy — bez tego przy rodzeństwie nie da się filtrować portalu. */
  participantIds?: string[] | null;
};

/** Wpis o postępie w wersji dla rodzica — wyłącznie pola pisane z myślą o nim. */
export type ParentProgressEntry = {
  updatedAt: string;
  autonomyLabel: string;
  autonomyRank: number;
  lessonCompleted: boolean;
  noteForParent: string | null;
  nextStep: string | null;
};

export type ParentProjectVersion = {
  version: number;
  url: string | null;
  fileName: string | null;
  downloadUrl: string | null;
  submittedAt: string;
  instructorComment: string | null;
};

export type ParentProject = {
  projectId: string;
  title: string;
  description: string | null;
  versions: ParentProjectVersion[];
};

export type ParentChildProgress = {
  participantId: string;
  firstName: string;
  lastName: string;
  entries: ParentProgressEntry[];
  projects: ParentProject[];
};

export type ParentPortal = {
  children: ParentChild[];
  schedule: ParentScheduleItem[];
  attendance: ParentAttendanceItem[];
  invoices: ParentInvoice[];
  materials: ParentMaterial[];
  progress: ParentChildProgress[];
  summary?: ParentSummary | null;
  credits?: ParentCredit[] | null;
  courseProgress?: ParentCourseProgress[] | null;
  consents?: ParentConsent[] | null;
};

export type ParentParticipantLink = {
  parentUserId: string;
  participantId: string;
  /** Kim opiekun jest dla dziecka: „mama”, „tata”, „opiekun prawny”. */
  relation?: string | null;
  /** Kontakt pierwszego wyboru - jeden na dziecko. */
  isPrimaryContact?: boolean;
  /** Czy ten opiekun dostaje powiadomienia e-mail. */
  receivesNotifications?: boolean;
};

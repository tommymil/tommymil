export type LessonStatus = "draft" | "review" | "ready";
export type LessonKind = "standard" | "showcase";

export type LessonSummary = {
  id: string;
  title: string;
  subject: string;
  level: string;
  description: string;
  order: number;
  status: LessonStatus;
  statusLabel: string;
  stepCount: number;
  durationMinutes: number;
  kind: LessonKind;
  kindLabel: string;
  scheduledDurationMinutes: number;
  earlyLeaveAfterMinutes: number | null;
};

export type LessonDetails = LessonSummary & {
  tags: string[];
  projectFiles: LessonProjectFiles;
  steps: LessonStep[];
} & LessonIntent;

/**
 * Po co są te zajęcia i co po nich zostaje.
 *
 * `description` opisuje lekcję na zewnątrz (biblioteka), `objective` odpowiada prowadzącemu,
 * co jest w niej najważniejsze. Do lipca 2026 nie było gdzie tego zapisać.
 */
export type LessonIntent = {
  objective?: string | null;
  /** Po zajęciach dziecko potrafi… */
  successCriteria?: string[] | null;
  /** Co przygotować przed zajęciami. */
  preparation?: string[] | null;
  /** Zadanie domowe albo co pokazać rodzicom. */
  homework?: string[] | null;
};

export type LessonProjectFiles = {
  starter?: LessonProjectFile | null;
  final?: LessonProjectFile | null;
};

export type LessonProjectFile = {
  label: string;
  url: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  downloadToken?: string | null;
  downloadUrl?: string | null;
};

export type LessonStep = {
  id: string;
  order: number;
  type: string;
  title: string;
  durationMinutes: number;
  script: string[];
  studentItems: StudentItem[];
  resources: LessonResource[];
  notes: LessonNote[];
};

export type StudentItem = {
  kind: string;
  text?: string | null;
  caption?: string | null;
  url?: string | null;
};

export type LessonResource = {
  kind: string;
  label?: string | null;
  url?: string | null;
  code?: string | null;
  language?: string | null;
};

export type LessonNote = {
  kind: string;
  text: string;
};

export type StoredFile = {
  url: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
};

export type LessonRunSession = {
  id: string;
  lessonId: string;
  userId: string;
  stepIndex: number;
  elapsedTotalSeconds: number;
  elapsedStepSeconds: number;
  running: boolean;
  startedAt: string;
  updatedAt: string;
};

export type UpdateLessonRunSessionRequest = {
  stepIndex: number;
  elapsedTotalSeconds: number;
  elapsedStepSeconds: number;
  running: boolean;
};

export type CreateLessonRequest = {
  kind: LessonKind;
  title: string;
  subject: string;
  level: string;
  description: string;
  tags: string[];
  projectFiles?: LessonProjectFiles | null;
  steps: CreateLessonStepRequest[];
  order?: number;
} & LessonIntent;

export type CreateLessonStepRequest = {
  type: string;
  title: string;
  durationMinutes: number;
  script: string[];
  studentItems: StudentItem[];
  resources: LessonResource[];
  notes: LessonNote[];
};

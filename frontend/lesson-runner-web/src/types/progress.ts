export type AutonomyOption = {
  value: string;
  label: string;
  rank: number;
};

export type ProgressEntry = {
  id: string;
  participantId: string;
  sessionId: string | null;
  groupId: string | null;
  lessonId: string | null;
  autonomy: string;
  autonomyLabel: string;
  autonomyRank: number;
  lessonCompleted: boolean;
  noteForParent: string | null;
  nextStep: string | null;
  updatedAt: string;
};

export type SessionProgress = {
  sessionId: string;
  entries: ProgressEntry[];
  autonomyOptions: AutonomyOption[];
};

export type SaveProgressEntry = {
  participantId: string;
  autonomy: string;
  lessonCompleted: boolean;
  noteForParent: string | null;
  nextStep: string | null;
};

export type ProjectSubmission = {
  id: string;
  version: number;
  url: string | null;
  fileName: string | null;
  sizeBytes: number | null;
  downloadUrl: string | null;
  submittedAt: string;
  instructorComment: string | null;
};

export type Project = {
  id: string;
  participantId: string;
  title: string;
  description: string | null;
  groupId: string | null;
  createdAt: string;
  submissions: ProjectSubmission[];
};

export type ParticipantProgress = {
  participantId: string;
  entries: ProgressEntry[];
  projects: Project[];
};

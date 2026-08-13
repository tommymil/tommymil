import { apiGet, apiPost, apiPut } from "./client";
import type {
  ParticipantProgress,
  Project,
  SaveProgressEntry,
  SessionProgress,
} from "../types/progress";

export function getSessionProgress(sessionId: string): Promise<SessionProgress> {
  return apiGet<SessionProgress>(`/api/progress/sessions/${sessionId}`);
}

export function saveSessionProgress(sessionId: string, entries: SaveProgressEntry[]): Promise<SessionProgress> {
  return apiPut<{ entries: SaveProgressEntry[] }, SessionProgress>(`/api/progress/sessions/${sessionId}`, {
    entries,
  });
}

export function getParticipantProgress(participantId: string): Promise<ParticipantProgress> {
  return apiGet<ParticipantProgress>(`/api/progress/participants/${participantId}`);
}

export function createProject(request: {
  participantId: string;
  title: string;
  description?: string | null;
  groupId?: string | null;
  lessonId?: string | null;
}): Promise<Project> {
  return apiPost<typeof request, Project>("/api/progress/projects", request);
}

/** Nowa wersja projektu: link **albo** plik. Podanie obu backend odrzuca. */
export function addProjectSubmission(
  projectId: string,
  request: {
    url?: string | null;
    fileUrl?: string | null;
    fileName?: string | null;
    contentType?: string | null;
    sizeBytes?: number | null;
    instructorComment?: string | null;
  },
): Promise<Project> {
  return apiPost<typeof request, Project>(`/api/progress/projects/${projectId}/submissions`, request);
}

export function setSubmissionComment(projectId: string, submissionId: string, comment: string | null): Promise<void> {
  return apiPut<{ comment: string | null }, void>(
    `/api/progress/projects/${projectId}/submissions/${submissionId}/comment`,
    { comment },
  );
}

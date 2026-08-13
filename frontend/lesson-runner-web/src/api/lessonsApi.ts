import { apiDelete, apiGet, apiPost, apiPostEmpty, apiPut } from "./client";
import type {
  CreateLessonRequest,
  LessonDetails,
  LessonRunSession,
  LessonSummary,
  UpdateLessonRunSessionRequest,
} from "../types/lesson";

export function getLessons(): Promise<LessonSummary[]> {
  return apiGet<LessonSummary[]>("/api/lessons");
}

export function getLesson(id: string): Promise<LessonDetails> {
  return apiGet<LessonDetails>(`/api/lessons/${id}`);
}

export function createLesson(request: CreateLessonRequest): Promise<LessonDetails> {
  return apiPost<CreateLessonRequest, LessonDetails>("/api/lessons", request);
}

export function updateLesson(id: string, request: CreateLessonRequest): Promise<LessonDetails> {
  return apiPut<CreateLessonRequest, LessonDetails>(`/api/lessons/${id}`, request);
}

export function sendLessonToReview(id: string): Promise<LessonDetails> {
  return apiPostEmpty<LessonDetails>(`/api/lessons/${id}/send-to-review`);
}

export function publishLesson(id: string): Promise<LessonDetails> {
  return apiPostEmpty<LessonDetails>(`/api/lessons/${id}/publish`);
}

export function deleteLesson(id: string): Promise<void> {
  return apiDelete(`/api/lessons/${id}`);
}

function runSessionPath(lessonId: string, scheduledSessionId?: string): string {
  const suffix = scheduledSessionId ? `?scheduledSessionId=${scheduledSessionId}` : "";
  return `/api/lessons/${lessonId}/run-session${suffix}`;
}

export function startLessonRunSession(lessonId: string, scheduledSessionId?: string): Promise<LessonRunSession> {
  return apiPostEmpty<LessonRunSession>(runSessionPath(lessonId, scheduledSessionId));
}

export function updateLessonRunSession(
  lessonId: string,
  request: UpdateLessonRunSessionRequest,
  scheduledSessionId?: string,
): Promise<LessonRunSession> {
  return apiPut<UpdateLessonRunSessionRequest, LessonRunSession>(runSessionPath(lessonId, scheduledSessionId), request);
}

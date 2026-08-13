import { apiDelete, apiGet, apiPost, apiPut } from "./client";
import type {
  CreateTrialRequest,
  DeclineTrialRequest,
  EnrollTrialRequest,
  SaveTrialDiagnosisRequest,
  ScheduleTrialRequest,
  TrialBoard,
  TrialEnrollmentResult,
  TrialLesson,
} from "../types/trial";

export function getTrials(): Promise<TrialBoard> {
  return apiGet<TrialBoard>("/api/trials");
}

/** Lekcje próbne zalogowanego instruktora — osobna trasa, osobna lista. */
export function getMyTrials(): Promise<TrialBoard> {
  return apiGet<TrialBoard>("/api/my-trials");
}

export function createTrial(request: CreateTrialRequest): Promise<TrialLesson> {
  return apiPost<CreateTrialRequest, TrialLesson>("/api/trials", request);
}

export function scheduleTrial(id: string, request: ScheduleTrialRequest): Promise<TrialLesson> {
  return apiPut<ScheduleTrialRequest, TrialLesson>(`/api/trials/${id}/schedule`, request);
}

export function saveTrialDiagnosis(id: string, request: SaveTrialDiagnosisRequest): Promise<TrialLesson> {
  return apiPut<SaveTrialDiagnosisRequest, TrialLesson>(`/api/my-trials/${id}/diagnosis`, request);
}

export function enrollTrial(id: string, request: EnrollTrialRequest): Promise<TrialEnrollmentResult> {
  return apiPost<EnrollTrialRequest, TrialEnrollmentResult>(`/api/trials/${id}/enroll`, request);
}

export function declineTrial(id: string, request: DeclineTrialRequest): Promise<TrialLesson> {
  return apiPost<DeclineTrialRequest, TrialLesson>(`/api/trials/${id}/decline`, request);
}

export function deleteTrial(id: string): Promise<void> {
  return apiDelete(`/api/trials/${id}`);
}

import { apiDelete, apiGet, apiPost, apiPostEmpty, apiPut, apiPutEmpty } from "./client";
import type {
  CreateParticipantRequest,
  GuardianAccountResult,
  ParticipantDetails,
  ParticipantSummary,
  UpdateParticipantRequest,
} from "../types/participant";

export function getParticipants(query?: string, includeArchived = false): Promise<ParticipantSummary[]> {
  const params = new URLSearchParams();
  const trimmed = query?.trim();
  if (trimmed) {
    params.set("q", trimmed);
  }
  if (includeArchived) {
    params.set("includeArchived", "true");
  }
  const search = params.toString();
  return apiGet<ParticipantSummary[]>(`/api/participants${search ? `?${search}` : ""}`);
}

export function getParticipant(id: string): Promise<ParticipantDetails> {
  return apiGet<ParticipantDetails>(`/api/participants/${id}`);
}

export function createParticipant(request: CreateParticipantRequest): Promise<ParticipantDetails> {
  return apiPost<CreateParticipantRequest, ParticipantDetails>("/api/participants", request);
}

export function updateParticipant(id: string, request: UpdateParticipantRequest): Promise<ParticipantDetails> {
  return apiPut<UpdateParticipantRequest, ParticipantDetails>(`/api/participants/${id}`, request);
}

export function deleteParticipant(id: string): Promise<void> {
  return apiDelete(`/api/participants/${id}`);
}

export function archiveParticipant(id: string): Promise<void> {
  return apiPostEmpty<void>(`/api/participants/${id}/archive`);
}

export function restoreParticipant(id: string): Promise<void> {
  return apiPostEmpty<void>(`/api/participants/${id}/restore`);
}

export function anonymizeParticipant(id: string): Promise<void> {
  return apiPostEmpty<void>(`/api/participants/${id}/anonymize`);
}

export function enrollParticipant(id: string, groupId: string): Promise<ParticipantDetails> {
  return apiPutEmpty<ParticipantDetails>(`/api/participants/${id}/groups/${groupId}`);
}

export function unenrollParticipant(id: string, groupId: string): Promise<ParticipantDetails> {
  return apiDelete<ParticipantDetails>(`/api/participants/${id}/groups/${groupId}`);
}

export function createGuardianAccount(id: string): Promise<GuardianAccountResult> {
  return apiPostEmpty<GuardianAccountResult>(`/api/participants/${id}/guardian-account`);
}

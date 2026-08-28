import { apiGet, apiPost, apiPostEmpty, apiPut } from "./client";
import type { InvitationResult } from "../types/auth";
import type { CreateUserRequest, ManagedUser, UpdateUserProfileRequest } from "../types/user";

export function getUsers(): Promise<ManagedUser[]> {
  return apiGet<ManagedUser[]>("/api/users");
}

export function createUser(request: CreateUserRequest): Promise<ManagedUser> {
  return apiPost<CreateUserRequest, ManagedUser>("/api/users", request);
}

export function updateUserProfile(id: string, request: UpdateUserProfileRequest): Promise<void> {
  return apiPut<UpdateUserProfileRequest, void>(`/api/users/${id}/profile`, request);
}

export function setUserActive(id: string, isActive: boolean): Promise<void> {
  return apiPostEmpty<void>(`/api/users/${id}/${isActive ? "activate" : "deactivate"}`);
}

export function inviteUser(id: string): Promise<InvitationResult> {
  return apiPostEmpty<InvitationResult>(`/api/users/${id}/invite`);
}

export function setUserPassword(id: string, password: string): Promise<void> {
  return apiPost<{ password: string }, void>(`/api/users/${id}/password`, { password });
}

/**
 * Zmiana roli konta. Serwer unieważnia przy tym sesje tego użytkownika, bo rola jedzie
 * w tokenie — zmieniona osoba musi zalogować się ponownie.
 */
export function setUserRole(id: string, role: string): Promise<void> {
  return apiPut<{ role: string }, void>(`/api/users/${id}/role`, { role });
}

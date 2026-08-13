import { apiGet, apiPost } from "./client";
import type {
  AccountTokenInfo,
  AuthResponse,
  AuthUser,
  ChangePasswordRequest,
  LoginRequest,
} from "../types/auth";

export function login(request: LoginRequest): Promise<AuthResponse> {
  return apiPost<LoginRequest, AuthResponse>("/api/auth/login", request);
}

export function getCurrentUser(): Promise<AuthUser> {
  return apiGet<AuthUser>("/api/auth/me");
}

export function changePassword(request: ChangePasswordRequest): Promise<void> {
  return apiPost<ChangePasswordRequest, void>("/api/auth/change-password", request);
}

/** Kończy się tak samo dla adresu znanego i nieznanego — backend celowo nie zdradza, czy konto istnieje. */
export function requestPasswordReset(email: string): Promise<void> {
  return apiPost<{ email: string }, void>("/api/auth/password-reset", { email });
}

export function describeAccountToken(token: string): Promise<AccountTokenInfo> {
  return apiGet<AccountTokenInfo>(`/api/auth/password-reset/${encodeURIComponent(token)}`);
}

export function confirmPasswordReset(token: string, password: string): Promise<void> {
  return apiPost<{ token: string; password: string }, void>("/api/auth/password-reset/confirm", {
    token,
    password,
  });
}

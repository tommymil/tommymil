export type UserRole = "admin" | "instructor" | "parent";

export type AuthUser = {
  id: string;
  email: string;
  role: UserRole;
  displayName: string;
};

export type AuthResponse = {
  token: string;
  expiresAt: string;
  user: AuthUser;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type ChangePasswordRequest = {
  currentPassword: string;
  newPassword: string;
};

/** Po co wydano token: reset hasła czy zaproszenie. Ekran mówi co innego w każdym przypadku. */
export type AccountTokenPurpose = "passwordreset" | "invitation";

export type AccountTokenInfo = {
  purpose: AccountTokenPurpose;
  email: string;
  displayName: string;
  expiresAt: string;
};

export type InvitationResult = {
  sent: boolean;
  expiresAt: string;
  error: string | null;
};

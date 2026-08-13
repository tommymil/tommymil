export type ManagedUser = {
  id: string;
  email: string;
  role: string;
  isActive: boolean;
  firstName: string | null;
  lastName: string | null;
  phone: string | null;
  displayName: string;
};

export type CreateUserRequest = {
  email: string;
  /** Puste = konto bez hasła; użytkownik ustawia je sam z zaproszenia. */
  password: string | null;
  role: string;
  firstName?: string | null;
  lastName?: string | null;
  phone?: string | null;
};

export type UpdateUserProfileRequest = {
  firstName: string | null;
  lastName: string | null;
  phone: string | null;
};

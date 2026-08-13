import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import type { PropsWithChildren } from "react";
import { getCurrentUser, login as loginRequest } from "../../api/authApi";
import { setAuthToken, setUnauthorizedHandler } from "../../api/client";
import type { AuthUser } from "../../types/auth";

const TOKEN_KEY = "lesson-runner:auth:token";
const USER_KEY = "lesson-runner:auth:user";

type AuthStatus = "loading" | "authenticated" | "anonymous";

type AuthContextValue = {
  user: AuthUser | null;
  status: AuthStatus;
  isAdmin: boolean;
  isParent: boolean;
  isStaff: boolean;
  signIn: (email: string, password: string) => Promise<AuthUser>;
  signOut: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

function readStoredUser(): AuthUser | null {
  const raw = localStorage.getItem(USER_KEY);

  if (!raw) {
    return null;
  }

  try {
    return JSON.parse(raw) as AuthUser;
  } catch {
    return null;
  }
}

function persistSession(token: string, user: AuthUser) {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(USER_KEY, JSON.stringify(user));
}

function clearSession() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

export function AuthProvider({ children }: PropsWithChildren) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [status, setStatus] = useState<AuthStatus>("loading");

  const signOut = useCallback(() => {
    clearSession();
    setAuthToken(null);
    setUser(null);
    setStatus("anonymous");
  }, []);

  // Wyloguj, gdy którykolwiek request zwroci 401.
  useEffect(() => {
    setUnauthorizedHandler(() => signOut());
    return () => setUnauthorizedHandler(null);
  }, [signOut]);

  // Odtworzenie sesji przy starcie i walidacja tokenu przez /me.
  useEffect(() => {
    const token = localStorage.getItem(TOKEN_KEY);
    const storedUser = readStoredUser();

    if (!token || !storedUser) {
      setStatus("anonymous");
      return;
    }

    setAuthToken(token);
    setUser(storedUser);

    let ignore = false;

    getCurrentUser()
      .then((fresh) => {
        if (ignore) {
          return;
        }
        persistSession(token, fresh);
        setUser(fresh);
        setStatus("authenticated");
      })
      .catch(() => {
        // 401 zostanie obsłużony przez handler (signOut); dla innych błędów też czyścimy sesje.
        if (!ignore) {
          signOut();
        }
      });

    return () => {
      ignore = true;
    };
  }, [signOut]);

  const signIn = useCallback(async (email: string, password: string) => {
    const response = await loginRequest({ email, password });
    persistSession(response.token, response.user);
    setAuthToken(response.token);
    setUser(response.user);
    setStatus("authenticated");
    return response.user;
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      status,
      isAdmin: user?.role === "admin",
      isParent: user?.role === "parent",
      isStaff: user?.role === "admin" || user?.role === "instructor",
      signIn,
      signOut,
    }),
    [user, status, signIn, signOut],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider.");
  }

  return context;
}

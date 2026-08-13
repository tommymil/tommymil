import { useEffect, useState } from "react";
import { ApiError } from "../../api/client";
import { createUser, getUsers, inviteUser, setUserActive, setUserPassword, updateUserProfile } from "../../api/usersApi";
import type { ManagedUser, UpdateUserProfileRequest } from "../../types/user";

export function useUsersViewModel() {
  const [users, setUsers] = useState<ManagedUser[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);
  const [notice, setNotice] = useState<string | null>(null);

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("instructor");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phone, setPhone] = useState("");

  useEffect(() => {
    let ignore = false;

    async function load() {
      try {
        setLoading(true);
        setError(null);
        const response = await getUsers();
        if (!ignore) {
          setUsers(response);
        }
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać kont.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    void load();

    return () => {
      ignore = true;
    };
  }, []);

  /**
   * Zakłada konto. Puste hasło jest dopuszczalne i oznacza konto z zaproszeniem: hasło ustawi
   * sam użytkownik z linku, a admin nigdy go nie zna. Podane hasło nadal musi mieć 8 znaków.
   */
  async function addUser() {
    const withoutPassword = password.length === 0;

    if (!email.trim() || (!withoutPassword && password.length < 8)) {
      setError("Podaj e-mail oraz hasło o długości min. 8 znaków (albo zostaw hasło puste i wyślij zaproszenie).");
      return;
    }

    try {
      setBusy(true);
      setError(null);
      setNotice(null);
      const created = await createUser({
        email: email.trim(),
        password: withoutPassword ? null : password,
        role,
        firstName: firstName.trim() || null,
        lastName: lastName.trim() || null,
        phone: phone.trim() || null,
      });
      setUsers((current) => [...current, created].sort((a, b) => a.email.localeCompare(b.email)));

      // Konto bez hasła jest bezużyteczne, dopóki nie wyjdzie zaproszenie — wysyłamy je od razu,
      // żeby nie zostawiać kont, do których nikt nie może się zalogować.
      if (withoutPassword) {
        // Osobne przechwycenie: konto już powstało, więc błąd wysyłki nie może być zgłoszony
        // jako „nie udało się utworzyć konta” — admin zakładałby je drugi raz.
        try {
          await inviteUser(created.id);
          setNotice(`Konto utworzone. Zaproszenie wysłane na ${created.email}.`);
        } catch (caught) {
          setError(
            `Konto utworzone, ale zaproszenie nie wyszło: ${
              caught instanceof ApiError ? caught.message : "błąd wysyłki"
            }. Wyślij je ponownie z listy.`,
          );
        }
      } else {
        setNotice("Konto utworzone.");
      }

      setEmail("");
      setPassword("");
      setRole("instructor");
      setFirstName("");
      setLastName("");
      setPhone("");
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się utworzyć konta.");
    } finally {
      setBusy(false);
    }
  }

  async function saveProfile(id: string, request: UpdateUserProfileRequest): Promise<boolean> {
    try {
      setBusy(true);
      setError(null);
      await updateUserProfile(id, request);
      setUsers((current) =>
        current.map((item) =>
          item.id === id
            ? {
                ...item,
                firstName: request.firstName,
                lastName: request.lastName,
                phone: request.phone,
                displayName: `${request.firstName ?? ""} ${request.lastName ?? ""}`.trim() || item.email,
              }
            : item,
        ),
      );
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać profilu.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  async function resetPassword(id: string, newPassword: string): Promise<boolean> {
    if (newPassword.length < 8) {
      setError("Nowe hasło musi mieć co najmniej 8 znaków.");
      return false;
    }

    try {
      setBusy(true);
      setError(null);
      await setUserPassword(id, newPassword);
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się ustawić hasła.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  async function invite(user: ManagedUser): Promise<boolean> {
    try {
      setBusy(true);
      setError(null);
      setNotice(null);
      await inviteUser(user.id);
      setNotice(`Zaproszenie wysłane na ${user.email}. Link jest ważny 7 dni.`);
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się wysłać zaproszenia.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  async function toggleActive(user: ManagedUser) {
    try {
      setBusy(true);
      setError(null);
      await setUserActive(user.id, !user.isActive);
      setUsers((current) => current.map((item) => (item.id === user.id ? { ...item, isActive: !item.isActive } : item)));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zmienić statusu konta.");
    } finally {
      setBusy(false);
    }
  }

  return {
    users,
    loading,
    error,
    notice,
    busy,
    email,
    setEmail,
    password,
    setPassword,
    role,
    setRole,
    firstName,
    setFirstName,
    lastName,
    setLastName,
    phone,
    setPhone,
    addUser,
    saveProfile,
    toggleActive,
    resetPassword,
    invite,
  };
}

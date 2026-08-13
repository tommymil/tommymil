import { useState } from "react";
import type { FormEvent } from "react";
import { KeyRound, ShieldCheck, UserRound } from "lucide-react";
import { changePassword } from "../api/authApi";
import { ApiError } from "../api/client";
import { Button } from "../components/ui/Button";
import { useAuth } from "../features/auth/AuthContext";
import { useToast } from "../features/toast/ToastContext";

export function ProfilePage() {
  const { user, isAdmin } = useAuth();
  const toast = useToast();
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (newPassword.length < 8) {
      setError("Nowe hasło musi mieć co najmniej 8 znaków.");
      return;
    }

    if (newPassword !== confirmPassword) {
      setError("Powtórzone hasło nie zgadza się z nowym hasłem.");
      return;
    }

    setSubmitting(true);

    try {
      await changePassword({ currentPassword, newPassword });
      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");
      toast.success("Hasło zostało zmienione.");
    } catch (caught) {
      if (caught instanceof ApiError) {
        setError(caught.message);
      } else {
        setError("Nie udało się zmienić hasła.");
      }
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <section className="profile-page">
      <header className="page-header">
        <div>
          <span className="eyebrow">Moje konto</span>
          <h1>Moj profil</h1>
          <p>Dane zalogowanego konta i zmiana hasła.</p>
        </div>
      </header>

      <div className="profile-grid">
        <section className="profile-panel" aria-labelledby="profile-details-heading">
          <div className="profile-panel-head">
            <UserRound size={22} aria-hidden="true" />
            <div>
              <h2 id="profile-details-heading">Dane konta</h2>
              <p>{user?.displayName || user?.email}</p>
            </div>
          </div>

          <dl className="profile-details">
            <div>
              <dt>Email</dt>
              <dd>{user?.email}</dd>
            </div>
            <div>
              <dt>Rola</dt>
              <dd>{isAdmin ? "Administrator" : "Instruktor"}</dd>
            </div>
            <div>
              <dt>Status</dt>
              <dd>
                <ShieldCheck size={16} aria-hidden="true" />
                Konto aktywne
              </dd>
            </div>
          </dl>
        </section>

        <section className="profile-panel" aria-labelledby="change-password-heading">
          <div className="profile-panel-head">
            <KeyRound size={22} aria-hidden="true" />
            <div>
              <h2 id="change-password-heading">Zmiana hasła</h2>
              <p>Użyj obecnego hasła, aby ustawić nowe.</p>
            </div>
          </div>

          <form className="profile-form" onSubmit={handleSubmit}>
            <label className="form-field" htmlFor="current-password">
              <span>Obecne hasło</span>
              <input
                id="current-password"
                type="password"
                autoComplete="current-password"
                required
                value={currentPassword}
                onChange={(event) => setCurrentPassword(event.target.value)}
              />
            </label>

            <label className="form-field" htmlFor="new-password">
              <span>Nowe hasło</span>
              <input
                id="new-password"
                type="password"
                autoComplete="new-password"
                minLength={8}
                required
                value={newPassword}
                onChange={(event) => setNewPassword(event.target.value)}
              />
            </label>

            <label className="form-field" htmlFor="confirm-password">
              <span>Powtórz nowe hasło</span>
              <input
                id="confirm-password"
                type="password"
                autoComplete="new-password"
                minLength={8}
                required
                value={confirmPassword}
                onChange={(event) => setConfirmPassword(event.target.value)}
              />
            </label>

            {error ? (
              <p className="form-error" role="alert">
                {error}
              </p>
            ) : null}

            <Button type="submit" disabled={submitting}>
              {submitting ? "Zapisywanie..." : "Zmień hasło"}
            </Button>
          </form>
        </section>
      </div>
    </section>
  );
}

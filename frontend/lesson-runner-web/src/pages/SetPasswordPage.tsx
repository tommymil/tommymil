import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { confirmPasswordReset, describeAccountToken } from "../api/authApi";
import { ApiError } from "../api/client";
import type { AccountTokenInfo } from "../types/auth";

/**
 * Ekran ustawiania hasła z linku (reset albo zaproszenie).
 *
 * Token siedzi w adresie, więc najpierw pytamy backend, czy w ogóle żyje — inaczej rodzic
 * wpisywałby hasło dwa razy tylko po to, żeby dowiedzieć się, że link stracił ważność.
 */
export function SetPasswordPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = searchParams.get("token") ?? "";

  const [info, setInfo] = useState<AccountTokenInfo | null>(null);
  const [checking, setChecking] = useState(true);
  const [password, setPassword] = useState("");
  const [repeated, setRepeated] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [done, setDone] = useState(false);

  useEffect(() => {
    let ignore = false;

    async function check() {
      if (!token) {
        if (!ignore) {
          setChecking(false);
        }
        return;
      }

      try {
        const response = await describeAccountToken(token);
        if (!ignore) {
          setInfo(response);
        }
      } catch {
        if (!ignore) {
          setInfo(null);
        }
      } finally {
        if (!ignore) {
          setChecking(false);
        }
      }
    }

    void check();

    return () => {
      ignore = true;
    };
  }, [token]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (password.length < 8) {
      setError("Hasło musi mieć co najmniej 8 znaków.");
      return;
    }

    if (password !== repeated) {
      setError("Hasła nie są takie same.");
      return;
    }

    setSubmitting(true);

    try {
      await confirmPasswordReset(token, password);
      setDone(true);
      // Krótka pauza, żeby komunikat zdążył zostać przeczytany, i wracamy na logowanie.
      window.setTimeout(() => navigate("/", { replace: true }), 2500);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się ustawić hasła.");
    } finally {
      setSubmitting(false);
    }
  }

  const invitation = info?.purpose === "invitation";

  return (
    <section className="login-page">
      <div className="login-panel">
        <span className="eyebrow">Szkoła Programowania</span>

        {checking ? (
          <p>Sprawdzanie linku...</p>
        ) : done ? (
          <>
            <h1>Hasło ustawione</h1>
            <p>Możesz się teraz zalogować. Za chwilę przeniesiemy Cię na ekran logowania.</p>
            <Link to="/">Przejdź do logowania</Link>
          </>
        ) : !info ? (
          <>
            <h1>Link stracił ważność</h1>
            <p>
              Ten link został już użyty albo minął czas jego ważności. Poproś o nowy — wystarczy
              formularz „Nie pamiętam hasła”.
            </p>
            <Link to="/reset-password">Poproś o nowy link</Link>
          </>
        ) : (
          <>
            <h1>{invitation ? "Witamy w panelu rodzica" : "Ustaw nowe hasło"}</h1>
            <p>
              {invitation
                ? "Ustaw hasło do swojego konta, żeby zobaczyć harmonogram zajęć, frekwencję i rozliczenia."
                : "Ustaw nowe hasło do swojego konta."}{" "}
              Konto: <strong>{info.email}</strong>.
            </p>

            <form className="login-form" onSubmit={handleSubmit}>
              <label htmlFor="new-password">Nowe hasło (min. 8 znaków)</label>
              <input
                id="new-password"
                type="password"
                autoComplete="new-password"
                required
                value={password}
                onChange={(event) => setPassword(event.target.value)}
              />

              <label htmlFor="repeat-password">Powtórz hasło</label>
              <input
                id="repeat-password"
                type="password"
                autoComplete="new-password"
                required
                value={repeated}
                onChange={(event) => setRepeated(event.target.value)}
              />

              {error ? (
                <p className="form-error" role="alert">
                  {error}
                </p>
              ) : null}

              <Button type="submit" disabled={submitting}>
                {submitting ? "Zapisywanie..." : "Ustaw hasło"}
              </Button>
            </form>
          </>
        )}
      </div>
    </section>
  );
}

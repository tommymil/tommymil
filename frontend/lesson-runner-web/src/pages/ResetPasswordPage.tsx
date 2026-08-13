import { useState } from "react";
import type { FormEvent } from "react";
import { Link } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { requestPasswordReset } from "../api/authApi";
import { ApiError } from "../api/client";

/**
 * Formularz „nie pamiętam hasła”.
 *
 * Po wysłaniu pokazujemy zawsze ten sam komunikat — także dla adresu, którego nie ma w bazie.
 * Backend odpowiada identycznie w obu przypadkach i ekran nie może tego psuć, bo inaczej
 * formularz stałby się sprawdzaczem, kto korzysta ze szkoły.
 */
export function ResetPasswordPage() {
  const [email, setEmail] = useState("");
  const [sent, setSent] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setSubmitting(true);

    try {
      await requestPasswordReset(email.trim());
      setSent(true);
    } catch (caught) {
      setError(
        caught instanceof ApiError && caught.status === 429
          ? "Zbyt wiele prób. Spróbuj ponownie za kilka minut."
          : "Nie udało się wysłać wiadomości. Spróbuj ponownie za chwilę.",
      );
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <section className="login-page">
      <div className="login-panel">
        <span className="eyebrow">Szkoła Programowania</span>
        <h1>Nie pamiętam hasła</h1>

        {sent ? (
          <>
            <p>
              Jeśli konto o podanym adresie istnieje, wysłaliśmy na nie link do ustawienia nowego
              hasła. Link jest ważny 2 godziny i zadziała tylko raz.
            </p>
            <p className="login-hint">
              Nic nie przyszło? Sprawdź folder ze spamem albo poproś administratora o zaproszenie.
            </p>
            <Link to="/">Wróć do logowania</Link>
          </>
        ) : (
          <>
            <p>Podaj adres e-mail konta. Wyślemy na niego link do ustawienia nowego hasła.</p>

            <form className="login-form" onSubmit={handleSubmit}>
              <label htmlFor="reset-email">Email</label>
              <input
                id="reset-email"
                type="email"
                autoComplete="username"
                required
                value={email}
                onChange={(event) => setEmail(event.target.value)}
              />

              {error ? (
                <p className="form-error" role="alert">
                  {error}
                </p>
              ) : null}

              <Button type="submit" disabled={submitting}>
                {submitting ? "Wysyłanie..." : "Wyślij link"}
              </Button>
            </form>

            <p className="login-hint">
              <Link to="/">Wróć do logowania</Link>
            </p>
          </>
        )}
      </div>
    </section>
  );
}

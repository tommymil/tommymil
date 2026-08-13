import { useState } from "react";
import type { FormEvent } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { useAuth } from "../features/auth/AuthContext";
import { ApiError } from "../api/client";
import type { AuthUser } from "../types/auth";

function homeForRole(user: AuthUser): string {
  if (user.role === "admin") {
    return "/admin/dashboard";
  }

  if (user.role === "parent") {
    return "/parent/portal";
  }

  return "/instructor/schedule";
}

export function LoginPage() {
  const { status, user, signIn } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  if (status === "authenticated" && user) {
    return <Navigate to={homeForRole(user)} replace />;
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setSubmitting(true);

    try {
      const signedIn = await signIn(email.trim(), password);
      navigate(homeForRole(signedIn), { replace: true });
    } catch (caught) {
      if (caught instanceof ApiError && caught.status === 401) {
        setError("Nieprawidłowy email lub hasło.");
      } else {
        setError("Logowanie nie powiodło się. Spróbuj ponownie.");
      }
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <section className="login-page">
      <div className="login-panel">
        <span className="eyebrow">Szkoła Programowania</span>
        <h1>Zaloguj się, aby prowadzić lekcje</h1>
        <p>
          Administratorzy tworzą konspekty, instruktorzy prowadzą gotowe lekcje, a rodzice
          śledzą postępy dzieci w swoim portalu.
        </p>

        <form className="login-form" onSubmit={handleSubmit}>
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="email"
            autoComplete="username"
            required
            value={email}
            onChange={(event) => setEmail(event.target.value)}
          />

          <label htmlFor="password">Hasło</label>
          <input
            id="password"
            type="password"
            autoComplete="current-password"
            required
            value={password}
            onChange={(event) => setPassword(event.target.value)}
          />

          {error ? (
            <p className="form-error" role="alert">
              {error}
            </p>
          ) : null}

          <Button type="submit" disabled={submitting}>
            {submitting ? "Logowanie..." : "Zaloguj"}
          </Button>
        </form>

        <p className="login-hint">
          <Link to="/reset-password">Nie pamiętam hasła</Link>
        </p>

        {import.meta.env.DEV ? (
          <>
            {/* Trzy konta, bo w systemie są trzy drogi. Rodzica tu brakowało, więc portal
                rodzica wyglądał na niedokończony - nie było się jak do niego zalogować. */}
            <p className="login-hint">
              Konta developerskie: <code>admin@lessonrunner.local / admin12345</code>,{" "}
              <code>instructor@lessonrunner.local / teacher12345</code>,{" "}
              <code>parent@lessonrunner.local / parent12345</code>.
            </p>
          </>
        ) : null}
      </div>
    </section>
  );
}

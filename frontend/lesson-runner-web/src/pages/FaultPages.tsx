import { Link, isRouteErrorResponse, useNavigate, useRouteError } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { useAuth } from "../features/auth/AuthContext";

/** Ekran startowy właściwy dla roli — używany jako „wyjście awaryjne” ze wszystkich błędów. */
function useHomePath(): string {
  const { user, isAdmin, isParent } = useAuth();

  if (!user) {
    return "/";
  }

  if (isAdmin) {
    return "/admin/dashboard";
  }

  return isParent ? "/parent/portal" : "/instructor/schedule";
}

/**
 * Ekran nieznanego adresu.
 *
 * Wcześniej trasy `*` nie było w ogóle: literówka w adresie albo stary link z e-maila
 * kończyły się pustym ekranem bez żadnej wskazówki.
 */
export function NotFoundPage() {
  const home = useHomePath();

  return (
    <section className="page-section">
      <div className="fault-page">
        <span className="fault-code" aria-hidden="true">
          404
        </span>
        <h1>Nie znaleźliśmy tej strony</h1>
        <p>
          Adres jest nieprawidłowy albo zasób został usunięty. Jeśli trafiłeś tu z linku
          w wiadomości, mógł się już zdezaktualizować.
        </p>
        <div className="fault-actions">
          <Link to={home}>
            <Button>Wróć do panelu</Button>
          </Link>
        </div>
      </div>
    </section>
  );
}

/**
 * Ekran braku uprawnień.
 *
 * Świadomie nie mówi, co znajduje się pod tym adresem — sam fakt istnienia grupy albo
 * dziecka bywa informacją, do której użytkownik nie ma prawa. Ta sama zasada rządzi
 * odpowiedziami API (404 zamiast 403 przy cudzych zasobach).
 */
export function ForbiddenPage() {
  const home = useHomePath();

  return (
    <section className="page-section">
      <div className="fault-page">
        <span className="fault-code" aria-hidden="true">
          403
        </span>
        <h1>Nie masz dostępu do tej części systemu</h1>
        <p>
          Twoja rola nie obejmuje tego widoku. Jeśli uważasz, że to pomyłka, skontaktuj się
          z administratorem systemu.
        </p>
        <div className="fault-actions">
          <Link to={home}>
            <Button>Wróć do panelu</Button>
          </Link>
        </div>
      </div>
    </section>
  );
}

/**
 * Ostatnia linia obrony: błąd renderowania albo błąd ładowania trasy.
 *
 * Bez `errorElement` React Router pokazuje własny, angielski ekran diagnostyczny,
 * który dla rodzica wygląda jak awaria serwera.
 */
export function RouteErrorPage() {
  const error = useRouteError();
  const navigate = useNavigate();

  if (isRouteErrorResponse(error) && error.status === 404) {
    return <NotFoundPage />;
  }

  const detail = error instanceof Error ? error.message : null;

  return (
    <section className="page-section">
      <div className="fault-page">
        <span className="fault-code" aria-hidden="true">
          !
        </span>
        <h1>Coś poszło nie tak</h1>
        <p>
          Nie udało się wyświetlić tego widoku. Spróbuj odświeżyć stronę — jeśli błąd się
          powtarza, przekaż administratorowi treść komunikatu poniżej.
        </p>
        {detail ? <p className="cue-empty">{detail}</p> : null}
        <div className="fault-actions">
          <Button onClick={() => navigate(0)}>Odśwież</Button>
          <Button variant="secondary" onClick={() => navigate("/", { replace: true })}>
            Strona główna
          </Button>
        </div>
      </div>
    </section>
  );
}

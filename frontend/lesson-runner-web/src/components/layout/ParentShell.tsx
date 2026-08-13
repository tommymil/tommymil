import type { PropsWithChildren, ReactNode } from "react";
import { CalendarDays, CreditCard, LogOut, TrendingUp, UserRound } from "lucide-react";
import { NavLink } from "react-router-dom";
import { Button } from "../ui/Button";
import type { AuthUser } from "../../types/auth";

type ParentShellProps = PropsWithChildren<{
  user: AuthUser;
  onSignOut: () => void;
  themeToggle: ReactNode;
}>;

/**
 * Układ portalu rodzica.
 *
 * Rodzic dostawał wcześniej ten sam szkielet co administrator: 292-pikselowy pasek
 * boczny z dwiema pozycjami, belkę z napisem „Dostępne opcje są po lewej stronie”
 * i chłodną paletę panelu roboczego. Poniżej 760 px pasek rozwijał się w blok
 * kafelków nad treścią, więc na telefonie trzeba było przewinąć pół ekranu, zanim
 * pojawiła się pierwsza informacja.
 *
 * Tu jest odwrotnie: górna belka zamiast paska, dolna nawigacja na telefonie,
 * ciepła skóra (`data-skin="parent"`) i większy tekst bazowy. To ten sam system
 * tokenów — zmienia się wyłącznie warstwa semantyczna.
 *
 * Nawigacja prowadzi do zakładek jednego ekranu (`?widok=...`), a nie do osobnych
 * tras: portal jest jedną stroną z hierarchią, a nie zestawem podstron, więc
 * cofnięcie w przeglądarce ma wracać do poprzedniej zakładki.
 */
const parentTabs = [
  { key: "plan", label: "Plan", icon: CalendarDays },
  { key: "postepy", label: "Postępy", icon: TrendingUp },
  { key: "rozliczenia", label: "Rozliczenia", icon: CreditCard },
] as const;

export function ParentShell({ user, onSignOut, themeToggle, children }: ParentShellProps) {
  return (
    <div className="parent-shell" data-skin="parent">
      <a className="skip-link" href="#main-content">
        Przejdź do treści
      </a>

      <header className="parent-topbar">
        <NavLink to="/parent/portal" className="brand">
          <span className="brand-mark">SP</span>
          <span>
            <strong>Szkoła Programowania</strong>
            <small>Portal rodzica</small>
          </span>
        </NavLink>

        <div className="parent-topbar-actions">
          {themeToggle}
          <NavLink to="/profile" className="parent-account">
            <UserRound size={18} aria-hidden="true" />
            <span>{user.displayName || user.email}</span>
          </NavLink>
          <Button variant="ghost" onClick={onSignOut}>
            <LogOut className="button-icon" aria-hidden="true" />
            <span className="parent-signout-label">Wyloguj</span>
          </Button>
        </div>
      </header>

      <main className="parent-frame" id="main-content">
        {children}
      </main>

      {/* Dolna nawigacja pojawia się wyłącznie na wąskich ekranach - tam, gdzie
          rodzic korzysta z portalu najczęściej. */}
      <nav className="parent-bottom-nav" aria-label="Sekcje portalu">
        {parentTabs.map((tab) => {
          const Icon = tab.icon;

          return (
            <NavLink
              key={tab.key}
              to={`/parent/portal?widok=${tab.key}`}
              className={({ isActive }) =>
                isActive && currentView() === tab.key ? "parent-bottom-link is-active" : "parent-bottom-link"
              }
            >
              <Icon size={20} aria-hidden="true" />
              <span>{tab.label}</span>
            </NavLink>
          );
        })}
        <NavLink
          to="/profile"
          className={({ isActive }) => (isActive ? "parent-bottom-link is-active" : "parent-bottom-link")}
        >
          <UserRound size={20} aria-hidden="true" />
          <span>Konto</span>
        </NavLink>
      </nav>
    </div>
  );
}

function currentView(): string {
  return new URLSearchParams(window.location.search).get("widok") ?? "plan";
}

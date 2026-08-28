import { useEffect, useState } from "react";
import type { PropsWithChildren } from "react";
import { LogOut, Menu, Moon, PanelLeftClose, PanelLeftOpen, Sun, X } from "lucide-react";
import { NavLink, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../../features/auth/AuthContext";
import { useTheme } from "../../features/theme/useTheme";
import { Button } from "../ui/Button";
import { GlobalSearch } from "./GlobalSearch";
import { ParentShell } from "./ParentShell";
import { crumbsForPath, sidebarSections } from "./navigation";
import type { SidebarItem } from "./navigation";

const SIDEBAR_COLLAPSED_KEY = "lesson-runner:sidebar-collapsed";

export function AppShell({ children }: PropsWithChildren) {
  const { user, status, isAdmin, isParent, isStaff, signOut } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();
  const location = useLocation();
  const [menuOpen, setMenuOpen] = useState(false);
  const [sidebarCollapsed, setSidebarCollapsed] = useState(
    () => localStorage.getItem(SIDEBAR_COLLAPSED_KEY) === "true",
  );

  const isAuthenticated = status === "authenticated" && Boolean(user);

  // Szuflada zamyka się przy każdej zmianie trasy - inaczej po kliknięciu pozycji
  // menu na telefonie zasłaniałaby ekran, na który użytkownik właśnie wszedł.
  useEffect(() => {
    setMenuOpen(false);
  }, [location.pathname]);

  function handleSignOut() {
    signOut();
    navigate("/", { replace: true });
  }

  function collapseSidebar() {
    setSidebarCollapsed(true);
    localStorage.setItem(SIDEBAR_COLLAPSED_KEY, "true");
  }

  function expandSidebar() {
    setSidebarCollapsed(false);
    localStorage.setItem(SIDEBAR_COLLAPSED_KEY, "false");
  }

  const themeToggle = (
    <Button
      variant="secondary"
      className="theme-toggle"
      onClick={toggleTheme}
      aria-label={theme === "dark" ? "Włącz tryb jasny" : "Włącz tryb ciemny"}
    >
      {theme === "dark" ? (
        <Sun className="button-icon" aria-hidden="true" />
      ) : (
        <Moon className="button-icon" aria-hidden="true" />
      )}
      {theme === "dark" ? "Jasny" : "Ciemny"}
    </Button>
  );

  // Rodzic dostaje własny układ: bez bocznego menu, z dolną nawigacją na telefonie
  // i cieplejszą skórą. To klient szkoły, a nie użytkownik panelu administracyjnego.
  if (isAuthenticated && isParent && user) {
    return (
      <ParentShell user={user} onSignOut={handleSignOut} themeToggle={themeToggle}>
        {children}
      </ParentShell>
    );
  }

  if (!isAuthenticated || !user) {
    return (
      <div className="app-shell app-shell-public">
        <a className="skip-link" href="#main-content">
          Przejdź do treści
        </a>
        <div className="app-main">
          <header className="top-bar">
            <NavLink to="/" className="brand">
              <span className="brand-mark">SP</span>
              <span>
                <strong>Szkoła Programowania</strong>
                <small>Zajęcia online dla dzieci</small>
              </span>
            </NavLink>
            <div className="top-bar-right">{themeToggle}</div>
          </header>
          <main className="page-frame" id="main-content">
            {children}
          </main>
        </div>
      </div>
    );
  }

  const visibleSections = sidebarSections
    .map((section) => ({
      ...section,
      items: section.items.filter((item) => (!item.adminOnly || isAdmin) && (!item.staffOnly || isStaff)),
    }))
    .filter((section) => section.items.length > 0);

  const home = isAdmin ? "/admin/dashboard" : "/instructor/schedule";
  const crumbs = crumbsForPath(location.pathname);

  return (
    <div
      className={`app-shell${menuOpen ? " app-shell-menu-open" : ""}${
        sidebarCollapsed ? " app-shell-sidebar-collapsed" : ""
      }`}
    >
      <a className="skip-link" href="#main-content">
        Przejdź do treści
      </a>

      <aside className="sidebar" aria-label="Nawigacja systemu" id="nawigacja">
        <div className="sidebar-head">
          <NavLink to={home} className="brand">
            <span className="brand-mark">SP</span>
            <span>
              <strong>Szkoła Programowania</strong>
              <small>Zajęcia online dla dzieci</small>
            </span>
          </NavLink>
          <button
            type="button"
            className="sidebar-close"
            onClick={() => setMenuOpen(false)}
            aria-label="Zamknij menu"
          >
            <X size={18} aria-hidden="true" />
          </button>
          <button
            type="button"
            className="sidebar-collapse"
            onClick={collapseSidebar}
            aria-label="Ukryj boczny pasek nawigacji"
            aria-controls="nawigacja"
            title="Ukryj pasek nawigacji"
          >
            <PanelLeftClose size={19} aria-hidden="true" />
          </button>
        </div>

        <nav className="side-nav" aria-label="Dostępne opcje">
          {visibleSections.map((section) => (
            <section className="side-nav-section" key={section.title}>
              <h2>{section.title}</h2>
              <div className="side-nav-list">
                {section.items.map((item) => (
                  <SidebarLink item={item} key={item.to} />
                ))}
              </div>
            </section>
          ))}
        </nav>
      </aside>

      {menuOpen ? (
        <button
          type="button"
          className="sidebar-scrim"
          aria-label="Zamknij menu"
          onClick={() => setMenuOpen(false)}
        />
      ) : null}

      <div className="app-main">
        <header className="top-bar">
          <button
            type="button"
            className="sidebar-open"
            aria-label="Pokaż boczny pasek nawigacji"
            aria-controls="nawigacja"
            onClick={expandSidebar}
            title="Pokaż pasek nawigacji"
          >
            <PanelLeftOpen size={20} aria-hidden="true" />
          </button>
          <button
            type="button"
            className="menu-toggle"
            aria-label="Otwórz menu"
            aria-controls="nawigacja"
            aria-expanded={menuOpen}
            onClick={() => setMenuOpen(true)}
          >
            <Menu size={20} aria-hidden="true" />
          </button>

          {/* Belka pokazuje, gdzie użytkownik jest, a nie gdzie jest nawigacja.
              Wcześniej stał tu napis „Dostępne opcje są po lewej stronie” — czyli
              instrukcja obsługi na najlepiej widocznym miejscu w aplikacji. */}
          <nav className="breadcrumbs" aria-label="Ścieżka nawigacji">
            <NavLink to={home}>{isAdmin ? "Administracja" : "Panel instruktora"}</NavLink>
            {crumbs.map((crumb, index) => (
              <span key={`${crumb.label}-${index}`} className="breadcrumb-item">
                <span className="breadcrumbs-sep" aria-hidden="true">
                  ›
                </span>
                {crumb.to ? (
                  <NavLink to={crumb.to}>{crumb.label}</NavLink>
                ) : (
                  <span aria-current="page">{crumb.label}</span>
                )}
              </span>
            ))}
          </nav>

          <GlobalSearch />

          <div className="top-bar-right">
            {themeToggle}
            <div className="user-box">
              <span className="user-meta">
                <strong>{user.displayName || user.email}</strong>
                <small>{isAdmin ? "Administrator" : "Instruktor"}</small>
              </span>
              <Button variant="ghost" onClick={handleSignOut}>
                <LogOut className="button-icon" aria-hidden="true" />
                Wyloguj
              </Button>
            </div>
          </div>
        </header>

        <main className="page-frame" id="main-content">
          {children}
        </main>
      </div>
    </div>
  );
}

function SidebarLink({ item }: { item: SidebarItem }) {
  const Icon = item.icon;

  return (
    <NavLink
      to={item.to}
      end={item.end}
      className={({ isActive }) => (isActive ? "side-nav-link side-nav-link-active" : "side-nav-link")}
    >
      <span className="side-nav-icon" aria-hidden="true">
        <Icon size={19} strokeWidth={2.2} />
      </span>
      <span className="side-nav-text">
        <strong>{item.label}</strong>
        <small>{item.description}</small>
      </span>
    </NavLink>
  );
}

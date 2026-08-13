import type { PropsWithChildren } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "./AuthContext";
import { ForbiddenPage } from "../../pages/FaultPages";
import { SkeletonList } from "../../components/ui/Skeleton";

function AuthLoading() {
  return (
    <section className="page-section">
      <SkeletonList rows={2} label="Sprawdzanie sesji" />
    </section>
  );
}

export function RequireAuth({ children }: PropsWithChildren) {
  const { status } = useAuth();

  if (status === "loading") {
    return <AuthLoading />;
  }

  if (status === "anonymous") {
    return <Navigate to="/" replace />;
  }

  return children ? <>{children}</> : <Outlet />;
}

/**
 * Zalogowany użytkownik bez wymaganej roli dostaje ekran 403, a nie ciche przekierowanie.
 *
 * Wcześniej strażnicy odsyłali na sztywno wpisane adresy (`/instructor/lessons`,
 * `/admin/dashboard`), przez co rodzic klikający nieaktualny link z e-maila
 * przechodził przez dwa przekierowania i lądował w swoim portalu bez żadnego
 * wyjaśnienia — wyglądało to jak zgubiony link, a nie jak brak uprawnień.
 */
function RoleGuard({ allowed, children }: PropsWithChildren<{ allowed: boolean }>) {
  const { status } = useAuth();

  if (status === "loading") {
    return <AuthLoading />;
  }

  if (status === "anonymous") {
    return <Navigate to="/" replace />;
  }

  if (!allowed) {
    return <ForbiddenPage />;
  }

  return children ? <>{children}</> : <Outlet />;
}

export function RequireAdmin({ children }: PropsWithChildren) {
  const { isAdmin } = useAuth();
  return <RoleGuard allowed={isAdmin}>{children}</RoleGuard>;
}

export function RequireParent({ children }: PropsWithChildren) {
  const { isParent } = useAuth();
  return <RoleGuard allowed={isParent}>{children}</RoleGuard>;
}

export function RequireStaff({ children }: PropsWithChildren) {
  const { isStaff } = useAuth();
  return <RoleGuard allowed={isStaff}>{children}</RoleGuard>;
}

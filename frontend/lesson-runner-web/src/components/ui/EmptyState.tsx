import type { ReactNode } from "react";
import type { LucideIcon } from "lucide-react";
import { Inbox } from "lucide-react";

type EmptyStateProps = {
  title: string;
  description?: string;
  icon?: LucideIcon;
  /** Jedno działanie wyprowadzające ze stanu pustego. Więcej niż jedno rozprasza. */
  action?: ReactNode;
  compact?: boolean;
};

/**
 * Stan pusty z ikoną, zdaniem wyjaśniającym i jednym działaniem.
 *
 * Wcześniej większość ekranów kończyła się szarym akapitem („Brak faktur.”).
 * Taki komunikat mówi, czego nie ma, ale nie mówi, co z tym zrobić ani czy to
 * stan normalny — a przy nowej szkole prawie wszystkie listy startują puste.
 */
export function EmptyState({ title, description, icon, action, compact = false }: EmptyStateProps) {
  const Icon = icon ?? Inbox;

  return (
    <div className={`empty-state${compact ? " empty-state-compact" : ""}`}>
      <span className="empty-state-icon" aria-hidden="true">
        <Icon size={24} />
      </span>
      <h3>{title}</h3>
      {description ? <p>{description}</p> : null}
      {action}
    </div>
  );
}

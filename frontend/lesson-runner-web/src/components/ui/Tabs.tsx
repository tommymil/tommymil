import { useId } from "react";

export type TabDefinition<TValue extends string> = {
  value: TValue;
  label: string;
  /** Licznik obok etykiety — pokazuje wagę zakładki bez jej otwierania. */
  count?: number;
};

type TabsProps<TValue extends string> = {
  value: TValue;
  tabs: TabDefinition<TValue>[];
  onChange: (value: TValue) => void;
  ariaLabel: string;
};

/**
 * Zakładki z obsługą strzałek, Home i End.
 *
 * Używane tam, gdzie jeden ekran obsługiwał kilka niezależnych obszarów naraz:
 * portal rodzica (siedem sekcji rozwiniętych jednocześnie) i szczegóły grupy
 * (ustawienia, terminy, historia, uczestnicy i frekwencja na jednej stronie).
 */
export function Tabs<TValue extends string>({ value, tabs, onChange, ariaLabel }: TabsProps<TValue>) {
  const baseId = useId();

  function handleKeyDown(event: React.KeyboardEvent<HTMLDivElement>) {
    const index = tabs.findIndex((tab) => tab.value === value);

    if (index < 0) {
      return;
    }

    const moves: Record<string, number> = {
      ArrowRight: index + 1,
      ArrowLeft: index - 1,
      Home: 0,
      End: tabs.length - 1,
    };

    const target = moves[event.key];

    if (target === undefined) {
      return;
    }

    event.preventDefault();
    const next = tabs[(target + tabs.length) % tabs.length];
    onChange(next.value);
    document.getElementById(`${baseId}-${next.value}`)?.focus();
  }

  return (
    <div className="tabs" role="tablist" aria-label={ariaLabel} onKeyDown={handleKeyDown}>
      {tabs.map((tab) => (
        <button
          key={tab.value}
          id={`${baseId}-${tab.value}`}
          type="button"
          role="tab"
          aria-selected={tab.value === value}
          tabIndex={tab.value === value ? 0 : -1}
          onClick={() => onChange(tab.value)}
        >
          {tab.label}
          {tab.count !== undefined ? <span className="tab-count">{tab.count}</span> : null}
        </button>
      ))}
    </div>
  );
}

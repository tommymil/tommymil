import { useEffect, useRef, useState } from "react";
import { MoreHorizontal } from "lucide-react";

export type ActionMenuItem = {
  label: string;
  onSelect: () => void;
  danger?: boolean;
  disabled?: boolean;
  /** Kreska oddzielająca od poprzedniej pozycji — dla operacji nieodwracalnych. */
  separatorBefore?: boolean;
};

type ActionMenuProps = {
  items: ActionMenuItem[];
  /** Etykieta musi wskazywać, czego dotyczy menu (np. „Akcje terminu #3”). */
  ariaLabel: string;
  disabled?: boolean;
};

/**
 * Menu akcji dla wiersza listy.
 *
 * Zastępuje wzorzec `<select value="">Zmień status…</select>`, w którym wybór pozycji
 * w liście rozwijanej **wykonywał operację natychmiast** — bez potwierdzenia i bez
 * możliwości cofnięcia. Lista rozwijana jest kontrolką wyboru wartości; użytkownik
 * słusznie zakłada, że rozwinięcie jej niczego jeszcze nie przesądza.
 */
export function ActionMenu({ items, ariaLabel, disabled = false }: ActionMenuProps) {
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) {
      return;
    }

    function handlePointerDown(event: MouseEvent) {
      if (!containerRef.current?.contains(event.target as Node)) {
        setOpen(false);
      }
    }

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setOpen(false);
      }
    }

    document.addEventListener("mousedown", handlePointerDown);
    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("mousedown", handlePointerDown);
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [open]);

  return (
    <div className="action-menu" ref={containerRef}>
      <button
        type="button"
        className="action-menu-trigger"
        aria-label={ariaLabel}
        aria-haspopup="menu"
        aria-expanded={open}
        disabled={disabled}
        onClick={() => setOpen((current) => !current)}
      >
        <MoreHorizontal size={18} aria-hidden="true" />
      </button>

      {open ? (
        <div className="action-menu-list" role="menu">
          {items.map((item, index) => (
            <div key={item.label} role="none">
              {item.separatorBefore && index > 0 ? <hr /> : null}
              <button
                type="button"
                role="menuitem"
                className={item.danger ? "is-danger" : undefined}
                disabled={item.disabled}
                onClick={() => {
                  setOpen(false);
                  item.onSelect();
                }}
              >
                {item.label}
              </button>
            </div>
          ))}
        </div>
      ) : null}
    </div>
  );
}

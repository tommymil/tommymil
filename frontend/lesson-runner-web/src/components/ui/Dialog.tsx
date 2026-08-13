import { useCallback, useEffect, useId, useRef } from "react";
import type { PropsWithChildren, ReactNode } from "react";
import { X } from "lucide-react";

type DialogProps = PropsWithChildren<{
  title: string;
  description?: ReactNode;
  onClose: () => void;
  /** Stopka z przyciskami. Pierwszy element dostaje fokus po otwarciu, jeśli w treści nie ma pola. */
  actions?: ReactNode;
  /** Szersza odmiana dla list i tabel (np. obecność, historia zmian). */
  wide?: boolean;
  /** Nagłówek w kolorze ostrzegawczym — dla operacji nieodwracalnych. */
  tone?: "default" | "danger";
  /** Zamykanie kliknięciem w tło. Wyłączamy tam, gdzie w formularzu są niezapisane dane. */
  dismissOnScrim?: boolean;
}>;

/**
 * Jedyny sposób pokazywania okien modalnych w aplikacji.
 *
 * Powstał, bo wcześniej istniały trzy niezależne nakładki `.overlay` (lista obecności,
 * postępy, zakończenie zajęć) — zwykłe `div`-y bez `role="dialog"`, bez pułapki fokusu,
 * bez obsługi klawisza Escape i bez powrotu fokusu na element wywołujący. Obok nich
 * siedem miejsc używało `window.confirm` / `window.prompt` / `window.alert`, w tym
 * zgłaszanie nieobecności przez rodzica i nieodwracalna anonimizacja danych dziecka.
 *
 * Decyzje warte zapamiętania:
 * - **Fokus wraca na element wywołujący.** Bez tego użytkownik klawiatury po zamknięciu
 *   okna ląduje na początku dokumentu i przechodzi całą nawigację od nowa.
 * - **Escape zawsze zamyka**, ale kliknięcie w tło jest opcjonalne — przy formularzu
 *   z niezapisanymi danymi przypadkowe kliknięcie obok kasowałoby pracę.
 * - **Pułapka fokusu jest wyliczana przy każdym Tab**, a nie raz przy otwarciu:
 *   zawartość okien bywa dynamiczna (lista obecności rośnie o pola przy odznaczeniu dziecka).
 */
export function Dialog({
  title,
  description,
  onClose,
  actions,
  children,
  wide = false,
  tone = "default",
  dismissOnScrim = true,
}: DialogProps) {
  const cardRef = useRef<HTMLDivElement>(null);
  const restoreFocusTo = useRef<Element | null>(null);
  const titleId = useId();
  const descriptionId = useId();

  const focusable = useCallback(() => {
    const card = cardRef.current;

    if (!card) {
      return [];
    }

    return Array.from(
      card.querySelectorAll<HTMLElement>(
        'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])',
      ),
      // Widoczność sprawdzamy przez `hidden` i `aria-hidden`, a **nie** przez `offsetParent`.
      //
      // Pierwsza wersja filtrowała po `offsetParent !== null` i przez to nie działała wcale:
      // jsdom zawsze zwraca tu `null`, więc lista wychodziła pusta, fokus nigdy nie wchodził
      // do okna, a pułapka Tab nie miała czego pilnować. W przeglądarce byłoby podobnie dla
      // elementów w kontenerze `position: fixed` — a scrim właśnie taki jest.
      //
      // Warunek jest teraz strukturalny, więc daje ten sam wynik w przeglądarce i w testach.
    ).filter((element) => !element.hasAttribute("hidden") && element.getAttribute("aria-hidden") !== "true");
  }, []);

  useEffect(() => {
    restoreFocusTo.current = document.activeElement;

    // Fokus na pierwszym polu, a gdy okno jest samym potwierdzeniem - na pierwszym przycisku.
    const first = focusable()[0];
    first?.focus();

    return () => {
      if (restoreFocusTo.current instanceof HTMLElement) {
        restoreFocusTo.current.focus();
      }
    };
  }, [focusable]);

  useEffect(() => {
    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        event.stopPropagation();
        onClose();
        return;
      }

      if (event.key !== "Tab") {
        return;
      }

      const elements = focusable();

      if (elements.length === 0) {
        return;
      }

      const first = elements[0];
      const last = elements[elements.length - 1];
      const active = document.activeElement;

      if (event.shiftKey && (active === first || !cardRef.current?.contains(active))) {
        event.preventDefault();
        last.focus();
      } else if (!event.shiftKey && active === last) {
        event.preventDefault();
        first.focus();
      }
    }

    document.addEventListener("keydown", handleKeyDown, true);
    return () => document.removeEventListener("keydown", handleKeyDown, true);
  }, [focusable, onClose]);

  return (
    <div
      className="dialog-scrim"
      onMouseDown={(event) => {
        if (dismissOnScrim && event.target === event.currentTarget) {
          onClose();
        }
      }}
    >
      <div
        className={`dialog${wide ? " dialog-wide" : ""}${tone === "danger" ? " dialog-danger" : ""}`}
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
        aria-describedby={description ? descriptionId : undefined}
        ref={cardRef}
      >
        <div className="dialog-head">
          <div className="dialog-head-row">
            <h2 id={titleId}>{title}</h2>
            <button type="button" className="dialog-close" onClick={onClose} aria-label="Zamknij okno">
              <X size={18} aria-hidden="true" />
            </button>
          </div>
          {description ? <p id={descriptionId}>{description}</p> : null}
        </div>

        {children ? <div className="dialog-body">{children}</div> : null}
        {actions ? <div className="dialog-actions">{actions}</div> : null}
      </div>
    </div>
  );
}

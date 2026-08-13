import { useEffect, useRef, useState } from "react";
import { Search } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { search } from "../../api/searchApi";
import type { SearchResult } from "../../api/searchApi";

const kindLabels: Record<SearchResult["kind"], string> = {
  participant: "Dziecko",
  group: "Grupa",
  guardian: "Opiekun",
  lesson: "Konspekt",
};

/**
 * Wyszukiwanie globalne (Ctrl+K).
 *
 * Znalezienie dziecka wymagało wcześniej wejścia w Uczestników i przefiltrowania
 * listy. Przy pięćdziesięciu grupach i trzystu dzieciach — a do takiej skali system
 * się przygotowuje — to najczęstsza czynność w panelu, więc nie powinna kosztować
 * dwóch przeładowań ekranu.
 *
 * Zapytania są opóźnione o 220 ms i **przerywane** przy kolejnym naciśnięciu klawisza:
 * bez tego wolniejsza odpowiedź z wcześniejszej litery nadpisywała świeższe wyniki.
 */
export function GlobalSearch() {
  const [open, setOpen] = useState(false);
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<SearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [active, setActive] = useState(0);
  const navigate = useNavigate();
  const inputRef = useRef<HTMLInputElement>(null);
  const restoreFocusTo = useRef<Element | null>(null);

  useEffect(() => {
    function handleKeyDown(event: KeyboardEvent) {
      if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === "k") {
        event.preventDefault();
        restoreFocusTo.current = document.activeElement;
        setOpen(true);
      }
    }

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, []);

  useEffect(() => {
    if (open) {
      inputRef.current?.focus();
    }
  }, [open]);

  useEffect(() => {
    const trimmed = query.trim();

    if (!open || trimmed.length < 2) {
      setResults([]);
      setLoading(false);
      return;
    }

    let cancelled = false;
    setLoading(true);

    const timer = window.setTimeout(async () => {
      try {
        const response = await search(trimmed);

        if (!cancelled) {
          setResults(response.results);
          setActive(0);
        }
      } catch {
        if (!cancelled) {
          setResults([]);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }, 220);

    return () => {
      cancelled = true;
      window.clearTimeout(timer);
    };
  }, [open, query]);

  function close() {
    setOpen(false);
    setQuery("");
    setResults([]);

    if (restoreFocusTo.current instanceof HTMLElement) {
      restoreFocusTo.current.focus();
    }
  }

  function go(result: SearchResult) {
    close();
    navigate(result.path);
  }

  return (
    <>
      <button
        type="button"
        className="global-search-trigger"
        onClick={() => {
          restoreFocusTo.current = document.activeElement;
          setOpen(true);
        }}
      >
        <Search size={16} aria-hidden="true" />
        <span>Szukaj dziecka, grupy, konspektu…</span>
        <kbd>Ctrl K</kbd>
      </button>

      {open ? (
        <div
          className="global-search-scrim"
          onMouseDown={(event) => {
            if (event.target === event.currentTarget) {
              close();
            }
          }}
        >
          <div className="global-search-panel" role="dialog" aria-modal="true" aria-label="Wyszukiwanie">
            <input
              ref={inputRef}
              value={query}
              placeholder="Wpisz imię dziecka, nazwę grupy albo tytuł konspektu"
              aria-label="Szukana fraza"
              onChange={(event) => setQuery(event.target.value)}
              onKeyDown={(event) => {
                if (event.key === "Escape") {
                  close();
                  return;
                }

                if (event.key === "ArrowDown") {
                  event.preventDefault();
                  setActive((current) => Math.min(current + 1, results.length - 1));
                }

                if (event.key === "ArrowUp") {
                  event.preventDefault();
                  setActive((current) => Math.max(current - 1, 0));
                }

                if (event.key === "Enter" && results[active]) {
                  event.preventDefault();
                  go(results[active]);
                }
              }}
            />

            <div className="global-search-results">
              {results.map((result, index) => (
                <button
                  key={`${result.kind}-${result.id}`}
                  type="button"
                  className={index === active ? "is-active" : undefined}
                  onMouseEnter={() => setActive(index)}
                  onClick={() => go(result)}
                >
                  <span>
                    <strong>{result.title}</strong>
                    {result.subtitle ? <small> — {result.subtitle}</small> : null}
                  </span>
                  <span className="global-search-kind">{kindLabels[result.kind]}</span>
                </button>
              ))}

              {!loading && query.trim().length >= 2 && results.length === 0 ? (
                <p className="global-search-empty">Nic nie pasuje do „{query.trim()}”.</p>
              ) : null}

              {query.trim().length < 2 ? (
                <p className="global-search-empty">Wpisz co najmniej dwa znaki.</p>
              ) : null}
            </div>
          </div>
        </div>
      ) : null}
    </>
  );
}

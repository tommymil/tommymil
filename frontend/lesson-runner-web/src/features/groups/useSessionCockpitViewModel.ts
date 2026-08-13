import { useCallback, useEffect, useRef, useState } from "react";
import { ApiError } from "../../api/client";
import {
  finishSession,
  getAttendance,
  getScheduledSession,
  saveAttendance,
  setLiveStatus as setLiveStatusRequest,
  startSession,
} from "../../api/groupsApi";
import type {
  AttendanceEntry,
  AttendanceStatusOption,
  FinishSessionRequest,
  ScheduledSession,
  SessionAttendance,
} from "../../types/group";

/** Po tylu milisekundach bez kolejnej zmiany lecimy z zapisem. */
const AUTOSAVE_DELAY_MS = 900;

export function useSessionCockpitViewModel(sessionId: string | undefined) {
  const [session, setSession] = useState<ScheduledSession | null>(null);
  const [attendance, setAttendance] = useState<SessionAttendance | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);
  const [savedAt, setSavedAt] = useState<Date | null>(null);
  const [dirty, setDirty] = useState(false);
  const autosaveTimer = useRef<number | null>(null);

  useEffect(() => {
    if (!sessionId) {
      setError("Brak identyfikatora zajęć.");
      setLoading(false);
      return undefined;
    }

    let ignore = false;

    async function load(id: string) {
      try {
        setLoading(true);
        setError(null);
        const loaded = await getScheduledSession(id);

        if (ignore) {
          return;
        }

        setSession(loaded);

        if (loaded.status === "inprogress" || loaded.status === "completed") {
          const loadedAttendance = await getAttendance(id);
          if (!ignore) {
            setAttendance(loadedAttendance);
          }
        }
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać zajęć.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    void load(sessionId);

    return () => {
      ignore = true;
    };
  }, [sessionId]);

  async function start() {
    if (!sessionId) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      const loadedAttendance = await startSession(sessionId);
      setAttendance(loadedAttendance);
      setSession((current) => (current ? { ...current, status: "inprogress", statusLabel: "W toku" } : current));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się rozpocząć zajęć.");
    } finally {
      setBusy(false);
    }
  }

  /** Status jest źródłem prawdy; `present` przeliczamy ze słownika przysłanego przez backend. */
  function applyStatus(entry: AttendanceEntry, status: string, options: AttendanceStatusOption[]): AttendanceEntry {
    const option = options.find((item) => item.value === status);
    const present = option?.countsAsPresent ?? false;

    return {
      ...entry,
      status,
      statusLabel: option?.label ?? entry.statusLabel,
      present,
      // Odrabianie ma sens tylko dla dziecka, którego nie było.
      makeupRequired: present ? false : entry.makeupRequired,
      makeupSessionId: present ? null : entry.makeupSessionId ?? null,
    };
  }

  function setStatus(participantId: string, status: string) {
    setDirty(true);
    setAttendance((current) =>
      current
        ? {
            ...current,
            entries: current.entries.map((entry) =>
              entry.participantId === participantId ? applyStatus(entry, status, current.statusOptions) : entry,
            ),
          }
        : current,
    );
  }

  function setNote(participantId: string, note: string) {
    setDirty(true);
    setAttendance((current) =>
      current
        ? {
            ...current,
            entries: current.entries.map((entry) =>
              entry.participantId === participantId ? { ...entry, note } : entry,
            ),
          }
        : current,
    );
  }

  /**
   * Odhaczenie całej grupy naraz.
   *
   * Zostaje mimo przejścia na przełącznik segmentowy: na początku zajęć zwykle są
   * wszyscy, więc jedno kliknięcie zamiast dziesięciu ma realną wartość. Zniknął
   * natomiast `togglePresent` — pojedynczy stan ustawia się teraz wprost segmentem,
   * bez zgadywania, czy „odznaczenie” oznacza nieobecność, czy spóźnienie.
   */
  function setAllPresent(present: boolean) {
    setDirty(true);
    setAttendance((current) =>
      current
        ? {
            ...current,
            entries: current.entries.map((entry) =>
              applyStatus(entry, present ? "present" : "unexcusedabsence", current.statusOptions),
            ),
          }
        : current,
    );
  }

  function toggleMakeupRequired(participantId: string) {
    setDirty(true);
    setAttendance((current) =>
      current
        ? {
            ...current,
            entries: current.entries.map((entry) =>
              entry.participantId === participantId
                ? {
                    ...entry,
                    makeupRequired: !entry.makeupRequired,
                    makeupSessionId: entry.makeupRequired ? null : entry.makeupSessionId ?? null,
                    present: entry.makeupRequired ? entry.present : false,
                  }
                : entry,
            ),
          }
        : current,
    );
  }

  function setMakeupSession(participantId: string, makeupSessionId: string | null) {
    setDirty(true);
    setAttendance((current) =>
      current
        ? {
            ...current,
            entries: current.entries.map((entry) =>
              entry.participantId === participantId ? { ...entry, makeupSessionId } : entry,
            ),
          }
        : current,
    );
  }

  /**
   * Zapis obecności.
   *
   * Wcześniej zmiany żyły w stanie komponentu do momentu kliknięcia przycisku zapisu
   * w oknie modalnym, a zamknięcie okna nie ostrzegało. Przy dziesięciorgu dzieci
   * i zajęciach trwających półtorej godziny utrata listy była kwestią czasu — a obecność
   * to dokument, którym broni się reklamacje.
   *
   * Backend przyjmuje listę częściową i nie kasuje wierszy, których nie przysłano, więc
   * autozapis jest tu bezpieczny.
   */
  const persistAttendance = useCallback(async () => {
    if (!sessionId || !attendance) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      const saved = await saveAttendance(sessionId, {
        entries: attendance.entries.map((entry) => ({
          participantId: entry.participantId,
          present: entry.present,
          status: entry.status,
          note: entry.note?.trim() || null,
          makeupRequired: !entry.present && Boolean(entry.makeupRequired),
          makeupSessionId: !entry.present && entry.makeupRequired ? entry.makeupSessionId ?? null : null,
        })),
      });
      setAttendance(saved);
      setDirty(false);
      setSavedAt(new Date());
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać obecności.");
    } finally {
      setBusy(false);
    }
  }, [attendance, sessionId]);

  // Zapis po chwili bezczynności, a nie po każdym klawiszu: instruktor wpisujący notatkę
  // nie ma generować żądania na literę.
  useEffect(() => {
    if (!dirty || !sessionId || !attendance) {
      return undefined;
    }

    autosaveTimer.current = window.setTimeout(() => void persistAttendance(), AUTOSAVE_DELAY_MS);

    return () => {
      if (autosaveTimer.current !== null) {
        window.clearTimeout(autosaveTimer.current);
      }
    };
  }, [dirty, attendance, sessionId, persistAttendance]);

  /**
   * Znacznik pracy na żywo.
   *
   * Idzie osobnym, wąskim żądaniem i **z pominięciem autozapisu**: to jest sygnał
   * „potrzebuję Cię teraz”, więc opóźnienie o sekundę byłoby tu bez sensu.
   */
  async function setLive(participantId: string, liveStatus: string) {
    if (!sessionId) {
      return;
    }

    // Optymistycznie, żeby lista przestawiła się natychmiast po kliknięciu.
    setAttendance((current) =>
      current
        ? {
            ...current,
            entries: current.entries.map((entry) =>
              entry.participantId === participantId
                ? { ...entry, liveStatus: liveStatus as AttendanceEntry["liveStatus"] }
                : entry,
            ),
          }
        : current,
    );

    try {
      const saved = await setLiveStatusRequest(sessionId, { participantId, liveStatus });

      // Scalamy, a nie podmieniamy. Ten endpoint świadomie nie liczy listy terminów
      // odrabiania (czytanie wszystkich grup przy każdym kliknięciu byłoby marnotrawstwem),
      // więc podmiana całego obiektu wyczyściłaby select „Termin odrabiania”.
      setAttendance((current) =>
        current
          ? {
              ...saved,
              makeupOptions: saved.makeupOptions?.length ? saved.makeupOptions : current.makeupOptions,
              statusOptions: saved.statusOptions?.length ? saved.statusOptions : current.statusOptions,
            }
          : saved,
      );
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać znacznika.");
    }
  }

  async function finish(request: FinishSessionRequest) {
    if (!sessionId) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      // Obecność musi być zapisana przed zamknięciem terminu: to `FinishAsync` wysyła
      // powiadomienia o nieobecnościach, więc czyta listę taką, jaka jest w bazie.
      if (dirty) {
        await persistAttendance();
      }

      const updated = await finishSession(sessionId, request);
      setSession(updated);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zakończyć zajęć.");
    } finally {
      setBusy(false);
    }
  }

  return {
    session,
    attendance,
    loading,
    error,
    busy,
    savedAt,
    dirty,
    setLive,
    start,
    setStatus,
    setNote,
    toggleMakeupRequired,
    setMakeupSession,
    setAllPresent,
    persistAttendance,
    finish,
    presentCount: attendance?.entries.filter((entry) => entry.present).length ?? 0,
    allPresent: (attendance?.entries.length ?? 0) > 0 && (attendance?.entries.every((entry) => entry.present) ?? false),
  };
}

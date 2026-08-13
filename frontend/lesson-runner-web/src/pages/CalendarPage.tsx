import { CalendarDays, Plus, Trash2 } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { ApiError } from "../api/client";
import { createHoliday, deleteHoliday, getCalendar } from "../api/schedulingApi";
import { useAuth } from "../features/auth/AuthContext";
import { formatDateTime } from "../features/groups/datetime";
import type { CalendarData } from "../types/scheduling";

export function CalendarPage() {
  const { isAdmin } = useAuth();
  const [from, setFrom] = useState(toDateInput(startOfWeek(new Date())));
  const [to, setTo] = useState(toDateInput(addDays(startOfWeek(new Date()), 34)));
  const [data, setData] = useState<CalendarData | null>(null);
  const [holidayDate, setHolidayDate] = useState("");
  const [holidayName, setHolidayName] = useState("");
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    try {
      setLoading(true);
      setError(null);
      setData(await getCalendar(dateStartIso(from), dateEndIso(to)));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać kalendarza.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, [from, to]);

  const sessionsByDay = useMemo(() => {
    const map = new Map<string, CalendarData["sessions"]>();

    for (const session of data?.sessions ?? []) {
      const key = toDateInput(new Date(session.scheduledAt));
      map.set(key, [...(map.get(key) ?? []), session]);
    }

    return [...map.entries()].sort(([first], [second]) => first.localeCompare(second));
  }, [data]);

  async function addHoliday() {
    if (!holidayDate || !holidayName.trim()) {
      setError("Data i nazwa dnia wolnego są wymagane.");
      return;
    }

    try {
      setBusy(true);
      setError(null);
      await createHoliday({ date: holidayDate, name: holidayName.trim() });
      setHolidayDate("");
      setHolidayName("");
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się dodać dnia wolnego.");
    } finally {
      setBusy(false);
    }
  }

  async function removeHoliday(id: string) {
    try {
      setBusy(true);
      setError(null);
      await deleteHoliday(id);
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się usunąć dnia wolnego.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <section className="page-section calendar-page">
      <div className="page-header">
        <div>
          <span className="eyebrow">Planowanie</span>
          <h1>Kalendarz</h1>
          <p>{isAdmin ? "Widok wszystkich grup i dni wolnych." : "Twój harmonogram zajęć."}</p>
        </div>
        <div className="page-header-actions">
          {isAdmin ? (
            <Link to="/admin/groups/new">
              <Button>
                <Plus className="button-icon" aria-hidden="true" />
                Nowa grupa
              </Button>
            </Link>
          ) : null}
        </div>
      </div>

      <div className="calendar-toolbar">
        <label className="form-field">
          <span>Od</span>
          <input type="date" value={from} onChange={(event) => setFrom(event.target.value)} />
        </label>
        <label className="form-field">
          <span>Do</span>
          <input type="date" value={to} onChange={(event) => setTo(event.target.value)} />
        </label>
      </div>

      {error ? <div className="list-state list-state-error">{error}</div> : null}
      {loading ? <div className="list-state">Ładowanie kalendarza…</div> : null}

      {!loading && data ? (
        <div className="calendar-layout">
          <div className="calendar-main">
            {sessionsByDay.length === 0 ? <p className="cue-empty">Brak terminów w wybranym zakresie.</p> : null}
            {sessionsByDay.map(([day, sessions]) => (
              <section className="calendar-day" key={day}>
                <h2>{formatDay(day)}</h2>
                <div className="calendar-session-list">
                  {sessions.map((session) => (
                    <Link
                      to={isAdmin ? `/admin/groups/${session.groupId}` : `/instructor/sessions/${session.id}`}
                      className="calendar-session"
                      key={session.id}
                    >
                      <span className={`status-dot status-${session.status}`} aria-hidden="true" />
                      <div>
                        <strong>{formatDateTime(session.scheduledAt)}</strong>
                        <span>{session.groupName}</span>
                        <small>
                          {session.instructorName} · {session.lessonTitle ?? "Bez lekcji"}
                        </small>
                      </div>
                    </Link>
                  ))}
                </div>
              </section>
            ))}
          </div>

          <aside className="calendar-side">
            <section className="editor-fieldset">
              <legend>Dni wolne</legend>
              {data.holidays.length === 0 ? <p className="cue-empty">Brak dni wolnych.</p> : null}
              <div className="compact-list">
                {data.holidays.map((holiday) => (
                  <div className="compact-row" key={holiday.id}>
                    <CalendarDays size={16} aria-hidden="true" />
                    <span>
                      <strong>{formatHolidayDate(holiday.date)}</strong>
                      <small>{holiday.name}</small>
                    </span>
                    {isAdmin ? (
                      <button type="button" disabled={busy} onClick={() => void removeHoliday(holiday.id)} aria-label="Usuń dzień wolny">
                        <Trash2 size={14} aria-hidden="true" />
                      </button>
                    ) : null}
                  </div>
                ))}
              </div>

              {isAdmin ? (
                <form
                  className="holiday-form"
                  onSubmit={(event) => {
                    event.preventDefault();
                    void addHoliday();
                  }}
                >
                  <h3>Dodaj dzień wolny</h3>
                  <p>W tych dniach generator terminów pomija zajęcia i przesuwa je na kolejny tydzień.</p>

                  <div className="holiday-form-fields">
                    <label className="form-field">
                      <span>Data</span>
                      <input
                        type="date"
                        value={holidayDate}
                        onChange={(event) => setHolidayDate(event.target.value)}
                        required
                      />
                    </label>
                    <label className="form-field">
                      <span>Nazwa</span>
                      <input
                        value={holidayName}
                        onChange={(event) => setHolidayName(event.target.value)}
                        placeholder="np. Boże Narodzenie, ferie"
                        maxLength={160}
                        required
                      />
                    </label>
                  </div>

                  <Button type="submit" variant="secondary" disabled={busy || !holidayDate || !holidayName.trim()}>
                    <Plus className="button-icon" aria-hidden="true" />
                    Dodaj dzień wolny
                  </Button>
                </form>
              ) : null}
            </section>
          </aside>
        </div>
      ) : null}
    </section>
  );
}

function startOfWeek(date: Date): Date {
  const result = new Date(date);
  const day = result.getDay() || 7;
  result.setDate(result.getDate() - day + 1);
  return result;
}

function addDays(date: Date, days: number): Date {
  const result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
}

function toDateInput(date: Date): string {
  const year = date.getFullYear();
  const month = `${date.getMonth() + 1}`.padStart(2, "0");
  const day = `${date.getDate()}`.padStart(2, "0");
  return `${year}-${month}-${day}`;
}

function dateStartIso(date: string): string {
  return new Date(`${date}T00:00:00`).toISOString();
}

function dateEndIso(date: string): string {
  return new Date(`${date}T23:59:59`).toISOString();
}

/** Data dnia wolnego po polsku, np. "25 grudnia 2026 (piątek)" zamiast surowego "2026-12-25". */
function formatHolidayDate(date: string): string {
  const parsed = new Date(`${date}T12:00:00`);

  if (Number.isNaN(parsed.getTime())) {
    return date;
  }

  const dzien = new Intl.DateTimeFormat("pl-PL", { day: "numeric", month: "long", year: "numeric" }).format(parsed);
  const tydzien = new Intl.DateTimeFormat("pl-PL", { weekday: "long" }).format(parsed);
  return `${dzien} (${tydzien})`;
}

function formatDay(date: string): string {
  return new Intl.DateTimeFormat("pl-PL", {
    weekday: "long",
    day: "2-digit",
    month: "long",
    year: "numeric",
  }).format(new Date(`${date}T12:00:00`));
}

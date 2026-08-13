import { useState } from "react";
import { Check, CircleAlert, HandHelping, Loader, Save, StickyNote, Wrench } from "lucide-react";
import { Button } from "../../components/ui/Button";
import { SegmentedControl } from "../../components/ui/SegmentedControl";
import { StatusBadge, attendanceStatusTone } from "../../components/ui/StatusBadge";
import { formatDateTime, formatTime } from "../groups/datetime";
import type { AttendanceEntry, SessionAttendance } from "../../types/group";

type RailTab = "attendance" | "work";

type SessionRailProps = {
  attendance: SessionAttendance | null;
  busy: boolean;
  dirty: boolean;
  savedAt: Date | null;
  onStatus: (participantId: string, status: string) => void;
  onNote: (participantId: string, note: string) => void;
  onLive: (participantId: string, liveStatus: string) => void;
  onMakeupToggle: (participantId: string) => void;
  onMakeupSession: (participantId: string, sessionId: string | null) => void;
  onSaveNow: () => void;
  onAllPresent: (present: boolean) => void;
  allPresent: boolean;
  /** Otwiera zgłoszenie problemu technicznego dla wskazanego dziecka. */
  onReportProblem?: (entry: AttendanceEntry) => void;
};

/**
 * Szyna kokpitu prowadzenia.
 *
 * Zastępuje trzy niezależne nakładki (`.overlay`) na listę obecności, panel postępów
 * i zakończenie zajęć. Każda z nich zasłaniała scenariusz, więc żeby odhaczyć spóźnione
 * dziecko, instruktor musiał zgubić z oczu krok lekcji, na którym właśnie był.
 *
 * Szyna jest stała i zwijana. Scenariusz nigdy nie znika.
 */
export function SessionRail({
  attendance,
  busy,
  dirty,
  savedAt,
  onStatus,
  onNote,
  onLive,
  onMakeupToggle,
  onMakeupSession,
  onSaveNow,
  onAllPresent,
  allPresent,
  onReportProblem,
}: SessionRailProps) {
  const [tab, setTab] = useState<RailTab>("work");
  const [expanded, setExpanded] = useState<string | null>(null);

  if (!attendance) {
    return null;
  }

  const waiting = attendance.entries.filter(
    (entry) => entry.liveStatus === "needshelp" || entry.liveStatus === "blocked",
  ).length;

  return (
    <aside className="session-rail" aria-label="Panel prowadzenia">
      <div className="session-rail-tabs" role="tablist" aria-label="Widok panelu">
        <button
          type="button"
          role="tab"
          aria-selected={tab === "work"}
          onClick={() => setTab("work")}
        >
          Praca
          {waiting > 0 ? <span className="rail-badge">{waiting}</span> : null}
        </button>
        <button
          type="button"
          role="tab"
          aria-selected={tab === "attendance"}
          onClick={() => setTab("attendance")}
        >
          Obecność
        </button>
      </div>

      {/* Autozapis pokazuje stan wprost. Bez tego użytkownik nie wie, czy jego zmiany
          gdziekolwiek trafiły — a przy zniknięciu przycisku „Zapisz” to jedyny sygnał. */}
      <div className="session-rail-save" aria-live="polite">
        {busy ? (
          <>
            <Loader size={14} aria-hidden="true" /> Zapisywanie…
          </>
        ) : dirty ? (
          <>
            <span className="rail-dot" aria-hidden="true" /> Niezapisane zmiany
          </>
        ) : savedAt ? (
          <>
            <Check size={14} aria-hidden="true" /> Zapisano {formatTime(savedAt.toISOString())}
          </>
        ) : (
          <span className="cue-empty">Zmiany zapisują się same</span>
        )}
        {dirty ? (
          <Button variant="ghost" onClick={onSaveNow} disabled={busy}>
            <Save className="button-icon" aria-hidden="true" />
            Zapisz teraz
          </Button>
        ) : null}
      </div>

      {/* Na początku zajęć zwykle są wszyscy — jedno kliknięcie zamiast dziesięciu. */}
      {tab === "attendance" && attendance.entries.length > 0 ? (
        <Button variant="ghost" onClick={() => onAllPresent(!allPresent)}>
          {allPresent ? "Odznacz wszystkich" : "Zaznacz wszystkich obecnych"}
        </Button>
      ) : null}

      <ul className="session-rail-list">
        {attendance.entries.map((entry) => (
          <li key={entry.participantId} className={`rail-row rail-live-${entry.liveStatus ?? "working"}`}>
            <div className="rail-row-head">
              <span className="rail-name">
                {entry.firstName} {entry.lastName}
              </span>
              {tab === "work" ? (
                <StatusBadge label={entry.statusLabel} tone={attendanceStatusTone(entry.status)} />
              ) : null}
            </div>

            {tab === "work" ? (
              <LiveControls entry={entry} onLive={onLive} onReportProblem={onReportProblem} />
            ) : (
              <AttendanceControls
                entry={entry}
                attendance={attendance}
                expanded={expanded === entry.participantId}
                onToggleExpand={() =>
                  setExpanded((current) => (current === entry.participantId ? null : entry.participantId))
                }
                onStatus={onStatus}
                onNote={onNote}
                onMakeupToggle={onMakeupToggle}
                onMakeupSession={onMakeupSession}
              />
            )}
          </li>
        ))}

        {attendance.entries.length === 0 ? <li className="cue-empty">Grupa nie ma uczestników.</li> : null}
      </ul>
    </aside>
  );
}

/**
 * Znaczniki pracy na żywo — rozdział 4 dokumentu koncepcyjnego.
 *
 * To jest ta funkcja, która odróżnia platformę do prowadzenia zajęć od kalendarza z listą
 * obecności: instruktor widzi na jednym ekranie, kto czeka na pomoc, kto skończył i komu
 * trzeba przygotować zadanie dodatkowe. Lista sortuje się po pilności po stronie backendu.
 */
function LiveControls({
  entry,
  onLive,
  onReportProblem,
}: {
  entry: AttendanceEntry;
  onLive: (participantId: string, liveStatus: string) => void;
  onReportProblem?: (entry: AttendanceEntry) => void;
}) {
  return (
    <div className="rail-live">
      <SegmentedControl
        ariaLabel={`Stan pracy: ${entry.firstName} ${entry.lastName}`}
        value={entry.liveStatus ?? "working"}
        onChange={(value) => onLive(entry.participantId, value)}
        segments={[
          { value: "working", label: "Pracuje" },
          { value: "needshelp", label: "Pomoc", tone: "seg-late", title: "Potrzebuje pomocy" },
          { value: "finished", label: "Gotowe", tone: "seg-present", title: "Skończył zadanie" },
          { value: "blocked", label: "Problem", tone: "seg-absent", title: "Problem techniczny" },
        ]}
      />

      {entry.liveStatus === "needshelp" ? (
        <span className="rail-hint rail-hint-help">
          <HandHelping size={14} aria-hidden="true" /> czeka na Ciebie
        </span>
      ) : null}
      {/* Znacznik „problem techniczny” żyje tylko na czas tych zajęć — po nich nie zostaje
          po nim ślad. Dlatego dokładnie tutaj proponujemy zapisanie zgłoszenia: to jedyny
          moment, w którym instruktor wie, co się właściwie zepsuło, i jedyny sposób,
          żeby za tydzień nie zaczynać od pytania „co ci znowu nie działa”. */}
      {entry.liveStatus === "blocked" ? (
        <div className="rail-blocked-action">
          <span className="rail-hint rail-hint-blocked">
            <CircleAlert size={14} aria-hidden="true" /> nie może pracować
          </span>
          {onReportProblem ? (
            <Button variant="ghost" onClick={() => onReportProblem(entry)}>
              <Wrench className="button-icon" aria-hidden="true" />
              Zapisz problem
            </Button>
          ) : null}
        </div>
      ) : null}
      {entry.liveStatus === "finished" ? (
        <span className="rail-hint rail-hint-done">
          <Check size={14} aria-hidden="true" /> przygotuj zadanie dodatkowe
        </span>
      ) : null}
    </div>
  );
}

/**
 * Obecność: trzy najczęstsze stany widoczne od razu, rzadsze pod „Więcej”.
 *
 * Wcześniej każde dziecko miało pięć kontrolek naraz — checkbox obecności, listę rozwijaną
 * z dziewięcioma statusami, pole notatki, checkbox „do odrobienia” i wybór terminu odrabiania.
 * Przy dziesięciorgu dzieci dawało to pięćdziesiąt kontrolek w oknie modalnym otwieranym
 * w trakcie prowadzenia zajęć. Bogaty model statusów jest słuszny, ale nie każdy status
 * jest równie częsty.
 */
function AttendanceControls({
  entry,
  attendance,
  expanded,
  onToggleExpand,
  onStatus,
  onNote,
  onMakeupToggle,
  onMakeupSession,
}: {
  entry: AttendanceEntry;
  attendance: SessionAttendance;
  expanded: boolean;
  onToggleExpand: () => void;
  onStatus: (participantId: string, status: string) => void;
  onNote: (participantId: string, note: string) => void;
  onMakeupToggle: (participantId: string) => void;
  onMakeupSession: (participantId: string, sessionId: string | null) => void;
}) {
  const quick = ["present", "late", "unexcusedabsence"];
  const isQuick = quick.includes(entry.status);

  return (
    <div className="rail-attendance">
      <div className="rail-attendance-row">
        <SegmentedControl
          ariaLabel={`Obecność: ${entry.firstName} ${entry.lastName}`}
          value={isQuick ? entry.status : null}
          onChange={(value) => onStatus(entry.participantId, value)}
          segments={[
            { value: "present", label: "Obecny", tone: "seg-present" },
            { value: "late", label: "Spóźniony", tone: "seg-late" },
            { value: "unexcusedabsence", label: "Nieobecny", tone: "seg-absent" },
          ]}
        />
        <Button variant="ghost" onClick={onToggleExpand} aria-expanded={expanded}>
          <StickyNote className="button-icon" aria-hidden="true" />
          {expanded ? "Mniej" : "Więcej"}
        </Button>
      </div>

      {!isQuick ? <StatusBadge label={entry.statusLabel} tone={attendanceStatusTone(entry.status)} /> : null}

      {expanded ? (
        <div className="rail-attendance-more">
          <label className="form-field">
            <span>Pełny status</span>
            <select value={entry.status} onChange={(event) => onStatus(entry.participantId, event.target.value)}>
              {attendance.statusOptions.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </label>

          <label className="form-field">
            <span>Notatka o tych zajęciach</span>
            <input
              value={entry.note ?? ""}
              maxLength={500}
              placeholder="np. dołączył 15 minut później"
              onChange={(event) => onNote(entry.participantId, event.target.value)}
            />
          </label>

          {!entry.present ? (
            <>
              <label className="checkbox-field">
                <input
                  type="checkbox"
                  checked={Boolean(entry.makeupRequired)}
                  onChange={() => onMakeupToggle(entry.participantId)}
                />
                <span>Do odrobienia</span>
              </label>

              {entry.makeupRequired ? (
                <label className="form-field">
                  <span>Termin odrabiania</span>
                  <select
                    value={entry.makeupSessionId ?? ""}
                    onChange={(event) => onMakeupSession(entry.participantId, event.target.value || null)}
                  >
                    <option value="">Termin do ustalenia</option>
                    {attendance.makeupOptions.map((option) => (
                      <option key={option.sessionId} value={option.sessionId}>
                        {formatDateTime(option.scheduledAt)} — {option.groupName}
                        {option.lessonTitle ? `, ${option.lessonTitle}` : ""}
                      </option>
                    ))}
                  </select>
                </label>
              ) : null}
            </>
          ) : null}
        </div>
      ) : null}
    </div>
  );
}

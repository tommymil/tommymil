import { useState } from "react";
import { CalendarPlus } from "lucide-react";
import { ActionMenu } from "../../components/ui/ActionMenu";
import { Button } from "../../components/ui/Button";
import { Dialog } from "../../components/ui/Dialog";
import { EmptyState } from "../../components/ui/EmptyState";
import { StatusBadge, sessionStatusTone } from "../../components/ui/StatusBadge";
import { CancelSessionDialog } from "./CancelSessionDialog";
import { formatFriendlyDateTime, plural } from "./datetime";
import { isUpcomingSession } from "../../types/group";
import type { CancelSessionRequest, GroupDetails, ScheduledSession, SessionStatusOption } from "../../types/group";

type SessionsPanelProps = {
  group: GroupDetails;
  busy: boolean;
  instructors: { id: string; displayName: string }[];
  statusOptions: SessionStatusOption[];
  onCancel: (
    id: string,
    reason?: string | null,
    guardiansNotified?: boolean,
    compensation?: string | null,
    shiftFollowingLessons?: boolean,
  ) => void;
  onReschedule: (
    id: string,
    scheduledAt: string,
    substituteInstructorId?: string | null,
    reason?: string | null,
    guardiansNotified?: boolean,
  ) => void;
  onSetSubstitute: (id: string, substituteInstructorId?: string | null) => void;
  onSetLinks: (id: string, meetingUrl: string | null, recordingUrl: string | null) => void;
  onChangeStatus: (id: string, status: string, reason?: string | null, guardiansNotified?: boolean) => void;
  onExport: (id: string) => void;
};

type OpenDialog =
  | { kind: "cancel"; session: ScheduledSession }
  | { kind: "reschedule"; session: ScheduledSession }
  | { kind: "links"; session: ScheduledSession }
  | { kind: "status"; session: ScheduledSession }
  | { kind: "bulk-cancel" }
  | null;

/**
 * Lista terminów grupy.
 *
 * Poprzednia wersja rozwijała trzy różne formularze wprost w wierszu listy: pole nowego
 * terminu, wybór zastępcy, powód, dwa checkboxy, wybór rekompensaty i pola linków —
 * `SessionRow` miał dziesięć propsów i osiem stanów lokalnych. Był to najgęstszy widok
 * w aplikacji i jednocześnie ten, w którym wykonuje się najbardziej ryzykowne operacje.
 *
 * Tutaj wiersz zostaje wierszem, a każda operacja ma własne okno z podsumowaniem skutków.
 */
export function SessionsPanel({
  group,
  busy,
  instructors,
  statusOptions,
  onCancel,
  onReschedule,
  onSetSubstitute,
  onSetLinks,
  onChangeStatus,
  onExport,
}: SessionsPanelProps) {
  const [dialog, setDialog] = useState<OpenDialog>(null);
  const [selected, setSelected] = useState<string[]>([]);

  const enrolledCount = group.participants.filter(
    (participant) => participant.enrollmentStatus !== "waitlisted",
  ).length;

  function toggleSelected(id: string) {
    setSelected((current) => (current.includes(id) ? current.filter((item) => item !== id) : [...current, id]));
  }

  const selectableIds = group.sessions
    .filter((session) => isUpcomingSession(session.status))
    .map((session) => session.id);

  return (
    <div className="group-tab-panel">
      <div className="group-section-head">
        <div>
          <span className="eyebrow">Plan</span>
          <h2>Terminy</h2>
        </div>
        <span>
          {group.sessions.length} {plural(group.sessions.length, "termin", "terminy", "terminów")}
        </span>
      </div>

      {group.sessions.length === 0 ? (
        <EmptyState
          icon={CalendarPlus}
          title="Brak zaplanowanych terminów"
          description="Dodaj pierwszy termin w zakładce Ustawienia — grupa bez terminów nie pojawi się w grafiku instruktora."
          compact
        />
      ) : null}

      {/* Pasek zbiorczy pojawia się dopiero po zaznaczeniu. Odwołanie tygodnia ferii
          było wcześniej osobną operacją na każdym terminie w każdej grupie. */}
      {selected.length > 0 ? (
        <div className="bulk-bar">
          <span>
            Zaznaczono {selected.length} {plural(selected.length, "termin", "terminy", "terminów")}
          </span>
          <div className="bulk-bar-actions">
            <Button variant="ghost" onClick={() => setSelected([])}>
              Wyczyść
            </Button>
            <Button variant="ghost" onClick={() => setSelected(selectableIds)}>
              Zaznacz wszystkie nadchodzące
            </Button>
            <Button variant="danger" onClick={() => setDialog({ kind: "bulk-cancel" })}>
              Odwołaj zaznaczone
            </Button>
          </div>
        </div>
      ) : null}

      <ul className="session-list">
        {group.sessions.map((session) => {
          const planned = isUpcomingSession(session.status);

          return (
            <li
              key={session.id}
              className={`session-row-compact${selected.includes(session.id) ? " is-selected" : ""}`}
            >
              {planned ? (
                <input
                  type="checkbox"
                  checked={selected.includes(session.id)}
                  onChange={() => toggleSelected(session.id)}
                  aria-label={`Zaznacz termin ${session.sequenceNumber}`}
                />
              ) : (
                <span className="session-seq" aria-hidden="true">
                  {session.sequenceNumber}
                </span>
              )}

              <div className="session-main">
                <strong>{formatFriendlyDateTime(session.scheduledAt)}</strong>
                <small>
                  Lekcja {session.sequenceNumber} · {session.lessonTitle ?? "brak przypisanej lekcji"}
                  {session.substituteInstructorName ? ` · zastępstwo: ${session.substituteInstructorName}` : ""}
                  {session.sessionMeetingUrl ? " · własny link" : ""}
                  {session.recordingUrl ? " · nagranie" : ""}
                </small>
              </div>

              <StatusBadge label={session.statusLabel} tone={sessionStatusTone(session.status)} dot />

              <ActionMenu
                ariaLabel={`Akcje terminu ${session.sequenceNumber}`}
                disabled={busy}
                items={[
                  { label: "Przełóż termin…", onSelect: () => setDialog({ kind: "reschedule", session }) },
                  { label: "Linki i nagranie…", onSelect: () => setDialog({ kind: "links", session }) },
                  { label: "Zmień status…", onSelect: () => setDialog({ kind: "status", session }) },
                  { label: "Eksportuj obecność (CSV)", onSelect: () => onExport(session.id) },
                  {
                    label: "Odwołaj zajęcia…",
                    onSelect: () => setDialog({ kind: "cancel", session }),
                    danger: true,
                    separatorBefore: true,
                    disabled: !planned,
                  },
                ]}
              />
            </li>
          );
        })}
      </ul>

      {dialog?.kind === "cancel" ? (
        <CancelSessionDialog
          session={dialog.session}
          enrolledCount={enrolledCount}
          followingCount={group.sessions.filter(
            (item) => item.sequenceNumber > dialog.session.sequenceNumber,
          ).length}
          busy={busy}
          onClose={() => setDialog(null)}
          onConfirm={async (request: CancelSessionRequest) => {
            onCancel(
              dialog.session.id,
              request.reason,
              request.guardiansNotified,
              request.compensation,
              request.shiftFollowingLessons,
            );
            setDialog(null);
          }}
        />
      ) : null}

      {dialog?.kind === "bulk-cancel" ? (
        <BulkCancelDialog
          sessions={group.sessions.filter((session) => selected.includes(session.id))}
          enrolledCount={enrolledCount}
          busy={busy}
          onClose={() => setDialog(null)}
          onConfirm={(reason, compensation, notify) => {
            // Odwołujemy po jednym terminie istniejącym endpointem. Przy tygodniu ferii
            // to kilka żądań, a nie tysiąc — nie warto za to płacić nową operacją zbiorczą
            // w backendzie, która i tak musiałaby robić dokładnie to samo w pętli.
            for (const id of selected) {
              onCancel(id, reason, notify, compensation, false);
            }

            setSelected([]);
            setDialog(null);
          }}
        />
      ) : null}

      {dialog?.kind === "reschedule" ? (
        <RescheduleDialog
          session={dialog.session}
          instructors={instructors}
          busy={busy}
          onClose={() => setDialog(null)}
          onConfirm={(when, substituteId, reason, notify) => {
            onReschedule(dialog.session.id, when, substituteId, reason, notify);
            setDialog(null);
          }}
          onSetSubstitute={(substituteId) => onSetSubstitute(dialog.session.id, substituteId)}
        />
      ) : null}

      {dialog?.kind === "links" ? (
        <LinksDialog
          session={dialog.session}
          busy={busy}
          onClose={() => setDialog(null)}
          onConfirm={(meetingUrl, recordingUrl) => {
            onSetLinks(dialog.session.id, meetingUrl, recordingUrl);
            setDialog(null);
          }}
        />
      ) : null}

      {dialog?.kind === "status" ? (
        <StatusDialog
          session={dialog.session}
          statusOptions={statusOptions}
          busy={busy}
          onClose={() => setDialog(null)}
          onConfirm={(status, reason, notify) => {
            onChangeStatus(dialog.session.id, status, reason, notify);
            setDialog(null);
          }}
        />
      ) : null}
    </div>
  );
}

/* -------------------------------------------------------------------------- */

function BulkCancelDialog({
  sessions,
  enrolledCount,
  busy,
  onClose,
  onConfirm,
}: {
  sessions: ScheduledSession[];
  enrolledCount: number;
  busy: boolean;
  onClose: () => void;
  onConfirm: (reason: string, compensation: string, notify: boolean) => void;
}) {
  const [reason, setReason] = useState("");
  const [compensation, setCompensation] = useState("credit");
  const [notify, setNotify] = useState(true);

  return (
    <Dialog
      title={`Odwołać ${sessions.length} ${plural(sessions.length, "termin", "terminy", "terminów")}?`}
      description="Typowy przypadek: ferie, święta albo dłuższa nieobecność instruktora."
      tone="danger"
      wide
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button variant="danger" disabled={busy || !reason.trim()} onClick={() => onConfirm(reason.trim(), compensation, notify)}>
            Odwołaj wszystkie
          </Button>
        </>
      }
    >
      <div className="dialog-consequences">
        <ul>
          {sessions.map((session) => (
            <li key={session.id}>
              <span>
                Lekcja {session.sequenceNumber} — {formatFriendlyDateTime(session.scheduledAt)}
              </span>
            </li>
          ))}
          <li>
            <span>
              {compensation === "credit"
                ? `Każde z ${enrolledCount} zapisanych dzieci dostanie ${sessions.length} ${plural(sessions.length, "kredyt", "kredyty", "kredytów")}.`
                : "Rekompensaty nie przyznajemy automatycznie."}
            </span>
          </li>
          <li>
            <span>Materiału nie przesuwamy — przy odwołaniu zbiorczym numeracja rozjechałaby się nieprzewidywalnie.</span>
          </li>
          <li>
            {/* Wysyłka jest automatyczna przy każdym odwołaniu. Przy odwołaniu zbiorczym
                oznacza to jeden e-mail na termin — dlatego mówimy to wprost, zanim
                administrator zaznaczy dwanaście terminów ferii. */}
            <span>
              Opiekunowie dostaną <b>{sessions.length}</b>{" "}
              {plural(sessions.length, "wiadomość", "wiadomości", "wiadomości")} — po jednej
              na każdy odwołany termin.
            </span>
          </li>
        </ul>
      </div>

      <label className="form-field">
        <span>Powód (wymagany, trafia do historii każdego terminu)</span>
        <input
          value={reason}
          maxLength={500}
          placeholder="np. ferie zimowe"
          onChange={(event) => setReason(event.target.value)}
        />
      </label>

      <label className="form-field">
        <span>Rozliczenie</span>
        <select value={compensation} onChange={(event) => setCompensation(event.target.value)}>
          <option value="credit">Kredyt zajęciowy za każdy termin</option>
          <option value="none">Bez rekompensaty</option>
        </select>
      </label>

      <label className="checkbox-field">
        <input type="checkbox" checked={notify} onChange={() => setNotify((current) => !current)} />
        <span>Dodatkowo poinformowano opiekunów telefonicznie lub SMS-em</span>
      </label>
    </Dialog>
  );
}

function RescheduleDialog({
  session,
  instructors,
  busy,
  onClose,
  onConfirm,
  onSetSubstitute,
}: {
  session: ScheduledSession;
  instructors: { id: string; displayName: string }[];
  busy: boolean;
  onClose: () => void;
  onConfirm: (when: string, substituteId: string | null, reason: string | null, notify: boolean) => void;
  onSetSubstitute: (substituteId: string | null) => void;
}) {
  const [when, setWhen] = useState("");
  const [substituteId, setSubstituteId] = useState(session.substituteInstructorId ?? "");
  const [reason, setReason] = useState("");
  const [notify, setNotify] = useState(false);

  return (
    <Dialog
      title="Przełóż termin"
      description={`Lekcja ${session.sequenceNumber} — obecnie ${formatFriendlyDateTime(session.scheduledAt)}`}
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button
            disabled={busy || !when}
            onClick={() => onConfirm(when, substituteId || null, reason.trim() || null, notify)}
          >
            Przełóż
          </Button>
        </>
      }
    >
      <div className="dialog-consequences">
        <ul>
          <li>
            <span>
              Ten sam termin zmieni datę — status zostaje „zaplanowany”, bo zajęcia nadal się odbędą.
            </span>
          </li>
          <li>
            {/* W odróżnieniu od odwołania, informacja o przełożeniu podlega przełącznikowi
                przypomnień w ustawieniach — stąd „jeśli włączone”. */}
            <span>
              Opiekunowie dostaną e-mail ze starym i nowym terminem, jeśli powiadomienia
              o zajęciach są włączone w ustawieniach.
            </span>
          </li>
          <li>
            <span>Poprzednia data, powód i Twoje konto trafią do historii zmian terminów.</span>
          </li>
          <li>
            <span>Kolejne terminy zostają na swoich datach.</span>
          </li>
        </ul>
      </div>

      <label className="form-field">
        <span>Nowy termin</span>
        <input type="datetime-local" value={when} onChange={(event) => setWhen(event.target.value)} />
      </label>

      <label className="form-field">
        <span>Zastępstwo na te zajęcia</span>
        <select
          value={substituteId}
          onChange={(event) => {
            setSubstituteId(event.target.value);
            onSetSubstitute(event.target.value || null);
          }}
        >
          <option value="">Bez zastępcy</option>
          {instructors.map((instructor) => (
            <option key={instructor.id} value={instructor.id}>
              {instructor.displayName}
            </option>
          ))}
        </select>
      </label>

      <label className="form-field">
        <span>Powód zmiany</span>
        <input
          value={reason}
          maxLength={1000}
          placeholder="np. kolizja z wywiadówką"
          onChange={(event) => setReason(event.target.value)}
        />
      </label>

      <label className="checkbox-field">
        <input type="checkbox" checked={notify} onChange={() => setNotify((current) => !current)} />
        <span>Dodatkowo poinformowano opiekunów telefonicznie lub SMS-em</span>
      </label>
    </Dialog>
  );
}

function LinksDialog({
  session,
  busy,
  onClose,
  onConfirm,
}: {
  session: ScheduledSession;
  busy: boolean;
  onClose: () => void;
  onConfirm: (meetingUrl: string | null, recordingUrl: string | null) => void;
}) {
  const [meetingUrl, setMeetingUrl] = useState(session.sessionMeetingUrl ?? "");
  const [recordingUrl, setRecordingUrl] = useState(session.recordingUrl ?? "");

  return (
    <Dialog
      title="Linki tego terminu"
      description={`Lekcja ${session.sequenceNumber} — ${formatFriendlyDateTime(session.scheduledAt)}`}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button disabled={busy} onClick={() => onConfirm(meetingUrl.trim() || null, recordingUrl.trim() || null)}>
            Zapisz linki
          </Button>
        </>
      }
    >
      <label className="form-field">
        <span>Link do spotkania — puste przywraca link grupy</span>
        <input
          type="url"
          value={meetingUrl}
          placeholder="https://meet.google.com/..."
          onChange={(event) => setMeetingUrl(event.target.value)}
        />
      </label>

      <label className="form-field">
        <span>Nagranie — rodzic zobaczy je po zakończeniu zajęć</span>
        <input
          type="url"
          value={recordingUrl}
          placeholder="https://..."
          onChange={(event) => setRecordingUrl(event.target.value)}
        />
      </label>
    </Dialog>
  );
}

/**
 * Zmiana statusu terminu.
 *
 * Zastępuje `<select value="">Zmień status…</select>`, który wykonywał operację
 * natychmiast po wybraniu pozycji — bez potwierdzenia, bez możliwości cofnięcia
 * i z powodem dociąganym z pola, które użytkownik mógł zostawić puste.
 */
function StatusDialog({
  session,
  statusOptions,
  busy,
  onClose,
  onConfirm,
}: {
  session: ScheduledSession;
  statusOptions: SessionStatusOption[];
  busy: boolean;
  onClose: () => void;
  onConfirm: (status: string, reason: string | null, notify: boolean) => void;
}) {
  const [status, setStatus] = useState("");
  const [reason, setReason] = useState("");
  const [notify, setNotify] = useState(false);
  const chosen = statusOptions.find((option) => option.value === status);

  return (
    <Dialog
      title="Zmień status terminu"
      description={`Lekcja ${session.sequenceNumber} — obecnie „${session.statusLabel}”`}
      onClose={onClose}
      dismissOnScrim={false}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button disabled={busy || !status} onClick={() => onConfirm(status, reason.trim() || null, notify)}>
            Zmień status
          </Button>
        </>
      }
    >
      <label className="form-field">
        <span>Nowy status</span>
        <select value={status} onChange={(event) => setStatus(event.target.value)}>
          <option value="">Wybierz status…</option>
          {statusOptions
            .filter((option) => option.value !== session.status)
            .map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
        </select>
      </label>

      {chosen ? (
        <div className="dialog-consequences">
          <ul>
            <li>
              <span>
                {chosen.countsAsHeld
                  ? "Ten status liczy się jako zajęcia odbyte — wejdzie do frekwencji dzieci."
                  : "Ten status nie liczy się do frekwencji — dzieci nie stracą na nim obecności."}
              </span>
            </li>
            <li>
              <span>Zmiana trafi do historii jako „{session.statusLabel} → {chosen.label}”.</span>
            </li>
          </ul>
        </div>
      ) : null}

      <label className="form-field">
        <span>Powód</span>
        <input
          value={reason}
          maxLength={500}
          placeholder="np. zerwane połączenie po 20 minutach"
          onChange={(event) => setReason(event.target.value)}
        />
      </label>

      <label className="checkbox-field">
        <input type="checkbox" checked={notify} onChange={() => setNotify((current) => !current)} />
        <span>Dodatkowo poinformowano opiekunów telefonicznie lub SMS-em</span>
      </label>
    </Dialog>
  );
}

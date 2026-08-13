import {
  ArrowLeft,
  CalendarClock,
  CalendarPlus,
  Clock3,
  Download,
  History,
  Save,
  UserRound,
  UsersRound,
  Video,
  X,
} from "lucide-react";
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { EmptyState } from "../components/ui/EmptyState";
import { SkeletonList } from "../components/ui/Skeleton";
import { Tabs } from "../components/ui/Tabs";
import { formatDateTime, formatFriendlyDateTime } from "../features/groups/datetime";
import { SessionsPanel } from "../features/groups/SessionsPanel";
import { useGroupDetailsViewModel } from "../features/groups/useGroupDetailsViewModel";
import { ParticipantPicker } from "../features/participants/ParticipantPicker";
import { isUpcomingSession } from "../types/group";
import type { Participant, ScheduledSession } from "../types/group";

type ParticipantRowProps = {
  participant: Participant;
  busy: boolean;
  onUnenroll: (id: string) => void;
};

function ParticipantRow({ participant, busy, onUnenroll }: ParticipantRowProps) {
  return (
    <div className="participant-card">
      <div className="participant-card-header">
        <div className="participant-avatar" aria-hidden="true">
          {getParticipantInitials(participant)}
        </div>
        <div>
          <strong>
            {participant.firstName} {participant.lastName}
          </strong>
          <span>
            {participant.enrollmentStatus === "waitlisted" ? "Lista oczekujących · " : ""}
            {participant.phone || participant.email || "Brak danych kontaktowych"}
          </span>
        </div>
      </div>

      <div className="participant-row-actions">
        <Link to="/admin/participants">
          <Button variant="secondary">Edytuj dane</Button>
        </Link>
        <Button variant="ghost" onClick={() => onUnenroll(participant.id)} disabled={busy}>
          <X className="button-icon" aria-hidden="true" />
          Wypisz z grupy
        </Button>
      </div>
    </div>
  );
}

type GroupTab = "terminy" | "uczestnicy" | "frekwencja" | "historia" | "ustawienia";

/**
 * Szczegóły grupy.
 *
 * Poprzednia wersja miała 663 linie i sześć sekcji na jednej stronie: ustawienia grupy,
 * dodawanie terminu, listę terminów z rozwijanymi formularzami, historię zmian, uczestników
 * i frekwencję. Był to najgęstszy widok w aplikacji i jednocześnie ten, w którym wykonuje
 * się najbardziej ryzykowne operacje.
 *
 * Zakładki nie chowają niczego przed użytkownikiem — pokazują, że to są odrębne zadania,
 * i pozwalają wejść od razu w to jedno, po które się przyszło.
 */
export function GroupDetailsPage() {
  const { groupId } = useParams();
  const vm = useGroupDetailsViewModel(groupId);

  const [tab, setTab] = useState<GroupTab>("terminy");
  const [editName, setEditName] = useState("");
  const [editInstructorId, setEditInstructorId] = useState("");
  const [editCapacity, setEditCapacity] = useState("");
  const [editMeetingUrl, setEditMeetingUrl] = useState("");
  const [addLessonId, setAddLessonId] = useState("");
  const [addWhen, setAddWhen] = useState("");
  const [addSubstituteInstructorId, setAddSubstituteInstructorId] = useState("");

  useEffect(() => {
    if (vm.group) {
      setEditName(vm.group.name);
      setEditInstructorId(vm.group.instructorId);
      setEditCapacity(vm.group.capacity?.toString() ?? "");
      setEditMeetingUrl(vm.group.meetingUrl ?? "");
    }
  }, [vm.group?.id]);

  if (vm.loading) {
    return (
      <section className="page-section">
        <SkeletonList rows={3} label="Ładowanie grupy" />
      </section>
    );
  }

  if (vm.error && !vm.group) {
    return (
      <section className="page-section">
        <div className="list-state list-state-error" role="alert">
          {vm.error}
        </div>
      </section>
    );
  }

  if (!vm.group) {
    return (
      <section className="page-section">
        <div className="list-state">Nie znaleziono grupy.</div>
      </section>
    );
  }

  const group = vm.group;
  const nextSession = getNextSession(group.sessions);
  const waitlisted = group.participants.filter((participant) => participant.enrollmentStatus === "waitlisted").length;

  return (
    <section className="page-section group-details-page">
      <div className="group-details-hero">
        <div className="group-details-title">
          <span className="group-avatar group-details-avatar" aria-hidden="true">
            {getGroupInitials(group.name)}
          </span>
          <div>
            <span className="eyebrow">Grupa</span>
            <h1>{group.name}</h1>
            <p>
              <UserRound size={15} aria-hidden="true" />
              {group.instructorName}
            </p>
            {group.meetingUrl ? (
              <p className="group-meeting-link">
                <Video size={15} aria-hidden="true" />
                <a href={group.meetingUrl} target="_blank" rel="noreferrer">
                  Dołącz do spotkania online
                </a>
              </p>
            ) : null}
          </div>
        </div>
        <div className="group-details-actions">
          <Link to="/admin/groups">
            <Button variant="secondary">
              <ArrowLeft className="button-icon" aria-hidden="true" />
              Wróć
            </Button>
          </Link>
        </div>
      </div>

      {vm.error ? (
        <div className="list-state list-state-error" role="alert">
          {vm.error}
        </div>
      ) : null}

      <div className="groups-overview group-details-overview" aria-label="Podsumowanie grupy">
        <SummaryTile icon={<UsersRound size={20} />} value={group.participants.length} label={`Uczestnicy (${waitlisted} oczek.)`} />
        <SummaryTile icon={<CalendarClock size={20} />} value={group.sessions.length} label="Terminy" />
        <SummaryTile icon={<Clock3 size={20} />} value={vm.summary?.heldSessions ?? 0} label="Odbyte zajęcia" />
        <SummaryTile
          icon={<CalendarPlus size={20} />}
          value={nextSession ? formatFriendlyDateTime(nextSession.scheduledAt) : "Brak"}
          label="Najbliższy termin"
        />
      </div>

      <Tabs
        ariaLabel="Sekcje grupy"
        value={tab}
        onChange={(next) => setTab(next as GroupTab)}
        tabs={[
          { value: "terminy", label: "Terminy", count: group.sessions.length },
          { value: "uczestnicy", label: "Uczestnicy", count: group.participants.length },
          { value: "frekwencja", label: "Frekwencja" },
          { value: "historia", label: "Historia zmian" },
          { value: "ustawienia", label: "Ustawienia" },
        ]}
      />

      {tab === "terminy" ? (
        <SessionsPanel
          group={group}
          busy={vm.busy}
          instructors={vm.instructors}
          statusOptions={vm.statusOptions}
          onCancel={vm.cancelTerm}
          onReschedule={vm.rescheduleTerm}
          onSetSubstitute={vm.setSubstitute}
          onSetLinks={vm.setLinks}
          onChangeStatus={vm.changeStatus}
          onExport={vm.downloadSessionAttendanceExport}
        />
      ) : null}

      {tab === "uczestnicy" ? (
        <div className="group-tab-panel">
          <div className="group-section-head">
            <div>
              <span className="eyebrow">Lista</span>
              <h2>Uczestnicy</h2>
            </div>
            <span>{group.participants.length} osób</span>
          </div>

          {group.participants.length === 0 ? (
            <EmptyState
              icon={UsersRound}
              title="Grupa jest pusta"
              description="Dopisz dziecko z bazy albo utwórz nowe — zapis do grupy uruchamia harmonogram i powiadomienia."
              compact
            />
          ) : null}

          <div className="participant-list">
            {group.participants.map((participant) => (
              <ParticipantRow
                key={participant.id}
                participant={participant}
                busy={vm.busy}
                onUnenroll={vm.unenrollParticipantFromGroup}
              />
            ))}
          </div>

          <ParticipantPicker
            excludeIds={group.participants.map((participant) => participant.id)}
            onPickExisting={vm.pickExistingParticipant}
            onCreateNew={vm.createAndEnrollParticipant}
            disabled={vm.busy}
          />
        </div>
      ) : null}

      {tab === "frekwencja" ? (
        <div className="group-tab-panel">
          <div className="group-section-head">
            <div>
              <span className="eyebrow">Obecność</span>
              <h2>Frekwencja</h2>
            </div>
            <Button variant="secondary" disabled={vm.busy} onClick={vm.downloadAttendanceExport}>
              <Download className="button-icon" aria-hidden="true" />
              Eksport CSV
            </Button>
          </div>

          {!vm.summary || vm.summary.heldSessions === 0 ? (
            <EmptyState
              icon={Clock3}
              title="Brak zakończonych zajęć"
              description="Frekwencja pojawi się po pierwszych poprowadzonych zajęciach."
              compact
            />
          ) : (
            <table className="attendance-table">
              <thead>
                <tr>
                  <th>Uczestnik</th>
                  <th>Obecności</th>
                  <th>Frekwencja</th>
                </tr>
              </thead>
              <tbody>
                {vm.summary.participants.map((participant) => (
                  <tr key={participant.participantId}>
                    <td>
                      {participant.firstName} {participant.lastName}
                    </td>
                    <td>
                      {participant.presentCount} / {participant.heldCount}
                    </td>
                    <td>{participant.ratePercent}%</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      ) : null}

      {tab === "historia" ? (
        <div className="group-tab-panel">
          <div className="group-section-head">
            <div>
              <span className="eyebrow">Kontrola</span>
              <h2>Historia zmian terminów</h2>
              <p>Kto, kiedy i dlaczego przesunął albo odwołał zajęcia — oraz czy poszła informacja do opiekunów.</p>
            </div>
            <Button variant="secondary" disabled={vm.busy} onClick={vm.loadHistory}>
              <History className="button-icon" aria-hidden="true" />
              {vm.history ? "Odśwież" : "Pokaż historię"}
            </Button>
          </div>

          {/* Historia ładuje się na żądanie — to widok kontrolny, otwierany rzadko. */}
          {vm.history === null ? <p className="cue-empty">Kliknij „Pokaż historię”, żeby ją wczytać.</p> : null}
          {vm.history?.length === 0 ? <p className="cue-empty">Brak zmian terminów w tej grupie.</p> : null}

          {vm.history && vm.history.length > 0 ? (
            <div className="compact-list">
              {vm.history.map((change) => (
                <div className="compact-row session-change-row" key={change.id}>
                  <History size={16} aria-hidden="true" />
                  <span>
                    <strong>
                      Termin #{change.sequenceNumber} · {change.changeTypeLabel}
                    </strong>
                    {change.previousScheduledAt ? (
                      <small>
                        {formatDateTime(change.previousScheduledAt)}
                        {change.newScheduledAt ? ` → ${formatDateTime(change.newScheduledAt)}` : ""}
                      </small>
                    ) : null}
                    {change.details ? <small>{change.details}</small> : null}
                    {change.reason ? <small>Powód: {change.reason}</small> : null}
                    <small>
                      {formatDateTime(change.changedAt)} · {change.changedByName ?? "system"} ·{" "}
                      {change.guardiansNotified ? "opiekunowie powiadomieni" : "bez powiadomienia"}
                    </small>
                  </span>
                </div>
              ))}
            </div>
          ) : null}
        </div>
      ) : null}

      {tab === "ustawienia" ? (
        <div className="group-details-layout">
          <section className="editor-fieldset group-settings-card">
            <legend>Edycja grupy</legend>
            <label className="form-field">
              <span>Nazwa</span>
              <input value={editName} onChange={(event) => setEditName(event.target.value)} />
            </label>
            <label className="form-field">
              <span>Instruktor</span>
              <select value={editInstructorId} onChange={(event) => setEditInstructorId(event.target.value)}>
                {vm.instructors.map((instructor) => (
                  <option key={instructor.id} value={instructor.id}>
                    {instructor.displayName}
                  </option>
                ))}
              </select>
            </label>
            <label className="form-field">
              <span>Limit miejsc</span>
              <input
                type="number"
                min={1}
                value={editCapacity}
                onChange={(event) => setEditCapacity(event.target.value)}
                placeholder="Bez limitu"
              />
            </label>
            <label className="form-field">
              <span>Link do spotkania online</span>
              <input
                type="url"
                value={editMeetingUrl}
                onChange={(event) => setEditMeetingUrl(event.target.value)}
                placeholder="https://zoom.us/j/... lub https://meet.google.com/..."
              />
            </label>
            <Button
              onClick={() =>
                vm.saveGroup(
                  editName,
                  editInstructorId,
                  editCapacity ? Number(editCapacity) : null,
                  editMeetingUrl || null,
                )
              }
              disabled={vm.busy}
            >
              <Save className="button-icon" aria-hidden="true" />
              Zapisz grupę
            </Button>
          </section>

          <section className="editor-fieldset add-session-card">
            <legend>Dodaj termin</legend>
            <label className="form-field">
              <span>Lekcja</span>
              <select value={addLessonId} onChange={(event) => setAddLessonId(event.target.value)}>
                <option value="">Wybierz lekcję...</option>
                {vm.readyLessons.map((lesson) => (
                  <option key={lesson.id} value={lesson.id}>
                    {lesson.title}
                  </option>
                ))}
              </select>
            </label>
            <label className="form-field">
              <span>Data</span>
              <input type="datetime-local" value={addWhen} onChange={(event) => setAddWhen(event.target.value)} />
            </label>
            <label className="form-field">
              <span>Zastępca</span>
              <select
                value={addSubstituteInstructorId}
                onChange={(event) => setAddSubstituteInstructorId(event.target.value)}
              >
                <option value="">Bez zastępcy</option>
                {vm.instructors.map((instructor) => (
                  <option key={instructor.id} value={instructor.id}>
                    {instructor.displayName}
                  </option>
                ))}
              </select>
            </label>
            <Button
              variant="secondary"
              disabled={vm.busy || !addLessonId || !addWhen}
              onClick={() => {
                vm.addTerm(addLessonId, addWhen, addSubstituteInstructorId || null);
                setAddLessonId("");
                setAddWhen("");
                setAddSubstituteInstructorId("");
              }}
            >
              <CalendarPlus className="button-icon" aria-hidden="true" />
              Dodaj termin
            </Button>
          </section>
        </div>
      ) : null}
    </section>
  );
}

function SummaryTile({ icon, value, label }: { icon: React.ReactNode; value: string | number; label: string }) {
  return (
    <div className="groups-summary-card">
      <span className="groups-summary-icon" aria-hidden="true">
        {icon}
      </span>
      <div>
        <strong>{value}</strong>
        <span>{label}</span>
      </div>
    </div>
  );
}

function getNextSession(sessions: ScheduledSession[]): ScheduledSession | null {
  return (
    sessions
      .filter((session) => isUpcomingSession(session.status))
      .sort((first, second) => new Date(first.scheduledAt).getTime() - new Date(second.scheduledAt).getTime())[0] ?? null
  );
}

function getGroupInitials(name: string): string {
  return name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0])
    .join("")
    .toUpperCase();
}

function getParticipantInitials(participant: Participant): string {
  const initials = `${participant.firstName.trim()[0] ?? ""}${participant.lastName.trim()[0] ?? ""}`.toUpperCase();
  return initials || "?";
}

import { useState } from "react";
import { CalendarClock, GraduationCap, Trash2, UserPlus, X } from "lucide-react";
import { Button } from "../components/ui/Button";
import { Dialog } from "../components/ui/Dialog";
import { EmptyState } from "../components/ui/EmptyState";
import { StatusBadge } from "../components/ui/StatusBadge";
import { Tabs } from "../components/ui/Tabs";
import { useDialogs } from "../features/dialog/DialogContext";
import { useToast } from "../features/toast/ToastContext";
import { formatDateTime } from "../features/groups/datetime";
import { useTrialsViewModel } from "../features/trials/useTrialsViewModel";
import type { TrialView } from "../features/trials/useTrialsViewModel";
import type { TrialLesson } from "../types/trial";

type ViewModel = ReturnType<typeof useTrialsViewModel>;

/**
 * Lekcje próbne 1:1 — droga od zgłoszenia do zapisanego uczestnika.
 *
 * Ekran jest osobny od grup i uczestników celowo. Kandydat nie jest jeszcze uczestnikiem,
 * a zgłoszenie sprzed dwóch tygodni bez oddzwonienia nie ma się gdzie schować między
 * aktywnymi grupami.
 */
export function AdminTrialsPage() {
  const vm = useTrialsViewModel();
  const [scheduling, setScheduling] = useState<TrialLesson | null>(null);
  const [enrolling, setEnrolling] = useState<TrialLesson | null>(null);

  const visible = vm.buckets[vm.view];

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Administrator</span>
          <h1>Lekcje próbne</h1>
          <p>
            Zgłoszenia na lekcję próbną 1:1 — sprawdzamy, czy dziecko odnajdzie się w programowaniu,
            i dopiero potem zapisujemy je do systemu. Ta lista nie miesza się z aktywnymi grupami.
          </p>
        </div>
      </div>

      {vm.error ? <div className="list-state list-state-error">{vm.error}</div> : null}

      <NewTrialForm vm={vm} />

      <Tabs
        ariaLabel="Etapy zgłoszeń"
        value={vm.view}
        onChange={(next) => vm.setView(next as TrialView)}
        tabs={[
          { value: "uwaga", label: "Czeka na ruch", count: vm.buckets.uwaga.length },
          { value: "umowione", label: "Umówione", count: vm.buckets.umowione.length },
          { value: "zamkniete", label: "Zamknięte", count: vm.buckets.zamkniete.length },
        ]}
      />

      {vm.loading ? <div className="list-state">Ładowanie zgłoszeń…</div> : null}

      {!vm.loading && visible.length === 0 ? (
        <EmptyState
          icon={GraduationCap}
          title={vm.view === "uwaga" ? "Nic nie czeka na decyzję" : "Pusto w tej zakładce"}
          description="Zgłoszenia przyjmujesz formularzem powyżej — wystarczy imię i nazwisko dziecka."
          compact
        />
      ) : null}

      <div className="trial-list">
        {visible.map((trial) => (
          <TrialCard
            key={trial.id}
            trial={trial}
            vm={vm}
            onSchedule={() => setScheduling(trial)}
            onEnroll={() => setEnrolling(trial)}
          />
        ))}
      </div>

      {scheduling ? (
        <ScheduleDialog vm={vm} trial={scheduling} onClose={() => setScheduling(null)} />
      ) : null}

      {enrolling ? <EnrollDialog vm={vm} trial={enrolling} onClose={() => setEnrolling(null)} /> : null}
    </section>
  );
}

function NewTrialForm({ vm }: { vm: ViewModel }) {
  return (
    <div className="editor-fieldset" style={{ marginBottom: 20 }}>
      <legend>Nowe zgłoszenie</legend>
      <div className="participant-fields">
        <label className="form-field">
          <span>Imię dziecka</span>
          <input value={vm.draft.childFirstName} disabled={vm.busy} onChange={(e) => vm.setField("childFirstName", e.target.value)} />
        </label>
        <label className="form-field">
          <span>Nazwisko dziecka</span>
          <input value={vm.draft.childLastName} disabled={vm.busy} onChange={(e) => vm.setField("childLastName", e.target.value)} />
        </label>
        <label className="form-field">
          <span>Data urodzenia</span>
          <input type="date" value={vm.draft.childBirthDate} disabled={vm.busy} onChange={(e) => vm.setField("childBirthDate", e.target.value)} />
        </label>
      </div>

      <div className="participant-fields">
        <label className="form-field">
          <span>Opiekun</span>
          <input value={vm.draft.guardianName} disabled={vm.busy} onChange={(e) => vm.setField("guardianName", e.target.value)} />
        </label>
        <label className="form-field">
          <span>E-mail opiekuna</span>
          <input value={vm.draft.guardianEmail} disabled={vm.busy} onChange={(e) => vm.setField("guardianEmail", e.target.value)} />
        </label>
        <label className="form-field">
          <span>Telefon opiekuna</span>
          <input value={vm.draft.guardianPhone} disabled={vm.busy} onChange={(e) => vm.setField("guardianPhone", e.target.value)} />
        </label>
        {/* Bez tego pola nie da się powiedzieć, który kanał faktycznie przyprowadza dzieci. */}
        <label className="form-field">
          <span>Skąd o nas wie</span>
          <input
            value={vm.draft.source}
            disabled={vm.busy}
            placeholder="polecenie, Facebook, szkoła…"
            onChange={(e) => vm.setField("source", e.target.value)}
          />
        </label>
      </div>

      <label className="form-field">
        <span>Notatka z rozmowy</span>
        <textarea
          rows={2}
          value={vm.draft.requestNote}
          disabled={vm.busy}
          placeholder="Co powiedział rodzic: czego oczekuje, co dziecko już robiło, godziny które pasują"
          onChange={(e) => vm.setField("requestNote", e.target.value)}
        />
      </label>

      <div className="participant-row-actions">
        <Button disabled={vm.busy} onClick={() => void vm.submitTrial()}>
          Przyjmij zgłoszenie
        </Button>
      </div>
    </div>
  );
}

function TrialCard({
  trial,
  vm,
  onSchedule,
  onEnroll,
}: {
  trial: TrialLesson;
  vm: ViewModel;
  onSchedule: () => void;
  onEnroll: () => void;
}) {
  const { confirm } = useDialogs();
  const toast = useToast();

  async function handleDecline(noShow: boolean) {
    const confirmed = await confirm({
      title: noShow ? `Nikt nie przyszedł na termin ${trial.childFirstName}?` : `Zamknąć zgłoszenie ${trial.childFirstName}?`,
      description: "Zgłoszenie trafi do zamkniętych. Danych nie kasujemy — zostają w historii pozyskania.",
      confirmLabel: noShow ? "Oznacz nieobecność" : "Zamknij zgłoszenie",
      tone: "danger",
    });

    if (confirmed && (await vm.decline(trial.id, { noShow }))) {
      toast.success("Zgłoszenie zamknięte.");
    }
  }

  async function handleDelete() {
    const confirmed = await confirm({
      title: `Usunąć zgłoszenie ${trial.childFirstName} ${trial.childLastName}?`,
      description: "Rekord zniknie z bazy razem z diagnozą.",
      confirmLabel: "Usuń zgłoszenie",
      tone: "danger",
      consequences: ["Do zamykania spraw służy „Rezygnacja” — zachowuje historię pozyskania klienta."],
    });

    if (confirmed) {
      await vm.remove(trial.id);
    }
  }

  const contact = [trial.guardianName, trial.guardianPhone, trial.guardianEmail].filter(Boolean).join(" · ");

  return (
    <div className={`trial-card${trial.needsAttention ? " trial-card-attention" : ""}`}>
      <div className="trial-card-head">
        <div>
          <strong>
            {trial.childFirstName} {trial.childLastName}
            {trial.childAge !== null ? <span className="participant-age"> · {trial.childAge} lat</span> : null}
          </strong>
          <span>{contact || "Brak danych kontaktowych"}</span>
        </div>
        <StatusBadge label={trial.statusLabel} tone={statusTone(trial.status)} dot />
      </div>

      <div className="trial-card-meta">
        {trial.scheduledAt ? (
          <span>
            {formatDateTime(trial.scheduledAt)}
            {trial.instructorName ? ` · ${trial.instructorName}` : ""}
          </span>
        ) : (
          <span className="cue-empty">Bez terminu</span>
        )}
        {trial.source ? <span className="chip chip-muted">{trial.source}</span> : null}
      </div>

      {trial.requestNote ? <p className="trial-note">{trial.requestNote}</p> : null}

      {trial.hasDiagnosis ? (
        <div className="trial-diagnosis">
          <strong>Diagnoza po lekcji</strong>
          <ul>
            <li>{trial.readingLabel}</li>
            <li>{trial.computerLabel}</li>
            <li>{trial.programmingLabel}</li>
            <li>
              <b>{trial.recommendationLabel}</b>
              {trial.recommendedLevel ? ` — ${trial.recommendedLevel}` : ""}
            </li>
          </ul>
          {trial.diagnosisNote ? <p>{trial.diagnosisNote}</p> : null}
        </div>
      ) : null}

      {trial.declineReason ? <p className="trial-note">Powód: {trial.declineReason}</p> : null}

      <div className="participant-row-actions">
        {trial.status !== "enrolled" ? (
          <Button variant="secondary" disabled={vm.busy} onClick={onSchedule}>
            <CalendarClock className="button-icon" aria-hidden="true" />
            {trial.scheduledAt ? "Zmień termin" : "Umów lekcję"}
          </Button>
        ) : null}

        {/* Zapis jest możliwy dopiero po diagnozie: decyzja bez niej to zgadywanie,
            a instruktor jest jedyną osobą, która widziała dziecko przy komputerze. */}
        {trial.status !== "enrolled" ? (
          <Button disabled={vm.busy || !trial.hasDiagnosis} onClick={onEnroll} title={trial.hasDiagnosis ? undefined : "Poczekaj na diagnozę instruktora."}>
            <UserPlus className="button-icon" aria-hidden="true" />
            Dodaj do systemu
          </Button>
        ) : (
          <span className="cue-empty">Zapisany jako uczestnik</span>
        )}

        {trial.status !== "enrolled" ? (
          <>
            <Button variant="ghost" disabled={vm.busy} onClick={() => void handleDecline(false)}>
              <X className="button-icon" aria-hidden="true" />
              Rezygnacja
            </Button>
            <Button variant="ghost" disabled={vm.busy} onClick={() => void handleDecline(true)}>
              Nie pojawił się
            </Button>
          </>
        ) : null}

        <Button variant="ghost" disabled={vm.busy} onClick={() => void handleDelete()}>
          <Trash2 className="button-icon" aria-hidden="true" />
          Usuń
        </Button>
      </div>
    </div>
  );
}

function ScheduleDialog({ vm, trial, onClose }: { vm: ViewModel; trial: TrialLesson; onClose: () => void }) {
  const [instructorId, setInstructorId] = useState(trial.instructorId ?? "");
  const [scheduledAt, setScheduledAt] = useState(toLocalInput(trial.scheduledAt));
  const [meetingUrl, setMeetingUrl] = useState(trial.meetingUrl ?? "");
  const [lessonId, setLessonId] = useState(trial.lessonId ?? "");
  const toast = useToast();

  return (
    <Dialog
      title={`Termin lekcji próbnej: ${trial.childFirstName}`}
      description="Lekcja jest jeden na jeden — instruktor zobaczy ją w swojej osobnej zakładce, poza grafikiem grup."
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button
            disabled={vm.busy}
            onClick={async () => {
              const ok = await vm.schedule(trial.id, {
                instructorId: instructorId || null,
                scheduledAt: scheduledAt ? new Date(scheduledAt).toISOString() : null,
                meetingUrl: meetingUrl.trim() || null,
                lessonId: lessonId || null,
              });

              if (ok) {
                toast.success("Termin zapisany.");
                onClose();
              }
            }}
          >
            Zapisz termin
          </Button>
        </>
      }
    >
      <label className="form-field">
        <span>Instruktor</span>
        <select value={instructorId} onChange={(event) => setInstructorId(event.target.value)}>
          <option value="">Bez przypisania</option>
          {vm.instructors.map((instructor) => (
            <option key={instructor.id} value={instructor.id}>
              {instructor.displayName}
            </option>
          ))}
        </select>
      </label>

      <label className="form-field">
        <span>Termin</span>
        <input type="datetime-local" value={scheduledAt} onChange={(event) => setScheduledAt(event.target.value)} />
      </label>

      <label className="form-field">
        <span>Konspekt pokazowy</span>
        <select value={lessonId} onChange={(event) => setLessonId(event.target.value)}>
          <option value="">Bez przypisanego konspektu</option>
          {(vm.board?.lessonOptions ?? []).map((lesson) => (
            <option key={lesson.id} value={lesson.id}>
              {lesson.title} — {lesson.durationMinutes} min
            </option>
          ))}
        </select>
        <small>Lista zawiera wyłącznie konspekty oznaczone jako pokazowe.</small>
      </label>

      <label className="form-field">
        <span>Link do spotkania</span>
        <input value={meetingUrl} placeholder="https://meet.google.com/..." onChange={(event) => setMeetingUrl(event.target.value)} />
      </label>
    </Dialog>
  );
}

function EnrollDialog({ vm, trial, onClose }: { vm: ViewModel; trial: TrialLesson; onClose: () => void }) {
  const [withAccount, setWithAccount] = useState(Boolean(trial.guardianEmail));
  const toast = useToast();

  return (
    <Dialog
      title={`Dodać ${trial.childFirstName} do systemu?`}
      description="Kandydat staje się uczestnikiem — od tego momentu można go zapisać do grupy."
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button
            disabled={vm.busy}
            onClick={async () => {
              const result = await vm.enroll(trial.id, withAccount);

              if (!result) {
                return;
              }

              if (result.error) {
                toast.error(`Dziecko zapisane, ale konto opiekuna nie powstało: ${result.error}`);
              } else if (result.guardianAccountCreated && result.invitationSent) {
                toast.success(`Zapisane. Zaproszenie poszło na ${result.guardianEmail}.`);
              } else {
                toast.success(`${result.participantName} jest w bazie uczestników.`);
              }

              onClose();
            }}
          >
            Dodaj do systemu
          </Button>
        </>
      }
    >
      <div className="dialog-consequences">
        <ul>
          <li>Dane dziecka i opiekuna przeniosą się ze zgłoszenia — nie trzeba ich przepisywać.</li>
          <li>Obserwacje z lekcji próbnej trafią do notatek uczestnika, żeby prowadzący wiedział, czego się spodziewać.</li>
          <li>Zgody RODO zostają puste — nikt ich jeszcze nie udzielił. Uzupełnij je w kartotece dziecka.</li>
          <li>Do grupy zapisujesz osobno, na ekranie uczestników.</li>
        </ul>
      </div>

      <label className="checkbox-field">
        <input
          type="checkbox"
          checked={withAccount}
          disabled={!trial.guardianEmail}
          onChange={(event) => setWithAccount(event.target.checked)}
        />
        <span>
          {trial.guardianEmail
            ? `Załóż konto opiekunowi (${trial.guardianEmail}) i wyślij zaproszenie`
            : "Brak e-maila opiekuna — konto założysz później z karty dziecka"}
        </span>
      </label>
    </Dialog>
  );
}

function statusTone(status: string): "success" | "warning" | "danger" | "neutral" {
  if (status === "enrolled") {
    return "success";
  }

  if (status === "declined" || status === "noshow") {
    return "neutral";
  }

  return status === "diagnosed" ? "warning" : "neutral";
}

/** ISO → wartość dla `datetime-local`, w czasie lokalnym przeglądarki. */
function toLocalInput(value: string | null): string {
  if (!value) {
    return "";
  }

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "";
  }

  const pad = (part: number) => String(part).padStart(2, "0");
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

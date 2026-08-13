import { useState } from "react";
import { GraduationCap, Video } from "lucide-react";
import { Button } from "../components/ui/Button";
import { EmptyState } from "../components/ui/EmptyState";
import { StatusBadge } from "../components/ui/StatusBadge";
import { useToast } from "../features/toast/ToastContext";
import { formatFriendlyDateTime } from "../features/groups/datetime";
import { useMyTrialsViewModel } from "../features/trials/useMyTrialsViewModel";
import type { TrialBoard, TrialLesson } from "../types/trial";

/**
 * Lekcje próbne instruktora — osobno od grafiku grup.
 *
 * To nie są zajęcia, tylko badanie: jedno dziecko, żadnej listy obecności, żadnego kursu.
 * Jedyne, co prowadzący ma stąd wynieść, to odpowiedź na trzy pytania i rekomendacja —
 * dlatego formularz diagnozy jest całą treścią tego ekranu.
 */
export function InstructorTrialsPage() {
  const vm = useMyTrialsViewModel();

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Instruktor</span>
          <h1>Lekcje próbne</h1>
          <p>
            Spotkania jeden na jeden z dzieckiem, które dopiero się do nas zgłosiło. Po lekcji wypełnij
            diagnozę — na jej podstawie administracja decyduje o zapisie.
          </p>
        </div>
      </div>

      {vm.error ? <div className="list-state list-state-error">{vm.error}</div> : null}
      {vm.loading ? <div className="list-state">Ładowanie lekcji próbnych…</div> : null}

      {!vm.loading && vm.trials.length === 0 ? (
        <EmptyState
          icon={GraduationCap}
          title="Nie masz przypisanych lekcji próbnych"
          description="Gdy administracja umówi kandydata z Tobą, spotkanie pojawi się tutaj."
        />
      ) : null}

      {vm.upcoming.length > 0 ? <h2 className="trial-section-title">Do poprowadzenia</h2> : null}
      {vm.upcoming.map((trial) => (
        <TrialCard key={trial.id} trial={trial} board={vm.board} busy={vm.busy} onSave={vm.saveDiagnosis} />
      ))}

      {vm.done.length > 0 ? <h2 className="trial-section-title">Po lekcji</h2> : null}
      {vm.done.map((trial) => (
        <TrialCard key={trial.id} trial={trial} board={vm.board} busy={vm.busy} onSave={vm.saveDiagnosis} />
      ))}
    </section>
  );
}

function TrialCard({
  trial,
  board,
  busy,
  onSave,
}: {
  trial: TrialLesson;
  board: TrialBoard | null;
  busy: boolean;
  onSave: (id: string, request: {
    reading: string;
    computer: string;
    programming: string;
    recommendation: string;
    recommendedLevel: string | null;
    diagnosisNote: string | null;
  }) => Promise<boolean>;
}) {
  const [reading, setReading] = useState(trial.reading);
  const [computer, setComputer] = useState(trial.computer);
  const [programming, setProgramming] = useState(trial.programming);
  const [recommendation, setRecommendation] = useState(trial.recommendation);
  const [level, setLevel] = useState(trial.recommendedLevel ?? "");
  const [note, setNote] = useState(trial.diagnosisNote ?? "");
  const toast = useToast();

  return (
    <div className="trial-card">
      <div className="trial-card-head">
        <div>
          <strong>
            {trial.childFirstName} {trial.childLastName}
            {trial.childAge !== null ? <span className="participant-age"> · {trial.childAge} lat</span> : null}
          </strong>
          <span>{trial.scheduledAt ? formatFriendlyDateTime(trial.scheduledAt) : "Termin nieustalony"}</span>
        </div>
        <StatusBadge label={trial.statusLabel} tone={trial.hasDiagnosis ? "success" : "warning"} dot />
      </div>

      {/* Notatka ze zgłoszenia to jedyne, co prowadzący wie o dziecku przed spotkaniem. */}
      {trial.requestNote ? <p className="trial-note">Ze zgłoszenia: {trial.requestNote}</p> : null}

      {trial.meetingUrl ? (
        <div className="participant-row-actions">
          <a href={trial.meetingUrl} target="_blank" rel="noreferrer">
            <Button variant="secondary">
              <Video className="button-icon" aria-hidden="true" />
              Dołącz do spotkania
            </Button>
          </a>
        </div>
      ) : null}

      <div className="participant-fields">
        <Select label="Czytanie" value={reading} options={board?.readingOptions} onChange={setReading} />
        <Select label="Obsługa komputera" value={computer} options={board?.computerOptions} onChange={setComputer} />
        <Select label="Doświadczenie" value={programming} options={board?.programmingOptions} onChange={setProgramming} />
        <Select label="Rekomendacja" value={recommendation} options={board?.recommendationOptions} onChange={setRecommendation} />
      </div>

      <label className="form-field">
        <span>Poziom, od którego zacząć</span>
        <input value={level} placeholder="np. Poziom 1 - Scratch" onChange={(event) => setLevel(event.target.value)} />
      </label>

      <label className="form-field">
        <span>Obserwacje z lekcji</span>
        <textarea
          rows={3}
          value={note}
          placeholder="Co poszło łatwo, co sprawiło trudność, jak dziecko reagowało na podpowiedzi"
          onChange={(event) => setNote(event.target.value)}
        />
      </label>

      <div className="participant-row-actions">
        <Button
          disabled={busy}
          onClick={async () => {
            const saved = await onSave(trial.id, {
              reading,
              computer,
              programming,
              recommendation,
              recommendedLevel: level.trim() || null,
              diagnosisNote: note.trim() || null,
            });

            if (saved) {
              toast.success("Diagnoza zapisana.");
            }
          }}
        >
          Zapisz diagnozę
        </Button>
        {trial.status === "enrolled" ? <span className="cue-empty">Dziecko zostało zapisane do systemu.</span> : null}
      </div>
    </div>
  );
}

function Select({
  label,
  value,
  options,
  onChange,
}: {
  label: string;
  value: string;
  options?: { value: string; label: string }[];
  onChange: (value: string) => void;
}) {
  return (
    <label className="form-field">
      <span>{label}</span>
      <select value={value} onChange={(event) => onChange(event.target.value)}>
        {(options ?? []).map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </label>
  );
}

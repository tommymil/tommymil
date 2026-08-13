import { Button } from "../../components/ui/Button";
import { Dialog } from "../../components/ui/Dialog";
import { SkeletonList } from "../../components/ui/Skeleton";
import { useSessionProgressViewModel } from "./useSessionProgressViewModel";

type Props = {
  sessionId: string | undefined;
  /** Świadomie nie `children`: to nazwa zarezerwowana przez Reacta i tablica obiektów pod tą
   * nazwą prosi się o pomyłkę przy renderowaniu. */
  participants: { participantId: string; firstName: string; lastName: string }[];
  onClose: () => void;
};

/**
 * Wpisy o postępach dla terminu — odpowiedź na pytanie rodzica „czego dziecko się nauczyło?”.
 *
 * Obie notatki są opisane jako widoczne dla rodzica wprost w formularzu. Uwagi wewnętrzne mają
 * swoje miejsce w notatce z zajęć i nie mogą trafić tutaj przez pomyłkę.
 */
export function SessionProgressPanel({ sessionId, participants, onClose }: Props) {
  const vm = useSessionProgressViewModel(sessionId, participants, true);

  return (
    <Dialog
      title="Postępy dzieci"
      description="To, co tu wpiszesz, zobaczy rodzic w swoim portalu. Uwagi wyłącznie dla zespołu wpisz w notatkę z zajęć."
      wide
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Zamknij
          </Button>
          <Button onClick={vm.persist} disabled={vm.busy || vm.drafts.length === 0}>
            {vm.busy ? "Zapisywanie…" : "Zapisz postępy"}
          </Button>
        </>
      }
    >
      <>
        {vm.error ? (
          <div className="list-state list-state-error" role="alert">
            {vm.error}
          </div>
        ) : null}
        {vm.loading ? <SkeletonList rows={2} label="Ładowanie postępów" /> : null}
        {vm.saved ? (
          <div className="list-state" role="status">
            Zapisano.
          </div>
        ) : null}

        <ul className="attendance-list">
          {vm.drafts.map((draft) => (
            <li key={draft.participantId}>
              <strong>
                {draft.firstName} {draft.lastName}
              </strong>

              <div className="attendance-detail">
                <select
                  value={draft.autonomy}
                  onChange={(event) => vm.update(draft.participantId, { autonomy: event.target.value })}
                  aria-label={`Samodzielność: ${draft.firstName} ${draft.lastName}`}
                >
                  {vm.options.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>

                <label className="attendance-makeup">
                  <input
                    type="checkbox"
                    checked={draft.lessonCompleted}
                    onChange={(event) =>
                      vm.update(draft.participantId, { lessonCompleted: event.target.checked })
                    }
                  />
                  <span>Materiał ukończony</span>
                </label>
              </div>

              <div className="attendance-detail">
                <input
                  value={draft.noteForParent ?? ""}
                  onChange={(event) =>
                    vm.update(draft.participantId, { noteForParent: event.target.value || null })
                  }
                  placeholder="Notatka dla rodzica, np. samodzielnie znalazł błąd w pętli"
                  maxLength={2000}
                  aria-label={`Notatka dla rodzica: ${draft.firstName} ${draft.lastName}`}
                />
                <input
                  value={draft.nextStep ?? ""}
                  onChange={(event) => vm.update(draft.participantId, { nextStep: event.target.value || null })}
                  placeholder="Kolejny krok, np. powtórzyć zmienne w domu"
                  maxLength={2000}
                  aria-label={`Kolejny krok: ${draft.firstName} ${draft.lastName}`}
                />
              </div>
            </li>
          ))}
          {!vm.loading && vm.drafts.length === 0 ? <li className="cue-empty">Grupa nie ma uczestników.</li> : null}
        </ul>
      </>
    </Dialog>
  );
}

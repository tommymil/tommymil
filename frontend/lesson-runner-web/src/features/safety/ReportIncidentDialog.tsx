import { useState } from "react";
import { ShieldAlert } from "lucide-react";
import { Button } from "../../components/ui/Button";
import { Dialog } from "../../components/ui/Dialog";
import type { SafetyOption } from "../../types/safety";

type Child = { participantId: string; firstName: string; lastName: string };

type ReportIncidentDialogProps = {
  kindOptions: SafetyOption[];
  severityOptions: SafetyOption[];
  /**
   * Dzieci, których incydent może dotyczyć — w kokpicie to lista grupy.
   *
   * Świadomie **nie** `children`: to nazwa zarezerwowana przez Reacta na treść komponentu
   * i przekazana jawnie jako atrybut czyta się jak pomyłka.
   */
  participants?: Child[];
  groupId?: string | null;
  sessionId?: string | null;
  busy: boolean;
  onClose: () => void;
  onSubmit: (payload: {
    kind: string;
    severity: string;
    description: string;
    participantIds: string[];
  }) => Promise<void>;
};

/**
 * Zgłoszenie incydentu.
 *
 * Okno mówi wprost, gdzie ten zapis trafia i gdzie **nie** trafia. To nie jest ozdobnik:
 * instruktor musi wiedzieć, że pisze do rejestru bezpieczeństwa, a nie do notatki z zajęć —
 * inaczej albo wpisze tu uwagę edukacyjną, albo (gorzej) nie wpisze incydentu w obawie,
 * że przeczyta go rodzic.
 */
export function ReportIncidentDialog({
  kindOptions,
  severityOptions,
  participants = [],
  groupId,
  sessionId,
  busy,
  onClose,
  onSubmit,
}: ReportIncidentDialogProps) {
  const [kind, setKind] = useState(kindOptions[0]?.value ?? "other");
  const [severity, setSeverity] = useState("medium");
  const [description, setDescription] = useState("");
  const [selected, setSelected] = useState<string[]>([]);

  function toggleChild(participantId: string) {
    setSelected((current) =>
      current.includes(participantId)
        ? current.filter((id) => id !== participantId)
        : [...current, participantId],
    );
  }

  return (
    <Dialog
      title="Zgłoś incydent"
      description="Bezpieczeństwo i zachowanie — rejestr osobny od dziennika zajęć."
      tone="danger"
      wide
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button
            variant="danger"
            disabled={busy || description.trim().length === 0}
            onClick={() =>
              void onSubmit({ kind, severity, description: description.trim(), participantIds: selected })
            }
          >
            {busy ? "Zgłaszanie…" : "Zgłoś incydent"}
          </Button>
        </>
      }
    >
      <div className="dialog-consequences">
        <ul>
          <li>
            <ShieldAlert size={16} aria-hidden="true" />
            <span>
              Zgłoszenie trafia do administracji. <b>Rodzic nigdy go nie zobaczy</b> —
              o sprawie informuje człowiek, w rozmowie.
            </span>
          </li>
          <li>
            <span>
              Ten wpis nie miesza się z notatką z zajęć ani z postępami dziecka. Uwagi
              edukacyjne wpisz tam, gdzie zwykle.
            </span>
          </li>
          <li>
            <span>Inni instruktorzy nie zobaczą tego zgłoszenia — tylko Ty i administracja.</span>
          </li>
        </ul>
      </div>

      <label className="form-field">
        <span>Czego dotyczy</span>
        <select value={kind} onChange={(event) => setKind(event.target.value)}>
          {kindOptions.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </label>

      <label className="form-field">
        <span>Waga</span>
        <select value={severity} onChange={(event) => setSeverity(event.target.value)}>
          {severityOptions.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </label>

      {participants.length > 0 ? (
        <fieldset className="consent-group">
          <legend>Kogo dotyczy (opcjonalnie)</legend>
          {participants.map((child) => (
            <label className="checkbox-field" key={child.participantId}>
              <input
                type="checkbox"
                checked={selected.includes(child.participantId)}
                onChange={() => toggleChild(child.participantId)}
              />
              <span>
                {child.firstName} {child.lastName}
              </span>
            </label>
          ))}
        </fieldset>
      ) : null}

      <label className="form-field">
        <span>Co się stało</span>
        <textarea
          rows={5}
          value={description}
          maxLength={4000}
          placeholder="Opisz przebieg zdarzenia możliwie konkretnie: kto, co, kiedy, w jakiej kolejności."
          onChange={(event) => setDescription(event.target.value)}
        />
      </label>

      {groupId || sessionId ? (
        <p className="cue-empty">Grupa i termin zostaną dopisane automatycznie.</p>
      ) : null}
    </Dialog>
  );
}

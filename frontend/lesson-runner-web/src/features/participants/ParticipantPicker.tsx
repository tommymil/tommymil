import { useEffect, useState } from "react";
import { UserPlus } from "lucide-react";
import { Button } from "../../components/ui/Button";
import { getParticipants } from "../../api/participantsApi";
import type { ParticipantDraft, ParticipantSummary } from "../../types/participant";
import { emptyParticipantDraft } from "../../types/participant";

type ParticipantPickerProps = {
  excludeIds: string[];
  onPickExisting: (participant: ParticipantSummary) => void;
  onCreateNew: (draft: ParticipantDraft) => Promise<boolean>;
  disabled?: boolean;
};

export function ParticipantPicker({ excludeIds, onPickExisting, onCreateNew, disabled }: ParticipantPickerProps) {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<ParticipantSummary[]>([]);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [draft, setDraft] = useState<ParticipantDraft>(emptyParticipantDraft());
  const [creating, setCreating] = useState(false);

  useEffect(() => {
    let ignore = false;

    const timer = setTimeout(() => {
      void getParticipants(query)
        .then((list) => {
          if (!ignore) {
            setResults(list.filter((participant) => !excludeIds.includes(participant.id)).slice(0, 8));
          }
        })
        .catch(() => {
          if (!ignore) {
            setResults([]);
          }
        });
    }, 250);

    return () => {
      ignore = true;
      clearTimeout(timer);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [query, excludeIds.join(",")]);

  function setDraftField(field: keyof ParticipantDraft, value: string) {
    setDraft((current) => ({ ...current, [field]: value }));
  }

  async function submitCreate() {
    if (!draft.firstName.trim() || !draft.lastName.trim()) {
      return;
    }

    try {
      setCreating(true);
      const ok = await onCreateNew(draft);
      if (ok) {
        setDraft(emptyParticipantDraft());
        setShowCreateForm(false);
      }
    } finally {
      setCreating(false);
    }
  }

  return (
    <div className="participant-picker">
      <label className="form-field">
        <span>Szukaj uczestnika z bazy</span>
        <input
          value={query}
          onChange={(event) => setQuery(event.target.value)}
          placeholder="Imię, nazwisko, telefon lub e-mail..."
          disabled={disabled}
        />
      </label>

      {results.length > 0 ? (
        <ul className="picker-results">
          {results.map((participant) => (
            <li key={participant.id} className="picker-result-row">
              <span>
                <strong>{participant.firstName} {participant.lastName}</strong>
                <small>{participant.phone || participant.email || "Brak danych kontaktowych"}</small>
              </span>
              <Button variant="secondary" disabled={disabled} onClick={() => onPickExisting(participant)}>
                Dodaj
              </Button>
            </li>
          ))}
        </ul>
      ) : null}

      {query.trim().length > 0 && results.length === 0 ? (
        <p className="cue-empty">Brak wyników w bazie uczestników.</p>
      ) : null}

      {showCreateForm ? (
        <div className="participant-card participant-card-new">
          <div className="participant-fields">
            <label className="form-field">
              <span>Imię</span>
              <input value={draft.firstName} onChange={(event) => setDraftField("firstName", event.target.value)} />
            </label>
            <label className="form-field">
              <span>Nazwisko</span>
              <input value={draft.lastName} onChange={(event) => setDraftField("lastName", event.target.value)} />
            </label>
            <label className="form-field">
              <span>Telefon</span>
              <input value={draft.phone} onChange={(event) => setDraftField("phone", event.target.value)} />
            </label>
            <label className="form-field">
              <span>E-mail</span>
              <input value={draft.email} onChange={(event) => setDraftField("email", event.target.value)} />
            </label>
          </div>
          <div className="participant-row-actions">
            <Button onClick={submitCreate} disabled={disabled || creating}>
              {creating ? "Dodawanie..." : "Dodaj i przypisz"}
            </Button>
            <Button variant="ghost" onClick={() => setShowCreateForm(false)} disabled={creating}>
              Anuluj
            </Button>
          </div>
        </div>
      ) : (
        <Button variant="secondary" onClick={() => setShowCreateForm(true)} disabled={disabled}>
          <UserPlus className="button-icon" aria-hidden="true" />
          Nowa osoba
        </Button>
      )}
    </div>
  );
}

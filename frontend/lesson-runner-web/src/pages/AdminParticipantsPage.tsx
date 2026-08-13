import { useState } from "react";
import { Archive, ArchiveRestore, Download, Save, ShieldOff, Trash2, UserPlus, UsersRound, X } from "lucide-react";
import { Button } from "../components/ui/Button";
import { useDialogs } from "../features/dialog/DialogContext";
import { useToast } from "../features/toast/ToastContext";
import { useParticipantsViewModel } from "../features/participants/useParticipantsViewModel";
import type { GroupSummary } from "../types/group";
import { computeAge, draftFromParticipant, draftToUpdateRequest } from "../types/participant";
import type {
  GuardianAccountResult,
  ParticipantDetails,
  ParticipantDraft,
  ParticipantSummary,
  UpdateParticipantRequest,
} from "../types/participant";

function downloadParticipantData(details: ParticipantDetails) {
  const json = JSON.stringify(details, null, 2);
  const blob = new Blob([json], { type: "application/json" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = `uczestnik-${details.lastName}-${details.firstName}.json`.toLowerCase().replace(/\s+/g, "-");
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
}

function getInitials(firstName: string, lastName: string): string {
  const initials = `${firstName.trim()[0] ?? ""}${lastName.trim()[0] ?? ""}`.toUpperCase();
  return initials || "?";
}

function ageLabel(birthDate: string | null): string | null {
  const age = computeAge(birthDate);
  return age === null ? null : `${age} lat`;
}

type ChildFieldsProps = {
  draft: ParticipantDraft;
  disabled: boolean;
  onField: (field: keyof ParticipantDraft, value: string | boolean) => void;
};

/** Wspólny zestaw pól dziecka + opiekuna, używany przy dodawaniu i edycji. */
function ChildFields({ draft, disabled, onField }: ChildFieldsProps) {
  return (
    <>
      <div className="participant-fields">
        <label className="form-field">
          <span>Imię</span>
          <input value={draft.firstName} disabled={disabled} onChange={(event) => onField("firstName", event.target.value)} />
        </label>
        <label className="form-field">
          <span>Nazwisko</span>
          <input value={draft.lastName} disabled={disabled} onChange={(event) => onField("lastName", event.target.value)} />
        </label>
        <label className="form-field">
          <span>Data urodzenia</span>
          <input type="date" value={draft.birthDate} disabled={disabled} onChange={(event) => onField("birthDate", event.target.value)} />
        </label>
      </div>

      <div className="participant-fields">
        <label className="form-field">
          <span>Opiekun (imię i nazwisko)</span>
          <input value={draft.guardianName} disabled={disabled} onChange={(event) => onField("guardianName", event.target.value)} />
        </label>
        <label className="form-field">
          <span>Pokrewieństwo</span>
          <input
            value={draft.guardianRelation}
            disabled={disabled}
            placeholder="np. mama, tata, opiekun"
            onChange={(event) => onField("guardianRelation", event.target.value)}
          />
        </label>
        <label className="form-field">
          <span>Telefon opiekuna</span>
          <input value={draft.guardianPhone} disabled={disabled} onChange={(event) => onField("guardianPhone", event.target.value)} />
        </label>
        <label className="form-field">
          <span>E-mail opiekuna</span>
          <input value={draft.guardianEmail} disabled={disabled} onChange={(event) => onField("guardianEmail", event.target.value)} />
        </label>
      </div>

      <div className="participant-fields">
        <label className="form-field">
          <span>Telefon dziecka</span>
          <input value={draft.phone} disabled={disabled} onChange={(event) => onField("phone", event.target.value)} />
        </label>
        <label className="form-field">
          <span>E-mail dziecka</span>
          <input value={draft.email} disabled={disabled} onChange={(event) => onField("email", event.target.value)} />
        </label>
      </div>

      <label className="form-field">
        <span>Notatki (potrzeby specjalne, alergie, uwagi)</span>
        <textarea rows={2} value={draft.notes} disabled={disabled} onChange={(event) => onField("notes", event.target.value)} />
      </label>

      <fieldset className="consent-group">
        <legend>Zgody RODO</legend>
        <label className="checkbox-field">
          <input
            type="checkbox"
            checked={draft.consentDataProcessing}
            disabled={disabled}
            onChange={(event) => onField("consentDataProcessing", event.target.checked)}
          />
          <span>Zgoda na przetwarzanie danych osobowych dziecka</span>
        </label>
        <label className="checkbox-field">
          <input
            type="checkbox"
            checked={draft.consentImage}
            disabled={disabled}
            onChange={(event) => onField("consentImage", event.target.checked)}
          />
          <span>Zgoda na wykorzystanie wizerunku (zdjęcia z zajęć)</span>
        </label>
      </fieldset>
    </>
  );
}

type ParticipantRowProps = {
  participant: ParticipantSummary;
  groups: GroupSummary[];
  busy: boolean;
  onSave: (id: string, request: UpdateParticipantRequest) => Promise<boolean>;
  onArchive: (id: string) => Promise<boolean>;
  onRestore: (id: string) => Promise<boolean>;
  onAnonymize: (id: string) => Promise<boolean>;
  onExport: (id: string) => Promise<void>;
  onDelete: (id: string) => Promise<boolean>;
  onAssign: (id: string, groupId: string) => Promise<boolean>;
  onRemove: (id: string, groupId: string) => Promise<boolean>;
  onCreateGuardianAccount: (id: string) => Promise<GuardianAccountResult | null>;
};

function ParticipantRow({
  participant,
  groups,
  busy,
  onSave,
  onArchive,
  onRestore,
  onAnonymize,
  onExport,
  onDelete,
  onAssign,
  onRemove,
  onCreateGuardianAccount,
}: ParticipantRowProps) {
  const [editing, setEditing] = useState(false);
  const [draft, setDraft] = useState<ParticipantDraft>(() => draftFromParticipant(participant));
  const [assignGroupId, setAssignGroupId] = useState("");
  const { confirm } = useDialogs();
  const toast = useToast();

  function setDraftField(field: keyof ParticipantDraft, value: string | boolean) {
    setDraft((current) => ({ ...current, [field]: value }));
  }

  /**
   * Anonimizacja jest nieodwracalna, więc wymaga przepisania nazwiska dziecka.
   *
   * Wcześniej chronił ją ten sam `window.confirm`, co usunięcie konspektu — a odruch
   * „OK” jest silniejszy niż czytanie. Przepisanie nazwiska wymusza spojrzenie na to,
   * czyich danych dotyczy operacja.
   */
  async function handleAnonymize() {
    const fullName = `${participant.firstName} ${participant.lastName}`;

    const confirmed = await confirm({
      title: "Zanonimizować dane dziecka?",
      description: "Operacja jest nieodwracalna. Wykonuje się ją na żądanie opiekuna (RODO, art. 17).",
      tone: "danger",
      confirmLabel: "Zanonimizuj",
      typeToConfirm: fullName,
      consequences: [
        "Znikną: imię, nazwisko, data urodzenia oraz dane kontaktowe dziecka i opiekuna.",
        "Zostaną: historia obecności, frekwencja i rozliczenia — bez nich nie da się obronić reklamacji.",
        "Danych osobowych nie da się odzyskać z kopii zapasowej po jej rotacji.",
      ],
    });

    if (confirmed) {
      await onAnonymize(participant.id);
      toast.success("Dane zostały zanonimizowane.");
    }
  }

  async function handleDelete() {
    const confirmed = await confirm({
      title: `Usunąć ${participant.firstName} ${participant.lastName}?`,
      description: "Rekord zniknie z bazy uczestników razem z powiązaniami do grup.",
      tone: "danger",
      confirmLabel: "Usuń uczestnika",
      consequences: [
        participant.groups.length > 0
          ? `Dziecko zostanie wypisane z ${participant.groups.length} ${participant.groups.length === 1 ? "grupy" : "grup"}.`
          : "Dziecko nie jest przypisane do żadnej grupy.",
        "Jeśli chodzi o żądanie usunięcia danych (RODO), właściwym działaniem jest anonimizacja — zachowuje historię zajęć.",
      ],
    });

    if (confirmed) {
      await onDelete(participant.id);
    }
  }

  /**
   * Konto opiekuna z danych, które już są przy dziecku.
   *
   * Pytamy o potwierdzenie, bo to jedyna operacja na tym ekranie, która wysyła wiadomość
   * na zewnątrz - i od razu otwiera komuś dostęp do danych dziecka.
   */
  async function handleGuardianAccount() {
    const confirmed = await confirm({
      title: `Założyć konto opiekunowi ${participant.firstName}?`,
      description: `Zaproszenie pójdzie na adres ${participant.guardianEmail}.`,
      confirmLabel: "Załóż i wyślij zaproszenie",
      consequences: [
        "Opiekun ustawi hasło sam, z jednorazowego linku — nikt nie zna cudzego hasła.",
        `W portalu zobaczy plan zajęć, postępy, materiały i rozliczenia dziecka: ${participant.firstName} ${participant.lastName}.`,
        "Konspektów, danych innych dzieci ani panelu administracyjnego nie zobaczy.",
      ],
    });

    if (!confirmed) {
      return;
    }

    const result = await onCreateGuardianAccount(participant.id);

    if (!result) {
      return;
    }

    if (!result.created) {
      toast.success(`Konto ${result.email} już istniało — dopięliśmy do niego dziecko.`);
      return;
    }

    if (result.invitationSent) {
      toast.success(`Konto założone, zaproszenie poszło na ${result.email}.`);
    } else {
      // Konto i powiązanie są, brakuje tylko maila. Cisza w tym miejscu kończyłaby się
      // czekaniem na rodzica, który niczego nie dostał.
      toast.error(`Konto założone, ale zaproszenie nie wyszło: ${result.error ?? "błąd wysyłki"}. Ponów wysyłkę w panelu użytkowników.`);
    }
  }

  async function submitEdit() {
    const ok = await onSave(participant.id, draftToUpdateRequest(draft));
    if (ok) {
      setEditing(false);
    }
  }

  function cancelEdit() {
    setDraft(draftFromParticipant(participant));
    setEditing(false);
  }

  const availableGroups = groups.filter(
    (group) => !participant.groups.some((assigned) => assigned.groupId === group.id),
  );

  const age = ageLabel(participant.birthDate);
  const guardianLine = [participant.guardianName, participant.guardianPhone].filter(Boolean).join(" · ");
  const childContact = participant.phone || participant.email;

  return (
    <div className={`participant-card${participant.isArchived ? " participant-card-archived" : ""}`}>
      <div className="participant-card-header">
        <div className="participant-avatar" aria-hidden="true">
          {getInitials(participant.firstName, participant.lastName)}
        </div>
        <div>
          <strong>
            {participant.firstName} {participant.lastName}
            {age ? <span className="participant-age"> · {age}</span> : null}
            {participant.isArchived ? <span className="chip chip-muted">zarchiwizowany</span> : null}
            {!participant.hasDataConsent ? <span className="chip chip-warn">brak zgody RODO</span> : null}
            {participant.hasImageConsent ? <span className="chip chip-ok">wizerunek ✓</span> : null}
            {participant.hasGuardianAccount ? <span className="chip chip-ok">opiekun ma konto</span> : null}
          </strong>
          <span>{guardianLine ? `Opiekun: ${guardianLine}` : childContact || "Brak danych kontaktowych"}</span>
        </div>
      </div>

      {editing ? (
        <>
          <ChildFields draft={draft} disabled={busy} onField={setDraftField} />
          <div className="participant-row-actions">
            <Button onClick={submitEdit} disabled={busy}>
              <Save className="button-icon" aria-hidden="true" />
              Zapisz
            </Button>
            <Button variant="ghost" onClick={cancelEdit} disabled={busy}>
              Anuluj
            </Button>
          </div>
        </>
      ) : (
        <>
          <div className="participant-groups">
            {participant.groups.length === 0 ? (
              <span className="cue-empty">Brak przypisanych grup.</span>
            ) : (
              participant.groups.map((group) => (
                <span key={group.groupId} className="chip">
                  {group.groupName}
                  <button
                    type="button"
                    aria-label={`Wypisz z grupy ${group.groupName}`}
                    disabled={busy}
                    onClick={() => onRemove(participant.id, group.groupId)}
                  >
                    <X size={13} aria-hidden="true" />
                  </button>
                </span>
              ))
            )}
          </div>

          <div className="participant-assign">
            <select
              value={assignGroupId}
              onChange={(event) => setAssignGroupId(event.target.value)}
              disabled={busy || availableGroups.length === 0}
              aria-label="Przypisz do grupy"
            >
              <option value="">{availableGroups.length === 0 ? "Brak grup do przypisania" : "Przypisz do grupy..."}</option>
              {availableGroups.map((group) => (
                <option key={group.id} value={group.id}>{group.name}</option>
              ))}
            </select>
            <Button
              variant="secondary"
              disabled={busy || !assignGroupId}
              onClick={async () => {
                const ok = await onAssign(participant.id, assignGroupId);
                if (ok) {
                  setAssignGroupId("");
                }
              }}
            >
              <UsersRound className="button-icon" aria-hidden="true" />
              Przypisz
            </Button>
          </div>

          <div className="participant-row-actions">
            <Button variant="secondary" onClick={() => setEditing(true)} disabled={busy}>
              Edytuj dane
            </Button>
            {/* Dane opiekuna są tuż obok, więc zakładanie mu dostępu nie ma powodu
                prowadzić przez przepisywanie adresu do panelu użytkowników. */}
            {participant.hasGuardianAccount ? null : (
              <Button
                variant="secondary"
                onClick={() => void handleGuardianAccount()}
                disabled={busy || !participant.guardianEmail}
                title={
                  participant.guardianEmail
                    ? undefined
                    : "Najpierw uzupełnij e-mail opiekuna w danych dziecka."
                }
              >
                <UserPlus className="button-icon" aria-hidden="true" />
                Załóż konto opiekunowi
              </Button>
            )}
            {participant.isArchived ? (
              <Button variant="ghost" onClick={() => onRestore(participant.id)} disabled={busy}>
                <ArchiveRestore className="button-icon" aria-hidden="true" />
                Przywróć
              </Button>
            ) : (
              <Button variant="ghost" onClick={() => onArchive(participant.id)} disabled={busy}>
                <Archive className="button-icon" aria-hidden="true" />
                Archiwizuj
              </Button>
            )}
            <Button variant="ghost" onClick={() => onExport(participant.id)} disabled={busy}>
              <Download className="button-icon" aria-hidden="true" />
              Eksportuj (RODO)
            </Button>
            <Button
              variant="ghost"
              onClick={() => void handleAnonymize()}
              disabled={busy}
            >
              <ShieldOff className="button-icon" aria-hidden="true" />
              Anonimizuj
            </Button>
            <Button variant="ghost" onClick={() => void handleDelete()} disabled={busy}>
              <Trash2 className="button-icon" aria-hidden="true" />
              Usuń
            </Button>
          </div>
        </>
      )}
    </div>
  );
}

export function AdminParticipantsPage() {
  const vm = useParticipantsViewModel();

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Administrator</span>
          <h1>Uczestnicy</h1>
          <p>Centralna baza dzieci - dane opiekuna, wiek i notatki. Dodawaj osoby niezależnie od grup, a potem przypisuj je do wielu grup.</p>
        </div>
      </div>

      {vm.error ? <div className="list-state list-state-error">{vm.error}</div> : null}

      <div className="editor-fieldset" style={{ marginBottom: 20 }}>
        <legend>Nowy uczestnik</legend>
        <ChildFields draft={vm.createDraft} disabled={vm.busy} onField={vm.setCreateField} />
        <div className="participant-row-actions">
          <Button onClick={vm.createParticipant} disabled={vm.busy}>Dodaj uczestnika</Button>
        </div>
      </div>

      <div className="participant-toolbar">
        <label className="form-field" style={{ flex: 1 }}>
          <span>Szukaj</span>
          <input
            value={vm.query}
            onChange={(event) => vm.setQuery(event.target.value)}
            placeholder="Dziecko, opiekun, telefon lub e-mail..."
          />
        </label>
        <label className="checkbox-field">
          <input
            type="checkbox"
            checked={vm.includeArchived}
            onChange={(event) => vm.setIncludeArchived(event.target.checked)}
          />
          <span>Pokaż zarchiwizowanych</span>
        </label>
      </div>

      {vm.loading ? <div className="list-state">Ładowanie uczestników...</div> : null}

      {!vm.loading && vm.participants.length === 0 ? (
        <p className="cue-empty">Brak uczestników spełniających kryteria wyszukiwania.</p>
      ) : null}

      <div className="participant-list">
        {vm.participants.map((participant) => (
          <ParticipantRow
            key={participant.id}
            participant={participant}
            groups={vm.groups}
            busy={vm.busy}
            onSave={vm.updateParticipant}
            onArchive={vm.archiveParticipant}
            onRestore={vm.restoreParticipant}
            onAnonymize={vm.anonymizeParticipant}
            onExport={async (id) => {
              const details = await vm.fetchParticipantForExport(id);
              if (details) {
                downloadParticipantData(details);
              }
            }}
            onDelete={vm.deleteParticipant}
            onAssign={vm.assignToGroup}
            onRemove={vm.removeFromGroup}
            onCreateGuardianAccount={vm.createGuardianAccount}
          />
        ))}
      </div>
    </section>
  );
}

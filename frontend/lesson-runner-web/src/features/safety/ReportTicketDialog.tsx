import { useState } from "react";
import { History } from "lucide-react";
import { Button } from "../../components/ui/Button";
import { Dialog } from "../../components/ui/Dialog";
import { formatDayMonth } from "../groups/datetime";
import type { SafetyOption, SupportTicket } from "../../types/safety";

type ReportTicketDialogProps = {
  /** Kogo dotyczy problem. Podpis jest w tytule, żeby nie było wątpliwości przy dziesięciorgu dzieci. */
  participantName?: string | null;
  categoryOptions: SafetyOption[];
  busy: boolean;
  /** Wstępnie wybrana kategoria — kokpit podpowiada tę, którą sugeruje znacznik pracy. */
  defaultCategory?: string;
  /** Wcześniejsze problemy tego dziecka. `null` = jeszcze się ładują. */
  history?: SupportTicket[] | null;
  loadingHistory?: boolean;
  onClose: () => void;
  onSubmit: (category: string, description: string, costLessonTime: boolean) => Promise<void>;
};

/** Ile wcześniejszych zgłoszeń pokazujemy. Więcej nie zmieści się w oknie otwieranym w biegu. */
const HISTORY_LIMIT = 3;

/**
 * Zgłoszenie problemu technicznego.
 *
 * Otwierane głównie **z kokpitu, w trakcie zajęć** — a to znaczy, że instruktor ma na nie
 * kilkanaście sekund, przy dzieciach. Stąd trzy pola i nic więcej: kategoria z listy, jedno
 * zdanie opisu i jeden przełącznik. Wszystko pozostałe (kto zgłosił, którego terminu dotyczy,
 * kiedy) dokłada system, bo zna odpowiedzi.
 */
export function ReportTicketDialog({
  participantName,
  categoryOptions,
  busy,
  defaultCategory,
  history,
  loadingHistory = false,
  onClose,
  onSubmit,
}: ReportTicketDialogProps) {
  const [category, setCategory] = useState(defaultCategory ?? categoryOptions[0]?.value ?? "other");
  const [description, setDescription] = useState("");
  const [costLessonTime, setCostLessonTime] = useState(false);
  const earlier = (history ?? []).slice(0, HISTORY_LIMIT);

  return (
    <Dialog
      title={participantName ? `Problem techniczny: ${participantName}` : "Zgłoś problem techniczny"}
      description="Zapis trafia do historii dziecka. Przy powtórce tego samego problemu nie zaczniesz od zera."
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button
            disabled={busy || description.trim().length === 0}
            onClick={() => void onSubmit(category, description.trim(), costLessonTime)}
          >
            {busy ? "Zapisywanie…" : "Zgłoś"}
          </Button>
        </>
      }
    >
      {/* Historia idzie **nad** formularzem, nie pod nim. Jeżeli ten sam problem był już
          rozwiązany miesiąc temu, instruktor ma to zobaczyć zanim zacznie opisywać go od nowa. */}
      {loadingHistory ? <p className="cue-empty">Sprawdzam wcześniejsze zgłoszenia…</p> : null}

      {earlier.length > 0 ? (
        <div className="support-history">
          <h3>
            <History size={16} aria-hidden="true" /> Było już wcześniej
          </h3>
          <ul>
            {earlier.map((ticket) => (
              <li key={ticket.id}>
                <span className="support-history-head">
                  {formatDayMonth(ticket.createdAt)} — {ticket.categoryLabel} ({ticket.statusLabel})
                </span>
                <span className="support-history-desc">{ticket.description}</span>
                {ticket.resolution ? (
                  <span className="support-history-fix">
                    <b>Pomogło:</b> {ticket.resolution}
                  </span>
                ) : null}
              </li>
            ))}
          </ul>
        </div>
      ) : null}

      <label className="form-field">
        <span>Czego dotyczy</span>
        <select value={category} onChange={(event) => setCategory(event.target.value)}>
          {categoryOptions.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </label>

      <label className="form-field">
        <span>Co się dzieje</span>
        <textarea
          rows={3}
          value={description}
          maxLength={4000}
          placeholder="np. Roblox nie instaluje się — blokuje kontrola rodzicielska"
          onChange={(event) => setDescription(event.target.value)}
        />
      </label>

      {/* Osobne pole, bo od niego zależy rozliczenie: rozdział 7 dokumentu traktuje awarię
          po stronie uczestnika i po stronie organizatora jako dwa różne przypadki. */}
      <label className="checkbox-field">
        <input
          type="checkbox"
          checked={costLessonTime}
          onChange={() => setCostLessonTime((current) => !current)}
        />
        <span>Problem kosztował dziecko część zajęć</span>
      </label>
    </Dialog>
  );
}

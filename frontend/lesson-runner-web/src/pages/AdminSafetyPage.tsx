import { useState } from "react";
import { CheckCircle2, Plus, ShieldAlert, Wrench } from "lucide-react";
import { Button } from "../components/ui/Button";
import { Dialog } from "../components/ui/Dialog";
import { EmptyState } from "../components/ui/EmptyState";
import { SkeletonList } from "../components/ui/Skeleton";
import { StatusBadge } from "../components/ui/StatusBadge";
import { Tabs } from "../components/ui/Tabs";
import { formatDateTime, formatFriendlyDateTime } from "../features/groups/datetime";
import { ReportIncidentDialog } from "../features/safety/ReportIncidentDialog";
import { ReportTicketDialog } from "../features/safety/ReportTicketDialog";
import { useSafetyViewModel } from "../features/safety/useSafetyViewModel";
import type { Incident, SupportTicket } from "../types/safety";

type SafetyTab = "incydenty" | "zgloszenia";

/**
 * Rejestr incydentów i zgłoszeń technicznych — rozdziały 3 i 8 dokumentu koncepcyjnego.
 *
 * Dwa rejestry na jednym ekranie, ale w osobnych zakładkach i z osobnym tonem. Incydent
 * dotyczy bezpieczeństwa dziecka, zgłoszenie techniczne — zepsutego mikrofonu. Wspólna lista
 * spłaszczyłaby tę różnicę, a to ona decyduje o tym, na co administracja patrzy najpierw.
 */
export function AdminSafetyPage() {
  const vm = useSafetyViewModel();
  const [tab, setTab] = useState<SafetyTab>("incydenty");
  const [reporting, setReporting] = useState<"incident" | "ticket" | null>(null);
  const [openIncident, setOpenIncident] = useState<Incident | null>(null);
  const [openTicket, setOpenTicket] = useState<SupportTicket | null>(null);

  if (vm.loading) {
    return (
      <section className="page-section">
        <SkeletonList rows={3} label="Ładowanie rejestru" />
      </section>
    );
  }

  const incidents = vm.incidents?.incidents ?? [];
  const tickets = vm.tickets?.tickets ?? [];
  const urgent = incidents.filter((incident) => incident.needsImmediateAttention);
  const openTickets = tickets.filter((ticket) => ticket.status === "open" || ticket.status === "inprogress");

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Bezpieczeństwo i wsparcie</span>
          <h1>Rejestr zdarzeń</h1>
          <p>Incydenty dotyczące bezpieczeństwa i zachowania oraz problemy techniczne uczestników.</p>
        </div>
        <div className="page-header-actions">
          <Button variant="secondary" onClick={() => setReporting("ticket")}>
            <Wrench className="button-icon" aria-hidden="true" />
            Zgłoś problem techniczny
          </Button>
          <Button variant="danger" onClick={() => setReporting("incident")}>
            <Plus className="button-icon" aria-hidden="true" />
            Zgłoś incydent
          </Button>
        </div>
      </div>

      {vm.error ? (
        <div className="list-state list-state-error" role="alert">
          {vm.error}
        </div>
      ) : null}

      {/* Sprawy pilne nad zakładkami — zgłoszenie o dobru dziecka nie może czekać
          na to, aż ktoś kliknie właściwą zakładkę. */}
      {urgent.length > 0 ? (
        <div className="attention-panel">
          <div className="attention-head">
            <h2>Wymaga reakcji dziś</h2>
            <span className="cue-empty">{urgent.length}</span>
          </div>
          <ul className="attention-list">
            {urgent.map((incident) => (
              <li key={incident.id} className="attention-item attention-danger">
                <ShieldAlert size={17} aria-hidden="true" />
                <div>
                  <strong>{incident.kindLabel}</strong>
                  <span>
                    {formatFriendlyDateTime(incident.occurredAt)}
                    {incident.groupName ? ` · ${incident.groupName}` : ""} · zgłosił {incident.reportedByName}
                  </span>
                </div>
                <Button variant="secondary" onClick={() => setOpenIncident(incident)}>
                  Otwórz
                </Button>
              </li>
            ))}
          </ul>
        </div>
      ) : null}

      <Tabs
        ariaLabel="Rodzaj rejestru"
        value={tab}
        onChange={(next) => setTab(next as SafetyTab)}
        tabs={[
          { value: "incydenty", label: "Incydenty", count: incidents.length },
          { value: "zgloszenia", label: "Problemy techniczne", count: openTickets.length },
        ]}
      />

      {tab === "incydenty" ? (
        incidents.length === 0 ? (
          <EmptyState
            icon={CheckCircle2}
            title="Brak zgłoszonych incydentów"
            description="To dobra wiadomość. Gdy coś się wydarzy, zgłoś to tutaj — rejestr jest osobny od dziennika zajęć i rodzic go nie widzi."
          />
        ) : (
          <div className="safety-list">
            {incidents.map((incident) => (
              <button
                type="button"
                key={incident.id}
                className={`safety-row${incident.needsImmediateAttention ? " is-urgent" : ""}`}
                onClick={() => setOpenIncident(incident)}
              >
                <span className="safety-row-main">
                  <strong>{incident.kindLabel}</strong>
                  <small>
                    {formatFriendlyDateTime(incident.occurredAt)}
                    {incident.groupName ? ` · ${incident.groupName}` : ""}
                    {incident.participants.length > 0
                      ? ` · ${incident.participants.map((child) => child.name).join(", ")}`
                      : ""}
                  </small>
                  <small className="safety-row-desc">{incident.description}</small>
                </span>
                <StatusBadge
                  label={incident.severityLabel}
                  tone={incident.severity === "high" ? "danger" : incident.severity === "low" ? "neutral" : "warning"}
                />
                <StatusBadge
                  label={incident.statusLabel}
                  tone={incident.status === "resolved" ? "success" : incident.status === "dismissed" ? "neutral" : "info"}
                  dot
                />
              </button>
            ))}
          </div>
        )
      ) : null}

      {tab === "zgloszenia" ? (
        tickets.length === 0 ? (
          <EmptyState
            icon={Wrench}
            title="Brak zgłoszeń technicznych"
            description="Zgłoszenia zapisane w trakcie zajęć trafią tutaj razem z historią dziecka."
          />
        ) : (
          <div className="safety-list">
            {tickets.map((ticket) => (
              <button
                type="button"
                key={ticket.id}
                className="safety-row"
                onClick={() => setOpenTicket(ticket)}
              >
                <span className="safety-row-main">
                  <strong>
                    {ticket.categoryLabel}
                    {ticket.participantName ? ` — ${ticket.participantName}` : ""}
                  </strong>
                  <small>
                    {formatFriendlyDateTime(ticket.createdAt)}
                    {ticket.groupName ? ` · ${ticket.groupName}` : ""} · zgłosił {ticket.reportedByName}
                  </small>
                  <small className="safety-row-desc">{ticket.description}</small>
                </span>
                {ticket.costLessonTime ? <StatusBadge label="kosztował zajęcia" tone="warning" /> : null}
                <StatusBadge
                  label={ticket.statusLabel}
                  tone={ticket.status === "resolved" ? "success" : ticket.status === "closed" ? "neutral" : "info"}
                  dot
                />
              </button>
            ))}
          </div>
        )
      ) : null}

      {reporting === "incident" && vm.incidents ? (
        <ReportIncidentDialog
          kindOptions={vm.incidents.kindOptions}
          severityOptions={vm.incidents.severityOptions}
          busy={vm.busy}
          onClose={() => setReporting(null)}
          onSubmit={async (payload) => {
            await vm.createIncident({
              kind: payload.kind,
              severity: payload.severity,
              description: payload.description,
              participantIds: payload.participantIds,
            });
            setReporting(null);
          }}
        />
      ) : null}

      {reporting === "ticket" && vm.tickets ? (
        <ReportTicketDialog
          categoryOptions={vm.tickets.categoryOptions}
          busy={vm.busy}
          onClose={() => setReporting(null)}
          onSubmit={async (category, description, costLessonTime) => {
            await vm.createTicket({ category, description, costLessonTime });
            setReporting(null);
          }}
        />
      ) : null}

      {openIncident ? (
        <IncidentDialog
          incident={openIncident}
          statusOptions={vm.incidents?.statusOptions ?? []}
          severityOptions={vm.incidents?.severityOptions ?? []}
          busy={vm.busy}
          onClose={() => setOpenIncident(null)}
          onSave={async (request) => {
            await vm.saveIncident(openIncident.id, request);
            setOpenIncident(null);
          }}
        />
      ) : null}

      {openTicket ? (
        <TicketDialog
          ticket={openTicket}
          statusOptions={vm.tickets?.statusOptions ?? []}
          busy={vm.busy}
          onClose={() => setOpenTicket(null)}
          onSave={async (request) => {
            await vm.saveTicket(openTicket.id, request);
            setOpenTicket(null);
          }}
        />
      ) : null}
    </section>
  );
}

/* -------------------------------------------------------------------------- */

/**
 * Prowadzenie sprawy.
 *
 * Zamknięcie wymaga opisu rozwiązania — pilnuje tego backend, ale okno mówi o tym wprost,
 * zamiast pozwolić użytkownikowi trafić na błąd po kliknięciu zapisu.
 */
function IncidentDialog({
  incident,
  statusOptions,
  severityOptions,
  busy,
  onClose,
  onSave,
}: {
  incident: Incident;
  statusOptions: { value: string; label: string }[];
  severityOptions: { value: string; label: string }[];
  busy: boolean;
  onClose: () => void;
  onSave: (request: {
    status: string;
    severity: string;
    actionsTaken: string | null;
    resolution: string | null;
  }) => Promise<void>;
}) {
  const [status, setStatus] = useState(incident.status);
  const [severity, setSeverity] = useState(incident.severity);
  const [actionsTaken, setActionsTaken] = useState(incident.actionsTaken ?? "");
  const [resolution, setResolution] = useState(incident.resolution ?? "");

  const closing = status === "resolved" || status === "dismissed";
  const blocked = closing && resolution.trim().length === 0;

  return (
    <Dialog
      title={incident.kindLabel}
      description={`${formatDateTime(incident.occurredAt)} · zgłosił ${incident.reportedByName}`}
      wide
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Zamknij
          </Button>
          <Button
            disabled={busy || blocked}
            onClick={() =>
              void onSave({
                status,
                severity,
                actionsTaken: actionsTaken.trim() || null,
                resolution: resolution.trim() || null,
              })
            }
          >
            {busy ? "Zapisywanie..." : "Zapisz"}
          </Button>
        </>
      }
    >
      <div className="dialog-consequences">
        <ul>
          <li>
            <span>{incident.description}</span>
          </li>
          {incident.participants.length > 0 ? (
            <li>
              <span>Dotyczy: {incident.participants.map((child) => child.name).join(", ")}</span>
            </li>
          ) : null}
          {incident.groupName ? (
            <li>
              <span>Grupa: {incident.groupName}</span>
            </li>
          ) : null}
        </ul>
      </div>

      <label className="form-field">
        <span>Status</span>
        <select value={status} onChange={(event) => setStatus(event.target.value)}>
          {statusOptions.map((option) => (
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

      <label className="form-field">
        <span>Podjęte działania</span>
        <textarea
          rows={3}
          value={actionsTaken}
          maxLength={4000}
          placeholder="np. rozmowa z opiekunem 5.08, ustalenie zasad na czacie grupy"
          onChange={(event) => setActionsTaken(event.target.value)}
        />
      </label>

      <label className="form-field">
        <span>
          Jak sprawa się skończyła{closing ? " (wymagane przy zamknięciu)" : ""}
        </span>
        <textarea
          rows={3}
          value={resolution}
          maxLength={4000}
          placeholder="Sprawa ze statusem „rozwiązana” i pustym polem nie jest dowodem na nic."
          onChange={(event) => setResolution(event.target.value)}
        />
      </label>
    </Dialog>
  );
}

function TicketDialog({
  ticket,
  statusOptions,
  busy,
  onClose,
  onSave,
}: {
  ticket: SupportTicket;
  statusOptions: { value: string; label: string }[];
  busy: boolean;
  onClose: () => void;
  onSave: (request: { status: string; resolution: string | null; costLessonTime: boolean }) => Promise<void>;
}) {
  const [status, setStatus] = useState(ticket.status);
  const [resolution, setResolution] = useState(ticket.resolution ?? "");
  const [costLessonTime, setCostLessonTime] = useState(ticket.costLessonTime);

  const blocked = status === "resolved" && resolution.trim().length === 0;

  return (
    <Dialog
      title={`${ticket.categoryLabel}${ticket.participantName ? ` — ${ticket.participantName}` : ""}`}
      description={`${formatDateTime(ticket.createdAt)} · zgłosił ${ticket.reportedByName}`}
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Zamknij
          </Button>
          <Button
            disabled={busy || blocked}
            onClick={() => void onSave({ status, resolution: resolution.trim() || null, costLessonTime })}
          >
            {busy ? "Zapisywanie..." : "Zapisz"}
          </Button>
        </>
      }
    >
      <div className="dialog-consequences">
        <ul>
          <li>
            <span>{ticket.description}</span>
          </li>
        </ul>
      </div>

      <label className="form-field">
        <span>Status</span>
        <select value={status} onChange={(event) => setStatus(event.target.value)}>
          {statusOptions.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </label>

      <label className="form-field">
        <span>Co pomogło{status === "resolved" ? " (wymagane)" : ""}</span>
        <textarea
          rows={3}
          value={resolution}
          maxLength={4000}
          placeholder="To jest cała wartość rejestru — przy powtórce nie zaczynamy od zera."
          onChange={(event) => setResolution(event.target.value)}
        />
      </label>

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

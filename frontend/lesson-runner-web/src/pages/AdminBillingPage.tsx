import { CreditCard, FileText, Plus, RefreshCw } from "lucide-react";
import { useEffect, useState } from "react";
import { ApiError } from "../api/client";
import {
  cancelInvoice,
  createBillingEnrollment,
  createInvoice,
  createPricePlan,
  getBillingOverview,
  issueLessonCredit,
  revokeLessonCredit,
  useLessonCredit,
  payInvoice,
} from "../api/billingApi";
import { getCourses } from "../api/coursesApi";
import { getGroups } from "../api/groupsApi";
import { getParticipants } from "../api/participantsApi";
import { Button } from "../components/ui/Button";
import { formatDateTime } from "../features/groups/datetime";
import { useDialogs } from "../features/dialog/DialogContext";
import { useToast } from "../features/toast/ToastContext";
import type { BillingOverview, Invoice } from "../types/billing";
import type { CourseSummary } from "../types/course";
import type { GroupSummary } from "../types/group";
import type { ParticipantSummary } from "../types/participant";

export function AdminBillingPage() {
  const [overview, setOverview] = useState<BillingOverview | null>(null);
  const [participants, setParticipants] = useState<ParticipantSummary[]>([]);
  const [groups, setGroups] = useState<GroupSummary[]>([]);
  const [courses, setCourses] = useState<CourseSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { confirm, prompt } = useDialogs();
  const toast = useToast();

  const [planName, setPlanName] = useState("");
  const [planAmount, setPlanAmount] = useState("");
  const [planCurrency, setPlanCurrency] = useState("PLN");
  const [planCourseId, setPlanCourseId] = useState("");
  const [planGroupId, setPlanGroupId] = useState("");

  const [participantId, setParticipantId] = useState("");
  const [groupId, setGroupId] = useState("");
  const [pricePlanId, setPricePlanId] = useState("");
  const [trial, setTrial] = useState(false);
  const [trialEndsAt, setTrialEndsAt] = useState("");

  const [invoiceEnrollmentId, setInvoiceEnrollmentId] = useState("");
  const [invoiceDueDate, setInvoiceDueDate] = useState("");
  const [invoiceAmount, setInvoiceAmount] = useState("");

  const [creditParticipantId, setCreditParticipantId] = useState("");
  const [creditGroupId, setCreditGroupId] = useState("");
  const [creditReason, setCreditReason] = useState("");
  const [creditExpiresAt, setCreditExpiresAt] = useState("");

  async function load() {
    try {
      setLoading(true);
      setError(null);
      const [loadedOverview, loadedParticipants, loadedGroups, loadedCourses] = await Promise.all([
        getBillingOverview(),
        getParticipants(undefined, false),
        getGroups(),
        getCourses(),
      ]);
      setOverview(loadedOverview);
      setParticipants(loadedParticipants);
      setGroups(loadedGroups);
      setCourses(loadedCourses);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać płatności.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, []);

  async function savePricePlan() {
    try {
      setBusy(true);
      setError(null);
      await createPricePlan({
        name: planName,
        amountCents: zlotyToCents(planAmount),
        currency: planCurrency,
        courseId: planCourseId || null,
        groupId: planGroupId || null,
        isActive: true,
      });
      setPlanName("");
      setPlanAmount("");
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać cennika.");
    } finally {
      setBusy(false);
    }
  }

  async function saveEnrollment() {
    try {
      setBusy(true);
      setError(null);
      await createBillingEnrollment({
        participantId,
        groupId,
        pricePlanId: pricePlanId || null,
        trial,
        trialEndsAt: trial ? trialEndsAt || null : null,
      });
      setParticipantId("");
      setGroupId("");
      setPricePlanId("");
      setTrial(false);
      setTrialEndsAt("");
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się utworzyć zapisu rozliczeniowego.");
    } finally {
      setBusy(false);
    }
  }

  async function issueInvoice() {
    try {
      setBusy(true);
      setError(null);
      await createInvoice({
        billingEnrollmentId: invoiceEnrollmentId,
        dueDate: invoiceDueDate || null,
        amountCents: invoiceAmount ? zlotyToCents(invoiceAmount) : null,
      });
      setInvoiceEnrollmentId("");
      setInvoiceDueDate("");
      setInvoiceAmount("");
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się wystawić faktury.");
    } finally {
      setBusy(false);
    }
  }

  async function markPaid(invoice: Invoice) {
    try {
      setBusy(true);
      setError(null);
      await payInvoice(invoice.id, {});
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się oznaczyć płatności.");
    } finally {
      setBusy(false);
    }
  }

  async function cancel(invoice: Invoice) {
    const confirmed = await confirm({
      title: `Anulować fakturę ${invoice.number}?`,
      description: "Dokument zostanie oznaczony jako anulowany. Numeracja nie zostanie ponownie użyta.",
      tone: "danger",
      confirmLabel: "Anuluj fakturę",
      consequences: [
        `Kwota ${money(invoice.amountCents, invoice.currency)} przestanie być wymagalna.`,
        "Operacja trafia do dziennika audytu wraz z Twoim kontem.",
      ],
    });

    if (!confirmed) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      await cancelInvoice(invoice.id);
      await load();
      toast.success(`Faktura ${invoice.number} została anulowana.`);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się anulować faktury.");
    } finally {
      setBusy(false);
    }
  }

  async function handleIssueCredit() {
    if (!creditParticipantId || !creditReason.trim()) {
      setError("Wybierz uczestnika i podaj powód przyznania kredytu.");
      return;
    }

    try {
      setBusy(true);
      setError(null);
      await issueLessonCredit({
        participantId: creditParticipantId,
        reason: creditReason.trim(),
        groupId: creditGroupId || null,
        expiresAt: creditExpiresAt || null,
      });
      setCreditReason("");
      setCreditExpiresAt("");
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się przyznać kredytu.");
    } finally {
      setBusy(false);
    }
  }

  async function handleUseCredit(id: string, usage: string) {
    try {
      setBusy(true);
      setError(null);
      await useLessonCredit(id, { usage });
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się oznaczyć kredytu jako wykorzystanego.");
    } finally {
      setBusy(false);
    }
  }

  async function handleRevokeCredit(id: string) {
    // Powód jest wymagany: kredyt wycofany bez uzasadnienia jest nie do obronienia
    // ani w rozmowie z rodzicem, ani przy kontroli. Ta sama zasada obowiązuje
    // przy przyznawaniu kredytu po stronie backendu.
    const reason = await prompt({
      title: "Wycofać kredyt zajęciowy?",
      description: "Kredyt przestanie być widoczny jako możliwy do wykorzystania.",
      label: "Powód wycofania",
      placeholder: "np. przyznany omyłkowo przy podwójnym odwołaniu",
      optional: false,
      confirmLabel: "Wycofaj kredyt",
    });

    if (reason === null) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      await revokeLessonCredit(id, reason);
      await load();
      toast.success("Kredyt został wycofany.");
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się wycofać kredytu.");
    } finally {
      setBusy(false);
    }
  }

  if (loading) {
    return (
      <section className="page-section">
        <div className="list-state">Ładowanie płatności…</div>
      </section>
    );
  }

  if (!overview) {
    return (
      <section className="page-section">
        <div className="list-state list-state-error">{error ?? "Brak danych płatności."}</div>
      </section>
    );
  }

  return (
    <section className="page-section billing-page">
      <div className="page-header">
        <div>
          <span className="eyebrow">Rozliczenia</span>
          <h1>Płatności</h1>
          <p>Cenniki, zapisy rozliczeniowe, faktury i ręczne księgowanie wpłat.</p>
        </div>
        <div className="page-header-actions">
          <Button variant="secondary" onClick={load} disabled={busy}>
            <RefreshCw className="button-icon" aria-hidden="true" />
            Odśwież
          </Button>
        </div>
      </div>

      {error ? <div className="list-state list-state-error">{error}</div> : null}

      <div className="dashboard-kpi-grid">
        <Kpi label="Do zapłaty" value={money(overview.kpis.openAmountCents)} />
        <Kpi label="Zaległe" value={money(overview.kpis.overdueAmountCents)} />
        <Kpi label="Opłacone" value={money(overview.kpis.paidAmountCents)} />
        <Kpi label="Trial" value={overview.kpis.trialEnrollments.toString()} />
        <Kpi label="Aktywne zapisy" value={overview.kpis.activeEnrollments.toString()} />
        <Kpi label="Kredyty do wykorzystania" value={overview.kpis.availableCredits.toString()} />
      </div>

      <div className="notifications-grid">
        <section className="editor-fieldset">
          <legend>Cennik</legend>
          <label className="form-field">
            <span>Nazwa</span>
            <input value={planName} onChange={(event) => setPlanName(event.target.value)} />
          </label>
          <label className="form-field">
            <span>Kwota</span>
            <input type="number" min="0" step="0.01" value={planAmount} onChange={(event) => setPlanAmount(event.target.value)} />
          </label>
          <label className="form-field">
            <span>Waluta</span>
            <input value={planCurrency} onChange={(event) => setPlanCurrency(event.target.value.toUpperCase())} />
          </label>
          <label className="form-field">
            <span>Kurs</span>
            <select value={planCourseId} onChange={(event) => setPlanCourseId(event.target.value)}>
              <option value="">Bez kursu</option>
              {courses.map((course) => <option key={course.id} value={course.id}>{course.name}</option>)}
            </select>
          </label>
          <label className="form-field">
            <span>Grupa</span>
            <select value={planGroupId} onChange={(event) => setPlanGroupId(event.target.value)}>
              <option value="">Bez grupy</option>
              {groups.map((group) => <option key={group.id} value={group.id}>{group.name}</option>)}
            </select>
          </label>
          <Button onClick={savePricePlan} disabled={busy}>
            <Plus className="button-icon" aria-hidden="true" />
            Dodaj cennik
          </Button>
        </section>

        <section className="editor-fieldset">
          <legend>Zapis rozliczeniowy</legend>
          <label className="form-field">
            <span>Uczestnik</span>
            <select value={participantId} onChange={(event) => setParticipantId(event.target.value)}>
              <option value="">Wybierz</option>
              {participants.map((participant) => (
                <option key={participant.id} value={participant.id}>{participant.firstName} {participant.lastName}</option>
              ))}
            </select>
          </label>
          <label className="form-field">
            <span>Grupa</span>
            <select value={groupId} onChange={(event) => setGroupId(event.target.value)}>
              <option value="">Wybierz</option>
              {groups.map((group) => <option key={group.id} value={group.id}>{group.name}</option>)}
            </select>
          </label>
          <label className="form-field">
            <span>Cennik</span>
            <select value={pricePlanId} onChange={(event) => setPricePlanId(event.target.value)}>
              <option value="">Automatycznie</option>
              {overview.pricePlans.map((plan) => <option key={plan.id} value={plan.id}>{plan.name} · {money(plan.amountCents, plan.currency)}</option>)}
            </select>
          </label>
          <label className="toggle-row">
            <input type="checkbox" checked={trial} onChange={(event) => setTrial(event.target.checked)} />
            <span>Trial</span>
          </label>
          <label className="form-field">
            <span>Koniec trial</span>
            <input type="date" value={trialEndsAt} onChange={(event) => setTrialEndsAt(event.target.value)} disabled={!trial} />
          </label>
          <Button onClick={saveEnrollment} disabled={busy}>
            <CreditCard className="button-icon" aria-hidden="true" />
            Utwórz zapis
          </Button>
        </section>

        <section className="editor-fieldset">
          <legend>Faktura</legend>
          <label className="form-field">
            <span>Zapis</span>
            <select value={invoiceEnrollmentId} onChange={(event) => setInvoiceEnrollmentId(event.target.value)}>
              <option value="">Wybierz</option>
              {overview.enrollments.map((enrollment) => (
                <option key={enrollment.id} value={enrollment.id}>{enrollment.participantName} · {enrollment.groupName}</option>
              ))}
            </select>
          </label>
          <label className="form-field">
            <span>Termin płatności</span>
            <input type="date" value={invoiceDueDate} onChange={(event) => setInvoiceDueDate(event.target.value)} />
          </label>
          <label className="form-field">
            <span>Kwota niestandardowa</span>
            <input type="number" min="0" step="0.01" value={invoiceAmount} onChange={(event) => setInvoiceAmount(event.target.value)} />
          </label>
          <Button onClick={issueInvoice} disabled={busy}>
            <FileText className="button-icon" aria-hidden="true" />
            Wystaw fakturę
          </Button>
        </section>
      </div>

      <section className="editor-panel">
        <div className="group-section-head">
          <div>
            <span className="eyebrow">Faktury</span>
            <h2>Rozrachunki</h2>
          </div>
          <span>{overview.invoices.length} pozycji</span>
        </div>
        {overview.invoices.length === 0 ? <p className="cue-empty">Brak faktur.</p> : null}
        {overview.invoices.length > 0 ? (
          <table className="attendance-table">
            <thead>
              <tr>
                <th>Numer</th>
                <th>Uczestnik</th>
                <th>Grupa</th>
                <th>Kwota</th>
                <th>Termin</th>
                <th>Status</th>
                <th>Akcje</th>
              </tr>
            </thead>
            <tbody>
              {overview.invoices.map((invoice) => (
                <tr key={invoice.id}>
                  <td>{invoice.number}</td>
                  <td>{invoice.participantName}</td>
                  <td>{invoice.groupName}</td>
                  <td>{money(invoice.amountCents, invoice.currency)}</td>
                  <td>{invoice.dueDate}</td>
                  <td>{invoice.statusLabel}</td>
                  <td>
                    {invoice.status !== "paid" && invoice.status !== "cancelled" ? (
                      <div className="button-row">
                        <Button variant="secondary" onClick={() => markPaid(invoice)} disabled={busy}>Opłać</Button>
                        <Button variant="ghost" onClick={() => cancel(invoice)} disabled={busy}>Anuluj</Button>
                      </div>
                    ) : null}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : null}
      </section>

      <section className="editor-panel">
        <div className="group-section-head">
          <div>
            <span className="eyebrow">Płatności</span>
            <h2>Wpłaty</h2>
          </div>
          <span>{overview.payments.length} wpisów</span>
        </div>
        {overview.payments.length === 0 ? <p className="cue-empty">Brak wpłat.</p> : null}
        {overview.payments.length > 0 ? (
          <table className="attendance-table">
            <thead>
              <tr>
                <th>Czas</th>
                <th>Provider</th>
                <th>Kwota</th>
                <th>Status</th>
                <th>Id zew.</th>
              </tr>
            </thead>
            <tbody>
              {overview.payments.map((payment) => (
                <tr key={payment.id}>
                  <td>{formatDateTime(payment.createdAt)}</td>
                  <td>{payment.provider}</td>
                  <td>{money(payment.amountCents, payment.currency)}</td>
                  <td>{payment.statusLabel}</td>
                  <td>{payment.externalId ?? "-"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : null}
      </section>

      <section className="editor-panel">
        <div className="group-section-head">
          <div>
            <span className="eyebrow">Rekompensaty</span>
            <h2>Kredyty zajęciowe</h2>
            <p>
              Jeden kredyt = jedne zajęcia. Przyznanie jest decyzją osobną od odwołania terminu —
              to samo odwołanie może skończyć się kredytem, zwrotem, odrobieniem albo niczym.
            </p>
          </div>
        </div>

        <div className="credit-form">
          <label className="form-field">
            <span>Uczestnik</span>
            <select value={creditParticipantId} onChange={(event) => setCreditParticipantId(event.target.value)}>
              <option value="">Wybierz</option>
              {participants.map((participant) => (
                <option key={participant.id} value={participant.id}>
                  {participant.firstName} {participant.lastName}
                </option>
              ))}
            </select>
          </label>
          <label className="form-field">
            <span>Grupa (opcjonalnie)</span>
            <select value={creditGroupId} onChange={(event) => setCreditGroupId(event.target.value)}>
              <option value="">Kredyt ogólny</option>
              {groups.map((group) => (
                <option key={group.id} value={group.id}>
                  {group.name}
                </option>
              ))}
            </select>
          </label>
          <label className="form-field">
            <span>Powód</span>
            <input
              value={creditReason}
              onChange={(event) => setCreditReason(event.target.value)}
              placeholder="np. odwołane zajęcia 12.05"
              maxLength={1000}
            />
          </label>
          <label className="form-field">
            <span>Ważny do (opcjonalnie)</span>
            <input type="date" value={creditExpiresAt} onChange={(event) => setCreditExpiresAt(event.target.value)} />
          </label>
          <Button variant="secondary" disabled={busy} onClick={handleIssueCredit}>
            Przyznaj kredyt
          </Button>
        </div>

        {overview.credits.length === 0 ? <p className="cue-empty">Brak kredytów.</p> : null}
        {overview.credits.length > 0 ? (
          <table className="attendance-table">
            <thead>
              <tr>
                <th>Uczestnik</th>
                <th>Powód</th>
                <th>Grupa</th>
                <th>Ważny do</th>
                <th>Status</th>
                <th>Wykorzystanie</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {overview.credits.map((credit) => (
                <tr key={credit.id}>
                  <td>{credit.participantName}</td>
                  <td>{credit.reason}</td>
                  <td>{credit.groupName ?? "—"}</td>
                  <td>{credit.expiresAt ?? "bezterminowo"}</td>
                  <td>{credit.statusLabel}</td>
                  <td>{credit.usage === "none" ? "—" : credit.usageLabel}</td>
                  <td>
                    {credit.status === "available" ? (
                      <div className="credit-row-actions">
                        <select
                          value=""
                          disabled={busy}
                          onChange={(event) => {
                            if (event.target.value) {
                              void handleUseCredit(credit.id, event.target.value);
                            }
                          }}
                          aria-label={`Wykorzystaj kredyt: ${credit.participantName}`}
                        >
                          <option value="">Wykorzystaj…</option>
                          <option value="makeupsession">Odrabianie zajęć</option>
                          <option value="invoicediscount">Pomniejszenie płatności</option>
                          <option value="extrasession">Zajęcia dodatkowe</option>
                          <option value="courseextension">Przedłużenie kursu</option>
                        </select>
                        <Button variant="ghost" disabled={busy} onClick={() => handleRevokeCredit(credit.id)}>
                          Wycofaj
                        </Button>
                      </div>
                    ) : (
                      <span className="cue-empty">{credit.usageNote ?? ""}</span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : null}
      </section>
    </section>
  );
}

function Kpi({ label, value }: { label: string; value: string }) {
  return (
    <div className="groups-summary-card dashboard-kpi-card">
      <span className="groups-summary-icon" aria-hidden="true">
        <CreditCard size={20} />
      </span>
      <div>
        <strong>{value}</strong>
        <span>{label}</span>
      </div>
    </div>
  );
}

function zlotyToCents(value: string): number {
  return Math.round(Number(value.replace(",", ".")) * 100);
}

function money(amountCents: number, currency = "PLN"): string {
  return new Intl.NumberFormat("pl-PL", { style: "currency", currency }).format(amountCents / 100);
}

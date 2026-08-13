import { Bell, CreditCard, Eye, Mail, RefreshCw, Send } from "lucide-react";
import { useEffect, useState } from "react";
import { ApiError } from "../api/client";
import {
  getNotificationLogs,
  getNotificationSettings,
  previewNotification,
  sendPaymentReminders,
  sendReminderNotifications,
  updateNotificationSettings,
} from "../api/notificationsApi";
import { Button } from "../components/ui/Button";
import { SkeletonList } from "../components/ui/Skeleton";
import { formatDateTime, plural } from "../features/groups/datetime";
import { useDialogs } from "../features/dialog/DialogContext";
import { useToast } from "../features/toast/ToastContext";
import type { NotificationLog, NotificationPreview, NotificationSettings } from "../types/notification";

export function AdminNotificationsPage() {
  const [settings, setSettings] = useState<NotificationSettings | null>(null);
  const [logs, setLogs] = useState<NotificationLog[]>([]);
  const [preview, setPreview] = useState<NotificationPreview | null>(null);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { confirm } = useDialogs();
  const toast = useToast();

  async function load() {
    try {
      setLoading(true);
      setError(null);
      const [loadedSettings, loadedLogs] = await Promise.all([getNotificationSettings(), getNotificationLogs()]);
      setSettings(loadedSettings);
      setLogs(loadedLogs);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać powiadomień.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, []);

  async function save() {
    if (!settings) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      setSettings(await updateNotificationSettings(settings));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać ustawień.");
    } finally {
      setBusy(false);
    }
  }

  async function showPreview(type: "reminder" | "absence") {
    try {
      setBusy(true);
      setError(null);
      setPreview(await previewNotification(type));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać podglądu.");
    } finally {
      setBusy(false);
    }
  }

  async function sendReminders() {
    try {
      setBusy(true);
      setError(null);
      await sendReminderNotifications();
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się uruchomić przypomnień.");
    } finally {
      setBusy(false);
    }
  }

  /**
   * Przypomnienia o zaległych płatnościach.
   *
   * Za każdym razem pytamy o zgodę i pokazujemy, czego dotyczy — to wiadomość, która
   * może zepsuć relację z rodzicem, jeśli wyjdzie omyłkowo albo do kogoś, kto właśnie
   * zapłacił.
   */
  async function handlePaymentReminders() {
    const confirmed = await confirm({
      title: "Wysłać przypomnienia o płatnościach?",
      description: "Wiadomość dostaną opiekunowie dzieci z fakturami po terminie.",
      confirmLabel: "Wyślij przypomnienia",
      consequences: [
        "Zaległość liczymy z daty płatności, a nie ze statusu faktury w bazie.",
        "Każda faktura jest przypominana raz — ponowne kliknięcie nie wyśle drugiego maila.",
        "Wysyłka trafia do dziennika razem z adresem i tematem.",
      ],
    });

    if (!confirmed) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      const { sent } = await sendPaymentReminders();
      toast.success(
        sent === 0
          ? "Brak zaległych płatności — nic nie wysłaliśmy."
          : `Wysłano ${sent} ${plural(sent, "przypomnienie", "przypomnienia", "przypomnień")}.`,
      );
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się wysłać przypomnień o płatnościach.");
    } finally {
      setBusy(false);
    }
  }

  if (loading) {
    return (
      <section className="page-section">
        <SkeletonList rows={2} label="Ładowanie powiadomień" />
      </section>
    );
  }

  if (!settings) {
    return (
      <section className="page-section">
        <div className="list-state list-state-error" role="alert">
          {error ?? "Brak ustawień powiadomień."}
        </div>
      </section>
    );
  }

  return (
    <section className="page-section notifications-page">
      <div className="page-header">
        <div>
          <span className="eyebrow">Komunikacja</span>
          <h1>Powiadomienia</h1>
          <p>Szablony e-maili, przypomnienia i log wysyłek do opiekunów.</p>
        </div>
        <div className="page-header-actions">
          <Button variant="secondary" onClick={load} disabled={busy}>
            <RefreshCw className="button-icon" aria-hidden="true" />
            Odśwież
          </Button>
          <Button variant="secondary" onClick={sendReminders} disabled={busy}>
            <Send className="button-icon" aria-hidden="true" />
            Wyślij przypomnienia
          </Button>
          <Button variant="secondary" onClick={() => void handlePaymentReminders()} disabled={busy}>
            <CreditCard className="button-icon" aria-hidden="true" />
            Przypomnij o płatnościach
          </Button>
          <Button onClick={save} disabled={busy}>
            Zapisz
          </Button>
        </div>
      </div>

      {error ? <div className="list-state list-state-error">{error}</div> : null}

      <div className="notifications-grid">
        <section className="editor-fieldset">
          <legend>Opcje</legend>
          <label className="toggle-row">
            <input
              type="checkbox"
              checked={settings.remindersEnabled}
              onChange={(event) => setSettings({ ...settings, remindersEnabled: event.target.checked })}
            />
            <span>Przypomnienia o zajęciach</span>
          </label>
          <label className="toggle-row">
            <input
              type="checkbox"
              checked={settings.absenceEnabled}
              onChange={(event) => setSettings({ ...settings, absenceEnabled: event.target.checked })}
            />
            <span>Powiadomienia o nieobecności</span>
          </label>
          <label className="form-field">
            <span>Wyprzedzenie przypomnień (h)</span>
            <input
              type="number"
              min={1}
              max={168}
              value={settings.reminderLeadHours}
              onChange={(event) => setSettings({ ...settings, reminderLeadHours: Number(event.target.value) })}
            />
          </label>
          <label className="form-field">
            <span>Nadawca</span>
            <input value={settings.fromName} onChange={(event) => setSettings({ ...settings, fromName: event.target.value })} />
          </label>
          <label className="form-field">
            <span>E-mail nadawcy</span>
            <input value={settings.fromEmail} onChange={(event) => setSettings({ ...settings, fromEmail: event.target.value })} />
          </label>
        </section>

        <section className="editor-fieldset">
          <legend>Podgląd</legend>
          <div className="button-row">
            <Button variant="secondary" onClick={() => showPreview("reminder")} disabled={busy}>
              <Eye className="button-icon" aria-hidden="true" />
              Przypomnienie
            </Button>
            <Button variant="secondary" onClick={() => showPreview("absence")} disabled={busy}>
              <Eye className="button-icon" aria-hidden="true" />
              Nieobecność
            </Button>
          </div>
          {preview ? (
            <div className="preview-box">
              <strong>{preview.subject}</strong>
              <pre>{preview.body}</pre>
            </div>
          ) : (
            <p className="cue-empty">Wybierz typ wiadomości, aby zobaczyć podgląd.</p>
          )}
        </section>
      </div>

      <section className="editor-panel">
        <div className="group-section-head">
          <div>
            <span className="eyebrow">Szablony</span>
            <h2>Treść wiadomości</h2>
          </div>
          <span>{"{{participant}}, {{group}}, {{lesson}}, {{sessionAt}}, {{guardian}}"}</span>
        </div>
        <div className="notifications-grid">
          <div className="template-column">
            <h3>
              <Bell size={17} aria-hidden="true" /> Przypomnienie
            </h3>
            <label className="form-field">
              <span>Temat</span>
              <input
                value={settings.reminderSubject}
                onChange={(event) => setSettings({ ...settings, reminderSubject: event.target.value })}
              />
            </label>
            <label className="form-field">
              <span>Treść</span>
              <textarea
                rows={8}
                value={settings.reminderBody}
                onChange={(event) => setSettings({ ...settings, reminderBody: event.target.value })}
              />
            </label>
          </div>
          <div className="template-column">
            <h3>
              <Mail size={17} aria-hidden="true" /> Nieobecność
            </h3>
            <label className="form-field">
              <span>Temat</span>
              <input
                value={settings.absenceSubject}
                onChange={(event) => setSettings({ ...settings, absenceSubject: event.target.value })}
              />
            </label>
            <label className="form-field">
              <span>Treść</span>
              <textarea
                rows={8}
                value={settings.absenceBody}
                onChange={(event) => setSettings({ ...settings, absenceBody: event.target.value })}
              />
            </label>
          </div>
        </div>
      </section>

      <section className="editor-panel">
        <div className="group-section-head">
          <div>
            <span className="eyebrow">Log</span>
            <h2>Wysyłki</h2>
          </div>
          <span>{logs.length} wpisów</span>
        </div>
        {logs.length === 0 ? <p className="cue-empty">Brak wysyłek.</p> : null}
        {logs.length > 0 ? (
          <table className="attendance-table">
            <thead>
              <tr>
                <th>Czas</th>
                <th>Typ</th>
                <th>Adresat</th>
                <th>Status</th>
                <th>Temat</th>
              </tr>
            </thead>
            <tbody>
              {logs.map((log) => (
                <tr key={log.id}>
                  <td>{formatDateTime(log.createdAt)}</td>
                  <td>{log.type}</td>
                  <td>{log.recipient}</td>
                  <td>{log.status}</td>
                  <td>{log.subject}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : null}
      </section>
    </section>
  );
}

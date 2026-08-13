import { Activity, DatabaseBackup, RefreshCw, ShieldCheck } from "lucide-react";
import { useEffect, useState } from "react";
import { ApiError } from "../api/client";
import { createBackup, getAuditLogs, getBackups, getHealth } from "../api/operationsApi";
import { Button } from "../components/ui/Button";
import { formatDateTime } from "../features/groups/datetime";
import type { AuditLog, BackupFile, HealthResponse } from "../types/operations";

export function AdminOperationsPage() {
  const [health, setHealth] = useState<HealthResponse | null>(null);
  const [auditLogs, setAuditLogs] = useState<AuditLog[]>([]);
  const [backups, setBackups] = useState<BackupFile[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [auditEntityType, setAuditEntityType] = useState("");
  const [auditAction, setAuditAction] = useState("");
  const [auditOnlyFailures, setAuditOnlyFailures] = useState(false);

  async function load() {
    try {
      setLoading(true);
      setError(null);
      const [healthResponse, backupList, auditList] = await Promise.all([
        getHealth(),
        getBackups(),
        getAuditLogs({
          entityType: auditEntityType || undefined,
          action: auditAction || undefined,
          success: auditOnlyFailures ? false : undefined,
        }),
      ]);
      setHealth(healthResponse);
      setBackups(backupList);
      setAuditLogs(auditList);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać danych operacyjnych.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, []);

  async function handleCreateBackup() {
    try {
      setBusy(true);
      setError(null);
      await createBackup();
      await load();
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się utworzyć backupu.");
    } finally {
      setBusy(false);
    }
  }

  if (loading) {
    return (
      <section className="page-section">
        <div className="list-state">Ładowanie danych operacyjnych…</div>
      </section>
    );
  }

  return (
    <section className="page-section operations-page">
      <div className="page-header">
        <div>
          <span className="eyebrow">Eksploatacja</span>
          <h1>Operacje</h1>
          <p>Health-check, audyt i kopie zapasowe systemu.</p>
        </div>
        <div className="page-header-actions">
          <Button variant="secondary" onClick={load} disabled={busy}>
            <RefreshCw className="button-icon" aria-hidden="true" />
            Odśwież
          </Button>
          <Button onClick={handleCreateBackup} disabled={busy}>
            <DatabaseBackup className="button-icon" aria-hidden="true" />
            Utwórz backup
          </Button>
        </div>
      </div>

      {error ? <div className="list-state list-state-error">{error}</div> : null}

      <div className="operations-grid">
        <section className="editor-panel">
          <div className="group-section-head">
            <div>
              <span className="eyebrow">Stan</span>
              <h2>Health-check</h2>
            </div>
            <span className={health?.status === "Healthy" ? "status-pill status-completed" : "status-pill status-cancelled"}>
              {health?.status ?? "Unknown"}
            </span>
          </div>
          <div className="compact-list">
            {health?.checks.map((check) => (
              <div className="compact-row" key={check.name}>
                <Activity size={16} aria-hidden="true" />
                <span>
                  <strong>{check.name}</strong>
                  <small>{check.description || check.status}</small>
                </span>
              </div>
            ))}
          </div>
        </section>

        <section className="editor-panel">
          <div className="group-section-head">
            <div>
              <span className="eyebrow">Backup</span>
              <h2>Kopie zapasowe</h2>
            </div>
            <span>{backups.length} plikow</span>
          </div>
          {backups.length === 0 ? <p className="cue-empty">Nie ma jeszcze kopii zapasowych.</p> : null}
          <div className="compact-list">
            {backups.map((backup) => (
              <div className="compact-row" key={backup.fileName}>
                <DatabaseBackup size={16} aria-hidden="true" />
                <span>
                  <strong>{backup.fileName}</strong>
                  <small>
                    {formatDateTime(backup.createdAt)} · {formatBytes(backup.sizeBytes)}
                  </small>
                </span>
              </div>
            ))}
          </div>
        </section>
      </div>

      <section className="editor-panel">
        <div className="group-section-head">
          <div>
            <span className="eyebrow">Audyt</span>
            <h2>Ostatnie akcje</h2>
            <p>Każda operacja zmieniająca dane zostawia wpis: kto, co, kiedy i z jakim skutkiem.</p>
          </div>
          <span>{auditLogs.length} wpisów</span>
        </div>

        <div className="audit-filters">
          <label className="form-field">
            <span>Obszar</span>
            <select value={auditEntityType} onChange={(event) => setAuditEntityType(event.target.value)}>
              <option value="">Wszystkie</option>
              <option value="groups">Grupy i terminy</option>
              <option value="schedule">Prowadzenie zajęć</option>
              <option value="participants">Uczestnicy</option>
              <option value="users">Konta</option>
              <option value="billing">Płatności</option>
              <option value="lessons">Konspekty</option>
              <option value="courses">Kursy</option>
              <option value="notifications">Powiadomienia</option>
            </select>
          </label>
          <label className="form-field">
            <span>Akcja zawiera</span>
            <input
              value={auditAction}
              onChange={(event) => setAuditAction(event.target.value)}
              placeholder="np. Cancel"
            />
          </label>
          <label className="session-notified">
            <input
              type="checkbox"
              checked={auditOnlyFailures}
              onChange={(event) => setAuditOnlyFailures(event.target.checked)}
            />
            <span>Tylko nieudane</span>
          </label>
          <Button variant="secondary" disabled={busy} onClick={load}>
            Filtruj
          </Button>
        </div>
        {auditLogs.length === 0 ? <p className="cue-empty">Brak wpisów audytu.</p> : null}
        {auditLogs.length > 0 ? (
          <table className="attendance-table operations-table">
            <thead>
              <tr>
                <th>Czas</th>
                <th>Akcja</th>
                <th>Obiekt</th>
                <th>Status</th>
                <th>Szczegóły</th>
              </tr>
            </thead>
            <tbody>
              {auditLogs.map((log) => (
                <tr key={log.id}>
                  <td>{formatDateTime(log.occurredAt)}</td>
                  <td>
                    <ShieldCheck size={14} aria-hidden="true" /> {log.action}
                  </td>
                  <td>
                    {log.entityType}
                    {log.entityId ? `:${log.entityId}` : ""}
                  </td>
                  <td>{log.success ? "OK" : "Błąd"}</td>
                  <td>{log.details || "-"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : null}
      </section>
    </section>
  );
}

function formatBytes(value: number): string {
  if (value < 1024) {
    return `${value} B`;
  }

  if (value < 1024 * 1024) {
    return `${(value / 1024).toFixed(1)} KB`;
  }

  return `${(value / 1024 / 1024).toFixed(1)} MB`;
}

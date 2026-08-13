import { apiGet, apiPostEmpty } from "./client";
import type { AuditLog, BackupFile, BackupResult, HealthResponse } from "../types/operations";

export function getHealth(): Promise<HealthResponse> {
  return apiGet<HealthResponse>("/health");
}

export type AuditFilter = {
  limit?: number;
  /** Obszar, np. `groups`, `users`, `billing`. */
  entityType?: string;
  /** Fragment nazwy akcji, np. `Cancel`. */
  action?: string;
  /** `true` = tylko udane, `false` = tylko nieudane, pominięte = wszystkie. */
  success?: boolean;
};

export function getAuditLogs(filter: AuditFilter = {}): Promise<AuditLog[]> {
  const query = new URLSearchParams({ limit: String(filter.limit ?? 100) });

  if (filter.entityType) {
    query.set("entityType", filter.entityType);
  }

  if (filter.action) {
    query.set("action", filter.action);
  }

  if (filter.success !== undefined) {
    query.set("success", String(filter.success));
  }

  return apiGet<AuditLog[]>(`/api/audit?${query.toString()}`);
}

export function getBackups(): Promise<BackupFile[]> {
  return apiGet<BackupFile[]>("/api/operations/backups");
}

export function createBackup(): Promise<BackupResult> {
  return apiPostEmpty<BackupResult>("/api/operations/backup");
}

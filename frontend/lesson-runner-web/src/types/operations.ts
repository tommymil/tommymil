export type HealthResponse = {
  status: string;
  checks: { name: string; status: string; description: string | null }[];
};

export type BackupResult = {
  fileName: string;
  path: string;
  sizeBytes: number;
  createdAt: string;
};

export type BackupFile = {
  fileName: string;
  sizeBytes: number;
  createdAt: string;
};

export type AuditLog = {
  id: string;
  occurredAt: string;
  actorUserId: string | null;
  action: string;
  entityType: string;
  entityId: string | null;
  success: boolean;
  details: string | null;
};

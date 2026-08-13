/** Słownik do list rozwijanych. Etykiety przychodzą z domeny, front nie trzyma własnej kopii. */
export type SafetyOption = { value: string; label: string };

export type IncidentParticipant = { participantId: string; name: string };

export type Incident = {
  id: string;
  kind: string;
  kindLabel: string;
  severity: string;
  severityLabel: string;
  status: string;
  statusLabel: string;
  occurredAt: string;
  groupId: string | null;
  groupName: string | null;
  sessionId: string | null;
  participants: IncidentParticipant[];
  description: string;
  actionsTaken: string | null;
  resolution: string | null;
  reportedByUserId: string;
  reportedByName: string;
  assignedToUserId: string | null;
  assignedToName: string | null;
  createdAt: string;
  resolvedAt: string | null;
  /** Wysoka waga albo rodzaj z definicji poważny — sprawa na dziś, nie na kiedyś. */
  needsImmediateAttention: boolean;
};

export type IncidentBoard = {
  incidents: Incident[];
  kindOptions: SafetyOption[];
  severityOptions: SafetyOption[];
  statusOptions: SafetyOption[];
};

export type CreateIncidentRequest = {
  kind: string;
  severity: string;
  occurredAt?: string | null;
  groupId?: string | null;
  sessionId?: string | null;
  participantIds?: string[] | null;
  description: string;
};

export type UpdateIncidentRequest = {
  status: string;
  severity?: string | null;
  actionsTaken?: string | null;
  resolution?: string | null;
  assignedToUserId?: string | null;
};

export type SupportTicket = {
  id: string;
  participantId: string | null;
  participantName: string | null;
  sessionId: string | null;
  groupId: string | null;
  groupName: string | null;
  category: string;
  categoryLabel: string;
  status: string;
  statusLabel: string;
  description: string;
  resolution: string | null;
  /** Czy problem kosztował dziecko część zajęć — od tego zależy rozliczenie. */
  costLessonTime: boolean;
  reportedByUserId: string;
  reportedByName: string;
  createdAt: string;
  resolvedAt: string | null;
};

export type SupportBoard = {
  tickets: SupportTicket[];
  categoryOptions: SafetyOption[];
  statusOptions: SafetyOption[];
};

/**
 * Awaryjna lista kategorii zgłoszenia technicznego.
 *
 * Normalnie etykiety przychodzą z backendu razem z rejestrem. Ale okno zgłoszenia otwiera się
 * **w trakcie zajęć**, przy dzieciach — i jeśli akurat to pobranie nie dojdzie, instruktor
 * nie może zostać z pustą listą rozwijaną. Wartości muszą się zgadzać z `SupportCategory`
 * w domenie; przy dopisaniu nowej kategorii uzupełnij też tę listę.
 */
export const SUPPORT_CATEGORY_FALLBACK: SafetyOption[] = [
  { value: "connection", label: "Połączenie" },
  { value: "audiovideo", label: "Dźwięk lub kamera" },
  { value: "device", label: "Sprzęt" },
  { value: "software", label: "Program lub instalacja" },
  { value: "account", label: "Konto lub hasło" },
  { value: "projectfile", label: "Plik projektu" },
  { value: "other", label: "Inne" },
];

/** Jak wyżej, dla rejestru incydentów — musi się zgadzać z `IncidentKind` i `IncidentSeverity`. */
export const INCIDENT_KIND_FALLBACK: SafetyOption[] = [
  { value: "unknownparticipant", label: "Nieznana osoba na spotkaniu" },
  { value: "datadisclosure", label: "Ujawnienie danych osobowych" },
  { value: "inappropriatecontent", label: "Nieodpowiednia treść" },
  { value: "harassment", label: "Nękanie lub wyśmiewanie" },
  { value: "recordingmisuse", label: "Nagranie poza systemem" },
  { value: "childwelfareconcern", label: "Niepokojąca sytuacja dziecka" },
  { value: "staffconduct", label: "Zachowanie osoby prowadzącej" },
  { value: "other", label: "Inne" },
];

export const INCIDENT_SEVERITY_FALLBACK: SafetyOption[] = [
  { value: "low", label: "Niska" },
  { value: "medium", label: "Średnia" },
  { value: "high", label: "Wysoka" },
];

export type CreateSupportTicketRequest = {
  category: string;
  description: string;
  participantId?: string | null;
  sessionId?: string | null;
  groupId?: string | null;
  costLessonTime?: boolean;
};

export type UpdateSupportTicketRequest = {
  status: string;
  category?: string | null;
  resolution?: string | null;
  costLessonTime?: boolean | null;
};

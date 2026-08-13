import { apiDelete, apiDownload, apiGet, apiPost, apiPut } from "./client";
import type { DownloadedFile } from "./client";
import type { ParentParticipantLink, ParentPortal } from "../types/parent";

export function getParentPortal(): Promise<ParentPortal> {
  return apiGet<ParentPortal>("/api/parent/portal");
}

export function getParentLinks(): Promise<ParentParticipantLink[]> {
  return apiGet<ParentParticipantLink[]>("/api/parent-links");
}

export function createParentLink(request: ParentParticipantLink): Promise<ParentParticipantLink> {
  return apiPost<ParentParticipantLink, ParentParticipantLink>("/api/parent-links", request);
}

export function deleteParentLink(parentUserId: string, participantId: string): Promise<void> {
  return apiDelete(`/api/parent-links/${parentUserId}/${participantId}`);
}

/** Zgłoszenie nieobecności dziecka na nadchodzącym terminie. */
export function reportAbsence(sessionId: string, participantId: string, reason: string | null): Promise<void> {
  return apiPost<{ participantId: string; reason: string | null }, void>(
    `/api/parent/sessions/${sessionId}/absence`,
    { participantId, reason },
  );
}

/** Harmonogram dziecka jako plik iCalendar. */
export function exportParentScheduleIcs(): Promise<DownloadedFile> {
  return apiDownload("/api/parent/export.ics");
}

/**
 * Zmiana zgody na wizerunek przez opiekuna.
 *
 * Zgoda na przetwarzanie danych nie ma tu odpowiednika — jest warunkiem świadczenia
 * usługi, więc jej wycofanie to rozmowa z administracją, a nie przełącznik w portalu.
 */
export function updateParentConsent(participantId: string, imageConsent: boolean): Promise<void> {
  return apiPut<{ participantId: string; imageConsent: boolean }, void>("/api/parent/consents", {
    participantId,
    imageConsent,
  });
}

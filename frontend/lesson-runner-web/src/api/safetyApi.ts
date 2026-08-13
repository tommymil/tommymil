import { apiGet, apiPost, apiPut } from "./client";
import type {
  CreateIncidentRequest,
  CreateSupportTicketRequest,
  Incident,
  IncidentBoard,
  SupportBoard,
  SupportTicket,
  UpdateIncidentRequest,
  UpdateSupportTicketRequest,
} from "../types/safety";

/**
 * Rejestr incydentów.
 *
 * Backend sam przycina listę do tego, co wolno zobaczyć: administrator dostaje wszystko,
 * instruktor wyłącznie własne zgłoszenia. Front nie filtruje niczego po swojej stronie —
 * to nie jest miejsce, w którym można sobie na to pozwolić.
 */
export function getIncidents(): Promise<IncidentBoard> {
  return apiGet<IncidentBoard>("/api/safety/incidents");
}

export function reportIncident(request: CreateIncidentRequest): Promise<Incident> {
  return apiPost<CreateIncidentRequest, Incident>("/api/safety/incidents", request);
}

export function updateIncident(id: string, request: UpdateIncidentRequest): Promise<Incident> {
  return apiPut<UpdateIncidentRequest, Incident>(`/api/safety/incidents/${id}`, request);
}

/** Bez `participantId` — wszystkie zgłoszenia. Z nim — historia problemów jednego dziecka. */
export function getSupportTickets(participantId?: string): Promise<SupportBoard> {
  const query = participantId ? `?participantId=${encodeURIComponent(participantId)}` : "";
  return apiGet<SupportBoard>(`/api/safety/tickets${query}`);
}

export function reportSupportTicket(request: CreateSupportTicketRequest): Promise<SupportTicket> {
  return apiPost<CreateSupportTicketRequest, SupportTicket>("/api/safety/tickets", request);
}

export function updateSupportTicket(id: string, request: UpdateSupportTicketRequest): Promise<SupportTicket> {
  return apiPut<UpdateSupportTicketRequest, SupportTicket>(`/api/safety/tickets/${id}`, request);
}

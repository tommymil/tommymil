import { useCallback, useEffect, useState } from "react";
import { ApiError } from "../../api/client";
import {
  getIncidents,
  getSupportTickets,
  reportIncident,
  reportSupportTicket,
  updateIncident,
  updateSupportTicket,
} from "../../api/safetyApi";
import { useToast } from "../toast/ToastContext";
import type {
  CreateIncidentRequest,
  CreateSupportTicketRequest,
  IncidentBoard,
  SupportBoard,
  UpdateIncidentRequest,
  UpdateSupportTicketRequest,
} from "../../types/safety";

/**
 * Model widoku rejestru incydentów i zgłoszeń technicznych.
 *
 * Oba rejestry ładujemy razem, bo panel pokazuje je obok siebie — ale trzymamy w osobnych
 * stanach, żeby błąd jednego nie wygaszał drugiego. Zgłoszenia techniczne są przyziemne
 * i częste; incydenty rzadkie i poważne. Nie ma powodu, żeby awaria listy usterek zabierała
 * administracji dostęp do spraw bezpieczeństwa.
 */
export function useSafetyViewModel() {
  const [incidents, setIncidents] = useState<IncidentBoard | null>(null);
  const [tickets, setTickets] = useState<SupportBoard | null>(null);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const toast = useToast();

  const load = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const [loadedIncidents, loadedTickets] = await Promise.all([getIncidents(), getSupportTickets()]);
      setIncidents(loadedIncidents);
      setTickets(loadedTickets);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać rejestru.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  async function run(action: () => Promise<void>, success: string, failure: string) {
    try {
      setBusy(true);
      setError(null);
      await action();
      await load();
      toast.success(success);
    } catch (caught) {
      const message = caught instanceof ApiError ? caught.message : failure;
      setError(message);
      toast.error(message);
    } finally {
      setBusy(false);
    }
  }

  return {
    incidents,
    tickets,
    loading,
    busy,
    error,
    reload: load,

    createIncident: (request: CreateIncidentRequest) =>
      run(
        async () => void (await reportIncident(request)),
        "Incydent zgłoszony. Administracja go zobaczy.",
        "Nie udało się zgłosić incydentu.",
      ),

    saveIncident: (id: string, request: UpdateIncidentRequest) =>
      run(
        async () => void (await updateIncident(id, request)),
        "Sprawa zaktualizowana.",
        "Nie udało się zapisać sprawy.",
      ),

    createTicket: (request: CreateSupportTicketRequest) =>
      run(
        async () => void (await reportSupportTicket(request)),
        "Zgłoszenie zapisane w historii dziecka.",
        "Nie udało się zapisać zgłoszenia.",
      ),

    saveTicket: (id: string, request: UpdateSupportTicketRequest) =>
      run(
        async () => void (await updateSupportTicket(id, request)),
        "Zgłoszenie zaktualizowane.",
        "Nie udało się zapisać zgłoszenia.",
      ),
  };
}

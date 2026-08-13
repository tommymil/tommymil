import { useCallback, useState } from "react";
import { ApiError } from "../../api/client";
import { getIncidents, getSupportTickets, reportIncident, reportSupportTicket } from "../../api/safetyApi";
import { useToast } from "../toast/ToastContext";
import {
  INCIDENT_KIND_FALLBACK,
  INCIDENT_SEVERITY_FALLBACK,
  SUPPORT_CATEGORY_FALLBACK,
} from "../../types/safety";
import type { SafetyOption, SupportTicket } from "../../types/safety";

/** Dziecko, którego dotyczy zgłoszenie — tyle, ile kokpit ma pod ręką. */
export type SupportTarget = { participantId: string; name: string };

/**
 * Zgłoszenie problemu technicznego prosto z kokpitu.
 *
 * Model celowo **nie ładuje niczego przy wejściu na zajęcia**. Zgłoszenie to zdarzenie
 * rzadkie, a kokpit i tak startuje z kilkoma żądaniami naraz. Historię pobieramy dopiero
 * przy otwarciu okna — i przy okazji dostajemy z tego samego żądania listę kategorii,
 * więc nie ma osobnego zapytania o słownik.
 *
 * Historia jest tu sednem, nie dodatkiem: przy trzecim z rzędu „nie działa mikrofon”
 * u tego samego dziecka instruktor ma pod ręką to, co pomogło poprzednim razem, zamiast
 * przechodzić całą diagnostykę od nowa przy dziesięciorgu patrzących dzieci.
 */
export function useSessionSupportViewModel(sessionId: string | null, groupId: string | null) {
  const [target, setTarget] = useState<SupportTarget | null>(null);
  const [history, setHistory] = useState<SupportTicket[] | null>(null);
  const [categoryOptions, setCategoryOptions] = useState<SafetyOption[]>(SUPPORT_CATEGORY_FALLBACK);
  const [loadingHistory, setLoadingHistory] = useState(false);
  const [busy, setBusy] = useState(false);
  const [incidentOpen, setIncidentOpen] = useState(false);
  const [kindOptions, setKindOptions] = useState<SafetyOption[]>(INCIDENT_KIND_FALLBACK);
  const [severityOptions, setSeverityOptions] = useState<SafetyOption[]>(INCIDENT_SEVERITY_FALLBACK);
  const toast = useToast();

  const open = useCallback(async (participantId: string, name: string) => {
    setTarget({ participantId, name });
    setHistory(null);
    setLoadingHistory(true);

    try {
      const board = await getSupportTickets(participantId);
      setHistory(board.tickets);

      if (board.categoryOptions.length > 0) {
        setCategoryOptions(board.categoryOptions);
      }
    } catch {
      // Świadomie po cichu. Historia jest pomocą, nie warunkiem zgłoszenia — okno działa
      // dalej na awaryjnej liście kategorii, a instruktor nie dostaje w trakcie zajęć
      // komunikatu o błędzie czegoś, o co nie prosił.
      setHistory([]);
    } finally {
      setLoadingHistory(false);
    }
  }, []);

  const close = useCallback(() => {
    setTarget(null);
    setHistory(null);
  }, []);

  const submit = useCallback(
    async (category: string, description: string, costLessonTime: boolean) => {
      if (!target) {
        return;
      }

      try {
        setBusy(true);
        await reportSupportTicket({
          category,
          description,
          costLessonTime,
          participantId: target.participantId,
          sessionId,
          groupId,
        });
        toast.success("Zgłoszenie zapisane w historii dziecka.");
        setTarget(null);
        setHistory(null);
      } catch (caught) {
        toast.error(caught instanceof ApiError ? caught.message : "Nie udało się zapisać zgłoszenia.");
      } finally {
        setBusy(false);
      }
    },
    [groupId, sessionId, target, toast],
  );

  /**
   * Zgłoszenie incydentu z poziomu zajęć.
   *
   * Instruktor **nie ma** dostępu do prowadzenia spraw (to należy do administracji) i dlatego
   * nie ma pozycji w menu — ale musi móc zgłosić rzecz, która dzieje się teraz, nie po
   * powrocie do domu. Słownik rodzajów pobieramy przy otwarciu okna; gdyby nie doszedł,
   * zostaje lista awaryjna, bo zgłoszenia incydentu nie wolno zablokować na błędzie sieci.
   */
  const openIncidentDialog = useCallback(async () => {
    setIncidentOpen(true);

    try {
      const board = await getIncidents();

      if (board.kindOptions.length > 0) {
        setKindOptions(board.kindOptions);
        setSeverityOptions(board.severityOptions);
      }
    } catch {
      // Zostajemy na liście awaryjnej.
    }
  }, []);

  const submitIncident = useCallback(
    async (payload: { kind: string; severity: string; description: string; participantIds: string[] }) => {
      try {
        setBusy(true);
        await reportIncident({ ...payload, sessionId, groupId });
        toast.success("Incydent zgłoszony. Administracja go zobaczy.");
        setIncidentOpen(false);
      } catch (caught) {
        toast.error(caught instanceof ApiError ? caught.message : "Nie udało się zgłosić incydentu.");
      } finally {
        setBusy(false);
      }
    },
    [groupId, sessionId, toast],
  );

  return {
    target,
    history,
    categoryOptions,
    loadingHistory,
    busy,
    open,
    close,
    submit,

    incidentOpen,
    kindOptions,
    severityOptions,
    openIncidentDialog,
    closeIncidentDialog: () => setIncidentOpen(false),
    submitIncident,
  };
}

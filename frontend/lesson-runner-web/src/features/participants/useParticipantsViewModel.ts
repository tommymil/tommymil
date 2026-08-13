import { useEffect, useState } from "react";
import { ApiError } from "../../api/client";
import { getGroups } from "../../api/groupsApi";
import {
  anonymizeParticipant as anonymizeParticipantRequest,
  archiveParticipant as archiveParticipantRequest,
  createGuardianAccount as createGuardianAccountRequest,
  createParticipant as createParticipantRequest,
  deleteParticipant as deleteParticipantRequest,
  enrollParticipant as enrollParticipantRequest,
  getParticipant,
  getParticipants,
  restoreParticipant as restoreParticipantRequest,
  unenrollParticipant as unenrollParticipantRequest,
  updateParticipant as updateParticipantRequest,
} from "../../api/participantsApi";
import type { GroupSummary } from "../../types/group";
import { draftToRequest, emptyParticipantDraft } from "../../types/participant";
import type {
  GuardianAccountResult,
  ParticipantDetails,
  ParticipantDraft,
  ParticipantSummary,
  UpdateParticipantRequest,
} from "../../types/participant";

export function useParticipantsViewModel() {
  const [participants, setParticipants] = useState<ParticipantSummary[]>([]);
  const [groups, setGroups] = useState<GroupSummary[]>([]);
  const [query, setQuery] = useState("");
  const [includeArchived, setIncludeArchived] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [createDraft, setCreateDraft] = useState<ParticipantDraft>(emptyParticipantDraft());

  useEffect(() => {
    let ignore = false;

    getGroups()
      .then((list) => {
        if (!ignore) {
          setGroups(list);
        }
      })
      .catch(() => {
        // brak grup nie blokuje listy uczestników
      });

    return () => {
      ignore = true;
    };
  }, []);

  useEffect(() => {
    let ignore = false;

    const timer = setTimeout(() => {
      void (async () => {
        try {
          setLoading(true);
          setError(null);
          const list = await getParticipants(query, includeArchived);
          if (!ignore) {
            setParticipants(list);
          }
        } catch {
          if (!ignore) {
            setError("Nie udało się pobrać uczestników.");
          }
        } finally {
          if (!ignore) {
            setLoading(false);
          }
        }
      })();
    }, 250);

    return () => {
      ignore = true;
      clearTimeout(timer);
    };
  }, [query, includeArchived, reloadKey]);

  function setCreateField(field: keyof ParticipantDraft, value: string | boolean) {
    setCreateDraft((current) => ({ ...current, [field]: value }));
  }

  async function createParticipant(): Promise<boolean> {
    if (!createDraft.firstName.trim() || !createDraft.lastName.trim()) {
      setError("Uczestnik musi mieć imię i nazwisko.");
      return false;
    }

    try {
      setBusy(true);
      setError(null);
      const created = await createParticipantRequest(draftToRequest(createDraft));
      setParticipants((current) => [...current, created].sort(byName));
      setCreateDraft(emptyParticipantDraft());
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się dodać uczestnika.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  async function updateParticipant(id: string, request: UpdateParticipantRequest): Promise<boolean> {
    try {
      setBusy(true);
      setError(null);
      const updated = await updateParticipantRequest(id, request);
      setParticipants((current) => current.map((item) => (item.id === id ? updated : item)).sort(byName));
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać uczestnika.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  async function archiveParticipant(id: string): Promise<boolean> {
    try {
      setBusy(true);
      setError(null);
      await archiveParticipantRequest(id);
      setReloadKey((key) => key + 1);
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zarchiwizować uczestnika.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  async function restoreParticipant(id: string): Promise<boolean> {
    try {
      setBusy(true);
      setError(null);
      await restoreParticipantRequest(id);
      setReloadKey((key) => key + 1);
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się przywrócić uczestnika.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  async function anonymizeParticipant(id: string): Promise<boolean> {
    try {
      setBusy(true);
      setError(null);
      await anonymizeParticipantRequest(id);
      setReloadKey((key) => key + 1);
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zanonimizować uczestnika.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  /** Pobiera pełne dane osobowe uczestnika (RODO - eksport); zwraca null przy błędzie. */
  async function fetchParticipantForExport(id: string): Promise<ParticipantDetails | null> {
    try {
      setError(null);
      return await getParticipant(id);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać danych do eksportu.");
      return null;
    }
  }

  async function deleteParticipant(id: string): Promise<boolean> {
    try {
      setBusy(true);
      setError(null);
      await deleteParticipantRequest(id);
      setParticipants((current) => current.filter((item) => item.id !== id));
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się usunąć uczestnika.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  /**
   * Konto opiekuna zakładane z karty dziecka.
   *
   * Zwraca wynik, a nie `boolean`, bo strona ma o czym powiedzieć: czy poszło zaproszenie
   * i czy konto powstało, czy tylko podpięliśmy istniejące (drugie dziecko tej rodziny).
   * Lista wymaga przeładowania - zmienia się znacznik `hasGuardianAccount`.
   */
  async function createGuardianAccount(participantId: string): Promise<GuardianAccountResult | null> {
    try {
      setBusy(true);
      setError(null);
      const result = await createGuardianAccountRequest(participantId);
      setReloadKey((key) => key + 1);
      return result;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się założyć konta opiekunowi.");
      return null;
    } finally {
      setBusy(false);
    }
  }

  async function assignToGroup(participantId: string, groupId: string): Promise<boolean> {
    if (!groupId) {
      return false;
    }

    try {
      setBusy(true);
      setError(null);
      const updated = await enrollParticipantRequest(participantId, groupId);
      setParticipants((current) => current.map((item) => (item.id === participantId ? updated : item)));
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się przypisać uczestnika do grupy.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  async function removeFromGroup(participantId: string, groupId: string): Promise<boolean> {
    try {
      setBusy(true);
      setError(null);
      const updated = await unenrollParticipantRequest(participantId, groupId);
      setParticipants((current) => current.map((item) => (item.id === participantId ? updated : item)));
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się wypisać uczestnika z grupy.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  return {
    participants,
    groups,
    query,
    setQuery,
    includeArchived,
    setIncludeArchived,
    loading,
    busy,
    error,
    createDraft,
    setCreateField,
    createParticipant,
    updateParticipant,
    archiveParticipant,
    restoreParticipant,
    anonymizeParticipant,
    fetchParticipantForExport,
    deleteParticipant,
    createGuardianAccount,
    assignToGroup,
    removeFromGroup,
  };
}

function byName(first: ParticipantSummary, second: ParticipantSummary): number {
  return `${first.lastName} ${first.firstName}`.localeCompare(`${second.lastName} ${second.firstName}`, "pl");
}

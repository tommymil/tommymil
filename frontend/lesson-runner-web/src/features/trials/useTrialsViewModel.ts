import { useCallback, useEffect, useMemo, useState } from "react";
import { ApiError } from "../../api/client";
import { getUsers } from "../../api/usersApi";
import {
  createTrial,
  declineTrial,
  deleteTrial,
  enrollTrial,
  getTrials,
  scheduleTrial,
} from "../../api/trialsApi";
import { draftToTrialRequest, emptyTrialDraft } from "../../types/trial";
import type {
  DeclineTrialRequest,
  ScheduleTrialRequest,
  TrialBoard,
  TrialDraft,
  TrialEnrollmentResult,
} from "../../types/trial";
import type { ManagedUser } from "../../types/user";

/** Zakładki listy — pokrywają się z etapami ścieżki, a nie z surowymi statusami. */
export type TrialView = "uwaga" | "umowione" | "zamkniete";

export function useTrialsViewModel() {
  const [board, setBoard] = useState<TrialBoard | null>(null);
  const [instructors, setInstructors] = useState<ManagedUser[]>([]);
  const [view, setView] = useState<TrialView>("uwaga");
  const [draft, setDraft] = useState<TrialDraft>(emptyTrialDraft());
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const reload = useCallback(async () => {
    try {
      setError(null);
      setBoard(await getTrials());
    } catch {
      setError("Nie udało się pobrać lekcji próbnych.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void reload();
  }, [reload]);

  useEffect(() => {
    getUsers()
      .then((users) => setInstructors(users.filter((user) => user.role === "instructor" && user.isActive)))
      .catch(() => {
        // Brak listy instruktorów nie blokuje przyjmowania zgłoszeń - termin można ustawić później.
      });
  }, []);

  const trials = board?.trials ?? [];

  const buckets = useMemo(
    () => ({
      // „Czeka na ruch” to jedyna liczba, po którą warto tu wchodzić: zgłoszenie bez terminu
      // albo dziecko po lekcji, którego nikt jeszcze nie przyjął ani nie odrzucił.
      uwaga: trials.filter((trial) => trial.needsAttention),
      umowione: trials.filter((trial) => trial.status === "scheduled"),
      zamkniete: trials.filter((trial) => ["enrolled", "declined", "noshow"].includes(trial.status)),
    }),
    [trials],
  );

  function setField(field: keyof TrialDraft, value: string) {
    setDraft((current) => ({ ...current, [field]: value }));
  }

  async function run<T>(action: () => Promise<T>, fallbackMessage: string): Promise<T | null> {
    try {
      setBusy(true);
      setError(null);
      const result = await action();
      await reload();
      return result;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : fallbackMessage);
      return null;
    } finally {
      setBusy(false);
    }
  }

  async function submitTrial(): Promise<boolean> {
    if (!draft.childFirstName.trim() || !draft.childLastName.trim()) {
      setError("Podaj imię i nazwisko dziecka.");
      return false;
    }

    const created = await run(() => createTrial(draftToTrialRequest(draft)), "Nie udało się zapisać zgłoszenia.");

    if (created) {
      setDraft(emptyTrialDraft());
    }

    return Boolean(created);
  }

  const schedule = (id: string, request: ScheduleTrialRequest) =>
    run(() => scheduleTrial(id, request), "Nie udało się zapisać terminu.").then(Boolean);

  const enroll = (id: string, createGuardianAccount: boolean): Promise<TrialEnrollmentResult | null> =>
    run(() => enrollTrial(id, { createGuardianAccount }), "Nie udało się zapisać dziecka do systemu.");

  const decline = (id: string, request: DeclineTrialRequest) =>
    run(() => declineTrial(id, request), "Nie udało się zamknąć zgłoszenia.").then(Boolean);

  const remove = (id: string) => run(() => deleteTrial(id), "Nie udało się usunąć zgłoszenia.").then(Boolean);

  return {
    board,
    trials,
    buckets,
    instructors,
    view,
    setView,
    draft,
    setField,
    submitTrial,
    schedule,
    enroll,
    decline,
    remove,
    loading,
    busy,
    error,
  };
}

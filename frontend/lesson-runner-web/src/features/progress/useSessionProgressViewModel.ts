import { useEffect, useState } from "react";
import { ApiError } from "../../api/client";
import { getSessionProgress, saveSessionProgress } from "../../api/progressApi";
import type { AutonomyOption, SaveProgressEntry } from "../../types/progress";

export type ProgressDraft = SaveProgressEntry & {
  firstName: string;
  lastName: string;
};

type Child = { participantId: string; firstName: string; lastName: string };

/**
 * Wpisy o postępach dla jednego terminu.
 *
 * Lista dzieci pochodzi z listy obecności, nie z zapisanych wpisów — inaczej instruktor
 * widziałby tylko te dzieci, którym już coś wpisał, i nigdy nie dopisałby reszty.
 */
export function useSessionProgressViewModel(sessionId: string | undefined, children: Child[], open: boolean) {
  const [drafts, setDrafts] = useState<ProgressDraft[]>([]);
  const [options, setOptions] = useState<AutonomyOption[]>([]);
  const [loading, setLoading] = useState(false);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saved, setSaved] = useState(false);

  // Tablica `children` dostaje nową tożsamość przy każdym renderze rodzica, więc w zależnościach
  // efektu musi siedzieć jej treść, a nie referencja - inaczej panel pobierałby dane w pętli.
  const childrenKey = children.map((child) => child.participantId).join(",");

  useEffect(() => {
    if (!open || !sessionId) {
      return;
    }

    let ignore = false;

    async function load() {
      try {
        setLoading(true);
        setError(null);
        setSaved(false);
        const response = await getSessionProgress(sessionId!);

        if (ignore) {
          return;
        }

        setOptions(response.autonomyOptions);
        setDrafts(
          children.map((child) => {
            const existing = response.entries.find((entry) => entry.participantId === child.participantId);

            return {
              participantId: child.participantId,
              firstName: child.firstName,
              lastName: child.lastName,
              autonomy: existing?.autonomy ?? "withhelp",
              lessonCompleted: existing?.lessonCompleted ?? false,
              noteForParent: existing?.noteForParent ?? null,
              nextStep: existing?.nextStep ?? null,
            };
          }),
        );
      } catch (caught) {
        if (!ignore) {
          setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać postępów.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    void load();

    return () => {
      ignore = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, sessionId, childrenKey]);

  function update(participantId: string, patch: Partial<SaveProgressEntry>) {
    setSaved(false);
    setDrafts((current) =>
      current.map((draft) => (draft.participantId === participantId ? { ...draft, ...patch } : draft)),
    );
  }

  async function persist(): Promise<boolean> {
    if (!sessionId) {
      return false;
    }

    try {
      setBusy(true);
      setError(null);
      await saveSessionProgress(
        sessionId,
        drafts.map(({ firstName: _firstName, lastName: _lastName, ...entry }) => entry),
      );
      setSaved(true);
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać postępów.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  return { drafts, options, loading, busy, error, saved, update, persist };
}

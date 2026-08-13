import { useCallback, useEffect, useMemo, useState } from "react";
import { ApiError } from "../../api/client";
import { getMyTrials, saveTrialDiagnosis } from "../../api/trialsApi";
import type { SaveTrialDiagnosisRequest, TrialBoard } from "../../types/trial";

/**
 * Lekcje próbne instruktora.
 *
 * Instruktor robi tu jedno: prowadzi swoją lekcję i wypełnia diagnozę. Nie widzi cudzych
 * kandydatur i nie decyduje o przyjęciu — to należy do administracji i ma osobne trasy.
 */
export function useMyTrialsViewModel() {
  const [board, setBoard] = useState<TrialBoard | null>(null);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const reload = useCallback(async () => {
    try {
      setError(null);
      setBoard(await getMyTrials());
    } catch {
      setError("Nie udało się pobrać lekcji próbnych.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void reload();
  }, [reload]);

  const trials = board?.trials ?? [];

  const upcoming = useMemo(
    () => trials.filter((trial) => trial.status === "scheduled"),
    [trials],
  );
  const done = useMemo(
    () => trials.filter((trial) => trial.status !== "scheduled"),
    [trials],
  );

  async function saveDiagnosis(id: string, request: SaveTrialDiagnosisRequest): Promise<boolean> {
    try {
      setBusy(true);
      setError(null);
      await saveTrialDiagnosis(id, request);
      await reload();
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać diagnozy.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  return { board, trials, upcoming, done, saveDiagnosis, loading, busy, error };
}

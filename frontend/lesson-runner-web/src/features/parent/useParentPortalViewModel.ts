import { useCallback, useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { ApiError } from "../../api/client";
import { getParentPortal, reportAbsence, updateParentConsent } from "../../api/parentApi";
import { useToast } from "../toast/ToastContext";
import type {
  ParentAttendanceItem,
  ParentChildProgress,
  ParentCourseProgress,
  ParentCredit,
  ParentInvoice,
  ParentMaterial,
  ParentPortal,
  ParentScheduleItem,
} from "../../types/parent";

export type ParentView = "plan" | "postepy" | "materialy" | "rozliczenia" | "zgody";

const views: ParentView[] = ["plan", "postepy", "materialy", "rozliczenia", "zgody"];

/**
 * Model widoku portalu rodzica.
 *
 * Portal był jedną stroną z siedmioma sekcjami rozwiniętymi naraz, a rodzic wchodzi
 * po jedno pytanie: kiedy są zajęcia i gdzie kliknąć. Tutaj hierarchia jest jawna:
 * najbliższe zajęcia i cztery liczby zawsze na wierzchu, reszta pod zakładkami.
 *
 * Wybór dziecka i zakładka żyją w adresie (`?dziecko=`, `?widok=`), a nie w stanie
 * komponentu. Dzięki temu cofnięcie w przeglądarce wraca do poprzedniego widoku,
 * a link do konkretnej zakładki da się wysłać.
 */
export function useParentPortalViewModel() {
  const [portal, setPortal] = useState<ParentPortal | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);
  const [params, setParams] = useSearchParams();
  const toast = useToast();

  const load = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      setPortal(await getParentPortal());
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać portalu rodzica.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const requestedView = params.get("widok") as ParentView | null;
  const view: ParentView = requestedView && views.includes(requestedView) ? requestedView : "plan";

  // Przy jednym dziecku przełącznik nie ma sensu, więc filtr jest wtedy wyłączony.
  const children = portal?.children ?? [];
  const requestedChild = params.get("dziecko");
  const selectedChildId =
    children.length > 1 && requestedChild && children.some((child) => child.participantId === requestedChild)
      ? requestedChild
      : null;

  function setView(next: ParentView) {
    const updated = new URLSearchParams(params);
    updated.set("widok", next);
    setParams(updated, { replace: true });
  }

  function setChild(participantId: string | null) {
    const updated = new URLSearchParams(params);

    if (participantId) {
      updated.set("dziecko", participantId);
    } else {
      updated.delete("dziecko");
    }

    setParams(updated, { replace: true });
  }

  /** Filtr po dziecku stosowany do każdej sekcji naraz. */
  const filtered = useMemo(() => {
    const empty = {
      schedule: [] as ParentScheduleItem[],
      materials: [] as ParentMaterial[],
      progress: [] as ParentChildProgress[],
      invoices: [] as ParentInvoice[],
      attendance: [] as ParentAttendanceItem[],
      credits: [] as ParentCredit[],
      courseProgress: [] as ParentCourseProgress[],
    };

    if (!portal) {
      return empty;
    }

    const keep = (participantId: string | undefined | null) =>
      !selectedChildId || participantId === selectedChildId;

    return {
      schedule: portal.schedule.filter(
        (item) => !selectedChildId || (item.children ?? []).some((child) => child.participantId === selectedChildId),
      ),
      materials: portal.materials.filter(
        (item) => !selectedChildId || (item.participantIds ?? []).includes(selectedChildId),
      ),
      progress: portal.progress.filter((item) => keep(item.participantId)),
      invoices: portal.invoices.filter((item) => keep(item.participantId)),
      attendance: portal.attendance.filter((item) => keep(item.participantId)),
      credits: (portal.credits ?? []).filter((item) => keep(item.participantId)),
      courseProgress: (portal.courseProgress ?? []).filter((item) => keep(item.participantId)),
    };
  }, [portal, selectedChildId]);

  async function submitAbsence(sessionId: string, participantId: string, reason: string | null, childName: string) {
    try {
      setBusy(true);
      await reportAbsence(sessionId, participantId, reason);
      toast.success(`Zgłosiliśmy nieobecność: ${childName}. Instruktor zobaczy to na liście obecności.`);
      await load();
    } catch (caught) {
      toast.error(
        caught instanceof ApiError
          ? caught.message
          : "Nie udało się zgłosić nieobecności. Odśwież stronę i spróbuj ponownie.",
      );
    } finally {
      setBusy(false);
    }
  }

  async function submitConsent(participantId: string, imageConsent: boolean, childName: string) {
    try {
      setBusy(true);
      await updateParentConsent(participantId, imageConsent);
      toast.success(
        imageConsent
          ? `Zgoda na wizerunek dla ${childName} została udzielona.`
          : `Zgoda na wizerunek dla ${childName} została wycofana.`,
      );
      await load();
    } catch (caught) {
      toast.error(caught instanceof ApiError ? caught.message : "Nie udało się zapisać zgody.");
    } finally {
      setBusy(false);
    }
  }

  return {
    portal,
    loading,
    error,
    busy,
    view,
    setView,
    children,
    selectedChildId,
    setChild,
    ...filtered,
    reload: load,
    submitAbsence,
    submitConsent,
  };
}

import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { getLesson, startLessonRunSession, updateLessonRunSession } from "../../api/lessonsApi";
import type { LessonDetails, LessonRunSession } from "../../types/lesson";

export function usePresenterViewModel(lessonId: string | undefined, scheduledSessionId?: string) {
  const [lesson, setLesson] = useState<LessonDetails | null>(null);
  const [sessionReady, setSessionReady] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [stepIndex, setStepIndex] = useState(0);
  const [running, setRunning] = useState(false);
  const [elapsedTotalSeconds, setElapsedTotalSeconds] = useState(0);
  const [elapsedStepSeconds, setElapsedStepSeconds] = useState(0);
  const [showNotes, setShowNotes] = useState(true);
  const [showStudent, setShowStudent] = useState(true);
  const [copiedResourceIndex, setCopiedResourceIndex] = useState<number | null>(null);
  const [syncError, setSyncError] = useState(false);

  useEffect(() => {
    if (!lessonId) {
      setError("Brak identyfikatora lekcji.");
      setLoading(false);
      return undefined;
    }

    const id = lessonId;
    let ignore = false;

    async function loadLesson() {
      try {
        setLoading(true);
        setError(null);
        setSessionReady(false);
        setSyncError(false);
        const response = await getLesson(id);

        if (!ignore) {
          const fallbackStep = getSavedStepIndex(id, response.steps.length);
          setLesson(response);
          setStepIndex(fallbackStep);
          setElapsedStepSeconds(0);
          setElapsedTotalSeconds(0);
          setRunning(false);
        }

        try {
          const session = await startLessonRunSession(id, scheduledSessionId);

          if (ignore) {
            return;
          }

          const elapsedOffset = getElapsedOffset(session);
          const initialStep = Math.min(session.stepIndex, Math.max(response.steps.length - 1, 0));
          setLesson(response);
          setStepIndex(initialStep);
          setElapsedStepSeconds(session.elapsedStepSeconds + elapsedOffset);
          setElapsedTotalSeconds(session.elapsedTotalSeconds + elapsedOffset);
          setRunning(session.running);
          setSessionReady(true);
        } catch {
          if (!ignore) {
            setSessionReady(false);
          }
        }
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać szczegółów lekcji.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    void loadLesson();

    return () => {
      ignore = true;
    };
  }, [lessonId, scheduledSessionId]);

  // Najświeższy stan trzymamy w ref, żeby zapis mógł wysłać aktualne wartości bez zależności
  // od tykającego co sekundę licznika (inaczej lecielibyśmy z zapisem raz na sekundę).
  const sessionStateRef = useRef({ stepIndex, elapsedTotalSeconds, elapsedStepSeconds, running });
  sessionStateRef.current = { stepIndex, elapsedTotalSeconds, elapsedStepSeconds, running };

  const persistSession = useCallback(() => {
    if (!lessonId) {
      return;
    }

    void updateLessonRunSession(lessonId, { ...sessionStateRef.current }, scheduledSessionId)
      .then(() => setSyncError(false))
      .catch(() => {
        // Sesja jest pomocnicza; błąd zapisu nie powinien zatrzymywać prowadzenia lekcji,
        // ale prowadzący musi wiedzieć, że postęp przestał się zapisywać.
        setSyncError(true);
      });
  }, [lessonId, scheduledSessionId]);

  // Stan istotny dla wznowienia (krok i start/pauza) zapisujemy od razu. Nie zależy od licznika
  // sekund, więc podczas biegnącego timera nie generuje zapytania co sekundę.
  useEffect(() => {
    if (!lessonId || !sessionReady) {
      return undefined;
    }

    const timeoutId = window.setTimeout(persistSession, 400);
    return () => window.clearTimeout(timeoutId);
  }, [stepIndex, running, sessionReady, lessonId, persistSession]);

  // Upływ czasu utrwalamy tylko gdy timer stoi (reset/pauza). Podczas biegu odtwarza go
  // getElapsedOffset na podstawie updatedAt, więc każdej sekundy nie trzeba zapisywać.
  useEffect(() => {
    if (!lessonId || !sessionReady || running) {
      return undefined;
    }

    const timeoutId = window.setTimeout(persistSession, 400);
    return () => window.clearTimeout(timeoutId);
  }, [elapsedTotalSeconds, elapsedStepSeconds, running, sessionReady, lessonId, persistSession]);

  useEffect(() => {
    if (!running) {
      return undefined;
    }

    const timerId = window.setInterval(() => {
      setElapsedStepSeconds((current) => current + 1);
      setElapsedTotalSeconds((current) => current + 1);
    }, 1000);

    return () => window.clearInterval(timerId);
  }, [running]);

  useEffect(() => {
    function handleKeyDown(event: KeyboardEvent) {
      const target = event.target as HTMLElement | null;

      if (target?.tagName === "INPUT" || target?.tagName === "TEXTAREA" || target?.tagName === "SELECT") {
        return;
      }

      if (event.key === "ArrowRight" || event.key === " " || event.key === "PageDown") {
        event.preventDefault();
        nextStep();
      } else if (event.key === "ArrowLeft" || event.key === "PageUp") {
        event.preventDefault();
        previousStep();
      } else if (event.key === "p" || event.key === "P") {
        event.preventDefault();
        toggleRunning();
      } else if (event.key === "n" || event.key === "N") {
        setShowNotes((current) => !current);
      } else if (event.key === "m" || event.key === "M") {
        setShowStudent((current) => !current);
      }
    }

    window.addEventListener("keydown", handleKeyDown);

    return () => window.removeEventListener("keydown", handleKeyDown);
  });

  const currentStep = lesson?.steps[stepIndex] ?? null;
  const plannedStepSeconds = (currentStep?.durationMinutes ?? 0) * 60;
  const plannedTotalSeconds = useMemo(
    () => lesson?.steps.reduce((sum, step) => sum + step.durationMinutes * 60, 0) ?? 0,
    [lesson],
  );

  function goToStep(index: number) {
    if (!lesson) {
      return;
    }

    const nextIndex = Math.min(Math.max(index, 0), Math.max(lesson.steps.length - 1, 0));
    setStepIndex(nextIndex);
    setElapsedStepSeconds(0);
    setCopiedResourceIndex(null);

    if (!sessionReady && lessonId) {
      localStorage.setItem(positionKey(lessonId), String(nextIndex));
    }
  }

  function nextStep() {
    if (!lesson) {
      return;
    }

    goToStep(Math.min(stepIndex + 1, Math.max(lesson.steps.length - 1, 0)));
  }

  function previousStep() {
    goToStep(Math.max(stepIndex - 1, 0));
  }

  function resetTimers() {
    setElapsedStepSeconds(0);
    setElapsedTotalSeconds(0);
    setRunning(false);
  }

  function toggleRunning() {
    setRunning((current) => !current);
  }

  async function copyResourceCode(resourceIndex: number, code: string | null | undefined) {
    if (!code) {
      return;
    }

    await navigator.clipboard?.writeText(code);
    setCopiedResourceIndex(resourceIndex);
    window.setTimeout(() => setCopiedResourceIndex(null), 1400);
  }

  return {
    atFirst: stepIndex === 0,
    atLast: lesson ? stepIndex === lesson.steps.length - 1 : true,
    copyResourceCode,
    copiedResourceIndex,
    currentStep,
    elapsedStepLabel: formatSeconds(elapsedStepSeconds),
    elapsedTotalLabel: formatSeconds(elapsedTotalSeconds),
    error,
    goToStep,
    lesson,
    loading,
    nextStep,
    plannedStepLabel: formatSeconds(plannedStepSeconds),
    plannedTotalLabel: formatSeconds(plannedTotalSeconds),
    previousStep,
    progressPercent: lesson && lesson.steps.length > 0 ? Math.round(((stepIndex + 1) / lesson.steps.length) * 100) : 0,
    resetTimers,
    running,
    showNotes,
    showStudent,
    stepIndex,
    stepLabel: lesson ? `${stepIndex + 1} / ${lesson.steps.length}` : "0 / 0",
    stepOverTime: plannedStepSeconds > 0 && elapsedStepSeconds > plannedStepSeconds,
    syncError,
    toggleNotes: () => setShowNotes((current) => !current),
    toggleRunning,
    toggleStudent: () => setShowStudent((current) => !current),
  };
}

function formatSeconds(seconds: number) {
  const safeSeconds = Math.max(0, Math.floor(seconds));
  const minutes = Math.floor(safeSeconds / 60);
  const remainingSeconds = safeSeconds % 60;

  return `${minutes.toString().padStart(2, "0")}:${remainingSeconds.toString().padStart(2, "0")}`;
}

function getSavedStepIndex(lessonId: string, stepCount: number) {
  const saved = Number(localStorage.getItem(positionKey(lessonId)));
  const fallback = Number.isFinite(saved) && saved >= 0 ? saved : 0;
  return Math.min(fallback, Math.max(stepCount - 1, 0));
}

function positionKey(lessonId: string) {
  return `lesson-runner:presenter:${lessonId}:step`;
}

function getElapsedOffset(session: LessonRunSession) {
  if (!session.running) {
    return 0;
  }

  const updatedAt = Date.parse(session.updatedAt);

  if (!Number.isFinite(updatedAt)) {
    return 0;
  }

  return Math.max(0, Math.floor((Date.now() - updatedAt) / 1000));
}

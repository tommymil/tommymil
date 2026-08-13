import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { usePresenterViewModel } from "./usePresenterViewModel";
import type { LessonDetails, LessonRunSession, LessonStep } from "../../types/lesson";
import * as lessonsApi from "../../api/lessonsApi";

vi.mock("../../api/lessonsApi");

function step(order: number, durationMinutes: number): LessonStep {
  return {
    id: `step-${order}`,
    order,
    type: "concept",
    title: `Krok ${order}`,
    durationMinutes,
    script: [],
    studentItems: [],
    resources: [],
    notes: [],
  };
}

const lesson: LessonDetails = {
  id: "lesson-1",
  title: "Lekcja",
  subject: "Scratch",
  level: "Poziom 1",
  description: "Opis",
  order: 1,
  status: "ready",
  statusLabel: "Gotowa",
  stepCount: 3,
  durationMinutes: 6,
  projectFiles: {},
  tags: [],
  steps: [step(1, 2), step(2, 3), step(3, 1)],
};

const session: LessonRunSession = {
  id: "session-1",
  lessonId: "lesson-1",
  userId: "user-1",
  stepIndex: 0,
  elapsedTotalSeconds: 0,
  elapsedStepSeconds: 0,
  running: false,
  startedAt: "2026-06-13T10:00:00.000Z",
  updatedAt: "2026-06-13T10:00:00.000Z",
};

describe("usePresenterViewModel", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.mocked(lessonsApi.getLesson).mockResolvedValue(lesson);
    vi.mocked(lessonsApi.startLessonRunSession).mockResolvedValue(session);
    vi.mocked(lessonsApi.updateLessonRunSession).mockResolvedValue(session);
  });

  it("starts on the first step", async () => {
    const { result } = renderHook(() => usePresenterViewModel("lesson-1"));
    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.stepIndex).toBe(0);
    expect(result.current.atFirst).toBe(true);
    expect(result.current.atLast).toBe(false);
    expect(result.current.stepLabel).toBe("1 / 3");
    expect(result.current.progressPercent).toBe(33);
  });

  it("advances and clamps navigation at the bounds", async () => {
    const { result } = renderHook(() => usePresenterViewModel("lesson-1"));
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.nextStep());
    expect(result.current.stepIndex).toBe(1);

    act(() => result.current.nextStep());
    act(() => result.current.nextStep()); // próba wyjścia poza ostatni krok
    expect(result.current.stepIndex).toBe(2);
    expect(result.current.atLast).toBe(true);

    act(() => result.current.previousStep());
    expect(result.current.stepIndex).toBe(1);
  });

  it("restores the current step from the persistent run session", async () => {
    vi.mocked(lessonsApi.startLessonRunSession).mockResolvedValueOnce({
      ...session,
      stepIndex: 2,
      elapsedTotalSeconds: 90,
      elapsedStepSeconds: 15,
    });

    const { result } = renderHook(() => usePresenterViewModel("lesson-1"));
    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.stepIndex).toBe(2);
    expect(result.current.elapsedTotalLabel).toBe("01:30");
    expect(result.current.elapsedStepLabel).toBe("00:15");
  });

  it("falls back to local state when the run session cannot be loaded", async () => {
    localStorage.setItem("lesson-runner:presenter:lesson-1:step", "1");
    vi.mocked(lessonsApi.startLessonRunSession).mockRejectedValueOnce(new Error("session endpoint missing"));

    const { result } = renderHook(() => usePresenterViewModel("lesson-1"));
    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.error).toBeNull();
    expect(result.current.lesson?.id).toBe("lesson-1");
    expect(result.current.stepIndex).toBe(1);
  });

  it("formats planned timings as mm:ss", async () => {
    const { result } = renderHook(() => usePresenterViewModel("lesson-1"));
    await waitFor(() => expect(result.current.loading).toBe(false));

    // Pierwszy krok: 2 min -> 02:00, cała lekcja: 6 min -> 06:00.
    expect(result.current.plannedStepLabel).toBe("02:00");
    expect(result.current.plannedTotalLabel).toBe("06:00");
    expect(result.current.elapsedTotalLabel).toBe("00:00");
  });

  it("flags a sync error when the session fails to persist, and clears it on the next successful save", async () => {
    vi.mocked(lessonsApi.updateLessonRunSession).mockRejectedValueOnce(new Error("network down"));

    const { result } = renderHook(() => usePresenterViewModel("lesson-1"));
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.toggleRunning());

    await waitFor(() => expect(result.current.syncError).toBe(true));

    act(() => result.current.toggleRunning());

    await waitFor(() => expect(result.current.syncError).toBe(false));
  });

  it("reports an error when there is no lesson id", async () => {
    const { result } = renderHook(() => usePresenterViewModel(undefined));
    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.error).toBe("Brak identyfikatora lekcji.");
  });
});

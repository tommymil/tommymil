import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useLessonsViewModel } from "./useLessonsViewModel";
import type { LessonSummary } from "../../types/lesson";
import * as lessonsApi from "../../api/lessonsApi";

vi.mock("../../api/lessonsApi");

const lessons: LessonSummary[] = [
  {
    id: "1",
    title: "Pierwsza gra",
    subject: "Scratch",
    level: "Poziom 1",
    description: "Ruch postaci",
    order: 1,
    status: "ready",
    statusLabel: "Gotowa",
    stepCount: 7,
    durationMinutes: 53,
    kind: "standard", kindLabel: "Standardowa grupowa", scheduledDurationMinutes: 95, earlyLeaveAfterMinutes: null,
  },
  {
    id: "2",
    title: "Agent buduje most",
    subject: "Minecraft",
    level: "Poziom 1",
    description: "Kodowanie agenta",
    order: 2,
    status: "ready",
    statusLabel: "Gotowa",
    stepCount: 6,
    durationMinutes: 45,
    kind: "standard", kindLabel: "Standardowa grupowa", scheduledDurationMinutes: 95, earlyLeaveAfterMinutes: null,
  },
  {
    id: "3",
    title: "Pętle w Pythonie",
    subject: "Python",
    level: "Poziom 2",
    description: "For i while",
    order: 3,
    status: "draft",
    statusLabel: "Szkic",
    stepCount: 5,
    durationMinutes: 40,
    kind: "standard", kindLabel: "Standardowa grupowa", scheduledDurationMinutes: 95, earlyLeaveAfterMinutes: null,
  },
  {
    id: "4",
    title: "Zmienne",
    subject: "Python",
    level: "Poziom 2",
    description: "Typy danych",
    order: 4,
    status: "review",
    statusLabel: "Do sprawdzenia",
    stepCount: 4,
    durationMinutes: 30,
    kind: "standard", kindLabel: "Standardowa grupowa", scheduledDurationMinutes: 95, earlyLeaveAfterMinutes: null,
  },
  {
    id: "5",
    title: "Pierwszy świat",
    subject: "Minecraft Education",
    level: "Poziom 1",
    description: "Lekcja pokazowa w Minecraft Education",
    order: 5,
    status: "ready",
    statusLabel: "Gotowa",
    stepCount: 6,
    durationMinutes: 60,
    kind: "showcase", kindLabel: "Pokazowa", scheduledDurationMinutes: 60, earlyLeaveAfterMinutes: 55,
  },
];

describe("useLessonsViewModel", () => {
  beforeEach(() => {
    vi.mocked(lessonsApi.getLessons).mockResolvedValue(lessons);
    vi.mocked(lessonsApi.deleteLesson).mockResolvedValue(undefined);
  });

  it("loads lessons from every technology by default", async () => {
    const { result } = renderHook(() => useLessonsViewModel());

    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.subjectFilter).toBe("all");
    expect(result.current.lessonCount).toBe(5);
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["1", "2", "3", "4", "5"]);
    expect(result.current.subjectOptions).toEqual(["Minecraft", "Minecraft Education", "Python", "Scratch"]);
    expect(result.current.showStatusFilter).toBe(true);
  });

  it("filters by selected technology", async () => {
    const { result } = renderHook(() => useLessonsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setSubjectFilter("Minecraft"));
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["2"]);
  });

  it("keeps Scratch in the technology filter even when there are no Scratch lessons", async () => {
    vi.mocked(lessonsApi.getLessons).mockResolvedValueOnce(
      lessons.filter((lesson) => lesson.subject !== "Scratch"),
    );
    const { result } = renderHook(() => useLessonsViewModel());

    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.subjectOptions).toContain("Scratch");
    expect(result.current.subjectOptions).toContain("Minecraft Education");
  });

  it("filters by free-text query across title, subject and description", async () => {
    const { result } = renderHook(() => useLessonsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setSubjectFilter("Minecraft"));
    act(() => result.current.setQuery("agent"));
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["2"]);

    act(() => result.current.setSubjectFilter("Scratch"));
    act(() => result.current.setQuery("ruch"));
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["1"]);
  });

  it("filters by status", async () => {
    const { result } = renderHook(() => useLessonsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setStatusFilter("ready"));
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["1", "2", "5"]);
  });

  it("separates standard lessons from showcase lessons", async () => {
    const { result } = renderHook(() => useLessonsViewModel({ onlyReady: true }));
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setKindFilter("standard"));
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["1", "2"]);

    act(() => result.current.setKindFilter("showcase"));
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["5"]);

    act(() => result.current.setKindFilter("all"));
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["1", "2", "5"]);
  });

  it("removes a lesson from the list after delete", async () => {
    const { result } = renderHook(() => useLessonsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => result.current.removeLesson("1"));

    expect(lessonsApi.deleteLesson).toHaveBeenCalledWith("1");
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["2", "3", "4", "5"]);
  });

  it("restricts to ready lessons and hides the status filter when onlyReady is set", async () => {
    const { result } = renderHook(() => useLessonsViewModel({ onlyReady: true }));
    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.showStatusFilter).toBe(false);
    expect(result.current.lessons.map((lesson) => lesson.id)).toEqual(["1", "2", "5"]);
  });

  it("reports an error when the API call fails", async () => {
    vi.mocked(lessonsApi.getLessons).mockRejectedValueOnce(new Error("boom"));
    const { result } = renderHook(() => useLessonsViewModel());

    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.error).toBe("Nie udało się pobrać lekcji z API.");
  });
});

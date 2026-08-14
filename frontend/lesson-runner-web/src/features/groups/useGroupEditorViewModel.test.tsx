import type { PropsWithChildren } from "react";
import { act, renderHook, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useGroupEditorViewModel } from "./useGroupEditorViewModel";
import type { GroupDetails, Instructor } from "../../types/group";
import type { LessonSummary } from "../../types/lesson";
import { emptyParticipantDraft } from "../../types/participant";
import type { ParticipantSummary } from "../../types/participant";

const participantExtras = { birthDate: null, guardianName: null, guardianPhone: null, isArchived: false, hasDataConsent: false, hasImageConsent: false, guardianEmail: null, hasGuardianAccount: false };
import * as groupsApi from "../../api/groupsApi";
import * as lessonsApi from "../../api/lessonsApi";
import * as participantsApi from "../../api/participantsApi";
import * as coursesApi from "../../api/coursesApi";
import * as schedulingApi from "../../api/schedulingApi";

vi.mock("../../api/groupsApi");
vi.mock("../../api/lessonsApi");
vi.mock("../../api/participantsApi");
vi.mock("../../api/coursesApi");
vi.mock("../../api/schedulingApi");

const instructors: Instructor[] = [{ id: "inst-1", email: "i@x.pl", displayName: "Instruktor Test" }];

const lessons: LessonSummary[] = [
  { id: "l1", title: "Lekcja 1", subject: "Scratch", level: "P1", description: "", order: 1, status: "ready", statusLabel: "Gotowa", stepCount: 3, durationMinutes: 20, kind: "standard", kindLabel: "Standardowa grupowa", scheduledDurationMinutes: 95, earlyLeaveAfterMinutes: null },
  { id: "l2", title: "Lekcja 2", subject: "Scratch", level: "P1", description: "", order: 2, status: "ready", statusLabel: "Gotowa", stepCount: 4, durationMinutes: 25, kind: "standard", kindLabel: "Standardowa grupowa", scheduledDurationMinutes: 95, earlyLeaveAfterMinutes: null },
  { id: "l3", title: "Szkic", subject: "Scratch", level: "P1", description: "", order: 3, status: "draft", statusLabel: "Szkic", stepCount: 2, durationMinutes: 10, kind: "standard", kindLabel: "Standardowa grupowa", scheduledDurationMinutes: 95, earlyLeaveAfterMinutes: null },
];

function wrapper({ children }: PropsWithChildren) {
  return <MemoryRouter>{children}</MemoryRouter>;
}

describe("useGroupEditorViewModel", () => {
  beforeEach(() => {
    vi.mocked(groupsApi.getInstructors).mockResolvedValue(instructors);
    vi.mocked(lessonsApi.getLessons).mockResolvedValue(lessons);
    vi.mocked(coursesApi.getCourses).mockResolvedValue([]);
    vi.mocked(schedulingApi.getLocations).mockResolvedValue([]);
    vi.mocked(groupsApi.createGroup).mockResolvedValue({ id: "group-1" } as GroupDetails);
  });

  it("loads instructors and only Ready lessons", async () => {
    const { result } = renderHook(() => useGroupEditorViewModel(), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.instructorId).toBe("inst-1");
    expect(result.current.availableLessons.map((lesson) => lesson.id)).toEqual(["l1", "l2"]);
  });

  it("builds a weekly preview in lesson order", async () => {
    const { result } = renderHook(() => useGroupEditorViewModel(), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setName("Grupa A"));
    act(() => result.current.addLesson("l1"));
    act(() => result.current.addLesson("l2"));
    act(() => result.current.setFirstSessionAt("2026-06-15T16:00"));

    expect(result.current.sessionPreviews).toHaveLength(2);
    expect(result.current.sessionPreviews.map((preview) => preview.sequenceNumber)).toEqual([1, 2]);

    const first = new Date(result.current.sessionPreviews[0].scheduledAtIso).getTime();
    const second = new Date(result.current.sessionPreviews[1].scheduledAtIso).getTime();
    expect(second - first).toBe(7 * 24 * 60 * 60 * 1000);
  });

  it("reorders selected lessons", async () => {
    const { result } = renderHook(() => useGroupEditorViewModel(), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.addLesson("l1"));
    act(() => result.current.addLesson("l2"));
    act(() => result.current.moveLesson(0, 1));

    expect(result.current.selectedLessons.map((lesson) => lesson.id)).toEqual(["l2", "l1"]);
  });

  it("creates the group with ordered lesson ids and picked participants", async () => {
    const jan: ParticipantSummary = { id: "p-1", firstName: "Jan", lastName: "Kowalski", phone: null, email: null, groups: [], ...participantExtras };
    const { result } = renderHook(() => useGroupEditorViewModel(), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setName("Grupa A"));
    act(() => result.current.addLesson("l2"));
    act(() => result.current.addLesson("l1"));
    act(() => result.current.setFirstSessionAt("2026-06-15T16:00"));
    act(() => result.current.pickExistingParticipant(jan));

    await act(async () => result.current.save());

    expect(groupsApi.createGroup).toHaveBeenCalledTimes(1);
    const payload = vi.mocked(groupsApi.createGroup).mock.calls[0][0];
    expect(payload.name).toBe("Grupa A");
    expect(payload.instructorId).toBe("inst-1");
    expect(payload.lessonIds).toEqual(["l2", "l1"]);
    expect(payload.participantIds).toEqual(["p-1"]);
  });

  it("includes the trimmed meeting url in the create payload", async () => {
    const { result } = renderHook(() => useGroupEditorViewModel(), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setName("Grupa online"));
    act(() => result.current.addLesson("l1"));
    act(() => result.current.setFirstSessionAt("2026-06-15T16:00"));
    act(() => result.current.setMeetingUrl("  https://zoom.us/j/123  "));

    await act(async () => result.current.save());

    const calls = vi.mocked(groupsApi.createGroup).mock.calls;
    const payload = calls[calls.length - 1][0];
    expect(payload.meetingUrl).toBe("https://zoom.us/j/123");
  });

  it("creates a brand new participant and adds it to the selection", async () => {
    const created: ParticipantSummary = { id: "p-2", firstName: "Ola", lastName: "Nowak", phone: null, email: null, groups: [], ...participantExtras };
    vi.mocked(participantsApi.createParticipant).mockResolvedValue({
      ...created,
      notes: null,
      guardianEmail: null,
      guardianRelation: null,
      archivedAt: null,
      dataProcessingConsentAt: null,
      imageConsentAt: null,
      createdAt: new Date().toISOString(),
    });

    const { result } = renderHook(() => useGroupEditorViewModel(), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => {
      await result.current.createAndPickParticipant({ ...emptyParticipantDraft(), firstName: "Ola", lastName: "Nowak" });
    });

    expect(result.current.selectedParticipants.map((participant) => participant.id)).toEqual(["p-2"]);
  });
});

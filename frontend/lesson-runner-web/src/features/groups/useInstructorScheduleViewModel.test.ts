import { renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useInstructorScheduleViewModel } from "./useInstructorScheduleViewModel";
import type { ScheduledSession } from "../../types/group";
import * as groupsApi from "../../api/groupsApi";

vi.mock("../../api/groupsApi");

function session(overrides: Partial<ScheduledSession> & Pick<ScheduledSession, "id" | "groupId" | "sequenceNumber" | "scheduledAt" | "status">): ScheduledSession {
  return {
    groupName: overrides.groupId === "g2" ? "Grupa B" : "Grupa A",
    lessonId: `lesson-${overrides.id}`,
    lessonTitle: `Lekcja ${overrides.sequenceNumber}`,
    statusLabel: overrides.status,
    startedAt: null,
    completedAt: null,
    instructorNote: null,
    ...overrides,
  };
}

describe("useInstructorScheduleViewModel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("groups sessions into courses ordered by sequence and picks the current lesson", async () => {
    vi.mocked(groupsApi.getSchedule).mockResolvedValue([
      session({ id: "a2", groupId: "g1", sequenceNumber: 2, scheduledAt: "2026-06-15T16:00:00.000Z", status: "planned" }),
      session({ id: "a1", groupId: "g1", sequenceNumber: 1, scheduledAt: "2026-06-08T16:00:00.000Z", status: "completed" }),
      session({ id: "a3", groupId: "g1", sequenceNumber: 3, scheduledAt: "2026-06-22T16:00:00.000Z", status: "planned" }),
      session({ id: "b1", groupId: "g2", sequenceNumber: 1, scheduledAt: "2026-06-10T16:00:00.000Z", status: "inprogress" }),
    ]);

    const { result } = renderHook(() => useInstructorScheduleViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.courses).toHaveLength(2);

    const courseA = result.current.courses.find((course) => course.groupId === "g1")!;
    expect(courseA.sessions.map((s) => s.id)).toEqual(["a1", "a2", "a3"]); // uporządkowane po sekwencji
    expect(courseA.currentSessionId).toBe("a2"); // najbliższy zaplanowany

    const courseB = result.current.courses.find((course) => course.groupId === "g2")!;
    expect(courseB.currentSessionId).toBe("b1"); // trwający ma pierwszeństwo

    // Najbliższe zajęcia w ogóle: trwający termin przed zaplanowanymi.
    expect(result.current.nextUp?.id).toBe("b1");
  });

  it("has no current lesson when a course is finished", async () => {
    vi.mocked(groupsApi.getSchedule).mockResolvedValue([
      session({ id: "a1", groupId: "g1", sequenceNumber: 1, scheduledAt: "2026-06-08T16:00:00.000Z", status: "completed" }),
      session({ id: "a2", groupId: "g1", sequenceNumber: 2, scheduledAt: "2026-06-15T16:00:00.000Z", status: "cancelled" }),
    ]);

    const { result } = renderHook(() => useInstructorScheduleViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.courses[0].currentSessionId).toBeNull();
    expect(result.current.nextUp).toBeNull();
  });
});

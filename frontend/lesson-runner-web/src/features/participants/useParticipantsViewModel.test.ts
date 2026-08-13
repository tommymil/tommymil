import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useParticipantsViewModel } from "./useParticipantsViewModel";
import type { GroupSummary } from "../../types/group";
import type { ParticipantDetails, ParticipantSummary } from "../../types/participant";
import * as groupsApi from "../../api/groupsApi";
import * as participantsApi from "../../api/participantsApi";

vi.mock("../../api/groupsApi");
vi.mock("../../api/participantsApi");

const groups: GroupSummary[] = [
  { id: "g1", name: "Grupa A", instructorId: "i1", instructorEmail: "i@x.pl", instructorName: "Instruktor Test", participantCount: 0, sessionCount: 0, nextSessionAt: null },
];

const summaryBase = { phone: null, email: null, birthDate: null, guardianName: null, guardianPhone: null, guardianEmail: null, isArchived: false, hasDataConsent: false, hasImageConsent: false, groups: [], hasGuardianAccount: false };
const detailExtras = { notes: null, guardianRelation: null, archivedAt: null, dataProcessingConsentAt: null, imageConsentAt: null, createdAt: "2026-01-01" };
const jan: ParticipantSummary = { id: "p1", firstName: "Jan", lastName: "Kowalski", ...summaryBase };
const ola: ParticipantSummary = { id: "p2", firstName: "Ola", lastName: "Nowak", ...summaryBase };

describe("useParticipantsViewModel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(groupsApi.getGroups).mockResolvedValue(groups);
    vi.mocked(participantsApi.getParticipants).mockResolvedValue([jan, ola]);
  });

  it("loads participants and groups on mount", async () => {
    const { result } = renderHook(() => useParticipantsViewModel());

    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.participants.map((p) => p.id)).toEqual(["p1", "p2"]);
    expect(result.current.groups).toEqual(groups);
  });

  it("creates a participant and adds it to the list", async () => {
    const created: ParticipantDetails = {
      id: "p3",
      firstName: "Ada",
      lastName: "Lovelace",
      ...summaryBase,
      ...detailExtras,
    };
    vi.mocked(participantsApi.createParticipant).mockResolvedValue(created);

    const { result } = renderHook(() => useParticipantsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setCreateField("firstName", "Ada"));
    act(() => result.current.setCreateField("lastName", "Lovelace"));

    await act(async () => {
      const ok = await result.current.createParticipant();
      expect(ok).toBe(true);
    });

    expect(participantsApi.createParticipant).toHaveBeenCalledWith({
      firstName: "Ada",
      lastName: "Lovelace",
      phone: null,
      email: null,
      birthDate: null,
      notes: null,
      guardianName: null,
      guardianPhone: null,
      guardianEmail: null,
      guardianRelation: null,
      consentDataProcessing: false,
      consentImage: false,
      groupIds: undefined,
    });
    expect(result.current.participants.some((p) => p.id === "p3")).toBe(true);
  });

  it("rejects creating a participant without a name", async () => {
    const { result } = renderHook(() => useParticipantsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => {
      const ok = await result.current.createParticipant();
      expect(ok).toBe(false);
    });

    expect(result.current.error).toBeTruthy();
    expect(participantsApi.createParticipant).not.toHaveBeenCalled();
  });

  it("assigns a participant to a group", async () => {
    const updated: ParticipantDetails = { ...jan, ...detailExtras, groups: [{ groupId: "g1", groupName: "Grupa A" }] };
    vi.mocked(participantsApi.enrollParticipant).mockResolvedValue(updated);

    const { result } = renderHook(() => useParticipantsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => {
      const ok = await result.current.assignToGroup("p1", "g1");
      expect(ok).toBe(true);
    });

    expect(participantsApi.enrollParticipant).toHaveBeenCalledWith("p1", "g1");
    expect(result.current.participants.find((p) => p.id === "p1")?.groups).toEqual([{ groupId: "g1", groupName: "Grupa A" }]);
  });

  it("removes a participant from a group", async () => {
    const updated: ParticipantDetails = { ...jan, ...detailExtras, groups: [] };
    vi.mocked(participantsApi.unenrollParticipant).mockResolvedValue(updated);

    const { result } = renderHook(() => useParticipantsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => {
      const ok = await result.current.removeFromGroup("p1", "g1");
      expect(ok).toBe(true);
    });

    expect(participantsApi.unenrollParticipant).toHaveBeenCalledWith("p1", "g1");
  });

  it("deletes a participant and surfaces a conflict error", async () => {
    const { ApiError } = await import("../../api/client");
    vi.mocked(participantsApi.deleteParticipant).mockRejectedValue(new ApiError(409, "Uczestnik ma historię obecności."));

    const { result } = renderHook(() => useParticipantsViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => {
      const ok = await result.current.deleteParticipant("p1");
      expect(ok).toBe(false);
    });

    expect(result.current.error).toBe("Uczestnik ma historię obecności.");
    expect(result.current.participants.some((p) => p.id === "p1")).toBe(true);
  });
});

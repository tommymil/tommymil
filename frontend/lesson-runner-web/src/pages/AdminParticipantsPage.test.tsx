import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AdminParticipantsPage } from "./AdminParticipantsPage";
import type { GroupSummary } from "../types/group";
import type { ParticipantDetails, ParticipantSummary } from "../types/participant";
import * as groupsApi from "../api/groupsApi";
import * as participantsApi from "../api/participantsApi";
import { DialogProvider } from "../features/dialog/DialogContext";
import { ToastProvider } from "../features/toast/ToastContext";

vi.mock("../api/groupsApi");
vi.mock("../api/participantsApi");

const groups: GroupSummary[] = [
  {
    id: "g1",
    name: "Grupa A",
    instructorId: "i1",
    instructorEmail: "i@x.pl",
    instructorName: "Instruktor Test",
    participantCount: 0,
    sessionCount: 0,
    nextSessionAt: null,
  },
];

const jan: ParticipantSummary = {
  id: "p1",
  firstName: "Jan",
  lastName: "Kowalski",
  phone: "111222333",
  email: null,
  birthDate: null,
  guardianName: null,
  guardianPhone: null,
  guardianEmail: null,
  isArchived: false,
  hasDataConsent: true,
  hasImageConsent: false,
  groups: [],
  hasGuardianAccount: false,
};

function renderPage() {
  return render(
    <ToastProvider>
      <DialogProvider>
        <AdminParticipantsPage />
      </DialogProvider>
    </ToastProvider>,
  );
}

describe("AdminParticipantsPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(groupsApi.getGroups).mockResolvedValue(groups);
    vi.mocked(participantsApi.getParticipants).mockResolvedValue([jan]);
  });

  it("lists participants and lets you create a new one", async () => {
    const created: ParticipantDetails = {
      ...jan,
      id: "p2",
      firstName: "Ola",
      lastName: "Nowak",
      phone: null,
      notes: null,
      guardianEmail: null,
      guardianRelation: null,
      archivedAt: null,
      dataProcessingConsentAt: null,
      imageConsentAt: null,
      createdAt: "2026-01-01",
    };
    vi.mocked(participantsApi.createParticipant).mockResolvedValue(created);

    renderPage();

    expect(await screen.findByText("Jan Kowalski")).toBeInTheDocument();

    fireEvent.change(screen.getByRole("textbox", { name: "Imię" }), { target: { value: "Ola" } });
    fireEvent.change(screen.getByRole("textbox", { name: "Nazwisko" }), { target: { value: "Nowak" } });
    fireEvent.click(screen.getByRole("button", { name: "Dodaj uczestnika" }));

    await waitFor(() =>
      expect(participantsApi.createParticipant).toHaveBeenCalledWith({
        firstName: "Ola",
        lastName: "Nowak",
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
      }),
    );
    expect(await screen.findByText("Ola Nowak")).toBeInTheDocument();
  });

  it("assigns a participant to a group", async () => {
    const updated: ParticipantDetails = {
      ...jan,
      groups: [{ groupId: "g1", groupName: "Grupa A" }],
      notes: null,
      guardianEmail: null,
      guardianRelation: null,
      archivedAt: null,
      dataProcessingConsentAt: null,
      imageConsentAt: null,
      createdAt: "2026-01-01",
    };
    vi.mocked(participantsApi.enrollParticipant).mockResolvedValue(updated);

    renderPage();
    await screen.findByText("Jan Kowalski");

    fireEvent.change(screen.getByRole("combobox", { name: "Przypisz do grupy" }), {
      target: { value: "g1" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Przypisz" }));

    await waitFor(() => expect(participantsApi.enrollParticipant).toHaveBeenCalledWith("p1", "g1"));
    expect(await screen.findByText("Grupa A")).toBeInTheDocument();
  });

  it("pyta o potwierdzenie przed usunięciem uczestnika", async () => {
    vi.mocked(participantsApi.deleteParticipant).mockResolvedValue();

    renderPage();
    await screen.findByText("Jan Kowalski");

    fireEvent.click(screen.getByRole("button", { name: "Usuń" }));

    // Samo kliknięcie nie kasuje - najpierw okno z konsekwencjami.
    expect(participantsApi.deleteParticipant).not.toHaveBeenCalled();
    expect(await screen.findByRole("dialog")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Usuń uczestnika" }));

    await waitFor(() => expect(participantsApi.deleteParticipant).toHaveBeenCalledWith("p1"));
  });

  it("shows a conflict error when deleting a participant with attendance history", async () => {
    const { ApiError } = await import("../api/client");
    vi.mocked(participantsApi.deleteParticipant).mockRejectedValue(
      new ApiError(409, "Uczestnik ma zapisaną historię obecności."),
    );

    renderPage();
    await screen.findByText("Jan Kowalski");

    fireEvent.click(screen.getByRole("button", { name: "Usuń" }));
    fireEvent.click(await screen.findByRole("button", { name: "Usuń uczestnika" }));

    expect(await screen.findByText("Uczestnik ma zapisaną historię obecności.")).toBeInTheDocument();
    expect(screen.getByText("Jan Kowalski")).toBeInTheDocument();
  });

  /**
   * Anonimizacja jest nieodwracalna, więc odruch „OK” nie może wystarczyć.
   * Przycisk potwierdzenia zostaje wyłączony, dopóki nazwisko nie zostanie przepisane.
   */
  it("wymaga przepisania nazwiska przy anonimizacji", async () => {
    vi.mocked(participantsApi.anonymizeParticipant).mockResolvedValue();

    renderPage();
    await screen.findByText("Jan Kowalski");

    fireEvent.click(screen.getByRole("button", { name: "Anonimizuj" }));

    const dialog = await screen.findByRole("dialog");
    const confirmButton = within(dialog).getByRole("button", { name: "Zanonimizuj" });
    expect(confirmButton).toBeDisabled();

    fireEvent.change(within(dialog).getByRole("textbox"), { target: { value: "Jan Kowalski" } });
    expect(confirmButton).toBeEnabled();

    fireEvent.click(confirmButton);
    await waitFor(() => expect(participantsApi.anonymizeParticipant).toHaveBeenCalledWith("p1"));
  });
});

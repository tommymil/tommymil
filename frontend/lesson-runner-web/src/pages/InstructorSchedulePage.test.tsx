import { render, screen, waitFor, within } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { InstructorSchedulePage } from "./InstructorSchedulePage";
import type { ScheduledSession } from "../types/group";
import * as groupsApi from "../api/groupsApi";

vi.mock("../api/groupsApi");

/** Data dzisiejsza o 16:00 — pasek dnia liczy w pełnych dniach kalendarzowych. */
function todayAt(hour: number): string {
  const date = new Date();
  date.setHours(hour, 0, 0, 0);
  return date.toISOString();
}

function inDays(days: number): string {
  const date = new Date();
  date.setDate(date.getDate() + days);
  date.setHours(16, 0, 0, 0);
  return date.toISOString();
}

const base = {
  groupId: "g1",
  groupName: "Grupa A",
  startedAt: null,
  completedAt: null,
  instructorNote: null,
};

const sessions: ScheduledSession[] = [
  {
    ...base,
    id: "s1",
    lessonId: "l1",
    lessonTitle: "Lekcja 1",
    scheduledAt: inDays(-7),
    sequenceNumber: 1,
    status: "completed",
    statusLabel: "Zakończone",
    completedAt: inDays(-7),
    instructorNote: "ok",
    unfinishedNote: "Została prezentacja efektów",
  },
  {
    ...base,
    id: "s2",
    lessonId: "l2",
    lessonTitle: "Lekcja 2",
    scheduledAt: todayAt(16),
    sequenceNumber: 2,
    status: "planned",
    statusLabel: "Zaplanowane",
  },
  {
    ...base,
    id: "s3",
    lessonId: "l3",
    lessonTitle: "Lekcja 3",
    scheduledAt: inDays(7),
    sequenceNumber: 3,
    status: "planned",
    statusLabel: "Zaplanowane",
  },
];

function renderPage() {
  return render(
    <MemoryRouter>
      <InstructorSchedulePage />
    </MemoryRouter>,
  );
}

describe("InstructorSchedulePage", () => {
  beforeEach(() => {
    vi.mocked(groupsApi.getSchedule).mockResolvedValue(sessions);
  });

  it("pokazuje pasek dnia z zajęciami na dziś", async () => {
    renderPage();

    const strip = await screen.findByText("Dziś");
    const panel = strip.closest(".today-strip") as HTMLElement;

    expect(within(panel).getByText(/Grupa A — Lekcja 2/)).toBeInTheDocument();
    expect(within(panel).getByRole("link", { name: "Prowadź" })).toHaveAttribute(
      "href",
      "/instructor/sessions/s2",
    );
  });

  it("nie powtarza dzisiejszych zajęć w kaflu „najbliższe”", async () => {
    renderPage();

    await screen.findByText("Dziś");

    // Kafel ma sens tylko wtedy, gdy najbliższy termin nie jest dzisiaj.
    expect(screen.queryByText("Najbliższe zajęcia")).not.toBeInTheDocument();
  });

  it("wyróżnia bieżącą lekcję kursu i zwija przyszłe", async () => {
    renderPage();

    await waitFor(() => expect(screen.getByRole("heading", { name: "Grupa A" })).toBeInTheDocument());

    const card = screen.getByRole("heading", { name: "Grupa A" }).closest(".course-card") as HTMLElement;

    // Oś kursu kończy się na lekcji bieżącej; przyszłe siedzą w osobnym rozwinięciu,
    // więc szukamy w pierwszej liście, a nie w całej karcie — „Otwórz” jest w obu.
    const timeline = card.querySelector(".course-timeline") as HTMLElement;

    // Zakończona lekcja pozostaje dostępna do podglądu.
    expect(within(timeline).getByRole("link", { name: "Otwórz" })).toHaveAttribute(
      "href",
      "/instructor/sessions/s1",
    );

    // Bieżąca lekcja ma inną akcję niż reszta.
    expect(within(timeline).getByRole("link", { name: "Prowadź" })).toHaveAttribute(
      "href",
      "/instructor/sessions/s2",
    );

    // Przyszłe lekcje nie zaśmiecają osi - są pod jednym rozwinięciem.
    expect(screen.getByText(/Kolejne lekcje \(1\)/)).toBeInTheDocument();
  });

  it("pokazuje „czego nie zdążyliśmy” wprost na osi kursu", async () => {
    renderPage();

    expect(await screen.findByText(/Została prezentacja efektów/)).toBeInTheDocument();
  });

  it("pokazuje stan pusty, gdy instruktor nie ma grup", async () => {
    vi.mocked(groupsApi.getSchedule).mockResolvedValue([]);

    renderPage();

    expect(await screen.findByText("Nie masz jeszcze przypisanych zajęć")).toBeInTheDocument();
  });
});

import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { PresenterPage } from "./PresenterPage";
import type { LessonDetails } from "../types/lesson";
import * as lessonsApi from "../api/lessonsApi";
import { ToastProvider } from "../features/toast/ToastContext";

vi.mock("../api/lessonsApi");

const lesson: LessonDetails = {
  id: "lesson-1",
  title: "Pierwsza gra",
  subject: "Scratch",
  level: "Poziom 1",
  description: "Opis",
  order: 1,
  status: "ready",
  statusLabel: "Gotowa",
  stepCount: 1,
  durationMinutes: 8,
  projectFiles: {},
  tags: [],
  steps: [
    {
      id: "step-1",
      order: 1,
      type: "concept",
      title: "Petła zawsze",
      durationMinutes: 8,
      script: ["Weź blok 'zawsze'", "Dodaj 'zmień x o 10'"],
      studentItems: [{ kind: "image", caption: "screen bloczków", url: "/uploads/blocks.png" }],
      resources: [{ kind: "link", label: "Scratch", url: "https://scratch.mit.edu" }],
      notes: [{ kind: "error", text: "Blok ruchu poza pętlą" }],
    },
  ],
};

function renderPresenter() {
  return render(
    <MemoryRouter initialEntries={["/presenter/lesson-1"]}>
      <ToastProvider>
        <Routes>
          <Route path="/presenter/:lessonId" element={<PresenterPage />} />
        </Routes>
      </ToastProvider>
    </MemoryRouter>,
  );
}

describe("PresenterPage (kokpit prowadzenia)", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.mocked(lessonsApi.getLesson).mockResolvedValue(lesson);
    vi.mocked(lessonsApi.startLessonRunSession).mockResolvedValue({
      id: "session-1",
      lessonId: "lesson-1",
      userId: "user-1",
      stepIndex: 0,
      elapsedTotalSeconds: 0,
      elapsedStepSeconds: 0,
      running: false,
      startedAt: "2026-06-13T10:00:00.000Z",
      updatedAt: "2026-06-13T10:00:00.000Z",
    });
    vi.mocked(lessonsApi.updateLessonRunSession).mockResolvedValue({
      id: "session-1",
      lessonId: "lesson-1",
      userId: "user-1",
      stepIndex: 0,
      elapsedTotalSeconds: 0,
      elapsedStepSeconds: 0,
      running: false,
      startedAt: "2026-06-13T10:00:00.000Z",
      updatedAt: "2026-06-13T10:00:00.000Z",
    });
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("shows the 'what to do now' instructions as an ordered list", async () => {
    renderPresenter();

    expect(await screen.findByText("Co robić teraz")).toBeInTheDocument();
    expect(screen.getByText("Weź blok 'zawsze'")).toBeInTheDocument();
    expect(screen.getByText("Dodaj 'zmień x o 10'")).toBeInTheDocument();
  });

  it("tracks the current action and advances the marker on click", async () => {
    renderPresenter();

    const first = await screen.findByText("Weź blok 'zawsze'");
    const second = screen.getByText("Dodaj 'zmień x o 10'");
    expect(first).toHaveAttribute("aria-current", "step");
    expect(second).not.toHaveAttribute("aria-current");

    fireEvent.click(second);

    expect(second).toHaveAttribute("aria-current", "step");
    expect(first.className).toContain("action-done");
  });

  it("supports keyboard activation of an action step", async () => {
    renderPresenter();

    const second = await screen.findByText("Dodaj 'zmień x o 10'");
    fireEvent.keyDown(second, { key: "Enter" });

    expect(second).toHaveAttribute("aria-current", "step");
  });

  it("warns when the session stops syncing, and clears the warning once saving recovers", async () => {
    vi.mocked(lessonsApi.updateLessonRunSession).mockRejectedValueOnce(new Error("network down"));

    renderPresenter();
    await screen.findByText("Co robić teraz");

    fireEvent.click(screen.getByRole("button", { name: "Start" }));

    expect(await screen.findByText(/Nie udało się zapisać postępu lekcji/i)).toBeInTheDocument();
    expect(await screen.findByText("Zapis postępu wstrzymany")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Pauza" }));

    await waitFor(() => expect(screen.queryByText("Zapis postępu wstrzymany")).not.toBeInTheDocument());
  });

  it("renders step images in the screen panel with the API base url", async () => {
    renderPresenter();

    const image = await screen.findByAltText("screen bloczków");
    expect(image).toHaveAttribute("src", "http://localhost:5000/uploads/blocks.png");
    expect(screen.getByText("Na ekranie / wrzutki")).toBeInTheDocument();
  });

  it("opens a centered gallery preview after clicking a screen image", async () => {
    renderPresenter();

    fireEvent.click(await screen.findByRole("button", { name: /screen bloczk/i }));

    const dialog = screen.getByRole("dialog", { name: /screen bloczk/i });
    expect(within(dialog).getByAltText(/screen bloczk/i)).toHaveAttribute(
      "src",
      "http://localhost:5000/uploads/blocks.png",
    );

    fireEvent.click(within(dialog).getByRole("button", { name: "Zamknij podgląd" }));

    expect(screen.queryByRole("dialog", { name: /screen bloczk/i })).not.toBeInTheDocument();
  });

  it("opens the break countdown in a separate window", async () => {
    const write = vi.fn();
    const fakeWindow = {
      document: { open: vi.fn(), write, close: vi.fn() },
      focus: vi.fn(),
    } as unknown as Window;
    const openSpy = vi.spyOn(window, "open").mockReturnValue(fakeWindow);

    renderPresenter();
    fireEvent.click(await screen.findByRole("button", { name: "PRZERWA" }));

    expect(openSpy).toHaveBeenCalledWith("", "lesson-runner-break", expect.stringContaining("width=640"));
    const writtenDocument = write.mock.calls[0][0] as string;
    expect(writtenDocument).toContain("05:00");
    expect(writtenDocument).toContain("Przerwa");
  });

  it("warns with a toast when the browser blocks the break window", async () => {
    vi.spyOn(window, "open").mockReturnValue(null);

    renderPresenter();
    fireEvent.click(await screen.findByRole("button", { name: "PRZERWA" }));

    expect(await screen.findByText(/zablokowała okno przerwy/i)).toBeInTheDocument();
  });
});

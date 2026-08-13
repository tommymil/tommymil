import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { SessionCockpitPage } from "./SessionCockpitPage";
import type { ScheduledSession, SessionAttendance } from "../types/group";
import type { LessonDetails } from "../types/lesson";
import * as groupsApi from "../api/groupsApi";
import * as lessonsApi from "../api/lessonsApi";
import { DialogProvider } from "../features/dialog/DialogContext";
import { ToastProvider } from "../features/toast/ToastContext";

vi.mock("../api/groupsApi");
vi.mock("../api/lessonsApi");

const planned: ScheduledSession = {
  id: "s1",
  groupId: "g1",
  groupName: "Grupa A",
  lessonId: "lesson-1",
  lessonTitle: "Lekcja 1",
  scheduledAt: "2026-06-15T16:00:00.000Z",
  sequenceNumber: 1,
  status: "planned",
  statusLabel: "Zaplanowane",
  startedAt: null,
  completedAt: null,
  instructorNote: null,
};

const attendance: SessionAttendance = {
  sessionId: "s1",
  status: "inprogress",
  statusLabel: "W toku",
  entries: [
    {
      participantId: "p1",
      firstName: "Jan",
      lastName: "Kowalski",
      present: false,
      status: "unexcusedabsence",
      statusLabel: "Nieobecność niezgłoszona",
      liveStatus: "working",
      liveStatusLabel: "Pracuje",
    },
    {
      participantId: "p2",
      firstName: "Ola",
      lastName: "Nowak",
      present: false,
      status: "unexcusedabsence",
      statusLabel: "Nieobecność niezgłoszona",
      liveStatus: "working",
      liveStatusLabel: "Pracuje",
    },
  ],
  makeupOptions: [
    {
      sessionId: "s2",
      groupId: "g2",
      groupName: "Grupa B",
      scheduledAt: "2026-06-22T16:00:00.000Z",
      lessonTitle: "Lekcja 2",
    },
  ],
  statusOptions: [
    { value: "present", label: "Obecny", countsAsPresent: true },
    { value: "late", label: "Spóźniony", countsAsPresent: true },
    { value: "technicalissues", label: "Problemy techniczne", countsAsPresent: true },
    { value: "excusedabsence", label: "Nieobecność zgłoszona", countsAsPresent: false },
    { value: "unexcusedabsence", label: "Nieobecność niezgłoszona", countsAsPresent: false },
  ],
};

const lesson: LessonDetails = {
  id: "lesson-1",
  title: "Lekcja 1",
  subject: "Scratch",
  level: "P1",
  description: "",
  order: 1,
  status: "ready",
  statusLabel: "Gotowa",
  stepCount: 1,
  durationMinutes: 8,
  projectFiles: {},
  tags: [],
  steps: [
    {
      id: "st1",
      order: 1,
      type: "concept",
      title: "Krok",
      durationMinutes: 8,
      script: ["Zrób X"],
      studentItems: [],
      resources: [],
      notes: [],
    },
  ],
};

function renderPage() {
  return render(
    <MemoryRouter initialEntries={["/instructor/sessions/s1"]}>
      <ToastProvider>
        <DialogProvider>
          <Routes>
            <Route path="/instructor/sessions/:sessionId" element={<SessionCockpitPage />} />
          </Routes>
        </DialogProvider>
      </ToastProvider>
    </MemoryRouter>,
  );
}

/**
 * Otwiera kokpit i doprowadza go do stanu „zajęcia w toku”.
 *
 * Czekamy na sterowanie z paska scenariusza, a nie tylko na listę dzieci: `PresenterCockpit`
 * ładuje lekcję asynchronicznie i do tego czasu renderuje sam stan ładowania, bez przycisków.
 */
async function startClass() {
  renderPage();
  fireEvent.click(await screen.findByRole("button", { name: "Start zajęć" }));
  await screen.findByText("Jan Kowalski");
  await screen.findByRole("button", { name: /Ukryj panel grupy/ });
}

/**
 * Wymusza zapis zamiast czekania na autozapis.
 *
 * Przycisk „Zapisz teraz” pojawia się dokładnie wtedy, gdy są niezapisane zmiany, więc
 * jego obecność jest jednocześnie sprawdzeniem, że zmiana w ogóle dotarła do modelu widoku.
 * Sam mechanizm opóźnionego zapisu pokrywa osobny test — tutaj chodzi o **treść żądania**,
 * a przy debounce zależałaby ona od tego, ile czasu minęło między klikanymi kontrolkami.
 */
async function saveNow() {
  fireEvent.click(await screen.findByRole("button", { name: "Zapisz teraz" }));
  await waitFor(() => expect(groupsApi.saveAttendance).toHaveBeenCalled());
}

describe("SessionCockpitPage", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.mocked(groupsApi.getScheduledSession).mockResolvedValue(planned);
    vi.mocked(groupsApi.startSession).mockResolvedValue(attendance);
    vi.mocked(groupsApi.saveAttendance).mockResolvedValue(attendance);
    vi.mocked(groupsApi.setLiveStatus).mockResolvedValue(attendance);
    vi.mocked(groupsApi.finishSession).mockResolvedValue({
      ...planned,
      status: "completed",
      statusLabel: "Zakończone",
      instructorNote: "Dobre zajęcia",
      completedAt: "2026-06-15T17:00:00.000Z",
    });
    vi.mocked(lessonsApi.getLesson).mockResolvedValue(lesson);
    vi.mocked(lessonsApi.startLessonRunSession).mockResolvedValue({
      id: "run-1",
      lessonId: "lesson-1",
      userId: "u1",
      stepIndex: 0,
      elapsedTotalSeconds: 0,
      elapsedStepSeconds: 0,
      running: false,
      startedAt: "x",
      updatedAt: "x",
    });
    vi.mocked(lessonsApi.updateLessonRunSession).mockResolvedValue({
      id: "run-1",
      lessonId: "lesson-1",
      userId: "u1",
      stepIndex: 0,
      elapsedTotalSeconds: 0,
      elapsedStepSeconds: 0,
      running: false,
      startedAt: "x",
      updatedAt: "x",
    });
  });

  it("pokazuje szynę z grupą obok scenariusza po starcie zajęć", async () => {
    await startClass();

    // Szyna stoi obok scenariusza, a nie zamiast niego - krok lekcji zostaje widoczny.
    expect(screen.getByText("Ola Nowak")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Ukryj panel grupy/ })).toBeInTheDocument();
    expect(groupsApi.startSession).toHaveBeenCalledWith("s1");
  });

  it("wysyła znacznik pracy osobnym żądaniem, z pominięciem autozapisu", async () => {
    await startClass();

    const helpButtons = screen.getAllByRole("button", { name: "Pomoc" });
    fireEvent.click(helpButtons[0]);

    await waitFor(() =>
      expect(groupsApi.setLiveStatus).toHaveBeenCalledWith("s1", {
        participantId: "p1",
        liveStatus: "needshelp",
      }),
    );

    // Znacznik nie może przy okazji przepychać całej listy obecności.
    expect(groupsApi.saveAttendance).not.toHaveBeenCalled();
  });

  it("zapisuje obecność sam, bez klikania przycisku zapisu", async () => {
    await startClass();

    fireEvent.click(screen.getByRole("tab", { name: "Obecność" }));
    fireEvent.click(screen.getAllByRole("button", { name: "Obecny" })[0]);

    await waitFor(() => expect(groupsApi.saveAttendance).toHaveBeenCalled(), { timeout: 3000 });

    const entries = vi.mocked(groupsApi.saveAttendance).mock.calls.at(-1)?.[1].entries;
    expect(entries?.find((entry) => entry.participantId === "p1")).toMatchObject({
      present: true,
      status: "present",
    });
  });

  it("odhacza całą grupę jednym kliknięciem", async () => {
    await startClass();

    fireEvent.click(screen.getByRole("tab", { name: "Obecność" }));
    fireEvent.click(screen.getByRole("button", { name: "Zaznacz wszystkich obecnych" }));
    await saveNow();

    const entries = vi.mocked(groupsApi.saveAttendance).mock.calls.at(-1)?.[1].entries;
    expect(entries).toHaveLength(2);
    expect(entries?.every((entry) => entry.present)).toBe(true);
  });

  it("kończy zajęcia trzema polami zamiast jednego", async () => {
    await startClass();

    fireEvent.click(screen.getByRole("button", { name: "Zakończ zajęcia" }));

    fireEvent.change(await screen.findByPlaceholderText(/Co się udało/), {
      target: { value: "Dobre zajęcia" },
    });
    fireEvent.change(screen.getByPlaceholderText(/została prezentacja/), {
      target: { value: "Prezentacja efektów" },
    });
    fireEvent.change(screen.getByPlaceholderText(/po ludzku/), {
      target: { value: "Zosia zrobiła własny tor przeszkód." },
    });

    fireEvent.click(screen.getByRole("button", { name: "Zapisz i zakończ" }));

    await waitFor(() =>
      expect(groupsApi.finishSession).toHaveBeenCalledWith("s1", {
        note: "Dobre zajęcia",
        unfinishedNote: "Prezentacja efektów",
        parentSummary: "Zosia zrobiła własny tor przeszkód.",
      }),
    );
  });

  it("pozwala wrócić do grafiku w trakcie zajęć", async () => {
    await startClass();

    expect(screen.getByRole("link", { name: "Wróć do grafiku" })).toHaveAttribute(
      "href",
      "/instructor/schedule",
    );
  });

  it("zapisuje odrabianie wybrane dla nieobecnego dziecka", async () => {
    await startClass();

    fireEvent.click(screen.getByRole("tab", { name: "Obecność" }));
    // Rzadsze ustawienia chowają się pod „Więcej” - odrabianie jest jednym z nich.
    // Lista jest posortowana po nazwisku, więc pierwszy wiersz to Jan Kowalski (p1).
    fireEvent.click(screen.getAllByRole("button", { name: /Więcej/ })[0]);
    fireEvent.click(screen.getByLabelText("Do odrobienia"));
    fireEvent.change(screen.getByLabelText("Termin odrabiania"), { target: { value: "s2" } });
    await saveNow();

    const entries = vi.mocked(groupsApi.saveAttendance).mock.calls.at(-1)?.[1].entries;
    expect(entries?.find((entry) => entry.participantId === "p1")).toMatchObject({
      makeupRequired: true,
      makeupSessionId: "s2",
    });
  });

  it("pokazuje zamknięte zajęcia w podziale na trzy pola", async () => {
    vi.mocked(groupsApi.getScheduledSession).mockResolvedValue({
      ...planned,
      status: "completed",
      statusLabel: "Zakończone",
      instructorNote: "Uwagi wewnętrzne",
      unfinishedNote: "Nie zdążyliśmy prezentacji",
      // Treść musi się różnić od nagłówka sekcji, inaczej zapytanie trafia w oba naraz.
      parentSummary: "Zosia zrobiła własny tor przeszkód",
    });
    vi.mocked(groupsApi.getAttendance).mockResolvedValue(attendance);

    renderPage();

    expect(await screen.findByText("Uwagi wewnętrzne")).toBeInTheDocument();
    expect(screen.getByText("Nie zdążyliśmy prezentacji")).toBeInTheDocument();
    expect(screen.getByText("Zosia zrobiła własny tor przeszkód")).toBeInTheDocument();
  });
});

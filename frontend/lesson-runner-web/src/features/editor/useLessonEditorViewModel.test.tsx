import type { ReactNode } from "react";
import { act, renderHook, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useLessonEditorViewModel } from "./useLessonEditorViewModel";
import type { LessonDetails, StoredFile } from "../../types/lesson";
import * as lessonsApi from "../../api/lessonsApi";
import { ApiError, uploadFile } from "../../api/client";
import { ToastProvider } from "../toast/ToastContext";

vi.mock("../../api/lessonsApi");

vi.mock("../../api/client", async (importActual) => {
  const actual = await importActual<typeof import("../../api/client")>();
  return { ...actual, uploadFile: vi.fn() };
});

function stored(contentType: string, fileName: string): StoredFile {
  return { url: `/uploads/${fileName}`, fileName, contentType, sizeBytes: 10 };
}

function wrapper({ children }: { children: ReactNode }) {
  return (
    <MemoryRouter>
      <ToastProvider>{children}</ToastProvider>
    </MemoryRouter>
  );
}

const richLesson: LessonDetails = {
  id: "lesson-1",
  title: "Lekcja",
  subject: "Scratch",
  level: "Poziom 1",
  description: "Opis",
  order: 1,
  status: "draft",
  statusLabel: "Szkic",
  stepCount: 2,
  durationMinutes: 9,
  projectFiles: {},
  tags: ["Pętle"],
  steps: [
    {
      id: "step-1",
      order: 1,
      type: "intro",
      title: "Powitanie",
      durationMinutes: 3,
      script: ["Przywitaj się."],
      studentItems: [
        { kind: "text", text: "Cel lekcji" },
        { kind: "image", caption: "gotowa gra" },
      ],
      resources: [{ kind: "link", label: "Scratch", url: "https://scratch.mit.edu" }],
      notes: [
        { kind: "pace", text: "Skróć pokaz." },
        { kind: "hint", text: "Zapisz cele na tablicy." },
      ],
    },
    {
      id: "step-2",
      order: 2,
      type: "concept",
      title: "Petła zawsze",
      durationMinutes: 6,
      script: ["Wyjaśnij pętle."],
      studentItems: [],
      resources: [],
      notes: [],
    },
  ],
};

function addSimpleStep(
  result: { current: ReturnType<typeof useLessonEditorViewModel> },
  title: string,
) {
  act(() => result.current.updateStep("title", title));
  act(() => result.current.commitStep());
}

describe("useLessonEditorViewModel", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(lessonsApi.getLesson).mockResolvedValue(richLesson);
  });

  it("adds a step through commitStep", () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });

    addSimpleStep(result, "Nowy krok");

    expect(result.current.steps).toHaveLength(1);
    expect(result.current.steps[0].title).toBe("Nowy krok");
    expect(result.current.isEditingStep).toBe(false);
  });

  it("edits core fields of a step without dropping its student items, resources or notes", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel("lesson-1"), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.editStep(0));
    expect(result.current.isEditingStep).toBe(true);
    expect(result.current.editingStepIndex).toBe(0);
    expect(result.current.stepForm.title).toBe("Powitanie");

    act(() => result.current.updateStep("title", "Powitanie i cel"));
    act(() => result.current.updateStep("durationMinutes", 5));
    act(() => result.current.commitStep());

    const edited = result.current.steps[0];
    expect(edited.title).toBe("Powitanie i cel");
    expect(edited.durationMinutes).toBe(5);
    // Pod-elementy kroku zostają nietknięte.
    expect(edited.studentItems).toHaveLength(2);
    expect(edited.resources).toEqual([
      { kind: "link", label: "Scratch", url: "https://scratch.mit.edu" },
    ]);
    expect(edited.notes.map((note) => note.kind)).toEqual(["pace", "hint"]);
    expect(result.current.isEditingStep).toBe(false);
  });

  it("collects several student items, notes and resources into one step", () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });

    act(() => result.current.updateStep("title", "Krok bogaty"));

    act(() => result.current.updateStep("studentText", "Cel lekcji"));
    act(() => result.current.addStudentItem());
    act(() => result.current.updateStep("studentText", "Zadanie dla chętnych"));
    act(() => result.current.addStudentItem());

    act(() => result.current.updateStep("noteKind", "error"));
    act(() => result.current.updateStep("noteText", "Częsty błąd uczniów"));
    act(() => result.current.addNote());
    act(() => result.current.updateStep("noteKind", "pace"));
    act(() => result.current.updateStep("noteText", "Skróć pokaz"));
    act(() => result.current.addNote());

    act(() => result.current.updateStep("resourceKind", "link"));
    act(() => result.current.updateStep("resourceUrl", "https://scratch.mit.edu"));
    act(() => result.current.addResource());
    act(() => result.current.updateStep("resourceKind", "code"));
    act(() => result.current.updateStep("resourceCode", "zawsze\n  ruch"));
    act(() => result.current.addResource());

    act(() => result.current.commitStep());

    const step = result.current.steps[0];
    expect(step.studentItems).toHaveLength(2);
    expect(step.notes).toEqual([
      { kind: "error", text: "Częsty błąd uczniów" },
      { kind: "pace", text: "Skróć pokaz" },
    ]);
    expect(step.resources).toHaveLength(2);
    expect(step.resources[0]).toEqual({
      kind: "link",
      label: "https://scratch.mit.edu",
      url: "https://scratch.mit.edu",
    });
    expect(step.resources[1].kind).toBe("code");
    // Po dodaniu krok zostaje wyczyszczony.
    expect(result.current.stepForm.studentItems).toHaveLength(0);
  });

  it("rejects an empty student item, note or link resource", () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });

    act(() => result.current.addStudentItem());
    expect(result.current.error).toBe("Wpisz treść materiału ucznia.");
    expect(result.current.stepForm.studentItems).toHaveLength(0);

    act(() => result.current.addNote());
    expect(result.current.error).toBe("Wpisz treść notatki.");

    act(() => result.current.updateStep("resourceKind", "link"));
    act(() => result.current.addResource());
    expect(result.current.error).toBe("Podaj adres URL zasobu link.");
    expect(result.current.stepForm.resources).toHaveLength(0);
  });

  it("removes a single sub-item while editing without touching the rest", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel("lesson-1"), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.editStep(0));
    expect(result.current.stepForm.notes).toHaveLength(2);

    act(() => result.current.removeNote(0));
    act(() => result.current.commitStep());

    const edited = result.current.steps[0];
    expect(edited.notes).toEqual([{ kind: "hint", text: "Zapisz cele na tablicy." }]);
    expect(edited.studentItems).toHaveLength(2);
  });

  it("appends an uploaded image as an image resource and a pdf as a file resource", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });

    vi.mocked(uploadFile).mockResolvedValueOnce(stored("image/png", "diagram.png"));
    await act(async () => {
      await result.current.uploadResourceFile(new File(["x"], "diagram.png", { type: "image/png" }));
    });

    vi.mocked(uploadFile).mockResolvedValueOnce(stored("application/pdf", "karta.pdf"));
    await act(async () => {
      await result.current.uploadResourceFile(new File(["x"], "karta.pdf", { type: "application/pdf" }));
    });

    expect(result.current.stepForm.resources).toEqual([
      { kind: "image", label: "diagram.png", url: "/uploads/diagram.png" },
      { kind: "file", label: "karta.pdf", url: "/uploads/karta.pdf" },
    ]);
  });

  it("appends an uploaded image as an image student item", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });

    vi.mocked(uploadFile).mockResolvedValueOnce(stored("image/jpeg", "ekran.jpg"));
    await act(async () => {
      await result.current.uploadStudentImage(new File(["x"], "ekran.jpg", { type: "image/jpeg" }));
    });

    expect(result.current.stepForm.studentItems).toEqual([
      { kind: "image", caption: "ekran.jpg", url: "/uploads/ekran.jpg" },
    ]);
  });

  it("flags an unsaved step while the form has content and clears it after commit", () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });

    expect(result.current.hasUnsavedStep).toBe(false);

    act(() => result.current.updateStep("title", "Krok roboczy"));
    expect(result.current.hasUnsavedStep).toBe(true);

    act(() => result.current.commitStep());
    expect(result.current.hasUnsavedStep).toBe(false);
  });

  it("auto-commits a started step with an uploaded image when saving the lesson", async () => {
    vi.mocked(lessonsApi.createLesson).mockResolvedValue(richLesson);
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });

    act(() => result.current.updateMeta("title", "Nowa lekcja"));
    act(() => result.current.updateMeta("description", "Opis lekcji"));
    act(() => result.current.updateStep("title", "Krok z ekranem"));

    vi.mocked(uploadFile).mockResolvedValueOnce(stored("image/png", "ekran.png"));
    await act(async () => {
      await result.current.uploadStudentImage(new File(["x"], "ekran.png", { type: "image/png" }));
    });

    // Zapis lekcji BEZ wcześniejszego "Zapisz krok" - obraz nie może zniknąć.
    await act(async () => {
      await result.current.saveLesson();
    });

    const request = vi.mocked(lessonsApi.createLesson).mock.calls[0][0];
    expect(request.steps).toHaveLength(1);
    expect(request.steps[0].title).toBe("Krok z ekranem");
    expect(request.steps[0].studentItems).toEqual([
      { kind: "image", caption: "ekran.png", url: "/uploads/ekran.png" },
    ]);
    // Formularz kroku zostaje zatwierdzony i wyczyszczony.
    expect(result.current.stepForm.studentItems).toHaveLength(0);
  });

  it("persists an uploaded image before changing status (publish must not drop it)", async () => {
    vi.mocked(lessonsApi.updateLesson).mockResolvedValue(richLesson);
    vi.mocked(lessonsApi.publishLesson).mockResolvedValue({ ...richLesson, status: "ready", statusLabel: "Gotowa" });

    const { result } = renderHook(() => useLessonEditorViewModel("lesson-1"), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    // Edytuj istniejący krok i wgraj obraz - czeka w formularzu, bez kliknięcia "Zapisz".
    act(() => result.current.editStep(0));
    vi.mocked(uploadFile).mockResolvedValueOnce(stored("image/png", "ekran.png"));
    await act(async () => {
      await result.current.uploadStudentImage(new File(["x"], "ekran.png", { type: "image/png" }));
    });

    // Publikacja musi najpierw utrwalić treść z obrazem, a dopiero potem zmienić status.
    await act(async () => {
      await result.current.publish();
    });

    expect(vi.mocked(lessonsApi.updateLesson)).toHaveBeenCalledTimes(1);
    const [, request] = vi.mocked(lessonsApi.updateLesson).mock.calls[0];
    expect(request.steps[0].studentItems).toContainEqual({
      kind: "image",
      caption: "ekran.png",
      url: "/uploads/ekran.png",
    });
    expect(vi.mocked(lessonsApi.publishLesson)).toHaveBeenCalledWith("lesson-1");
  });

  it("blocks saving when a started step has content but no title", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });

    act(() => result.current.updateMeta("title", "Nowa lekcja"));
    act(() => result.current.updateMeta("description", "Opis lekcji"));

    vi.mocked(uploadFile).mockResolvedValueOnce(stored("image/png", "ekran.png"));
    await act(async () => {
      await result.current.uploadStudentImage(new File(["x"], "ekran.png", { type: "image/png" }));
    });

    await act(async () => {
      await result.current.saveLesson();
    });

    expect(result.current.error).toBe('Masz niezapisany krok bez tytułu. Uzupełnij tytuł i kliknij "Zapisz krok".');
    expect(vi.mocked(lessonsApi.createLesson)).not.toHaveBeenCalled();
    // Obraz zostaje w formularzu, żeby nic nie zginęło.
    expect(result.current.stepForm.studentItems).toHaveLength(1);
  });

  it("surfaces an upload error and adds nothing", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });

    vi.mocked(uploadFile).mockRejectedValueOnce(new ApiError(400, "Niedozwolony typ pliku."));
    await act(async () => {
      await result.current.uploadResourceFile(new File(["x"], "x.txt", { type: "text/plain" }));
    });

    expect(result.current.error).toBe("Niedozwolony typ pliku.");
    expect(result.current.stepForm.resources).toHaveLength(0);
  });

  it("edits a note kind and text in place and reorders notes", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel("lesson-1"), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.editStep(0));
    // start: [pace "Skróć pokaz.", hint "Zapisz cele na tablicy."]
    act(() => result.current.updateNoteItem(0, { kind: "error" }));
    act(() => result.current.updateNoteItem(0, { text: "Najczęstszy błąd" }));
    act(() => result.current.moveNoteItem(0, 1));
    act(() => result.current.commitStep());

    expect(result.current.steps[0].notes).toEqual([
      { kind: "hint", text: "Zapisz cele na tablicy." },
      { kind: "error", text: "Najczęstszy błąd" },
    ]);
  });

  it("does not move a note past the boundaries", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel("lesson-1"), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.editStep(0));
    act(() => result.current.moveNoteItem(0, -1));

    expect(result.current.stepForm.notes.map((note) => note.kind)).toEqual(["pace", "hint"]);
  });

  it("edits a student item text and an image caption in place", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel("lesson-1"), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.editStep(0));
    // start: [text "Cel lekcji", image caption "gotowa gra"]
    act(() => result.current.updateStudentItem(0, { text: "Nowy cel" }));
    act(() => result.current.updateStudentItem(1, { caption: "nowy podpis" }));
    act(() => result.current.commitStep());

    expect(result.current.steps[0].studentItems).toEqual([
      { kind: "text", text: "Nowy cel" },
      { kind: "image", caption: "nowy podpis" },
    ]);
  });

  it("edits a resource label and url in place", async () => {
    const { result } = renderHook(() => useLessonEditorViewModel("lesson-1"), { wrapper });
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.editStep(0));
    act(() => result.current.updateResourceItem(0, { label: "Scratch - nowy projekt", url: "https://scratch.mit.edu/projects/editor" }));
    act(() => result.current.commitStep());

    expect(result.current.steps[0].resources).toEqual([
      { kind: "link", label: "Scratch - nowy projekt", url: "https://scratch.mit.edu/projects/editor" },
    ]);
  });

  it("reorders steps and is a no-op at the boundaries", () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });
    addSimpleStep(result, "A");
    addSimpleStep(result, "B");
    addSimpleStep(result, "C");

    act(() => result.current.moveStep(2, -1));
    expect(result.current.steps.map((step) => step.title)).toEqual(["A", "C", "B"]);

    act(() => result.current.moveStep(0, -1));
    expect(result.current.steps.map((step) => step.title)).toEqual(["A", "C", "B"]);
  });

  it("keeps editingStepIndex pointing at the moved step", () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });
    addSimpleStep(result, "A");
    addSimpleStep(result, "B");

    act(() => result.current.editStep(0));
    act(() => result.current.moveStep(0, 1));

    expect(result.current.steps.map((step) => step.title)).toEqual(["B", "A"]);
    expect(result.current.editingStepIndex).toBe(1);
  });

  it("cancels the edit when the edited step is removed", () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });
    addSimpleStep(result, "A");
    addSimpleStep(result, "B");

    act(() => result.current.editStep(1));
    act(() => result.current.removeStep(1));

    expect(result.current.steps.map((step) => step.title)).toEqual(["A"]);
    expect(result.current.isEditingStep).toBe(false);
  });

  it("shifts editingStepIndex when an earlier step is removed", () => {
    const { result } = renderHook(() => useLessonEditorViewModel(undefined), { wrapper });
    addSimpleStep(result, "A");
    addSimpleStep(result, "B");

    act(() => result.current.editStep(1));
    act(() => result.current.removeStep(0));

    expect(result.current.steps.map((step) => step.title)).toEqual(["B"]);
    expect(result.current.editingStepIndex).toBe(0);
  });
});

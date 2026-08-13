import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  createLesson,
  getLesson,
  publishLesson,
  sendLessonToReview,
  updateLesson,
} from "../../api/lessonsApi";
import { ApiError, uploadFile } from "../../api/client";
import { useToast } from "../toast/ToastContext";
import type {
  CreateLessonRequest,
  CreateLessonStepRequest,
  LessonDetails,
  LessonNote,
  LessonProjectFile,
  LessonProjectFiles,
  LessonResource,
  StoredFile,
  StudentItem,
} from "../../types/lesson";

type LessonMetaForm = {
  title: string;
  subject: string;
  level: string;
  description: string;
  tags: string;
  // Puste pole = kolejność zostanie nadana automatycznie (na koniec listy).
  order: string;
  /** Cel dydaktyczny - jedno zdanie dla prowadzącego. */
  objective: string;
  // Trzy listy trzymane jako tekst z podziałem na linie, tak samo jak `script`.
  // Formularz z dynamicznymi wierszami byłby tu cięższy w obsłudze niż pole tekstowe.
  successCriteria: string;
  preparation: string;
  homework: string;
};

type StepForm = {
  type: string;
  title: string;
  durationMinutes: number;
  script: string;
  // Zebrane pod-elementy kroku.
  studentItems: StudentItem[];
  resources: LessonResource[];
  notes: LessonNote[];
  // Pola robocze dla dodawanego właśnie pod-elementu.
  studentText: string;
  noteText: string;
  noteKind: "error" | "hint" | "pace";
  resourceKind: "link" | "code";
  resourceLabel: string;
  resourceUrl: string;
  resourceCode: string;
  resourceLanguage: string;
};

const emptyMeta: LessonMetaForm = {
  title: "",
  subject: "Scratch",
  level: "Poziom 1 - 8-10 lat",
  description: "",
  tags: "",
  order: "",
  objective: "",
  successCriteria: "",
  preparation: "",
  homework: "",
};

const emptyStep: StepForm = {
  type: "concept",
  title: "",
  durationMinutes: 6,
  script: "",
  studentItems: [],
  resources: [],
  notes: [],
  studentText: "",
  noteText: "",
  noteKind: "hint",
  resourceKind: "link",
  resourceLabel: "",
  resourceUrl: "",
  resourceCode: "",
  resourceLanguage: "Scratch",
};

export function useLessonEditorViewModel(lessonId: string | undefined) {
  const navigate = useNavigate();
  const toast = useToast();
  const [lesson, setLesson] = useState<LessonDetails | null>(null);
  const [meta, setMeta] = useState<LessonMetaForm>(emptyMeta);
  const [projectFiles, setProjectFiles] = useState<LessonProjectFiles>({});
  const [stepForm, setStepForm] = useState<StepForm>(emptyStep);
  const [steps, setSteps] = useState<CreateLessonStepRequest[]>([]);
  const [editingIndex, setEditingIndex] = useState<number | null>(null);
  const [uploading, setUploading] = useState(false);
  const [loading, setLoading] = useState(Boolean(lessonId));
  const [saving, setSaving] = useState(false);
  const [statusSaving, setStatusSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!lessonId) {
      return undefined;
    }

    const id = lessonId;
    let ignore = false;

    async function loadLesson() {
      try {
        setLoading(true);
        setError(null);
        const response = await getLesson(id);

        if (!ignore) {
          applyLesson(response);
        }
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać lekcji do edycji.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    void loadLesson();

    return () => {
      ignore = true;
    };
  }, [lessonId]);

  function applyLesson(details: LessonDetails) {
    setLesson(details);
    setEditingIndex(null);
    setStepForm(emptyStep);
    setMeta({
      title: details.title,
      subject: details.subject,
      level: details.level,
      description: details.description,
      tags: details.tags.join(", "),
      order: details.order > 0 ? String(details.order) : "",
      objective: details.objective ?? "",
      successCriteria: (details.successCriteria ?? []).join("\n"),
      preparation: (details.preparation ?? []).join("\n"),
      homework: (details.homework ?? []).join("\n"),
    });
    setProjectFiles(details.projectFiles ?? {});
    setSteps(details.steps.map((step) => ({
      type: step.type,
      title: step.title,
      durationMinutes: step.durationMinutes,
      script: step.script,
      studentItems: step.studentItems,
      resources: step.resources,
      notes: step.notes,
    })));
  }

  function updateMeta(field: keyof LessonMetaForm, value: string) {
    setMeta((current) => ({ ...current, [field]: value }));
  }

  function updateStep(field: keyof StepForm, value: string | number) {
    setStepForm((current) => ({ ...current, [field]: value }));
  }

  function parseScript(value: string): string[] {
    return value
      .split("\n")
      .map((line) => line.trim())
      .filter(Boolean);
  }

  function addStudentItem() {
    const text = stepForm.studentText.trim();

    if (!text) {
      setError("Wpisz treść materiału ucznia.");
      return;
    }

    setStepForm((current) => ({
      ...current,
      studentItems: [...current.studentItems, { kind: "text", text }],
      studentText: "",
    }));
    setError(null);
  }

  function removeStudentItem(index: number) {
    setStepForm((current) => ({
      ...current,
      studentItems: current.studentItems.filter((_, currentIndex) => currentIndex !== index),
    }));
  }

  function addNote() {
    const text = stepForm.noteText.trim();

    if (!text) {
      setError("Wpisz treść notatki.");
      return;
    }

    setStepForm((current) => ({
      ...current,
      notes: [...current.notes, { kind: current.noteKind, text }],
      noteText: "",
    }));
    setError(null);
  }

  function removeNote(index: number) {
    setStepForm((current) => ({
      ...current,
      notes: current.notes.filter((_, currentIndex) => currentIndex !== index),
    }));
  }

  function addResource() {
    let resource: LessonResource;

    if (stepForm.resourceKind === "link") {
      const url = stepForm.resourceUrl.trim();

      if (!url) {
        setError("Podaj adres URL zasobu link.");
        return;
      }

      resource = { kind: "link", label: stepForm.resourceLabel.trim() || url, url };
    } else {
      if (!stepForm.resourceCode.trim()) {
        setError("Wklej kod zasobu.");
        return;
      }

      resource = {
        kind: "code",
        label: stepForm.resourceLabel.trim() || null,
        code: stepForm.resourceCode,
        language: stepForm.resourceLanguage.trim() || "text",
      };
    }

    setStepForm((current) => ({
      ...current,
      resources: [...current.resources, resource],
      resourceLabel: "",
      resourceUrl: "",
      resourceCode: "",
    }));
    setError(null);
  }

  function removeResource(index: number) {
    setStepForm((current) => ({
      ...current,
      resources: current.resources.filter((_, currentIndex) => currentIndex !== index),
    }));
  }

  function updateStudentItem(index: number, patch: Partial<StudentItem>) {
    setStepForm((current) => ({
      ...current,
      studentItems: current.studentItems.map((item, currentIndex) =>
        (currentIndex === index ? { ...item, ...patch } : item)),
    }));
  }

  function moveStudentItem(index: number, direction: -1 | 1) {
    setStepForm((current) => ({
      ...current,
      studentItems: moveInArray(current.studentItems, index, direction),
    }));
  }

  function updateNoteItem(index: number, patch: Partial<LessonNote>) {
    setStepForm((current) => ({
      ...current,
      notes: current.notes.map((note, currentIndex) =>
        (currentIndex === index ? { ...note, ...patch } : note)),
    }));
  }

  function moveNoteItem(index: number, direction: -1 | 1) {
    setStepForm((current) => ({
      ...current,
      notes: moveInArray(current.notes, index, direction),
    }));
  }

  function updateResourceItem(index: number, patch: Partial<LessonResource>) {
    setStepForm((current) => ({
      ...current,
      resources: current.resources.map((resource, currentIndex) =>
        (currentIndex === index ? { ...resource, ...patch } : resource)),
    }));
  }

  function moveResourceItem(index: number, direction: -1 | 1) {
    setStepForm((current) => ({
      ...current,
      resources: moveInArray(current.resources, index, direction),
    }));
  }

  async function upload(file: File): Promise<StoredFile | null> {
    try {
      setUploading(true);
      setError(null);
      return await uploadFile<StoredFile>(file);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się przesłać pliku.");
      return null;
    } finally {
      setUploading(false);
    }
  }

  async function uploadResourceFile(file: File) {
    const stored = await upload(file);

    if (!stored) {
      return;
    }

    // Obraz renderujemy w prezenterze; PDF i inne pliki traktujemy jako zasób do pobrania.
    const kind = stored.contentType.startsWith("image/") ? "image" : "file";

    setStepForm((current) => ({
      ...current,
      resources: [...current.resources, { kind, label: stored.fileName, url: stored.url }],
    }));
  }

  async function uploadStudentImage(file: File) {
    const stored = await upload(file);

    if (!stored) {
      return;
    }

    setStepForm((current) => ({
      ...current,
      studentItems: [...current.studentItems, { kind: "image", caption: stored.fileName, url: stored.url }],
    }));
  }

  async function uploadProjectFile(slot: keyof LessonProjectFiles, file: File) {
    const stored = await upload(file);

    if (!stored) {
      return;
    }

    const fallbackLabel = slot === "starter" ? "Lekcja startowa" : "Lekcja końcowa";

    setProjectFiles((current) => ({
      ...current,
      [slot]: {
        label: current[slot]?.label ?? fallbackLabel,
        url: stored.url,
        fileName: stored.fileName,
        contentType: stored.contentType,
        sizeBytes: stored.sizeBytes,
        downloadToken: current[slot]?.downloadToken ?? null,
      } satisfies LessonProjectFile,
    }));
  }

  function updateProjectFile(slot: keyof LessonProjectFiles, patch: Partial<LessonProjectFile>) {
    setProjectFiles((current) => {
      const existing = current[slot];

      return existing
        ? { ...current, [slot]: { ...existing, ...patch } }
        : current;
    });
  }

  function removeProjectFile(slot: keyof LessonProjectFiles) {
    setProjectFiles((current) => ({ ...current, [slot]: null }));
  }

  function stepFromForm(form: StepForm): CreateLessonStepRequest {
    return {
      type: form.type,
      title: form.title.trim(),
      durationMinutes: Math.max(1, Number(form.durationMinutes) || 1),
      script: parseScript(form.script),
      studentItems: form.studentItems,
      resources: form.resources,
      notes: form.notes,
    };
  }

  // Formularz uznajemy za "rozpoczęty", gdy ma tytuł lub jakąkolwiek zatwierdzoną zawartość
  // (kroki skryptu, wrzutki/obrazy, materiały, wskazówki). Pola robocze typu studentText pomijamy.
  function isStepFormDirty(form: StepForm): boolean {
    return (
      form.title.trim().length > 0 ||
      form.script.trim().length > 0 ||
      form.studentItems.length > 0 ||
      form.resources.length > 0 ||
      form.notes.length > 0
    );
  }

  function mergeStep(
    current: CreateLessonStepRequest[],
    step: CreateLessonStepRequest,
  ): CreateLessonStepRequest[] {
    return editingIndex === null
      ? [...current, step]
      : current.map((existing, currentIndex) => (currentIndex === editingIndex ? step : existing));
  }

  function commitStep() {
    if (!stepForm.title.trim()) {
      setError("Podaj tytuł kroku.");
      return;
    }

    setSteps((current) => mergeStep(current, stepFromForm(stepForm)));
    setEditingIndex(null);
    setStepForm(emptyStep);
    setError(null);
  }

  function editStep(index: number) {
    const step = steps[index];

    if (!step) {
      return;
    }

    setEditingIndex(index);
    setStepForm({
      ...emptyStep,
      type: step.type,
      title: step.title,
      durationMinutes: step.durationMinutes,
      script: step.script.join("\n"),
      studentItems: step.studentItems,
      resources: step.resources,
      notes: step.notes,
    });
    setError(null);
  }

  function cancelStepEdit() {
    setEditingIndex(null);
    setStepForm(emptyStep);
    setError(null);
  }

  function moveStep(index: number, direction: -1 | 1) {
    const target = index + direction;

    setSteps((current) => {
      if (target < 0 || target >= current.length) {
        return current;
      }

      const next = [...current];
      [next[index], next[target]] = [next[target], next[index]];
      return next;
    });

    if (target < 0 || target >= steps.length) {
      return;
    }

    // Utrzymujemy wskaźnik edytowanego kroku przy tym samym kroku po przestawieniu.
    setEditingIndex((current) => {
      if (current === index) {
        return target;
      }

      if (current === target) {
        return index;
      }

      return current;
    });
  }

  function removeStep(index: number) {
    if (editingIndex !== null) {
      if (editingIndex === index) {
        setEditingIndex(null);
        setStepForm(emptyStep);
      } else if (editingIndex > index) {
        setEditingIndex(editingIndex - 1);
      }
    }

    setSteps((current) => current.filter((_, currentIndex) => currentIndex !== index));
  }

  // Pokazuje błąd jednocześnie w panelu pod nagłówkiem i jako popup na górze strony.
  function reportError(message: string) {
    setError(message);
    toast.error(message);
  }

  // Przygotowuje żądanie zapisu z bieżącego stanu edytora, auto-zatwierdzając
  // niezapisany krok z formularza (żeby np. dopiero co wgrany obraz, który czeka
  // jeszcze w formularzu kroku, nie zginął). Zwraca null, gdy walidacja nie przeszła.
  function prepareRequest(): CreateLessonRequest | null {
    let effectiveSteps = steps;

    if (isStepFormDirty(stepForm)) {
      if (!stepForm.title.trim()) {
        reportError('Masz niezapisany krok bez tytułu. Uzupełnij tytuł i kliknij "Zapisz krok".');
        return null;
      }

      effectiveSteps = mergeStep(steps, stepFromForm(stepForm));
      setSteps(effectiveSteps);
      setEditingIndex(null);
      setStepForm(emptyStep);
    }

    return buildRequest(effectiveSteps);
  }

  async function saveLesson() {
    const request = prepareRequest();

    if (!request) {
      return;
    }

    try {
      setSaving(true);
      setError(null);
      const saved = lessonId
        ? await updateLesson(lessonId, request)
        : await createLesson(request);

      applyLesson(saved);
      toast.success(lessonId ? "Zapisano zmiany w lekcji." : "Utworzono lekcję.");

      if (!lessonId) {
        navigate(`/admin/lessons/${saved.id}/edit`);
      }
    } catch {
      reportError("Nie udało się zapisać lekcji.");
    } finally {
      setSaving(false);
    }
  }

  async function markForReview() {
    if (!lesson?.id) {
      reportError("Najpierw zapisz lekcję.");
      return;
    }

    await saveThenChangeStatus(
      lesson.id,
      "Dodaj przynajmniej jeden krok przed wysłaniem do sprawdzenia.",
      "Wysłano lekcję do sprawdzenia.",
      (id) => sendLessonToReview(id),
    );
  }

  async function publish() {
    if (!lesson?.id) {
      reportError("Najpierw zapisz lekcję.");
      return;
    }

    await saveThenChangeStatus(
      lesson.id,
      "Dodaj przynajmniej jeden krok przed publikacją.",
      "Opublikowano lekcję.",
      (id) => publishLesson(id),
    );
  }

  // Zmiana statusu na backendzie operuje na zapisanej wersji lekcji, więc najpierw
  // utrwalamy bieżącą treść edytora (m.in. dopiero co wgrany obraz), a dopiero potem
  // zmieniamy status. Inaczej niezapisane zmiany zostałyby nadpisane wersją z bazy.
  async function saveThenChangeStatus(
    id: string,
    emptyStepsError: string,
    successMessage: string,
    action: (lessonId: string) => Promise<LessonDetails>,
  ) {
    const request = prepareRequest();

    if (!request) {
      return;
    }

    if (request.steps.length === 0) {
      reportError(emptyStepsError);
      return;
    }

    try {
      setStatusSaving(true);
      setError(null);
      await updateLesson(id, request);
      const response = await action(id);
      applyLesson(response);
      toast.success(successMessage);
    } catch {
      reportError("Nie udało się zmienić statusu lekcji.");
    } finally {
      setStatusSaving(false);
    }
  }

  function buildRequest(stepsForRequest: CreateLessonStepRequest[] = steps): CreateLessonRequest | null {
    if (!meta.title.trim()) {
      reportError("Podaj tytuł lekcji.");
      return null;
    }

    if (!meta.description.trim()) {
      reportError("Podaj opis lekcji.");
      return null;
    }

    const trimmedOrder = meta.order.trim();

    return {
      title: meta.title.trim(),
      subject: meta.subject.trim(),
      level: meta.level.trim(),
      description: meta.description.trim(),
      tags: meta.tags.split(",").map((tag) => tag.trim()).filter(Boolean),
      projectFiles,
      steps: stepsForRequest,
      order: trimmedOrder ? Number(trimmedOrder) : undefined,
      objective: meta.objective.trim() || null,
      successCriteria: parseScript(meta.successCriteria),
      preparation: parseScript(meta.preparation),
      homework: parseScript(meta.homework),
    };
  }

  return {
    addNote,
    addResource,
    addStudentItem,
    cancelStepEdit,
    canChangeStatus: Boolean(lesson?.id) && steps.length > 0 && !statusSaving,
    commitStep,
    editStep,
    editingStepIndex: editingIndex,
    error,
    isEditingStep: editingIndex !== null,
    isEditMode: Boolean(lessonId),
    hasUnsavedStep: isStepFormDirty(stepForm),
    lesson,
    loading,
    markForReview,
    meta,
    moveNoteItem,
    moveResourceItem,
    moveStep,
    moveStudentItem,
    publish,
    projectFiles,
    removeNote,
    removeProjectFile,
    removeResource,
    removeStep,
    removeStudentItem,
    saveLesson,
    updateNoteItem,
    updateResourceItem,
    updateStudentItem,
    saving,
    statusSaving,
    stepForm,
    steps,
    totalDuration: steps.reduce((sum, step) => sum + step.durationMinutes, 0),
    updateMeta,
    updateProjectFile,
    updateStep,
    uploading,
    uploadProjectFile,
    uploadResourceFile,
    uploadStudentImage,
  };
}

function moveInArray<TItem>(items: TItem[], index: number, direction: -1 | 1): TItem[] {
  const target = index + direction;

  if (target < 0 || target >= items.length) {
    return items;
  }

  const next = [...items];
  [next[index], next[target]] = [next[target], next[index]];
  return next;
}

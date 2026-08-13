import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createLesson } from "../../api/lessonsApi";
import { parseLessonMarkdown } from "./lessonMarkdown";

export function useLessonImportViewModel() {
  const navigate = useNavigate();
  const [text, setText] = useState("");
  const [importing, setImporting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [acceptErrors, setAcceptErrors] = useState(false);

  const parsed = useMemo(() => (text.trim() ? parseLessonMarkdown(text) : null), [text]);

  const errors = useMemo(
    () => parsed?.issues.filter((issue) => issue.severity === "error") ?? [],
    [parsed],
  );
  const notices = useMemo(
    () => parsed?.issues.filter((issue) => issue.severity === "notice") ?? [],
    [parsed],
  );

  // Zgoda dotyczy konkretnej treści, nie sesji. Po edycji pliku trzeba spojrzeć na listę
  // jeszcze raz - inaczej jedno kliknięcie na początku przepuszczałoby wszystko później.
  useEffect(() => {
    setAcceptErrors(false);
  }, [text]);

  const hasStructure = Boolean(parsed && parsed.lesson.title && parsed.lesson.steps.length > 0);
  const canImport = hasStructure && !importing && (errors.length === 0 || acceptErrors);

  async function loadFile(file: File) {
    try {
      setError(null);
      setText(await file.text());
    } catch {
      setError("Nie udało się wczytać pliku.");
    }
  }

  async function importLesson() {
    if (!parsed || !canImport) {
      return;
    }

    try {
      setImporting(true);
      setError(null);
      const saved = await createLesson(parsed.lesson);
      // Po imporcie otwieramy edytor - tam dorzucasz screeny ze Scratcha.
      navigate(`/admin/lessons/${saved.id}/edit`);
    } catch {
      setError("Nie udało się utworzyć konspektu z importu.");
    } finally {
      setImporting(false);
    }
  }

  return {
    text,
    setText,
    loadFile,
    parsed,
    errors,
    notices,
    acceptErrors,
    setAcceptErrors,
    canImport,
    importLesson,
    importing,
    error,
  };
}

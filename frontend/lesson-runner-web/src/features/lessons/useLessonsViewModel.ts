import { useEffect, useMemo, useState } from "react";
import { deleteLesson, getLessons } from "../../api/lessonsApi";
import { ApiError } from "../../api/client";
import type { LessonKind, LessonStatus, LessonSummary } from "../../types/lesson";

type UseLessonsViewModelOptions = {
  onlyReady?: boolean;
};

type StatusFilter = LessonStatus | "all";
export type LessonSubjectFilter = string;
export type LessonKindFilter = LessonKind | "all";
const defaultSubjectOptions = ["Scratch"];

export function useLessonsViewModel(options: UseLessonsViewModelOptions = {}) {
  const [lessons, setLessons] = useState<LessonSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deletingLessonId, setDeletingLessonId] = useState<string | null>(null);
  const [query, setQuery] = useState("");
  const [subjectFilter, setSubjectFilter] = useState<LessonSubjectFilter>("all");
  const [kindFilter, setKindFilter] = useState<LessonKindFilter>("all");
  const [statusFilter, setStatusFilter] = useState<StatusFilter>(options.onlyReady ? "ready" : "all");

  useEffect(() => {
    let ignore = false;

    async function loadLessons() {
      try {
        setLoading(true);
        setError(null);
        const response = await getLessons();

        if (!ignore) {
          setLessons(response);
        }
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać lekcji z API.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    void loadLessons();

    return () => {
      ignore = true;
    };
  }, []);

  const filteredLessons = useMemo(() => {
    const normalizedQuery = query.trim().toLowerCase();

    return lessons.filter((lesson) => {
      const matchesStatus = statusFilter === "all" || lesson.status === statusFilter;
      const matchesReadyConstraint = !options.onlyReady || lesson.status === "ready";
      const matchesKind = kindFilter === "all" || lesson.kind === kindFilter;
      const matchesSubject = subjectFilter === "all"
        || lesson.subject.toLocaleLowerCase("pl-PL") === subjectFilter.toLocaleLowerCase("pl-PL");
      const matchesQuery = normalizedQuery.length === 0
        || lesson.title.toLowerCase().includes(normalizedQuery)
        || lesson.subject.toLowerCase().includes(normalizedQuery)
        || lesson.description.toLowerCase().includes(normalizedQuery)
        || lesson.level.toLowerCase().includes(normalizedQuery);

      return matchesSubject && matchesStatus && matchesReadyConstraint && matchesKind && matchesQuery;
    });
  }, [kindFilter, lessons, options.onlyReady, query, statusFilter, subjectFilter]);

  const subjectOptions = useMemo(
    () => Array.from(new Set([
      ...defaultSubjectOptions,
      ...lessons.map((lesson) => lesson.subject.trim()).filter(Boolean),
    ]))
      .sort((left, right) => left.localeCompare(right, "pl-PL")),
    [lessons],
  );

  async function removeLesson(id: string) {
    try {
      setDeletingLessonId(id);
      setError(null);
      await deleteLesson(id);
      setLessons((current) => current.filter((lesson) => lesson.id !== id));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się usunąć konspektu.");
    } finally {
      setDeletingLessonId(null);
    }
  }

  return {
    deletingLessonId,
    error,
    kindFilter,
    lessons: filteredLessons,
    lessonCount: filteredLessons.length,
    loading,
    query,
    rawLessonCount: lessons.length,
    removeLesson,
    setQuery,
    setKindFilter,
    setStatusFilter,
    setSubjectFilter,
    showStatusFilter: !options.onlyReady,
    statusFilter,
    subjectFilter,
    subjectOptions,
  };
}

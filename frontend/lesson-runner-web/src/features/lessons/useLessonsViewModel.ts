import { useEffect, useMemo, useState } from "react";
import { deleteLesson, getLessons } from "../../api/lessonsApi";
import { ApiError } from "../../api/client";
import type { LessonStatus, LessonSummary } from "../../types/lesson";

type UseLessonsViewModelOptions = {
  onlyReady?: boolean;
};

type StatusFilter = LessonStatus | "all";
export type LessonSubjectFilter = "Scratch" | "Minecraft";

export const lessonSubjectFilters: LessonSubjectFilter[] = ["Scratch", "Minecraft"];

export function useLessonsViewModel(options: UseLessonsViewModelOptions = {}) {
  const [lessons, setLessons] = useState<LessonSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deletingLessonId, setDeletingLessonId] = useState<string | null>(null);
  const [query, setQuery] = useState("");
  const [subjectFilter, setSubjectFilter] = useState<LessonSubjectFilter>("Scratch");
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
      const matchesSubject = lesson.subject.toLowerCase() === subjectFilter.toLowerCase();
      const matchesQuery = normalizedQuery.length === 0
        || lesson.title.toLowerCase().includes(normalizedQuery)
        || lesson.subject.toLowerCase().includes(normalizedQuery)
        || lesson.description.toLowerCase().includes(normalizedQuery)
        || lesson.level.toLowerCase().includes(normalizedQuery);

      return matchesSubject && matchesStatus && matchesReadyConstraint && matchesQuery;
    });
  }, [lessons, options.onlyReady, query, statusFilter, subjectFilter]);

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
    lessons: filteredLessons,
    lessonCount: filteredLessons.length,
    loading,
    query,
    rawLessonCount: lessons.length,
    removeLesson,
    setQuery,
    setStatusFilter,
    setSubjectFilter,
    showStatusFilter: !options.onlyReady,
    statusFilter,
    subjectFilter,
  };
}

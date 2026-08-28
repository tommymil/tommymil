import type { LessonStatus } from "../../../types/lesson";
import type { LessonKindFilter, LessonSubjectFilter } from "../useLessonsViewModel";

type LessonsToolbarProps = {
  kindFilter: LessonKindFilter;
  query: string;
  setKindFilter: (value: LessonKindFilter) => void;
  setQuery: (value: string) => void;
  setStatusFilter: (value: LessonStatus | "all") => void;
  setSubjectFilter: (value: LessonSubjectFilter) => void;
  showStatusFilter: boolean;
  showKindFilter: boolean;
  statusFilter: LessonStatus | "all";
  subjectFilter: LessonSubjectFilter;
  subjectOptions: string[];
};

export function LessonsToolbar({
  kindFilter,
  query,
  setKindFilter,
  setQuery,
  setStatusFilter,
  setSubjectFilter,
  showStatusFilter,
  showKindFilter,
  statusFilter,
  subjectFilter,
  subjectOptions,
}: LessonsToolbarProps) {
  return (
    <div className="lessons-toolbar">
      {showKindFilter ? (
        <label>
          <span>Rodzaj lekcji</span>
          <select
            value={kindFilter}
            onChange={(event) => setKindFilter(event.target.value as LessonKindFilter)}
          >
            <option value="all">Wszystkie rodzaje</option>
            <option value="standard">Zwykłe — 95 min</option>
            <option value="showcase">Pokazowe — 60 min</option>
          </select>
        </label>
      ) : null}
      <label>
        <span>Technologia</span>
        <select
          value={subjectFilter}
          onChange={(event) => setSubjectFilter(event.target.value as LessonSubjectFilter)}
        >
          <option value="all">Wszystkie technologie</option>
          {subjectOptions.map((subject) => (
            <option key={subject} value={subject}>
              {subject}
            </option>
          ))}
        </select>
      </label>
      <label>
        <span>Szukaj</span>
        <input
          value={query}
          onChange={(event) => setQuery(event.target.value)}
          placeholder="Tytuł, przedmiot, opis"
        />
      </label>
      {showStatusFilter ? (
        <label>
          <span>Status</span>
          <select
            value={statusFilter}
            onChange={(event) => setStatusFilter(event.target.value as LessonStatus | "all")}
          >
            <option value="all">Wszystkie</option>
            <option value="draft">Szkic</option>
            <option value="review">Do sprawdzenia</option>
            <option value="ready">Gotowa</option>
          </select>
        </label>
      ) : null}
    </div>
  );
}

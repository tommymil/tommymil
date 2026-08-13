import type { LessonStatus } from "../../../types/lesson";
import { lessonSubjectFilters } from "../useLessonsViewModel";
import type { LessonSubjectFilter } from "../useLessonsViewModel";

type LessonsToolbarProps = {
  query: string;
  setQuery: (value: string) => void;
  setStatusFilter: (value: LessonStatus | "all") => void;
  setSubjectFilter: (value: LessonSubjectFilter) => void;
  showStatusFilter: boolean;
  statusFilter: LessonStatus | "all";
  subjectFilter: LessonSubjectFilter;
};

export function LessonsToolbar({
  query,
  setQuery,
  setStatusFilter,
  setSubjectFilter,
  showStatusFilter,
  statusFilter,
  subjectFilter,
}: LessonsToolbarProps) {
  return (
    <div className="lessons-toolbar">
      <label>
        <span>Technologia</span>
        <select
          value={subjectFilter}
          onChange={(event) => setSubjectFilter(event.target.value as LessonSubjectFilter)}
        >
          {lessonSubjectFilters.map((subject) => (
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

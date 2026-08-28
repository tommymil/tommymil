import { LessonCard } from "../features/lessons/components/LessonCard";
import { LessonsState } from "../features/lessons/components/LessonsState";
import { LessonsToolbar } from "../features/lessons/components/LessonsToolbar";
import { useLessonsViewModel } from "../features/lessons/useLessonsViewModel";

export function InstructorLessonsPage() {
  const viewModel = useLessonsViewModel({ onlyReady: true });

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Instruktor</span>
          <h1>Wybierz lekcje do prowadzenia</h1>
          <p>Na tym ekranie będą widoczne tylko lekcje oznaczone jako gotowe.</p>
        </div>
      </div>
      {!viewModel.loading && !viewModel.error ? (
        <LessonsToolbar
          kindFilter={viewModel.kindFilter}
          query={viewModel.query}
          setKindFilter={viewModel.setKindFilter}
          setQuery={viewModel.setQuery}
          setStatusFilter={viewModel.setStatusFilter}
          setSubjectFilter={viewModel.setSubjectFilter}
          showStatusFilter={viewModel.showStatusFilter}
          showKindFilter
          statusFilter={viewModel.statusFilter}
          subjectFilter={viewModel.subjectFilter}
          subjectOptions={viewModel.subjectOptions}
        />
      ) : null}
      {viewModel.loading ? <LessonsState message="Ładowanie lekcji…" /> : null}
      {viewModel.error ? <LessonsState message={viewModel.error} tone="error" /> : null}
      {!viewModel.loading && !viewModel.error && viewModel.rawLessonCount === 0 ? (
        <LessonsState message="Brak lekcji w bibliotece." />
      ) : null}
      {!viewModel.loading && !viewModel.error && viewModel.rawLessonCount > 0 && viewModel.lessons.length === 0 ? (
        <LessonsState message="Brak gotowych lekcji pasujących do wybranych filtrów." />
      ) : null}
      {!viewModel.loading && !viewModel.error && viewModel.lessons.length > 0 ? (
        <div className="lesson-grid">
          {viewModel.lessons.map((lesson) => (
            <LessonCard key={lesson.id} lesson={lesson} />
          ))}
        </div>
      ) : null}
    </section>
  );
}

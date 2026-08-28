import { LessonCard } from "../features/lessons/components/LessonCard";
import { LessonsState } from "../features/lessons/components/LessonsState";
import { LessonsToolbar } from "../features/lessons/components/LessonsToolbar";
import { useLessonsViewModel } from "../features/lessons/useLessonsViewModel";
import { useDialogs } from "../features/dialog/DialogContext";
import { useToast } from "../features/toast/ToastContext";

export function AdminLibraryPage() {
  const viewModel = useLessonsViewModel();
  const { confirm } = useDialogs();
  const toast = useToast();

  async function handleDeleteLesson(lessonTitle: string, lessonId: string) {
    const confirmed = await confirm({
      title: "Usunąć konspekt?",
      description: `„${lessonTitle}” zniknie z biblioteki. Tej operacji nie można cofnąć.`,
      tone: "danger",
      confirmLabel: "Usuń konspekt",
      consequences: [
        "Grupy z tym konspektem stracą przypisany materiał na swoich terminach.",
        "Pliki projektu dołączone do lekcji przestaną być dostępne dla rodziców.",
      ],
    });

    if (!confirmed) {
      return;
    }

    await viewModel.removeLesson(lessonId);
    toast.success("Konspekt został usunięty.");
  }

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Administrator</span>
          <h1>Biblioteka lekcji</h1>
          <p>
            {viewModel.lessonCount} z {viewModel.rawLessonCount} konspektów widocznych po filtrach.
          </p>
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
          showKindFilter={false}
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
        <LessonsState message="Brak lekcji pasujących do filtrów." />
      ) : null}
      {!viewModel.loading && !viewModel.error && viewModel.lessons.length > 0 ? (
        <div className="lesson-grid">
          {viewModel.lessons.map((lesson) => (
            <LessonCard
              key={lesson.id}
              canManage
              deleting={viewModel.deletingLessonId === lesson.id}
              lesson={lesson}
              onDelete={() => void handleDeleteLesson(lesson.title, lesson.id)}
            />
          ))}
        </div>
      ) : null}
    </section>
  );
}

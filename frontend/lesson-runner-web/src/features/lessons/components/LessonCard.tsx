import { Link } from "react-router-dom";
import { Button } from "../../../components/ui/Button";
import type { LessonSummary } from "../../../types/lesson";

type LessonCardProps = {
  canManage?: boolean;
  deleting?: boolean;
  lesson: LessonSummary;
  onDelete?: (lesson: LessonSummary) => void;
};

export function LessonCard({ canManage = false, deleting = false, lesson, onDelete }: LessonCardProps) {
  const canRun = lesson.status === "ready";

  return (
    <article className="lesson-card">
      <div className="lesson-card-header">
        <div className="lesson-card-header-left">
          {lesson.order > 0 ? <span className="order-pill">#{lesson.order}</span> : null}
          <span className="subject-pill">{lesson.subject}</span>
        </div>
        <span className={`status-pill status-${lesson.status}`}>{lesson.statusLabel}</span>
      </div>
      <div>
        <h2>{lesson.title}</h2>
        <p>{lesson.description}</p>
      </div>
      <div className="lesson-meta">
        <span>{lesson.level}</span>
        <span>{lesson.stepCount} kroków</span>
        <span>{lesson.durationMinutes} min</span>
      </div>
      <div className="lesson-actions">
        <div className="lesson-action-primary">
          {canRun ? (
            <Link to={`/presenter/${lesson.id}`}>
              <Button>Prowadź lekcję</Button>
            </Link>
          ) : (
            <Button variant="secondary" disabled>
              Prowadź lekcję
            </Button>
          )}
        </div>
        {canManage ? (
          <div className="lesson-action-admin">
            <Link to={`/admin/lessons/${lesson.id}/edit`}>
              <Button variant="secondary">Edytuj</Button>
            </Link>
            <Button
              variant="secondary"
              disabled={deleting}
              onClick={() => onDelete?.(lesson)}
            >
              {deleting ? "Usuwanie..." : "Usuń"}
            </Button>
          </div>
        ) : null}
      </div>
    </article>
  );
}

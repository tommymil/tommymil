import { Link } from "react-router-dom";
import { CalendarClock, CalendarDays, Play, Video } from "lucide-react";
import { exportScheduleIcs } from "../api/groupsApi";
import { saveDownloadedFile } from "../api/client";
import { Button } from "../components/ui/Button";
import { EmptyState } from "../components/ui/EmptyState";
import { SkeletonList } from "../components/ui/Skeleton";
import { StatusBadge, sessionStatusTone } from "../components/ui/StatusBadge";
import { daysFromToday, formatDateTime, formatFriendlyDateTime, formatTime, plural } from "../features/groups/datetime";
import { useInstructorScheduleViewModel } from "../features/groups/useInstructorScheduleViewModel";
import type { CourseSchedule } from "../features/groups/useInstructorScheduleViewModel";
import type { ScheduledSession } from "../types/group";

function lessonState(session: ScheduledSession, isCurrent: boolean): "is-current" | "is-done" | "is-future" {
  if (isCurrent) {
    return "is-current";
  }

  return session.status === "completed" || session.status === "cancelled" ? "is-done" : "is-future";
}

/**
 * Grafik instruktora.
 *
 * Poprzednia wersja była listą kursów z jednym zdaniem instrukcji obsługi nad nią
 * („Podświetlona jest lekcja na te zajęcia...”). Jeśli listę trzeba objaśniać zdaniem,
 * lista jest źle zaprojektowana. Instruktor rano pyta też o co innego niż o przebieg
 * swoich kursów: pyta, co ma dzisiaj i o której — stąd pasek dnia nad resztą.
 */
export function InstructorSchedulePage() {
  const vm = useInstructorScheduleViewModel();

  const today = vm.courses
    .flatMap((course) => course.sessions)
    .filter((session) => daysFromToday(session.scheduledAt) === 0)
    .filter((session) => session.status !== "cancelled")
    .sort((first, second) => first.scheduledAt.localeCompare(second.scheduledAt));

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Instruktor</span>
          <h1>Mój grafik</h1>
          <p>Dziś, najbliższe zajęcia i pełna ścieżka lekcji w każdym kursie.</p>
        </div>
        <div className="page-header-actions">
          <Button variant="secondary" onClick={async () => saveDownloadedFile(await exportScheduleIcs())}>
            <CalendarDays className="button-icon" aria-hidden="true" />
            Dodaj do kalendarza
          </Button>
        </div>
      </div>

      {vm.loading ? <SkeletonList rows={3} label="Ładowanie grafiku" /> : null}
      {vm.error ? (
        <div className="list-state list-state-error" role="alert">
          {vm.error}
        </div>
      ) : null}

      {!vm.loading && !vm.error && vm.courses.length === 0 ? (
        <EmptyState
          icon={CalendarClock}
          title="Nie masz jeszcze przypisanych zajęć"
          description="Gdy administrator przypisze Cię do grupy, jej terminy pojawią się tutaj."
        />
      ) : null}

      {!vm.loading && vm.courses.length > 0 ? (
        <div className="today-strip">
          <div className="today-strip-head">
            <h2>Dziś</h2>
            <span className="cue-empty">
              {today.length === 0
                ? "Brak zajęć"
                : `${today.length} ${plural(today.length, "zajęcia", "zajęcia", "zajęć")}`}
            </span>
          </div>

          {today.length === 0 ? (
            <p className="cue-empty">Dziś nic nie prowadzisz. Najbliższe zajęcia znajdziesz niżej.</p>
          ) : (
            <div className="today-list">
              {today.map((session) => (
                <div className={`today-row${session.status === "inprogress" ? " is-now" : ""}`} key={session.id}>
                  <span className="today-time">{formatTime(session.scheduledAt)}</span>
                  <div className="today-main">
                    <strong>
                      {session.groupName} — {session.lessonTitle ?? "(brak lekcji)"}
                    </strong>
                    <small>lekcja {session.sequenceNumber}</small>
                  </div>
                  <StatusBadge label={session.statusLabel} tone={sessionStatusTone(session.status)} dot />
                  <div className="today-actions">
                    {session.meetingUrl ? (
                      <a href={session.meetingUrl} target="_blank" rel="noreferrer">
                        <Button variant="secondary">
                          <Video className="button-icon" aria-hidden="true" />
                          Dołącz
                        </Button>
                      </a>
                    ) : null}
                    <Link to={`/instructor/sessions/${session.id}`}>
                      <Button>
                        <Play className="button-icon" aria-hidden="true" />
                        Prowadź
                      </Button>
                    </Link>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      ) : null}

      {/* Kafel „najbliższe” ma sens tylko wtedy, gdy nie są to zajęcia z dzisiaj —
          inaczej powtarzałby pierwszą pozycję paska dnia. */}
      {vm.nextUp && daysFromToday(vm.nextUp.scheduledAt) !== 0 ? (
        <div className="next-up-card">
          <div className="next-up-icon" aria-hidden="true">
            <CalendarClock size={22} />
          </div>
          <div className="next-up-main">
            <span className="eyebrow">Najbliższe zajęcia</span>
            <strong>
              {vm.nextUp.groupName} — {vm.nextUp.lessonTitle ?? "(brak lekcji)"}
            </strong>
            <span>{formatFriendlyDateTime(vm.nextUp.scheduledAt)}</span>
          </div>
          <div className="next-up-actions">
            <Link to={`/instructor/sessions/${vm.nextUp.id}`}>
              <Button variant="secondary">Otwórz kokpit</Button>
            </Link>
          </div>
        </div>
      ) : null}

      {vm.courses.map((course) => (
        <CourseCard key={course.groupId} course={course} />
      ))}
    </section>
  );
}

/**
 * Przebieg kursu.
 *
 * Zrealizowane lekcje są wyszarzone, bieżąca wyróżniona kolorem i przyciskiem „Prowadź”,
 * a przyszłe zwinięte pod jednym rozwinięciem. Wcześniej wszystkie wyglądały tak samo,
 * więc nad listą musiało stać zdanie tłumaczące, jak ją czytać.
 */
function CourseCard({ course }: { course: CourseSchedule }) {
  const meetingUrl = course.sessions.find((session) => session.meetingUrl)?.meetingUrl ?? null;
  const currentIndex = course.sessions.findIndex((session) => session.id === course.currentSessionId);
  const upcoming = currentIndex >= 0 ? course.sessions.slice(currentIndex + 1) : [];
  const visible = currentIndex >= 0 ? course.sessions.slice(0, currentIndex + 1) : course.sessions;

  return (
    <div className="course-card">
      <div className="course-head">
        <h2>{course.groupName}</h2>
        {meetingUrl ? (
          <a className="course-meeting-link" href={meetingUrl} target="_blank" rel="noreferrer">
            <Video size={15} aria-hidden="true" />
            Dołącz do spotkania online
          </a>
        ) : null}
      </div>

      <ol className="course-timeline">
        {visible.map((session) => (
          <CourseLesson key={session.id} session={session} isCurrent={session.id === course.currentSessionId} />
        ))}
      </ol>

      {upcoming.length > 0 ? (
        <details className="course-upcoming">
          <summary>Kolejne lekcje ({upcoming.length}) — otwórz, żeby się przygotować</summary>
          <ol className="course-timeline">
            {upcoming.map((session) => (
              <CourseLesson key={session.id} session={session} isCurrent={false} />
            ))}
          </ol>
        </details>
      ) : null}
    </div>
  );
}

function CourseLesson({ session, isCurrent }: { session: ScheduledSession; isCurrent: boolean }) {
  return (
    <li className={`course-lesson ${lessonState(session, isCurrent)}`}>
      <span className="course-seq" aria-hidden="true">
        {session.sequenceNumber}
      </span>
      <div className="course-lesson-main">
        <strong>{session.lessonTitle ?? "(brak lekcji)"}</strong>
        <span>
          {formatDateTime(session.scheduledAt)} · {session.statusLabel}
        </span>
        {/* „Czego nie zdążyliśmy” z zakończonych zajęć. Wcześniej ta informacja tkwiła
            w środku notatki instruktora i trzeba było po nią wejść w zamknięty termin. */}
        {session.unfinishedNote ? <span className="course-unfinished">Zostało: {session.unfinishedNote}</span> : null}
      </div>
      <Link to={`/instructor/sessions/${session.id}`}>
        <Button variant={isCurrent ? "primary" : "secondary"}>{isCurrent ? "Prowadź" : "Otwórz"}</Button>
      </Link>
    </li>
  );
}

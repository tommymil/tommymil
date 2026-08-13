import { ArrowDown, ArrowUp, Trash2, X } from "lucide-react";
import { Link } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { formatDateTime, plural } from "../features/groups/datetime";
import { useGroupEditorViewModel } from "../features/groups/useGroupEditorViewModel";
import { ParticipantPicker } from "../features/participants/ParticipantPicker";

export function GroupEditorPage() {
  const vm = useGroupEditorViewModel();

  if (vm.loading) {
    return (
      <section className="page-section">
        <div className="list-state">Ładowanie danych…</div>
      </section>
    );
  }

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Administrator</span>
          <h1>Nowa grupa</h1>
          <p>Wybierz instruktora, program zajęć i datę startu. Terminy powstaną co tydzień.</p>
        </div>
        <div className="page-header-actions">
          <Link to="/admin/groups">
            <Button variant="secondary">Wróć</Button>
          </Link>
        </div>
      </div>

      {vm.error ? <div className="list-state list-state-error">{vm.error}</div> : null}

      <div className="editor-grid">
        <div className="editor-main">
          <section className="editor-fieldset">
            <legend>Podstawy</legend>
            <label className="form-field">
              <span>Nazwa grupy</span>
              <input
                value={vm.name}
                onChange={(event) => vm.setName(event.target.value)}
                placeholder="np. Poniedziałek 16:00 - Scratch A"
              />
            </label>
            <label className="form-field">
              <span>Instruktor</span>
              <select value={vm.instructorId} onChange={(event) => vm.setInstructorId(event.target.value)}>
                {vm.instructors.length === 0 ? <option value="">Brak instruktorów</option> : null}
                {vm.instructors.map((instructor) => (
                  <option key={instructor.id} value={instructor.id}>
                    {instructor.displayName}
                  </option>
                ))}
              </select>
            </label>
            <label className="form-field">
              <span>Pierwsze zajęcia (data i godzina)</span>
              <input type="datetime-local" value={vm.firstSessionAt} onChange={(event) => vm.setFirstSessionAt(event.target.value)} />
            </label>
            <label className="form-field">
              <span>Limit miejsc</span>
              <input
                type="number"
                min={1}
                value={vm.capacity}
                onChange={(event) => vm.setCapacity(event.target.value)}
                placeholder="Bez limitu"
              />
            </label>
            <label className="form-field">
              <span>Link do spotkania online</span>
              <input
                type="url"
                value={vm.meetingUrl}
                onChange={(event) => vm.setMeetingUrl(event.target.value)}
                placeholder="https://zoom.us/j/... lub https://meet.google.com/..."
              />
            </label>
          </section>

          <section className="editor-fieldset">
            <legend>Program zajęć</legend>
            <label className="form-field">
              <span>Utwórz z kursu</span>
              <select value={vm.selectedCourseId} onChange={(event) => void vm.selectCourse(event.target.value)}>
                <option value="">Bez kursu - ręczny wybór lekcji</option>
                {vm.courses.map((course) => (
                  <option key={course.id} value={course.id}>
                    {course.name} ({course.lessonCount} {plural(course.lessonCount, "lekcja", "lekcje", "lekcji")})
                  </option>
                ))}
              </select>
            </label>

            {vm.selectedCourse ? (
              <p className="cue-empty">
                Kurs: {vm.selectedCourse.subject} · {vm.selectedCourse.level}. Sekwencja lekcji jest pobierana z kursu.
              </p>
            ) : null}

            {!vm.selectedCourseId ? (
              <div className="form-field">
                <span>Dodaj lekcję (tylko opublikowane)</span>
                <select
                  value=""
                  onChange={(event) => vm.addLesson(event.target.value)}
                  disabled={vm.availableLessons.length === 0}
                >
                  <option value="">{vm.availableLessons.length === 0 ? "Brak dostępnych lekcji Ready" : "Wybierz lekcję…"}</option>
                  {vm.availableLessons.map((lesson) => (
                    <option key={lesson.id} value={lesson.id}>
                      {lesson.title} ({lesson.subject})
                    </option>
                  ))}
                </select>
              </div>
            ) : null}

            {vm.selectedLessons.length === 0 ? (
              <p className="cue-empty">Nie wybrano jeszcze żadnej lekcji.</p>
            ) : (
              <ol className="ordered-list">
                {vm.selectedLessons.map((lesson, index) => (
                  <li key={lesson.id} className="ordered-row">
                    <span className="ordered-title">{lesson.title}</span>
                    <span className="sub-controls">
                      <button type="button" onClick={() => vm.moveLesson(index, -1)} disabled={index === 0 || Boolean(vm.selectedCourseId)} aria-label="W górę">
                        <ArrowUp size={14} aria-hidden="true" />
                      </button>
                      <button
                        type="button"
                        onClick={() => vm.moveLesson(index, 1)}
                        disabled={index === vm.selectedLessons.length - 1 || Boolean(vm.selectedCourseId)}
                        aria-label="W dół"
                      >
                        <ArrowDown size={14} aria-hidden="true" />
                      </button>
                      <button type="button" onClick={() => vm.removeLesson(lesson.id)} disabled={Boolean(vm.selectedCourseId)} aria-label="Usuń">
                        <Trash2 size={14} aria-hidden="true" />
                      </button>
                    </span>
                  </li>
                ))}
              </ol>
            )}
          </section>

          <section className="editor-fieldset">
            <legend>Uczestnicy</legend>

            {vm.selectedParticipants.length === 0 ? (
              <p className="cue-empty">Nie wybrano jeszcze żadnego uczestnika.</p>
            ) : (
              <div className="participant-groups">
                {vm.selectedParticipants.map((participant) => (
                  <span key={participant.id} className="chip">
                    {participant.firstName} {participant.lastName}
                    <button
                      type="button"
                      aria-label={`Usuń ${participant.firstName} ${participant.lastName} z listy`}
                      onClick={() => vm.removeSelectedParticipant(participant.id)}
                    >
                      <X size={13} aria-hidden="true" />
                    </button>
                  </span>
                ))}
              </div>
            )}

            <ParticipantPicker
              excludeIds={vm.selectedParticipants.map((participant) => participant.id)}
              onPickExisting={vm.pickExistingParticipant}
              onCreateNew={vm.createAndPickParticipant}
            />
          </section>
        </div>

        <aside className="editor-side">
          <div className="editor-summary">
            <h3>Podgląd terminów</h3>
            {vm.sessionPreviews.length === 0 ? (
              <p className="cue-empty">Wybierz program i datę startu, aby zobaczyć terminy.</p>
            ) : (
              <ol className="preview-list">
                {vm.sessionPreviews.map((preview) => (
                  <li key={preview.sequenceNumber}>
                    <strong>{preview.sequenceNumber}.</strong> {formatDateTime(preview.scheduledAtIso)}
                    <small>{preview.lessonTitle}</small>
                  </li>
                ))}
              </ol>
            )}
            <Button onClick={vm.save} disabled={!vm.canSave}>
              {vm.saving ? "Zapisywanie…" : "Utwórz grupę"}
            </Button>
          </div>
        </aside>
      </div>
    </section>
  );
}

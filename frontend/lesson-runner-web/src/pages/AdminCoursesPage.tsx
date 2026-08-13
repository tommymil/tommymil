import { useEffect, useMemo, useState } from "react";
import { ArrowDown, ArrowUp, BookCopy, Plus, Trash2 } from "lucide-react";
import { createCourse, deleteCourse, getCourse, getCourses, updateCourse } from "../api/coursesApi";
import { getLessons } from "../api/lessonsApi";
import { ApiError } from "../api/client";
import { Button } from "../components/ui/Button";
import { useDialogs } from "../features/dialog/DialogContext";
import { useToast } from "../features/toast/ToastContext";
import type { CourseDetails, CourseSummary, UpsertCourseRequest } from "../types/course";
import type { LessonSummary } from "../types/lesson";

type CourseForm = {
  id: string | null;
  name: string;
  subject: string;
  level: string;
  description: string;
  lessonIds: string[];
};

const emptyForm: CourseForm = {
  id: null,
  name: "",
  subject: "",
  level: "",
  description: "",
  lessonIds: [],
};

export function AdminCoursesPage() {
  const toast = useToast();
  const { confirm } = useDialogs();
  const [courses, setCourses] = useState<CourseSummary[]>([]);
  const [readyLessons, setReadyLessons] = useState<LessonSummary[]>([]);
  const [form, setForm] = useState<CourseForm>(emptyForm);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let ignore = false;

    async function load() {
      try {
        setLoading(true);
        setError(null);
        const [courseList, lessonList] = await Promise.all([getCourses(), getLessons()]);

        if (ignore) {
          return;
        }

        setCourses(courseList);
        setReadyLessons(lessonList.filter((lesson) => lesson.status === "ready"));
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać kursów i lekcji.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    void load();

    return () => {
      ignore = true;
    };
  }, []);

  const selectedLessons = useMemo(
    () =>
      form.lessonIds
        .map((id) => readyLessons.find((lesson) => lesson.id === id))
        .filter((lesson): lesson is LessonSummary => lesson !== undefined),
    [form.lessonIds, readyLessons],
  );

  const availableLessons = useMemo(
    () => readyLessons.filter((lesson) => !form.lessonIds.includes(lesson.id)),
    [form.lessonIds, readyLessons],
  );

  function updateField(field: keyof Omit<CourseForm, "lessonIds" | "id">, value: string) {
    setForm((current) => ({ ...current, [field]: value }));
  }

  function addLesson(lessonId: string) {
    if (!lessonId || form.lessonIds.includes(lessonId)) {
      return;
    }

    setForm((current) => ({ ...current, lessonIds: [...current.lessonIds, lessonId] }));
  }

  function removeLesson(lessonId: string) {
    setForm((current) => ({ ...current, lessonIds: current.lessonIds.filter((id) => id !== lessonId) }));
  }

  function moveLesson(index: number, direction: -1 | 1) {
    setForm((current) => {
      const target = index + direction;
      if (target < 0 || target >= current.lessonIds.length) {
        return current;
      }

      const lessonIds = [...current.lessonIds];
      [lessonIds[index], lessonIds[target]] = [lessonIds[target], lessonIds[index]];
      return { ...current, lessonIds };
    });
  }

  async function editCourse(id: string) {
    try {
      setError(null);
      const details = await getCourse(id);
      setForm(fromDetails(details));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać kursu.");
    }
  }

  async function saveCourse() {
    if (!form.name.trim() || !form.subject.trim() || !form.level.trim() || form.lessonIds.length === 0) {
      setError("Uzupełnij nazwę, przedmiot, poziom i przynajmniej jedną lekcję.");
      return;
    }

    const request: UpsertCourseRequest = {
      name: form.name.trim(),
      subject: form.subject.trim(),
      level: form.level.trim(),
      description: form.description.trim(),
      lessonIds: form.lessonIds,
    };

    try {
      setSaving(true);
      setError(null);
      const saved = form.id ? await updateCourse(form.id, request) : await createCourse(request);
      const updatedCourses = await getCourses();
      setCourses(updatedCourses);
      setForm(fromDetails(saved));
      toast.success(form.id ? "Kurs został zapisany." : "Kurs został utworzony.");
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać kursu.");
    } finally {
      setSaving(false);
    }
  }

  async function removeCourse(course: CourseSummary) {
    const confirmed = await confirm({
      title: "Usunąć kurs?",
      description: `Kurs „${course.name}” przestanie być dostępny jako szablon nowych grup.`,
      tone: "danger",
      confirmLabel: "Usuń kurs",
      consequences: ["Istniejące grupy zachowają swoje terminy i przypisane lekcje."],
    });

    if (!confirmed) {
      return;
    }

    try {
      setError(null);
      await deleteCourse(course.id);
      setCourses((current) => current.filter((item) => item.id !== course.id));
      if (form.id === course.id) {
        setForm(emptyForm);
      }
      toast.success("Kurs został usunięty.");
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się usunąć kursu.");
    }
  }

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Administrator</span>
          <h1>Kursy</h1>
          <p>Program kursu określa kolejność lekcji, z której można utworzyć grupę.</p>
        </div>
        <div className="page-header-actions">
          <Button variant="secondary" onClick={() => setForm(emptyForm)}>
            <Plus className="button-icon" aria-hidden="true" />
            Nowy kurs
          </Button>
        </div>
      </div>

      {error ? <div className="list-state list-state-error">{error}</div> : null}
      {loading ? <div className="list-state">Ładowanie kursów...</div> : null}

      {!loading ? (
        <div className="courses-layout">
          <section className="course-list-panel">
            {courses.length === 0 ? (
          <div className="list-state">Brak kursów.</div>
            ) : (
              <div className="group-list">
                {courses.map((course) => (
                  <article className="group-card course-list-card" key={course.id}>
                    <div className="group-card-main">
                      <span className="group-avatar">
                        <BookCopy size={20} aria-hidden="true" />
                      </span>
                      <div className="group-card-content">
                        <div className="group-card-heading">
                          <div>
                            <h2>{course.name}</h2>
                            <p className="group-meta">
                              {course.subject} - {course.level} - {course.lessonCount} lekcji
                            </p>
                          </div>
                        </div>
                        {course.description ? <p className="cell-sub">{course.description}</p> : null}
                      </div>
                    </div>
                    <div className="group-card-actions">
                      <Button variant="secondary" onClick={() => void editCourse(course.id)}>
                        Edytuj
                      </Button>
                      <Button variant="ghost" onClick={() => void removeCourse(course)}>
                        <Trash2 className="button-icon" aria-hidden="true" />
                        Usuń
                      </Button>
                    </div>
                  </article>
                ))}
              </div>
            )}
          </section>

          <section className="editor-fieldset course-editor">
            <legend>{form.id ? "Edycja kursu" : "Nowy kurs"}</legend>
            <label className="form-field">
              <span>Nazwa</span>
              <input value={form.name} onChange={(event) => updateField("name", event.target.value)} />
            </label>
            <div className="course-editor-row">
              <label className="form-field">
                <span>Przedmiot</span>
                <input value={form.subject} onChange={(event) => updateField("subject", event.target.value)} />
              </label>
              <label className="form-field">
                <span>Poziom</span>
                <input value={form.level} onChange={(event) => updateField("level", event.target.value)} />
              </label>
            </div>
            <label className="form-field">
              <span>Opis</span>
              <textarea rows={3} value={form.description} onChange={(event) => updateField("description", event.target.value)} />
            </label>

            <div className="form-field">
              <span>Dodaj lekcje Ready</span>
              <select value="" onChange={(event) => addLesson(event.target.value)} disabled={availableLessons.length === 0}>
                <option value="">{availableLessons.length === 0 ? "Brak dostępnych lekcji" : "Wybierz lekcję..."}</option>
                {availableLessons.map((lesson) => (
                  <option key={lesson.id} value={lesson.id}>
                    {lesson.title} ({lesson.subject})
                  </option>
                ))}
              </select>
            </div>

            {selectedLessons.length === 0 ? (
              <p className="cue-empty">Nie wybrano jeszcze lekcji.</p>
            ) : (
              <ol className="ordered-list">
                {selectedLessons.map((lesson, index) => (
                  <li className="ordered-row" key={lesson.id}>
                    <span className="ordered-title">
                      {index + 1}. {lesson.title}
                    </span>
                    <span className="sub-controls">
                      <button type="button" onClick={() => moveLesson(index, -1)} disabled={index === 0} aria-label="W górę">
                        <ArrowUp size={14} aria-hidden="true" />
                      </button>
                      <button
                        type="button"
                        onClick={() => moveLesson(index, 1)}
                        disabled={index === selectedLessons.length - 1}
                        aria-label="W dół"
                      >
                        <ArrowDown size={14} aria-hidden="true" />
                      </button>
                      <button type="button" onClick={() => removeLesson(lesson.id)} aria-label="Usuń">
                        <Trash2 size={14} aria-hidden="true" />
                      </button>
                    </span>
                  </li>
                ))}
              </ol>
            )}

            <Button onClick={() => void saveCourse()} disabled={saving}>
              {saving ? "Zapisywanie..." : "Zapisz kurs"}
            </Button>
          </section>
        </div>
      ) : null}
    </section>
  );
}

function fromDetails(course: CourseDetails): CourseForm {
  return {
    id: course.id,
    name: course.name,
    subject: course.subject,
    level: course.level,
    description: course.description,
    lessonIds: course.lessons.sort((a, b) => a.order - b.order).map((lesson) => lesson.lessonId),
  };
}

import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ApiError } from "../../api/client";
import { getCourse, getCourses } from "../../api/coursesApi";
import { createGroup, getInstructors } from "../../api/groupsApi";
import { getLessons } from "../../api/lessonsApi";
import { createParticipant } from "../../api/participantsApi";
import type { CourseDetails, CourseSummary } from "../../types/course";
import type { CreateGroupRequest, Instructor } from "../../types/group";
import type { LessonSummary } from "../../types/lesson";
import { draftToRequest } from "../../types/participant";
import type { ParticipantDraft, ParticipantSummary } from "../../types/participant";
import { addWeeksIso } from "./datetime";

export type SessionPreview = {
  sequenceNumber: number;
  lessonTitle: string;
  scheduledAtIso: string;
};

export function useGroupEditorViewModel() {
  const navigate = useNavigate();

  const [name, setName] = useState("");
  const [instructors, setInstructors] = useState<Instructor[]>([]);
  const [instructorId, setInstructorId] = useState("");
  const [courses, setCourses] = useState<CourseSummary[]>([]);
  const [selectedCourseId, setSelectedCourseId] = useState("");
  const [selectedCourse, setSelectedCourse] = useState<CourseDetails | null>(null);
  const [capacity, setCapacity] = useState("");
  const [meetingUrl, setMeetingUrl] = useState("");
  const [readyLessons, setReadyLessons] = useState<LessonSummary[]>([]);
  const [selectedLessonIds, setSelectedLessonIds] = useState<string[]>([]);
  const [firstSessionAt, setFirstSessionAt] = useState("");
  const [selectedParticipants, setSelectedParticipants] = useState<ParticipantSummary[]>([]);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let ignore = false;

    async function load() {
      try {
        setLoading(true);
        setError(null);
        const [instructorList, courseList, lessons] = await Promise.all([
          getInstructors(),
          getCourses(),
          getLessons(),
        ]);

        if (ignore) {
          return;
        }

        setInstructors(instructorList);
        setInstructorId((current) => current || instructorList[0]?.id || "");
        setCourses(courseList);
        setReadyLessons(lessons.filter((lesson) => lesson.status === "ready"));
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać instruktorów i lekcji.");
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
      selectedLessonIds
        .map((id) => readyLessons.find((lesson) => lesson.id === id))
        .filter((lesson): lesson is LessonSummary => lesson !== undefined),
    [readyLessons, selectedLessonIds],
  );

  const availableLessons = useMemo(
    () => readyLessons.filter((lesson) => !selectedLessonIds.includes(lesson.id)),
    [readyLessons, selectedLessonIds],
  );

  const sessionPreviews = useMemo<SessionPreview[]>(() => {
    if (!firstSessionAt) {
      return [];
    }

    const firstIso = new Date(firstSessionAt).toISOString();

    return selectedLessons.map((lesson, index) => ({
      sequenceNumber: index + 1,
      lessonTitle: lesson.title,
      scheduledAtIso: index === 0 ? firstIso : addWeeksIso(firstIso, index),
    }));
  }, [firstSessionAt, selectedLessons]);

  async function selectCourse(courseId: string) {
    setSelectedCourseId(courseId);
    setSelectedCourse(null);

    if (!courseId) {
      setSelectedLessonIds([]);
      return;
    }

    try {
      setError(null);
      const course = await getCourse(courseId);
      setSelectedCourse(course);
      setSelectedLessonIds([...course.lessons].sort((a, b) => a.order - b.order).map((lesson) => lesson.lessonId));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać kursu.");
    }
  }

  function addLesson(lessonId: string) {
    if (selectedCourseId) {
      return;
    }

    if (lessonId && !selectedLessonIds.includes(lessonId)) {
      setSelectedLessonIds((current) => [...current, lessonId]);
    }
  }

  function removeLesson(lessonId: string) {
    if (selectedCourseId) {
      return;
    }

    setSelectedLessonIds((current) => current.filter((id) => id !== lessonId));
  }

  function moveLesson(index: number, direction: -1 | 1) {
    if (selectedCourseId) {
      return;
    }

    setSelectedLessonIds((current) => {
      const target = index + direction;
      if (target < 0 || target >= current.length) {
        return current;
      }
      const next = [...current];
      [next[index], next[target]] = [next[target], next[index]];
      return next;
    });
  }

  function pickExistingParticipant(participant: ParticipantSummary) {
    setSelectedParticipants((current) =>
      current.some((item) => item.id === participant.id) ? current : [...current, participant],
    );
  }

  async function createAndPickParticipant(draft: ParticipantDraft): Promise<boolean> {
    try {
      setError(null);
      const created = await createParticipant(draftToRequest(draft));
      setSelectedParticipants((current) => [...current, created]);
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się dodać uczestnika.");
      return false;
    }
  }

  function removeSelectedParticipant(participantId: string) {
    setSelectedParticipants((current) => current.filter((item) => item.id !== participantId));
  }

  const canSave =
    name.trim().length > 0 && instructorId.length > 0 && selectedLessonIds.length > 0 && firstSessionAt.length > 0 && !saving;

  async function save() {
    if (!canSave) {
      setError("Uzupełnij nazwę, instruktora, przynajmniej jedną lekcję i datę pierwszych zajęć.");
      return;
    }

    const request: CreateGroupRequest = {
      name: name.trim(),
      instructorId,
      lessonIds: selectedLessonIds,
      firstSessionAt: new Date(firstSessionAt).toISOString(),
      participantIds: selectedParticipants.map((participant) => participant.id),
      courseId: selectedCourseId || null,
      capacity: capacity.trim() ? Number(capacity) : null,
      meetingUrl: meetingUrl.trim() || null,
    };

    try {
      setSaving(true);
      setError(null);
      const group = await createGroup(request);
      navigate(`/admin/groups/${group.id}`);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się utworzyć grupy.");
    } finally {
      setSaving(false);
    }
  }

  return {
    name,
    setName,
    instructors,
    instructorId,
    setInstructorId,
    courses,
    selectedCourseId,
    selectedCourse,
    selectCourse,
    capacity,
    setCapacity,
    meetingUrl,
    setMeetingUrl,
    availableLessons,
    selectedLessons,
    addLesson,
    removeLesson,
    moveLesson,
    firstSessionAt,
    setFirstSessionAt,
    selectedParticipants,
    pickExistingParticipant,
    createAndPickParticipant,
    removeSelectedParticipant,
    sessionPreviews,
    loading,
    saving,
    error,
    canSave,
    save,
  };
}

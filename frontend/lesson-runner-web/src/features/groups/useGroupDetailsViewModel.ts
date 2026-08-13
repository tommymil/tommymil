import { useEffect, useState } from "react";
import { ApiError } from "../../api/client";
import {
  addSession,
  cancelSession,
  exportGroupAttendance,
  exportSessionAttendance,
  getGroup,
  getGroupAttendance,
  getInstructors,
  getSessionHistory,
  getSessionStatusOptions,
  rescheduleSession,
  setSessionLinks,
  setSessionStatus,
  setSessionSubstitute,
  updateGroup,
} from "../../api/groupsApi";
import { getLessons } from "../../api/lessonsApi";
import { createParticipant, enrollParticipant, unenrollParticipant } from "../../api/participantsApi";
import type { GroupAttendanceSummary, GroupDetails, Instructor, SessionChange, SessionStatusOption } from "../../types/group";
import type { LessonSummary } from "../../types/lesson";
import { draftToRequest } from "../../types/participant";
import type { ParticipantDraft, ParticipantSummary } from "../../types/participant";

export function useGroupDetailsViewModel(groupId: string | undefined) {
  const [group, setGroup] = useState<GroupDetails | null>(null);
  const [summary, setSummary] = useState<GroupAttendanceSummary | null>(null);
  const [instructors, setInstructors] = useState<Instructor[]>([]);
  const [readyLessons, setReadyLessons] = useState<LessonSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);
  const [history, setHistory] = useState<SessionChange[] | null>(null);
  const [statusOptions, setStatusOptions] = useState<SessionStatusOption[]>([]);

  useEffect(() => {
    if (!groupId) {
      setError("Brak identyfikatora grupy.");
      setLoading(false);
      return undefined;
    }

    let ignore = false;

    async function load(id: string) {
      try {
        setLoading(true);
        setError(null);
        const [response, attendance, instructorList, lessons, statuses] = await Promise.all([
          getGroup(id),
          getGroupAttendance(id),
          getInstructors(),
          getLessons(),
          getSessionStatusOptions(),
        ]);
        if (!ignore) {
          setGroup(response);
          setSummary(attendance);
          setInstructors(instructorList);
          setReadyLessons(lessons.filter((lesson) => lesson.status === "ready"));
          setStatusOptions(statuses);
        }
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać grupy.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    void load(groupId);

    return () => {
      ignore = true;
    };
  }, [groupId]);

  async function pickExistingParticipant(participant: ParticipantSummary) {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      await enrollParticipant(participant.id, group.id);
      setGroup((current) =>
        current && !current.participants.some((item) => item.id === participant.id)
          ? { ...current, participants: [...current.participants, participant] }
          : current,
      );
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się przypisać uczestnika do grupy.");
    } finally {
      setBusy(false);
    }
  }

  async function createAndEnrollParticipant(draft: ParticipantDraft): Promise<boolean> {
    if (!group) {
      return false;
    }

    try {
      setBusy(true);
      setError(null);
      const created = await createParticipant(draftToRequest(draft, [group.id]));
      setGroup((current) => (current ? { ...current, participants: [...current.participants, created] } : current));
      return true;
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się dodać uczestnika.");
      return false;
    } finally {
      setBusy(false);
    }
  }

  async function unenrollParticipantFromGroup(participantId: string) {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      await unenrollParticipant(participantId, group.id);
      setGroup((current) =>
        current ? { ...current, participants: current.participants.filter((item) => item.id !== participantId) } : current,
      );
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się wypisać uczestnika z grupy.");
    } finally {
      setBusy(false);
    }
  }

  async function cancelTerm(
    sessionId: string,
    reason?: string | null,
    guardiansNotified = false,
    compensation?: string | null,
    shiftFollowingLessons = false,
  ) {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      await cancelSession(group.id, sessionId, {
        reason: reason?.trim() || null,
        guardiansNotified,
        compensation: compensation || null,
        shiftFollowingLessons,
      });
      // Przesunięcie materiału potrafi dopisać nowy termin na końcu kursu, więc zamiast
      // podmieniać jeden wiersz pobieramy grupę od nowa.
      setGroup(await getGroup(group.id));
      setHistory(null);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się odwołać terminu.");
    } finally {
      setBusy(false);
    }
  }

  async function rescheduleTerm(
    sessionId: string,
    scheduledAt: string,
    substituteInstructorId?: string | null,
    reason?: string | null,
    guardiansNotified = false,
  ) {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      const updated = await rescheduleSession(group.id, sessionId, {
        scheduledAt: new Date(scheduledAt).toISOString(),
        substituteInstructorId: substituteInstructorId || null,
        reason: reason?.trim() || null,
        guardiansNotified,
      });
      setGroup((current) =>
        current
          ? { ...current, sessions: current.sessions.map((session) => (session.id === sessionId ? updated : session)) }
          : current,
      );
      setHistory(null);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się przełożyć terminu.");
    } finally {
      setBusy(false);
    }
  }

  async function changeStatus(sessionId: string, status: string, reason?: string | null, guardiansNotified = false) {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      const updated = await setSessionStatus(group.id, sessionId, {
        status,
        reason: reason?.trim() || null,
        guardiansNotified,
      });
      setGroup((current) =>
        current
          ? { ...current, sessions: current.sessions.map((session) => (session.id === sessionId ? updated : session)) }
          : current,
      );
      setHistory(null);
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zmienić statusu terminu.");
    } finally {
      setBusy(false);
    }
  }

  /** Historię pobieramy na żądanie - to widok kontrolny, nie coś, co trzeba mieć zawsze pod ręką. */
  async function loadHistory() {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      setHistory(await getSessionHistory(group.id));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać historii zmian.");
    } finally {
      setBusy(false);
    }
  }

  async function saveGroup(
    name: string,
    instructorId: string,
    capacity?: number | null,
    meetingUrl?: string | null,
  ) {
    if (!group || !name.trim() || !instructorId) {
      setError("Nazwa i instruktor są wymagane.");
      return;
    }

    try {
      setBusy(true);
      setError(null);
      const updated = await updateGroup(group.id, {
        name: name.trim(),
        instructorId,
        capacity: capacity ?? null,
        meetingUrl: meetingUrl?.trim() || null,
      });
      setGroup((current) => (current ? updated : current));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać grupy.");
    } finally {
      setBusy(false);
    }
  }

  async function addTerm(lessonId: string, scheduledAt: string, substituteInstructorId?: string | null) {
    if (!group || !lessonId || !scheduledAt) {
      setError("Wybierz lekcję i datę nowego terminu.");
      return;
    }

    try {
      setBusy(true);
      setError(null);
      const created = await addSession(group.id, {
        lessonId,
        scheduledAt: new Date(scheduledAt).toISOString(),
        substituteInstructorId: substituteInstructorId || null,
      });
      setGroup((current) => (current ? { ...current, sessions: [...current.sessions, created] } : current));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się dodać terminu.");
    } finally {
      setBusy(false);
    }
  }

  async function setSubstitute(sessionId: string, substituteInstructorId?: string | null) {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      const updated = await setSessionSubstitute(group.id, sessionId, { substituteInstructorId: substituteInstructorId || null });
      setGroup((current) =>
        current
          ? { ...current, sessions: current.sessions.map((session) => (session.id === sessionId ? updated : session)) }
          : current,
      );
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać zastępcy.");
    } finally {
      setBusy(false);
    }
  }

  async function setLinks(sessionId: string, meetingUrl: string | null, recordingUrl: string | null) {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      const updated = await setSessionLinks(group.id, sessionId, { meetingUrl, recordingUrl });
      setGroup((current) =>
        current
          ? { ...current, sessions: current.sessions.map((session) => (session.id === sessionId ? updated : session)) }
          : current,
      );
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać linków terminu.");
    } finally {
      setBusy(false);
    }
  }

  async function downloadAttendanceExport() {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      saveDownloadedFile(await exportGroupAttendance(group.id));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać eksportu frekwencji.");
    } finally {
      setBusy(false);
    }
  }

  async function downloadSessionAttendanceExport(sessionId: string) {
    if (!group) {
      return;
    }

    try {
      setBusy(true);
      setError(null);
      saveDownloadedFile(await exportSessionAttendance(group.id, sessionId));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać listy obecności.");
    } finally {
      setBusy(false);
    }
  }

  return {
    group,
    summary,
    instructors,
    readyLessons,
    loading,
    error,
    busy,
    pickExistingParticipant,
    createAndEnrollParticipant,
    unenrollParticipantFromGroup,
    cancelTerm,
    rescheduleTerm,
    saveGroup,
    addTerm,
    setSubstitute,
    setLinks,
    history,
    loadHistory,
    statusOptions,
    changeStatus,
    downloadAttendanceExport,
    downloadSessionAttendanceExport,
  };
}

function saveDownloadedFile(file: { blob: Blob; fileName: string }) {
  const url = window.URL.createObjectURL(file.blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = file.fileName;
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(url);
}

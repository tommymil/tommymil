import { useEffect, useMemo, useState } from "react";
import { getSchedule } from "../../api/groupsApi";
import { isActiveSession, isUpcomingSession } from "../../types/group";
import type { ScheduledSession } from "../../types/group";

export type CourseSchedule = {
  groupId: string;
  groupName: string;
  /** Lekcje kursu w kolejności programu (po numerze sekwencji). */
  sessions: ScheduledSession[];
  /** Termin, który "wypada teraz": trwający, a jeśli brak - najbliższy zaplanowany. */
  currentSessionId: string | null;
};

function isActive(session: ScheduledSession): boolean {
  return isActiveSession(session.status);
}

function timeOf(session: ScheduledSession): number {
  const time = new Date(session.scheduledAt).getTime();
  return Number.isNaN(time) ? Number.MAX_SAFE_INTEGER : time;
}

/** Bieżący termin kursu: trwający ma pierwszeństwo, inaczej najbliższy zaplanowany. */
function pickCurrent(sessions: ScheduledSession[]): string | null {
  const inProgress = sessions.find((session) => session.status === "inprogress");
  if (inProgress) {
    return inProgress.id;
  }

  const planned = sessions
    .filter((session) => isUpcomingSession(session.status))
    .sort((a, b) => timeOf(a) - timeOf(b));

  return planned[0]?.id ?? null;
}

export function useInstructorScheduleViewModel() {
  const [sessions, setSessions] = useState<ScheduledSession[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let ignore = false;

    async function load() {
      try {
        setLoading(true);
        setError(null);
        const response = await getSchedule();
        if (!ignore) {
          setSessions(response);
        }
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać grafiku.");
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

  const courses = useMemo<CourseSchedule[]>(() => {
    const byGroup = new Map<string, ScheduledSession[]>();

    for (const session of sessions) {
      const list = byGroup.get(session.groupId) ?? [];
      list.push(session);
      byGroup.set(session.groupId, list);
    }

    return [...byGroup.values()]
      .map((groupSessions) => {
        const ordered = [...groupSessions].sort((a, b) => a.sequenceNumber - b.sequenceNumber);
        return {
          groupId: ordered[0].groupId,
          groupName: ordered[0].groupName,
          sessions: ordered,
          currentSessionId: pickCurrent(ordered),
        };
      })
      .sort((a, b) => a.groupName.localeCompare(b.groupName, "pl"));
  }, [sessions]);

  // Najbliższe zajęcia w ogóle (przez wszystkie grupy) - to je prowadzący ma "na już".
  const nextUp = useMemo<ScheduledSession | null>(() => {
    const active = sessions.filter(isActive);
    if (active.length === 0) {
      return null;
    }

    return active.sort((a, b) => {
      // Trwające przed zaplanowanymi, potem wg czasu.
      if (a.status !== b.status) {
        if (a.status === "inprogress") return -1;
        if (b.status === "inprogress") return 1;
      }
      return timeOf(a) - timeOf(b);
    })[0];
  }, [sessions]);

  return { courses, nextUp, loading, error };
}

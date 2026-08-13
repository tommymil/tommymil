import { useEffect, useState } from "react";
import { ApiError } from "../../api/client";
import { deleteGroup, getGroups } from "../../api/groupsApi";
import type { GroupSummary } from "../../types/group";

export function useGroupsViewModel() {
  const [groups, setGroups] = useState<GroupSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    let ignore = false;

    async function load() {
      try {
        setLoading(true);
        setError(null);
        const response = await getGroups();
        if (!ignore) {
          setGroups(response);
        }
      } catch {
        if (!ignore) {
          setError("Nie udało się pobrać grup.");
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

  async function removeGroup(id: string) {
    try {
      setDeletingId(id);
      setError(null);
      await deleteGroup(id);
      setGroups((current) => current.filter((group) => group.id !== id));
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się usunąć grupy.");
    } finally {
      setDeletingId(null);
    }
  }

  return { groups, loading, error, deletingId, removeGroup };
}

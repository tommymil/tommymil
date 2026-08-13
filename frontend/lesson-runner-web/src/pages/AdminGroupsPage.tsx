import { CalendarClock, FolderOpen, Plus, Trash2, UserRound, UsersRound } from "lucide-react";
import type { LucideIcon } from "lucide-react";
import { Link } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { EmptyState } from "../components/ui/EmptyState";
import { SkeletonList } from "../components/ui/Skeleton";
import { formatDateTime } from "../features/groups/datetime";
import { useDialogs } from "../features/dialog/DialogContext";
import { useToast } from "../features/toast/ToastContext";
import { useGroupsViewModel } from "../features/groups/useGroupsViewModel";
import type { GroupSummary } from "../types/group";

export function AdminGroupsPage() {
  const viewModel = useGroupsViewModel();
  const { confirm } = useDialogs();
  const toast = useToast();

  /**
   * Usunięcie grupy było jedyną operacją nieodwracalną w aplikacji **bez żadnego
   * potwierdzenia** — przycisk „Usuń” stał wprost w karcie i wołał API od razu.
   * Sąsiednie ekrany miały przynajmniej natywne okno przeglądarki.
   */
  async function handleRemoveGroup(group: GroupSummary) {
    const confirmed = await confirm({
      title: `Usunąć grupę „${group.name}”?`,
      description: "Grupa zniknie razem z zaplanowanymi terminami i zapisami uczestników.",
      tone: "danger",
      confirmLabel: "Usuń grupę",
      typeToConfirm: group.name,
      consequences: [
        `${group.sessionCount} ${group.sessionCount === 1 ? "zaplanowany termin" : "zaplanowanych terminów"} zostanie usuniętych.`,
        `${group.participantCount} ${group.participantCount === 1 ? "dziecko straci" : "dzieci straci"} przypisanie do tej grupy.`,
        "Historia zmian terminów, obecności i rozliczeń zostaje — te tabele celowo nie mają klucza obcego do grupy.",
      ],
    });

    if (!confirmed) {
      return;
    }

    await viewModel.removeGroup(group.id);
    toast.success(`Grupa „${group.name}” została usunięta.`);
  }

  const totalParticipants = viewModel.groups.reduce((sum, group) => sum + group.participantCount, 0);
  const totalSessions = viewModel.groups.reduce((sum, group) => sum + group.sessionCount, 0);
  const groupsWithNextSession = viewModel.groups.filter((group) => group.nextSessionAt).length;
  const nextGroup = getNextGroup(viewModel.groups);

  return (
    <section className="page-section groups-page">
      <div className="page-header groups-header">
        <div>
          <span className="eyebrow">Administrator</span>
          <h1>Grupy</h1>
          <p>Zarządzaj grupami, uczestnikami i najbliższymi terminami bez wchodzenia w każdy szczegół.</p>
        </div>
        <div className="page-header-actions">
          <Link to="/admin/groups/new">
            <Button>
              <Plus className="button-icon" aria-hidden="true" />
              Nowa grupa
            </Button>
          </Link>
        </div>
      </div>

      {viewModel.loading ? <SkeletonList rows={3} label="Ładowanie grup" /> : null}
      {viewModel.error ? (
        <div className="list-state list-state-error" role="alert">
          {viewModel.error}
        </div>
      ) : null}

      {!viewModel.loading && !viewModel.error && viewModel.groups.length === 0 ? (
        <EmptyState
          icon={FolderOpen}
          title="Nie ma jeszcze żadnych grup"
          description="Utwórz pierwszą grupę, przypisz instruktora i zaplanuj terminy zajęć."
          action={
            <Link to="/admin/groups/new">
              <Button>
                <Plus className="button-icon" aria-hidden="true" />
                Nowa grupa
              </Button>
            </Link>
          }
        />
      ) : null}

      {!viewModel.loading && viewModel.groups.length > 0 ? (
        <>
          <div className="groups-overview" aria-label="Podsumowanie grup">
            <SummaryCard label="Grupy" value={viewModel.groups.length} icon={FolderOpen} />
            <SummaryCard label="Uczestnicy" value={totalParticipants} icon={UsersRound} />
            <SummaryCard label="Zaplanowane terminy" value={totalSessions} icon={CalendarClock} />
            <SummaryCard label="Z najbliższą lekcją" value={groupsWithNextSession} icon={CalendarClock} />
          </div>

          {nextGroup ? (
            <div className="groups-highlight">
              <span className="groups-highlight-icon" aria-hidden="true">
                <CalendarClock size={20} />
              </span>
              <div>
                <strong>Najbliższe zajęcia</strong>
                <span>
                  {nextGroup.name} · {formatDateTime(nextGroup.nextSessionAt)}
                </span>
              </div>
            </div>
          ) : null}

          <div className="group-list">
            {viewModel.groups.map((group) => (
              <article key={group.id} className="group-card">
                <div className="group-card-main">
                  <div className="group-avatar" aria-hidden="true">
                    {getGroupInitials(group.name)}
                  </div>

                  <div className="group-card-content">
                    <div className="group-card-heading">
                      <div>
                        <h2>{group.name}</h2>
                        <p className="group-meta">
                          <UserRound size={15} aria-hidden="true" />
                          {group.instructorName}
                        </p>
                      </div>
                      <span className={group.nextSessionAt ? "status-pill status-planned" : "status-pill status-draft"}>
                        {group.nextSessionAt ? "Aktywna" : "Bez terminu"}
                      </span>
                    </div>

                    <dl className="group-metrics">
                      <div>
                        <dt>Uczestnicy</dt>
                        <dd>{group.participantCount}</dd>
                      </div>
                      <div>
                        <dt>Terminy</dt>
                        <dd>{group.sessionCount}</dd>
                      </div>
                      <div className="group-next-session">
                        <dt>Najbliższe zajęcia</dt>
                        <dd>{group.nextSessionAt ? formatDateTime(group.nextSessionAt) : "Brak zaplanowanego terminu"}</dd>
                      </div>
                    </dl>
                  </div>
                </div>

                <div className="group-card-actions">
                  <Link to={`/admin/groups/${group.id}`}>
                    <Button variant="secondary">Otwórz</Button>
                  </Link>
                  <Button
                    variant="ghost"
                    onClick={() => void handleRemoveGroup(group)}
                    disabled={viewModel.deletingId === group.id}
                  >
                    <Trash2 className="button-icon" aria-hidden="true" />
                    {viewModel.deletingId === group.id ? "Usuwanie…" : "Usuń"}
                  </Button>
                </div>
              </article>
            ))}
          </div>
        </>
      ) : null}
    </section>
  );
}

function SummaryCard({
  label,
  value,
  icon: Icon,
}: {
  label: string;
  value: number;
  icon: LucideIcon;
}) {
  return (
    <div className="groups-summary-card">
      <span className="groups-summary-icon" aria-hidden="true">
        <Icon size={20} />
      </span>
      <div>
        <strong>{value}</strong>
        <span>{label}</span>
      </div>
    </div>
  );
}

function getNextGroup(groups: GroupSummary[]): GroupSummary | null {
  return groups
    .filter((group) => group.nextSessionAt)
    .sort((first, second) => new Date(first.nextSessionAt ?? "").getTime() - new Date(second.nextSessionAt ?? "").getTime())[0] ?? null;
}

function getGroupInitials(name: string): string {
  return name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0])
    .join("")
    .toUpperCase();
}

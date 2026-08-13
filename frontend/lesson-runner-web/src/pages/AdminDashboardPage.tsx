import type { ReactNode } from "react";
import { useEffect, useState } from "react";
import {
  AlertTriangle,
  BookOpenText,
  CalendarClock,
  CheckCircle2,
  CreditCard,
  Gauge,
  TrendingUp,
  UserRound,
  UsersRound,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";
import { Link } from "react-router-dom";
import { ApiError } from "../api/client";
import { getDashboard } from "../api/dashboardApi";
import { Button } from "../components/ui/Button";
import { SkeletonList } from "../components/ui/Skeleton";
import { formatFriendlyDateTime } from "../features/groups/datetime";
import type { Dashboard, DashboardAttention } from "../types/dashboard";

export function AdminDashboardPage() {
  const [dashboard, setDashboard] = useState<Dashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let ignore = false;

    async function load() {
      try {
        setLoading(true);
        setError(null);
        const loaded = await getDashboard();
        if (!ignore) {
          setDashboard(loaded);
        }
      } catch (caught) {
        if (!ignore) {
          setError(caught instanceof ApiError ? caught.message : "Nie udało się pobrać dashboardu.");
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

  if (loading) {
    return (
      <section className="page-section">
        <SkeletonList rows={3} label="Ładowanie pulpitu" />
      </section>
    );
  }

  if (error || !dashboard) {
    return (
      <section className="page-section">
        <div className="list-state list-state-error" role="alert">
          {error ?? "Brak danych pulpitu."}
        </div>
      </section>
    );
  }

  const kpis = dashboard.kpis;

  return (
    <section className="page-section dashboard-page">
      <div className="page-header">
        <div>
          <span className="eyebrow">Administrator</span>
          <h1>Pulpit</h1>
          <p>Najpierw to, co wymaga reakcji. Liczby i najbliższe terminy niżej.</p>
        </div>
        <div className="page-header-actions">
          <Link to="/admin/groups/new">
            <Button>Nowa grupa</Button>
          </Link>
        </div>
      </div>

      <AttentionPanel items={dashboard.attention ?? []} />

      <div className="dashboard-kpi-grid">
        <KpiCard label="Aktywni uczestnicy" value={kpis.activeParticipants} icon={<UsersRound size={20} />} />
        <KpiCard label="Aktywne grupy" value={kpis.activeGroups} icon={<Gauge size={20} />} />
        <KpiCard label="Frekwencja" value={`${kpis.averageAttendancePercent}%`} icon={<TrendingUp size={20} />} />
        <KpiCard label="Do odrobienia" value={kpis.pendingMakeups} icon={<CalendarClock size={20} />} />
        <KpiCard label="Lista oczekujących" value={kpis.waitlistedParticipants} icon={<UserRound size={20} />} />
        <KpiCard label="Grupy pełne" value={kpis.groupsAtCapacity} icon={<UsersRound size={20} />} />
      </div>

      <div className="dashboard-layout">
        <section className="editor-panel">
          <div className="group-section-head">
            <div>
              <span className="eyebrow">Plan</span>
              <h2>Najbliższe terminy</h2>
            </div>
            <span>{kpis.upcomingSessions} aktywnych</span>
          </div>
          {dashboard.upcomingSessions.length === 0 ? <p className="cue-empty">Brak nadchodzących terminów.</p> : null}
          <div className="dashboard-list">
            {dashboard.upcomingSessions.map((session) => (
              <Link to={`/admin/groups/${session.groupId}`} className="dashboard-row" key={session.sessionId}>
                <CalendarClock size={17} aria-hidden="true" />
                <span>
                  <strong>{formatFriendlyDateTime(session.scheduledAt)}</strong>
                  <small>
                    {session.groupName} · {session.lessonTitle ?? "Bez lekcji"} · {session.instructorName}
                  </small>
                </span>
              </Link>
            ))}
          </div>
        </section>

        <section className="editor-panel">
          <div className="group-section-head">
            <div>
              <span className="eyebrow">Zapełnienie</span>
              <h2>Grupy</h2>
            </div>
            <span>{kpis.enrolledParticipants} zapisanych</span>
          </div>
          <div className="dashboard-list">
            {dashboard.groupFill.map((group) => (
              <Link to={`/admin/groups/${group.groupId}`} className="dashboard-fill-row" key={group.groupId}>
                <span>
                  <strong>{group.groupName}</strong>
                  <small>
                    {group.enrolled}
                    {group.capacity ? ` / ${group.capacity}` : ""} miejsc
                    {group.waitlisted ? ` · ${group.waitlisted} oczek.` : ""}
                  </small>
                </span>
                <div className="dashboard-progress" aria-label={`Zapełnienie ${group.fillPercent}%`}>
                  <span style={{ width: `${group.capacity ? group.fillPercent : 0}%` }} />
                </div>
              </Link>
            ))}
            {dashboard.groupFill.length === 0 ? <p className="cue-empty">Brak grup do pokazania.</p> : null}
          </div>
        </section>
      </div>

      <section className="editor-panel">
        <div className="group-section-head">
          <div>
            <span className="eyebrow">Instruktorzy</span>
            <h2>Obłożenie</h2>
          </div>
          <span>{dashboard.instructorLoads.length} osób</span>
        </div>
        {dashboard.instructorLoads.length === 0 ? <p className="cue-empty">Brak aktywnego obłożenia.</p> : null}
        {dashboard.instructorLoads.length > 0 ? (
          <table className="attendance-table">
            <thead>
              <tr>
                <th>Instruktor</th>
                <th>Grupy</th>
                <th>Nadchodzące</th>
                <th>Zastępstwa</th>
              </tr>
            </thead>
            <tbody>
              {dashboard.instructorLoads.map((load) => (
                <tr key={load.instructorId}>
                  <td>{load.instructorName}</td>
                  <td>{load.activeGroups}</td>
                  <td>{load.upcomingSessions}</td>
                  <td>{load.substituteSessions}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : null}
      </section>
    </section>
  );
}

const attentionIcons: Record<DashboardAttention["kind"], LucideIcon> = {
  overdue: CreditCard,
  nolesson: BookOpenText,
  noinstructor: AlertTriangle,
  lowattendance: TrendingUp,
  expiringcredit: CalendarClock,
  waitlist: UsersRound,
};

/**
 * „Wymaga uwagi” — lista zadań zamiast tablicy wyników.
 *
 * Stoi nad wskaźnikami świadomie: administrator otwiera pulpit z pytaniem o to,
 * czym się dziś zająć, a nie ile szkoła ma grup. Pusta lista też jest odpowiedzią.
 */
function AttentionPanel({ items }: { items: DashboardAttention[] }) {
  if (items.length === 0) {
    return (
      <div className="attention-panel attention-panel-clear">
        <CheckCircle2 size={20} aria-hidden="true" />
        <div>
          <strong>Nic nie wymaga uwagi</strong>
          <span>Brak zaległych faktur, terminów bez lekcji i grup bez prowadzącego.</span>
        </div>
      </div>
    );
  }

  return (
    <section className="attention-panel">
      <div className="attention-head">
        <h2>Wymaga uwagi</h2>
        <span className="cue-empty">{items.length}</span>
      </div>

      <ul className="attention-list">
        {items.map((item, index) => {
          const Icon = attentionIcons[item.kind] ?? AlertTriangle;

          return (
            <li key={`${item.kind}-${index}`} className={`attention-item attention-${item.severity}`}>
              <Icon size={17} aria-hidden="true" />
              <div>
                <strong>{item.title}</strong>
                <span>{item.detail}</span>
              </div>
              <Link to={item.path}>
                <Button variant="secondary">Przejdź</Button>
              </Link>
            </li>
          );
        })}
      </ul>
    </section>
  );
}

function KpiCard({ label, value, icon }: { label: string; value: number | string; icon: ReactNode }) {
  return (
    <div className="groups-summary-card dashboard-kpi-card">
      <span className="groups-summary-icon" aria-hidden="true">
        {icon}
      </span>
      <div>
        <strong>{value}</strong>
        <span>{label}</span>
      </div>
    </div>
  );
}

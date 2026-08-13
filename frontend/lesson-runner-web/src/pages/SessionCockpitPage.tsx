import { useEffect, useState } from "react";
import { CalendarDays, Coffee, PanelRightClose, PanelRightOpen, Play, ShieldAlert, Square, TrendingUp, X } from "lucide-react";
import { Link, useParams } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { Dialog } from "../components/ui/Dialog";
import { SkeletonList } from "../components/ui/Skeleton";
import { isCancelledSession, isUpcomingSession } from "../types/group";
import { formatDateTime } from "../features/groups/datetime";
import { useSessionCockpitViewModel } from "../features/groups/useSessionCockpitViewModel";
import { PresenterCockpit } from "../features/presenter/PresenterCockpit";
import { SessionRail } from "../features/presenter/SessionRail";
import { SessionProgressPanel } from "../features/progress/SessionProgressPanel";
import { openBreakWindow } from "../features/presenter/breakWindow";
import { ReportIncidentDialog } from "../features/safety/ReportIncidentDialog";
import { ReportTicketDialog } from "../features/safety/ReportTicketDialog";
import { useSessionSupportViewModel } from "../features/safety/useSessionSupportViewModel";

/** Po ilu minutach od startu przypominamy o przerwie w połowie zajęć. */
const BREAK_REMINDER_MINUTES = 42;

export function SessionCockpitPage() {
  const { sessionId } = useParams();
  const vm = useSessionCockpitViewModel(sessionId);
  const [showFinish, setShowFinish] = useState(false);
  const [showProgress, setShowProgress] = useState(false);
  const [railOpen, setRailOpen] = useState(true);
  // Hak stoi przed wcześniejszymi `return` — inaczej kolejność hooków zmieniałaby się
  // między stanem ładowania a gotowym kokpitem. Sam z siebie nic nie pobiera.
  const support = useSessionSupportViewModel(sessionId ?? null, vm.session?.groupId ?? null);

  if (vm.loading) {
    return (
      <section className="page-section">
        <SkeletonList rows={2} label="Ładowanie zajęć" />
      </section>
    );
  }

  if (!vm.session) {
    return (
      <section className="page-section">
        <div className="list-state list-state-error" role="alert">
          {vm.error ?? "Nie znaleziono zajęć."}
        </div>
        <Link to="/instructor/schedule">
          <Button variant="secondary">Wróć do grafiku</Button>
        </Link>
      </section>
    );
  }

  const session = vm.session;

  const header = (
    <div className="page-header">
      <div>
        <span className="eyebrow">{session.groupName}</span>
        <h1>{session.lessonTitle ?? "(brak lekcji)"}</h1>
        <p>
          {formatDateTime(session.scheduledAt)} — {session.statusLabel}
        </p>
      </div>
      <div className="page-header-actions">
        <Link to="/instructor/schedule">
          <Button variant="secondary">Wróć do grafiku</Button>
        </Link>
      </div>
    </div>
  );

  if (isCancelledSession(session.status)) {
    return (
      <section className="page-section">
        {header}
        <div className="list-state">Tego terminu nie poprowadzisz: {session.statusLabel.toLowerCase()}.</div>
      </section>
    );
  }

  // Zajęcia zamknięte - podsumowanie w trzech blokach, tak jak je wpisano.
  if (session.status === "completed" || session.status === "technicalfailure" || session.status === "notdelivered") {
    return (
      <section className="page-section">
        {header}
        <div className="summary-card">
          <h2>{session.statusLabel}</h2>
          <p>
            Obecni: {vm.presentCount} z {vm.attendance?.entries.length ?? 0}.
          </p>

          <h3>Notatka wewnętrzna</h3>
          <p className={session.instructorNote ? "material-text" : "cue-empty"}>
            {session.instructorNote ?? "Bez notatki."}
          </p>

          <h3>Czego nie zdążyliśmy</h3>
          <p className={session.unfinishedNote ? "material-text" : "cue-empty"}>
            {session.unfinishedNote ?? "Zrealizowano cały materiał."}
          </p>

          <h3>Podsumowanie dla rodzica</h3>
          <p className={session.parentSummary ? "material-text" : "cue-empty"}>
            {session.parentSummary ?? "Nie wysłano podsumowania."}
          </p>
        </div>
      </section>
    );
  }

  // Lobby przed startem.
  if (isUpcomingSession(session.status)) {
    return (
      <section className="page-section">
        {header}
        <div className="summary-card">
          <h2>Gotowy do rozpoczęcia</h2>
          <p>Kliknij „Start zajęć”, aby otworzyć listę obecności i rozpocząć prowadzenie.</p>
          {vm.error ? (
            <div className="list-state list-state-error" role="alert">
              {vm.error}
            </div>
          ) : null}
          <Button onClick={vm.start} disabled={vm.busy}>
            <Play className="button-icon" aria-hidden="true" />
            {vm.busy ? "Rozpoczynanie…" : "Start zajęć"}
          </Button>
        </div>
      </section>
    );
  }

  return (
    <>
      <BreakReminder startedAt={session.startedAt} />

      <div className={`cockpit-layout${railOpen ? "" : " cockpit-rail-closed"}`}>
        <div className="cockpit-stage">
          <PresenterCockpit
            lessonId={session.lessonId ?? undefined}
            scheduledSessionId={session.id}
            extraViewControls={
              <>
                <Button variant="secondary" onClick={() => setRailOpen((current) => !current)}>
                  {railOpen ? <PanelRightClose className="button-icon" aria-hidden="true" /> : <PanelRightOpen className="button-icon" aria-hidden="true" />}
                  {railOpen ? "Ukryj panel grupy" : "Pokaż panel grupy"}
                </Button>
                <Button variant="secondary" onClick={() => setShowProgress(true)}>
                  <TrendingUp className="button-icon" aria-hidden="true" />
                  Postępy
                </Button>
              </>
            }
            sessionControls={
              <>
                <Link to="/instructor/schedule" className="button button-ghost presenter-control-link">
                  <CalendarDays className="button-icon" aria-hidden="true" />
                  Wróć do grafiku
                </Link>
                <Button variant="secondary" onClick={() => void support.openIncidentDialog()}>
                  <ShieldAlert className="button-icon" aria-hidden="true" />
                  Zgłoś incydent
                </Button>
                <Button variant="danger" onClick={() => setShowFinish(true)}>
                  <Square className="button-icon" aria-hidden="true" />
                  Zakończ zajęcia
                </Button>
              </>
            }
          />
        </div>

        {/* Szyna zamiast trzech nakładek: scenariusz zostaje na ekranie. */}
        {railOpen ? (
          <SessionRail
            attendance={vm.attendance}
            busy={vm.busy}
            dirty={vm.dirty}
            savedAt={vm.savedAt}
            onStatus={vm.setStatus}
            onNote={vm.setNote}
            onLive={(participantId, liveStatus) => void vm.setLive(participantId, liveStatus)}
            onMakeupToggle={vm.toggleMakeupRequired}
            onMakeupSession={vm.setMakeupSession}
            onSaveNow={() => void vm.persistAttendance()}
            onAllPresent={vm.setAllPresent}
            allPresent={vm.allPresent}
            onReportProblem={(entry) =>
              void support.open(entry.participantId, `${entry.firstName} ${entry.lastName}`)
            }
          />
        ) : null}
      </div>

      {support.target ? (
        <ReportTicketDialog
          participantName={support.target.name}
          categoryOptions={support.categoryOptions}
          history={support.history}
          loadingHistory={support.loadingHistory}
          busy={support.busy}
          onClose={support.close}
          onSubmit={support.submit}
        />
      ) : null}

      {support.incidentOpen ? (
        <ReportIncidentDialog
          kindOptions={support.kindOptions}
          severityOptions={support.severityOptions}
          participants={(vm.attendance?.entries ?? []).map((entry) => ({
            participantId: entry.participantId,
            firstName: entry.firstName,
            lastName: entry.lastName,
          }))}
          groupId={session.groupId}
          sessionId={session.id}
          busy={vm.busy || support.busy}
          onClose={support.closeIncidentDialog}
          onSubmit={support.submitIncident}
        />
      ) : null}

      {showProgress ? (
        <SessionProgressPanel
          sessionId={session.id}
          participants={(vm.attendance?.entries ?? []).map((entry) => ({
            participantId: entry.participantId,
            firstName: entry.firstName,
            lastName: entry.lastName,
          }))}
          onClose={() => setShowProgress(false)}
        />
      ) : null}

      {showFinish ? (
        <FinishDialog
          busy={vm.busy}
          initialUnfinished={session.unfinishedNote ?? ""}
          onClose={() => setShowFinish(false)}
          onFinish={async (request) => {
            await vm.finish(request);
            setShowFinish(false);
          }}
        />
      ) : null}
    </>
  );
}

/**
 * Przypomnienie o przerwie liczone od startu zajęć.
 *
 * „Instruktor zapomni o przerwie” to pierwsza pozycja na liście problemów z rozdziału 5
 * dokumentu koncepcyjnego. Okno przerwy istniało od dawna, ale trzeba było o nim pamiętać
 * samemu — czyli dokładnie to, czego dokument się obawia.
 *
 * Świadomie belka, a nie okno modalne: przypomnienie w połowie zajęć nie może przerywać
 * tego, co instruktor właśnie robi. Da się je odrzucić raz i nie wraca.
 */
function BreakReminder({ startedAt }: { startedAt: string | null }) {
  const [due, setDue] = useState(false);
  const [dismissed, setDismissed] = useState(false);

  useEffect(() => {
    if (!startedAt) {
      return undefined;
    }

    function check() {
      const started = new Date(startedAt ?? "").getTime();

      if (Number.isNaN(started)) {
        return;
      }

      setDue(Date.now() - started >= BREAK_REMINDER_MINUTES * 60_000);
    }

    check();
    const timer = window.setInterval(check, 30_000);
    return () => window.clearInterval(timer);
  }, [startedAt]);

  if (!due || dismissed) {
    return null;
  }

  return (
    <div className="break-reminder" role="status">
      <Coffee size={18} aria-hidden="true" />
      <span>Minęło {BREAK_REMINDER_MINUTES} minut zajęć — czas na przerwę.</span>
      <Button variant="secondary" onClick={() => openBreakWindow()}>
        Otwórz okno przerwy
      </Button>
      <button type="button" className="break-reminder-close" onClick={() => setDismissed(true)} aria-label="Odrzuć przypomnienie">
        <X size={16} aria-hidden="true" />
      </button>
    </div>
  );
}

/**
 * Zakończenie zajęć w trzech polach zamiast jednego.
 *
 * Wcześniej wszystko szło do jednego `textarea`: uwagi wewnętrzne, to czego nie zdążyliśmy
 * i ewentualne podsumowanie dla rodzica. Z takiego bloku nie da się nic odczytać ani pokazać
 * w portalu, a instruktor musiał sam pamiętać, żeby te trzy rzeczy w ogóle rozdzielić.
 *
 * Pole dla rodzica jest oznaczone wprost — notatka, o której trzeba pamiętać, że jest
 * wewnętrzna, prędzej czy później zostanie pokazana.
 */
function FinishDialog({
  busy,
  initialUnfinished,
  onClose,
  onFinish,
}: {
  busy: boolean;
  initialUnfinished: string;
  onClose: () => void;
  onFinish: (request: { note: string | null; unfinishedNote: string | null; parentSummary: string | null }) => Promise<void>;
}) {
  const [note, setNote] = useState("");
  const [unfinished, setUnfinished] = useState(initialUnfinished);
  const [parentSummary, setParentSummary] = useState("");

  return (
    <Dialog
      title="Zakończ zajęcia"
      description="Trzy pola trafiają w trzy różne miejsca. Wszystkie są opcjonalne."
      onClose={onClose}
      dismissOnScrim={false}
      wide
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button
            disabled={busy}
            onClick={() =>
              void onFinish({
                note: note.trim() || null,
                unfinishedNote: unfinished.trim() || null,
                parentSummary: parentSummary.trim() || null,
              })
            }
          >
            {busy ? "Zapisywanie…" : "Zapisz i zakończ"}
          </Button>
        </>
      }
    >
      <div className="dialog-consequences">
        <ul>
          <li>Opiekunowie dzieci ze statusem „nieobecność niezgłoszona” dostaną e-mail.</li>
          <li>Materiały lekcji i nagranie staną się widoczne w portalu rodzica.</li>
          <li>Termin przejdzie w status „Zakończone” i policzy się do frekwencji.</li>
        </ul>
      </div>

      <label className="form-field">
        <span>Notatka wewnętrzna — widzisz Ty i administracja</span>
        <textarea
          rows={3}
          value={note}
          maxLength={4000}
          placeholder="Co się udało, kto był aktywny, uwagi organizacyjne…"
          onChange={(event) => setNote(event.target.value)}
        />
      </label>

      <label className="form-field">
        <span>Czego nie zdążyliśmy — podpowiemy to na kolejnym terminie</span>
        <textarea
          rows={2}
          value={unfinished}
          maxLength={2000}
          placeholder="np. została prezentacja efektów i zapisanie projektów"
          onChange={(event) => setUnfinished(event.target.value)}
        />
      </label>

      <label className="form-field">
        <span>Podsumowanie dla rodzica — pojawi się w portalu</span>
        <textarea
          rows={3}
          value={parentSummary}
          maxLength={2000}
          placeholder="Krótko, po ludzku: nad czym pracowaliśmy i co dziecko wyniosło z zajęć."
          onChange={(event) => setParentSummary(event.target.value)}
        />
      </label>
    </Dialog>
  );
}

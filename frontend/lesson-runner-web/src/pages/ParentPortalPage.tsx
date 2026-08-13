import { useState } from "react";
import {
  CalendarDays,
  CalendarX,
  Check,
  Copy,
  FileDown,
  FolderOpen,
  Image,
  Inbox,
  ShieldCheck,
  Sparkles,
  TrendingUp,
  Video,
} from "lucide-react";
import { resolveAssetUrl, saveDownloadedFile } from "../api/client";
import { exportParentScheduleIcs } from "../api/parentApi";
import { Button } from "../components/ui/Button";
import { Dialog } from "../components/ui/Dialog";
import { EmptyState } from "../components/ui/EmptyState";
import { SkeletonList } from "../components/ui/Skeleton";
import { StatusBadge, sessionStatusTone } from "../components/ui/StatusBadge";
import { Tabs } from "../components/ui/Tabs";
import { formatDateTime, formatDayDistance, formatFriendlyDateTime, plural } from "../features/groups/datetime";
import { useParentPortalViewModel } from "../features/parent/useParentPortalViewModel";
import type { ParentView } from "../features/parent/useParentPortalViewModel";
import type { ParentProject, ParentScheduleItem, ParentSessionChild } from "../types/parent";

/**
 * Portal rodzica.
 *
 * Poprzednia wersja pokazywała siedem sekcji rozwiniętych jednocześnie, każdą o równej
 * wadze wizualnej, a wskaźniki liczyły liczbę faktur zamiast kwoty do zapłaty. Rodzic
 * wchodzi tu po jedno pytanie — kiedy są zajęcia i gdzie kliknąć — więc odpowiedź na
 * nie jest teraz dominantą ekranu, a reszta czeka pod zakładkami.
 */
export function ParentPortalPage() {
  const vm = useParentPortalViewModel();
  const [absenceFor, setAbsenceFor] = useState<{ session: ParentScheduleItem; child: ParentSessionChild } | null>(null);

  if (vm.loading) {
    return (
      <section className="page-section">
        <SkeletonList rows={3} label="Ładowanie portalu" />
      </section>
    );
  }

  if (vm.error || !vm.portal) {
    return (
      <section className="page-section">
        <div className="list-state list-state-error" role="alert">
          {vm.error ?? "Brak danych."}
        </div>
      </section>
    );
  }

  const summary = vm.portal.summary;
  const next = vm.schedule[0] ?? null;
  const credits = summary?.availableCredits ?? vm.credits.length;

  return (
    <section className="page-section">
      {next ? (
        <NextSessionHero session={next} onReportAbsence={(child) => setAbsenceFor({ session: next, child })} />
      ) : (
        <EmptyState
          icon={CalendarDays}
          title="Brak zaplanowanych zajęć"
          description="Gdy pojawi się kolejny termin, zobaczysz go tutaj razem z linkiem do spotkania."
        />
      )}

      {/* Cztery liczby, które rodzic sprawdza naprawdę. Wcześniej stało tu
          „Dzieci: 1” i „Rozliczenia: 3” — czyli liczba faktur zamiast kwoty. */}
      <div className="parent-tiles">
        <Tile
          value={summary ? money(summary.outstandingCents, summary.currency) : "—"}
          label={
            summary?.nextDueDate && summary.outstandingCents > 0
              ? `do zapłaty do ${formatDate(summary.nextDueDate)}`
              : "wszystko opłacone"
          }
          alert={Boolean(summary && summary.overdueCents > 0)}
        />
        <Tile value={courseProgressLabel(vm.courseProgress)} label="lekcji za nami" />
        <Tile
          value={String(credits)}
          label={`${plural(credits, "kredyt", "kredyty", "kredytów")} do wykorzystania`}
        />
        <Tile value={`${summary?.attendancePercent ?? 0}%`} label="frekwencja" />
      </div>

      {/* Przełącznik pojawia się dopiero przy rodzeństwie: przy jednym dziecku
          byłby pytaniem bez alternatywy. */}
      {vm.children.length > 1 ? (
        <div className="parent-child-switch" role="group" aria-label="Wybór dziecka">
          <button type="button" aria-pressed={vm.selectedChildId === null} onClick={() => vm.setChild(null)}>
            Wszystkie dzieci
          </button>
          {vm.children.map((child) => (
            <button
              key={child.participantId}
              type="button"
              aria-pressed={vm.selectedChildId === child.participantId}
              onClick={() => vm.setChild(child.participantId)}
            >
              {child.firstName}
            </button>
          ))}
        </div>
      ) : null}

      <Tabs
        ariaLabel="Sekcje portalu"
        value={vm.view}
        onChange={(next) => vm.setView(next as ParentView)}
        tabs={[
          { value: "plan", label: "Plan", count: vm.schedule.length },
          { value: "postepy", label: "Postępy", count: vm.progress.length },
          { value: "materialy", label: "Materiały", count: vm.materials.length },
          { value: "rozliczenia", label: "Rozliczenia", count: vm.invoices.length },
          { value: "zgody", label: "Zgody" },
        ]}
      />

      {vm.view === "plan" ? (
        <PlanTab vm={vm} onReportAbsence={(session, child) => setAbsenceFor({ session, child })} />
      ) : null}
      {vm.view === "postepy" ? <ProgressTab vm={vm} /> : null}
      {vm.view === "materialy" ? <MaterialsTab vm={vm} /> : null}
      {vm.view === "rozliczenia" ? <BillingTab vm={vm} /> : null}
      {vm.view === "zgody" ? <ConsentsTab vm={vm} /> : null}

      {absenceFor ? (
        <AbsenceDialog
          session={absenceFor.session}
          child={absenceFor.child}
          busy={vm.busy}
          onClose={() => setAbsenceFor(null)}
          onSubmit={async (reason) => {
            await vm.submitAbsence(
              absenceFor.session.sessionId,
              absenceFor.child.participantId,
              reason,
              absenceFor.child.firstName,
            );
            setAbsenceFor(null);
          }}
        />
      ) : null}
    </section>
  );
}

type ViewModel = ReturnType<typeof useParentPortalViewModel>;

/* -------------------------------------------------------------------------- */
/* Kafel najbliższych zajęć                                                    */
/* -------------------------------------------------------------------------- */

function NextSessionHero({
  session,
  onReportAbsence,
}: {
  session: ParentScheduleItem;
  onReportAbsence: (child: ParentSessionChild) => void;
}) {
  const [copied, setCopied] = useState(false);
  const pending = (session.children ?? []).filter((child) => !child.absenceReported);

  async function copyLink() {
    if (!session.meetingUrl) {
      return;
    }

    try {
      await navigator.clipboard.writeText(session.meetingUrl);
      setCopied(true);
      window.setTimeout(() => setCopied(false), 2000);
    } catch {
      setCopied(false);
    }
  }

  const lessonLine = [
    session.groupName,
    session.instructorName ? `prowadzi ${session.instructorName}` : null,
    session.sequenceNumber && session.courseLength
      ? `lekcja ${session.sequenceNumber} z ${session.courseLength}`
      : null,
  ]
    .filter(Boolean)
    .join(" · ");

  return (
    <div className="parent-hero">
      <div className="parent-hero-main">
        <span className="parent-hero-when">
          {formatFriendlyDateTime(session.scheduledAt)} · {formatDayDistance(session.scheduledAt)}
        </span>
        <span className="parent-hero-what">{session.lessonTitle ?? "Zajęcia"}</span>
        <span className="parent-hero-sub">{lessonLine}</span>
      </div>

      <div className="parent-hero-actions">
        {session.meetingUrl ? (
          <>
            <a href={session.meetingUrl} target="_blank" rel="noreferrer">
              <Button>
                <Video className="button-icon" aria-hidden="true" />
                Dołącz do zajęć
              </Button>
            </a>
            <Button variant="secondary" onClick={copyLink}>
              {copied ? (
                <Check className="button-icon" aria-hidden="true" />
              ) : (
                <Copy className="button-icon" aria-hidden="true" />
              )}
              {copied ? "Skopiowano" : "Kopiuj link"}
            </Button>
          </>
        ) : (
          // Wcześniej stało tu „Link pojawi się przed zajęciami” — bez terminu,
          // więc rodzic wracał i sprawdzał co jakiś czas.
          <span className="cue-empty">Link będzie aktywny na krótko przed zajęciami.</span>
        )}

        {pending.map((child) => (
          <Button key={child.participantId} variant="ghost" onClick={() => onReportAbsence(child)}>
            <CalendarX className="button-icon" aria-hidden="true" />
            Zgłoś nieobecność: {child.firstName}
          </Button>
        ))}
      </div>
    </div>
  );
}

/* -------------------------------------------------------------------------- */
/* Zakładki                                                                    */
/* -------------------------------------------------------------------------- */

function PlanTab({
  vm,
  onReportAbsence,
}: {
  vm: ViewModel;
  onReportAbsence: (session: ParentScheduleItem, child: ParentSessionChild) => void;
}) {
  if (vm.schedule.length === 0) {
    return <EmptyState icon={CalendarDays} title="Brak nadchodzących terminów" compact />;
  }

  return (
    <div className="parent-card">
      <div className="parent-card-head">
        <h2>Najbliższe zajęcia</h2>
        <Button variant="secondary" onClick={async () => saveDownloadedFile(await exportParentScheduleIcs())}>
          <CalendarDays className="button-icon" aria-hidden="true" />
          Dodaj do kalendarza
        </Button>
      </div>

      {vm.schedule.map((session) => (
        <div className="parent-session" key={session.sessionId}>
          <div className="parent-session-main">
            <strong>{formatFriendlyDateTime(session.scheduledAt)}</strong>
            <small>
              {session.groupName} · {session.lessonTitle ?? "Zajęcia"}
              {session.instructorName ? ` · ${session.instructorName}` : ""}
            </small>
          </div>

          <StatusBadge label={session.statusLabel} tone={sessionStatusTone(session.status)} dot />

          <div className="parent-session-actions">
            {session.meetingUrl ? (
              <a href={session.meetingUrl} target="_blank" rel="noreferrer">
                <Button variant="secondary">
                  <Video className="button-icon" aria-hidden="true" />
                  Dołącz
                </Button>
              </a>
            ) : null}

            {(session.children ?? []).map((child) =>
              child.absenceReported ? (
                <StatusBadge
                  key={child.participantId}
                  label={`${child.firstName}: nieobecność zgłoszona`}
                  tone="neutral"
                />
              ) : (
                <Button key={child.participantId} variant="ghost" onClick={() => onReportAbsence(session, child)}>
                  <CalendarX className="button-icon" aria-hidden="true" />
                  {child.firstName}
                </Button>
              ),
            )}
          </div>
        </div>
      ))}
    </div>
  );
}

function ProgressTab({ vm }: { vm: ViewModel }) {
  if (vm.progress.length === 0 && vm.courseProgress.length === 0) {
    return (
      <EmptyState
        icon={Sparkles}
        title="Postępy pojawią się po pierwszych zajęciach"
        description="Instruktor zapisuje po lekcji, co dziecko zrobiło samodzielnie i nad czym warto popracować."
        compact
      />
    );
  }

  return (
    <>
      {vm.courseProgress.map((course) => (
        <div className="parent-card" key={`${course.participantId}-${course.groupId}`}>
          <div className="parent-card-head">
            <h2>{course.groupName}</h2>
            <span className="cue-empty">
              {course.completedLessons} z {course.totalLessons} lekcji
            </span>
          </div>
          <div className="parent-progress-bar" aria-hidden="true">
            <span
              style={{
                width: `${
                  course.totalLessons === 0
                    ? 0
                    : Math.round((100 * course.completedLessons) / course.totalLessons)
                }%`,
              }}
            />
          </div>
          {course.nextLessonTitle ? (
            <small className="cue-empty">
              Następna lekcja: {course.nextLessonTitle}
              {course.nextSessionAt ? ` — ${formatFriendlyDateTime(course.nextSessionAt)}` : ""}
            </small>
          ) : null}
        </div>
      ))}

      {vm.progress.map((child) => (
        <div className="parent-card" key={child.participantId}>
          <div className="parent-card-head">
            <h2>
              {child.firstName} {child.lastName}
            </h2>
          </div>

          {child.entries.slice(0, 8).map((entry) => (
            <div className="parent-session" key={entry.updatedAt}>
              <div className="parent-session-main">
                <strong>{entry.autonomyLabel}</strong>
                <small>{formatDateTime(entry.updatedAt)}</small>
                {entry.noteForParent ? <small>{entry.noteForParent}</small> : null}
                {entry.nextStep ? <small>Kolejny krok: {entry.nextStep}</small> : null}
              </div>
              {entry.lessonCompleted ? <StatusBadge label="materiał ukończony" tone="success" /> : null}
            </div>
          ))}

          {child.projects.map((project) => (
            <ProjectCard key={project.projectId} project={project} />
          ))}
        </div>
      ))}
    </>
  );
}

/**
 * Karta projektu dziecka.
 *
 * Wcześniej były to trzy przyciski „Wersja 1”, „Wersja 2”, „Wersja 3” — bez dat i bez
 * wskazania, która jest aktualna. Rodzic chce zobaczyć, co dziecko zrobiło, a nie
 * pobrać plik `.sb3`, którego u siebie nie otworzy. Na wierzchu jest więc najnowsza
 * wersja z komentarzem instruktora, a starsze chowają się w historii — zgodnie
 * z zasadą z backendu, że wersje dopisujemy, a nie nadpisujemy.
 */
function ProjectCard({ project }: { project: ParentProject }) {
  const [latest, ...older] = project.versions;

  return (
    <div className="parent-project">
      <div className="parent-project-thumb" aria-hidden="true">
        <FolderOpen size={26} />
      </div>

      <div className="parent-project-main">
        <div>
          <strong>{project.title}</strong>
          {project.description ? <small className="cue-empty"> — {project.description}</small> : null}
        </div>

        {latest ? (
          <>
            <small className="cue-empty">
              Najnowsza wersja {latest.version} · {formatDateTime(latest.submittedAt)}
            </small>
            {latest.instructorComment ? <small>{latest.instructorComment}</small> : null}

            <div className="parent-project-versions">
              {latest.downloadUrl ? (
                <a href={resolveAssetUrl(latest.downloadUrl)} download={latest.fileName ?? undefined}>
                  <Button variant="secondary">
                    <FileDown className="button-icon" aria-hidden="true" />
                    Pobierz projekt
                  </Button>
                </a>
              ) : latest.url ? (
                <a href={latest.url} target="_blank" rel="noreferrer">
                  <Button variant="secondary">Otwórz projekt</Button>
                </a>
              ) : null}
            </div>
          </>
        ) : (
          <small className="cue-empty">Brak przesłanych wersji.</small>
        )}

        {older.length > 0 ? (
          <details className="parent-project-history">
            <summary>
              Historia: {older.length}{" "}
              {plural(older.length, "starsza wersja", "starsze wersje", "starszych wersji")}
            </summary>
            <div className="parent-project-versions">
              {older.map((version) =>
                version.downloadUrl ? (
                  <a
                    key={version.version}
                    href={resolveAssetUrl(version.downloadUrl)}
                    download={version.fileName ?? undefined}
                  >
                    <Button variant="ghost">
                      Wersja {version.version} · {formatDateTime(version.submittedAt)}
                    </Button>
                  </a>
                ) : version.url ? (
                  <a key={version.version} href={version.url} target="_blank" rel="noreferrer">
                    <Button variant="ghost">Wersja {version.version}</Button>
                  </a>
                ) : null,
              )}
            </div>
          </details>
        ) : null}
      </div>
    </div>
  );
}

function MaterialsTab({ vm }: { vm: ViewModel }) {
  if (vm.materials.length === 0) {
    return (
      <EmptyState
        icon={Inbox}
        title="Materiały pojawią się po zajęciach"
        description="Pliki projektu i nagranie udostępniamy dopiero po zakończonej lekcji."
        compact
      />
    );
  }

  return (
    <div className="parent-card">
      <div className="parent-card-head">
        <h2>Do pobrania po zajęciach</h2>
      </div>

      {vm.materials.map((material) => (
        <div className="parent-session" key={material.sessionId}>
          <div className="parent-session-main">
            <strong>{material.lessonTitle ?? "Zajęcia"}</strong>
            <small>
              {material.groupName} · {formatDateTime(material.scheduledAt)}
            </small>
          </div>
          <div className="parent-session-actions">
            {material.files.map((file) => (
              <a key={file.downloadUrl} href={resolveAssetUrl(file.downloadUrl)} download={file.fileName}>
                <Button variant="secondary">
                  <FileDown className="button-icon" aria-hidden="true" />
                  {file.label} ({formatSize(file.sizeBytes)})
                </Button>
              </a>
            ))}
            {material.recordingUrl ? (
              <a href={material.recordingUrl} target="_blank" rel="noreferrer">
                <Button variant="secondary">
                  <Video className="button-icon" aria-hidden="true" />
                  Nagranie
                </Button>
              </a>
            ) : null}
          </div>
        </div>
      ))}
    </div>
  );
}

function BillingTab({ vm }: { vm: ViewModel }) {
  return (
    <>
      {/* Kredyty istniały w systemie od lipca, ale wyłącznie w panelu administratora —
          czyli osoba, której się należały, nie miała jak się o nich dowiedzieć. */}
      {vm.credits.length > 0 ? (
        <div className="parent-card">
          <div className="parent-card-head">
            <h2>Kredyty do wykorzystania</h2>
            <span className="cue-empty">Jeden kredyt = jedne zajęcia</span>
          </div>
          {vm.credits.map((credit) => (
            <div className="parent-session" key={credit.creditId}>
              <div className="parent-session-main">
                <strong>{credit.childName}</strong>
                <small>
                  {credit.reason}
                  {credit.groupName ? ` · ${credit.groupName}` : ""}
                </small>
              </div>
              <StatusBadge
                label={credit.expiresAt ? `ważny do ${formatDate(credit.expiresAt)}` : "bezterminowy"}
                tone="credit"
              />
            </div>
          ))}
        </div>
      ) : null}

      <div className="parent-card">
        <div className="parent-card-head">
          <h2>Rozliczenia</h2>
        </div>

        {vm.invoices.length === 0 ? (
          <EmptyState icon={Inbox} title="Brak dokumentów rozliczeniowych" compact />
        ) : (
          vm.invoices.map((invoice) => (
            <div className="parent-session" key={invoice.invoiceId}>
              <div className="parent-session-main">
                <strong>{money(invoice.amountCents, invoice.currency)}</strong>
                <small>
                  {invoice.number} · {invoice.groupName} · termin {formatDate(invoice.dueDate)}
                </small>
              </div>
              <StatusBadge
                label={invoice.isOverdue ? "Zaległa" : invoice.statusLabel}
                tone={invoice.isOverdue ? "danger" : invoice.status === "paid" ? "success" : "warning"}
              />
            </div>
          ))
        )}
      </div>

      <div className="parent-card">
        <div className="parent-card-head">
          <h2>Frekwencja</h2>
        </div>
        {vm.attendance.length === 0 ? (
          <EmptyState icon={TrendingUp} title="Brak zakończonych zajęć" compact />
        ) : (
          vm.attendance.map((item) => (
            <div className="parent-session" key={`${item.groupId}-${item.participantId ?? ""}`}>
              <div className="parent-session-main">
                <strong>{item.groupName}</strong>
                <small>
                  obecność na {item.presentCount} z {item.heldCount} zajęć
                </small>
              </div>
              <StatusBadge
                label={`${item.ratePercent}%`}
                tone={item.ratePercent >= 75 ? "success" : item.ratePercent >= 50 ? "warning" : "danger"}
              />
            </div>
          ))
        )}
      </div>
    </>
  );
}

/**
 * Zgody opiekuna.
 *
 * Zgoda na wizerunek jest przełącznikiem, bo to opiekun jej udziela i może ją wycofać —
 * przy RODO on jest stroną, a do tej pory było to pole ustawiane wyłącznie przez
 * administratora. Zgoda na przetwarzanie danych zostaje do wglądu: bez niej nie da się
 * prowadzić dziennika ani wysyłać powiadomień, więc jej wycofanie oznacza rozwiązanie
 * umowy, a nie kliknięcie w portalu.
 */
function ConsentsTab({ vm }: { vm: ViewModel }) {
  const consents = vm.portal?.consents ?? [];

  if (consents.length === 0) {
    return <EmptyState icon={ShieldCheck} title="Brak powiązanych dzieci" compact />;
  }

  return (
    <div className="parent-card">
      <div className="parent-card-head">
        <h2>Zgody</h2>
      </div>

      {consents.map((consent) => (
        <div key={consent.participantId}>
          <div className="parent-consent">
            <div className="parent-consent-main">
              <strong>{consent.childName} — przetwarzanie danych</strong>
              <small>
                {consent.dataProcessing
                  ? `Udzielona ${formatDateTime(consent.dataProcessingAt)}. Bez niej nie możemy prowadzić dziennika ani wysyłać powiadomień — wycofanie zgłoś administracji.`
                  : "Brak zgody. Powiadomienia o zajęciach nie będą wysyłane."}
              </small>
            </div>
            <StatusBadge
              label={consent.dataProcessing ? "Udzielona" : "Brak"}
              tone={consent.dataProcessing ? "success" : "danger"}
            />
          </div>

          <div className="parent-consent">
            <div className="parent-consent-main">
              <strong>{consent.childName} — wizerunek</strong>
              <small>
                {consent.image
                  ? `Udzielona ${formatDateTime(consent.imageAt)}. Możesz ją wycofać w każdej chwili.`
                  : "Bez tej zgody nie publikujemy zdjęć ani prac dziecka poza portalem."}
              </small>
            </div>
            <Button
              variant={consent.image ? "secondary" : "primary"}
              disabled={vm.busy}
              onClick={() => void vm.submitConsent(consent.participantId, !consent.image, consent.childName)}
            >
              <Image className="button-icon" aria-hidden="true" />
              {consent.image ? "Wycofaj zgodę" : "Udziel zgody"}
            </Button>
          </div>
        </div>
      ))}
    </div>
  );
}

/* -------------------------------------------------------------------------- */
/* Dialog nieobecności                                                         */
/* -------------------------------------------------------------------------- */

/**
 * Zgłoszenie nieobecności.
 *
 * Wcześniej o powód pytał `window.prompt`, a błąd zgłaszał `window.alert` — surowe okna
 * systemowe bez stylu, bez walidacji i bez informacji, co się właściwie stanie. Był to
 * najbardziej amatorski moment w całej aplikacji, a trafiał dokładnie w klienta szkoły.
 */
function AbsenceDialog({
  session,
  child,
  busy,
  onClose,
  onSubmit,
}: {
  session: ParentScheduleItem;
  child: ParentSessionChild;
  busy: boolean;
  onClose: () => void;
  onSubmit: (reason: string | null) => Promise<void>;
}) {
  const [reason, setReason] = useState("");

  return (
    <Dialog
      title={`Zgłosić nieobecność: ${child.firstName}?`}
      description={`${session.lessonTitle ?? "Zajęcia"} — ${formatFriendlyDateTime(session.scheduledAt)}`}
      onClose={onClose}
      dismissOnScrim={false}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button disabled={busy} onClick={() => void onSubmit(reason.trim() || null)}>
            {busy ? "Zgłaszanie…" : "Zgłoś nieobecność"}
          </Button>
        </>
      }
    >
      <div className="dialog-consequences">
        <ul>
          <li>Instruktor zobaczy nieobecność na liście obecności jeszcze przed zajęciami.</li>
          <li>Nie wyślemy Ci maila o nieobecności — powiadamiamy tylko o niezgłoszonych.</li>
          <li>O odrobieniu albo kredycie decyduje organizator; napisz, jeśli chcesz o to zapytać.</li>
        </ul>
      </div>

      <label className="form-field">
        <span>Powód (opcjonalnie)</span>
        <textarea
          rows={3}
          value={reason}
          maxLength={500}
          placeholder="np. choroba, wyjazd rodzinny"
          onChange={(event) => setReason(event.target.value)}
        />
      </label>
    </Dialog>
  );
}

/* -------------------------------------------------------------------------- */
/* Drobiazgi                                                                   */
/* -------------------------------------------------------------------------- */

function Tile({ value, label, alert = false }: { value: string; label: string; alert?: boolean }) {
  return (
    <div className={`parent-tile${alert ? " parent-tile-alert" : ""}`}>
      <strong>{value}</strong>
      <span>{label}</span>
    </div>
  );
}

function courseProgressLabel(courses: { completedLessons: number; totalLessons: number }[]): string {
  if (courses.length === 0) {
    return "—";
  }

  const completed = courses.reduce((sum, course) => sum + course.completedLessons, 0);
  const total = courses.reduce((sum, course) => sum + course.totalLessons, 0);
  return `${completed} z ${total}`;
}

function money(amountCents: number, currency: string): string {
  return new Intl.NumberFormat("pl-PL", { style: "currency", currency }).format(amountCents / 100);
}

function formatDate(value: string): string {
  const date = new Date(value);
  return Number.isNaN(date.getTime())
    ? value
    : new Intl.DateTimeFormat("pl-PL", { day: "2-digit", month: "2-digit", year: "numeric" }).format(date);
}

function formatSize(sizeBytes: number): string {
  if (sizeBytes < 1024) {
    return `${sizeBytes} B`;
  }

  const kilobytes = sizeBytes / 1024;
  return kilobytes < 1024 ? `${Math.round(kilobytes)} kB` : `${(kilobytes / 1024).toFixed(1)} MB`;
}

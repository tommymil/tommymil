import { useEffect, useState } from "react";
import type { KeyboardEvent as ReactKeyboardEvent, ReactNode } from "react";
import { Coffee, Eye, EyeOff, Pause, Play, RotateCcw } from "lucide-react";
import { Button } from "../../components/ui/Button";
import { resolveAssetUrl } from "../../api/client";
import type { LessonDetails } from "../../types/lesson";
import { SPEECH_PREFIX } from "../import/lessonMarkdown";
import { noteKindLabel, stepTypeLabel } from "../lessons/lessonLabels";
import { usePresenterViewModel } from "./usePresenterViewModel";
import { openBreakWindow } from "./breakWindow";
import { useToast } from "../toast/ToastContext";

/**
 * Cel lekcji i to, co z niej zostaje — pokazywane wtedy, kiedy są potrzebne.
 *
 * Cel wisi przez całe zajęcia, bo odpowiada na pytanie „co jest tu najważniejsze”.
 * Przygotowanie ma sens wyłącznie przed startem, a kryteria i zadanie domowe dopiero na
 * ostatnim kroku — wcześniej byłyby czterema panelami zabierającymi miejsce scenariuszowi.
 */
function LessonIntent({
  lesson,
  atFirst,
  atLast,
}: {
  lesson: LessonDetails;
  atFirst: boolean;
  atLast: boolean;
}) {
  const preparation = atFirst ? lesson.preparation ?? [] : [];
  const criteria = atLast ? lesson.successCriteria ?? [] : [];
  const homework = atLast ? lesson.homework ?? [] : [];

  if (!lesson.objective && preparation.length === 0 && criteria.length === 0 && homework.length === 0) {
    return null;
  }

  return (
    <div className="lesson-intent">
      {lesson.objective ? (
        <p className="lesson-intent-goal">
          <span>Cel</span>
          {lesson.objective}
        </p>
      ) : null}

      <IntentList title="Przygotuj przed zajęciami" items={preparation} />
      <IntentList title="Po zajęciach dziecko potrafi" items={criteria} />
      <IntentList title="Zadanie domowe" items={homework} />
    </div>
  );
}

function IntentList({ title, items }: { title: string; items: string[] }) {
  if (items.length === 0) {
    return null;
  }

  return (
    <div className="lesson-intent-list">
      <strong>{title}</strong>
      <ul>
        {items.map((item, index) => (
          <li key={index}>{item}</li>
        ))}
      </ul>
    </div>
  );
}

/**
 * Kwestia do wypowiedzenia wprost, oznaczona w konspekcie prefiksem `[mów]`.
 *
 * Zwraca samą treść albo `null`. Prefiks jest umową na poziomie tekstu, bo kroki lekcji
 * leżą w bazie jako JSON bez wersji — patrz `SPEECH_PREFIX` w parserze importu.
 */
function asSpeech(line: string): string | null {
  return line.startsWith(SPEECH_PREFIX) ? line.slice(SPEECH_PREFIX.length).trim() : null;
}

function renderInlineMd(text: string): ReactNode {
  const parts = text.split(/\*\*(.+?)\*\*/g);
  if (parts.length === 1) {
    return text;
  }
  return parts.map((part, i) => (i % 2 === 1 ? <strong key={i}>{part}</strong> : part));
}

type PresenterCockpitProps = {
  lessonId: string | undefined;
  /** Termin z grafiku - stan prowadzenia (krok/timery) zapisuje się per-termin. */
  scheduledSessionId?: string;
  /** Dodatkowe kontrolki widoku, np. panel grupy i postępy. */
  extraViewControls?: ReactNode;
  /** Działania organizacyjne dotyczące całego terminu. */
  sessionControls?: ReactNode;
};

export function PresenterCockpit({ lessonId, scheduledSessionId, extraViewControls, sessionControls }: PresenterCockpitProps) {
  const presenter = usePresenterViewModel(lessonId, scheduledSessionId);
  const toast = useToast();
  // Która czynność kroku jest „teraz” - prowadzący klika, by przesuwać wskaźnik.
  const [activeAction, setActiveAction] = useState(0);
  const [copiedDownloadSlot, setCopiedDownloadSlot] = useState<string | null>(null);
  const [selectedImage, setSelectedImage] = useState<{ url: string; caption: string } | null>(null);

  function startBreak() {
    const popup = openBreakWindow();

    if (!popup) {
      toast.error("Przeglądarka zablokowała okno przerwy. Zezwól na wyskakujące okna dla tej strony.");
    }
  }

  async function copyDownloadLink(slot: string, url: string) {
    await navigator.clipboard?.writeText(url);
    setCopiedDownloadSlot(slot);
    toast.success("Skopiowano link pobierania.");
    window.setTimeout(() => setCopiedDownloadSlot(null), 1400);
  }

  useEffect(() => {
    setActiveAction(0);
    setSelectedImage(null);
  }, [presenter.stepIndex]);

  // Powiadamiamy tylko przy wejściu w stan błędu, żeby nie spamować przy każdej
  // kolejnej nieudanej próbie zapisu (debounce odpala się co krok/sekundę).
  useEffect(() => {
    if (presenter.syncError) {
      toast.error("Nie udało się zapisać postępu lekcji. Sprawdź połączenie - prowadzenie lekcji działa dalej lokalnie.");
    }
  }, [presenter.syncError]);

  function handleActionKeyDown(event: ReactKeyboardEvent<HTMLLIElement>, index: number) {
    if (event.key === "Enter" || event.key === " ") {
      event.preventDefault();
      setActiveAction(index);
    }
  }

  useEffect(() => {
    if (!selectedImage) {
      return undefined;
    }

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setSelectedImage(null);
      }
    }

    window.addEventListener("keydown", handleKeyDown);

    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [selectedImage]);

  if (presenter.loading) {
    return <div className="list-state">Ładowanie lekcji…</div>;
  }

  if (presenter.error || !presenter.lesson || !presenter.currentStep) {
    return <div className="list-state list-state-error">{presenter.error ?? "Nie znaleziono lekcji."}</div>;
  }

  const step = presenter.currentStep;

  // Wszystkie obrazy kroku (wrzutki/screeny) - niezależnie czy podpięte jako materiał czy zasób -
  // trafiają na duży panel „Na ekranie”.
  const images = [
    ...step.studentItems
      .filter((item) => item.kind === "image" && item.url)
      .map((item, index) => ({ key: `student-${index}`, url: item.url as string, caption: item.caption ?? "" })),
    ...step.resources
      .filter((resource) => resource.kind === "image" && resource.url)
      .map((resource, index) => ({ key: `resource-${index}`, url: resource.url as string, caption: resource.label ?? "" })),
  ];

  const textInserts = step.studentItems.filter((item) => item.kind !== "image");
  const hasMaterials = textInserts.length > 0 || step.resources.some((resource) => resource.kind !== "image");

  const projectDownloads = [
    { slot: "starter", title: "Lekcja startowa", file: presenter.lesson.projectFiles?.starter },
    { slot: "final", title: "Lekcja końcowa", file: presenter.lesson.projectFiles?.final },
  ].filter((item) => item.file?.downloadUrl);

  return (
    <section className="presenter-page presenter-workspace">
      <div className="presenter-topbar">
        <div>
          <span className="eyebrow">Zaplecze prowadzenia</span>
          <h1>{presenter.lesson.title}</h1>
          <p>
            {presenter.lesson.subject} · krok {presenter.stepLabel} · {presenter.progressPercent}%
          </p>
        </div>
        <div className="presenter-clock">
          <span className="timer-label">Czas lekcji</span>
          <strong className="timer">{presenter.elapsedTotalLabel}</strong>
          <small>plan {presenter.plannedTotalLabel}</small>
          {presenter.syncError ? (
            <small className="sync-warning" role="status">Zapis postępu wstrzymany</small>
          ) : null}
        </div>
      </div>

      <div className="presenter-command-bar" aria-label="Sterowanie zajęciami">
        <div className="presenter-control-group presenter-control-group-primary" role="group" aria-labelledby="control-flow-label">
          <span className="presenter-control-label" id="control-flow-label">Przebieg zajęć</span>
          <div className="presenter-control-actions">
            <Button onClick={presenter.toggleRunning}>
              {presenter.running ? <Pause className="button-icon" aria-hidden="true" /> : <Play className="button-icon" aria-hidden="true" />}
              {presenter.running ? "Pauza" : "Start"}
            </Button>
            <Button variant="secondary" onClick={startBreak}>
              <Coffee className="button-icon" aria-hidden="true" />
              PRZERWA
            </Button>
            <Button variant="ghost" onClick={presenter.resetTimers}>
              <RotateCcw className="button-icon" aria-hidden="true" />
              Reset czasu
            </Button>
          </div>
        </div>

        <div className="presenter-control-group" role="group" aria-labelledby="control-view-label">
          <span className="presenter-control-label" id="control-view-label">Panele i materiały</span>
          <div className="presenter-control-actions">
            <Button variant="secondary" onClick={presenter.toggleStudent}>
              {presenter.showStudent ? <EyeOff className="button-icon" aria-hidden="true" /> : <Eye className="button-icon" aria-hidden="true" />}
              {presenter.showStudent ? "Ukryj materiały" : "Pokaż materiały"}
            </Button>
            <Button variant="secondary" onClick={presenter.toggleNotes}>
              {presenter.showNotes ? <EyeOff className="button-icon" aria-hidden="true" /> : <Eye className="button-icon" aria-hidden="true" />}
              {presenter.showNotes ? "Ukryj wskazówki" : "Pokaż wskazówki"}
            </Button>
            {extraViewControls}
          </div>
        </div>

        {sessionControls ? (
          <div className="presenter-control-group presenter-control-group-session" role="group" aria-labelledby="control-session-label">
            <span className="presenter-control-label" id="control-session-label">Organizacja zajęć</span>
            <div className="presenter-control-actions">{sessionControls}</div>
          </div>
        ) : null}
      </div>

      {projectDownloads.length > 0 ? (
        <div className="lesson-downloads-panel" aria-label="Pliki lekcji do pobrania">
          <div>
            <strong>Pliki dla ucznia</strong>
            <span>Link możesz wysłać dziecku. Po otwarciu w przeglądarce plik zacznie się pobierać.</span>
          </div>
          <div className="lesson-downloads-actions">
            {projectDownloads.map(({ slot, title, file }) => {
              const downloadUrl = resolveAssetUrl(file?.downloadUrl);

              return (
                <div key={slot} className="lesson-download-item">
                  <a className="button button-secondary" href={downloadUrl}>
                    {file?.label || title}
                  </a>
                  <Button variant="secondary" onClick={() => copyDownloadLink(slot, downloadUrl)}>
                    {copiedDownloadSlot === slot ? "Skopiowano" : "Kopiuj link"}
                  </Button>
                </div>
              );
            })}
          </div>
        </div>
      ) : null}

      <div className="presenter-progress">
        <span style={{ width: `${presenter.progressPercent}%` }} />
      </div>

      <LessonIntent lesson={presenter.lesson} atFirst={presenter.atFirst} atLast={presenter.atLast} />

      <div className="presenter-grid">
        <aside className="presenter-rail">
          {presenter.lesson.steps.map((railStep, index) => (
            <button
              key={railStep.id}
              className={`rail-step ${index === presenter.stepIndex ? "rail-step-active" : ""}`}
              onClick={() => presenter.goToStep(index)}
            >
              <span>{index + 1}</span>
              <strong>{railStep.title}</strong>
              <small>{railStep.durationMinutes} min</small>
            </button>
          ))}
        </aside>

        <section className="presenter-cues">
          <span className="subject-pill">{stepTypeLabel(step.type)}</span>
          <h2>{step.title}</h2>

          <div className="cue-block">
            <h3>Co robić teraz</h3>
            {step.script.length > 0 ? (
              <ol className="action-list">
                {step.script.map((paragraph, index) => {
                  const state =
                    index === activeAction ? "action-current"
                    : index < activeAction ? "action-done"
                    : "action-todo";

                  const speech = asSpeech(paragraph);

                  return (
                    <li
                      key={index}
                      className={`action-item ${state}${speech ? " action-speech" : ""}`}
                      aria-current={index === activeAction ? "step" : undefined}
                      role="button"
                      tabIndex={0}
                      onClick={() => setActiveAction(index)}
                      onKeyDown={(event) => handleActionKeyDown(event, index)}
                    >
                      {speech ? (
                        <>
                          <span className="action-speech-label">powiedz</span>{" "}
                          <q>{renderInlineMd(speech)}</q>
                        </>
                      ) : (
                        renderInlineMd(paragraph)
                      )}
                    </li>
                  );
                })}
              </ol>
            ) : (
              <p className="cue-empty">Brak instrukcji dla tego kroku.</p>
            )}
          </div>

          {presenter.showNotes && step.notes.length > 0 ? (
            <div className="cue-block">
              <h3>Wskazówki</h3>
              {step.notes.map((note, index) => (
                <div key={`${note.kind}-${index}`} className={`note-item note-${note.kind}`}>
                  <strong>{noteKindLabel(note.kind)}</strong>
                  <p>{note.text}</p>
                </div>
              ))}
            </div>
          ) : null}

          {presenter.showStudent && hasMaterials ? (
            <div className="cue-block">
              <h3>Materiały pomocnicze</h3>
              {textInserts.map((item, index) => (
                <p key={`text-${index}`} className="material-text">
                  {item.text ?? item.caption ?? item.url}
                </p>
              ))}
              {step.resources.map((resource, index) => {
                if (resource.kind === "link") {
                  return (
                    <a key={index} className="material-link" href={resource.url ?? "#"} target="_blank" rel="noreferrer">
                      {resource.label ?? resource.url}
                    </a>
                  );
                }

                if (resource.kind === "code") {
                  return (
                    <div key={index} className="resource-item">
                      <div className="resource-code-header">
                        {/* Etykieta wygrywa z językiem: prowadzącemu mówi więcej
                            „Powitanie (Wygląd)” niż „scratch”. */}
                        <strong>{resource.label ?? resource.language ?? "Kod"}</strong>
                        <button onClick={() => presenter.copyResourceCode(index, resource.code)}>
                          {presenter.copiedResourceIndex === index ? "Skopiowano" : "Kopiuj"}
                        </button>
                      </div>
                      <pre>{resource.code}</pre>
                    </div>
                  );
                }

                if (resource.kind === "file" && resource.url) {
                  return (
                    <a key={index} className="material-link" href={resolveAssetUrl(resource.url)} target="_blank" rel="noreferrer">
                      {resource.label ?? "Pobierz plik"}
                    </a>
                  );
                }

                return null;
              })}
            </div>
          ) : null}

          <div className="presenter-actions">
            <Button variant="secondary" onClick={presenter.previousStep} disabled={presenter.atFirst}>
              Poprzedni
            </Button>
            <Button onClick={presenter.nextStep} disabled={presenter.atLast}>
              Następny krok
            </Button>
          </div>
        </section>

        <aside className="presenter-screen">
          <h3 className="screen-title">Na ekranie / wrzutki</h3>
          {images.length > 0 ? (
            <div className="screen-gallery">
              {images.map((image) => (
                <figure key={image.key} className="screen-figure">
                  <button
                    className="screen-image-button"
                    type="button"
                    onClick={() => setSelectedImage({ url: resolveAssetUrl(image.url), caption: image.caption })}
                  >
                    <img src={resolveAssetUrl(image.url)} alt={image.caption} />
                  </button>
                  {image.caption ? <figcaption>{image.caption}</figcaption> : null}
                </figure>
              ))}
            </div>
          ) : (
            <div className="screen-empty">
              Brak wrzutek dla tego kroku. Dodaj screeny lub obrazy w edytorze, żeby pojawiły się tutaj.
            </div>
          )}
        </aside>
      </div>

      {selectedImage ? (
        <div
          className="gallery-overlay"
          role="dialog"
          aria-modal="true"
          aria-label={selectedImage.caption || "Podgląd obrazu"}
          onClick={() => setSelectedImage(null)}
        >
          <figure className="gallery-modal" onClick={(event) => event.stopPropagation()}>
            <button
              className="gallery-close"
              type="button"
              aria-label="Zamknij podgląd"
              onClick={() => setSelectedImage(null)}
            >
              ×
            </button>
            <img src={selectedImage.url} alt={selectedImage.caption} />
            {selectedImage.caption ? <figcaption>{selectedImage.caption}</figcaption> : null}
          </figure>
        </div>
      ) : null}
    </section>
  );
}

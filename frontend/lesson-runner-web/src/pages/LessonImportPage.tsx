import { Button } from "../components/ui/Button";
import { plural } from "../features/groups/datetime";
import { stepTypeLabel } from "../features/lessons/lessonLabels";
import { useLessonImportViewModel } from "../features/import/useLessonImportViewModel";
import { STANDARD_LESSON_MINUTES, blockMinutes } from "../features/import/lessonMarkdown";
import type { ParseIssue } from "../features/import/lessonMarkdown";

const formatExample = `# Tytuł lekcji
Subject: Scratch
Level: Poziom 1
Czas: 95 min
Tags: Pętle, Sterowanie
Opis: Krótki opis lekcji.
Cel: Czego dziecko ma się nauczyć.

### Po zajęciach dziecko potrafi
- użyć bloku „powtórz”

### Przygotuj przed zajęciami
- otwarty projekt startowy

### Zadanie domowe
- pokaż rodzicom swój projekt

## [concept] Tytuł kroku (8 min)

### Co robić teraz
- [mów] Kwestia do wypowiedzenia wprost
- Zwykła czynność

### Wskazówki
- [błąd] Częsty błąd
- [dla szybszych] Co dać tym, którzy skończyli
- [gdy nie zdążysz] Co wolno wyciąć
- [tempo] Uwaga o rytmie lekcji
- [podpowiedź] Zwykła podpowiedź

### Materiały
- [link] Scratch | https://scratch.mit.edu
- [kod] Powitanie (Wygląd) | scratch:
  \`\`\`
  kiedy klawisz [prawo] naciśnięty
    przesuń o (10) kroków
  \`\`\`
- ![Podpis screena](/uploads/bloczki.png)
- Tekstowa wrzutka na ekran

## [przerwa] Przerwa (5 min)
   Zajęcia mają kształt 45 min + 5 min przerwy + 45 min, razem 95.

## [summary] Podsumowanie (5 min)`;

export function LessonImportPage() {
  const importer = useLessonImportViewModel();
  const parsed = importer.parsed;
  const blocked = importer.errors.length > 0 && !importer.acceptErrors;

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">Import</span>
          <h1>Import konspektu z Markdown</h1>
          <p>Wklej lub wczytaj plik. Po imporcie otworzy się edytor, w którym dodasz screeny ze Scratcha.</p>
        </div>
        <div className="editor-header-actions">
          <Button onClick={importer.importLesson} disabled={!importer.canImport}>
            {importer.importing ? "Importowanie…" : "Utwórz konspekt"}
          </Button>
        </div>
      </div>

      {importer.error ? <div className="list-state list-state-error">{importer.error}</div> : null}

      <div className="editor-grid">
        <section className="editor-panel">
          <h2>Plik konspektu</h2>
          <label className="upload-button">
            Wczytaj plik (.md)
            <input
              type="file"
              accept=".md,.markdown,.txt,text/markdown,text/plain"
              onChange={(event) => {
                const file = event.target.files?.[0];
                if (file) {
                  void importer.loadFile(file);
                }
                event.target.value = "";
              }}
            />
          </label>
          <label>
            Treść Markdown
            <textarea
              className="import-textarea"
              value={importer.text}
              onChange={(event) => importer.setText(event.target.value)}
              placeholder={formatExample}
              spellCheck={false}
            />
          </label>
          <details className="import-help">
            <summary>Ściąga formatu</summary>
            <pre>{formatExample}</pre>
          </details>
        </section>

        <section className="editor-panel">
          <h2>Podgląd</h2>
          {!parsed ? (
            <p className="empty-inline">Wklej treść, żeby zobaczyć podgląd konspektu.</p>
          ) : (
            <>
              <div className="import-summary">
                <h3>{parsed.lesson.title || "(brak tytułu)"}</h3>
                <p>
                  {[parsed.lesson.subject, parsed.lesson.level].filter(Boolean).join(" - ") || "brak metadanych"}
                  {parsed.lesson.tags.length > 0 ? ` - tagi: ${parsed.lesson.tags.join(", ")}` : ""}
                </p>
                {parsed.lesson.description ? <p>{parsed.lesson.description}</p> : null}
                {/* Cel i listy pokazujemy w podglądzie, bo to nowe pola formatu -
                    bez nich autor nie ma jak sprawdzić, czy w ogóle zostały rozpoznane. */}
                {parsed.lesson.objective ? (
                  <p className="import-objective">
                    <span>Cel</span>
                    {parsed.lesson.objective}
                  </p>
                ) : null}
                <IntentSummary label="Po zajęciach dziecko potrafi" items={parsed.lesson.successCriteria} />
                <IntentSummary label="Przygotuj przed zajęciami" items={parsed.lesson.preparation} />
                <IntentSummary label="Zadanie domowe" items={parsed.lesson.homework} />
              </div>

              {parsed.lesson.steps.length > 0 ? <TimeBudget parsed={parsed} /> : null}

              {parsed.lesson.steps.length > 0 ? (
                <ol className="import-step-list">
                  {parsed.lesson.steps.map((step, index) => (
                    <li key={index}>
                      <strong>{step.title}</strong>
                      <span>
                        {stepTypeLabel(step.type)} · {step.durationMinutes} min · {step.script.length} pkt ·
                        {" "}{step.studentItems.length} {plural(step.studentItems.length, "wrzutka", "wrzutki", "wrzutek")} ·
                        {" "}{step.notes.length} {plural(step.notes.length, "wskazówka", "wskazówki", "wskazówek")} ·
                        {" "}{step.resources.length} {plural(step.resources.length, "materiał", "materiały", "materiałów")}
                      </span>
                    </li>
                  ))}
                </ol>
              ) : (
                <p className="empty-inline">Brak rozpoznanych kroków.</p>
              )}

              {importer.errors.length > 0 ? (
                <div className="import-issues import-issues-error">
                  <h3>Ta treść nie trafi do konspektu ({importer.errors.length})</h3>
                  <IssueList issues={importer.errors} />
                  {/* Import zostaje możliwy - czasem świadomie zostawia się w pliku fragment
                      roboczy. Ale musi być decyzją, a nie skutkiem nieprzeczytania listy. */}
                  <label className="checkbox-field import-accept">
                    <input
                      type="checkbox"
                      checked={importer.acceptErrors}
                      onChange={(event) => importer.setAcceptErrors(event.target.checked)}
                    />
                    <span>Wiem, że powyższe fragmenty przepadną — importuj mimo to.</span>
                  </label>
                </div>
              ) : null}

              {importer.notices.length > 0 ? (
                <div className="import-issues import-issues-notice">
                  <h3>Przyjęte założenia i uwagi do planu ({importer.notices.length})</h3>
                  <IssueList issues={importer.notices} />
                </div>
              ) : null}

              {blocked ? (
                <p className="cue-empty">
                  Zaznacz zgodę powyżej albo popraw plik — dopiero wtedy utworzysz konspekt.
                </p>
              ) : null}
            </>
          )}
        </section>
      </div>
    </section>
  );
}

/**
 * Budżet czasu zajęć.
 *
 * Jedyna liczba, którą prowadzący zna z góry, to długość zajęć. Do tej pory podgląd jej
 * nie pokazywał, więc konspekt sumujący się do 25 minut przy zajęciach 90-minutowych
 * importował się bez słowa i rozjazd wychodził dopiero na lekcji.
 */
function TimeBudget({ parsed }: { parsed: NonNullable<ReturnType<typeof useLessonImportViewModel>["parsed"]> }) {
  const { totalMinutes, plannedMinutes } = parsed;

  const blocks = blockMinutes(parsed.lesson);
  const blockLine = `Bloki pracy: ${blocks.join(" min · przerwa · ")} min`;

  if (plannedMinutes === null || plannedMinutes <= 0) {
    return (
      <p className="import-time">
        Suma kroków: <strong>{totalMinutes} min</strong>. {blockLine}. Dopisz{" "}
        <code>Czas: {STANDARD_LESSON_MINUTES} min</code> w metadanych, żeby sprawdzić plan względem
        długości zajęć.
      </p>
    );
  }

  const drift = totalMinutes - plannedMinutes;
  const off = Math.abs(drift) / plannedMinutes > 0.1;
  const fill = Math.min(100, Math.round((100 * totalMinutes) / plannedMinutes));

  return (
    <div className={`import-time${off ? " import-time-off" : ""}`}>
      <p>
        Suma kroków: <strong>{totalMinutes} min</strong> z zaplanowanych {plannedMinutes} min
        {off ? ` (${drift > 0 ? "+" : ""}${drift} min)` : " — zgadza się"}
      </p>
      {/* Sama suma nie wystarcza: 95 minut w jednym ciągu i 45 + przerwa + 45 to dwa
          różne konspekty, a różnicę widać dopiero w rozbiciu na bloki. */}
      <p className="import-blocks">{blockLine}</p>
      <div className="import-time-bar" aria-hidden="true">
        <span style={{ width: `${fill}%` }} />
      </div>
    </div>
  );
}

function IntentSummary({ label, items }: { label: string; items?: string[] | null }) {
  if (!items || items.length === 0) {
    return null;
  }

  return (
    <p className="import-intent">
      <strong>{label}:</strong> {items.join(" · ")}
    </p>
  );
}

function IssueList({ issues }: { issues: ParseIssue[] }) {
  return (
    <ul>
      {issues.map((issue, index) => (
        <li key={index}>
          {issue.line !== null ? <span className="import-issue-line">linia {issue.line}</span> : null}
          {/* Odstęp jest znakiem, nie tylko marginesem - inaczej po skopiowaniu listy
              i w czytniku ekranu wychodzi „linia 7Nieznana sekcja”. */}
          {issue.line !== null ? " " : null}
          {issue.message}
        </li>
      ))}
    </ul>
  );
}

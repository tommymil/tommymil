import {
  ArrowDown,
  ArrowUp,
  BookOpenText,
  CheckCircle2,
  Clock3,
  FileCode2,
  FileText,
  ImagePlus,
  LinkIcon,
  ListChecks,
  NotebookPen,
  PackagePlus,
  Plus,
  Save,
  Send,
  Sparkles,
  Trash2,
  UploadCloud,
  X,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";
import { useParams } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { resolveAssetUrl } from "../api/client";
import { resourceKindLabel, stepTypeLabel } from "../features/lessons/lessonLabels";
import { useLessonEditorViewModel } from "../features/editor/useLessonEditorViewModel";

type SubItemControlsProps = {
  index: number;
  count: number;
  /** Mianownik — trafia do „{nazwa} 1 w górę”. */
  label: string;
  /** Biernik — trafia do „Usuń {nazwę} 1”. Jednym stringiem się tego nie da:
   *  „Usuń Wrzutka 1” i „Wskazówkę 1 w górę” to ta sama pomyłka co „w sobota o 17:00”.
   *  Rzeczowniki męskie nieżywotne („Materiał”) mają obie formy równe. */
  labelAccusative: string;
  onMove: (index: number, direction: -1 | 1) => void;
  onRemove: (index: number) => void;
};

function SubItemControls({ index, count, label, labelAccusative, onMove, onRemove }: SubItemControlsProps) {
  return (
    <div className="sub-item-controls">
      <Button
        variant="secondary"
        aria-label={`${label} ${index + 1} w górę`}
        disabled={index === 0}
        onClick={() => onMove(index, -1)}
      >
        <ArrowUp className="button-icon" aria-hidden="true" />
      </Button>
      <Button
        variant="secondary"
        aria-label={`${label} ${index + 1} w dół`}
        disabled={index === count - 1}
        onClick={() => onMove(index, 1)}
      >
        <ArrowDown className="button-icon" aria-hidden="true" />
      </Button>
      <Button variant="secondary" aria-label={`Usuń ${labelAccusative} ${index + 1}`} onClick={() => onRemove(index)}>
        <X className="button-icon" aria-hidden="true" />
      </Button>
    </div>
  );
}

export function LessonEditorPage() {
  const { lessonId } = useParams();
  const editor = useLessonEditorViewModel(lessonId);
  const totalExtras = editor.steps.reduce(
    (sum, step) => sum + step.studentItems.length + step.notes.length + step.resources.length,
    0,
  );

  if (editor.loading) {
    return (
      <section className="page-section">
        <div className="list-state">Ładowanie lekcji…</div>
      </section>
    );
  }

  return (
    <section className="page-section editor-shell lesson-builder">
      <div className="page-header editor-hero">
        <div className="editor-hero-copy">
          <span className="eyebrow">Edytor</span>
          <h1>{editor.isEditMode ? "Edycja konspektu" : "Nowy konspekt"}</h1>
          <p>
            Zbuduj konspekt z metadanych, kroków prowadzenia, materiałów dla uczniów i wskazówek dla
            instruktora.
          </p>
        </div>

        <div className="editor-header-actions">
          {editor.lesson ? (
            <>
              <Button variant="secondary" onClick={editor.markForReview} disabled={!editor.canChangeStatus}>
                <Send className="button-icon" aria-hidden="true" />
                Do sprawdzenia
              </Button>
              <Button variant="secondary" onClick={editor.publish} disabled={!editor.canChangeStatus}>
                <CheckCircle2 className="button-icon" aria-hidden="true" />
                Publikuj
              </Button>
            </>
          ) : null}
          <Button onClick={editor.saveLesson} disabled={editor.saving}>
            <Save className="button-icon" aria-hidden="true" />
            {editor.saving ? "Zapisywanie…" : "Zapisz"}
          </Button>
        </div>
      </div>

      {editor.error ? <div className="list-state list-state-error">{editor.error}</div> : null}

      <div className="lesson-builder-layout">
        <aside className="lesson-builder-side">
          <section className="editor-panel editor-meta-panel lesson-builder-meta">
            <PanelTitle icon={NotebookPen} title="Podstawy konspektu" />
            <label>
              Tytuł
              <input
                required
                value={editor.meta.title}
                onChange={(event) => editor.updateMeta("title", event.target.value)}
                placeholder="np. Pierwsza gra: ruch postaci"
              />
            </label>
            <label>
              Kolejność
              <input
                type="number"
                min={1}
                value={editor.meta.order}
                onChange={(event) => editor.updateMeta("order", event.target.value)}
                placeholder="np. 1 - decyduje o pozycji na liście"
              />
            </label>
            <label>
              Przedmiot
              <input value={editor.meta.subject} onChange={(event) => editor.updateMeta("subject", event.target.value)} />
            </label>
            <label>
              Poziom
              <input value={editor.meta.level} onChange={(event) => editor.updateMeta("level", event.target.value)} />
            </label>
            <label>
              Opis
              <textarea
                required
                value={editor.meta.description}
                onChange={(event) => editor.updateMeta("description", event.target.value)}
                placeholder="Krótki opis lekcji widoczny w bibliotece"
              />
            </label>
            <label>
              Tagi
              <input
                value={editor.meta.tags}
                onChange={(event) => editor.updateMeta("tags", event.target.value)}
                placeholder="Pętle, Sterowanie, Scratch"
              />
            </label>

            {/* Opis mówi, czym lekcja jest w bibliotece. Poniższe cztery pola mówią,
                po co ją prowadzimy - i to one trafiają do kokpitu w trakcie zajęć. */}
            <label>
              Cel lekcji
              <input
                value={editor.meta.objective}
                onChange={(event) => editor.updateMeta("objective", event.target.value)}
                placeholder="Czego dziecko ma się nauczyć - jedno zdanie"
              />
            </label>
            <label>
              Po zajęciach dziecko potrafi
              <textarea
                value={editor.meta.successCriteria}
                onChange={(event) => editor.updateMeta("successCriteria", event.target.value)}
                placeholder={"Jedna umiejętność w linii, np.\nużyć bloku „powtórz”"}
              />
            </label>
            <label>
              Przygotuj przed zajęciami
              <textarea
                value={editor.meta.preparation}
                onChange={(event) => editor.updateMeta("preparation", event.target.value)}
                placeholder={"Jedna rzecz w linii, np.\notwarty projekt startowy"}
              />
            </label>
            <label>
              Zadanie domowe
              <textarea
                value={editor.meta.homework}
                onChange={(event) => editor.updateMeta("homework", event.target.value)}
                placeholder={"Jedna pozycja w linii, np.\npokaż rodzicom taniec duszka"}
              />
            </label>
          </section>

          <section className="editor-panel lesson-builder-checklist">
            <PanelTitle icon={BookOpenText} title="Co warto uzupełnić" />
            <ChecklistItem done={Boolean(editor.meta.title.trim())} label="Tytuł i opis lekcji" />
            <ChecklistItem done={editor.steps.length > 0} label="Minimum jeden krok prowadzenia" />
            <ChecklistItem done={editor.totalDuration > 0} label="Realny czas trwania" />
            <ChecklistItem done={totalExtras > 0} label="Materiały lub wskazówki" />
            <ChecklistItem
              done={Boolean(editor.projectFiles.starter || editor.projectFiles.final)}
              label="Plik startowy lub końcowy"
            />
          </section>

          <section className="editor-panel editor-meta-panel">
            <PanelTitle icon={FileText} title="Pliki dla ucznia" />
            <ProjectFileSlot
              title="Lekcja startowa"
              description="Projekt do rozpoczęcia pracy, np. Scratch z samymi grafikami."
              file={editor.projectFiles.starter}
              uploading={editor.uploading}
              onUpload={(file) => editor.uploadProjectFile("starter", file)}
              onLabelChange={(label) => editor.updateProjectFile("starter", { label })}
              onRemove={() => editor.removeProjectFile("starter")}
            />
            <ProjectFileSlot
              title="Lekcja końcowa"
              description="Gotowy projekt wzorcowy do pobrania po zajęciach."
              file={editor.projectFiles.final}
              uploading={editor.uploading}
              onUpload={(file) => editor.uploadProjectFile("final", file)}
              onLabelChange={(label) => editor.updateProjectFile("final", { label })}
              onRemove={() => editor.removeProjectFile("final")}
            />
          </section>

          <section className="editor-panel">
            <PanelTitle icon={ListChecks} title="Stan konspektu" />
            <div className="editor-stats" aria-label="Stan konspektu">
              <EditorStat icon={ListChecks} label="Kroki" value={editor.steps.length} />
              <EditorStat icon={Clock3} label="Czas" value={`${editor.totalDuration} min`} />
              <EditorStat icon={Sparkles} label="Materiały" value={totalExtras} />
            </div>
          </section>
        </aside>

        <section className="editor-panel lesson-step-panel">
          <div className="lesson-step-panel-head">
            <PanelTitle
              icon={PackagePlus}
              title={editor.isEditingStep ? `Edycja kroku ${(editor.editingStepIndex ?? 0) + 1}` : "Dodaj krok"}
            />
            <span className="status-pill status-planned">{stepTypeLabel(editor.stepForm.type)}</span>
          </div>

          <div className="step-core-grid">
            <label>
              Typ
              <select value={editor.stepForm.type} onChange={(event) => editor.updateStep("type", event.target.value)}>
                <option value="intro">Wprowadzenie</option>
                <option value="review">Powtórka</option>
                <option value="concept">Nowy koncept</option>
                <option value="demo">Demo na żywo</option>
                <option value="guided">Ćwiczenie z prowadzeniem</option>
                <option value="challenge">Samodzielne wyzwanie</option>
                <option value="break">Przerwa</option>
                <option value="summary">Podsumowanie</option>
              </select>
            </label>
            <label>
              Czas w minutach
              <input
                min={1}
                required
                type="number"
                value={editor.stepForm.durationMinutes}
                onChange={(event) => editor.updateStep("durationMinutes", Number(event.target.value))}
              />
            </label>
            <label className="step-title-field">
              Tytuł kroku
              <input
                required
                value={editor.stepForm.title}
                onChange={(event) => editor.updateStep("title", event.target.value)}
                placeholder="np. Pętla zawsze i ruch"
              />
            </label>
          </div>

          <label>
            Co robić teraz
            <textarea
              value={editor.stepForm.script}
              onChange={(event) => editor.updateStep("script", event.target.value)}
              placeholder="Kolejne czynności - każdy punkt w osobnej linii (np. Weź blok 'zawsze')"
            />
          </label>

          <div className="lesson-builder-sections">
            <div className="sub-editor">
              <PanelTitle
                icon={ImagePlus}
                title={`Na ekranie ucznia (${editor.stepForm.studentItems.length})`}
                compact
              />
              {editor.stepForm.studentItems.length > 0 ? (
                <ul className="sub-item-list">
                  {editor.stepForm.studentItems.map((item, index) => (
                    <li key={index} className="sub-item">
                      <div className="sub-item-fields">
                        {item.kind === "image" && item.url ? (
                          <img className="sub-item-thumb" src={resolveAssetUrl(item.url)} alt={item.caption ?? ""} />
                        ) : null}
                        {item.kind === "image" ? (
                          <input
                            aria-label={`Podpis obrazu ${index + 1}`}
                            value={item.caption ?? ""}
                            onChange={(event) => editor.updateStudentItem(index, { caption: event.target.value })}
                            placeholder="Podpis obrazu"
                          />
                        ) : (
                          <textarea
                            aria-label={`Materiał ucznia ${index + 1}`}
                            value={item.text ?? ""}
                            onChange={(event) => editor.updateStudentItem(index, { text: event.target.value })}
                          />
                        )}
                      </div>
                      <SubItemControls
                        index={index}
                        count={editor.stepForm.studentItems.length}
                        label="Wrzutka"
                        labelAccusative="Wrzutkę"
                        onMove={editor.moveStudentItem}
                        onRemove={editor.removeStudentItem}
                      />
                    </li>
                  ))}
                </ul>
              ) : null}
              <label>
                Nowa wrzutka tekstowa
                <textarea
                  value={editor.stepForm.studentText}
                  onChange={(event) => editor.updateStep("studentText", event.target.value)}
                  placeholder="Krótki tekst do pokazania lub przypomnienia"
                />
              </label>
              <div className="sub-item-actions">
                <Button variant="secondary" onClick={editor.addStudentItem}>
                  <Plus className="button-icon" aria-hidden="true" />
                  Dodaj tekst
                </Button>
                <label className="upload-button">
                  <UploadCloud className="button-icon" aria-hidden="true" />
                  {editor.uploading ? "Przesyłanie…" : "Dodaj obraz / screen"}
                  <input
                    type="file"
                    accept="image/png,image/jpeg,image/gif,image/webp"
                    disabled={editor.uploading}
                    onChange={(event) => {
                      const file = event.target.files?.[0];
                      if (file) {
                        void editor.uploadStudentImage(file);
                      }
                      event.target.value = "";
                    }}
                  />
                </label>
              </div>
            </div>

            <div className="sub-editor">
              <PanelTitle icon={Sparkles} title={`Wskazówki (${editor.stepForm.notes.length})`} compact />
              {editor.stepForm.notes.length > 0 ? (
                <ul className="sub-item-list">
                  {editor.stepForm.notes.map((note, index) => (
                    <li key={index} className="sub-item">
                      <div className="sub-item-fields">
                        <select
                          aria-label={`Rodzaj notatki ${index + 1}`}
                          value={note.kind}
                          onChange={(event) => editor.updateNoteItem(index, { kind: event.target.value })}
                        >
                          <option value="hint">Podpowiedź</option>
                          <option value="error">Częsty błąd</option>
                          <option value="pace">Tempo</option>
                          <option value="faster">Dla szybszych</option>
                          <option value="shorter">Gdy nie zdążysz</option>
                        </select>
                        <textarea
                          aria-label={`Treść notatki ${index + 1}`}
                          value={note.text}
                          onChange={(event) => editor.updateNoteItem(index, { text: event.target.value })}
                        />
                      </div>
                      <SubItemControls
                        index={index}
                        count={editor.stepForm.notes.length}
                        label="Wskazówka"
                        labelAccusative="Wskazówkę"
                        onMove={editor.moveNoteItem}
                        onRemove={editor.removeNote}
                      />
                    </li>
                  ))}
                </ul>
              ) : null}
              <div className="note-compose-grid">
                <label>
                  Rodzaj
                  <select value={editor.stepForm.noteKind} onChange={(event) => editor.updateStep("noteKind", event.target.value)}>
                    <option value="hint">Podpowiedź</option>
                    <option value="error">Częsty błąd</option>
                    <option value="pace">Tempo</option>
                  </select>
                </label>
                <label>
                  Nowa wskazówka
                  <textarea
                    value={editor.stepForm.noteText}
                    onChange={(event) => editor.updateStep("noteText", event.target.value)}
                    placeholder="Częsty błąd, podpowiedź lub uwaga o tempie"
                  />
                </label>
              </div>
              <Button variant="secondary" onClick={editor.addNote}>
                <Plus className="button-icon" aria-hidden="true" />
                Dodaj wskazówkę
              </Button>
            </div>

            <div className="sub-editor">
              <PanelTitle icon={FileText} title={`Materiały pomocnicze (${editor.stepForm.resources.length})`} compact />
              {editor.stepForm.resources.length > 0 ? (
                <ul className="sub-item-list">
                  {editor.stepForm.resources.map((resource, index) => (
                    <li key={index} className="sub-item">
                      <div className="sub-item-fields">
                        <span className="sub-item-kind">{resourceKindLabel(resource.kind)}</span>
                        {resource.kind === "image" && resource.url ? (
                          <img className="sub-item-thumb" src={resolveAssetUrl(resource.url)} alt={resource.label ?? ""} />
                        ) : null}
                        <input
                          aria-label={`Etykieta zasobu ${index + 1}`}
                          value={resource.label ?? ""}
                          onChange={(event) => editor.updateResourceItem(index, { label: event.target.value })}
                          placeholder="Etykieta"
                        />
                        {resource.kind === "link" ? (
                          <input
                            aria-label={`Adres URL zasobu ${index + 1}`}
                            value={resource.url ?? ""}
                            onChange={(event) => editor.updateResourceItem(index, { url: event.target.value })}
                            placeholder="https://..."
                          />
                        ) : null}
                        {resource.kind === "code" ? (
                          <>
                            <input
                              aria-label={`Język zasobu ${index + 1}`}
                              value={resource.language ?? ""}
                              onChange={(event) => editor.updateResourceItem(index, { language: event.target.value })}
                              placeholder="Język"
                            />
                            <textarea
                              aria-label={`Kod zasobu ${index + 1}`}
                              value={resource.code ?? ""}
                              onChange={(event) => editor.updateResourceItem(index, { code: event.target.value })}
                            />
                          </>
                        ) : null}
                      </div>
                      <SubItemControls
                        index={index}
                        count={editor.stepForm.resources.length}
                        label="Materiał"
                        labelAccusative="Materiał"
                        onMove={editor.moveResourceItem}
                        onRemove={editor.removeResource}
                      />
                    </li>
                  ))}
                </ul>
              ) : null}
              <div className="resource-compose-grid">
                <label>
                  Typ
                  <select
                    value={editor.stepForm.resourceKind}
                    onChange={(event) => editor.updateStep("resourceKind", event.target.value)}
                  >
                    <option value="link">Link</option>
                    <option value="code">Kod</option>
                  </select>
                </label>
                <label>
                  Etykieta
                  <input
                    value={editor.stepForm.resourceLabel}
                    onChange={(event) => editor.updateStep("resourceLabel", event.target.value)}
                    placeholder="np. Karta podpowiedzi albo Scratch"
                  />
                </label>
                {editor.stepForm.resourceKind === "link" ? (
                  <label className="resource-wide-field">
                    Adres URL
                    <input
                      value={editor.stepForm.resourceUrl}
                      onChange={(event) => editor.updateStep("resourceUrl", event.target.value)}
                      placeholder="https://..."
                    />
                  </label>
                ) : (
                  <>
                    <label>
                      Język
                      <input
                        value={editor.stepForm.resourceLanguage}
                        onChange={(event) => editor.updateStep("resourceLanguage", event.target.value)}
                        placeholder="Scratch, Python, JavaScript"
                      />
                    </label>
                    <label className="resource-wide-field">
                      Kod
                      <textarea
                        value={editor.stepForm.resourceCode}
                        onChange={(event) => editor.updateStep("resourceCode", event.target.value)}
                        placeholder="Wklej kod lub pseudo-kod dla prowadzącego"
                      />
                    </label>
                  </>
                )}
              </div>
              <div className="sub-item-actions">
                <Button variant="secondary" onClick={editor.addResource}>
                  {editor.stepForm.resourceKind === "link" ? (
                    <LinkIcon className="button-icon" aria-hidden="true" />
                  ) : (
                    <FileCode2 className="button-icon" aria-hidden="true" />
                  )}
                  Dodaj materiał
                </Button>
                <label className="upload-button">
                  <UploadCloud className="button-icon" aria-hidden="true" />
                  {editor.uploading ? "Przesyłanie…" : "Dodaj obraz lub PDF"}
                  <input
                    type="file"
                    accept="image/png,image/jpeg,image/gif,image/webp,application/pdf"
                    disabled={editor.uploading}
                    onChange={(event) => {
                      const file = event.target.files?.[0];
                      if (file) {
                        void editor.uploadResourceFile(file);
                      }
                      event.target.value = "";
                    }}
                  />
                </label>
              </div>
            </div>
          </div>

          <div className="step-form-actions">
            <Button
              onClick={editor.commitStep}
              className={editor.hasUnsavedStep ? "button-attention" : ""}
            >
              <Plus className="button-icon" aria-hidden="true" />
              {editor.isEditingStep ? "Zapisz krok" : "Dodaj krok"}
            </Button>
            {editor.isEditingStep ? (
              <Button variant="secondary" onClick={editor.cancelStepEdit}>
                Anuluj
              </Button>
            ) : null}
            {editor.hasUnsavedStep ? (
              <span className="step-form-hint" role="status">
                Masz niezapisany krok — kliknij „Zapisz krok”, żeby go zachować.
              </span>
            ) : null}
          </div>
        </section>
      </div>

      <section className="editor-panel editor-steps lesson-timeline">
        <div className="lesson-timeline-head">
          <PanelTitle icon={ListChecks} title="Plan konspektu" />
          <span>
            {editor.steps.length} kroków · {editor.totalDuration} min
          </span>
        </div>
        {editor.steps.length === 0 ? (
          <div className="empty-inline lesson-empty-steps">
            <ListChecks size={28} aria-hidden="true" />
            <span>Dodaj pierwszy krok, żeby zapisać lekcję z planem prowadzenia.</span>
          </div>
        ) : (
          <div className="step-editor-list">
            {editor.steps.map((step, index) => (
              <article
                key={`${step.title}-${index}`}
                className={
                  editor.editingStepIndex === index
                    ? "step-editor-item step-editor-item-active"
                    : "step-editor-item"
                }
              >
                <div className="step-editor-main">
                  <span className="step-number">{index + 1}</span>
                  <div>
                    <span className="eyebrow">
                      {stepTypeLabel(step.type)}
                      {editor.editingStepIndex === index ? " · w edycji" : ""}
                    </span>
                    <h3>{step.title}</h3>
                    <p>
                      {step.durationMinutes} min · {step.script.length} pkt · {step.studentItems.length} wrzutek ·{" "}
                      {step.notes.length} wskazówek · {step.resources.length} materiałów
                    </p>
                  </div>
                </div>
                <div className="step-item-actions">
                  <Button
                    variant="secondary"
                    aria-label={`Przesuń krok ${index + 1} w górę`}
                    disabled={index === 0}
                    onClick={() => editor.moveStep(index, -1)}
                  >
                    <ArrowUp className="button-icon" aria-hidden="true" />
                  </Button>
                  <Button
                    variant="secondary"
                    aria-label={`Przesuń krok ${index + 1} w dół`}
                    disabled={index === editor.steps.length - 1}
                    onClick={() => editor.moveStep(index, 1)}
                  >
                    <ArrowDown className="button-icon" aria-hidden="true" />
                  </Button>
                  <Button variant="secondary" onClick={() => editor.editStep(index)}>
                    Edytuj
                  </Button>
                  <Button variant="secondary" onClick={() => editor.removeStep(index)}>
                    <Trash2 className="button-icon" aria-hidden="true" />
                    Usuń
                  </Button>
                </div>
              </article>
            ))}
          </div>
        )}
      </section>
    </section>
  );
}

function EditorStat({ icon: Icon, label, value }: { icon: LucideIcon; label: string; value: string | number }) {
  return (
    <div className="editor-stat">
      <span aria-hidden="true">
        <Icon size={18} />
      </span>
      <div>
        <strong>{value}</strong>
        <small>{label}</small>
      </div>
    </div>
  );
}

function PanelTitle({ icon: Icon, title, compact = false }: { icon: LucideIcon; title: string; compact?: boolean }) {
  return (
    <div className={compact ? "panel-title panel-title-compact" : "panel-title"}>
      <span aria-hidden="true">
        <Icon size={18} />
      </span>
      <h2>{title}</h2>
    </div>
  );
}

function ChecklistItem({ done, label }: { done: boolean; label: string }) {
  return (
    <div className={done ? "checklist-item checklist-item-done" : "checklist-item"}>
      <CheckCircle2 size={17} aria-hidden="true" />
      <span>{label}</span>
    </div>
  );
}

type ProjectFileSlotProps = {
  title: string;
  description: string;
  file?: {
    label: string;
    fileName: string;
    sizeBytes: number;
    downloadUrl?: string | null;
  } | null;
  uploading: boolean;
  onUpload: (file: File) => void;
  onLabelChange: (label: string) => void;
  onRemove: () => void;
};

function ProjectFileSlot({
  title,
  description,
  file,
  uploading,
  onUpload,
  onLabelChange,
  onRemove,
}: ProjectFileSlotProps) {
  return (
    <div className="project-file-slot">
      <div className="project-file-slot-head">
        <div>
          <strong>{title}</strong>
          <p>{description}</p>
        </div>
        {file ? (
          <Button variant="ghost" aria-label={`Usuń ${title}`} onClick={onRemove}>
            <X className="button-icon" aria-hidden="true" />
          </Button>
        ) : null}
      </div>

      {file ? (
        <div className="project-file-current">
          <label>
            Etykieta przycisku
            <input value={file.label} onChange={(event) => onLabelChange(event.target.value)} />
          </label>
          <span>
            {file.fileName} · {formatBytes(file.sizeBytes)}
          </span>
          {file.downloadUrl ? (
            <a className="material-link" href={resolveAssetUrl(file.downloadUrl)} target="_blank" rel="noreferrer">
              Otwórz link pobierania
            </a>
          ) : (
            <small>Link pobierania pojawi się po zapisaniu lekcji.</small>
          )}
        </div>
      ) : null}

      <label className="upload-button project-file-upload">
        <UploadCloud className="button-icon" aria-hidden="true" />
        {uploading ? "Przesyłanie…" : file ? "Zmień plik" : "Dodaj plik"}
        <input
          type="file"
          accept=".sb3,.sb2,.zip,.mcworld,.mctemplate"
          disabled={uploading}
          onChange={(event) => {
            const selected = event.target.files?.[0];
            if (selected) {
              onUpload(selected);
            }
            event.target.value = "";
          }}
        />
      </label>
    </div>
  );
}

function formatBytes(sizeBytes: number) {
  if (sizeBytes < 1024) {
    return `${sizeBytes} B`;
  }

  if (sizeBytes < 1024 * 1024) {
    return `${(sizeBytes / 1024).toFixed(1)} KB`;
  }

  return `${(sizeBytes / (1024 * 1024)).toFixed(1)} MB`;
}

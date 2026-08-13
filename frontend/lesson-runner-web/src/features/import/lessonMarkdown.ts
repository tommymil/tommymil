import type {
  CreateLessonRequest,
  CreateLessonStepRequest,
  LessonNote,
  LessonResource,
} from "../../types/lesson";

/**
 * `error` — fragment pliku **nie trafi** do konspektu.
 * `notice` — treść jest, ale parser musiał coś przyjąć za ciebie (typ kroku, czas, opis).
 *
 * Podział istnieje, bo wcześniej wszystko było jedną listą „ostrzeżeń”, a import był
 * aktywny niezależnie od jej długości. Zgubione dziesięć punktów scenariusza wyglądało
 * dokładnie tak samo jak informacja o domyślnym typie kroku — i tak samo łatwo było je
 * przeoczyć.
 */
export type ParseIssueSeverity = "error" | "notice";

export type ParseIssue = {
  severity: ParseIssueSeverity;
  /** Numer linii w pliku (od 1). `null` = uwaga o całym konspekcie, np. suma czasu. */
  line: number | null;
  message: string;
};

export type ParsedLesson = {
  lesson: CreateLessonRequest;
  issues: ParseIssue[];
  /** Suma czasu kroków w minutach. */
  totalMinutes: number;
  /** Deklarowana długość zajęć z metadanej `Czas:`. `null`, gdy jej nie podano.
   *
   * Świadomie nie trafia do konspektu: długość lekcji backend liczy z kroków. To jest
   * asercja autora („te zajęcia mają trwać 90 minut”), którą sprawdzamy przy imporcie. */
  plannedMinutes: number | null;
};

type Section = "script" | "notes" | "materials";

/** Sekcje `###` stojące **przed pierwszym krokiem** — dotyczą całej lekcji, nie kroku. */
type LessonSection = "criteria" | "preparation" | "homework";

/**
 * Znacznik kwestii do wypowiedzenia wprost, np. `[mów] Cześć! Gotowi programować?`.
 *
 * Zostaje w treści punktu jako prefiks, zamiast rozbijać `script` na obiekty. Kroki lekcji
 * leżą w bazie jako JSON bez wersji, więc zmiana `string[]` na listę obiektów wywróciłaby
 * odczyt wszystkich zapisanych konspektów. Prezenter rozpoznaje prefiks przy renderowaniu.
 */
export const SPEECH_PREFIX = "[mów]";

/** Domyślny czas kroku bez `(N min)`. Wartość musi być jawna, bo o niej ostrzegamy. */
const DEFAULT_STEP_MINUTES = 5;

/** Powyżej tylu minut jeden krok przestaje być krokiem — dzieci nie wytrzymują. */
const LONG_STEP_MINUTES = 20;

/**
 * Kształt zajęć: 45 minut + 5 minut przerwy + 45 minut, razem 95.
 *
 * Blok liczymy jako ciąg kroków między przerwami. Konspekt, w którym blok rośnie do
 * siedemdziesięciu minut, nie jest „ambitny” — po prostu nie zmieści się w tym oknie
 * i prowadzący będzie go skracał na żywo, przy dzieciach.
 */
const STANDARD_BLOCK_MINUTES = 45;
export const STANDARD_LESSON_MINUTES = 95;

/** O ile procent suma kroków może rozjechać się z deklarowanym czasem, zanim to zgłosimy. */
const TIME_TOLERANCE = 0.1;

const stepTypeAliases: Record<string, string[]> = {
  intro: ["intro", "wprowadzenie", "wstep", "powitanie"],
  review: ["review", "powtorka"],
  concept: ["concept", "koncept", "nowy koncept"],
  demo: ["demo", "pokaż"],
  guided: ["guided", "ćwiczenie", "ćwiczenie z prowadzeniem", "prowadzone"],
  challenge: ["challenge", "wyzwanie", "samodzielne wyzwanie"],
  summary: ["summary", "podsumowanie"],
  break: ["break", "przerwa"],
};

const noteKindAliases: Record<string, string[]> = {
  error: ["error", "błąd"],
  hint: ["hint", "podpowiedź", "wskazówka"],
  pace: ["pace", "tempo"],
  faster: ["faster", "dla szybszych", "szybsi", "rozszerzenie"],
  shorter: ["shorter", "gdy nie zdążysz", "gdy brakuje czasu", "skrót", "minimum"],
};

/** Nagłówki sekcji lekcji (przed pierwszym krokiem) rozpoznawane po słowie kluczowym. */
const lessonSectionKeywords: [LessonSection, string[]][] = [
  ["criteria", ["potrafi", "kryteri", "nauczy"]],
  ["preparation", ["przygotuj", "przed zajeciami", "sprzet"]],
  ["homework", ["domowe", "po zajeciach w domu", "dla rodzicow"]],
];

const stepTypeByAlias = buildAliasIndex(stepTypeAliases);
const noteKindByAlias = buildAliasIndex(noteKindAliases);

/** Lowercase + usuń polskie znaki diakrytyczne (do dopasowywania słów kluczowych). */
function normalize(value: string): string {
  return value
    .toLowerCase()
    .replace(/ł/g, "l")
    .normalize("NFD")
    .replace(/[̀-ͯ]/g, "")
    .trim();
}

function buildAliasIndex(aliases: Record<string, string[]>): Record<string, string> {
  const index: Record<string, string> = {};
  for (const [canonical, words] of Object.entries(aliases)) {
    for (const word of words) {
      index[normalize(word)] = canonical;
    }
  }
  return index;
}

/** Zbiera uwagi razem z numerem linii, żeby autor nie szukał ich po całym pliku. */
class IssueLog {
  readonly items: ParseIssue[] = [];

  error(line: number | null, message: string): void {
    this.items.push({ severity: "error", line, message });
  }

  notice(line: number | null, message: string): void {
    this.items.push({ severity: "notice", line, message });
  }
}

function emptyStep(): CreateLessonStepRequest {
  return {
    type: "concept",
    title: "Krok",
    durationMinutes: DEFAULT_STEP_MINUTES,
    script: [],
    studentItems: [],
    resources: [],
    notes: [],
  };
}

function parseStepHeading(text: string, line: number, issues: IssueLog): CreateLessonStepRequest {
  const step = emptyStep();
  let rest = text;

  const typeMatch = rest.match(/^\[([^\]]+)\]\s*/);
  if (typeMatch) {
    const canonical = stepTypeByAlias[normalize(typeMatch[1])];
    if (canonical) {
      step.type = canonical;
    } else {
      issues.notice(line, `Nieznany typ kroku „${typeMatch[1]}” — przyjęto „Nowy koncept”.`);
    }
    rest = rest.slice(typeMatch[0].length);
  }

  const durationMatch = rest.match(/\((\d+)\s*[^)]*\)\s*$/);
  if (durationMatch && durationMatch.index !== undefined) {
    step.durationMinutes = Math.max(1, parseInt(durationMatch[1], 10));
    rest = rest.slice(0, durationMatch.index);
  }

  step.title = rest.trim() || "Krok";

  if (!durationMatch) {
    issues.notice(line, `Krok „${step.title}” nie ma czasu — przyjęto ${DEFAULT_STEP_MINUTES} min.`);
  } else if (step.durationMinutes > LONG_STEP_MINUTES) {
    issues.notice(
      line,
      `Krok „${step.title}” trwa ${step.durationMinutes} min. Powyżej ${LONG_STEP_MINUTES} min warto go podzielić.`,
    );
  }

  return step;
}

function sectionFor(headingText: string): Section | null {
  const text = normalize(headingText);

  if (text.includes("robic") || text.includes("instrukcja") || text.includes("skrypt") || text.includes("czynnosc")) {
    return "script";
  }

  if (text.includes("wskazow") || text.includes("notat")) {
    return "notes";
  }

  if (text.includes("materia") || text.includes("zasob") || text.includes("ekran")) {
    return "materials";
  }

  return null;
}

function lessonSectionFor(headingText: string): LessonSection | null {
  const text = normalize(headingText);
  return lessonSectionKeywords.find(([, words]) => words.some((word) => text.includes(word)))?.[0] ?? null;
}

/** `Czas: 90 min`, `Czas: 90`, `Czas: 2 h` → minuty. `null`, gdy nic sensownego. */
function parseMinutes(value: string): number | null {
  const hours = value.match(/(\d+)\s*(h\b|godz)/i);
  if (hours) {
    return parseInt(hours[1], 10) * 60;
  }

  const minutes = value.match(/(\d+)/);
  return minutes ? parseInt(minutes[1], 10) : null;
}

type MetaResult = "applied" | "unknown";

function applyMeta(
  lesson: CreateLessonRequest,
  key: string,
  value: string,
  state: { plannedMinutes: number | null; description: string[] },
): MetaResult {
  const normalizedKey = normalize(key);

  if (normalizedKey === "title" || normalizedKey === "tytul") {
    if (!lesson.title) {
      lesson.title = value;
    }
    return "applied";
  }

  if (normalizedKey === "subject" || normalizedKey === "przedmiot") {
    lesson.subject = value;
    return "applied";
  }

  if (normalizedKey === "level" || normalizedKey === "poziom") {
    lesson.level = value;
    return "applied";
  }

  if (normalizedKey === "description" || normalizedKey === "opis") {
    state.description.push(value);
    return "applied";
  }

  if (normalizedKey === "tags" || normalizedKey === "tagi") {
    lesson.tags = value.split(",").map((tag) => tag.trim()).filter(Boolean);
    return "applied";
  }

  if (normalizedKey === "time" || normalizedKey === "czas" || normalizedKey === "dlugosc") {
    state.plannedMinutes = parseMinutes(value);
    return "applied";
  }

  if (normalizedKey === "objective" || normalizedKey === "cel") {
    lesson.objective = value;
    return "applied";
  }

  // Jednolinijkowe odpowiedniki sekcji `###` - przy jednym punkcie osobny nagłówek
  // to więcej pisania niż treści.
  if (normalizedKey === "homework" || normalizedKey === "zadanie domowe") {
    lesson.homework = [...(lesson.homework ?? []), value];
    return "applied";
  }

  if (normalizedKey === "preparation" || normalizedKey === "przygotuj") {
    lesson.preparation = [...(lesson.preparation ?? []), value];
    return "applied";
  }

  return "unknown";
}

function parseNote(content: string, line: number, issues: IssueLog): LessonNote {
  const tagMatch = content.match(/^\[([^\]]+)\]\s*(.*)$/);

  if (tagMatch) {
    const canonical = noteKindByAlias[normalize(tagMatch[1])];
    if (!canonical) {
      issues.notice(line, `Nieznany rodzaj wskazówki „${tagMatch[1]}” — przyjęto „Podpowiedź”.`);
    }
    return { kind: canonical ?? "hint", text: tagMatch[2].trim() };
  }

  return { kind: "hint", text: content.trim() };
}

const speechAliases = ["mów", "mow", "powiedz", "say"];

/**
 * Kwestia do wypowiedzenia wprost dostaje jednolity prefiks `[mów]`.
 *
 * Autorzy i tak to robili, tylko pogrubieniem (`**Powitanie:** przywitaj się...`), bo format
 * nie miał na to miejsca — a skoro konwencja była tylko wizualna, kokpit nie mógł jej użyć.
 */
function normalizeScriptLine(content: string): string {
  const tagMatch = content.match(/^\[([^\]]+)\]\s*(.*)$/);

  if (tagMatch && speechAliases.includes(normalize(tagMatch[1]))) {
    return `${SPEECH_PREFIX} ${tagMatch[2].trim()}`;
  }

  return content;
}

/** Usuwa wspólne wcięcie z linii bloku kodu, zachowując relatywne wcięcia pseudo-kodu. */
function dedent(lines: string[]): string {
  const indents = lines
    .filter((line) => line.trim().length > 0)
    .map((line) => line.match(/^\s*/)?.[0].length ?? 0);
  const common = indents.length > 0 ? Math.min(...indents) : 0;
  return lines.map((line) => line.slice(common)).join("\n").replace(/\s+$/, "");
}

export function parseLessonMarkdown(markdown: string): ParsedLesson {
  const lines = markdown.replace(/\r\n/g, "\n").split("\n");
  const issues = new IssueLog();
  const lesson: CreateLessonRequest = {
    title: "",
    subject: "",
    level: "",
    description: "",
    tags: [],
    steps: [],
    objective: null,
    successCriteria: [],
    preparation: [],
    homework: [],
  };
  const state = { plannedMinutes: null as number | null, description: [] as string[] };

  let step: CreateLessonStepRequest | null = null;
  let section: Section | null = null;
  let lessonSection: LessonSection | null = null;
  let pendingCode: LessonResource | null = null;

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i].trim();
    const lineNumber = i + 1;

    if (line === "") {
      continue;
    }

    // Blok kodu - wypełnia ostatni element [kod].
    if (line.startsWith("```")) {
      const codeLines: string[] = [];
      i++;
      while (i < lines.length && !lines[i].trim().startsWith("```")) {
        codeLines.push(lines[i]);
        i++;
      }

      if (pendingCode) {
        pendingCode.code = dedent(codeLines);
        pendingCode = null;
      } else {
        issues.error(lineNumber, "Blok kodu bez poprzedzającego punktu [kod] — pominięto.");
      }
      continue;
    }

    if (line.startsWith("# ") && !line.startsWith("## ")) {
      if (!lesson.title) {
        lesson.title = line.slice(2).trim();
      }
      continue;
    }

    if (line.startsWith("## ")) {
      if (step) {
        lesson.steps.push(step);
      }
      step = parseStepHeading(line.slice(3).trim(), lineNumber, issues);
      section = null;
      lessonSection = null;
      pendingCode = null;
      continue;
    }

    if (line.startsWith("### ")) {
      const heading = line.slice(4).trim();

      // Przed pierwszym krokiem `###` opisuje całą lekcję (kryteria, przygotowanie,
      // zadanie domowe), a nie krok - kroku jeszcze nie ma.
      if (!step) {
        lessonSection = lessonSectionFor(heading);
        if (lessonSection === null) {
          issues.error(
            lineNumber,
            `Nieznana sekcja lekcji „${heading}” — jej treść nie trafi do konspektu. Znane: „Po zajęciach dziecko potrafi”, „Przygotuj przed zajęciami”, „Zadanie domowe”.`,
          );
        }
        continue;
      }

      section = sectionFor(heading);
      if (section === null) {
        issues.error(
          lineNumber,
          `Nieznana sekcja „${heading}” — jej treść nie trafi do konspektu. Znane: „Co robić teraz”, „Wskazówki”, „Materiały”.`,
        );
      }
      continue;
    }

    // Punkt listy pod sekcją lekcji - kryteria, przygotowanie, zadanie domowe.
    if (!step && lessonSection !== null) {
      const bullet = line.match(/^(?:[-*]|\d+\.)\s+(.*)$/);
      const text = (bullet ? bullet[1] : line).trim();

      if (lessonSection === "criteria") {
        lesson.successCriteria = [...(lesson.successCriteria ?? []), text];
      } else if (lessonSection === "preparation") {
        lesson.preparation = [...(lesson.preparation ?? []), text];
      } else {
        lesson.homework = [...(lesson.homework ?? []), text];
      }
      continue;
    }

    // Metadane i opis (tylko przed pierwszym krokiem).
    if (!step) {
      // Klucz do dwóch słów: „Zadanie domowe:” to metadana, ale „Dzieci uczą się:” to
      // już zdanie opisu i nie ma po co go zgłaszać.
      const meta = line.match(/^([^\s:][^:]{0,30}):\s+(.+)$/);
      if (meta && meta[1].trim().split(/\s+/).length <= 2) {
        const result = applyMeta(lesson, meta[1].trim(), meta[2].trim(), state);
        if (result === "applied") {
          continue;
        }

        // Nic nie ginie: nierozpoznana metadana ląduje w opisie, ale autor się o tym dowiaduje.
        issues.notice(lineNumber, `Nierozpoznana metadana „${meta[1].trim()}” — cała linia trafiła do opisu.`);
      }

      state.description.push(line);
      continue;
    }

    const item = line.match(/^(?:[-*]|\d+\.)\s+(.*)$/);
    const content = item ? item[1].trim() : line;

    if (section === "script") {
      step.script.push(normalizeScriptLine(content));
    } else if (section === "notes") {
      step.notes.push(parseNote(content, lineNumber, issues));
    } else if (section === "materials") {
      pendingCode = appendMaterial(step, content, lineNumber, issues);
    } else {
      issues.error(
        lineNumber,
        `Treść poza sekcją (krok „${step.title}”) — pominięto: „${content}”.`,
      );
    }
  }

  if (step) {
    lesson.steps.push(step);
  }

  lesson.description = state.description.join("\n");

  if (!lesson.title) {
    issues.error(null, "Brak tytułu lekcji (nagłówek „# ...”).");
  }
  if (lesson.steps.length === 0) {
    issues.error(null, "Brak kroków (nagłówki „## ...”).");
  }

  const totalMinutes = lesson.steps.reduce((sum, item) => sum + item.durationMinutes, 0);
  appendPlanIssues(lesson, totalMinutes, state.plannedMinutes, issues);

  return { lesson, issues: issues.items, totalMinutes, plannedMinutes: state.plannedMinutes };
}

/**
 * Uwagi o samym planie zajęć, nie o składni pliku.
 *
 * Konspekt może być bez zarzutu formalnie i nie do poprowadzenia w praktyce: kroki sumujące
 * się do 25 minut przy zajęciach 90-minutowych, półtorej godziny bez przerwy albo lekcja,
 * która zaczyna się od razu od kodowania. Te trzy rzeczy widać z samego pliku, więc szkoda
 * czekać z nimi do pierwszych zajęć.
 */
function appendPlanIssues(
  lesson: CreateLessonRequest,
  totalMinutes: number,
  plannedMinutes: number | null,
  issues: IssueLog,
): void {
  if (lesson.steps.length === 0) {
    return;
  }

  if (plannedMinutes !== null && plannedMinutes > 0) {
    const drift = Math.abs(totalMinutes - plannedMinutes) / plannedMinutes;
    if (drift > TIME_TOLERANCE) {
      const direction = totalMinutes < plannedMinutes ? "mniej" : "więcej";
      issues.notice(
        null,
        `Kroki sumują się do ${totalMinutes} min, a zajęcia mają trwać ${plannedMinutes} min — to o ${Math.abs(totalMinutes - plannedMinutes)} min ${direction}.`,
      );
    }
  }

  blockMinutes(lesson).forEach((minutes, index, blocks) => {
    if (minutes <= STANDARD_BLOCK_MINUTES) {
      return;
    }

    const which = blocks.length === 1 ? "Zajęcia trwają" : `Blok ${index + 1} trwa`;
    issues.notice(
      null,
      `${which} ${minutes} min bez przerwy — zajęcia mają kształt ${STANDARD_BLOCK_MINUTES} min + 5 min przerwy + ${STANDARD_BLOCK_MINUTES} min. Dodaj krok „## [przerwa] ... (5 min)”.`,
    );
  });

  if (!lesson.steps.some((step) => step.type === "intro")) {
    issues.notice(null, "Brak kroku wprowadzającego — zajęcia zaczynają się od materiału.");
  }

  if (!lesson.steps.some((step) => step.type === "summary")) {
    issues.notice(null, "Brak podsumowania — nie ma kroku, w którym domykasz lekcję.");
  }

  if (!lesson.objective) {
    issues.notice(null, "Brak celu lekcji — dopisz „Cel: …”, żeby prowadzący wiedział, co jest w niej najważniejsze.");
  }
}

/**
 * Długości bloków pracy, czyli ciągów kroków między przerwami.
 *
 * Sam krok `[przerwa]` do bloku nie wchodzi. Pytanie brzmi: ile dzieci pracuje bez
 * oderwania się od ekranu — a nie ile trwa cała lekcja.
 */
export function blockMinutes(lesson: CreateLessonRequest): number[] {
  const blocks: number[] = [];
  let current = 0;

  for (const step of lesson.steps) {
    if (step.type === "break") {
      blocks.push(current);
      current = 0;
      continue;
    }

    current += step.durationMinutes;
  }

  blocks.push(current);
  return blocks.filter((minutes) => minutes > 0);
}

/** Dodaje materiał do kroku; zwraca zasób kodu oczekujący na blok ``` (lub null). */
function appendMaterial(
  step: CreateLessonStepRequest,
  content: string,
  line: number,
  issues: IssueLog,
): LessonResource | null {
  const linkMatch = content.match(/^\[link\]\s*(.*)$/i);
  if (linkMatch) {
    const [labelPart, urlPart] = linkMatch[1].split("|").map((part) => part.trim());
    const url = urlPart ?? labelPart;
    if (!url) {
      issues.error(line, "Materiał [link] bez adresu — pominięto.");
      return null;
    }
    step.resources.push({ kind: "link", label: urlPart ? labelPart : url, url });
    return null;
  }

  const codeMatch = content.match(/^\[kod\]\s*(.*)$/i);
  if (codeMatch) {
    // `[kod] Powitanie (Wygląd) | scratch:` — etykieta i język osobno. Bez kreski cały
    // tekst jest etykietą: autorzy pisali tam „Powitanie (Wygląd)”, co lądowało w polu
    // `language` i znaczyło co innego, niż zawierało.
    const [labelPart, languagePart] = codeMatch[1]
      .replace(/:\s*$/, "")
      .split("|")
      .map((part) => part.trim());

    const resource: LessonResource = {
      kind: "code",
      label: labelPart || null,
      code: "",
      language: languagePart || null,
    };
    step.resources.push(resource);
    return resource;
  }

  const imageMatch = content.match(/^!\[([^\]]*)\]\(([^)]+)\)$/);
  if (imageMatch) {
    const [, caption, url] = imageMatch;

    // Import nie wysyła plików - potrafi tylko wskazać adres. Ścieżka z dysku autora
    // dałaby dziecku na ekranie puste miejsce, więc lepiej powiedzieć to wprost.
    if (!/^(https?:\/\/|\/)/i.test(url.trim())) {
      issues.error(
        line,
        `Obraz „${url.trim()}” to plik lokalny — import nie wysyła plików. Wgraj go w edytorze po imporcie albo podaj pełny adres.`,
      );
      return null;
    }

    step.studentItems.push({ kind: "image", url: url.trim(), caption: caption.trim() || null });
    return null;
  }

  // Punkt bez tagu = tekstowa wrzutka na ekran.
  step.studentItems.push({ kind: "text", text: content });
  return null;
}

import { describe, expect, it } from "vitest";
import { parseLessonMarkdown } from "./lessonMarkdown";

const sample = `# Animacja i sterowanie
Subject: Scratch
Level: Poziom 1 - 8-10 lat
Tags: Kostiumy, Sterowanie
Opis: Dzieci animują postać i sterują nią strzałkami.

## [concept] Sterowanie strzałkami (8 min)

### Co robić teraz
- Ustaw styl obrotu na lewo-prawo
- Dodaj bloki ruchu dla strzałek

### Wskazówki
- [błąd] Postać do góry nogami -> styl lewo-prawo
- [tempo] Szybsza grupa: wersja B
- Zwykła podpowiedź bez tagu

### Materiały
- [link] Scratch | https://scratch.mit.edu
- [kod] Scratch:
  \`\`\`
  kiedy klawisz [prawo] naciśnięty
    przesuń o (10) kroków
    następny kostium
  \`\`\`
- Pokaż dzieciom gotowy efekt

## Podsumowanie

### Co robić teraz
- Zapytaj co dziś zbudowaliśmy
`;

describe("parseLessonMarkdown", () => {
  it("parses lesson metadata", () => {
    const { lesson } = parseLessonMarkdown(sample);
    expect(lesson.title).toBe("Animacja i sterowanie");
    expect(lesson.subject).toBe("Scratch");
    expect(lesson.level).toBe("Poziom 1 - 8-10 lat");
    expect(lesson.tags).toEqual(["Kostiumy", "Sterowanie"]);
    expect(lesson.description).toBe("Dzieci animują postać i sterują nią strzałkami.");
  });

  it("parses a typed step with duration and the 'what to do now' list", () => {
    const { lesson } = parseLessonMarkdown(sample);
    const step = lesson.steps[0];
    expect(step.type).toBe("concept");
    expect(step.durationMinutes).toBe(8);
    expect(step.title).toBe("Sterowanie strzałkami");
    expect(step.script).toEqual([
      "Ustaw styl obrotu na lewo-prawo",
      "Dodaj bloki ruchu dla strzałek",
    ]);
  });

  it("maps Polish note tags to kinds and defaults untagged notes to hint", () => {
    const step = parseLessonMarkdown(sample).lesson.steps[0];
    expect(step.notes).toEqual([
      { kind: "error", text: "Postać do góry nogami -> styl lewo-prawo" },
      { kind: "pace", text: "Szybsza grupa: wersja B" },
      { kind: "hint", text: "Zwykła podpowiedź bez tagu" },
    ]);
  });

  it("parses link and fenced code materials, and plain text inserts", () => {
    const step = parseLessonMarkdown(sample).lesson.steps[0];
    expect(step.resources[0]).toEqual({ kind: "link", label: "Scratch", url: "https://scratch.mit.edu" });
    expect(step.resources[1].kind).toBe("code");
    // Bez kreski cały tekst jest etykietą - „Powitanie (Wygląd)” nigdy nie było językiem.
    expect(step.resources[1].label).toBe("Scratch");
    expect(step.resources[1].language).toBeNull();
    expect(step.resources[1].code).toBe(
      "kiedy klawisz [prawo] naciśnięty\n  przesuń o (10) kroków\n  następny kostium",
    );
    expect(step.studentItems).toEqual([{ kind: "text", text: "Pokaż dzieciom gotowy efekt" }]);
  });

  it("creates a second step and defaults its type to concept", () => {
    const { lesson } = parseLessonMarkdown(sample);
    expect(lesson.steps).toHaveLength(2);
    expect(lesson.steps[1].title).toBe("Podsumowanie");
    expect(lesson.steps[1].type).toBe("concept");
    expect(lesson.steps[1].durationMinutes).toBe(5);
  });

  it("warns when the title or steps are missing", () => {
    const { issues } = parseLessonMarkdown("Subject: Scratch\n");
    expect(issues.some((issue) => issue.severity === "error" && issue.message.includes("tytułu"))).toBe(true);
    expect(issues.some((issue) => issue.severity === "error" && issue.message.includes("kroków"))).toBe(true);
  });

  /**
   * Cichy fallback był najgroźniejszym zachowaniem parsera: konspekt importował się bez
   * słowa, a dopiero na zajęciach okazywało się, że krok ma 5 minut zamiast 20 i typ
   * „Nowy koncept” zamiast wyzwania.
   */
  describe("przyjęte założenia", () => {
    const withDefaults = `# Lekcja
## [zadanie] Rozgrzewka
### Co robić teraz
- Przywitaj grupę

## Kodowanie (12 min)
### Wskazówki
- [uwaga] Coś ważnego
`;

    it("reports an unknown step type instead of silently using concept", () => {
      const { lesson, issues } = parseLessonMarkdown(withDefaults);
      expect(lesson.steps[0].type).toBe("concept");
      expect(issues).toContainEqual({
        severity: "notice",
        line: 2,
        message: 'Nieznany typ kroku „zadanie” — przyjęto „Nowy koncept”.',
      });
    });

    it("reports a missing step duration", () => {
      const { issues } = parseLessonMarkdown(withDefaults);
      expect(issues).toContainEqual({
        severity: "notice",
        line: 2,
        message: 'Krok „Rozgrzewka” nie ma czasu — przyjęto 5 min.',
      });
    });

    it("reports an unknown note tag", () => {
      const { lesson, issues } = parseLessonMarkdown(withDefaults);
      expect(lesson.steps[1].notes[0].kind).toBe("hint");
      expect(issues.some((issue) => issue.line === 8 && issue.message.includes("Nieznany rodzaj wskazówki"))).toBe(true);
    });
  });

  it("points to the line where content is dropped", () => {
    const { issues } = parseLessonMarkdown("# Lekcja\n## Krok (5 min)\n### Cel kroku\n- Coś ważnego\n");
    const errors = issues.filter((issue) => issue.severity === "error");

    expect(errors[0]).toMatchObject({ line: 3, severity: "error" });
    expect(errors[0].message).toContain("Nieznana sekcja");
    expect(errors[1]).toMatchObject({ line: 4, severity: "error" });
  });

  /** Wcześniej opis brał wyłącznie pierwszą linię, a reszta znikała bez śladu. */
  it("keeps every line before the first step, and flags unknown metadata", () => {
    const { lesson, issues } = parseLessonMarkdown(
      "# Lekcja\nAutor: Ktoś\nPierwsza linia opisu.\nDruga linia opisu.\n\n## Krok (5 min)\n",
    );

    expect(lesson.description).toBe("Autor: Ktoś\nPierwsza linia opisu.\nDruga linia opisu.");
    expect(issues.some((issue) => issue.message.includes('Nierozpoznana metadana „Autor”'))).toBe(true);
  });

  /**
   * Rzeczy, których do tej pory nie było gdzie zapisać, więc albo ginęły w scenariuszu,
   * albo w ogóle nie trafiały do konspektu.
   */
  describe("cel, kryteria i przygotowanie", () => {
    const md = `# Pętle
Cel: Dziecko rozumie powtarzanie.

### Po zajęciach dziecko potrafi
- użyć bloku „powtórz”
- policzyć, ile razy coś się wykona

### Przygotuj przed zajęciami
- otwarty projekt startowy

### Zadanie domowe
- pokaż rodzicom taniec duszka

## [intro] Start (5 min)
### Co robić teraz
- [mów] Cześć! Gotowi programować?
- Sprawdź obecność

## [summary] Koniec (5 min)
### Co robić teraz
- Zbierz grupę
`;

    it("reads the objective and the lesson-level lists", () => {
      const { lesson } = parseLessonMarkdown(md);

      expect(lesson.objective).toBe("Dziecko rozumie powtarzanie.");
      expect(lesson.successCriteria).toEqual(["użyć bloku „powtórz”", "policzyć, ile razy coś się wykona"]);
      expect(lesson.preparation).toEqual(["otwarty projekt startowy"]);
      expect(lesson.homework).toEqual(["pokaż rodzicom taniec duszka"]);
    });

    it("marks a line meant to be said out loud", () => {
      const { lesson } = parseLessonMarkdown(md);
      expect(lesson.steps[0].script).toEqual(["[mów] Cześć! Gotowi programować?", "Sprawdź obecność"]);
    });

    it("asks for an objective when it is missing", () => {
      const { issues } = parseLessonMarkdown("# Lekcja\n## [intro] Start (5 min)\n");
      expect(issues.some((issue) => issue.message.includes("Brak celu lekcji"))).toBe(true);
    });

    it("does not repeat that request once the objective is there", () => {
      const { issues } = parseLessonMarkdown(md);
      expect(issues.some((issue) => issue.message.includes("Brak celu lekcji"))).toBe(false);
    });
  });

  describe("materiały", () => {
    it("splits a code label from its language", () => {
      const { lesson } = parseLessonMarkdown(
        "# L\n## Krok (5 min)\n### Materiały\n- [kod] Powitanie (Wygląd) | scratch:\n```\npowiedz [Cześć]\n```\n",
      );
      const resource = lesson.steps[0].resources[0];

      expect(resource.label).toBe("Powitanie (Wygląd)");
      expect(resource.language).toBe("scratch");
      expect(resource.code).toBe("powiedz [Cześć]");
    });

    it("takes an image with an absolute address as a screen insert", () => {
      const { lesson } = parseLessonMarkdown(
        "# L\n## Krok (5 min)\n### Materiały\n- ![Bloczki ruchu](/uploads/bloczki.png)\n",
      );

      expect(lesson.steps[0].studentItems).toEqual([
        { kind: "image", url: "/uploads/bloczki.png", caption: "Bloczki ruchu" },
      ]);
    });

    /** Import nie wysyła plików, więc ścieżka z dysku autora dałaby puste miejsce na ekranie. */
    it("refuses a local file path instead of showing a broken image", () => {
      const { lesson, issues } = parseLessonMarkdown(
        "# L\n## Krok (5 min)\n### Materiały\n- ![Screen](C:/zdjecia/screen.png)\n",
      );

      expect(lesson.steps[0].studentItems).toEqual([]);
      expect(issues.some((issue) => issue.severity === "error" && issue.message.includes("plik lokalny"))).toBe(true);
    });
  });

  describe("budżet czasu i plan zajęć", () => {
    it("rozróżnia godzinną lekcję pokazową od standardowej", () => {
      const { lesson, issues, totalMinutes, plannedMinutes } = parseLessonMarkdown(
        `# Pokaz Scratcha
Rodzaj: pokazowa
Czas: 60 min
Cel: Sprawdzić, czy dziecko dobrze czuje się przy komputerze.

## [intro] Powitanie (10 min)
## [demo] Pokaz (15 min)
## [guided] Wspólne zadanie (20 min)
## [summary] Domknięcie i pytania (15 min)
`,
      );

      expect(lesson.kind).toBe("showcase");
      expect(plannedMinutes).toBe(60);
      expect(totalMinutes).toBe(60);
      expect(issues).toEqual([]);
    });

    it("nie wymaga przerwy w lekcji pokazowej — to jeden ciąg 60 minut", () => {
      const { issues } = parseLessonMarkdown(
        `# Pokaz
Rodzaj: pokazowa
Czas: 60 min
Cel: Poznać dziecko.
## [intro] Start (10 min)
## [guided] Zadanie (35 min)
## [summary] Domknięcie (15 min)
`,
      );

      expect(issues.some((issue) => issue.message.includes("bez przerwy"))).toBe(false);
    });

    /**
     * 55. minuta to granica wyjścia uczestnika, a nie dopuszczalna długość planu. Plan na
     * 55 minut zostawiałby pięć minut zarezerwowanego okna pustych.
     */
    it("zgłasza lekcję pokazową krótszą niż 60 minut, mimo że mieści się po 55. minucie", () => {
      const { issues } = parseLessonMarkdown(
        `# Pokaz
Rodzaj: pokazowa
Czas: 60 min
Cel: Poznać dziecko.
## [intro] Start (10 min)
## [guided] Zadanie (35 min)
## [summary] Domknięcie (10 min)
`,
      );

      expect(issues.some((issue) => issue.message.includes("dokładnie 60 min — brakuje 5 min"))).toBe(true);
    });

    it("ostrzega, gdy lekcja pokazowa ma długość standardowych zajęć", () => {
      const { issues } = parseLessonMarkdown(
        "# Pokaz\nRodzaj: pokazowa\nCzas: 95 min\nCel: X\n## [intro] Start (20 min)\n## [summary] Koniec (40 min)\n",
      );

      expect(issues.some((issue) => issue.message.includes("z 95 na 60 min"))).toBe(true);
    });

    it("reads the declared lesson length and sums the steps", () => {
      const { totalMinutes, plannedMinutes } = parseLessonMarkdown(
        "# Lekcja\nCzas: 90 min\n\n## [intro] Start (10 min)\n## [summary] Koniec (15 min)\n",
      );

      expect(plannedMinutes).toBe(90);
      expect(totalMinutes).toBe(25);
    });

    it("flags a plan that does not fill the declared time", () => {
      const { issues } = parseLessonMarkdown("# Lekcja\nCzas: 90 min\n\n## [intro] Start (10 min)\n");
      expect(issues.some((issue) => issue.message.includes("25 min") || issue.message.includes("10 min"))).toBe(true);
    });

    /**
     * Sumę porównujemy z długością wynikającą z rodzaju lekcji, a nie z metadaną „Czas”.
     * Wcześniej szło to przez tolerancję 10% od deklaracji autora i dla pokazówki przechodziło
     * wszystko od 54 do 66 minut.
     */
    it("zgłasza sumę kroków inną niż długość wynikająca z rodzaju lekcji", () => {
      const { issues } = parseLessonMarkdown(
        "# Lekcja\nCzas: 95 min\nCel: X\n\n## [intro] Start (10 min)\n## [concept] Praca (20 min)\n## [break] Przerwa (5 min)\n## [summary] Koniec (20 min)\n",
      );

      expect(issues.some((issue) => issue.message.includes("dokładnie 95 min — brakuje 40 min"))).toBe(true);
    });

    /** Zajęcia mają kształt 45 min + 5 min przerwy + 45 min, razem 95. */
    it("asks for a break once a block of work passes 45 minutes", () => {
      const { issues } = parseLessonMarkdown(
        "# Lekcja\n## [intro] Start (20 min)\n## [concept] Praca (20 min)\n## [challenge] Zadanie (20 min)\n## [summary] Koniec (10 min)\n",
      );

      expect(issues.some((issue) => issue.message.includes("70 min bez przerwy"))).toBe(true);
    });

    it("accepts the standard 45 + break + 45 shape", () => {
      const { issues, totalMinutes } = parseLessonMarkdown(
        `# Lekcja
Czas: 95 min
Cel: Cokolwiek.

## [intro] Start (15 min)
## [concept] Praca (15 min)
## [guided] Ćwiczenie (15 min)
## [przerwa] Przerwa (5 min)
## [concept] Dalej (20 min)
## [challenge] Zadanie (20 min)
## [summary] Koniec (5 min)
`,
      );

      expect(totalMinutes).toBe(95);
      expect(issues).toEqual([]);
    });

    it("counts blocks between breaks, not the whole lesson", () => {
      const { issues } = parseLessonMarkdown(
        "# Lekcja\nCel: X\n## [intro] Blok pierwszy (50 min)\n## [przerwa] Przerwa (5 min)\n## [summary] Blok drugi (40 min)\n",
      );

      expect(issues.some((issue) => issue.message.includes("Blok 1 trwa 50 min"))).toBe(true);
      expect(issues.some((issue) => issue.message.includes("Blok 2"))).toBe(false);
    });

    it("notices a missing intro or summary", () => {
      const { issues } = parseLessonMarkdown("# Lekcja\n## [concept] Praca (20 min)\n");

      expect(issues.some((issue) => issue.message.includes("Brak kroku wprowadzającego"))).toBe(true);
      expect(issues.some((issue) => issue.message.includes("Brak podsumowania"))).toBe(true);
    });

    it("flags a step that is too long to hold attention", () => {
      const { issues } = parseLessonMarkdown("# Lekcja\n## [concept] Bardzo długi krok (35 min)\n");
      expect(issues.some((issue) => issue.message.includes("warto go podzielić"))).toBe(true);
    });
  });
});

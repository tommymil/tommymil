import { existsSync, readFileSync, readdirSync } from "node:fs";
import { join, relative } from "node:path";
import { describe, expect, it } from "vitest";

import { STANDARD_LESSON_MINUTES, blockMinutes, parseLessonMarkdown } from "./lessonMarkdown";

/**
 * Strażnik gotowych konspektów z `docs/program`.
 *
 * Te pliki nie są przykładami — mają trafić do systemu importem i być prowadzone przy
 * dzieciach. Błąd parsera oznacza tu, że fragment scenariusza po cichu zniknie: nie na
 * ekranie autora, tylko na zajęciach, w kroku, którego prowadzący nie zobaczy.
 *
 * Sprawdzamy trzy rzeczy naraz, bo każda z nich już raz zdarzyła się w `docs`:
 * literówkę w nagłówku sekcji (treść wypada), sumę kroków rozjeżdżającą się z metadaną
 * `Czas:` oraz blok pracy dłuższy niż 45 minut bez przerwy.
 *
 * Pliki czytamy przez API Node'a, a nie `import.meta.glob` jak w `polishDiacritics.test.ts`:
 * katalog `docs` leży poza katalogiem aplikacji, więc Vite odmawia jego wczytania.
 */

const KATALOG_PROGRAMU = join(process.cwd(), "..", "..", "docs", "program");
const MAKSYMALNY_BLOK_MINUT = 45;

function znajdzKonspekty(katalog: string): string[] {
  if (!existsSync(katalog)) {
    return [];
  }

  return readdirSync(katalog, { withFileTypes: true }).flatMap((wpis) => {
    const sciezka = join(katalog, wpis.name);

    if (wpis.isDirectory()) {
      return znajdzKonspekty(sciezka);
    }

    // PROGRAM.md to mapa treści całego kursu, nie konspekt — kroków nie ma i mieć nie musi.
    return wpis.name.endsWith(".md") && wpis.name !== "PROGRAM.md" ? [sciezka] : [];
  });
}

const konspekty = znajdzKonspekty(KATALOG_PROGRAMU).sort();

describe("konspekty w docs/program", () => {
  it("katalog programu zawiera konspekty", () => {
    expect(konspekty.length).toBeGreaterThan(0);
  });

  it.each(konspekty.map((sciezka) => [relative(KATALOG_PROGRAMU, sciezka), sciezka]))(
    "%s importuje się bez błędów",
    (_nazwa, sciezka) => {
      const { lesson, issues, totalMinutes, plannedMinutes } = parseLessonMarkdown(
        readFileSync(sciezka, "utf8"),
      );

      const problemy = issues.filter((issue) => issue.severity === "error");
      expect(problemy.map((issue) => `linia ${issue.line}: ${issue.message}`)).toEqual([]);

      expect(lesson.title).not.toBe("");
      expect(lesson.kind).toBe("standard");
      expect(lesson.objective).toBeTruthy();
      expect(lesson.steps.length).toBeGreaterThan(0);

      // Prowadzący musi wiedzieć, czym lekcję otworzyć i czym ją domknąć.
      expect(lesson.steps.some((step) => step.type === "intro")).toBe(true);
      expect(lesson.steps.some((step) => step.type === "summary")).toBe(true);
      expect(lesson.steps.some((step) => step.type === "break")).toBe(true);

      // Termin w systemie ma stałą długość — konspekt nie może jej negocjować.
      expect(plannedMinutes).toBe(STANDARD_LESSON_MINUTES);
      expect(totalMinutes).toBe(STANDARD_LESSON_MINUTES);

      for (const minuty of blockMinutes(lesson)) {
        expect(minuty).toBeLessThanOrEqual(MAKSYMALNY_BLOK_MINUT);
      }

      // Zajęcia zdalne bez zapowiedzianych trudności prowadzi się na wyczucie —
      // każdy konspekt ma nieść wiedzę o tym, co pójdzie źle.
      expect(lesson.steps.flatMap((step) => step.notes).some((note) => note.kind === "error")).toBe(
        true,
      );
    },
  );
});

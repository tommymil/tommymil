import { existsSync, readFileSync, readdirSync } from "node:fs";
import { basename, join, relative } from "node:path";
import { describe, expect, it } from "vitest";

import { SHOWCASE_LESSON_MINUTES, parseLessonMarkdown } from "./lessonMarkdown";

const KATALOG_POKAZOWEK = join(process.cwd(), "..", "..", "docs", "showcase");

const MINECRAFT_BLOCKS = new Set([
  "l04-minecraft-magiczna-forteca.przygotowanie.md",
  "l05-minecraft-arena-lowcow-potworow.przygotowanie.md",
  "l06-minecraft-agent-gornik.przygotowanie.md",
]);

const MINECRAFT_PYTHON = new Set([
  "l07-minecraft-python-zlota-piramida.przygotowanie.md",
  "l08-minecraft-supermoce-w-pythonie.przygotowanie.md",
  "l09-minecraft-python-teleporter-do-podniebnej-bazy.przygotowanie.md",
]);

function znajdzKonspekty(): string[] {
  if (!existsSync(KATALOG_POKAZOWEK)) {
    return [];
  }

  return readdirSync(KATALOG_POKAZOWEK, { withFileTypes: true })
    .filter((wpis) => wpis.isFile()
      && wpis.name.endsWith(".md")
      && wpis.name !== "PROGRAM.md"
      && !wpis.name.endsWith(".przygotowanie.md"))
    .map((wpis) => join(KATALOG_POKAZOWEK, wpis.name))
    .sort();
}

const konspekty = znajdzKonspekty();
const przygotowania = existsSync(KATALOG_POKAZOWEK)
  ? readdirSync(KATALOG_POKAZOWEK)
    .filter((nazwa) => nazwa.endsWith(".przygotowanie.md"))
    .sort()
  : [];

describe("konspekty pokazowe w docs/showcase", () => {
  it("zawiera dziewięć osobnych pokazówek", () => {
    expect(konspekty).toHaveLength(9);
  });

  it("każdy konspekt ma osobną instrukcję przygotowania projektu", () => {
    expect(przygotowania).toHaveLength(9);

    for (const konspekt of konspekty) {
      const oczekiwanaNazwa = basename(konspekt, ".md") + ".przygotowanie.md";
      expect(przygotowania).toContain(oczekiwanaNazwa);

      const instrukcja = readFileSync(join(KATALOG_POKAZOWEK, oczekiwanaNazwa), "utf8");
      expect(instrukcja).toContain("# Przygotowanie projektu:");

      if (oczekiwanaNazwa.includes("scratch")) {
        expect(instrukcja).toContain("## Duszki");
      } else {
        // Wszystkie projekty Minecraft wklejamy najszybciej jako Python.
        expect(instrukcja).toContain("```python");
      }

      if (MINECRAFT_BLOCKS.has(oczekiwanaNazwa)) {
        expect(instrukcja).toContain("Tryb końcowy: **MakeCode Blocks**.");
        expect(instrukcja).toContain("Python służy tutaj tylko do szybkiego przygotowania projektu.");
      }

      if (MINECRAFT_PYTHON.has(oczekiwanaNazwa)) {
        expect(instrukcja).toContain("Tryb końcowy: **MakeCode Python**.");
        expect(instrukcja).toContain("nie przełączaj projektu na Blocks");
      }
    }
  });

  it("ma dokładnie trzy projekty końcowe w Blocks i trzy w Pythonie", () => {
    expect([...MINECRAFT_BLOCKS]).toHaveLength(3);
    expect([...MINECRAFT_PYTHON]).toHaveLength(3);

    for (const nazwa of [...MINECRAFT_BLOCKS, ...MINECRAFT_PYTHON]) {
      expect(przygotowania).toContain(nazwa);
    }
  });

  it("Arena Łowców zawiera pełną grę i mały mod Piorunowego Miecza", () => {
    const nazwa = "l05-minecraft-arena-lowcow-potworow.przygotowanie.md";
    const instrukcja = readFileSync(join(KATALOG_POKAZOWEK, nazwa), "utf8");

    expect(instrukcja).toContain("## Pełna gra START");
    expect(instrukcja).toContain("trzy fale");
    expect(instrukcja).toContain("finałowy ravager");
    expect(instrukcja).toContain("player.on_item_interacted(DIAMOND_SWORD, piorunowy_miecz)");
    expect(instrukcja).toContain("gotowa_moc");
    expect(instrukcja).toContain("mobs.spawn(LIGHTNING_BOLT");
    expect(instrukcja).toContain("cooldown");
  });

  it.each(konspekty.map((sciezka) => [relative(KATALOG_POKAZOWEK, sciezka), sciezka]))(
    "%s jest gotowy do importu jako lekcja pokazowa",
    (_nazwa, sciezka) => {
      const { lesson, issues, totalMinutes, plannedMinutes } = parseLessonMarkdown(
        readFileSync(sciezka, "utf8"),
      );

      expect(issues.map((issue) => `${issue.severity}: ${issue.message}`)).toEqual([]);
      expect(lesson.kind).toBe("showcase");
      expect(plannedMinutes).toBe(SHOWCASE_LESSON_MINUTES);
      expect(totalMinutes).toBe(SHOWCASE_LESSON_MINUTES);
      expect(lesson.objective).toBeTruthy();
      expect(lesson.steps).toHaveLength(7);
      expect(lesson.steps.some((step) => step.type === "intro")).toBe(true);
      expect(lesson.steps.some((step) => step.type === "summary")).toBe(true);
      expect(lesson.steps.some((step) => step.type === "break")).toBe(false);
      expect(lesson.steps.flatMap((step) => step.notes).some((note) => note.kind === "error")).toBe(true);
    },
  );
});

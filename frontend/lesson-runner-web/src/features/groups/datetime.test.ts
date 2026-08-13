import { describe, expect, it } from "vitest";
import {
  daysFromToday,
  formatDayDistance,
  formatFriendlyDateTime,
  plural,
} from "./datetime";

/** 12 sierpnia 2026 to środa. */
const SRODA = new Date("2026-08-12T10:00:00");

function at(iso: string): string {
  return new Date(iso).toISOString();
}

describe("daty w języku, jakim mówi o nich człowiek", () => {
  it("liczy w pełnych dniach kalendarzowych, nie w godzinach", () => {
    // Zajęcia jutro o 17:00, gdy jest 23:00 dnia poprzedniego, to jutro,
    // a nie „za 18 godzin”.
    const pozno = new Date("2026-08-12T23:00:00");
    expect(daysFromToday(at("2026-08-13T17:00:00"), pozno)).toBe(1);
  });

  it("nazywa dziś, jutro i wczoraj po imieniu", () => {
    expect(formatFriendlyDateTime(at("2026-08-12T17:00:00"), SRODA)).toContain("dziś o");
    expect(formatFriendlyDateTime(at("2026-08-13T17:00:00"), SRODA)).toContain("jutro o");
    expect(formatFriendlyDateTime(at("2026-08-11T17:00:00"), SRODA)).toContain("wczoraj o");
  });

  it("odmienia dzień tygodnia przez przypadki", () => {
    // Sobota 15.08 — trzy dni od środy. Sam Intl dałby tu „w sobota”.
    expect(formatFriendlyDateTime(at("2026-08-15T17:00:00"), SRODA)).toMatch(/w sobotę o/);
    expect(formatFriendlyDateTime(at("2026-08-16T17:00:00"), SRODA)).toMatch(/w niedzielę o/);
  });

  it("stawia we tylko przed wtorkiem", () => {
    // Wtorek 18.08 — sześć dni od środy, więc jeszcze w horyzoncie tygodnia.
    expect(formatFriendlyDateTime(at("2026-08-18T17:00:00"), SRODA)).toMatch(/we wtorek o/);
    expect(formatFriendlyDateTime(at("2026-08-14T17:00:00"), SRODA)).toMatch(/w piątek o/);
  });

  it("poza horyzontem tygodnia wraca do pełnej daty", () => {
    const daleko = formatFriendlyDateTime(at("2026-09-30T17:00:00"), SRODA);
    expect(daleko).toContain("2026");
    expect(daleko).not.toContain("dziś");
  });

  it("podaje odstęp opisowy bez godziny", () => {
    expect(formatDayDistance(at("2026-08-12T17:00:00"), SRODA)).toBe("dziś");
    expect(formatDayDistance(at("2026-08-14T17:00:00"), SRODA)).toBe("za 2 dni");
    expect(formatDayDistance(at("2026-08-19T17:00:00"), SRODA)).toBe("za tydzień");
    expect(formatDayDistance(at("2026-08-05T17:00:00"), SRODA)).toBe("tydzień temu");
  });

  it("zwraca pusty tekst dla braku daty", () => {
    expect(formatFriendlyDateTime(null)).toBe("");
    expect(formatFriendlyDateTime("nie-data")).toBe("");
    expect(daysFromToday("nie-data")).toBeNull();
  });
});

describe("polska odmiana przez liczbę", () => {
  /**
   * W interfejsie rodzica widniało wcześniej `3 wersje/wersji` — obie formy naraz,
   * bo nikt nie chciał tego rozstrzygać.
   */
  it("dobiera formę zgodnie z regułami języka", () => {
    expect(plural(1, "wersja", "wersje", "wersji")).toBe("wersja");
    expect(plural(2, "wersja", "wersje", "wersji")).toBe("wersje");
    expect(plural(3, "wersja", "wersje", "wersji")).toBe("wersje");
    expect(plural(5, "wersja", "wersje", "wersji")).toBe("wersji");
    expect(plural(12, "wersja", "wersje", "wersji")).toBe("wersji");
    expect(plural(22, "wersja", "wersje", "wersji")).toBe("wersje");
    expect(plural(0, "wersja", "wersje", "wersji")).toBe("wersji");
  });
});

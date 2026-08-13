const dateTimeFormatter = new Intl.DateTimeFormat("pl-PL", {
  weekday: "short",
  day: "2-digit",
  month: "2-digit",
  year: "numeric",
  hour: "2-digit",
  minute: "2-digit",
});

const timeFormatter = new Intl.DateTimeFormat("pl-PL", {
  hour: "2-digit",
  minute: "2-digit",
});

/**
 * Dni tygodnia w bierniku, czyli w formie, której wymaga przyimek „w”.
 *
 * `Intl` zwraca wyłącznie mianownik („sobota”), więc sklejenie go z przyimkiem dawało
 * „w sobota o 17:00”. Tabela jest krótka i zamknięta — nie ma tu czego generalizować.
 * Indeks odpowiada `Date.getDay()`, gdzie 0 to niedziela.
 */
const WEEKDAY_ACCUSATIVE = [
  "w niedzielę",
  "w poniedziałek",
  "we wtorek",
  "w środę",
  "w czwartek",
  "w piątek",
  "w sobotę",
] as const;

const dayMonthFormatter = new Intl.DateTimeFormat("pl-PL", {
  day: "numeric",
  month: "long",
});

/** Czytelna polska data i godzina z wartości ISO (lub pusty string dla braku). */
export function formatDateTime(iso: string | null | undefined): string {
  if (!iso) {
    return "";
  }

  const date = new Date(iso);
  return Number.isNaN(date.getTime()) ? "" : dateTimeFormatter.format(date);
}

/** Sama godzina — do widoku dnia w grafiku instruktora. */
export function formatTime(iso: string | null | undefined): string {
  if (!iso) {
    return "";
  }

  const date = new Date(iso);
  return Number.isNaN(date.getTime()) ? "" : timeFormatter.format(date);
}

/** Dzień i miesiąc słownie: „12 sierpnia”. */
export function formatDayMonth(iso: string | null | undefined): string {
  if (!iso) {
    return "";
  }

  const date = new Date(iso);
  return Number.isNaN(date.getTime()) ? "" : dayMonthFormatter.format(date);
}

/** Liczba pełnych dni kalendarzowych dzielących datę od dzisiaj (ujemna dla przeszłości). */
export function daysFromToday(iso: string, now: Date = new Date()): number | null {
  const date = new Date(iso);

  if (Number.isNaN(date.getTime())) {
    return null;
  }

  const startOfTarget = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  const startOfToday = new Date(now.getFullYear(), now.getMonth(), now.getDate());

  return Math.round((startOfTarget.getTime() - startOfToday.getTime()) / 86_400_000);
}

/**
 * Data w formie, w jakiej mówi o niej człowiek: „dziś o 17:00”, „jutro o 17:00”,
 * „we wtorek o 17:00”, a poza horyzontem tygodnia — pełna data.
 *
 * Powstało dla portalu rodzica. Różnica między „12.08.2026, 17:00” a „jutro o 17:00”
 * to różnica między czytaniem a rozumieniem: rodzic sprawdza harmonogram w biegu
 * i nie powinien przeliczać dat w pamięci.
 *
 * Liczymy w **pełnych dniach kalendarzowych**, a nie w różnicy godzin. Zajęcia
 * o 17:00, gdy jest 23:00 dnia poprzedniego, to „jutro”, a nie „za 18 godzin”.
 */
export function formatFriendlyDateTime(iso: string | null | undefined, now: Date = new Date()): string {
  if (!iso) {
    return "";
  }

  const date = new Date(iso);

  if (Number.isNaN(date.getTime())) {
    return "";
  }

  const days = daysFromToday(iso, now);

  if (days === null) {
    return dateTimeFormatter.format(date);
  }

  if (days === 0) {
    return `dziś o ${timeFormatter.format(date)}`;
  }

  if (days === 1) {
    return `jutro o ${timeFormatter.format(date)}`;
  }

  if (days === -1) {
    return `wczoraj o ${timeFormatter.format(date)}`;
  }

  if (days > 1 && days <= 6) {
    return `${WEEKDAY_ACCUSATIVE[date.getDay()]} o ${timeFormatter.format(date)}`;
  }

  return dateTimeFormatter.format(date);
}

/** Odstęp opisowy bez godziny: „za 2 dni”, „za tydzień”, „dziś”. */
export function formatDayDistance(iso: string | null | undefined, now: Date = new Date()): string {
  const days = iso ? daysFromToday(iso, now) : null;

  if (days === null) {
    return "";
  }

  if (days === 0) {
    return "dziś";
  }

  if (days === 1) {
    return "jutro";
  }

  if (days === -1) {
    return "wczoraj";
  }

  if (days > 1) {
    return days === 7 ? "za tydzień" : `za ${days} ${polishDays(days)}`;
  }

  const past = Math.abs(days);
  return past === 7 ? "tydzień temu" : `${past} ${polishDays(past)} temu`;
}

/**
 * Polska odmiana rzeczownika przez liczbę.
 *
 * `Intl.PluralRules` zna kategorie („one” / „few” / „many”), ale nie zna form wyrazu,
 * więc formy podajemy sami. Wcześniej w portalu rodzica widniało w interfejsie
 * `3 wersje/wersji` — obie formy naraz, bo nikt nie chciał tego rozstrzygać.
 */
const pluralRules = new Intl.PluralRules("pl-PL");

export function plural(count: number, one: string, few: string, many: string): string {
  const category = pluralRules.select(count);

  if (category === "one") {
    return one;
  }

  return category === "few" ? few : many;
}

function polishDays(count: number): string {
  return plural(count, "dzień", "dni", "dni");
}

/** Dodaje pełne tygodnie do daty ISO i zwraca nową datę ISO (do podglądu terminów). */
export function addWeeksIso(iso: string, weeks: number): string {
  const date = new Date(iso);

  if (Number.isNaN(date.getTime())) {
    return "";
  }

  date.setDate(date.getDate() + weeks * 7);
  return date.toISOString();
}

import { existsSync, readFileSync, readdirSync } from "node:fs";
import { basename, join, relative, resolve } from "node:path";

import { parseLessonMarkdown } from "../src/features/import/lessonMarkdown";
import type { CreateLessonRequest, LessonSummary } from "../src/types/lesson";

/**
 * Wsadowy import konspektów z katalogu do działającej instalacji.
 *
 * Ekran importu w aplikacji przyjmuje jeden plik naraz i po utworzeniu konspektu przenosi
 * do edytora - świadomie, bo autor ma tam dorzucić zrzuty ekranu. Przy całorocznym programie
 * to samo zachowanie oznacza jednak dwieście przejść przez ten sam formularz, więc partie
 * plików wgrywamy stąd.
 *
 * Skrypt **nie ma własnego parsera**. Używa tego samego `parseLessonMarkdown`, co ekran
 * importu, żeby plik wgrany wsadowo dawał dokładnie ten sam konspekt co wklejony ręcznie.
 */

type Opcje = {
  katalog: string;
  api: string;
  email: string | null;
  token: string | null;
  dryRun: boolean;
  update: boolean;
  publish: boolean;
  force: boolean;
};

type Konspekt = {
  plik: string;
  nazwa: string;
  lekcja: CreateLessonRequest;
  minuty: number;
  uwagi: string[];
  problemy: string[];
};

const POMOC = `
Wsadowy import konspektów.

  npx vite-node scripts/importKonspektow.ts -- --dir <katalog> [opcje]

Opcje:
  --dir <katalog>    katalog z plikami .md (przeszukiwany rekurencyjnie)
  --api <adres>      adres API, domyślnie http://localhost:5000
  --email <adres>    login do logowania (rola Admin albo Instructor)
  --dry-run          tylko sprawdzenie plików, bez łączenia się z API
  --update           konspekt o tym samym tytule nadpisz zamiast pomijać
  --publish          po utworzeniu opublikuj konspekt (wymaga roli Admin)
  --force            wgraj mimo błędów parsera (fragmenty plików przepadną)

Hasło podaje się zmienną środowiskową LESSON_RUNNER_PASSWORD, nie parametrem -
parametry lądują w historii powłoki. Zamiast loginu można podać gotowy token
zmienną LESSON_RUNNER_TOKEN.
`;

function czytajOpcje(argv: string[]): Opcje | null {
  const args = argv.slice(2).filter((arg) => arg !== "--");

  if (args.includes("--help") || args.includes("-h")) {
    return null;
  }

  function wartosc(nazwa: string): string | null {
    const index = args.indexOf(nazwa);
    return index >= 0 && index + 1 < args.length ? args[index + 1] : null;
  }

  const katalog = wartosc("--dir");
  if (!katalog) {
    return null;
  }

  return {
    katalog: resolve(katalog),
    api: (wartosc("--api") ?? process.env.LESSON_RUNNER_API ?? "http://localhost:5000").replace(/\/$/, ""),
    email: wartosc("--email") ?? process.env.LESSON_RUNNER_EMAIL ?? null,
    token: process.env.LESSON_RUNNER_TOKEN ?? null,
    dryRun: args.includes("--dry-run"),
    update: args.includes("--update"),
    publish: args.includes("--publish"),
    force: args.includes("--force"),
  };
}

/** Pliki .md w kolejności nazw - numer w nazwie pliku wyznacza kolejność lekcji w kursie. */
function znajdzPliki(katalog: string): string[] {
  return readdirSync(katalog, { withFileTypes: true })
    .flatMap((wpis) => {
      const sciezka = join(katalog, wpis.name);

      if (wpis.isDirectory()) {
        return znajdzPliki(sciezka);
      }

      // PROGRAM.md to mapa treści całego kursu, nie konspekt.
      return wpis.name.endsWith(".md") && wpis.name !== "PROGRAM.md" ? [sciezka] : [];
    })
    .sort((a, b) => a.localeCompare(b, "pl"));
}

function wczytaj(katalog: string, plik: string): Konspekt {
  const parsed = parseLessonMarkdown(readFileSync(plik, "utf8"));
  const opis = (issue: { line: number | null; message: string }) =>
    issue.line === null ? issue.message : `linia ${issue.line}: ${issue.message}`;

  return {
    plik,
    nazwa: relative(katalog, plik) || basename(plik),
    lekcja: parsed.lesson,
    minuty: parsed.totalMinutes,
    uwagi: parsed.issues.filter((issue) => issue.severity === "notice").map(opis),
    problemy: parsed.issues.filter((issue) => issue.severity === "error").map(opis),
  };
}

async function zaloguj(opcje: Opcje): Promise<string> {
  if (opcje.token) {
    return opcje.token;
  }

  const password = process.env.LESSON_RUNNER_PASSWORD;
  if (!opcje.email || !password) {
    throw new Error(
      "Brak danych logowania. Podaj --email oraz zmienną LESSON_RUNNER_PASSWORD, albo gotowy LESSON_RUNNER_TOKEN.",
    );
  }

  const odpowiedz = await fetch(`${opcje.api}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: opcje.email, password }),
  });

  if (!odpowiedz.ok) {
    throw new Error(`Logowanie odrzucone (HTTP ${odpowiedz.status}). Sprawdź adres API, login i hasło.`);
  }

  return ((await odpowiedz.json()) as { token: string }).token;
}

async function wyslij<T>(opcje: Opcje, token: string, metoda: string, sciezka: string, body?: unknown): Promise<T> {
  const odpowiedz = await fetch(`${opcje.api}${sciezka}`, {
    method: metoda,
    headers: {
      Authorization: `Bearer ${token}`,
      ...(body === undefined ? {} : { "Content-Type": "application/json" }),
    },
    body: body === undefined ? undefined : JSON.stringify(body),
  });

  if (!odpowiedz.ok) {
    throw new Error(`${metoda} ${sciezka} zwróciło HTTP ${odpowiedz.status}: ${await odpowiedz.text()}`);
  }

  return (await odpowiedz.json()) as T;
}

function raportPlikow(konspekty: Konspekt[]): void {
  for (const konspekt of konspekty) {
    const stan = konspekt.problemy.length > 0 ? "BŁĄD" : "ok";
    console.log(
      `[${stan}] ${konspekt.nazwa} — „${konspekt.lekcja.title}”, ${konspekt.lekcja.steps.length} kroków, ${konspekt.minuty} min`,
    );

    for (const problem of konspekt.problemy) {
      console.log(`        błąd: ${problem}`);
    }
    for (const uwaga of konspekt.uwagi) {
      console.log(`        uwaga: ${uwaga}`);
    }
  }
}

async function main(): Promise<number> {
  const opcje = czytajOpcje(process.argv);

  if (!opcje) {
    console.log(POMOC);
    return 1;
  }

  if (!existsSync(opcje.katalog)) {
    console.error(`Katalog nie istnieje: ${opcje.katalog}`);
    return 1;
  }

  const pliki = znajdzPliki(opcje.katalog);
  if (pliki.length === 0) {
    console.error(`Brak plików .md w katalogu ${opcje.katalog}`);
    return 1;
  }

  const konspekty = pliki.map((plik) => wczytaj(opcje.katalog, plik));
  raportPlikow(konspekty);

  const zProblemami = konspekty.filter((konspekt) => konspekt.problemy.length > 0);
  console.log(`\nPlików: ${konspekty.length}, z błędami: ${zProblemami.length}`);

  // Błąd parsera znaczy, że fragment pliku nie trafi do konspektu. Przy imporcie wsadowym
  // nikt tego nie zobaczy na ekranie, więc domyślnie przerywamy całą partię.
  if (zProblemami.length > 0 && !opcje.force) {
    console.error("\nPrzerwane. Popraw pliki albo powtórz z --force, godząc się na utratę tych fragmentów.");
    return 1;
  }

  if (opcje.dryRun) {
    console.log("\nTryb --dry-run: nic nie zostało wysłane.");
    return 0;
  }

  const token = await zaloguj(opcje);
  const istniejace = new Map(
    (await wyslij<LessonSummary[]>(opcje, token, "GET", "/api/lessons")).map((lekcja) => [lekcja.title, lekcja.id]),
  );

  let utworzone = 0;
  let nadpisane = 0;
  let pominiete = 0;

  for (const konspekt of konspekty) {
    const istniejacyId = istniejace.get(konspekt.lekcja.title);

    if (istniejacyId && !opcje.update) {
      console.log(`pominięty (jest już w systemie): ${konspekt.nazwa}`);
      pominiete++;
      continue;
    }

    const zapisany = istniejacyId
      ? await wyslij<LessonSummary>(opcje, token, "PUT", `/api/lessons/${istniejacyId}`, konspekt.lekcja)
      : await wyslij<LessonSummary>(opcje, token, "POST", "/api/lessons", konspekt.lekcja);

    if (opcje.publish) {
      await wyslij<LessonSummary>(opcje, token, "POST", `/api/lessons/${zapisany.id}/publish`);
    }

    console.log(`${istniejacyId ? "nadpisany" : "utworzony"}: ${konspekt.nazwa} → ${zapisany.id}`);
    istniejace.set(zapisany.title, zapisany.id);

    if (istniejacyId) {
      nadpisane++;
    } else {
      utworzone++;
    }
  }

  console.log(`\nGotowe. Utworzone: ${utworzone}, nadpisane: ${nadpisane}, pominięte: ${pominiete}.`);
  return 0;
}

main()
  .then((kod) => process.exit(kod))
  .catch((powod: unknown) => {
    console.error(`\n${powod instanceof Error ? powod.message : String(powod)}`);
    process.exit(1);
  });

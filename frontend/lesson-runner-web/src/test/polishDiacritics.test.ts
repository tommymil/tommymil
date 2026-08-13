import { describe, expect, it } from "vitest";

/**
 * Strażnik zasady z CLAUDE.md: teksty po polsku piszemy z pełnymi znakami diakrytycznymi.
 *
 * Test wyłapuje dwa rodzaje błędów, które już raz przeszły do UI:
 *  1. brakujące ogonki ("Odswiez", "Tresc", "hasla"),
 *  2. nadgorliwą podmianę, która wstawiła ogonek tam, gdzie go nie ma
 *     ("instruktóra", "postaći", "sprawdźenia") - ślad po dawnym find/replace.
 *
 * Sprawdzamy tylko kod źródłowy aplikacji. Identyfikatory, wartości enumów, klucze,
 * nazwy tras i klas CSS zostają w ASCII, dlatego lista zawiera wyłącznie formy wyrazowe,
 * które nie mają technicznego zastosowania.
 */

const BLEDNE_FORMY = [
  // brakujące znaki diakrytyczne
  "Odswiez", "odswiez", "Wyslij", "wyslij", "Tresc", "tresc", "tresci",
  "Podglad", "podglad", "podgladu", "wiadomosc", "wiadomosci",
  "haslo", "hasla", "hasel", "Uzyj", "uzyj",
  "ustawien", "powiadomien", "przypomnien", "wpisow",
  "jesli", "Jesli", "ktory", "ktora", "ktore", "wiecej", "mozna", "moze",
  "czesc", "blad", "bledy", "bledu", "zajec", "obecnosc", "nieobecnosc",
  "platnosc", "platnosci", "zaleglosc", "dostepny", "dostepne", "niedostepny",
  "oblozenie", "zapelnienie", "zastepstwo", "odwolanie", "rozliczen",
  "sprawdz", "Sprawdz", "wybor", "Wybor", "powrot", "nastepny", "nastepna",
  "zobaczyc", "ustawic", "usuniecie", "potwierdz", "zadnych", "wlacz", "wylacz",
  "Uzupelnij", "uzupelnij", "nazwe", "kursow", "gore", "W gore", "W dol",
  "wczesniej", "wylacznie", "tokenow", "widokow", "szczegoly",
  "slow", "zachowujac", "wypelnia", "trafiaja", "chetnych", "zostaja",
  // nadgorliwa podmiana - ogonek tam, gdzie nie powinien stać
  "instruktór", "instruktóra", "instruktórów", "postaći", "postaćia",
  "odpowiedźi", "sprawdźenia", "sprawdźić", "utwórzyc", "pokażywania",
  "pokażac", "przesuńac", "testówa", "zadańie", "zadańia", "imięnnie", "imięniu",
  "odpowiedźialnosci",
];

const wzorzec = new RegExp(`\\b(${BLEDNE_FORMY.join("|")})\\b`);

// Vite wciąga zawartość plików w czasie budowania testu - bez zależności od API Node'a.
const zrodla = import.meta.glob("../**/*.{ts,tsx,css}", { query: "?raw", import: "default", eager: true });

describe("polskie cudzysłowy", () => {
  /**
   * Otwierający „ musi mieć swoją parę ”. Zamknięcie zwykłym " psuje literał w C#
   * (`"Status „w toku" ..."` kończy string w środku zdania i wywala build), a w TypeScripcie
   * wygląda po prostu źle. Regresja z 27.07.2026.
   */
  it("są zawsze sparowane", () => {
    const zle: string[] = [];

    for (const [sciezka, zawartosc] of Object.entries(zrodla)) {
      if (sciezka.includes("polishDiacritics.test")) {
        continue;
      }

      (zawartosc as string).split("\n").forEach((linia, index) => {
        const otwierajace = (linia.match(/„/g) ?? []).length;
        const zamykajace = (linia.match(/”/g) ?? []).length;

        if (otwierajace !== zamykajace) {
          zle.push(`${sciezka.replace("../", "")}:${index + 1} → ${linia.trim().slice(0, 80)}`);
        }
      });
    }

    expect(zle).toEqual([]);
  });
});

describe("polskie znaki w tekstach", () => {
  it("nie zawiera form bez diakrytyków ani nadgorliwych podmian", () => {
    const znalezione: string[] = [];

    for (const [sciezka, zawartosc] of Object.entries(zrodla)) {
      // Ten plik z definicji zawiera błędne formy - to jego lista kontrolna.
      if (sciezka.includes("polishDiacritics.test")) {
        continue;
      }

      (zawartosc as string).split("\n").forEach((linia, index) => {
        const trafienie = wzorzec.exec(linia);

        if (trafienie) {
          znalezione.push(`${sciezka.replace("../", "")}:${index + 1} → "${trafienie[1]}"`);
        }
      });
    }

    expect(znalezione).toEqual([]);
  });
});

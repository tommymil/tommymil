// Wspólne, czytelne etykiety dla wartości enumów, które backend przesyła jako surowe stringi
// (typ kroku, rodzaj notatki, rodzaj zasobu). Dzięki temu prezenter i edytor pokazują
// to samo nazewnictwo zamiast technicznych "concept" / "hint" / "file".

export const stepTypeLabels: Record<string, string> = {
  intro: "Wprowadzenie",
  review: "Powtórka",
  concept: "Nowy koncept",
  demo: "Demo na żywo",
  guided: "Ćwiczenie z prowadzeniem",
  challenge: "Samodzielne wyzwanie",
  summary: "Podsumowanie",
  break: "Przerwa",
};

/**
 * `faster` i `shorter` powstały z rozbicia `pace`, który mieszał dwie przeciwne sytuacje:
 * „trójka skończyła, daj im coś więcej” i „zostało dziesięć minut, co wolno wyciąć”.
 * W kokpicie to są dwa różne pytania, zadawane w różnych momentach zajęć.
 */
export const noteKindLabels: Record<string, string> = {
  error: "Częsty błąd",
  hint: "Podpowiedź",
  pace: "Tempo",
  faster: "Dla szybszych",
  shorter: "Gdy nie zdążysz",
};

export const resourceKindLabels: Record<string, string> = {
  link: "Link",
  code: "Kod",
  image: "Obraz",
  file: "Plik",
};

export function stepTypeLabel(type: string): string {
  return stepTypeLabels[type] ?? type;
}

export function noteKindLabel(kind: string): string {
  return noteKindLabels[kind] ?? kind;
}

export function resourceKindLabel(kind: string): string {
  return resourceKindLabels[kind] ?? kind;
}

export type Theme = "light" | "dark";

const STORAGE_KEY = "lr-theme";

export function getStoredTheme(): Theme {
  const saved = localStorage.getItem(STORAGE_KEY);

  if (saved === "light" || saved === "dark") {
    return saved;
  }

  // Brak zapisanego wyboru - podążamy za ustawieniem systemu (gdy dostępne).
  const prefersDark =
    typeof window !== "undefined" &&
    typeof window.matchMedia === "function" &&
    window.matchMedia("(prefers-color-scheme: dark)").matches;

  return prefersDark ? "dark" : "light";
}

export function applyTheme(theme: Theme): void {
  document.documentElement.setAttribute("data-theme", theme);
}

export function storeTheme(theme: Theme): void {
  localStorage.setItem(STORAGE_KEY, theme);
}

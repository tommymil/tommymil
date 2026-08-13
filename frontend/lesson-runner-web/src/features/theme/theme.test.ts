import { beforeEach, describe, expect, it } from "vitest";
import { applyTheme, getStoredTheme, storeTheme } from "./theme";

describe("theme", () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute("data-theme");
  });

  it("applies the chosen theme to the document root", () => {
    applyTheme("dark");
    expect(document.documentElement.getAttribute("data-theme")).toBe("dark");
  });

  it("stores and reads back the chosen theme", () => {
    storeTheme("dark");
    expect(getStoredTheme()).toBe("dark");
  });

  it("falls back to light when nothing is stored and no system preference is set", () => {
    expect(getStoredTheme()).toBe("light");
  });
});

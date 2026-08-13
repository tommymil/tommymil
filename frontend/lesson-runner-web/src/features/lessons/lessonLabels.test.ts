import { describe, expect, it } from "vitest";
import { noteKindLabel, resourceKindLabel, stepTypeLabel } from "./lessonLabels";

describe("lessonLabels", () => {
  it("maps known step types, note kinds and resource kinds to Polish labels", () => {
    expect(stepTypeLabel("concept")).toBe("Nowy koncept");
    expect(stepTypeLabel("guided")).toBe("Ćwiczenie z prowadzeniem");
    expect(noteKindLabel("error")).toBe("Częsty błąd");
    expect(noteKindLabel("pace")).toBe("Tempo");
    expect(resourceKindLabel("file")).toBe("Plik");
  });

  it("falls back to the raw value for unknown keys", () => {
    expect(stepTypeLabel("unknown")).toBe("unknown");
    expect(noteKindLabel("custom")).toBe("custom");
    expect(resourceKindLabel("zip")).toBe("zip");
  });
});

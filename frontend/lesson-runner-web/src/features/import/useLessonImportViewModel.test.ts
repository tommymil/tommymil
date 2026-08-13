import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useLessonImportViewModel } from "./useLessonImportViewModel";
import type { LessonDetails } from "../../types/lesson";
import * as lessonsApi from "../../api/lessonsApi";

const navigateMock = vi.fn();

vi.mock("react-router-dom", async (importActual) => {
  const actual = await importActual<typeof import("react-router-dom")>();
  return { ...actual, useNavigate: () => navigateMock };
});

vi.mock("../../api/lessonsApi");

const sample = `# Lekcja testowa
Subject: Scratch

## [concept] Krok (5 min)

### Co robić teraz
- Zrób coś
`;

describe("useLessonImportViewModel", () => {
  beforeEach(() => {
    navigateMock.mockReset();
  });

  it("parses text and enables import for a valid document", () => {
    const { result } = renderHook(() => useLessonImportViewModel());

    act(() => result.current.setText(sample));

    expect(result.current.parsed?.lesson.title).toBe("Lekcja testowa");
    expect(result.current.canImport).toBe(true);
  });

  it("does not allow import without a title or steps", () => {
    const { result } = renderHook(() => useLessonImportViewModel());

    act(() => result.current.setText("Subject: Scratch"));

    expect(result.current.canImport).toBe(false);
  });

  it("creates the lesson and opens the editor on import", async () => {
    const saved = { id: "new-id" } as LessonDetails;
    vi.mocked(lessonsApi.createLesson).mockResolvedValue(saved);

    const { result } = renderHook(() => useLessonImportViewModel());
    act(() => result.current.setText(sample));

    await act(async () => {
      await result.current.importLesson();
    });

    expect(lessonsApi.createLesson).toHaveBeenCalledWith(
      expect.objectContaining({ title: "Lekcja testowa", steps: expect.any(Array) }),
    );
    expect(navigateMock).toHaveBeenCalledWith("/admin/lessons/new-id/edit");
  });

  /**
   * Import z zgubioną treścią musi być decyzją. Wcześniej przycisk był aktywny niezależnie
   * od liczby ostrzeżeń, więc konspekt bez połowy scenariusza tworzył się jednym kliknięciem.
   */
  it("blocks the import until dropped content is acknowledged", () => {
    const { result } = renderHook(() => useLessonImportViewModel());

    act(() => result.current.setText("# Lekcja\n## Krok (5 min)\n### Nieznana sekcja\n- Zgubiony punkt\n"));

    expect(result.current.errors).toHaveLength(2);
    expect(result.current.canImport).toBe(false);

    act(() => result.current.setAcceptErrors(true));

    expect(result.current.canImport).toBe(true);
  });

  it("withdraws the acknowledgement after the text changes", () => {
    const { result } = renderHook(() => useLessonImportViewModel());

    act(() => result.current.setText("# Lekcja\n## Krok (5 min)\n### Nieznana sekcja\n- Zgubiony punkt\n"));
    act(() => result.current.setAcceptErrors(true));
    act(() => result.current.setText("# Lekcja\n## Krok (5 min)\n### Inna nieznana\n- Nadal zgubiony\n"));

    expect(result.current.acceptErrors).toBe(false);
    expect(result.current.canImport).toBe(false);
  });

  it("loads file content into the editor text", async () => {
    const { result } = renderHook(() => useLessonImportViewModel());
    const file = { text: () => Promise.resolve(sample) } as unknown as File;

    await act(async () => {
      await result.current.loadFile(file);
    });

    expect(result.current.text).toContain("Lekcja testowa");
    expect(result.current.parsed?.lesson.steps).toHaveLength(1);
  });
});

import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { SegmentedControl } from "./SegmentedControl";

const segments = [
  { value: "present", label: "Obecny", tone: "seg-present" },
  { value: "late", label: "Spóźniony", tone: "seg-late" },
  { value: "unexcusedabsence", label: "Nieobecny", tone: "seg-absent" },
];

describe("SegmentedControl", () => {
  it("pokazuje wszystkie stany naraz i zaznacza wybrany", () => {
    render(
      <SegmentedControl
        ariaLabel="Obecność: Jan Kowalski"
        value="late"
        segments={segments}
        onChange={vi.fn()}
      />,
    );

    // Cała wartość tego komponentu: trzy stany widoczne bez rozwijania listy.
    expect(screen.getAllByRole("button")).toHaveLength(3);
    expect(screen.getByRole("button", { name: "Spóźniony" })).toHaveAttribute("aria-pressed", "true");
    expect(screen.getByRole("button", { name: "Obecny" })).toHaveAttribute("aria-pressed", "false");
  });

  it("zgłasza wybraną wartość", () => {
    const onChange = vi.fn();
    render(
      <SegmentedControl ariaLabel="Obecność" value={null} segments={segments} onChange={onChange} />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Obecny" }));

    expect(onChange).toHaveBeenCalledWith("present");
  });

  it("przy statusie spoza skrótu nie zaznacza żadnego segmentu", () => {
    render(
      <SegmentedControl ariaLabel="Obecność" value={null} segments={segments} onChange={vi.fn()} />,
    );

    // Rzadszy status (np. „problemy techniczne”) mieszka pod osobnym przyciskiem —
    // udawanie, że pasuje do jednego z trzech skrótów, byłoby kłamstwem o danych.
    expect(screen.queryByRole("button", { pressed: true })).not.toBeInTheDocument();
  });

  it("ma etykietę wskazującą, kogo dotyczy", () => {
    render(
      <SegmentedControl
        ariaLabel="Obecność: Jan Kowalski"
        value="present"
        segments={segments}
        onChange={vi.fn()}
      />,
    );

    expect(screen.getByRole("group", { name: "Obecność: Jan Kowalski" })).toBeInTheDocument();
  });
});

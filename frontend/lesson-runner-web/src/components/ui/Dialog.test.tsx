import { fireEvent, render, screen } from "@testing-library/react";
import { useState } from "react";
import { describe, expect, it, vi } from "vitest";
import { Dialog } from "./Dialog";
import { Button } from "./Button";

/**
 * Okno modalne jest jedynym miejscem, w którym aplikacja przejmuje uwagę użytkownika
 * w całości. Testujemy dokładnie te rzeczy, których brakowało trzem wcześniejszym
 * nakładkom `.overlay`: semantykę, pułapkę fokusu, Escape i powrót fokusu.
 */
describe("Dialog", () => {
  it("ma semantykę okna modalnego powiązaną z tytułem", () => {
    render(
      <Dialog title="Odwołać zajęcia?" onClose={vi.fn()}>
        <p>Treść</p>
      </Dialog>,
    );

    const dialog = screen.getByRole("dialog");
    expect(dialog).toHaveAttribute("aria-modal", "true");
    expect(dialog).toHaveAccessibleName("Odwołać zajęcia?");
  });

  it("zamyka się klawiszem Escape", () => {
    const onClose = vi.fn();
    render(<Dialog title="Tytuł" onClose={onClose} />);

    fireEvent.keyDown(document, { key: "Escape" });

    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("zamyka się kliknięciem w tło tylko wtedy, gdy na to pozwolimy", () => {
    const onClose = vi.fn();
    const { rerender } = render(<Dialog title="Tytuł" onClose={onClose} />);

    fireEvent.mouseDown(document.querySelector(".dialog-scrim") as HTMLElement);
    expect(onClose).toHaveBeenCalledTimes(1);

    // Przy formularzu z niezapisanymi danymi przypadkowe kliknięcie obok kasowałoby pracę.
    rerender(<Dialog title="Tytuł" onClose={onClose} dismissOnScrim={false} />);
    fireEvent.mouseDown(document.querySelector(".dialog-scrim") as HTMLElement);
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("ustawia fokus w oknie i oddaje go elementowi wywołującemu", () => {
    function Harness() {
      const [open, setOpen] = useState(false);

      return (
        <>
          <button type="button" onClick={() => setOpen(true)}>
            Otwórz
          </button>
          {open ? (
            <Dialog
              title="Tytuł"
              onClose={() => setOpen(false)}
              actions={<Button onClick={() => setOpen(false)}>Potwierdź</Button>}
            >
              <input aria-label="Powód" />
            </Dialog>
          ) : null}
        </>
      );
    }

    render(<Harness />);
    const trigger = screen.getByRole("button", { name: "Otwórz" });
    trigger.focus();
    fireEvent.click(trigger);

    // Fokus ląduje na pierwszym elemencie okna, a nie zostaje pod spodem.
    expect(document.activeElement).not.toBe(trigger);
    expect(screen.getByRole("dialog").contains(document.activeElement)).toBe(true);

    fireEvent.keyDown(document, { key: "Escape" });

    // Bez tego użytkownik klawiatury ląduje na początku dokumentu.
    expect(document.activeElement).toBe(trigger);
  });

  it("zapętla Tab wewnątrz okna", () => {
    render(
      <Dialog title="Tytuł" onClose={vi.fn()} actions={<Button>Ostatni</Button>}>
        <input aria-label="Pierwszy" />
      </Dialog>,
    );

    const last = screen.getByRole("button", { name: "Ostatni" });
    last.focus();
    fireEvent.keyDown(document, { key: "Tab" });

    expect(screen.getByRole("dialog").contains(document.activeElement)).toBe(true);
  });
});

import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { DialogProvider, useDialogs } from "./DialogContext";

/**
 * `DialogProvider` zastąpił siedem wywołań `window.confirm` / `window.prompt`.
 * Testujemy kontrakt, na którym opierają się wszystkie wywołania: obietnica rozwiązuje
 * się dopiero po decyzji użytkownika i niesie jej wynik.
 */
function Harness({ onResult }: { onResult: (value: unknown) => void }) {
  const { confirm, prompt } = useDialogs();

  return (
    <>
      <button
        type="button"
        onClick={async () =>
          onResult(
            await confirm({
              title: "Usunąć grupę?",
              tone: "danger",
              confirmLabel: "Usuń",
              consequences: ["12 terminów zniknie."],
            }),
          )
        }
      >
        Pytaj
      </button>

      <button
        type="button"
        onClick={async () =>
          onResult(await confirm({ title: "Nieodwracalne", confirmLabel: "Tak", typeToConfirm: "Jan Kowalski" }))
        }
      >
        Pytaj z przepisaniem
      </button>

      <button
        type="button"
        onClick={async () => onResult(await prompt({ title: "Powód", label: "Powód", optional: false }))}
      >
        Pytaj o powód
      </button>
    </>
  );
}

function renderHarness() {
  const onResult = vi.fn();
  render(
    <DialogProvider>
      <Harness onResult={onResult} />
    </DialogProvider>,
  );
  return onResult;
}

describe("DialogProvider", () => {
  it("zwraca true po potwierdzeniu i pokazuje skutki decyzji", async () => {
    const onResult = renderHarness();

    fireEvent.click(screen.getByRole("button", { name: "Pytaj" }));

    // Podsumowanie skutków to cała wartość potwierdzenia przy operacji nieodwracalnej.
    expect(await screen.findByText("12 terminów zniknie.")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Usuń" }));

    await waitFor(() => expect(onResult).toHaveBeenCalledWith(true));
  });

  it("zwraca false po anulowaniu", async () => {
    const onResult = renderHarness();

    fireEvent.click(screen.getByRole("button", { name: "Pytaj" }));
    fireEvent.click(await screen.findByRole("button", { name: "Anuluj" }));

    await waitFor(() => expect(onResult).toHaveBeenCalledWith(false));
  });

  it("blokuje potwierdzenie, dopóki nazwa nie zostanie przepisana", async () => {
    const onResult = renderHarness();

    fireEvent.click(screen.getByRole("button", { name: "Pytaj z przepisaniem" }));

    const confirmButton = await screen.findByRole("button", { name: "Tak" });
    expect(confirmButton).toBeDisabled();

    fireEvent.change(screen.getByRole("textbox"), { target: { value: "Jan Kowal" } });
    expect(confirmButton).toBeDisabled();

    fireEvent.change(screen.getByRole("textbox"), { target: { value: "Jan Kowalski" } });
    expect(confirmButton).toBeEnabled();

    fireEvent.click(confirmButton);
    await waitFor(() => expect(onResult).toHaveBeenCalledWith(true));
  });

  it("zwraca wpisany tekst z pytania o powód, a null po anulowaniu", async () => {
    const onResult = renderHarness();

    fireEvent.click(screen.getByRole("button", { name: "Pytaj o powód" }));
    fireEvent.change(await screen.findByRole("textbox"), { target: { value: "  ferie zimowe  " } });
    fireEvent.click(screen.getByRole("button", { name: "Zapisz" }));

    // Wartość przycinamy - „ ” nie jest powodem.
    await waitFor(() => expect(onResult).toHaveBeenCalledWith("ferie zimowe"));

    fireEvent.click(screen.getByRole("button", { name: "Pytaj o powód" }));
    fireEvent.click(await screen.findByRole("button", { name: "Anuluj" }));

    await waitFor(() => expect(onResult).toHaveBeenLastCalledWith(null));
  });

  it("nie pozwala zatwierdzić pustego powodu, gdy pole jest wymagane", async () => {
    renderHarness();

    fireEvent.click(screen.getByRole("button", { name: "Pytaj o powód" }));

    expect(await screen.findByRole("button", { name: "Zapisz" })).toBeDisabled();
  });
});

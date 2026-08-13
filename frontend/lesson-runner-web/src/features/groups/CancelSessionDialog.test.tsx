import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { CancelSessionDialog } from "./CancelSessionDialog";
import type { ScheduledSession } from "../../types/group";

const session: ScheduledSession = {
  id: "s1",
  groupId: "g1",
  groupName: "Roblox Wt 17:00",
  lessonId: "l1",
  lessonTitle: "Tor przeszkód",
  scheduledAt: "2026-08-18T15:00:00.000Z",
  sequenceNumber: 7,
  status: "planned",
  statusLabel: "Zaplanowane",
  startedAt: null,
  completedAt: null,
  instructorNote: null,
};

function renderDialog(overrides: Partial<Parameters<typeof CancelSessionDialog>[0]> = {}) {
  const onConfirm = vi.fn().mockResolvedValue(undefined);

  render(
    <CancelSessionDialog
      session={session}
      enrolledCount={8}
      followingCount={5}
      busy={false}
      onClose={vi.fn()}
      onConfirm={onConfirm}
      {...overrides}
    />,
  );

  return onConfirm;
}

/**
 * Najważniejsza operacja w systemie. Testujemy to, czego brakowało poprzedniej wersji
 * rozsypanej po wierszu listy: pokazanie skutków przed decyzją i wymuszenie powodu.
 */
describe("CancelSessionDialog", () => {
  it("pokazuje skutki decyzji przed potwierdzeniem", () => {
    renderDialog();

    expect(screen.getByText(/Dotyczy 8 dzieci/)).toBeInTheDocument();
    expect(screen.getByText(/Każde zapisane dziecko dostanie 1 kredyt/)).toBeInTheDocument();
  });

  /**
   * Wysyłka jest automatyczna i bezwarunkowa. Okno musi to mówić wprost — wcześniej
   * twierdziło „wiadomości wyślij osobno w panelu Powiadomienia”, co przestało być
   * prawdą, gdy powiadomienia o odwołaniu zaczęły wychodzić same.
   */
  it("zapowiada automatyczną wysyłkę e-maili do opiekunów", () => {
    renderDialog();

    expect(screen.getByText(/Opiekunowie 8 dzieci dostaną e-mail/)).toBeInTheDocument();
    expect(screen.queryByText(/wyślij osobno/)).not.toBeInTheDocument();
  });

  it("nie obiecuje wysyłki, gdy nie ma komu wysłać", () => {
    renderDialog({ enrolledCount: 0 });

    expect(screen.getByText(/Nie ma komu wysłać wiadomości/)).toBeInTheDocument();
  });

  /**
   * Checkbox przestał znaczyć „czy powiadomić” — system powiadamia sam. Zostaje jako ślad
   * kanału zapasowego, więc domyślnie musi być odznaczony: domyślnie nikt nie dzwonił.
   */
  it("domyślnie nie deklaruje kontaktu telefonicznego", () => {
    renderDialog();

    expect(screen.getByLabelText(/telefonicznie lub SMS-em/)).not.toBeChecked();
  });

  it("nie pozwala odwołać zajęć bez powodu", () => {
    renderDialog();

    const confirmButton = screen.getByRole("button", { name: "Odwołaj zajęcia" });
    expect(confirmButton).toBeDisabled();

    fireEvent.change(screen.getByPlaceholderText(/choroba instruktora/), {
      target: { value: "choroba instruktora" },
    });

    expect(confirmButton).toBeEnabled();
  });

  it("zmienia opis skutków razem z wyborem rozliczenia", () => {
    renderDialog();

    fireEvent.change(screen.getByDisplayValue("Kredyt zajęciowy dla każdego dziecka"), {
      target: { value: "refund" },
    });

    // Zwrot i odrobienie wymagają rozmowy z rodzicem — system ich nie utworzy sam.
    expect(screen.getByText(/wpisać ręcznie po ustaleniach z rodzicem/)).toBeInTheDocument();
  });

  it("opisuje skutek przesunięcia materiału dopiero po jego włączeniu", () => {
    renderDialog();

    expect(screen.getByText(/Materiał z tych zajęć przepada/)).toBeInTheDocument();

    fireEvent.click(screen.getByLabelText(/Przesuń materiał na kolejne terminy/));

    expect(screen.getByText(/kurs wydłuży się o jedne zajęcia/)).toBeInTheDocument();
  });

  it("przekazuje komplet decyzji jednym żądaniem", async () => {
    const onConfirm = renderDialog();

    fireEvent.change(screen.getByPlaceholderText(/choroba instruktora/), {
      target: { value: "  ferie zimowe  " },
    });
    fireEvent.click(screen.getByLabelText(/Przesuń materiał na kolejne terminy/));
    fireEvent.click(screen.getByRole("button", { name: "Odwołaj zajęcia" }));

    await waitFor(() =>
      expect(onConfirm).toHaveBeenCalledWith({
        reason: "ferie zimowe",
        guardiansNotified: false,
        cancelledBy: "instructor",
        compensation: "credit",
        shiftFollowingLessons: true,
      }),
    );
  });

  it("radzi sobie z pustą grupą", () => {
    renderDialog({ enrolledCount: 0 });

    expect(screen.getByText("Do tej grupy nie jest zapisane żadne dziecko.")).toBeInTheDocument();
  });
});

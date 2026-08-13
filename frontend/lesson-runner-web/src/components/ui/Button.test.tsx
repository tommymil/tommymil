import { describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Button } from "./Button";

describe("Button", () => {
  it("renders children and the default primary variant", () => {
    render(<Button>Zapisz</Button>);

    const button = screen.getByRole("button", { name: "Zapisz" });
    expect(button).toBeInTheDocument();
    expect(button).toHaveClass("button", "button-primary");
  });

  it("applies the requested variant", () => {
    render(<Button variant="secondary">Edytuj</Button>);

    expect(screen.getByRole("button", { name: "Edytuj" })).toHaveClass("button-secondary");
  });

  it("fires onClick when enabled", async () => {
    const onClick = vi.fn();
    render(<Button onClick={onClick}>Klik</Button>);

    await userEvent.click(screen.getByRole("button", { name: "Klik" }));

    expect(onClick).toHaveBeenCalledOnce();
  });

  it("does not fire onClick when disabled", async () => {
    const onClick = vi.fn();
    render(
      <Button onClick={onClick} disabled>
        Klik
      </Button>,
    );

    await userEvent.click(screen.getByRole("button", { name: "Klik" }));

    expect(onClick).not.toHaveBeenCalled();
  });
});

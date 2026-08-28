import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { MemoryRouter } from "react-router-dom";
import { AppShell } from "./AppShell";

vi.mock("../../features/auth/AuthContext", () => ({
  useAuth: () => ({
    user: {
      id: "admin-1",
      email: "admin@example.com",
      displayName: "Adam Admin",
      role: "admin",
    },
    status: "authenticated",
    isAdmin: true,
    isParent: false,
    isStaff: true,
    signOut: vi.fn(),
  }),
}));

vi.mock("../../features/theme/useTheme", () => ({
  useTheme: () => ({ theme: "light", toggleTheme: vi.fn() }),
}));

vi.mock("./GlobalSearch", () => ({ GlobalSearch: () => null }));

function renderShell() {
  return render(
    <MemoryRouter initialEntries={["/admin/dashboard"]}>
      <AppShell>
        <p>Treść strony</p>
      </AppShell>
    </MemoryRouter>,
  );
}

describe("AppShell sidebar", () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it("allows the desktop sidebar to be hidden and shown again", async () => {
    const user = userEvent.setup();
    const { container } = renderShell();
    const shell = container.querySelector(".app-shell");

    await user.click(screen.getByRole("button", { name: "Ukryj boczny pasek nawigacji" }));

    expect(shell).toHaveClass("app-shell-sidebar-collapsed");
    expect(localStorage.getItem("lesson-runner:sidebar-collapsed")).toBe("true");

    await user.click(screen.getByRole("button", { name: "Pokaż boczny pasek nawigacji" }));

    expect(shell).not.toHaveClass("app-shell-sidebar-collapsed");
    expect(localStorage.getItem("lesson-runner:sidebar-collapsed")).toBe("false");
  });

  it("restores the hidden sidebar preference", () => {
    localStorage.setItem("lesson-runner:sidebar-collapsed", "true");

    const { container } = renderShell();

    expect(container.querySelector(".app-shell")).toHaveClass("app-shell-sidebar-collapsed");
  });
});

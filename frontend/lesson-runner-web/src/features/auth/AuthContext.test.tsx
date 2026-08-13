import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { AuthProvider, useAuth } from "./AuthContext";
import * as authApi from "../../api/authApi";
import type { AuthResponse, AuthUser } from "../../types/auth";

vi.mock("../../api/authApi");

const adminUser: AuthUser = { id: "1", email: "admin@example.com", role: "admin", displayName: "Adam Admin" };
const adminResponse: AuthResponse = {
  token: "jwt-token",
  expiresAt: new Date(Date.now() + 3_600_000).toISOString(),
  user: adminUser,
};

function Harness() {
  const { status, user, isAdmin, signIn, signOut } = useAuth();

  return (
    <div>
      <span data-testid="status">{status}</span>
      <span data-testid="email">{user?.email ?? "-"}</span>
      <span data-testid="admin">{isAdmin ? "yes" : "no"}</span>
      <button onClick={() => void signIn("admin@example.com", "secret12345")}>login</button>
      <button onClick={() => signOut()}>logout</button>
    </div>
  );
}

function renderHarness() {
  return render(
    <AuthProvider>
      <Harness />
    </AuthProvider>,
  );
}

describe("AuthContext", () => {
  beforeEach(() => {
    localStorage.clear();
    vi.mocked(authApi.login).mockResolvedValue(adminResponse);
    vi.mocked(authApi.getCurrentUser).mockResolvedValue(adminUser);
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  it("starts anonymous when there is no stored session", async () => {
    renderHarness();

    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("anonymous"));
    expect(screen.getByTestId("email")).toHaveTextContent("-");
  });

  it("signs in, exposes the user and persists the session", async () => {
    renderHarness();
    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("anonymous"));

    await userEvent.click(screen.getByRole("button", { name: "login" }));

    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("authenticated"));
    expect(screen.getByTestId("email")).toHaveTextContent("admin@example.com");
    expect(screen.getByTestId("admin")).toHaveTextContent("yes");
    expect(localStorage.getItem("lesson-runner:auth:token")).toBe("jwt-token");
  });

  it("signs out and clears the stored session", async () => {
    renderHarness();
    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("anonymous"));
    await userEvent.click(screen.getByRole("button", { name: "login" }));
    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("authenticated"));

    await userEvent.click(screen.getByRole("button", { name: "logout" }));

    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("anonymous"));
    expect(localStorage.getItem("lesson-runner:auth:token")).toBeNull();
  });

  it("restores a stored session and validates it via /me", async () => {
    localStorage.setItem("lesson-runner:auth:token", "stored-token");
    localStorage.setItem("lesson-runner:auth:user", JSON.stringify(adminUser));

    renderHarness();

    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("authenticated"));
    expect(authApi.getCurrentUser).toHaveBeenCalled();
    expect(screen.getByTestId("email")).toHaveTextContent("admin@example.com");
  });

  it("drops a stored session when validation fails", async () => {
    localStorage.setItem("lesson-runner:auth:token", "stale-token");
    localStorage.setItem("lesson-runner:auth:user", JSON.stringify(adminUser));
    vi.mocked(authApi.getCurrentUser).mockRejectedValueOnce(new Error("401"));

    renderHarness();

    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("anonymous"));
    expect(localStorage.getItem("lesson-runner:auth:token")).toBeNull();
  });
});

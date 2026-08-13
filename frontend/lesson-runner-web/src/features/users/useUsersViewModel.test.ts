import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useUsersViewModel } from "./useUsersViewModel";
import type { ManagedUser } from "../../types/user";
import * as usersApi from "../../api/usersApi";

vi.mock("../../api/usersApi");

const profileBase = { firstName: null, lastName: null, phone: null };
const users: ManagedUser[] = [
  { id: "u1", email: "admin@x.pl", role: "admin", isActive: true, ...profileBase, displayName: "admin@x.pl" },
  { id: "u2", email: "i@x.pl", role: "instructor", isActive: true, ...profileBase, displayName: "i@x.pl" },
];

describe("useUsersViewModel", () => {
  beforeEach(() => {
    vi.mocked(usersApi.getUsers).mockResolvedValue(users);
    vi.mocked(usersApi.createUser).mockResolvedValue({ id: "u3", email: "nowy@x.pl", role: "instructor", isActive: true, ...profileBase, displayName: "nowy@x.pl" });
    vi.mocked(usersApi.setUserActive).mockResolvedValue(undefined);
    vi.mocked(usersApi.inviteUser).mockResolvedValue({
      sent: true,
      expiresAt: new Date().toISOString(),
      error: null,
    });
  });

  it("loads accounts", async () => {
    const { result } = renderHook(() => useUsersViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.users).toHaveLength(2);
  });

  it("creates an account and appends it", async () => {
    const { result } = renderHook(() => useUsersViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setEmail("nowy@x.pl"));
    act(() => result.current.setPassword("password123"));
    await act(async () => result.current.addUser());

    expect(usersApi.createUser).toHaveBeenCalledWith({ email: "nowy@x.pl", password: "password123", role: "instructor", firstName: null, lastName: null, phone: null });
    expect(result.current.users.some((user) => user.id === "u3")).toBe(true);
  });

  it("creates an account without a password and invites it right away", async () => {
    const { result } = renderHook(() => useUsersViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    act(() => result.current.setEmail("rodzic@x.pl"));
    await act(async () => result.current.addUser());

    // Puste hasło jest dozwolone: hasło ustawi sam użytkownik z zaproszenia. Konto bez
    // zaproszenia byłoby kontem, do którego nikt nie może się zalogować.
    expect(usersApi.createUser).toHaveBeenCalledWith(
      expect.objectContaining({ email: "rodzic@x.pl", password: null }),
    );
    expect(usersApi.inviteUser).toHaveBeenCalledWith("u3");
    expect(result.current.error).toBeNull();
  });

  it("toggles active state", async () => {
    const { result } = renderHook(() => useUsersViewModel());
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => result.current.toggleActive(users[1]));

    expect(usersApi.setUserActive).toHaveBeenCalledWith("u2", false);
    expect(result.current.users.find((user) => user.id === "u2")?.isActive).toBe(false);
  });
});

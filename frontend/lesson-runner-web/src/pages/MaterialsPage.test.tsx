import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { MaterialsPage } from "./MaterialsPage";
import * as materialsApi from "../api/materialsApi";
import { DialogProvider } from "../features/dialog/DialogContext";
import { ToastProvider } from "../features/toast/ToastContext";
import type { Material } from "../types/material";

let currentIsAdmin = false;

vi.mock("../api/materialsApi");
vi.mock("../features/auth/AuthContext", () => ({
  useAuth: () => ({ isAdmin: currentIsAdmin }),
}));

const sharedMaterial: Material = {
  id: "m1",
  title: "Instrukcja dla instruktorów",
  description: "Jak przygotować stanowisko przed zajęciami.",
  resourceUrl: "https://example.com/instrukcja",
  fileName: null,
  contentType: null,
  sizeBytes: null,
  visibility: "staff",
  createdAt: "2026-08-14T10:00:00Z",
  updatedAt: "2026-08-14T10:00:00Z",
};

const adminMaterial: Material = {
  ...sharedMaterial,
  id: "m2",
  title: "Procedura administracyjna",
  resourceUrl: "https://example.com/procedura",
  visibility: "admin",
};

function renderPage() {
  return render(
    <ToastProvider>
      <DialogProvider>
        <MaterialsPage />
      </DialogProvider>
    </ToastProvider>,
  );
}

describe("MaterialsPage", () => {
  beforeEach(() => {
    currentIsAdmin = false;
    vi.clearAllMocks();
    vi.mocked(materialsApi.getMaterials).mockResolvedValue([sharedMaterial]);
  });

  it("pokazuje instruktorowi udostępnione materiały bez kontrolek administracyjnych", async () => {
    renderPage();

    expect(await screen.findByText("Instrukcja dla instruktorów")).toBeInTheDocument();
    expect(screen.getByText("Administracja i instruktorzy")).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Nowy materiał" })).not.toBeInTheDocument();
    expect(screen.queryByRole("textbox", { name: "Tytuł" })).not.toBeInTheDocument();
  });

  it("pozwala administratorowi utworzyć materiał tylko dla administracji", async () => {
    currentIsAdmin = true;
    vi.mocked(materialsApi.getMaterials).mockResolvedValue([sharedMaterial, adminMaterial]);
    vi.mocked(materialsApi.createMaterial).mockResolvedValue(adminMaterial);
    renderPage();

    await screen.findByText("Procedura administracyjna");
    fireEvent.change(screen.getByRole("textbox", { name: "Tytuł" }), {
      target: { value: "Nowa procedura" },
    });
    fireEvent.change(screen.getByRole("textbox", { name: "Link" }), {
      target: { value: "https://example.com/nowa" },
    });
    fireEvent.change(screen.getAllByRole("combobox", { name: "Widoczność" })[1], {
      target: { value: "admin" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Dodaj materiał" }));

    await waitFor(() =>
      expect(materialsApi.createMaterial).toHaveBeenCalledWith({
        title: "Nowa procedura",
        description: "",
        resourceUrl: "https://example.com/nowa",
        fileName: null,
        contentType: null,
        sizeBytes: null,
        visibility: "admin",
      }),
    );
  });
});

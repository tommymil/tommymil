import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import { router } from "./app/router";
import { applyTheme, getStoredTheme } from "./features/theme/theme";
import "./styles/index.css";

// Ustawiamy motyw przed renderem, żeby uniknac migniecia jasnego ekranu.
applyTheme(getStoredTheme());

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
);

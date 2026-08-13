import { Outlet } from "react-router-dom";
import { AppShell } from "../components/layout/AppShell";
import { AuthProvider } from "../features/auth/AuthContext";
import { DialogProvider } from "../features/dialog/DialogContext";
import { ToastProvider } from "../features/toast/ToastContext";

export function App() {
  return (
    <ToastProvider>
      {/* DialogProvider stoi nad AppShell, bo z okien modalnych korzystają zarówno
          ekrany, jak i sam szkielet (potwierdzenie wylogowania z niezapisaną pracą). */}
      <DialogProvider>
        <AuthProvider>
          <AppShell>
            <Outlet />
          </AppShell>
        </AuthProvider>
      </DialogProvider>
    </ToastProvider>
  );
}

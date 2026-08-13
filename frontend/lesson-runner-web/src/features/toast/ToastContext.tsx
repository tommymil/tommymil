import { createContext, useCallback, useContext, useMemo, useRef, useState } from "react";
import type { PropsWithChildren } from "react";
import { AlertTriangle, CheckCircle2, Info, X } from "lucide-react";

export type ToastKind = "success" | "error" | "info";

type Toast = {
  id: number;
  kind: ToastKind;
  message: string;
};

type ToastContextValue = {
  notify: (kind: ToastKind, message: string) => void;
  success: (message: string) => void;
  error: (message: string) => void;
  info: (message: string) => void;
  dismiss: (id: number) => void;
};

const ToastContext = createContext<ToastContextValue | null>(null);

// Po tym czasie powiadomienie znika samo; użytkownik może też zamknąć je wcześniej.
const AUTO_DISMISS_MS = 4500;

export function ToastProvider({ children }: PropsWithChildren) {
  const [toasts, setToasts] = useState<Toast[]>([]);
  const nextId = useRef(1);
  const timers = useRef(new Map<number, number>());

  const dismiss = useCallback((id: number) => {
    setToasts((current) => current.filter((toast) => toast.id !== id));

    const timer = timers.current.get(id);
    if (timer !== undefined) {
      window.clearTimeout(timer);
      timers.current.delete(id);
    }
  }, []);

  const notify = useCallback(
    (kind: ToastKind, message: string) => {
      const id = nextId.current;
      nextId.current += 1;

      setToasts((current) => [...current, { id, kind, message }]);
      timers.current.set(id, window.setTimeout(() => dismiss(id), AUTO_DISMISS_MS));
    },
    [dismiss],
  );

  const value = useMemo<ToastContextValue>(
    () => ({
      notify,
      success: (message: string) => notify("success", message),
      error: (message: string) => notify("error", message),
      info: (message: string) => notify("info", message),
      dismiss,
    }),
    [notify, dismiss],
  );

  return (
    <ToastContext.Provider value={value}>
      {children}
      <ToastViewport toasts={toasts} onDismiss={dismiss} />
    </ToastContext.Provider>
  );
}

export function useToast(): ToastContextValue {
  const context = useContext(ToastContext);

  if (!context) {
    throw new Error("useToast must be used within a ToastProvider.");
  }

  return context;
}

const toastIcons = {
  success: CheckCircle2,
  error: AlertTriangle,
  info: Info,
} as const;

function ToastViewport({ toasts, onDismiss }: { toasts: Toast[]; onDismiss: (id: number) => void }) {
  if (toasts.length === 0) {
    return null;
  }

  return (
    <div className="toast-viewport" role="region" aria-label="Powiadomienia" aria-live="polite">
      {toasts.map((toast) => {
        const Icon = toastIcons[toast.kind];

        return (
          <div key={toast.id} className={`toast toast-${toast.kind}`} role="status">
            <Icon className="toast-icon" size={18} aria-hidden="true" />
            <span className="toast-message">{toast.message}</span>
            <button
              type="button"
              className="toast-close"
              onClick={() => onDismiss(toast.id)}
              aria-label="Zamknij powiadomienie"
            >
              <X size={16} aria-hidden="true" />
            </button>
          </div>
        );
      })}
    </div>
  );
}

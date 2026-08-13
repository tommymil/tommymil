import { createContext, useCallback, useContext, useMemo, useRef, useState } from "react";
import type { PropsWithChildren, ReactNode } from "react";
import { Dialog } from "../../components/ui/Dialog";
import { Button } from "../../components/ui/Button";

export type ConfirmRequest = {
  title: string;
  description?: ReactNode;
  /** Lista skutków decyzji pokazywana nad przyciskami. */
  consequences?: ReactNode[];
  confirmLabel?: string;
  cancelLabel?: string;
  tone?: "default" | "danger";
  /**
   * Gdy podane, potwierdzenie wymaga przepisania tego tekstu.
   * Dla operacji nieodwracalnych: odruch „OK” jest silniejszy niż czytanie.
   */
  typeToConfirm?: string;
};

export type PromptRequest = {
  title: string;
  description?: ReactNode;
  label: string;
  placeholder?: string;
  initialValue?: string;
  confirmLabel?: string;
  cancelLabel?: string;
  /**
   * `true` pozwala potwierdzić z pustą wartością. Domyślnie pole jest **wymagane**:
   * prawie każde pytanie w tej aplikacji to pytanie o powód operacji, a powód, którego
   * da się nie podać, w praktyce nie jest podawany.
   */
  optional?: boolean;
  multiline?: boolean;
  maxLength?: number;
};

type DialogContextValue = {
  confirm: (request: ConfirmRequest) => Promise<boolean>;
  prompt: (request: PromptRequest) => Promise<string | null>;
};

const DialogContext = createContext<DialogContextValue | null>(null);

type PendingConfirm = { kind: "confirm"; request: ConfirmRequest };
type PendingPrompt = { kind: "prompt"; request: PromptRequest };
type Pending = PendingConfirm | PendingPrompt;

/**
 * Zastępuje `window.confirm` i `window.prompt`.
 *
 * Natywne okna były używane w siedmiu miejscach, w tym w portalu rodzica (powód
 * nieobecności) i przy anonimizacji danych dziecka. Nie da się ich ostylować, nie
 * zachowują się przewidywalnie na urządzeniach mobilnych, a przede wszystkim nie
 * potrafią pokazać, **co się stanie po potwierdzeniu** — a to jest cała wartość
 * potwierdzenia przy operacji nieodwracalnej.
 *
 * Świadomie nie ma odpowiednika `window.alert`: komunikat o wyniku operacji należy
 * do warstwy powiadomień (`useToast`), a nie do okna blokującego pracę.
 */
export function DialogProvider({ children }: PropsWithChildren) {
  const [pending, setPending] = useState<Pending | null>(null);
  const [value, setValue] = useState("");
  const [typed, setTyped] = useState("");
  const resolver = useRef<((result: never) => void) | null>(null);

  const settle = useCallback((result: boolean | string | null) => {
    const resolve = resolver.current;
    resolver.current = null;
    setPending(null);
    setValue("");
    setTyped("");
    resolve?.(result as never);
  }, []);

  const confirm = useCallback((request: ConfirmRequest) => {
    return new Promise<boolean>((resolve) => {
      resolver.current = resolve as (result: never) => void;
      setTyped("");
      setPending({ kind: "confirm", request });
    });
  }, []);

  const prompt = useCallback((request: PromptRequest) => {
    return new Promise<string | null>((resolve) => {
      resolver.current = resolve as (result: never) => void;
      setValue(request.initialValue ?? "");
      setPending({ kind: "prompt", request });
    });
  }, []);

  const contextValue = useMemo<DialogContextValue>(() => ({ confirm, prompt }), [confirm, prompt]);

  return (
    <DialogContext.Provider value={contextValue}>
      {children}

      {pending?.kind === "confirm" ? (
        <ConfirmDialog
          request={pending.request}
          typed={typed}
          onTypedChange={setTyped}
          onCancel={() => settle(false)}
          onConfirm={() => settle(true)}
        />
      ) : null}

      {pending?.kind === "prompt" ? (
        <PromptDialog
          request={pending.request}
          value={value}
          onValueChange={setValue}
          onCancel={() => settle(null)}
          onConfirm={() => settle(value.trim())}
        />
      ) : null}
    </DialogContext.Provider>
  );
}

export function useDialogs(): DialogContextValue {
  const context = useContext(DialogContext);

  if (!context) {
    throw new Error("useDialogs musi być użyte wewnątrz DialogProvider.");
  }

  return context;
}

function ConfirmDialog({
  request,
  typed,
  onTypedChange,
  onCancel,
  onConfirm,
}: {
  request: ConfirmRequest;
  typed: string;
  onTypedChange: (next: string) => void;
  onCancel: () => void;
  onConfirm: () => void;
}) {
  const needsTyping = Boolean(request.typeToConfirm);
  const canConfirm = !needsTyping || typed.trim() === request.typeToConfirm;

  return (
    <Dialog
      title={request.title}
      description={request.description}
      tone={request.tone}
      dismissOnScrim={!needsTyping}
      onClose={onCancel}
      actions={
        <>
          <Button variant="secondary" onClick={onCancel}>
            {request.cancelLabel ?? "Anuluj"}
          </Button>
          <Button
            variant={request.tone === "danger" ? "danger" : "primary"}
            onClick={onConfirm}
            disabled={!canConfirm}
          >
            {request.confirmLabel ?? "Potwierdź"}
          </Button>
        </>
      }
    >
      {request.consequences && request.consequences.length > 0 ? (
        <div className="dialog-consequences">
          <ul>
            {request.consequences.map((item, index) => (
              <li key={index}>{item}</li>
            ))}
          </ul>
        </div>
      ) : null}

      {needsTyping ? (
        <label className="form-field dialog-typed-confirm">
          <span>
            Aby potwierdzić, przepisz: <code>{request.typeToConfirm}</code>
          </span>
          <input
            value={typed}
            onChange={(event) => onTypedChange(event.target.value)}
            autoComplete="off"
            spellCheck={false}
          />
        </label>
      ) : null}
    </Dialog>
  );
}

function PromptDialog({
  request,
  value,
  onValueChange,
  onCancel,
  onConfirm,
}: {
  request: PromptRequest;
  value: string;
  onValueChange: (next: string) => void;
  onCancel: () => void;
  onConfirm: () => void;
}) {
  const canConfirm = request.optional === true || value.trim().length > 0;

  return (
    <Dialog
      title={request.title}
      description={request.description}
      onClose={onCancel}
      dismissOnScrim={false}
      actions={
        <>
          <Button variant="secondary" onClick={onCancel}>
            {request.cancelLabel ?? "Anuluj"}
          </Button>
          <Button onClick={onConfirm} disabled={!canConfirm}>
            {request.confirmLabel ?? "Zapisz"}
          </Button>
        </>
      }
    >
      <label className="form-field">
        <span>
          {request.label}
          {request.optional ? " (opcjonalnie)" : ""}
        </span>
        {request.multiline ? (
          <textarea
            rows={4}
            value={value}
            maxLength={request.maxLength ?? 500}
            placeholder={request.placeholder}
            onChange={(event) => onValueChange(event.target.value)}
          />
        ) : (
          <input
            value={value}
            maxLength={request.maxLength ?? 200}
            placeholder={request.placeholder}
            onChange={(event) => onValueChange(event.target.value)}
            onKeyDown={(event) => {
              if (event.key === "Enter" && canConfirm) {
                event.preventDefault();
                onConfirm();
              }
            }}
          />
        )}
      </label>
    </Dialog>
  );
}

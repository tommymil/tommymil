import type { ButtonHTMLAttributes, PropsWithChildren } from "react";

// "danger" dla operacji nieodwracalnych (odwołanie zajęć, usunięcie grupy,
// anonimizacja danych dziecka). Wcześniej wyglądały tak samo jak zapis formularza.
type ButtonVariant = "primary" | "secondary" | "ghost" | "danger";

type ButtonProps = PropsWithChildren<
  ButtonHTMLAttributes<HTMLButtonElement> & {
    variant?: ButtonVariant;
  }
>;

export function Button({ children, className = "", variant = "primary", ...props }: ButtonProps) {
  return (
    <button className={`button button-${variant} ${className}`.trim()} {...props}>
      {children}
    </button>
  );
}

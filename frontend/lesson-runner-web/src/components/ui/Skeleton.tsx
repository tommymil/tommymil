/**
 * Zastępuje tekst „Ładowanie...”.
 *
 * Podmiana jednego zdania na pełną treść powodowała przeskok układu przy każdym
 * wejściu na ekran. Szkielet ma wymiary treści docelowej, więc strona nie skacze,
 * a użytkownik od razu widzi, ile mniej więcej danych się pojawi.
 *
 * `aria-hidden` jest celowe: dla czytnika ekranu istotny jest komunikat
 * „Ładowanie” w regionie `aria-live`, a nie kilkanaście pustych prostokątów.
 */
export function Skeleton({ width = "100%", height = 14 }: { width?: string | number; height?: string | number }) {
  return <span className="skeleton" style={{ width, height }} aria-hidden="true" />;
}

type SkeletonListProps = {
  /** Liczba wierszy szkieletu. Powinna odpowiadać typowej długości listy. */
  rows?: number;
  label?: string;
};

export function SkeletonList({ rows = 4, label = "Ładowanie danych" }: SkeletonListProps) {
  return (
    <div className="skeleton-stack" role="status" aria-live="polite" aria-busy="true">
      <span className="visually-hidden">{label}</span>
      {Array.from({ length: rows }, (_, index) => (
        <div className="skeleton-card" key={index}>
          <Skeleton width="38%" height={16} />
          <Skeleton width="72%" />
          <Skeleton width="54%" />
        </div>
      ))}
    </div>
  );
}

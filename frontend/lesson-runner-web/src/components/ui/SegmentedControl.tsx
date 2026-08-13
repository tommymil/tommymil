type Segment<TValue extends string> = {
  value: TValue;
  label: string;
  /** Klasa tonu (`seg-present`, `seg-late`, `seg-absent`) — kolor stanu wybranego. */
  tone?: string;
  title?: string;
};

type SegmentedControlProps<TValue extends string> = {
  value: TValue | null;
  segments: Segment<TValue>[];
  onChange: (value: TValue) => void;
  /** Etykieta dla czytnika ekranu — przy liście dzieci musi zawierać imię. */
  ariaLabel: string;
  disabled?: boolean;
};

/**
 * Przełącznik z kilkoma stanami widocznymi naraz.
 *
 * Powstał na potrzeby listy obecności: dziewięć statusów w liście rozwijanej przy
 * każdym dziecku oznaczało, że instruktor w trakcie zajęć klikał dwa razy i czytał
 * dziewięć pozycji, żeby zaznaczyć „obecny”. Trzy najczęstsze stany są tutaj widoczne
 * od razu, rzadsze zostają pod osobnym przyciskiem obok.
 *
 * Świadomie `aria-pressed` na przyciskach, a nie `role="radiogroup"`: to jest
 * ustawianie stanu, a nie wybór opcji w formularzu, i nie chcemy, żeby strzałki
 * przejmowały nawigację w gęstej liście.
 */
export function SegmentedControl<TValue extends string>({
  value,
  segments,
  onChange,
  ariaLabel,
  disabled = false,
}: SegmentedControlProps<TValue>) {
  return (
    <div className="segmented" role="group" aria-label={ariaLabel}>
      {segments.map((segment) => (
        <button
          key={segment.value}
          type="button"
          className={segment.tone}
          aria-pressed={value === segment.value}
          disabled={disabled}
          title={segment.title}
          onClick={() => onChange(segment.value)}
        >
          {segment.label}
        </button>
      ))}
    </div>
  );
}

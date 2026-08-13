import { useState } from "react";
import { CalendarClock, CreditCard, Mail, MoveRight, Users } from "lucide-react";
import { Button } from "../../components/ui/Button";
import { Dialog } from "../../components/ui/Dialog";
import { formatFriendlyDateTime, plural } from "./datetime";
import type { CancelSessionRequest, ScheduledSession } from "../../types/group";

type CancelSessionDialogProps = {
  session: ScheduledSession;
  /** Ile dzieci jest aktywnie zapisanych do grupy — do podsumowania skutków. */
  enrolledCount: number;
  /** Ile terminów zostało po tym, na potrzeby opisu przesunięcia materiału. */
  followingCount: number;
  busy: boolean;
  onClose: () => void;
  onConfirm: (request: CancelSessionRequest) => Promise<void>;
};

const compensations = [
  { value: "none", label: "Bez rekompensaty" },
  { value: "credit", label: "Kredyt zajęciowy dla każdego dziecka" },
  { value: "makeup", label: "Odrobienie w innym terminie" },
  { value: "refund", label: "Zwrot pieniędzy" },
] as const;

/**
 * Kreator odwołania zajęć.
 *
 * Rozdział 10 dokumentu koncepcyjnego opisuje to jako operację „jednym ruchem”: odwołaj
 * + powód + powiadom rodziców + rekompensata + przesuń numerację lekcji. Backend to
 * potrafił od lipca, ale interfejs rozbijał tę decyzję na cztery kontrolki wciśnięte
 * w wiersz listy terminów — pole powodu, listę rozwijaną rekompensaty i dwa checkboxy —
 * i nie mówił, **co się właściwie stanie** po kliknięciu.
 *
 * Tutaj administrator widzi skutki przed potwierdzeniem, a nie po.
 *
 * Świadomie automatyzujemy tylko kredyt. Zwrot i odrobienie wymagają rozmowy z rodzicem
 * i wpisuje się je ręcznie — udawanie automatyzacji byłoby tu gorsze niż jej brak;
 * ta sama zasada obowiązuje w backendzie.
 */
export function CancelSessionDialog({
  session,
  enrolledCount,
  followingCount,
  busy,
  onClose,
  onConfirm,
}: CancelSessionDialogProps) {
  const [reason, setReason] = useState("");
  const [cancelledBy, setCancelledBy] = useState("instructor");
  const [compensation, setCompensation] = useState<string>("credit");
  const [shiftFollowing, setShiftFollowing] = useState(false);
  const [notify, setNotify] = useState(false);

  const consequences = [
    {
      icon: CalendarClock,
      text: `Termin ${formatFriendlyDateTime(session.scheduledAt)} zmieni status na „odwołany” i przestanie liczyć się do frekwencji.`,
    },
    {
      icon: Users,
      text:
        enrolledCount === 0
          ? "Do tej grupy nie jest zapisane żadne dziecko."
          : `Dotyczy ${enrolledCount} ${plural(enrolledCount, "dziecka", "dzieci", "dzieci")} zapisanych do grupy.`,
    },
    {
      icon: CreditCard,
      text:
        compensation === "credit"
          ? `Każde zapisane dziecko dostanie 1 kredyt zajęciowy z powodem odwołania (${enrolledCount} ${plural(enrolledCount, "kredyt", "kredyty", "kredytów")}).`
          : compensation === "none"
            ? "Nie przyznajemy rekompensaty."
            : "Rekompensatę trzeba będzie wpisać ręcznie po ustaleniach z rodzicem — system jej nie utworzy.",
    },
    {
      icon: Mail,
      // Wysyłka jest automatyczna i bezwarunkowa — nie zależy od checkboxa ani od
      // przełącznika przypomnień w ustawieniach. „Nie przypominaj mi co tydzień” to nie
      // to samo, co „nie mów mi, że zajęcia się nie odbędą”.
      text:
        enrolledCount === 0
          ? "Nie ma komu wysłać wiadomości — do grupy nie jest zapisane żadne dziecko."
          : `Opiekunowie ${enrolledCount} ${plural(enrolledCount, "dziecka", "dzieci", "dzieci")} dostaną e-mail o odwołaniu — od razu po potwierdzeniu. Każdy adres dostanie go raz.`,
    },
    {
      icon: MoveRight,
      text: shiftFollowing
        ? `Materiał przesunie się na kolejne terminy (${followingCount} ${plural(followingCount, "termin", "terminy", "terminów")} dalej), a kurs wydłuży się o jedne zajęcia. Daty istniejących terminów zostaną nietknięte.`
        : "Materiał z tych zajęć przepada — kolejne lekcje zostają na swoich terminach.",
    },
  ];

  return (
    <Dialog
      title="Odwołać te zajęcia?"
      description={`${session.groupName} · lekcja ${session.sequenceNumber}${session.lessonTitle ? ` · ${session.lessonTitle}` : ""}`}
      tone="danger"
      wide
      dismissOnScrim={false}
      onClose={onClose}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            Anuluj
          </Button>
          <Button
            variant="danger"
            disabled={busy || reason.trim().length === 0}
            onClick={() =>
              void onConfirm({
                reason: reason.trim(),
                guardiansNotified: notify,
                cancelledBy,
                compensation,
                shiftFollowingLessons: shiftFollowing,
              })
            }
          >
            {busy ? "Odwoływanie..." : "Odwołaj zajęcia"}
          </Button>
        </>
      }
    >
      <div className="dialog-consequences">
        <ul>
          {consequences.map((item, index) => {
            const Icon = item.icon;
            return (
              <li key={index}>
                <Icon size={16} aria-hidden="true" />
                <span>{item.text}</span>
              </li>
            );
          })}
        </ul>
      </div>

      {/* Powód jest wymagany. Odwołanie bez uzasadnienia jest nie do obronienia
          ani w rozmowie z rodzicem, ani przy reklamacji — a to właśnie wtedy
          historia zmian terminów jest najbardziej potrzebna. */}
      <label className="form-field">
        <span>Powód odwołania (wymagany)</span>
        <input
          value={reason}
          maxLength={500}
          placeholder="np. choroba instruktora, awaria platformy"
          onChange={(event) => setReason(event.target.value)}
        />
      </label>

      <label className="form-field">
        <span>Kto odwołuje</span>
        <select value={cancelledBy} onChange={(event) => setCancelledBy(event.target.value)}>
          <option value="instructor">Organizator / instruktor</option>
          <option value="parent">Rodzic</option>
        </select>
      </label>

      <label className="form-field">
        <span>Rozliczenie</span>
        <select value={compensation} onChange={(event) => setCompensation(event.target.value)}>
          {compensations.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </label>

      {/* Checkbox przestał znaczyć „czy powiadomić” — system powiadamia sam, a historia
          zmian zapisuje wynik tej wysyłki. Zostaje jako ślad kanału zapasowego: telefonu
          albo SMS-a, o którym system nie ma skąd wiedzieć. Domyślnie odznaczony, bo
          domyślnie nikt nie dzwonił. */}
      <label className="checkbox-field">
        <input type="checkbox" checked={notify} onChange={() => setNotify((current) => !current)} />
        <span>Dodatkowo poinformowano opiekunów telefonicznie lub SMS-em</span>
      </label>

      <label className="checkbox-field">
        <input
          type="checkbox"
          checked={shiftFollowing}
          onChange={() => setShiftFollowing((current) => !current)}
        />
        <span>Przesuń materiał na kolejne terminy i wydłuż kurs o jedne zajęcia</span>
      </label>
    </Dialog>
  );
}

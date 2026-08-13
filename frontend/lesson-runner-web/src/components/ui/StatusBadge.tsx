/**
 * Jeden znacznik statusu dla całej aplikacji.
 *
 * Kolory przychodzą z tokenów, a nie z reguł pisanych osobno przy kalendarzu,
 * liście terminów, kokpicie i portalu rodzica. Dzięki temu „przerwane technicznie”
 * wygląda wszędzie tak samo — a przy dziesięciu statusach terminu i dziewięciu
 * statusach obecności rozjazd kolorów był kwestią czasu.
 *
 * Etykiety **nie są tutaj zapisane**: przychodzą z backendu (`statusLabel`),
 * bo domena jest jedynym właścicielem słownika statusów.
 */

type BadgeTone =
  | "planned"
  | "confirmed"
  | "inprogress"
  | "completed"
  | "cancelled"
  | "technical"
  | "pending"
  | "success"
  | "warning"
  | "danger"
  | "info"
  | "credit"
  | "neutral";

const sessionTones: Record<string, BadgeTone> = {
  planned: "planned",
  confirmed: "confirmed",
  inprogress: "inprogress",
  completed: "completed",
  cancelled: "cancelled",
  cancelledbyinstructor: "cancelled",
  cancelledbyparent: "cancelled",
  technicalfailure: "technical",
  notdelivered: "pending",
  awaitingreschedule: "pending",
};

/** Mapowanie statusu obecności na ton. Statusy pochodzą z `AttendanceStatus` w domenie. */
const attendanceTones: Record<string, BadgeTone> = {
  present: "success",
  late: "warning",
  leftearly: "warning",
  partial: "warning",
  presentinactive: "warning",
  technicalissues: "technical",
  absentreported: "neutral",
  absentunreported: "danger",
  makeupelsewhere: "info",
};

export function sessionStatusTone(status: string): BadgeTone {
  return sessionTones[status.toLowerCase()] ?? "neutral";
}

export function attendanceStatusTone(status: string): BadgeTone {
  return attendanceTones[status.toLowerCase()] ?? "neutral";
}

type StatusBadgeProps = {
  label: string;
  tone?: BadgeTone;
  /** Kropka przed etykietą — czytelniejsza przy gęstych listach terminów. */
  dot?: boolean;
  className?: string;
};

export function StatusBadge({ label, tone = "neutral", dot = false, className = "" }: StatusBadgeProps) {
  return (
    <span className={`status-badge badge-${tone}${dot ? " status-badge-dot" : ""} ${className}`.trim()}>
      {label}
    </span>
  );
}

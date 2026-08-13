/**
 * Lekcja próbna 1:1 — dziecko, które jeszcze nie jest uczestnikiem.
 *
 * Kandydat świadomie nie jest `Participant`: dopóki rodzina nie zdecyduje, dziecko nie ma
 * po co pojawiać się w bazie uczestników, na listach frekwencji ani w rozliczeniach.
 */
export type TrialLesson = {
  id: string;
  childFirstName: string;
  childLastName: string;
  childBirthDate: string | null;
  childAge: number | null;
  guardianName: string | null;
  guardianEmail: string | null;
  guardianPhone: string | null;
  source: string | null;
  requestNote: string | null;
  instructorId: string | null;
  instructorName: string | null;
  scheduledAt: string | null;
  meetingUrl: string | null;
  lessonId: string | null;
  lessonTitle: string | null;
  reading: string;
  readingLabel: string;
  computer: string;
  computerLabel: string;
  programming: string;
  programmingLabel: string;
  recommendation: string;
  recommendationLabel: string;
  recommendedLevel: string | null;
  diagnosisNote: string | null;
  diagnosedAt: string | null;
  status: string;
  statusLabel: string;
  participantId: string | null;
  declineReason: string | null;
  createdAt: string;
  closedAt: string | null;
  hasDiagnosis: boolean;
  /** Czeka na ruch: świeże zgłoszenie bez terminu albo dziecko po lekcji. */
  needsAttention: boolean;
};

export type TrialOption = { value: string; label: string };

export type TrialBoard = {
  trials: TrialLesson[];
  readingOptions: TrialOption[];
  computerOptions: TrialOption[];
  programmingOptions: TrialOption[];
  recommendationOptions: TrialOption[];
  statusOptions: TrialOption[];
};

export type CreateTrialRequest = {
  childFirstName: string;
  childLastName: string;
  childBirthDate?: string | null;
  guardianName?: string | null;
  guardianEmail?: string | null;
  guardianPhone?: string | null;
  source?: string | null;
  requestNote?: string | null;
};

export type ScheduleTrialRequest = {
  instructorId: string | null;
  scheduledAt: string | null;
  meetingUrl?: string | null;
  lessonId?: string | null;
};

export type SaveTrialDiagnosisRequest = {
  reading: string;
  computer: string;
  programming: string;
  recommendation: string;
  recommendedLevel?: string | null;
  diagnosisNote?: string | null;
};

export type EnrollTrialRequest = { createGuardianAccount: boolean };

export type DeclineTrialRequest = { reason?: string | null; noShow?: boolean };

export type TrialEnrollmentResult = {
  participantId: string;
  participantName: string;
  guardianAccountCreated: boolean;
  invitationSent: boolean;
  guardianEmail: string | null;
  error: string | null;
};

/** Pusty formularz zgłoszenia — pola tekstowe, żeby dało się je wiązać z inputami. */
export type TrialDraft = {
  childFirstName: string;
  childLastName: string;
  childBirthDate: string;
  guardianName: string;
  guardianEmail: string;
  guardianPhone: string;
  source: string;
  requestNote: string;
};

export function emptyTrialDraft(): TrialDraft {
  return {
    childFirstName: "",
    childLastName: "",
    childBirthDate: "",
    guardianName: "",
    guardianEmail: "",
    guardianPhone: "",
    source: "",
    requestNote: "",
  };
}

export function draftToTrialRequest(draft: TrialDraft): CreateTrialRequest {
  return {
    childFirstName: draft.childFirstName.trim(),
    childLastName: draft.childLastName.trim(),
    childBirthDate: draft.childBirthDate.trim() || null,
    guardianName: draft.guardianName.trim() || null,
    guardianEmail: draft.guardianEmail.trim() || null,
    guardianPhone: draft.guardianPhone.trim() || null,
    source: draft.source.trim() || null,
    requestNote: draft.requestNote.trim() || null,
  };
}

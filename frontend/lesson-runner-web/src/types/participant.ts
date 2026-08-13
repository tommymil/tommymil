export type ParticipantGroupRef = {
  groupId: string;
  groupName: string;
};

export type ParticipantSummary = {
  id: string;
  firstName: string;
  lastName: string;
  phone: string | null;
  email: string | null;
  birthDate: string | null;
  guardianName: string | null;
  guardianPhone: string | null;
  guardianEmail: string | null;
  isArchived: boolean;
  hasDataConsent: boolean;
  hasImageConsent: boolean;
  groups: ParticipantGroupRef[];
  /** Czy opiekun ma już konto powiązane z tym dzieckiem. */
  hasGuardianAccount: boolean;
};

export type ParticipantDetails = ParticipantSummary & {
  notes: string | null;
  guardianRelation: string | null;
  archivedAt: string | null;
  dataProcessingConsentAt: string | null;
  imageConsentAt: string | null;
  createdAt: string;
};

/** Dane kontaktowe, opiekuńcze i zgody wspólne dla tworzenia i edycji. */
export type ParticipantContact = {
  phone: string | null;
  email: string | null;
  birthDate: string | null;
  notes: string | null;
  guardianName: string | null;
  guardianPhone: string | null;
  guardianEmail: string | null;
  guardianRelation: string | null;
  consentDataProcessing: boolean;
  consentImage: boolean;
};

export type CreateParticipantRequest = ParticipantContact & {
  firstName: string;
  lastName: string;
  groupIds?: string[];
};

export type UpdateParticipantRequest = ParticipantContact & {
  firstName: string;
  lastName: string;
};

export type ParticipantDraft = {
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  birthDate: string;
  notes: string;
  guardianName: string;
  guardianPhone: string;
  guardianEmail: string;
  guardianRelation: string;
  consentDataProcessing: boolean;
  consentImage: boolean;
};

export function emptyParticipantDraft(): ParticipantDraft {
  return {
    firstName: "",
    lastName: "",
    phone: "",
    email: "",
    birthDate: "",
    notes: "",
    guardianName: "",
    guardianPhone: "",
    guardianEmail: "",
    guardianRelation: "",
    consentDataProcessing: false,
    consentImage: false,
  };
}

export function draftFromParticipant(participant: ParticipantSummary | ParticipantDetails): ParticipantDraft {
  const details = participant as Partial<ParticipantDetails>;
  return {
    firstName: participant.firstName,
    lastName: participant.lastName,
    phone: participant.phone ?? "",
    email: participant.email ?? "",
    birthDate: participant.birthDate ?? "",
    notes: details.notes ?? "",
    guardianName: participant.guardianName ?? "",
    guardianPhone: participant.guardianPhone ?? "",
    guardianEmail: details.guardianEmail ?? "",
    guardianRelation: details.guardianRelation ?? "",
    consentDataProcessing: participant.hasDataConsent,
    consentImage: participant.hasImageConsent,
  };
}

function contactFromDraft(draft: ParticipantDraft): ParticipantContact {
  return {
    phone: draft.phone.trim() || null,
    email: draft.email.trim() || null,
    birthDate: draft.birthDate.trim() || null,
    notes: draft.notes.trim() || null,
    guardianName: draft.guardianName.trim() || null,
    guardianPhone: draft.guardianPhone.trim() || null,
    guardianEmail: draft.guardianEmail.trim() || null,
    guardianRelation: draft.guardianRelation.trim() || null,
    consentDataProcessing: draft.consentDataProcessing,
    consentImage: draft.consentImage,
  };
}

export function draftToRequest(draft: ParticipantDraft, groupIds?: string[]): CreateParticipantRequest {
  return {
    firstName: draft.firstName.trim(),
    lastName: draft.lastName.trim(),
    ...contactFromDraft(draft),
    groupIds,
  };
}

export function draftToUpdateRequest(draft: ParticipantDraft): UpdateParticipantRequest {
  return {
    firstName: draft.firstName.trim(),
    lastName: draft.lastName.trim(),
    ...contactFromDraft(draft),
  };
}

/** Wiek w pełnych latach z daty urodzenia (ISO yyyy-MM-dd), albo null. */
export function computeAge(birthDate: string | null | undefined, today: Date = new Date()): number | null {
  if (!birthDate) {
    return null;
  }

  const born = new Date(birthDate);
  if (Number.isNaN(born.getTime())) {
    return null;
  }

  let age = today.getFullYear() - born.getFullYear();
  const monthDiff = today.getMonth() - born.getMonth();
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < born.getDate())) {
    age -= 1;
  }

  return age < 0 || age > 120 ? null : age;
}

/**
 * Wynik założenia konta opiekunowi z karty dziecka.
 *
 * `created` odróżnia nowe konto od podpięcia istniejącego (drugie dziecko tej samej
 * rodziny), a `invitationSent` mówi, czy poszła poczta - konto i powiązanie powstają
 * także wtedy, gdy wysyłka padnie.
 */
export type GuardianAccountResult = {
  parentUserId: string;
  email: string;
  displayName: string;
  created: boolean;
  invitationSent: boolean;
  error?: string | null;
};

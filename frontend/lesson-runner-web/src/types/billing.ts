export type PricePlan = {
  id: string;
  name: string;
  courseId: string | null;
  courseName: string | null;
  groupId: string | null;
  groupName: string | null;
  amountCents: number;
  currency: string;
  isActive: boolean;
};

export type UpsertPricePlanRequest = {
  name: string;
  courseId?: string | null;
  groupId?: string | null;
  amountCents: number;
  currency?: string | null;
  isActive: boolean;
};

export type BillingEnrollment = {
  id: string;
  participantId: string;
  participantName: string;
  groupId: string;
  groupName: string;
  courseId: string | null;
  courseName: string | null;
  pricePlanId: string | null;
  status: "trial" | "active" | "cancelled";
  statusLabel: string;
  trialEndsAt: string | null;
  currentPriceCents: number | null;
  currency: string | null;
};

export type CreateBillingEnrollmentRequest = {
  participantId: string;
  groupId: string;
  pricePlanId?: string | null;
  trial: boolean;
  trialEndsAt?: string | null;
};

export type Invoice = {
  id: string;
  billingEnrollmentId: string;
  participantId: string;
  participantName: string;
  groupId: string;
  groupName: string;
  number: string;
  amountCents: number;
  currency: string;
  status: "draft" | "open" | "paid" | "overdue" | "cancelled";
  statusLabel: string;
  dueDate: string;
  issuedAt: string;
  paidAt: string | null;
};

export type CreateInvoiceRequest = {
  billingEnrollmentId: string;
  dueDate?: string | null;
  amountCents?: number | null;
};

export type Payment = {
  id: string;
  invoiceId: string;
  provider: string;
  externalId: string | null;
  amountCents: number;
  currency: string;
  status: "pending" | "succeeded" | "failed";
  statusLabel: string;
  paidAt: string | null;
  createdAt: string;
};

export type RecordManualPaymentRequest = {
  amountCents?: number | null;
  paidAt?: string | null;
  externalId?: string | null;
};

export type BillingKpis = {
  openAmountCents: number;
  overdueAmountCents: number;
  paidAmountCents: number;
  trialEnrollments: number;
  activeEnrollments: number;
  /** Kredyty czekające na wykorzystanie - zobowiązanie wobec rodziców. */
  availableCredits: number;
};

export type BillingOverview = {
  pricePlans: PricePlan[];
  enrollments: BillingEnrollment[];
  invoices: Invoice[];
  payments: Payment[];
  kpis: BillingKpis;
  credits: LessonCredit[];
};

/** Kredyt zajęciowy: „należą się jedne zajęcia”. */
export type LessonCredit = {
  id: string;
  participantId: string;
  participantName: string;
  groupId: string | null;
  groupName: string | null;
  sourceSessionId: string | null;
  reason: string;
  amountCents: number | null;
  currency: string | null;
  status: string;
  statusLabel: string;
  issuedAt: string;
  issuedByUserId: string | null;
  expiresAt: string | null;
  usage: string;
  usageLabel: string;
  usedAt: string | null;
  usedForSessionId: string | null;
  usedForInvoiceId: string | null;
  usageNote: string | null;
};

export type IssueLessonCreditRequest = {
  participantId: string;
  reason: string;
  groupId?: string | null;
  sourceSessionId?: string | null;
  amountCents?: number | null;
  currency?: string | null;
  expiresAt?: string | null;
};

export type UseLessonCreditRequest = {
  usage: string;
  usedForSessionId?: string | null;
  usedForInvoiceId?: string | null;
  usageNote?: string | null;
};

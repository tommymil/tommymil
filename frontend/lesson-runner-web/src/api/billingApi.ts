import { apiDelete, apiGet, apiPost, apiPostEmpty, apiPut } from "./client";
import type {
  BillingEnrollment,
  BillingOverview,
  CreateBillingEnrollmentRequest,
  CreateInvoiceRequest,
  IssueLessonCreditRequest,
  LessonCredit,
  Invoice,
  PricePlan,
  RecordManualPaymentRequest,
  UpsertPricePlanRequest,
  UseLessonCreditRequest,
} from "../types/billing";

export function getBillingOverview(): Promise<BillingOverview> {
  return apiGet<BillingOverview>("/api/billing");
}

export function createPricePlan(request: UpsertPricePlanRequest): Promise<PricePlan> {
  return apiPost<UpsertPricePlanRequest, PricePlan>("/api/billing/price-plans", request);
}

export function updatePricePlan(id: string, request: UpsertPricePlanRequest): Promise<PricePlan> {
  return apiPut<UpsertPricePlanRequest, PricePlan>(`/api/billing/price-plans/${id}`, request);
}

export function deletePricePlan(id: string): Promise<void> {
  return apiDelete(`/api/billing/price-plans/${id}`);
}

export function createBillingEnrollment(request: CreateBillingEnrollmentRequest): Promise<BillingEnrollment> {
  return apiPost<CreateBillingEnrollmentRequest, BillingEnrollment>("/api/billing/enrollments", request);
}

export function createInvoice(request: CreateInvoiceRequest): Promise<Invoice> {
  return apiPost<CreateInvoiceRequest, Invoice>("/api/billing/invoices", request);
}

export function payInvoice(id: string, request: RecordManualPaymentRequest): Promise<Invoice> {
  return apiPost<RecordManualPaymentRequest, Invoice>(`/api/billing/invoices/${id}/pay`, request);
}

export function cancelInvoice(id: string): Promise<Invoice> {
  return apiPostEmpty<Invoice>(`/api/billing/invoices/${id}/cancel`);
}

// --- Kredyty zajęciowe ---

export function getLessonCredits(participantId?: string): Promise<LessonCredit[]> {
  const query = participantId ? `?participantId=${participantId}` : "";
  return apiGet<LessonCredit[]>(`/api/billing/credits${query}`);
}

export function issueLessonCredit(request: IssueLessonCreditRequest): Promise<LessonCredit> {
  return apiPost<IssueLessonCreditRequest, LessonCredit>("/api/billing/credits", request);
}

export function useLessonCredit(id: string, request: UseLessonCreditRequest): Promise<LessonCredit> {
  return apiPost<UseLessonCreditRequest, LessonCredit>(`/api/billing/credits/${id}/use`, request);
}

export function revokeLessonCredit(id: string, reason: string | null): Promise<LessonCredit> {
  return apiPost<{ reason: string | null }, LessonCredit>(`/api/billing/credits/${id}/revoke`, { reason });
}

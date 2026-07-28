namespace LessonRunner.Application.Billing;

public interface IBillingService
{
    Task<BillingOverviewDto> GetOverviewAsync(CancellationToken cancellationToken);
    Task<PricePlanDto> CreatePricePlanAsync(UpsertPricePlanDto dto, CancellationToken cancellationToken);
    Task<PricePlanDto?> UpdatePricePlanAsync(Guid id, UpsertPricePlanDto dto, CancellationToken cancellationToken);
    Task<bool> DeletePricePlanAsync(Guid id, CancellationToken cancellationToken);
    Task<BillingEnrollmentDto> CreateEnrollmentAsync(CreateBillingEnrollmentDto dto, CancellationToken cancellationToken);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken);
    Task<InvoiceDto?> MarkInvoicePaidAsync(Guid invoiceId, RecordManualPaymentDto dto, CancellationToken cancellationToken);
    Task<InvoiceDto?> CancelInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken);

    // --- Kredyty zajęciowe ---

    Task<IReadOnlyList<LessonCreditDto>> ListCreditsAsync(Guid? participantId, CancellationToken cancellationToken);
    Task<LessonCreditDto> IssueCreditAsync(IssueLessonCreditDto dto, Guid? actingUserId, CancellationToken cancellationToken);
    Task<LessonCreditDto?> UseCreditAsync(Guid creditId, UseLessonCreditDto dto, Guid? actingUserId, CancellationToken cancellationToken);
    Task<LessonCreditDto?> RevokeCreditAsync(Guid creditId, string? reason, Guid? actingUserId, CancellationToken cancellationToken);
}

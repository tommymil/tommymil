using LessonRunner.Domain.Billing;

namespace LessonRunner.Application.Billing;

public interface IBillingRepository
{
    Task<IReadOnlyList<PricePlan>> ListPricePlansAsync(CancellationToken cancellationToken);
    Task<PricePlan?> GetPricePlanAsync(Guid id, CancellationToken cancellationToken);
    Task AddPricePlanAsync(PricePlan pricePlan, CancellationToken cancellationToken);
    Task<bool> UpdatePricePlanAsync(PricePlan pricePlan, CancellationToken cancellationToken);
    Task<bool> DeletePricePlanAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<BillingEnrollment>> ListEnrollmentsAsync(CancellationToken cancellationToken);
    Task<BillingEnrollment?> GetEnrollmentAsync(Guid id, CancellationToken cancellationToken);
    Task AddEnrollmentAsync(BillingEnrollment enrollment, CancellationToken cancellationToken);
    Task<bool> UpdateEnrollmentAsync(BillingEnrollment enrollment, CancellationToken cancellationToken);

    Task<IReadOnlyList<Invoice>> ListInvoicesAsync(CancellationToken cancellationToken);
    Task<Invoice?> GetInvoiceAsync(Guid id, CancellationToken cancellationToken);
    Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);
    Task<bool> UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);

    Task<IReadOnlyList<Payment>> ListPaymentsAsync(CancellationToken cancellationToken);
    Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken);

    Task<IReadOnlyList<LessonCredit>> ListCreditsAsync(CancellationToken cancellationToken);
    Task<LessonCredit?> GetCreditAsync(Guid id, CancellationToken cancellationToken);
    Task AddCreditAsync(LessonCredit credit, CancellationToken cancellationToken);
    Task<bool> UpdateCreditAsync(LessonCredit credit, CancellationToken cancellationToken);
}

using LessonRunner.Application.Billing;
using LessonRunner.Domain.Billing;

namespace LessonRunner.Tests;

internal sealed class InMemoryBillingRepository : IBillingRepository
{
    private readonly List<PricePlan> _pricePlans = [];
    private readonly List<BillingEnrollment> _enrollments = [];
    private readonly List<Invoice> _invoices = [];
    private readonly List<Payment> _payments = [];

    public Task<IReadOnlyList<PricePlan>> ListPricePlansAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<PricePlan>>(_pricePlans.ToList());

    public Task<PricePlan?> GetPricePlanAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_pricePlans.FirstOrDefault(plan => plan.Id == id));

    public Task AddPricePlanAsync(PricePlan pricePlan, CancellationToken cancellationToken)
    {
        _pricePlans.Add(pricePlan);
        return Task.CompletedTask;
    }

    public Task<bool> UpdatePricePlanAsync(PricePlan pricePlan, CancellationToken cancellationToken) =>
        Task.FromResult(_pricePlans.Any(plan => plan.Id == pricePlan.Id));

    public Task<bool> DeletePricePlanAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_pricePlans.RemoveAll(plan => plan.Id == id) > 0);

    public Task<IReadOnlyList<BillingEnrollment>> ListEnrollmentsAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<BillingEnrollment>>(_enrollments.ToList());

    public Task<BillingEnrollment?> GetEnrollmentAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_enrollments.FirstOrDefault(enrollment => enrollment.Id == id));

    public Task AddEnrollmentAsync(BillingEnrollment enrollment, CancellationToken cancellationToken)
    {
        _enrollments.Add(enrollment);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateEnrollmentAsync(BillingEnrollment enrollment, CancellationToken cancellationToken) =>
        Task.FromResult(_enrollments.Any(item => item.Id == enrollment.Id));

    public Task<IReadOnlyList<Invoice>> ListInvoicesAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Invoice>>(_invoices.ToList());

    public Task<Invoice?> GetInvoiceAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_invoices.FirstOrDefault(invoice => invoice.Id == id));

    public Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        _invoices.Add(invoice);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken) =>
        Task.FromResult(_invoices.Any(item => item.Id == invoice.Id));

    public Task<IReadOnlyList<Payment>> ListPaymentsAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Payment>>(_payments.ToList());

    public Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        _payments.Add(payment);
        return Task.CompletedTask;
    }

    private readonly List<LessonCredit> _credits = [];

    public Task<IReadOnlyList<LessonCredit>> ListCreditsAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<LessonCredit> result = _credits.OrderByDescending(credit => credit.IssuedAt).ToList();
        return Task.FromResult(result);
    }

    public Task<LessonCredit?> GetCreditAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_credits.FirstOrDefault(credit => credit.Id == id));

    public Task AddCreditAsync(LessonCredit credit, CancellationToken cancellationToken)
    {
        _credits.Add(credit);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateCreditAsync(LessonCredit credit, CancellationToken cancellationToken)
    {
        var index = _credits.FindIndex(item => item.Id == credit.Id);

        if (index < 0)
        {
            return Task.FromResult(false);
        }

        _credits[index] = credit;
        return Task.FromResult(true);
    }
}

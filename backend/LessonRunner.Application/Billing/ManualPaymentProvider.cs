using LessonRunner.Domain.Billing;

namespace LessonRunner.Application.Billing;

public sealed class ManualPaymentProvider : IPaymentProvider
{
    public string Name => "manual";

    public Task<Payment> RecordManualPaymentAsync(
        Invoice invoice,
        long amountCents,
        DateTimeOffset paidAt,
        string? externalId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new Payment
        {
            InvoiceId = invoice.Id,
            Provider = Name,
            ExternalId = string.IsNullOrWhiteSpace(externalId) ? null : externalId.Trim(),
            AmountCents = amountCents,
            Currency = invoice.Currency,
            Status = PaymentStatus.Succeeded,
            PaidAt = paidAt
        });
    }
}

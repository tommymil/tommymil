using LessonRunner.Domain.Billing;

namespace LessonRunner.Application.Billing;

public interface IPaymentProvider
{
    string Name { get; }
    Task<Payment> RecordManualPaymentAsync(Invoice invoice, long amountCents, DateTimeOffset paidAt, string? externalId, CancellationToken cancellationToken);
}

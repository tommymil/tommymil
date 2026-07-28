using LessonRunner.Domain.Billing;
using LessonRunner.Infrastructure.Billing;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Round-trip repozytorium billingu na realnym SQLite. Chroni przed regresją sortowania po
/// DateTimeOffset - SQLite nie tłumaczy ORDER BY na tym typie, więc porządkujemy po stronie klienta.
/// </summary>
public sealed class EfBillingRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public EfBillingRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        using var context = new AppDbContext(_options);
        context.Database.EnsureCreated();
    }

    private AppDbContext CreateContext() => new(_options);

    [Fact]
    public async Task ListEnrollmentsInvoicesPayments_OrderByDateOnSqlite_DoesNotThrow()
    {
        var enrollment = new BillingEnrollment { ParticipantId = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var invoiceOld = new Invoice
        {
            BillingEnrollmentId = enrollment.Id,
            ParticipantId = enrollment.ParticipantId,
            GroupId = enrollment.GroupId,
            Number = "FV-1",
            AmountCents = 10000,
            Currency = "PLN",
            DueDate = new DateOnly(2026, 7, 1),
            IssuedAt = new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero)
        };
        var invoiceNew = new Invoice
        {
            BillingEnrollmentId = enrollment.Id,
            ParticipantId = enrollment.ParticipantId,
            GroupId = enrollment.GroupId,
            Number = "FV-2",
            AmountCents = 20000,
            Currency = "PLN",
            DueDate = new DateOnly(2026, 8, 1),
            IssuedAt = new DateTimeOffset(2026, 8, 1, 10, 0, 0, TimeSpan.Zero)
        };
        var payment = new Payment { InvoiceId = invoiceNew.Id, Provider = "manual", AmountCents = 20000, Currency = "PLN" };

        await using (var context = CreateContext())
        {
            var repository = new EfBillingRepository(context);
            await repository.AddEnrollmentAsync(enrollment, CancellationToken.None);
            await repository.AddInvoiceAsync(invoiceOld, CancellationToken.None);
            await repository.AddInvoiceAsync(invoiceNew, CancellationToken.None);
            await repository.AddPaymentAsync(payment, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var repository = new EfBillingRepository(context);

            var enrollments = await repository.ListEnrollmentsAsync(CancellationToken.None);
            var invoices = await repository.ListInvoicesAsync(CancellationToken.None);
            var payments = await repository.ListPaymentsAsync(CancellationToken.None);

            Assert.Single(enrollments);
            Assert.Equal(2, invoices.Count);
            // Najnowsza faktura (późniejszy IssuedAt) jest pierwsza.
            Assert.Equal("FV-2", invoices[0].Number);
            Assert.Single(payments);
        }
    }

    public void Dispose() => _connection.Dispose();
}

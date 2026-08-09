using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Auth;

internal sealed class EfAccountTokenRepository(AppDbContext dbContext) : IAccountTokenRepository
{
    public async Task AddAsync(AccountToken token, CancellationToken cancellationToken)
    {
        dbContext.AccountTokens.Add(new AccountTokenDocument
        {
            Id = token.Id,
            UserId = token.UserId,
            TokenHash = token.TokenHash,
            Purpose = token.Purpose.ToString(),
            CreatedAt = token.CreatedAt,
            ExpiresAt = token.ExpiresAt,
            UsedAt = token.UsedAt,
            IssuedByUserId = token.IssuedByUserId
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AccountToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        var document = await dbContext.AccountTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task<bool> MarkUsedAsync(Guid id, DateTimeOffset usedAt, CancellationToken cancellationToken)
    {
        var document = await dbContext.AccountTokens.FirstOrDefaultAsync(token => token.Id == id, cancellationToken);

        if (document is null || document.UsedAt is not null)
        {
            return false;
        }

        document.UsedAt = usedAt;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> InvalidateActiveAsync(Guid userId, DateTimeOffset usedAt, CancellationToken cancellationToken)
    {
        var documents = await dbContext.AccountTokens
            .Where(token => token.UserId == userId && token.UsedAt == null)
            .ToListAsync(cancellationToken);

        if (documents.Count == 0)
        {
            return 0;
        }

        foreach (var document in documents)
        {
            document.UsedAt = usedAt;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return documents.Count;
    }

    /// <summary>
    /// Ile tokenów wydano temu użytkownikowi od podanego momentu — limit na prośby o reset hasła.
    ///
    /// SQLite nie tłumaczy porównań `DateTimeOffset` na SQL, więc po użytkowniku filtrujemy
    /// w bazie (to się tłumaczy), a po dacie w pamięci. Tokeny jednego konta to garść wierszy,
    /// więc koszt jest żaden — a bez tego cały formularz „nie pamiętam hasła” kończył się
    /// wyjątkiem `InvalidOperationException`.
    /// </summary>
    public async Task<int> CountIssuedSinceAsync(Guid userId, DateTimeOffset since, CancellationToken cancellationToken)
    {
        var createdAt = await dbContext.AccountTokens
            .AsNoTracking()
            .Where(token => token.UserId == userId)
            .Select(token => token.CreatedAt)
            .ToListAsync(cancellationToken);

        return createdAt.Count(moment => moment >= since);
    }

    private static AccountToken ToDomain(AccountTokenDocument document) => new()
    {
        Id = document.Id,
        UserId = document.UserId,
        TokenHash = document.TokenHash,
        Purpose = Enum.TryParse<AccountTokenPurpose>(document.Purpose, ignoreCase: true, out var purpose)
            ? purpose
            : AccountTokenPurpose.PasswordReset,
        CreatedAt = document.CreatedAt,
        ExpiresAt = document.ExpiresAt,
        UsedAt = document.UsedAt,
        IssuedByUserId = document.IssuedByUserId
    };
}

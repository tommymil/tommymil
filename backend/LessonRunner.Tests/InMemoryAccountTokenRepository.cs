using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;

namespace LessonRunner.Tests;

internal sealed class InMemoryAccountTokenRepository : IAccountTokenRepository
{
    private readonly List<AccountToken> _tokens = [];

    public IReadOnlyList<AccountToken> Tokens => _tokens;

    public Task AddAsync(AccountToken token, CancellationToken cancellationToken)
    {
        _tokens.Add(token);
        return Task.CompletedTask;
    }

    public Task<AccountToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        Task.FromResult(_tokens.FirstOrDefault(token => token.TokenHash == tokenHash));

    public Task<bool> MarkUsedAsync(Guid id, DateTimeOffset usedAt, CancellationToken cancellationToken)
    {
        var token = _tokens.FirstOrDefault(item => item.Id == id);

        if (token is null || token.UsedAt is not null)
        {
            return Task.FromResult(false);
        }

        token.UsedAt = usedAt;
        return Task.FromResult(true);
    }

    public Task<int> InvalidateActiveAsync(Guid userId, DateTimeOffset usedAt, CancellationToken cancellationToken)
    {
        var active = _tokens.Where(token => token.UserId == userId && token.UsedAt is null).ToList();

        foreach (var token in active)
        {
            token.UsedAt = usedAt;
        }

        return Task.FromResult(active.Count);
    }

    public Task<int> CountIssuedSinceAsync(Guid userId, DateTimeOffset since, CancellationToken cancellationToken) =>
        Task.FromResult(_tokens.Count(token => token.UserId == userId && token.CreatedAt >= since));
}

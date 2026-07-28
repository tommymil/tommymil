using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;

namespace LessonRunner.Tests;

internal sealed class FakeTokenService : ITokenService
{
    public TokenResult CreateToken(User user)
    {
        return new TokenResult($"token-for-{user.Id}", DateTimeOffset.UtcNow.AddHours(1));
    }
}

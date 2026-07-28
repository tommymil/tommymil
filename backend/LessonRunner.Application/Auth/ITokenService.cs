using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Auth;

public sealed record TokenResult(string Token, DateTimeOffset ExpiresAt);

public interface ITokenService
{
    TokenResult CreateToken(User user);
}

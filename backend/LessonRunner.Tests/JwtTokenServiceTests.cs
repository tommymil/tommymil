using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;
using LessonRunner.Infrastructure.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace LessonRunner.Tests;

public sealed class JwtTokenServiceTests
{
    private static readonly JwtOptions Options = new()
    {
        Issuer = "LessonRunnerTest",
        Audience = "LessonRunnerTestClients",
        SigningKey = "test-signing-key-that-is-long-enough-32+bytes",
        ExpiryMinutes = 60
    };

    [Fact]
    public void CreateToken_ProducesValidatableTokenWithExpectedClaims()
    {
        var service = new JwtTokenService(MicrosoftOptions(Options));
        var user = new User
        {
            Email = "admin@example.com",
            PasswordHash = "irrelevant",
            Role = UserRole.Admin
        };

        var result = service.CreateToken(user);

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.True(result.ExpiresAt > DateTimeOffset.UtcNow);

        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Options.Issuer,
            ValidAudience = Options.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.SigningKey)),
            RoleClaimType = ClaimTypes.Role
        };

        var principal = handler.ValidateToken(result.Token, parameters, out _);

        Assert.Equal(user.Id.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal("admin@example.com", principal.FindFirst(ClaimTypes.Email)?.Value);
        Assert.True(principal.IsInRole("Admin"));
    }

    private static IOptions<JwtOptions> MicrosoftOptions(JwtOptions options) => Microsoft.Extensions.Options.Options.Create(options);
}

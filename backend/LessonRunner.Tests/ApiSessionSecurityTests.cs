using System.Net;
using System.Net.Http.Json;
using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Cykl życia sesji: dezaktywacja konta i zmiana hasła mają unieważniać token natychmiast,
/// a ostatni administrator nie może zostać wyłączony.
/// </summary>
public sealed class ApiSessionSecurityTests
{
    [Fact]
    public async Task DeactivatingUser_InvalidatesIssuedToken()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();
        var instructor = await factory.CreateClientForAsync(UserRole.Instructor, "trener@test.local");

        Assert.Equal(HttpStatusCode.OK, (await instructor.GetAsync("/api/auth/me")).StatusCode);

        var instructorId = await FindUserIdAsync(factory, "trener@test.local");
        var deactivated = await admin.PostAsync($"/api/users/{instructorId}/deactivate", null);
        Assert.Equal(HttpStatusCode.NoContent, deactivated.StatusCode);

        // Token wciąż ma ważny podpis i datę wygaśnięcia - odrzucić musi go kontrola stanu konta.
        Assert.Equal(HttpStatusCode.Unauthorized, (await instructor.GetAsync("/api/auth/me")).StatusCode);
    }

    [Fact]
    public async Task ChangingPassword_InvalidatesOtherSessions()
    {
        await using var factory = new ApiFactory();
        var first = await factory.CreateClientForAsync(UserRole.Instructor, "trener@test.local", "stare-haslo-123");
        var second = await factory.CreateAuthenticatedClientAsync("trener@test.local", "stare-haslo-123");

        var changed = await second.PostAsJsonAsync(
            "/api/auth/change-password",
            new { currentPassword = "stare-haslo-123", newPassword = "nowe-haslo-456" });
        Assert.Equal(HttpStatusCode.NoContent, changed.StatusCode);

        Assert.Equal(HttpStatusCode.Unauthorized, (await first.GetAsync("/api/auth/me")).StatusCode);
    }

    [Fact]
    public async Task LastActiveAdmin_CannotBeDeactivated()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();
        var adminId = await FindUserIdAsync(factory, ApiFactory.AdminEmail);

        var response = await admin.PostAsync($"/api/users/{adminId}/deactivate", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/users")).StatusCode);
    }

    [Fact]
    public async Task Login_IsRateLimited()
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateClient();

        HttpStatusCode? lastStatus = null;

        for (var attempt = 0; attempt < 20; attempt++)
        {
            var response = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { email = ApiFactory.AdminEmail, password = "zle-haslo" });

            lastStatus = response.StatusCode;

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                break;
            }
        }

        Assert.Equal(HttpStatusCode.TooManyRequests, lastStatus);
    }

    private static async Task<Guid> FindUserIdAsync(ApiFactory factory, string email)
    {
        using var scope = factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var user = await users.GetByEmailAsync(User.NormalizeEmail(email), CancellationToken.None);

        Assert.NotNull(user);
        return user!.Id;
    }
}

using System.Net;
using System.Net.Http.Json;
using LessonRunner.Application.Audit;
using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Automatyczny dziennik zmian. Testujemy przez realny pipeline HTTP, bo filtr audytu
/// jest przypięty do grup tras — testy serwisów go nie widzą.
/// </summary>
public sealed class ApiAuditTests
{
    [Fact]
    public async Task MutatingRequest_IsAudited_WithActorAndOutcome()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();
        var adminId = await FindUserIdAsync(factory, ApiFactory.AdminEmail);

        var response = await admin.PostAsJsonAsync(
            "/api/participants",
            new { firstName = "Jan", lastName = "Kowalski" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var logs = await ReadAuditAsync(admin, "?entityType=participants");
        var wpis = Assert.Single(logs, log => log.Action == "CreateParticipant");

        Assert.Equal(adminId, wpis.ActorUserId);
        Assert.True(wpis.Success);
        Assert.Equal("participants", wpis.EntityType);
    }

    [Fact]
    public async Task FailedRequest_IsAuditedAsUnsuccessful()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();

        // Uczestnik bez nazwiska - backend odrzuci żądanie.
        var response = await admin.PostAsJsonAsync("/api/participants", new { firstName = "", lastName = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var logs = await ReadAuditAsync(admin, "?entityType=participants&success=false");
        Assert.Contains(logs, log => log.Action == "CreateParticipant" && !log.Success);
    }

    [Fact]
    public async Task ReadRequests_AreNotAudited()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();

        await admin.GetAsync("/api/participants");
        await admin.GetAsync("/api/groups");

        var logs = await ReadAuditAsync(admin, "?limit=500");

        // Odczyty tylko zaśmiecałyby dziennik - zapisujemy wyłącznie zmiany stanu.
        Assert.DoesNotContain(logs, log => log.Action is "GetParticipants" or "GetGroups");
    }

    /// <summary>
    /// Regresja bezpieczeństwa: dziennik zapisuje ścieżkę i status, nigdy treści żądania.
    /// Inaczej ustawienie hasła zostawiłoby je otwartym tekstem w bazie.
    /// </summary>
    [Fact]
    public async Task PasswordChange_IsAudited_ButNeverStoresThePassword()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();
        await factory.CreateClientForAsync(UserRole.Instructor, "trener@test.local");
        var instructorId = await FindUserIdAsync(factory, "trener@test.local");

        const string tajneHaslo = "bardzo-tajne-haslo-987";
        var response = await admin.PostAsJsonAsync($"/api/users/{instructorId}/password", new { password = tajneHaslo });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var logs = await ReadAuditAsync(admin, "?limit=500");

        Assert.Contains(logs, log => log.Action == "user.password.set");
        Assert.DoesNotContain(logs, log => log.Details?.Contains(tajneHaslo, StringComparison.Ordinal) == true);
    }

    [Fact]
    public async Task SelfAuditedEndpoint_IsNotLoggedTwice()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();

        var response = await admin.PostAsJsonAsync(
            "/api/users",
            new { email = "nowy@test.local", password = "haslo-testowe-123", role = "instructor" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var logs = await ReadAuditAsync(admin, "?entityType=users");

        // Endpoint zapisuje własny wpis z adresem e-mail, więc filtr automatyczny go pomija.
        Assert.Single(logs, log => log.Action == "user.create");
        Assert.DoesNotContain(logs, log => log.Action == "CreateUser");
    }

    private static async Task<IReadOnlyList<AuditLogDto>> ReadAuditAsync(HttpClient client, string query)
    {
        var response = await client.GetAsync($"/api/audit{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<AuditLogDto>>() ?? [];
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

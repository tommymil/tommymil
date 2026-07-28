using System.Net.Http.Headers;
using System.Net.Http.Json;
using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace LessonRunner.Tests;

/// <summary>
/// Uruchamia realne API w pamięci (pełny pipeline: uwierzytelnianie, polityki autoryzacji,
/// middleware), na osobnym pliku SQLite per instancja testu. Testy jednostkowe sprawdzają
/// serwisy, te sprawdzają to, czego serwisy nie widzą: kto faktycznie dostaje 200, 403 i 401.
/// </summary>
internal sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(),
        $"lesson-runner-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:DefaultConnection", $"Data Source={_databasePath}");
        builder.UseSetting("Jwt:SigningKey", "test-signing-key-that-is-long-enough-32-bytes-min");
        builder.UseSetting("Jwt:Issuer", "LessonRunner");
        builder.UseSetting("Jwt:Audience", "LessonRunnerClients");
        builder.UseSetting("Jwt:ExpiryMinutes", "60");
        builder.UseSetting("Smtp:Mode", "Log");
        // Środowisko "Testing" nie jest developerskie, więc zadziała ścieżka bootstrapu admina
        // zamiast seedu - dokładnie tak, jak na produkcji.
        builder.UseSetting("BOOTSTRAP_ADMIN_EMAIL", AdminEmail);
        builder.UseSetting("BOOTSTRAP_ADMIN_PASSWORD", AdminPassword);
    }

    public const string AdminEmail = "admin@test.local";
    public const string AdminPassword = "admin-haslo-123";

    /// <summary>Zakłada konto o wskazanej roli i zwraca klienta z jego tokenem.</summary>
    public async Task<HttpClient> CreateClientForAsync(UserRole role, string email, string password = "haslo-testowe-123")
    {
        using (var scope = Services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            if (await users.GetByEmailAsync(User.NormalizeEmail(email), CancellationToken.None) is null)
            {
                await users.AddAsync(
                    new User
                    {
                        Email = User.NormalizeEmail(email),
                        PasswordHash = hasher.Hash(password),
                        Role = role
                    },
                    CancellationToken.None);
            }
        }

        return await CreateAuthenticatedClientAsync(email, password);
    }

    public async Task<HttpClient> CreateAdminClientAsync() =>
        await CreateAuthenticatedClientAsync(AdminEmail, AdminPassword);

    public async Task<HttpClient> CreateAuthenticatedClientAsync(string email, string password)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>()
            ?? throw new InvalidOperationException("Logowanie nie zwróciło tokenu.");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload.Token);
        return client;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        foreach (var suffix in new[] { string.Empty, "-wal", "-shm" })
        {
            var path = _databasePath + suffix;

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (IOException)
                {
                    // Plik trzymany jeszcze przez pulę połączeń - sprzątnie go system.
                }
            }
        }
    }

    private sealed record LoginResponse(string Token);
}

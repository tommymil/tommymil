using System.Text.RegularExpressions;
using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;
using LessonRunner.Infrastructure.Auth;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Reset hasła i zaproszenia. Testy pracują na tokenie wyciągniętym z treści wysłanego
/// e-maila - tak samo, jak zrobi to użytkownik - bo w bazie leży wyłącznie skrót i nie da się
/// odtworzyć postaci jawnej żadną inną drogą. Przy okazji pilnuje to formatu linku.
/// </summary>
public sealed class AccountTokenServiceTests
{
    private const string Password = "nowe-haslo-123";

    [Fact]
    public async Task PasswordReset_SendsLink_AndSetsNewPassword()
    {
        var fixture = new Fixture();
        var user = await fixture.AddUserAsync("rodzic@test.local", "stare-haslo-123");

        await fixture.Service.RequestPasswordResetAsync(new RequestPasswordResetDto(user.Email), CancellationToken.None);

        var message = Assert.Single(fixture.Emails.Messages);
        Assert.Equal(user.Email, message.ToEmail);
        Assert.Contains("/set-password?token=", message.Body);

        var token = ExtractToken(message.Body);
        var info = await fixture.Service.DescribeAsync(token, CancellationToken.None);

        Assert.NotNull(info);
        Assert.Equal("passwordreset", info!.Purpose);
        Assert.Equal(user.Email, info.Email);

        var changedFor = await fixture.Service.ConfirmPasswordResetAsync(
            new ConfirmPasswordResetDto(token, Password), CancellationToken.None);

        Assert.Equal(user.Id, changedFor);

        var updated = await fixture.Users.GetByIdAsync(user.Id, CancellationToken.None);
        Assert.True(fixture.Hasher.Verify(Password, updated!.PasswordHash));
    }

    [Fact]
    public async Task PasswordReset_TokenWorksOnlyOnce()
    {
        var fixture = new Fixture();
        var user = await fixture.AddUserAsync("jeden-raz@test.local");

        await fixture.Service.RequestPasswordResetAsync(new RequestPasswordResetDto(user.Email), CancellationToken.None);
        var token = ExtractToken(fixture.Emails.Messages[^1].Body);

        Assert.NotNull(await fixture.Service.ConfirmPasswordResetAsync(
            new ConfirmPasswordResetDto(token, Password), CancellationToken.None));

        // Drugie użycie tego samego linku nie może zadziałać - inaczej ktoś, kto raz zajrzał
        // do skrzynki, mógłby wracać po hasło w nieskończoność.
        Assert.Null(await fixture.Service.ConfirmPasswordResetAsync(
            new ConfirmPasswordResetDto(token, "inne-haslo-123"), CancellationToken.None));
        Assert.Null(await fixture.Service.DescribeAsync(token, CancellationToken.None));
    }

    [Fact]
    public async Task NewToken_InvalidatesPreviousOne()
    {
        var fixture = new Fixture();
        var user = await fixture.AddUserAsync("dwa-linki@test.local");

        await fixture.Service.RequestPasswordResetAsync(new RequestPasswordResetDto(user.Email), CancellationToken.None);
        var first = ExtractToken(fixture.Emails.Messages[^1].Body);

        await fixture.Service.RequestPasswordResetAsync(new RequestPasswordResetDto(user.Email), CancellationToken.None);
        var second = ExtractToken(fixture.Emails.Messages[^1].Body);

        // W skrzynce leżą dwie wiadomości; działa najnowsza.
        Assert.Null(await fixture.Service.DescribeAsync(first, CancellationToken.None));
        Assert.NotNull(await fixture.Service.DescribeAsync(second, CancellationToken.None));
    }

    [Fact]
    public async Task PasswordReset_ForUnknownOrInactiveAccount_SendsNothing_AndDoesNotThrow()
    {
        var fixture = new Fixture();
        var disabled = await fixture.AddUserAsync("wylaczony@test.local");
        await fixture.Users.SetActiveAsync(disabled.Id, false, CancellationToken.None);

        await fixture.Service.RequestPasswordResetAsync(
            new RequestPasswordResetDto("nie-ma-takiego@test.local"), CancellationToken.None);
        await fixture.Service.RequestPasswordResetAsync(
            new RequestPasswordResetDto(disabled.Email), CancellationToken.None);

        // Brak wyjątku i brak wysyłki: z zewnątrz oba przypadki wyglądają tak samo jak sukces,
        // a reset hasła nie może być obejściem dezaktywacji konta.
        Assert.Empty(fixture.Emails.Messages);
    }

    [Fact]
    public async Task Invitation_UsesOwnTextAndLongerLifetime()
    {
        var fixture = new Fixture();
        var admin = await fixture.AddUserAsync("admin@test.local", role: UserRole.Admin);
        var parent = await fixture.AddUserAsync("zaproszony@test.local");

        var result = await fixture.Service.SendInvitationAsync(parent.Id, admin.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result!.Sent);
        Assert.True(result.ExpiresAt > DateTimeOffset.UtcNow.AddDays(6));

        var message = Assert.Single(fixture.Emails.Messages);
        Assert.Contains("Zaproszenie", message.Subject);

        var info = await fixture.Service.DescribeAsync(ExtractToken(message.Body), CancellationToken.None);
        Assert.Equal("invitation", info!.Purpose);
    }

    [Fact]
    public async Task Invitation_ForUnknownAccount_ReturnsNull()
    {
        var fixture = new Fixture();

        Assert.Null(await fixture.Service.SendInvitationAsync(Guid.NewGuid(), null, CancellationToken.None));
    }

    [Fact]
    public async Task TooManyRequests_ForOneAccount_StopSending()
    {
        var fixture = new Fixture();
        var user = await fixture.AddUserAsync("zasypany@test.local");

        for (var attempt = 0; attempt < 8; attempt++)
        {
            await fixture.Service.RequestPasswordResetAsync(new RequestPasswordResetDto(user.Email), CancellationToken.None);
        }

        // Limit w API jest per adres IP; ten jest per konto, żeby z wielu adresów nie dało się
        // zasypać jednej skrzynki.
        Assert.Equal(5, fixture.Emails.Messages.Count);
    }

    [Fact]
    public async Task ShortPassword_IsRejected()
    {
        var fixture = new Fixture();
        var user = await fixture.AddUserAsync("krotkie@test.local");
        await fixture.Service.RequestPasswordResetAsync(new RequestPasswordResetDto(user.Email), CancellationToken.None);
        var token = ExtractToken(fixture.Emails.Messages[^1].Body);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Service.ConfirmPasswordResetAsync(new ConfirmPasswordResetDto(token, "krotkie"), CancellationToken.None));
    }

    [Fact]
    public async Task ResetEmail_IsRecordedInNotificationLog()
    {
        var fixture = new Fixture();
        var user = await fixture.AddUserAsync("dziennik@test.local");

        await fixture.Service.RequestPasswordResetAsync(new RequestPasswordResetDto(user.Email), CancellationToken.None);

        var log = Assert.Single(await fixture.Notifications.ListLogsAsync(50, CancellationToken.None));
        Assert.Equal("password-reset", log.Type);
        Assert.Equal("sent", log.Status);
    }

    private static string ExtractToken(string body)
    {
        var match = Regex.Match(body, @"/set-password\?token=([^\s]+)");
        Assert.True(match.Success, "W treści wiadomości nie ma linku z tokenem.");
        return Uri.UnescapeDataString(match.Groups[1].Value);
    }

    private sealed class Fixture
    {
        public InMemoryUserRepository Users { get; } = new();
        public InMemoryAccountTokenRepository Tokens { get; } = new();
        public InMemoryNotificationRepository Notifications { get; } = new();
        public FakeEmailSender Emails { get; } = new();
        public IPasswordHasher Hasher { get; } = new Pbkdf2PasswordHasher();

        public IAccountTokenService Service { get; }

        public Fixture()
        {
            Service = new AccountTokenService(
                Users,
                Tokens,
                Hasher,
                Emails,
                Notifications,
                new AppOptions { PublicOrigin = "https://zajecia.test" });
        }

        public async Task<User> AddUserAsync(
            string email,
            string password = "haslo-testowe-123",
            UserRole role = UserRole.Parent)
        {
            var user = new User
            {
                Email = User.NormalizeEmail(email),
                PasswordHash = Hasher.Hash(password),
                Role = role
            };

            await Users.AddAsync(user, CancellationToken.None);
            return user;
        }
    }
}

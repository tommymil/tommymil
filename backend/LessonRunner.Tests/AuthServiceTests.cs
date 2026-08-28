using LessonRunner.Application.Auth;
using LessonRunner.Infrastructure.Auth;
using Xunit;

namespace LessonRunner.Tests;

public sealed class AuthServiceTests
{
    private static AuthService BuildService(out InMemoryUserRepository repository)
    {
        repository = new InMemoryUserRepository();
        return new AuthService(repository, new Pbkdf2PasswordHasher(), new FakeTokenService());
    }

    [Fact]
    public async Task RegisterAsync_NormalizesEmail_ReturnsToken_AndDoesNotLeakPassword()
    {
        var service = BuildService(out var repository);

        var result = await service.RegisterAsync(
            new RegisterUserDto("Admin@Example.COM", "supersecret", "admin"),
            CancellationToken.None);

        Assert.Equal("admin@example.com", result.User.Email);
        Assert.Equal("admin", result.User.Role);
        Assert.NotEqual(Guid.Empty, result.User.Id);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.True(result.ExpiresAt > DateTimeOffset.UtcNow);

        var stored = await repository.GetByEmailAsync("admin@example.com", CancellationToken.None);
        Assert.NotNull(stored);
        Assert.NotEqual("supersecret", stored!.PasswordHash);
    }

    /// <summary>
    /// Nierozpoznana rola musi być odmową. Wcześniej wpadała cicho na `Instructor`, więc
    /// literówka w polu roli nadawała uprawnienia personelu zamiast zgłosić błąd.
    /// </summary>
    [Fact]
    public async Task RegisterAsync_Throws_ForUnknownRole()
    {
        var service = BuildService(out _);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.RegisterAsync(
            new RegisterUserDto("teacher@example.com", "supersecret", "wizard"),
            CancellationToken.None));

        Assert.Contains("wizard", exception.Message);
    }

    [Theory]
    [InlineData("not-an-email", "supersecret")]
    [InlineData("ok@example.com", "short")]
    public async Task RegisterAsync_Throws_OnInvalidInput(string email, string password)
    {
        var service = BuildService(out _);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(new RegisterUserDto(email, password, "admin"), CancellationToken.None));
    }

    [Fact]
    public async Task RegisterAsync_Throws_WhenEmailAlreadyTaken()
    {
        var service = BuildService(out _);
        await service.RegisterAsync(new RegisterUserDto("dup@example.com", "supersecret", "admin"), CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegisterAsync(new RegisterUserDto("DUP@example.com", "anotherpass", "admin"), CancellationToken.None));
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsUser_OnValidCredentials()
    {
        var service = BuildService(out _);
        await service.RegisterAsync(new RegisterUserDto("user@example.com", "supersecret", "instructor"), CancellationToken.None);

        var result = await service.AuthenticateAsync(new LoginDto("USER@example.com", "supersecret"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("user@example.com", result!.User.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_OnWrongPassword()
    {
        var service = BuildService(out _);
        await service.RegisterAsync(new RegisterUserDto("user@example.com", "supersecret", "instructor"), CancellationToken.None);

        var result = await service.AuthenticateAsync(new LoginDto("user@example.com", "wrongpass"), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WhenUserMissing()
    {
        var service = BuildService(out _);

        var result = await service.AuthenticateAsync(new LoginDto("ghost@example.com", "supersecret"), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_ChangesPassword_WhenCurrentPasswordMatches()
    {
        var service = BuildService(out var repository);
        await service.RegisterAsync(new RegisterUserDto("user@example.com", "supersecret", "instructor"), CancellationToken.None);
        var user = await repository.GetByEmailAsync("user@example.com", CancellationToken.None);

        var changed = await service.ChangePasswordAsync(
            user!.Id,
            new ChangePasswordDto("supersecret", "newsecret123"),
            CancellationToken.None);

        Assert.True(changed);
        Assert.Null(await service.AuthenticateAsync(new LoginDto("user@example.com", "supersecret"), CancellationToken.None));
        Assert.NotNull(await service.AuthenticateAsync(new LoginDto("user@example.com", "newsecret123"), CancellationToken.None));
    }

    [Fact]
    public async Task ChangePasswordAsync_ReturnsFalse_WhenCurrentPasswordIsWrong()
    {
        var service = BuildService(out var repository);
        await service.RegisterAsync(new RegisterUserDto("user@example.com", "supersecret", "instructor"), CancellationToken.None);
        var user = await repository.GetByEmailAsync("user@example.com", CancellationToken.None);

        var changed = await service.ChangePasswordAsync(
            user!.Id,
            new ChangePasswordDto("wrongpass", "newsecret123"),
            CancellationToken.None);

        Assert.False(changed);
        Assert.NotNull(await service.AuthenticateAsync(new LoginDto("user@example.com", "supersecret"), CancellationToken.None));
        Assert.Null(await service.AuthenticateAsync(new LoginDto("user@example.com", "newsecret123"), CancellationToken.None));
    }

    [Fact]
    public async Task ChangePasswordAsync_RejectsShortNewPassword()
    {
        var service = BuildService(out var repository);
        await service.RegisterAsync(new RegisterUserDto("user@example.com", "supersecret", "instructor"), CancellationToken.None);
        var user = await repository.GetByEmailAsync("user@example.com", CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ChangePasswordAsync(
                user!.Id,
                new ChangePasswordDto("supersecret", "short"),
                CancellationToken.None));
    }
}

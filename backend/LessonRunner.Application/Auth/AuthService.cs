using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Auth;

public sealed class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
        {
            throw new ArgumentException("A valid email is required.", nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long.", nameof(dto));
        }

        var role = ParseRole(dto.Role);
        var normalizedEmail = User.NormalizeEmail(dto.Email);

        if (await userRepository.ExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(dto.Password),
            Role = role
        };

        await userRepository.AddAsync(user, cancellationToken);

        return ToResponse(user);
    }

    public async Task<AuthResponseDto?> AuthenticateAsync(LoginDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return null;
        }

        var normalizedEmail = User.NormalizeEmail(dto.Email);
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive || !passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            return null;
        }

        return ToResponse(user);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
        {
            throw new ArgumentException("Podaj obecne hasło.");
        }

        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 8)
        {
            throw new ArgumentException("Nowe hasło musi mieć co najmniej 8 znaków.");
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null || !user.IsActive || !passwordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
        {
            return false;
        }

        return await userRepository.SetPasswordHashAsync(userId, passwordHasher.Hash(dto.NewPassword), cancellationToken);
    }

    private static UserRole ParseRole(string role)
    {
        return Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsed)
            ? parsed
            : UserRole.Instructor;
    }

    private AuthResponseDto ToResponse(User user)
    {
        var token = tokenService.CreateToken(user);
        var userDto = new AuthUserDto(user.Id, user.Email, user.Role.ToString().ToLowerInvariant(), user.DisplayName);

        return new AuthResponseDto(token.Token, token.ExpiresAt, userDto);
    }
}

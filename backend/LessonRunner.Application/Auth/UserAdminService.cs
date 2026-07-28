using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Auth;

public sealed class UserAdminService(IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserAdminService
{
    public async Task<IReadOnlyList<UserListItemDto>> ListAsync(CancellationToken cancellationToken)
    {
        var users = await userRepository.ListAsync(cancellationToken);
        return users.Select(ToDto).ToList();
    }

    public async Task<UserListItemDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
        {
            throw new ArgumentException("Podaj prawidłowy adres e-mail.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
        {
            throw new ArgumentException("Hasło musi mieć co najmniej 8 znaków.");
        }

        var role = Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var parsed) ? parsed : UserRole.Instructor;
        var normalizedEmail = User.NormalizeEmail(dto.Email);

        if (await userRepository.ExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new InvalidOperationException("Konto z tym adresem e-mail już istnieje.");
        }

        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(dto.Password),
            Role = role,
            FirstName = NullIfEmpty(dto.FirstName),
            LastName = NullIfEmpty(dto.LastName),
            Phone = NullIfEmpty(dto.Phone)
        };

        await userRepository.AddAsync(user, cancellationToken);
        return ToDto(user);
    }

    public async Task<bool> SetActiveAsync(Guid id, bool isActive, Guid? actingUserId, CancellationToken cancellationToken)
    {
        if (!isActive)
        {
            if (actingUserId == id)
            {
                throw new InvalidOperationException("Nie można dezaktywować własnego konta.");
            }

            var user = await userRepository.GetByIdAsync(id, cancellationToken);

            // Bez tej blokady wyłączenie ostatniego admina zamyka system na stałe:
            // zalogować się nie może nikt, a bootstrap widzi konto w bazie i go nie odtworzy.
            if (user is { Role: UserRole.Admin, IsActive: true }
                && await userRepository.CountActiveAdminsAsync(cancellationToken) <= 1)
            {
                throw new InvalidOperationException(
                    "To jedyne aktywne konto administratora. Najpierw dodaj innego administratora.");
            }
        }

        return await userRepository.SetActiveAsync(id, isActive, cancellationToken);
    }

    public Task<bool> UpdateProfileAsync(Guid id, UpdateUserProfileDto dto, CancellationToken cancellationToken) =>
        userRepository.SetProfileAsync(id, NullIfEmpty(dto.FirstName), NullIfEmpty(dto.LastName), NullIfEmpty(dto.Phone), cancellationToken);

    public Task<bool> SetPasswordAsync(Guid id, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new ArgumentException("Hasło musi mieć co najmniej 8 znaków.");
        }

        return userRepository.SetPasswordHashAsync(id, passwordHasher.Hash(password), cancellationToken);
    }

    private static UserListItemDto ToDto(User user) =>
        new(
            user.Id,
            user.Email,
            user.Role.ToString().ToLowerInvariant(),
            user.IsActive,
            user.FirstName,
            user.LastName,
            user.Phone,
            user.DisplayName);

    private static string? NullIfEmpty(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}

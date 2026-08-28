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

        // Puste hasło = konto bez hasła, do którego admin wyśle zaproszenie. Podane hasło
        // nadal musi być sensowne.
        var withoutPassword = string.IsNullOrWhiteSpace(dto.Password);

        if (!withoutPassword && dto.Password!.Length < 8)
        {
            throw new ArgumentException("Hasło musi mieć co najmniej 8 znaków.");
        }

        var role = ParseRole(dto.Role);
        var normalizedEmail = User.NormalizeEmail(dto.Email);

        if (await userRepository.ExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new InvalidOperationException("Konto z tym adresem e-mail już istnieje.");
        }

        var user = new User
        {
            Email = normalizedEmail,
            // Konto bez hasła dostaje skrót z losowego ciągu, którego nikt nie zna. Nie ma tu
            // osobnego stanu „brak hasła”, bo taki stan prędzej czy później ktoś potraktowałby
            // jako „hasło się zgadza”. Zalogować się da dopiero po ustawieniu hasła z zaproszenia.
            PasswordHash = passwordHasher.Hash(withoutPassword ? UnusablePassword() : dto.Password!),
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

    /// <summary>
    /// Zmiana roli istniejącego konta.
    ///
    /// Powstało dlatego, że wcześniej roli **nie dało się poprawić w żaden sposób** — konto
    /// założone z błędną rolą zostawało z nią na stałe, a usuwania kont nie ma. W połączeniu
    /// z cichym domyślnym wpadaniem na `Instructor` oznaczało to, że rodzic z literówką
    /// w formularzu dostawał nieodwracalny dostęp do konspektów, grafiku i rejestru zgłoszeń.
    ///
    /// Dwie blokady, obie z tego samego powodu co przy dezaktywacji: nie wolno odciąć sobie
    /// dostępu ani odebrać roli ostatniemu administratorowi, bo system nie ma jak tego cofnąć.
    /// </summary>
    public async Task<bool> SetRoleAsync(Guid id, string role, Guid? actingUserId, CancellationToken cancellationToken)
    {
        var target = ParseRole(role);
        var user = await userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return false;
        }

        if (user.Role == target)
        {
            return true;
        }

        if (actingUserId == id)
        {
            throw new InvalidOperationException("Nie można zmienić roli własnego konta.");
        }

        if (user is { Role: UserRole.Admin, IsActive: true }
            && await userRepository.CountActiveAdminsAsync(cancellationToken) <= 1)
        {
            throw new InvalidOperationException(
                "To jedyne aktywne konto administratora. Najpierw dodaj innego administratora.");
        }

        return await userRepository.SetRoleAsync(id, target, cancellationToken);
    }

    /// <summary>
    /// Rola musi zostać rozpoznana.
    ///
    /// Wcześniej nierozpoznana wartość cicho stawała się `Instructor`. Literówka w polu roli
    /// zakładała więc rodzicowi konto personelu — z dostępem do treści lekcji, grafiku,
    /// materiałów i wyszukiwarki po dzieciach. Odmowa jest jedyną bezpieczną reakcją:
    /// przy nadawaniu uprawnień zgadywanie intencji jest niedopuszczalne.
    /// </summary>
    private static UserRole ParseRole(string? role) =>
        Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed)
            ? parsed
            : throw new ArgumentException(
                $"Nieznana rola '{role}'. Dozwolone: {string.Join(", ", Enum.GetNames<UserRole>())}.");

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

    private static string UnusablePassword() =>
        Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

    private static string? NullIfEmpty(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}

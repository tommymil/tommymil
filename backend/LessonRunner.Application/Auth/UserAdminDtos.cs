namespace LessonRunner.Application.Auth;

public sealed record UserListItemDto(
    Guid Id,
    string Email,
    string Role,
    bool IsActive,
    string? FirstName,
    string? LastName,
    string? Phone,
    string DisplayName);

/// <summary>Zakładanie konta. <paramref name="Password"/> jest opcjonalne: puste oznacza konto
/// bez hasła, do którego wysyłamy zaproszenie. Rodzic ustawia hasło sam i nikt nigdy nie zna
/// cudzego hasła — dotąd admin musiał je wymyślić i przekazać kanałem, którego nie kontroluje.</summary>
public sealed record CreateUserDto(
    string Email,
    string? Password,
    string Role,
    string? FirstName = null,
    string? LastName = null,
    string? Phone = null);

public sealed record UpdateUserProfileDto(string? FirstName, string? LastName, string? Phone);

public sealed record SetPasswordDto(string Password);

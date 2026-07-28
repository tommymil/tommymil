namespace LessonRunner.Application.Auth;

public interface IAuthService
{
    /// <summary>Rejestruje nowego użytkownika i zwraca token. Rzuca, gdy email jest zajęty lub dane są niepoprawne.</summary>
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken);

    /// <summary>Weryfikuje poświadczenia i zwraca token. Zwraca null, gdy email lub hasło się nie zgadza.</summary>
    Task<AuthResponseDto?> AuthenticateAsync(LoginDto dto, CancellationToken cancellationToken);

    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken);
}

namespace LessonRunner.Application.Auth;

public sealed record RegisterUserDto(string Email, string Password, string Role);

public sealed record LoginDto(string Email, string Password);

public sealed record ChangePasswordDto(string CurrentPassword, string NewPassword);

/// <summary>Publiczna reprezentacja uwierzytelnionego użytkownika.</summary>
public sealed record AuthUserDto(Guid Id, string Email, string Role, string DisplayName);

/// <summary>Odpowiedź logowania/rejestracji: token JWT, czas ważności oraz dane użytkownika.</summary>
public sealed record AuthResponseDto(string Token, DateTimeOffset ExpiresAt, AuthUserDto User);

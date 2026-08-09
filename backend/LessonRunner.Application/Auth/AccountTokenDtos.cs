namespace LessonRunner.Application.Auth;

public sealed record RequestPasswordResetDto(string Email);

public sealed record ConfirmPasswordResetDto(string Token, string Password);

/// <summary>Co ekran ustawiania hasła ma pokazać: czyje to konto i z jakiego przepływu token
/// pochodzi (reset brzmi inaczej niż powitanie przy zaproszeniu).</summary>
public sealed record AccountTokenInfoDto(string Purpose, string Email, string DisplayName, DateTimeOffset ExpiresAt);

/// <summary>Wynik wysyłki zaproszenia. `Sent = false` oznacza, że token powstał, ale e-mail
/// nie wyszedł — admin musi o tym wiedzieć, bo inaczej czekałby na rodzica bez końca.</summary>
public sealed record InvitationResultDto(bool Sent, DateTimeOffset ExpiresAt, string? Error);

namespace LessonRunner.Application.Auth;

/// <summary>Reset hasła i zaproszenia — jedyne dwa przepływy, w których hasło ustawia ktoś inny
/// niż administrator ręcznie.</summary>
public interface IAccountTokenService
{
    /// <summary>Żądanie resetu. **Nie zwraca informacji, czy konto istnieje** — z założenia
    /// kończy się tak samo dla adresu znanego i nieznanego.</summary>
    Task RequestPasswordResetAsync(RequestPasswordResetDto dto, CancellationToken cancellationToken);

    /// <summary>Opis tokenu dla ekranu ustawiania hasła. `null` = token nieznany, zużyty
    /// lub przeterminowany.</summary>
    Task<AccountTokenInfoDto?> DescribeAsync(string token, CancellationToken cancellationToken);

    /// <summary>Ustawia hasło z tokenu. Zwraca id konta, dla którego się udało, albo `null`.</summary>
    Task<Guid?> ConfirmPasswordResetAsync(ConfirmPasswordResetDto dto, CancellationToken cancellationToken);

    /// <summary>Wysyła zaproszenie do istniejącego konta (administracja).</summary>
    Task<InvitationResultDto?> SendInvitationAsync(Guid userId, Guid? actingUserId, CancellationToken cancellationToken);
}

using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Auth;

public interface IAccountTokenRepository
{
    Task AddAsync(AccountToken token, CancellationToken cancellationToken);

    /// <summary>Wyszukanie po skrócie — bo w bazie nie ma tokenu w postaci jawnej.</summary>
    Task<AccountToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task<bool> MarkUsedAsync(Guid id, DateTimeOffset usedAt, CancellationToken cancellationToken);

    /// <summary>Unieważnia wszystkie żywe tokeny użytkownika. Wywołujemy przy wydaniu nowego
    /// i po udanej zmianie hasła — jeden e-mail w skrzynce ma prowadzić do jednego działającego
    /// linku, a stary link nie może wskrzesić hasła po fakcie.</summary>
    Task<int> InvalidateActiveAsync(Guid userId, DateTimeOffset usedAt, CancellationToken cancellationToken);

    /// <summary>Ile tokenów wydano temu użytkownikowi od wskazanej chwili. Limit żądań w API jest
    /// per adres IP, a to jest zabezpieczenie per konto: inaczej z wielu adresów dałoby się
    /// zasypać jedną skrzynkę.</summary>
    Task<int> CountIssuedSinceAsync(Guid userId, DateTimeOffset since, CancellationToken cancellationToken);
}

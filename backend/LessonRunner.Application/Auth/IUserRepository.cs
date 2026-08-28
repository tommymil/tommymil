using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Auth;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> ListByRoleAsync(UserRole role, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string normalizedEmail, CancellationToken cancellationToken);
    /// <summary>Czy istnieje przynajmniej jedno AKTYWNE konto administratora.</summary>
    Task<bool> HasAnyAdminAsync(CancellationToken cancellationToken);

    /// <summary>Liczba aktywnych administratorów - używana do blokady dezaktywacji ostatniego z nich.</summary>
    Task<int> CountActiveAdminsAsync(CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
    Task<bool> SetProfileAsync(Guid id, string? firstName, string? lastName, string? phone, CancellationToken cancellationToken);
    Task<bool> SetPasswordHashAsync(Guid id, string passwordHash, CancellationToken cancellationToken);

    /// <summary>
    /// Zmiana roli konta. Musi **unieważnić aktywne sesje**: rola jedzie w tokenie, więc bez
    /// odświeżenia znacznika zdegradowany administrator zostaje administratorem do wygaśnięcia
    /// tokenu, czyli nawet przez dwanaście godzin.
    /// </summary>
    Task<bool> SetRoleAsync(Guid id, UserRole role, CancellationToken cancellationToken);
}

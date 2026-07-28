using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Users;

public sealed class User : Entity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; } = UserRole.Instructor;
    public bool IsActive { get; set; } = true;

    // Profil trenera - opcjonalny, żeby istniejące konta i seed działały bez migracji danych.
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }

    /// <summary>Znacznik ważności wydanych tokenów. Trafia do JWT jako claim `sst` i jest
    /// sprawdzany przy każdym żądaniu. Zmiana znacznika (dezaktywacja konta, zmiana lub reset
    /// hasła) natychmiast unieważnia wszystkie aktywne sesje tego użytkownika - bez tego
    /// wylogowanie następowałoby dopiero po wygaśnięciu tokenu, czyli nawet po 12 godzinach.</summary>
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Nazwa do wyświetlania: pełne imię i nazwisko, a w razie braku - e-mail.</summary>
    public string DisplayName
    {
        get
        {
            var full = $"{FirstName} {LastName}".Trim();
            return full.Length > 0 ? full : Email;
        }
    }

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}

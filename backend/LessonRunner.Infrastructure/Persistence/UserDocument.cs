namespace LessonRunner.Infrastructure.Persistence;

internal sealed class UserDocument
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; }
    public bool IsActive { get; set; } = true;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }

    /// <summary>Znacznik ważności tokenów - patrz <see cref="LessonRunner.Domain.Users.User.SecurityStamp"/>.
    /// Pusty (rekordy sprzed migracji) oznacza "nie sprawdzaj", żeby aktualizacja nie wylogowała
    /// wszystkich w środku dnia; przy pierwszej zmianie hasła zostanie uzupełniony.</summary>
    public string SecurityStamp { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}

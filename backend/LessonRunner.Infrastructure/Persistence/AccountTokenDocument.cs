namespace LessonRunner.Infrastructure.Persistence;

internal sealed class AccountTokenDocument
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    /// <summary>Skrót SHA-256 tokenu — patrz <see cref="LessonRunner.Domain.Users.AccountToken.TokenHash"/>.
    /// Kolumna jest unikalna: skrót jest jednocześnie kluczem wyszukiwania.</summary>
    public required string TokenHash { get; set; }

    public required string Purpose { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public Guid? IssuedByUserId { get; set; }
}

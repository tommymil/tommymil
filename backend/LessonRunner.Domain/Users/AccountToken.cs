using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Users;

/// <summary>
/// Jednorazowy token do ustawienia hasła — z resetu albo z zaproszenia.
///
/// W bazie trzymamy **wyłącznie skrót** (<see cref="TokenHash"/>), nigdy samego tokenu. Token
/// w postaci jawnej istnieje dokładnie raz: w wysłanym e-mailu. Dzięki temu wyciek kopii bazy
/// nie pozwala przejąć żadnego konta — a przy tokenie do ustawienia hasła admina byłoby to
/// równoznaczne z oddaniem całego systemu.
/// </summary>
public sealed class AccountToken : Entity
{
    public required Guid UserId { get; init; }

    /// <summary>Skrót SHA-256 z tokenu, zapisany szesnastkowo małymi literami.</summary>
    public required string TokenHash { get; init; }

    public required AccountTokenPurpose Purpose { get; init; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public required DateTimeOffset ExpiresAt { get; init; }

    /// <summary>Kiedy token został zużyty. Niepusty = token jest już martwy.</summary>
    public DateTimeOffset? UsedAt { get; set; }

    /// <summary>Kto wydał token. Puste przy samodzielnym resecie — wtedy nie ma „kto”,
    /// bo żądanie przychodzi bez zalogowania.</summary>
    public Guid? IssuedByUserId { get; init; }

    /// <summary>Ważność liczona od wydania: reset to akcja w toku, zaproszenie czeka na rodzica.</summary>
    public static TimeSpan LifetimeFor(AccountTokenPurpose purpose) => purpose switch
    {
        AccountTokenPurpose.Invitation => TimeSpan.FromDays(7),
        _ => TimeSpan.FromHours(2)
    };

    public bool IsUsable(DateTimeOffset now) => UsedAt is null && ExpiresAt > now;
}

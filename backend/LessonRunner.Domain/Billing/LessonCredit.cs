using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Billing;

public enum LessonCreditStatus
{
    /// <summary>Do wykorzystania.</summary>
    Available = 0,

    Used = 1,

    /// <summary>Minął termin ważności.</summary>
    Expired = 2,

    /// <summary>Wycofany przez administratora (np. pomyłka przy przyznawaniu).</summary>
    Revoked = 3
}

/// <summary>Na co kredyt został wykorzystany.</summary>
public enum LessonCreditUsage
{
    None = 0,

    /// <summary>Odrabianie z inną grupą albo w innym terminie.</summary>
    MakeupSession = 1,

    /// <summary>Pomniejszenie kolejnej płatności.</summary>
    InvoiceDiscount = 2,

    /// <summary>Zajęcia indywidualne albo warsztaty dodatkowe.</summary>
    ExtraSession = 3,

    /// <summary>Przedłużenie kursu o jedne zajęcia.</summary>
    CourseExtension = 4
}

/// <summary>
/// Kredyt zajęciowy: „należą się jedne zajęcia”.
///
/// Kluczowa zasada z dokumentu koncepcyjnego: zmiany terminu NIE łączymy z rozliczeniem.
/// Odwołanie zajęć jest osobnym zdarzeniem od decyzji, czy rodzicowi należy się zwrot,
/// kredyt, odrobienie czy nic. Kredyt jest zapisem tej decyzji — i jedyną rzeczą, która
/// pozwala później odpowiedzieć na pytanie „co nam się należy za te odwołane zajęcia”.
///
/// Jeden kredyt = jedne zajęcia. `AmountCents` wypełniamy tylko wtedy, gdy kredyt ma
/// pomniejszyć płatność i znamy jego wartość pieniężną.
/// </summary>
public sealed class LessonCredit : Entity
{
    public required Guid ParticipantId { get; set; }

    /// <summary>Grupa, której kredyt dotyczy. Null = kredyt ogólny, do wykorzystania gdziekolwiek.</summary>
    public Guid? GroupId { get; set; }

    /// <summary>Odwołane zajęcia, za które przyznano kredyt. Null przy kredycie uznaniowym.</summary>
    public Guid? SourceSessionId { get; set; }

    /// <summary>Powód przyznania - wymagany, bo bez niego kredyt jest nie do obronienia.</summary>
    public required string Reason { get; set; }

    /// <summary>Wartość pieniężna w groszach, gdy kredyt ma pomniejszyć fakturę.</summary>
    public long? AmountCents { get; set; }
    public string? Currency { get; set; }

    public LessonCreditStatus Status { get; set; } = LessonCreditStatus.Available;

    public DateTimeOffset IssuedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid? IssuedByUserId { get; set; }

    /// <summary>Termin ważności. Null = bezterminowy.</summary>
    public DateOnly? ExpiresAt { get; set; }

    public LessonCreditUsage Usage { get; set; } = LessonCreditUsage.None;
    public DateTimeOffset? UsedAt { get; set; }
    public Guid? UsedByUserId { get; set; }

    /// <summary>Termin, na który kredyt został wykorzystany (odrabianie, zajęcia dodatkowe).</summary>
    public Guid? UsedForSessionId { get; set; }

    /// <summary>Faktura pomniejszona tym kredytem.</summary>
    public Guid? UsedForInvoiceId { get; set; }

    public string? UsageNote { get; set; }

    /// <summary>Czy kredyt nadaje się do wykorzystania na dzień <paramref name="today"/>.</summary>
    public bool IsUsable(DateOnly today) =>
        Status == LessonCreditStatus.Available && (ExpiresAt is null || ExpiresAt >= today);
}

public static class LessonCreditExtensions
{
    public static string Label(this LessonCreditStatus status) => status switch
    {
        LessonCreditStatus.Available => "Do wykorzystania",
        LessonCreditStatus.Used => "Wykorzystany",
        LessonCreditStatus.Expired => "Przeterminowany",
        LessonCreditStatus.Revoked => "Wycofany",
        _ => status.ToString()
    };

    public static string Label(this LessonCreditUsage usage) => usage switch
    {
        LessonCreditUsage.MakeupSession => "Odrabianie zajęć",
        LessonCreditUsage.InvoiceDiscount => "Pomniejszenie płatności",
        LessonCreditUsage.ExtraSession => "Zajęcia dodatkowe",
        LessonCreditUsage.CourseExtension => "Przedłużenie kursu",
        _ => "Nie wykorzystano"
    };

    public static string Name(this LessonCreditStatus status) => status.ToString().ToLowerInvariant();
    public static string Name(this LessonCreditUsage usage) => usage.ToString().ToLowerInvariant();
}

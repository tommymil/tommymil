namespace LessonRunner.Domain.Lessons;

/// <summary>
/// Format organizacyjny konspektu. Określa okno w kalendarzu i zasady planu, a nie temat zajęć.
/// </summary>
public enum LessonKind
{
    /// <summary>Zwykłe zajęcia grupowe: 45 minut pracy, 5 minut przerwy i 45 minut pracy.</summary>
    Standard = 0,

    /// <summary>Godzinna lekcja pokazowa; uczestnik może zakończyć ją od 55. minuty.</summary>
    Showcase = 1
}

/// <summary>
/// Reguły czasu obu formatów. Jedno miejsce, bo te same liczby opisują okno w kalendarzu,
/// walidację konspektu i podpowiedzi w edytorze.
///
/// Obie długości są <b>dokładne</b>, nie widełkowe. Pokazówka rezerwuje 60 minut i ma je
/// wypełnić — 55. minuta to wyłącznie granica, od której uczestnik może wyjść, a nie
/// dopuszczalna długość planu. Plan na 55 minut oznaczałby, że blokujemy rodzicowi godzinę
/// i oddajemy pięć minut pustych.
/// </summary>

public static class LessonKindExtensions
{
    public const int StandardDurationMinutes = 95;
    public const int StandardBlockMinutes = 45;
    public const int ShowcaseDurationMinutes = 60;
    public const int ShowcaseEarlyLeaveAfterMinutes = 55;

    public static string Name(this LessonKind kind) => kind.ToString().ToLowerInvariant();

    public static string Label(this LessonKind kind) => kind switch
    {
        LessonKind.Showcase => "Pokazowa (60 min)",
        _ => "Standardowa grupowa (45 + 5 + 45 min)"
    };

    public static int ScheduledDurationMinutes(this LessonKind kind) => kind switch
    {
        LessonKind.Showcase => ShowcaseDurationMinutes,
        _ => StandardDurationMinutes
    };

    public static int? EarlyLeaveAfterMinutes(this LessonKind kind) => kind switch
    {
        LessonKind.Showcase => ShowcaseEarlyLeaveAfterMinutes,
        _ => null
    };

    public static bool HasValidPlanDuration(this LessonKind kind, int totalMinutes) =>
        totalMinutes == kind.ScheduledDurationMinutes();

    /// <summary>Wyjaśnienie reguły do komunikatu o odmowie — mówi też, z czego wynika liczba.</summary>
    public static string PlanDurationRule(this LessonKind kind) => kind switch
    {
        LessonKind.Showcase =>
            $"Konspekt pokazowy musi mieć dokładnie {ShowcaseDurationMinutes} minut w krokach. "
            + $"Od {ShowcaseEarlyLeaveAfterMinutes}. minuty uczestnik może zakończyć zajęcia, "
            + "więc ostatnie minuty zaplanuj jako domknięcie, a nie nowy materiał.",
        _ =>
            $"Konspekt standardowy musi mieć dokładnie {StandardDurationMinutes} minut w krokach: "
            + $"{StandardBlockMinutes} minut pracy, {StandardDurationMinutes - 2 * StandardBlockMinutes} minut przerwy "
            + $"i {StandardBlockMinutes} minut pracy."
    };
}

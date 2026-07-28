namespace LessonRunner.Domain.Groups;

/// <summary>
/// Status obecności dziecka na konkretnych zajęciach.
///
/// Samo „był / nie był" jest za ubogie na zajęcia online: dziecko potrafi dołączyć 20 minut
/// później, wypaść przez zerwane łącze albo siedzieć na spotkaniu bez wykonywania zadań.
/// Rodzic i instruktor opisują te sytuacje inaczej, a przy reklamacji różnica ma znaczenie.
/// </summary>
public enum AttendanceStatus
{
    /// <summary>Nieobecność bez zgłoszenia - stan domyślny listy przed odhaczeniem.</summary>
    UnexcusedAbsence = 0,

    Present = 1,

    /// <summary>Dołączył z opóźnieniem, ale uczestniczył.</summary>
    Late = 2,

    /// <summary>Wyszedł przed końcem zajęć.</summary>
    LeftEarly = 3,

    /// <summary>Był tylko przez część zajęć (dołączył późno i wyszedł wcześniej).</summary>
    Partial = 4,

    /// <summary>Obecny na spotkaniu, ale nie pracował - nie odpowiadał, nie wykonywał zadań.</summary>
    Inactive = 5,

    /// <summary>Próbował uczestniczyć, ale uniemożliwiły to problemy techniczne.</summary>
    TechnicalIssues = 6,

    /// <summary>Nieobecność zgłoszona wcześniej przez opiekuna.</summary>
    ExcusedAbsence = 7,

    /// <summary>Nie było go na tych zajęciach, bo odrabiał je z inną grupą.</summary>
    MakeupElsewhere = 8
}

public static class AttendanceStatusExtensions
{
    /// <summary>
    /// Czy status liczy się do frekwencji jako obecność.
    ///
    /// Zasada: liczymy każde stawienie się na zajęciach, nawet nieudane.
    /// Problemy techniczne i bierna obecność to wina po stronie warunków albo powód do rozmowy
    /// z rodzicem - nie do odbierania dziecku frekwencji. Odrabianie z inną grupą też liczymy,
    /// bo materiał został zrealizowany.
    /// </summary>
    public static bool CountsAsPresent(this AttendanceStatus status) => status switch
    {
        AttendanceStatus.Present => true,
        AttendanceStatus.Late => true,
        AttendanceStatus.LeftEarly => true,
        AttendanceStatus.Partial => true,
        AttendanceStatus.Inactive => true,
        AttendanceStatus.TechnicalIssues => true,
        AttendanceStatus.MakeupElsewhere => true,
        _ => false
    };

    /// <summary>Czy status oznacza nieobecność, o której warto powiadomić opiekuna.
    /// Zgłoszonej wcześniej nieobecności nie zgłaszamy z powrotem rodzicowi.</summary>
    public static bool ShouldNotifyGuardian(this AttendanceStatus status) =>
        status == AttendanceStatus.UnexcusedAbsence;

    public static string Label(this AttendanceStatus status) => status switch
    {
        AttendanceStatus.Present => "Obecny",
        AttendanceStatus.Late => "Spóźniony",
        AttendanceStatus.LeftEarly => "Wyszedł wcześniej",
        AttendanceStatus.Partial => "Uczestniczył częściowo",
        AttendanceStatus.Inactive => "Obecny, ale nieaktywny",
        AttendanceStatus.TechnicalIssues => "Problemy techniczne",
        AttendanceStatus.ExcusedAbsence => "Nieobecność zgłoszona",
        AttendanceStatus.MakeupElsewhere => "Odrabiał z inną grupą",
        _ => "Nieobecność niezgłoszona"
    };
}

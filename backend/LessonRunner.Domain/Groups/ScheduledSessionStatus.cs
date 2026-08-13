namespace LessonRunner.Domain.Groups;

/// <summary>
/// Status pojedynczych zajęć.
///
/// Cztery stany (zaplanowane / w toku / zakończone / odwołane) nie wystarczają, gdy trzeba
/// rozliczyć się z rodzicem. „Odwołane” znaczy co innego, gdy odwołał instruktor, a co innego,
/// gdy zgłosił się rodzic; zajęcia przerwane awarią to nie to samo, co zajęcia, które
/// po prostu się nie odbyły.
///
/// Świadomie NIE ma tu statusu „przełożone”: w tym modelu przełożenie przesuwa ten sam rekord,
/// więc po zmianie termin nadal jest zaplanowany, tylko na inną datę. Fakt przełożenia
/// (z poprzednią datą, powodem i autorem) trzyma <see cref="SessionChangeLog"/>.
/// </summary>
public enum ScheduledSessionStatus
{
    Planned = 0,
    InProgress = 1,
    Completed = 2,

    /// <summary>Odwołane bez wskazania strony. Zostaje dla danych sprzed rozszerzenia statusów.</summary>
    Cancelled = 3,

    /// <summary>Termin potwierdzony - grupa wie, że zajęcia się odbędą.</summary>
    Confirmed = 4,

    CancelledByInstructor = 5,
    CancelledByParent = 6,

    /// <summary>Zajęcia się zaczęły, ale zostały przerwane awarią techniczną.</summary>
    TechnicalFailure = 7,

    /// <summary>Zajęcia się nie odbyły i nikt ich formalnie nie odwołał.</summary>
    NotDelivered = 8,

    /// <summary>Odwołane i czekające na ustalenie nowego terminu.</summary>
    AwaitingReschedule = 9
}

public static class ScheduledSessionStatusExtensions
{
    /// <summary>Termin jeszcze przed nami - liczy się do grafiku i przypomnień.</summary>
    public static bool IsUpcoming(this ScheduledSessionStatus status) =>
        status is ScheduledSessionStatus.Planned or ScheduledSessionStatus.Confirmed;

    /// <summary>Termin aktywny: przed zajęciami albo w ich trakcie.</summary>
    public static bool IsActive(this ScheduledSessionStatus status) =>
        status.IsUpcoming() || status == ScheduledSessionStatus.InProgress;

    /// <summary>Każdy wariant odwołania - także oczekiwanie na nowy termin.</summary>
    public static bool IsCancelled(this ScheduledSessionStatus status) => status
        is ScheduledSessionStatus.Cancelled
        or ScheduledSessionStatus.CancelledByInstructor
        or ScheduledSessionStatus.CancelledByParent
        or ScheduledSessionStatus.AwaitingReschedule;

    /// <summary>
    /// Czy zajęcia liczą się do frekwencji jako odbyte.
    ///
    /// Świadomie tylko `Completed`. Awarii technicznej po naszej stronie ani zajęć
    /// niezrealizowanych nie wliczamy - dziecko nie może tracić frekwencji za to,
    /// że zajęcia się nie odbyły.
    /// </summary>
    public static bool CountsAsHeld(this ScheduledSessionStatus status) =>
        status == ScheduledSessionStatus.Completed;

    /// <summary>Stan końcowy - nie da się już przełożyć ani poprowadzić.</summary>
    public static bool IsClosed(this ScheduledSessionStatus status) =>
        status.IsCancelled()
        || status is ScheduledSessionStatus.Completed
        or ScheduledSessionStatus.TechnicalFailure
        or ScheduledSessionStatus.NotDelivered;

    public static string Name(this ScheduledSessionStatus status) => status.ToString().ToLowerInvariant();

    public static string Label(this ScheduledSessionStatus status) => status switch
    {
        ScheduledSessionStatus.Planned => "Zaplanowane",
        ScheduledSessionStatus.Confirmed => "Potwierdzone",
        ScheduledSessionStatus.InProgress => "W toku",
        ScheduledSessionStatus.Completed => "Zakończone",
        ScheduledSessionStatus.Cancelled => "Odwołane",
        ScheduledSessionStatus.CancelledByInstructor => "Odwołane przez instruktora",
        ScheduledSessionStatus.CancelledByParent => "Odwołane przez rodzica",
        ScheduledSessionStatus.TechnicalFailure => "Przerwane technicznie",
        ScheduledSessionStatus.NotDelivered => "Niezrealizowane",
        ScheduledSessionStatus.AwaitingReschedule => "Czeka na nowy termin",
        _ => status.ToString()
    };
}

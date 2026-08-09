namespace LessonRunner.Domain.Groups;

/// <summary>
/// Znacznik pracy dziecka **w trakcie trwających zajęć**.
///
/// Rozdział 4 dokumentu koncepcyjnego opisuje to jako podstawowe narzędzie przy różnym tempie
/// dzieci: jedno kończy zadanie po dziesięciu minutach, inne po czterdziestu, a instruktor
/// traci orientację, komu pomógł i kto czeka. To jest właśnie ta funkcja, która odróżnia
/// platformę do prowadzenia zajęć od kalendarza z listą obecności.
///
/// Świadomie **nie jest to ocena ani ślad w dorobku dziecka**. Znacznik żyje tylko na czas
/// jednych zajęć i nie trafia do portalu rodzica: „potrzebuje pomocy" jest informacją
/// organizacyjną dla prowadzącego, a pokazane rodzicowi zamieniłoby się w etykietę.
/// </summary>
public enum LiveWorkStatus
{
    /// <summary>Stan domyślny: dziecko pracuje, nic nie wymaga reakcji.</summary>
    Working = 0,

    /// <summary>Zgłosiło, że utknęło - do kolejki pomocy.</summary>
    NeedsHelp = 1,

    /// <summary>Skończyło zadanie - kandydat do zadania dodatkowego.</summary>
    Finished = 2,

    /// <summary>Problem techniczny: brak dźwięku, program się nie uruchamia, zerwane łącze.</summary>
    Blocked = 3
}

public static class LiveWorkStatusExtensions
{
    public static string Label(this LiveWorkStatus status) => status switch
    {
        LiveWorkStatus.NeedsHelp => "Potrzebuje pomocy",
        LiveWorkStatus.Finished => "Skończył zadanie",
        LiveWorkStatus.Blocked => "Problem techniczny",
        _ => "Pracuje"
    };

    public static string Name(this LiveWorkStatus status) => status.ToString().ToLowerInvariant();

    /// <summary>
    /// Kolejność w panelu prowadzenia: najpierw ci, którzy czekają.
    ///
    /// Problem techniczny idzie przed zwykłą prośbą o pomoc, bo dziecko bez działającego
    /// programu nie robi nic, a to najdroższa minuta na zajęciach z programowania.
    /// </summary>
    public static int Priority(this LiveWorkStatus status) => status switch
    {
        LiveWorkStatus.Blocked => 0,
        LiveWorkStatus.NeedsHelp => 1,
        LiveWorkStatus.Finished => 2,
        _ => 3
    };
}

namespace LessonRunner.Domain.Progress;

/// <summary>
/// Skala samodzielności z rozdziału 11 dokumentu koncepcyjnego.
///
/// Celowo **nie jest to ocena tego, czy projekt działa**. Dziecko, które skopiowało gotowe
/// rozwiązanie, ma działający projekt i zerowe zrozumienie; dziecko, które samo znalazło błąd
/// w niedziałającym kodzie, nauczyło się więcej. Skala mierzy to drugie.
///
/// Wartości są numerowane rosnąco, żeby dało się je porównywać i pokazać kierunek zmiany.
/// </summary>
public enum AutonomyLevel
{
    NeedsFullHelp = 1,
    WithHelp = 2,
    Independently = 3,
    CanExplain = 4,
    CanExtend = 5
}

public static class AutonomyLevels
{
    public static string Label(AutonomyLevel level) => level switch
    {
        AutonomyLevel.NeedsFullHelp => "Wymaga pełnej pomocy",
        AutonomyLevel.WithHelp => "Wykonuje z pomocą",
        AutonomyLevel.Independently => "Wykonuje samodzielnie",
        AutonomyLevel.CanExplain => "Potrafi wyjaśnić innym",
        AutonomyLevel.CanExtend => "Potrafi rozbudować rozwiązanie",
        _ => level.ToString()
    };

    public static IReadOnlyList<AutonomyLevel> All =>
    [
        AutonomyLevel.NeedsFullHelp,
        AutonomyLevel.WithHelp,
        AutonomyLevel.Independently,
        AutonomyLevel.CanExplain,
        AutonomyLevel.CanExtend
    ];

    public static AutonomyLevel Parse(string? value) =>
        Enum.TryParse<AutonomyLevel>(value, ignoreCase: true, out var parsed) && All.Contains(parsed)
            ? parsed
            : AutonomyLevel.WithHelp;
}

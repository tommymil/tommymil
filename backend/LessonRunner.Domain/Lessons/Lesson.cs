using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Lessons;

public sealed class Lesson : Entity
{
    /// <summary>Format zajęć. Brak pola w starszym dokumencie JSON daje wartość Standard.</summary>
    public LessonKind Kind { get; set; } = LessonKind.Standard;
    public required string Title { get; set; }
    public required string Subject { get; set; }
    public required string Level { get; set; }
    public required string Description { get; set; }
    public int Order { get; set; }
    public LessonStatus Status { get; set; } = LessonStatus.Draft;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<string> Tags { get; set; } = [];
    public List<LessonStep> Steps { get; set; } = [];
    public LessonProjectFiles ProjectFiles { get; set; } = new();

    /// <summary>
    /// Po co są te zajęcia — jedno zdanie dla prowadzącego.
    ///
    /// `Description` opisuje lekcję na zewnątrz (biblioteka, oferta). To jest cel dydaktyczny:
    /// czego dziecko ma się nauczyć. Do tej pory nie było go gdzie zapisać, więc prowadzący
    /// dostawał scenariusz bez odpowiedzi, co jest w nim najważniejsze.
    /// </summary>
    public string? Objective { get; set; }

    /// <summary>Po zajęciach dziecko potrafi… — sprawdzalne kryteria, nie życzenia.
    /// Są jednocześnie gotowym szkieletem notatki dla rodzica.</summary>
    public List<string> SuccessCriteria { get; set; } = [];

    /// <summary>Co przygotować przed zajęciami: plik startowy, konto, sprawdzony dźwięk.
    /// Rzeczy, które odkryte w trakcie lekcji kosztują pięć minut ciszy.</summary>
    public List<string> Preparation { get; set; } = [];

    /// <summary>Zadanie domowe / co pokazać rodzicom po zajęciach.</summary>
    public List<string> Homework { get; set; } = [];
}

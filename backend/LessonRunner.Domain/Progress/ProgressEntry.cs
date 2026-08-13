using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Progress;

/// <summary>
/// Wpis o postępie dziecka — jeden na dziecko na termin.
///
/// To jest odpowiedź na najczęstsze pytanie rodzica („czego dziecko się nauczyło?”) i na
/// najtrudniejszą reklamację („moje dziecko niczego się nie nauczyło”). Bez takich wpisów
/// jedyne, co mamy po pół roku zajęć, to lista obecności.
///
/// **Notatka jest jedna i z definicji widoczna dla rodzica.** Świadomie nie ma tu drugiego,
/// „wewnętrznego” pola: notatka, o której trzeba pamiętać, że jej nie widać, prędzej czy później
/// zostanie pokazana. Uwagi wyłącznie dla zespołu wpisuje się w notatkę terminu, która do portalu
/// rodzica nie trafia w ogóle.
/// </summary>
public sealed class ProgressEntry : Entity
{
    public required Guid ParticipantId { get; init; }

    /// <summary>Termin, którego dotyczy wpis. Puste = podsumowanie okresowe spoza konkretnych zajęć.</summary>
    public Guid? SessionId { get; init; }

    public Guid? GroupId { get; init; }
    public Guid? LessonId { get; init; }

    public AutonomyLevel Autonomy { get; set; } = AutonomyLevel.WithHelp;

    /// <summary>Czy dziecko domknęło materiał tej lekcji. Osobno od samodzielności: można
    /// skończyć z pomocą i nie skończyć samodzielnie.</summary>
    public bool LessonCompleted { get; set; }

    /// <summary>Widoczne dla rodzica. Puste jest w porządku - wpis bez notatki nadal niesie
    /// poziom samodzielności.</summary>
    public string? NoteForParent { get; set; }

    /// <summary>Co do poprawy na kolejne zajęcia. Też widoczne dla rodzica - inaczej rodzic
    /// nie ma jak pomóc w domu.</summary>
    public string? NextStep { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid? AuthorUserId { get; set; }
}

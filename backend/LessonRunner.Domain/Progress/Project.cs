using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Progress;

/// <summary>
/// Projekt dziecka. Odpowiedź na reklamację „projekt dziecka zniknął" z rozdziału 12.
///
/// Projekt jest trwały, a jego kolejne wersje dopisujemy jako <see cref="ProjectSubmission"/> —
/// nigdy nie nadpisujemy poprzedniej. Nadpisywanie byłoby wygodniejsze i dokładnie w tym miejscu
/// zawodziłoby: „zniknął" prawie zawsze znaczy „został zastąpiony gorszą wersją".
/// </summary>
public sealed class Project : Entity
{
    public required Guid ParticipantId { get; init; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid? GroupId { get; init; }
    public Guid? LessonId { get; init; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid? CreatedByUserId { get; init; }

    public List<ProjectSubmission> Submissions { get; init; } = [];
}

/// <summary>Jedna wersja projektu: link albo plik. Wpisów nie edytujemy ani nie kasujemy —
/// komentarz instruktora jest jedynym polem, które można uzupełnić po fakcie.</summary>
public sealed class ProjectSubmission : Entity
{
    public required Guid ProjectId { get; init; }

    /// <summary>Numer wersji w obrębie projektu, liczony od 1.</summary>
    public required int Version { get; init; }

    /// <summary>Link do projektu (Scratch, repozytorium, dysk). Wypełniony wtedy, gdy nie ma pliku.</summary>
    public string? Url { get; init; }

    public string? FileUrl { get; init; }
    public string? FileName { get; init; }
    public string? ContentType { get; init; }
    public long? SizeBytes { get; init; }

    /// <summary>Nieodgadywalny klucz do pobrania pliku przez rodzica. Link do pobrania nie może
    /// nieść nagłówka `Authorization`, więc uprawnienie musi siedzieć w samym adresie —
    /// ten sam kompromis, co przy plikach projektów lekcji.</summary>
    public string? DownloadToken { get; init; }

    public DateTimeOffset SubmittedAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid? SubmittedByUserId { get; init; }

    /// <summary>Komentarz instruktora do tej wersji. Widoczny dla rodzica.</summary>
    public string? InstructorComment { get; set; }

    public bool HasFile => !string.IsNullOrWhiteSpace(FileUrl);
}

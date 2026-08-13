using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Groups;

/// <summary>Rodzaj zmiany zapisanej w historii terminu.</summary>
public enum SessionChangeType
{
    /// <summary>Termin dopisany do grupy.</summary>
    Created = 0,

    /// <summary>Zmiana daty lub godziny.</summary>
    Rescheduled = 1,

    Cancelled = 2,

    /// <summary>Przypisanie albo zdjęcie instruktora zastępczego.</summary>
    SubstituteChanged = 3,

    /// <summary>Zmiana linku do spotkania lub nagrania.</summary>
    LinksChanged = 4,

    /// <summary>Ręczna zmiana statusu (potwierdzenie, awaria techniczna, niezrealizowane).</summary>
    StatusChanged = 5
}

/// <summary>
/// Pojedynczy wpis w historii zmian terminu.
///
/// Powód istnienia jest prozaiczny: przy zdaniu „nie dostaliśmy informacji o zmianie” trzeba
/// mieć czym odpowiedzieć. Nadpisanie `ScheduledAt` nie zostawia śladu, kto, kiedy i dlaczego
/// przesunął zajęcia — a to najczęstsze źródło sporów z rodzicami.
///
/// Wpisy są tylko do dopisywania; nie edytujemy ich ani nie kasujemy.
/// </summary>
public sealed class SessionChangeLog : Entity
{
    public Guid ScheduledSessionId { get; set; }
    public Guid GroupId { get; set; }
    public SessionChangeType ChangeType { get; set; }

    /// <summary>Termin przed zmianą i po zmianie. Przy zmianach niedotyczących daty oba są null.</summary>
    public DateTimeOffset? PreviousScheduledAt { get; set; }
    public DateTimeOffset? NewScheduledAt { get; set; }

    /// <summary>Powód podany przez osobę wykonującą zmianę.</summary>
    public string? Reason { get; set; }

    /// <summary>Dodatkowy opis maszynowy, np. „zastępstwo: Anna Nowak”.</summary>
    public string? Details { get; set; }

    /// <summary>Kto dokonał zmiany. Null oznacza operację systemową (np. generator terminów).</summary>
    public Guid? ChangedByUserId { get; set; }

    /// <summary>Czy przy tej zmianie poszła informacja do opiekunów.
    /// Na razie ustawiane ręcznie przez wywołujący serwis; docelowo z modułu powiadomień.</summary>
    public bool GuardiansNotified { get; set; }

    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
}

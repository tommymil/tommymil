using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Parents;

/// <summary>
/// Powiązanie konta opiekuna z dzieckiem.
///
/// To jest **docelowe źródło prawdy o opiekunie**. Dane wpisane wprost przy dziecku
/// (`Participant.GuardianName/Email/Phone`) zostają jako zapas dla rodzin, które nie mają
/// jeszcze konta — ale gdy istnieje choć jedno powiązanie, wygrywa ono i tylko ono.
///
/// Relacja jest osobną encją, a nie kolumnami przy dziecku, bo opiekunów bywa dwoje
/// (i bywają skonfliktowani), a każde z nich może chcieć innych powiadomień.
/// </summary>
public sealed class ParentParticipantLink : Entity
{
    public Guid ParentUserId { get; set; }
    public Guid ParticipantId { get; set; }

    /// <summary>Kim jest dla dziecka: „mama”, „tata”, „opiekun prawny”, „babcia”.</summary>
    public string? Relation { get; set; }

    /// <summary>Kontakt pierwszego wyboru — do niego dzwonimy, gdy trzeba zadzwonić do jednej osoby.</summary>
    public bool IsPrimaryContact { get; set; }

    /// <summary>Czy ten opiekun ma dostawać powiadomienia e-mail. Domyślnie tak.
    /// Wyłączenie przydaje się, gdy drugi rodzic prosi o nieprzysyłanie wiadomości.</summary>
    public bool ReceivesNotifications { get; set; } = true;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}

using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Support;

/// <summary>
/// Kategoria problemu technicznego — podział wprost z rozdziału 3 dokumentu koncepcyjnego.
///
/// Kategorie są celowo grube. Przy zajęciach online liczy się to, czy problem powtarza się
/// u tego samego dziecka i czy da się go rozwiązać przed następnymi zajęciami — a nie
/// precyzyjna taksonomia usterek.
/// </summary>
public enum SupportCategory
{
    /// <summary>Słabe łącze, zerwane spotkanie.</summary>
    Connection = 0,

    /// <summary>Brak lub awaria mikrofonu, kamery, echo, sprzężenie.</summary>
    AudioVideo = 1,

    /// <summary>Wolny komputer, brak miejsca, aktualizacja, rozładowana bateria, telefon zamiast komputera.</summary>
    Device = 2,

    /// <summary>Brak programu, nieaktualna wersja, blokada instalacji, antywirus, uprawnienia administratora.</summary>
    Software = 3,

    /// <summary>Zapomniane hasło, zablokowane konto, brak dostępu do poczty, wygasła licencja.</summary>
    Account = 4,

    /// <summary>Usunięty, nadpisany, uszkodzony albo niezsynchronizowany projekt.</summary>
    ProjectFile = 5,

    Other = 99
}

public enum SupportTicketStatus
{
    Open = 0,

    /// <summary>Ktoś się tym zajmuje.</summary>
    InProgress = 1,

    Resolved = 2,

    /// <summary>Problem ustąpił sam albo zgłoszenie było pomyłką.</summary>
    Closed = 3
}

/// <summary>
/// Zgłoszenie techniczne — rozdział 3 dokumentu koncepcyjnego.
///
/// Dokument poświęca temu obszarowi cały rozdział z jednego powodu: przy zajęciach
/// z programowania problemy techniczne są **częstsze niż na zwykłych korepetycjach**
/// i to one zjadają czas lekcji. Bez rejestru każde zajęcia zaczynają się od tego samego
/// pytania „co ci znowu nie działa”, bo nikt nie pamięta, że u tego dziecka Roblox nie
/// instaluje się przez kontrolę rodzicielską.
///
/// Dlatego kluczowe jest tu **powiązanie z dzieckiem**, a nie z terminem: wartość tego
/// modułu bierze się z historii („trzeci raz to samo”), a nie z pojedynczego zgłoszenia.
/// Powiązanie z terminem jest opcjonalne i służy tylko do odtworzenia kontekstu.
///
/// Świadomie **osobna encja od incydentu**: zepsuty mikrofon i nękanie dziecka nie mają
/// ze sobą nic wspólnego poza tym, że oba są „zdarzeniem”. Wspólna tabela wymuszałaby
/// wspólne uprawnienia, a te muszą się różnić.
/// </summary>
public sealed class SupportTicket : Entity
{
    /// <summary>Czyjego sprzętu dotyczy. Null = problem po stronie szkoły albo platformy.</summary>
    public Guid? ParticipantId { get; set; }

    /// <summary>Termin, na którym problem wystąpił. Opcjonalny — kontekst, nie klucz.</summary>
    public Guid? SessionId { get; set; }

    public Guid? GroupId { get; set; }

    public SupportCategory Category { get; set; } = SupportCategory.Other;
    public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;

    public required string Description { get; set; }

    /// <summary>Co pomogło. To jest właściwa wartość rejestru — przy powtórce nie zaczynamy od zera.</summary>
    public string? Resolution { get; set; }

    /// <summary>
    /// Czy problem kosztował dziecko część zajęć.
    ///
    /// Osobne pole, bo to ono decyduje o rozliczeniu: „awaria techniczna po stronie
    /// uczestnika” i „awaria po stronie organizatora” to w rozdziale 7 dwa różne przypadki,
    /// a jeden z nich uzasadnia kredyt.
    /// </summary>
    public bool CostLessonTime { get; set; }

    public Guid ReportedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ResolvedAt { get; set; }
}

public static class SupportTicketExtensions
{
    public static string Name(this SupportCategory category) => category.ToString().ToLowerInvariant();
    public static string Name(this SupportTicketStatus status) => status.ToString().ToLowerInvariant();

    public static string Label(this SupportCategory category) => category switch
    {
        SupportCategory.Connection => "Połączenie",
        SupportCategory.AudioVideo => "Dźwięk lub kamera",
        SupportCategory.Device => "Sprzęt",
        SupportCategory.Software => "Program lub instalacja",
        SupportCategory.Account => "Konto lub hasło",
        SupportCategory.ProjectFile => "Plik projektu",
        _ => "Inne"
    };

    public static string Label(this SupportTicketStatus status) => status switch
    {
        SupportTicketStatus.Open => "Otwarte",
        SupportTicketStatus.InProgress => "W toku",
        SupportTicketStatus.Resolved => "Rozwiązane",
        SupportTicketStatus.Closed => "Zamknięte",
        _ => status.ToString()
    };

    public static bool IsOpen(this SupportTicketStatus status) =>
        status is SupportTicketStatus.Open or SupportTicketStatus.InProgress;
}

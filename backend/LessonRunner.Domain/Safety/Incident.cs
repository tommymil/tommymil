using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Safety;

/// <summary>Rodzaj incydentu — lista wprost z rozdziału 8 dokumentu koncepcyjnego.</summary>
public enum IncidentKind
{
    /// <summary>Nieznana osoba dołączyła do spotkania.</summary>
    UnknownParticipant = 0,

    /// <summary>Dziecko udostępniło adres, numer telefonu albo inne dane osobowe.</summary>
    DataDisclosure = 1,

    /// <summary>Uczestnik pokazał nieodpowiednią treść.</summary>
    InappropriateContent = 2,

    /// <summary>Wyśmiewanie, nękanie, celowe psucie pracy innych.</summary>
    Harassment = 3,

    /// <summary>Nagranie zajęć opublikowane poza systemem albo nagrywanie bez wiedzy innych.</summary>
    RecordingMisuse = 4,

    /// <summary>Dziecko zgłosiło niepokojącą sytuację domową.</summary>
    ChildWelfareConcern = 5,

    /// <summary>Zastrzeżenia do zachowania osoby prowadzącej.</summary>
    StaffConduct = 6,

    Other = 99
}

public enum IncidentSeverity
{
    /// <summary>Do odnotowania, bez pilnych działań.</summary>
    Low = 0,

    /// <summary>Wymaga reakcji w tym tygodniu.</summary>
    Medium = 1,

    /// <summary>Wymaga reakcji dziś — bezpieczeństwo dziecka albo ryzyko prawne.</summary>
    High = 2
}

public enum IncidentStatus
{
    Reported = 0,
    InReview = 1,
    Resolved = 2,

    /// <summary>Zamknięty bez działań — zgłoszenie okazało się nieuzasadnione.</summary>
    Dismissed = 3
}

/// <summary>
/// Incydent dotyczący bezpieczeństwa albo zachowania — rozdział 8 dokumentu koncepcyjnego.
///
/// **Najważniejsza zasada tego modułu:** dokument mówi wprost, że takich notatek *nie należy
/// mieszać ze zwykłymi uwagami edukacyjnymi*. Dlatego incydent jest osobną encją, a nie polem
/// przy obecności czy wpisem postępu. Ma to trzy praktyczne konsekwencje:
///
/// 1. **Nie trafia do portalu rodzica.** Nigdy, w żadnej formie. O incydencie informuje
///    człowiek, w rozmowie — nie powiadomienie e-mail wygenerowane z rekordu.
/// 2. **Instruktor widzi wyłącznie własne zgłoszenia.** Zgłoszenie o zachowaniu personelu
///    (`StaffConduct`) byłoby bezwartościowe, gdyby osoba, której dotyczy, mogła je czytać.
/// 3. **Osobne uprawnienia od reszty systemu.** Zgłosić może każdy pracownik, prowadzić
///    sprawę — tylko administrator.
///
/// Powiązania z grupą, terminem i dzieckiem są **opcjonalne i bez kluczy obcych**: incydent
/// ma przetrwać usunięcie grupy albo anonimizację dziecka. To rejestr zdarzeń, a nie część
/// dziennika zajęć.
/// </summary>
public sealed class Incident : Entity
{
    public IncidentKind Kind { get; set; } = IncidentKind.Other;
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Medium;
    public IncidentStatus Status { get; set; } = IncidentStatus.Reported;

    /// <summary>Kiedy zdarzenie miało miejsce — nie mylić z datą zgłoszenia.</summary>
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;

    public Guid? GroupId { get; set; }
    public Guid? SessionId { get; set; }

    /// <summary>
    /// Dzieci, których incydent dotyczy — lista identyfikatorów rozdzielona przecinkami.
    ///
    /// Świadomie bez tabeli łączącej: incydent dotyczy najczęściej jednego albo dwojga dzieci,
    /// a jedyne zapytanie, jakiego potrzebujemy, to „pokaż incydenty tego dziecka”. Osobna
    /// tabela dokładałaby złączenie i migrację bez żadnej nowej możliwości.
    /// </summary>
    public string ParticipantIds { get; set; } = string.Empty;

    /// <summary>Co się stało. Pole wymagane — incydent bez opisu jest bezużyteczny.</summary>
    public required string Description { get; set; }

    /// <summary>Co zrobiono. Uzupełniane w trakcie prowadzenia sprawy.</summary>
    public string? ActionsTaken { get; set; }

    /// <summary>Jak sprawa się skończyła. Wymagane przy zamknięciu.</summary>
    public string? Resolution { get; set; }

    public Guid ReportedByUserId { get; set; }

    /// <summary>Osoba odpowiedzialna za sprawę. Null = nikt jeszcze jej nie przejął.</summary>
    public Guid? AssignedToUserId { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ResolvedAt { get; set; }

    /// <summary>Identyfikatory dzieci w formie listy.</summary>
    public IReadOnlyList<Guid> ParticipantIdList() =>
        ParticipantIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => Guid.TryParse(value, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToList();

    public void SetParticipantIds(IEnumerable<Guid> ids) =>
        ParticipantIds = string.Join(',', ids.Distinct().Where(id => id != Guid.Empty));
}

public static class IncidentExtensions
{
    public static string Name(this IncidentKind kind) => kind.ToString().ToLowerInvariant();
    public static string Name(this IncidentSeverity severity) => severity.ToString().ToLowerInvariant();
    public static string Name(this IncidentStatus status) => status.ToString().ToLowerInvariant();

    public static string Label(this IncidentKind kind) => kind switch
    {
        IncidentKind.UnknownParticipant => "Nieznana osoba na spotkaniu",
        IncidentKind.DataDisclosure => "Ujawnienie danych osobowych",
        IncidentKind.InappropriateContent => "Nieodpowiednia treść",
        IncidentKind.Harassment => "Nękanie lub wyśmiewanie",
        IncidentKind.RecordingMisuse => "Nagranie poza systemem",
        IncidentKind.ChildWelfareConcern => "Niepokojąca sytuacja dziecka",
        IncidentKind.StaffConduct => "Zachowanie osoby prowadzącej",
        _ => "Inne"
    };

    public static string Label(this IncidentSeverity severity) => severity switch
    {
        IncidentSeverity.Low => "Niska",
        IncidentSeverity.High => "Wysoka",
        _ => "Średnia"
    };

    public static string Label(this IncidentStatus status) => status switch
    {
        IncidentStatus.Reported => "Zgłoszony",
        IncidentStatus.InReview => "W toku",
        IncidentStatus.Resolved => "Rozwiązany",
        IncidentStatus.Dismissed => "Zamknięty bez działań",
        _ => status.ToString()
    };

    /// <summary>Sprawa zamknięta — nie wymaga już niczyjej uwagi.</summary>
    public static bool IsClosed(this IncidentStatus status) =>
        status is IncidentStatus.Resolved or IncidentStatus.Dismissed;

    /// <summary>
    /// Sprawy dotyczące dobra dziecka i zachowania personelu są z definicji poważne —
    /// niezależnie od tego, jaką wagę wybrał zgłaszający. Nie chcemy, żeby zgłoszenie
    /// „dziecko powiedziało coś niepokojącego o domu” wylądowało na dole listy dlatego,
    /// że ktoś w pośpiechu zostawił domyślną wagę.
    /// </summary>
    public static bool NeedsImmediateAttention(this Incident incident) =>
        !incident.Status.IsClosed()
        && (incident.Severity == IncidentSeverity.High
            || incident.Kind is IncidentKind.ChildWelfareConcern or IncidentKind.StaffConduct);
}

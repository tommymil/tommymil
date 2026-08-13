using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Trials;

/// <summary>Droga zgłoszenia od telefonu do zapisu. Kolejność statusów jest jednokierunkowa
/// poza zamknięciem — z każdego etapu można wypaść na „Rezygnacja”.</summary>
public enum TrialStatus
{
    /// <summary>Zgłoszenie przyjęte, nie ma jeszcze terminu ani instruktora.</summary>
    Requested = 0,

    /// <summary>Termin ustalony, instruktor przypisany.</summary>
    Scheduled = 1,

    /// <summary>Lekcja się odbyła i instruktor wypełnił diagnozę.</summary>
    Diagnosed = 2,

    /// <summary>Dziecko trafiło do bazy uczestników.</summary>
    Enrolled = 3,

    /// <summary>Rodzina zrezygnowała albo my odmówiliśmy.</summary>
    Declined = 4,

    /// <summary>Nikt się nie pojawił na umówionym terminie.</summary>
    NoShow = 5
}

/// <summary>Czy dziecko czyta na tyle sprawnie, żeby samodzielnie korzystać z instrukcji.</summary>
public enum ReadingSkill
{
    Unknown = 0,
    Fluent = 1,
    Slow = 2,
    NotYet = 3
}

/// <summary>Obsługa komputera: mysz, klawiatura, okna. Najczęstsza realna bariera.</summary>
public enum ComputerSkill
{
    Unknown = 0,
    Confident = 1,
    Basic = 2,
    NeedsHelp = 3
}

public enum ProgrammingBackground
{
    Unknown = 0,
    None = 1,
    Blocks = 2,
    Text = 3
}

/// <summary>Rekomendacja instruktora po lekcji — podstawa decyzji administracji.</summary>
public enum TrialRecommendation
{
    Undecided = 0,

    /// <summary>Można zapisywać na standardowy kurs.</summary>
    Ready = 1,

    /// <summary>Poradzi sobie, ale potrzebuje mniejszej grupy albo wsparcia na starcie.</summary>
    ReadyWithSupport = 2,

    /// <summary>Za wcześnie — wiek, czytanie albo obsługa komputera.</summary>
    TooEarly = 3,

    /// <summary>Zainteresowanie jest, ale nie ten kurs (inny poziom albo inna technologia).</summary>
    DifferentTrack = 4
}

/// <summary>
/// Lekcja próbna 1:1 — pierwsze spotkanie z dzieckiem, które jeszcze nie jest uczestnikiem.
///
/// Świadomie **osobna encja, a nie grupa jednoosobowa**. Grupa niosłaby ze sobą uczestnika,
/// a uczestnika nie ma i nie chcemy go tworzyć na zapas: dopóki rodzina nie zdecyduje,
/// dziecko nie ma po co pojawiać się w bazie uczestników, na listach frekwencji ani
/// w rozliczeniach. Dane kandydata żyją więc tutaj, przy zgłoszeniu, i przenoszą się do
/// `Participant` dopiero przy zapisie.
///
/// Druga konsekwencja tej samej zasady: lekcja próbna nie generuje terminu w grafiku grup.
/// Instruktor widzi ją w osobnej sekcji, bo prowadzi się ją inaczej — to badanie, nie zajęcia.
/// </summary>
public sealed class TrialLesson : Entity
{
    // --- Kandydat: dane, które podał rodzic przy zgłoszeniu -------------------------------

    public required string ChildFirstName { get; set; }
    public required string ChildLastName { get; set; }
    public DateOnly? ChildBirthDate { get; set; }

    public string? GuardianName { get; set; }
    public string? GuardianEmail { get; set; }
    public string? GuardianPhone { get; set; }

    /// <summary>Skąd rodzina o nas wie — polecenie, reklama, szkoła. Bez tego nie da się
    /// powiedzieć, który kanał faktycznie przyprowadza dzieci.</summary>
    public string? Source { get; set; }

    /// <summary>Co rodzic powiedział przy zgłoszeniu. Wolny tekst, bo to notatka z rozmowy.</summary>
    public string? RequestNote { get; set; }

    // --- Umówienie -----------------------------------------------------------------------

    public Guid? InstructorId { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }
    public string? MeetingUrl { get; set; }

    /// <summary>Konspekt użyty jako scenariusz badania. Opcjonalny — część instruktorów
    /// prowadzi próbną z gotowej lekcji, część z własnego zestawu zadań.</summary>
    public Guid? LessonId { get; set; }

    // --- Diagnoza: wypełnia instruktor po lekcji ------------------------------------------

    public ReadingSkill Reading { get; set; } = ReadingSkill.Unknown;
    public ComputerSkill Computer { get; set; } = ComputerSkill.Unknown;
    public ProgrammingBackground Programming { get; set; } = ProgrammingBackground.Unknown;
    public TrialRecommendation Recommendation { get; set; } = TrialRecommendation.Undecided;

    /// <summary>Poziom albo kurs, od którego zdaniem instruktora warto zacząć.</summary>
    public string? RecommendedLevel { get; set; }

    /// <summary>Obserwacje z lekcji. To jedyne pole, które trafia do notatek uczestnika
    /// przy zapisie — reszta diagnozy zostaje przy zgłoszeniu.</summary>
    public string? DiagnosisNote { get; set; }

    public DateTimeOffset? DiagnosedAt { get; set; }
    public Guid? DiagnosedByUserId { get; set; }

    // --- Rozstrzygnięcie -------------------------------------------------------------------

    public TrialStatus Status { get; set; } = TrialStatus.Requested;

    /// <summary>Uczestnik utworzony przy zapisie. Zostaje jako ślad, skąd dziecko przyszło.</summary>
    public Guid? ParticipantId { get; set; }

    public string? DeclineReason { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ClosedAt { get; set; }

    public string ChildName => $"{ChildFirstName} {ChildLastName}".Trim();

    /// <summary>Diagnoza jest kompletna, gdy instruktor odpowiedział na wszystkie trzy
    /// pytania i postawił rekomendację. Bez tego administracja nie ma na czym oprzeć decyzji.</summary>
    public bool HasDiagnosis =>
        Reading != ReadingSkill.Unknown
        && Computer != ComputerSkill.Unknown
        && Programming != ProgrammingBackground.Unknown
        && Recommendation != TrialRecommendation.Undecided;
}

public static class TrialExtensions
{
    public static string Name(this TrialStatus status) => status.ToString().ToLowerInvariant();
    public static string Name(this ReadingSkill value) => value.ToString().ToLowerInvariant();
    public static string Name(this ComputerSkill value) => value.ToString().ToLowerInvariant();
    public static string Name(this ProgrammingBackground value) => value.ToString().ToLowerInvariant();
    public static string Name(this TrialRecommendation value) => value.ToString().ToLowerInvariant();

    public static string Label(this TrialStatus status) => status switch
    {
        TrialStatus.Requested => "Zgłoszenie",
        TrialStatus.Scheduled => "Umówiona",
        TrialStatus.Diagnosed => "Po lekcji",
        TrialStatus.Enrolled => "Zapisany",
        TrialStatus.Declined => "Rezygnacja",
        TrialStatus.NoShow => "Nie pojawił się",
        _ => status.ToString()
    };

    public static string Label(this ReadingSkill value) => value switch
    {
        ReadingSkill.Fluent => "Czyta płynnie",
        ReadingSkill.Slow => "Czyta wolno",
        ReadingSkill.NotYet => "Jeszcze nie czyta",
        _ => "Nie sprawdzono"
    };

    public static string Label(this ComputerSkill value) => value switch
    {
        ComputerSkill.Confident => "Swobodnie obsługuje komputer",
        ComputerSkill.Basic => "Podstawy obsługi",
        ComputerSkill.NeedsHelp => "Potrzebuje pomocy przy komputerze",
        _ => "Nie sprawdzono"
    };

    public static string Label(this ProgrammingBackground value) => value switch
    {
        ProgrammingBackground.None => "Bez doświadczenia",
        ProgrammingBackground.Blocks => "Zna bloczki (Scratch)",
        ProgrammingBackground.Text => "Programował tekstowo",
        _ => "Nie sprawdzono"
    };

    public static string Label(this TrialRecommendation value) => value switch
    {
        TrialRecommendation.Ready => "Można zapisać",
        TrialRecommendation.ReadyWithSupport => "Można zapisać ze wsparciem",
        TrialRecommendation.TooEarly => "Za wcześnie",
        TrialRecommendation.DifferentTrack => "Inny poziom lub kurs",
        _ => "Bez rekomendacji"
    };

    /// <summary>Sprawa zamknięta — nie czeka już na niczyją decyzję.</summary>
    public static bool IsClosed(this TrialStatus status) =>
        status is TrialStatus.Enrolled or TrialStatus.Declined or TrialStatus.NoShow;

    /// <summary>
    /// Zgłoszenie czeka na ruch administracji.
    ///
    /// To jedyna liczba, po którą warto wchodzić na listę: świeże zgłoszenie bez terminu
    /// i dziecko po lekcji, które czeka na decyzję. Reszta albo jest umówiona, albo zamknięta.
    /// </summary>
    public static bool NeedsAttention(this TrialLesson trial) =>
        trial.Status is TrialStatus.Requested or TrialStatus.Diagnosed;
}

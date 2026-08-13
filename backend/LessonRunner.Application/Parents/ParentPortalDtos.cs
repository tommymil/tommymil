namespace LessonRunner.Application.Parents;

public sealed record ParentChildDto(
    Guid ParticipantId,
    string FirstName,
    string LastName,
    IReadOnlyList<ParentChildGroupDto> Groups);

public sealed record ParentChildGroupDto(Guid GroupId, string GroupName);

public sealed record ParentScheduleItemDto(
    Guid SessionId,
    Guid GroupId,
    string GroupName,
    string? LessonTitle,
    DateTimeOffset ScheduledAt,
    string Status,
    string StatusLabel,
    string? MeetingUrl,
    /// <summary>Dzieci tego rodzica zapisane na ten termin - do zgłoszenia nieobecności.</summary>
    IReadOnlyList<ParentSessionChildDto>? Children = null,
    /// <summary>Numer lekcji w kursie i długość kursu - „lekcja 7 z 12”.
    /// Rodzic pyta o postęp kursu częściej niż o pojedynczy termin.</summary>
    int SequenceNumber = 0,
    int CourseLength = 0,
    string? InstructorName = null);

/// <summary>Dziecko na konkretnym terminie wraz z informacją, czy zgłoszono już nieobecność.</summary>
public sealed record ParentSessionChildDto(
    Guid ParticipantId,
    string FirstName,
    string LastName,
    bool AbsenceReported,
    string? AbsenceNote);

/// <summary>Zgłoszenie nieobecności dziecka przez opiekuna.</summary>
public sealed record ReportAbsenceDto(Guid ParticipantId, string? Reason = null);

/// <summary>Materiał lekcji udostępniany rodzicowi po zakończeniu zajęć.
/// Świadomie NIE zawiera scenariusza prowadzenia ani notatek instruktora - tylko pliki
/// projektu (starter/wersja końcowa), które dziecko może otworzyć w domu.</summary>
public sealed record ParentMaterialDto(
    Guid SessionId,
    Guid GroupId,
    string GroupName,
    string? LessonTitle,
    DateTimeOffset ScheduledAt,
    IReadOnlyList<ParentMaterialFileDto> Files,
    string? RecordingUrl,
    /// <summary>Które z dzieci tego rodzica dotyczą materiału. Bez tego przy rodzeństwie
    /// nie da się przefiltrować portalu po dziecku.</summary>
    IReadOnlyList<Guid>? ParticipantIds = null,
    /// <summary>Podsumowanie zajęć napisane przez instruktora z myślą o rodzicu.
    /// Notatka wewnętrzna terminu nie przechodzi tędy w ogóle.</summary>
    string? Summary = null);

public sealed record ParentMaterialFileDto(
    string Label,
    string FileName,
    long SizeBytes,
    string DownloadUrl);

public sealed record ParentAttendanceItemDto(
    Guid GroupId,
    string GroupName,
    int PresentCount,
    int HeldCount,
    int RatePercent,
    Guid ParticipantId = default);

public sealed record ParentInvoiceDto(
    Guid InvoiceId,
    string Number,
    string GroupName,
    long AmountCents,
    string Currency,
    string Status,
    string StatusLabel,
    DateOnly DueDate,
    DateTimeOffset? PaidAt,
    Guid ParticipantId = default,
    /// <summary>Czy dokument jest po terminie płatności. Liczone przy odczycie, bo status
    /// „Overdue" w bazie zmienia się dopiero przy jakiejś operacji na fakturze.</summary>
    bool IsOverdue = false);

/// <summary>
/// Kredyt zajęciowy w wersji dla rodzica.
///
/// Kredyty istniały w systemie od 27.07.2026, ale wyłącznie w panelu administratora —
/// czyli osoba, której się należały, nie miała jak się o nich dowiedzieć.
/// </summary>
public sealed record ParentCreditDto(
    Guid CreditId,
    Guid ParticipantId,
    string ChildName,
    string? GroupName,
    string Reason,
    DateTimeOffset IssuedAt,
    DateOnly? ExpiresAt);

/// <summary>Postęp dziecka w kursie: ile lekcji za nim, ile przed nim.</summary>
public sealed record ParentCourseProgressDto(
    Guid ParticipantId,
    Guid GroupId,
    string GroupName,
    int CompletedLessons,
    int TotalLessons,
    string? NextLessonTitle,
    DateTimeOffset? NextSessionAt);

/// <summary>
/// Zgody opiekuna widziane jego oczami.
///
/// Do tej pory zgoda na przetwarzanie danych i na wizerunek były polami ustawianymi
/// wyłącznie przez administratora — rodzic ich nie widział i nie mógł wycofać,
/// mimo że przy RODO to on jest stroną udzielającą zgody.
/// </summary>
public sealed record ParentConsentDto(
    Guid ParticipantId,
    string ChildName,
    bool DataProcessing,
    DateTimeOffset? DataProcessingAt,
    bool Image,
    DateTimeOffset? ImageAt);

/// <summary>
/// Zmiana zgody przez rodzica.
///
/// Świadomie obejmuje **wyłącznie zgodę na wizerunek**. Zgoda na przetwarzanie danych
/// jest warunkiem świadczenia usługi (wysyłka powiadomień, prowadzenie dziennika) —
/// jej wycofanie to rozwiązanie umowy, a nie przełącznik w portalu, więc kierujemy
/// z tym do administracji.
/// </summary>
public sealed record UpdateParentConsentDto(Guid ParticipantId, bool ImageConsent);

/// <summary>
/// Liczby, które rodzic sprawdza naprawdę.
///
/// Wcześniej portal pokazywał „Dzieci: 1", „Najbliższe terminy: 5" i „Rozliczenia: 3" —
/// czyli liczbę faktur zamiast kwoty do zapłaty. Rodzic wie, ile ma dzieci; nie wie,
/// ile jest winien i ile zajęć zostało w pakiecie.
/// </summary>
public sealed record ParentSummaryDto(
    long OutstandingCents,
    long OverdueCents,
    string Currency,
    DateOnly? NextDueDate,
    int AvailableCredits,
    int AttendancePercent,
    ParentScheduleItemDto? NextSession);

/// <summary>Wpis o postępie w wersji dla rodzica. Zawiera **wyłącznie pola pisane z myślą
/// o rodzicu** — notatka terminu (uwagi instruktora dla zespołu) nie ma tu odpowiednika.</summary>
public sealed record ParentProgressEntryDto(
    DateTimeOffset UpdatedAt,
    string AutonomyLabel,
    int AutonomyRank,
    bool LessonCompleted,
    string? NoteForParent,
    string? NextStep);

public sealed record ParentProjectVersionDto(
    int Version,
    string? Url,
    string? FileName,
    string? DownloadUrl,
    DateTimeOffset SubmittedAt,
    string? InstructorComment);

public sealed record ParentProjectDto(
    Guid ProjectId,
    string Title,
    string? Description,
    IReadOnlyList<ParentProjectVersionDto> Versions);

public sealed record ParentChildProgressDto(
    Guid ParticipantId,
    string FirstName,
    string LastName,
    IReadOnlyList<ParentProgressEntryDto> Entries,
    IReadOnlyList<ParentProjectDto> Projects);

public sealed record ParentPortalDto(
    IReadOnlyList<ParentChildDto> Children,
    IReadOnlyList<ParentScheduleItemDto> Schedule,
    IReadOnlyList<ParentAttendanceItemDto> Attendance,
    IReadOnlyList<ParentInvoiceDto> Invoices,
    IReadOnlyList<ParentMaterialDto> Materials,
    IReadOnlyList<ParentChildProgressDto> Progress,
    ParentSummaryDto? Summary = null,
    IReadOnlyList<ParentCreditDto>? Credits = null,
    IReadOnlyList<ParentCourseProgressDto>? CourseProgress = null,
    IReadOnlyList<ParentConsentDto>? Consents = null);

public sealed record ParentParticipantLinkDto(
    Guid ParentUserId,
    Guid ParticipantId,
    /// <summary>Kim opiekun jest dla dziecka: „mama", „tata", „opiekun prawny".</summary>
    string? Relation = null,
    bool IsPrimaryContact = false,
    bool ReceivesNotifications = true);

/// <summary>
/// Wynik założenia konta opiekunowi z karty dziecka.
///
/// <paramref name="Created"/> odróżnia nowe konto od podpięcia istniejącego — przy drugim
/// dziecku tej samej rodziny zakładamy tylko powiązanie i nie ma po co wysyłać zaproszenia.
/// <paramref name="InvitationSent"/> mówi, czy poszła poczta: token powstaje także wtedy,
/// gdy wysyłka padnie, a administracja musi widzieć różnicę, zamiast czekać na rodzica,
/// który niczego nie dostał.
/// </summary>
public sealed record GuardianAccountResultDto(
    Guid ParentUserId,
    string Email,
    string DisplayName,
    bool Created,
    bool InvitationSent,
    string? Error = null);

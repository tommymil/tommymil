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
    string? MeetingUrl);

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
    string? RecordingUrl);

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
    int RatePercent);

public sealed record ParentInvoiceDto(
    Guid InvoiceId,
    string Number,
    string GroupName,
    long AmountCents,
    string Currency,
    string Status,
    string StatusLabel,
    DateOnly DueDate,
    DateTimeOffset? PaidAt);

public sealed record ParentPortalDto(
    IReadOnlyList<ParentChildDto> Children,
    IReadOnlyList<ParentScheduleItemDto> Schedule,
    IReadOnlyList<ParentAttendanceItemDto> Attendance,
    IReadOnlyList<ParentInvoiceDto> Invoices,
    IReadOnlyList<ParentMaterialDto> Materials);

public sealed record ParentParticipantLinkDto(Guid ParentUserId, Guid ParticipantId);

namespace LessonRunner.Application.Participants;

public sealed record ParticipantGroupDto(Guid GroupId, string GroupName);

public sealed record ParticipantSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    DateOnly? BirthDate,
    string? GuardianName,
    string? GuardianPhone,
    bool IsArchived,
    bool HasDataConsent,
    bool HasImageConsent,
    IReadOnlyList<ParticipantGroupDto> Groups);

public sealed record ParticipantDetailsDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    DateOnly? BirthDate,
    string? Notes,
    string? GuardianName,
    string? GuardianPhone,
    string? GuardianEmail,
    string? GuardianRelation,
    bool IsArchived,
    DateTimeOffset? ArchivedAt,
    DateTimeOffset? DataProcessingConsentAt,
    DateTimeOffset? ImageConsentAt,
    DateTimeOffset CreatedAt,
    IReadOnlyList<ParticipantGroupDto> Groups);

public sealed record CreateParticipantDto(
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    DateOnly? BirthDate = null,
    string? Notes = null,
    string? GuardianName = null,
    string? GuardianPhone = null,
    string? GuardianEmail = null,
    string? GuardianRelation = null,
    bool ConsentDataProcessing = false,
    bool ConsentImage = false,
    IReadOnlyList<Guid>? GroupIds = null);

public sealed record UpdateParticipantDto(
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    DateOnly? BirthDate = null,
    string? Notes = null,
    string? GuardianName = null,
    string? GuardianPhone = null,
    string? GuardianEmail = null,
    string? GuardianRelation = null,
    bool ConsentDataProcessing = false,
    bool ConsentImage = false);

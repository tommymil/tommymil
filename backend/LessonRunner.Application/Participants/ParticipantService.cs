using LessonRunner.Application.Groups;
using LessonRunner.Application.Parents;
using LessonRunner.Domain.Groups;
using LessonRunner.Domain.Participants;

namespace LessonRunner.Application.Participants;

public sealed class ParticipantService(
    IParticipantRepository participantRepository,
    IGroupRepository groupRepository,
    // Opcjonalne, żeby testy budujące serwis ręcznie nadal się kompilowały. Brak repozytorium
    // oznacza „nie wiemy o żadnym koncie opiekuna”, a nie wywróconą listę uczestników.
    IParentPortalRepository? parentRepository = null) : IParticipantService
{
    public async Task<IReadOnlyList<ParticipantSummaryDto>> GetSummariesAsync(
        string? query,
        bool includeArchived,
        CancellationToken cancellationToken)
    {
        var participants = await participantRepository.ListAsync(cancellationToken);
        var groupsByParticipant = BuildGroupMap(await groupRepository.ListAsync(cancellationToken));
        var normalizedQuery = (query ?? string.Empty).Trim();

        var visible = participants
            .Where(participant => includeArchived || !participant.IsArchived)
            .Where(participant => normalizedQuery.Length == 0 || Matches(participant, normalizedQuery))
            .ToList();

        var withAccount = parentRepository is null
            ? []
            : (await parentRepository.ListByParticipantsAsync(
                    visible.Select(participant => participant.Id).ToList(),
                    cancellationToken))
                .Select(link => link.ParticipantId)
                .ToHashSet();

        return visible
            .Select(participant => ToSummary(participant, groupsByParticipant, withAccount.Contains(participant.Id)))
            .ToList();
    }

    public async Task<ParticipantDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        var participant = await participantRepository.GetByIdAsync(id, cancellationToken);

        if (participant is null)
        {
            return null;
        }

        var groupsByParticipant = BuildGroupMap(await groupRepository.ListAsync(cancellationToken));
        return ToDetails(participant, groupsByParticipant);
    }

    public async Task<ParticipantDetailsDto> CreateAsync(CreateParticipantDto dto, CancellationToken cancellationToken)
    {
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            FirstName = RequireName(dto.FirstName, "imię"),
            LastName = RequireName(dto.LastName, "nazwisko")
        };

        ApplyChildFields(
            participant,
            dto.Phone, dto.Email, dto.BirthDate, dto.Notes,
            dto.GuardianName, dto.GuardianPhone, dto.GuardianEmail, dto.GuardianRelation,
            dto.ConsentDataProcessing, dto.ConsentImage);

        await participantRepository.AddAsync(participant, cancellationToken);

        foreach (var groupId in (dto.GroupIds ?? []).Distinct())
        {
            await EnrollInternalAsync(participant.Id, groupId, cancellationToken);
        }

        return await GetDetailsAsync(participant.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created participant could not be loaded.");
    }

    public async Task<ParticipantDetailsDto?> UpdateAsync(Guid id, UpdateParticipantDto dto, CancellationToken cancellationToken)
    {
        var existing = await participantRepository.GetByIdAsync(id, cancellationToken);

        if (existing is null)
        {
            return null;
        }

        existing.FirstName = RequireName(dto.FirstName, "imię");
        existing.LastName = RequireName(dto.LastName, "nazwisko");
        ApplyChildFields(
            existing,
            dto.Phone, dto.Email, dto.BirthDate, dto.Notes,
            dto.GuardianName, dto.GuardianPhone, dto.GuardianEmail, dto.GuardianRelation,
            dto.ConsentDataProcessing, dto.ConsentImage);
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await participantRepository.UpdateAsync(existing, cancellationToken);

        return await GetDetailsAsync(id, cancellationToken);
    }

    public async Task<bool> SetArchivedAsync(Guid id, bool archived, CancellationToken cancellationToken)
    {
        var existing = await participantRepository.GetByIdAsync(id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        existing.IsArchived = archived;
        existing.ArchivedAt = archived ? DateTimeOffset.UtcNow : null;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        return await participantRepository.UpdateAsync(existing, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var groups = await groupRepository.ListAsync(cancellationToken);
        var hasAttendance = groups.Any(group =>
            group.Sessions.Any(session => session.Attendance.Any(record => record.ParticipantId == id)));

        if (hasAttendance)
        {
            throw new InvalidOperationException(
                "Uczestnik ma zapisaną historię obecności - nie można go usunąć z bazy. Zamiast tego zanonimizuj lub zarchiwizuj go.");
        }

        return await participantRepository.DeleteAsync(id, cancellationToken);
    }

    /// <summary>RODO - prawo do usunięcia: kasuje dane osobowe, ale zachowuje rekord i historię
    /// obecności (statystyki grup pozostają spójne). Dane nie do odzyskania.</summary>
    public async Task<bool> AnonymizeAsync(Guid id, CancellationToken cancellationToken)
    {
        var existing = await participantRepository.GetByIdAsync(id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        existing.FirstName = "Dane";
        existing.LastName = "usunięte";
        existing.Phone = null;
        existing.Email = null;
        existing.BirthDate = null;
        existing.Notes = null;
        existing.GuardianName = null;
        existing.GuardianPhone = null;
        existing.GuardianEmail = null;
        existing.GuardianRelation = null;
        existing.DataProcessingConsentAt = null;
        existing.ImageConsentAt = null;
        existing.IsArchived = true;
        existing.ArchivedAt = DateTimeOffset.UtcNow;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        return await participantRepository.UpdateAsync(existing, cancellationToken);
    }

    public async Task<ParticipantDetailsDto?> EnrollAsync(Guid participantId, Guid groupId, CancellationToken cancellationToken)
    {
        var participant = await participantRepository.GetByIdAsync(participantId, cancellationToken);

        if (participant is null)
        {
            return null;
        }

        await EnrollInternalAsync(participantId, groupId, cancellationToken);

        return await GetDetailsAsync(participantId, cancellationToken);
    }

    public async Task<ParticipantDetailsDto?> UnenrollAsync(Guid participantId, Guid groupId, CancellationToken cancellationToken)
    {
        var participant = await participantRepository.GetByIdAsync(participantId, cancellationToken);

        if (participant is null)
        {
            return null;
        }

        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);
        var wasEnrolled = group?.Enrollments.Any(enrollment =>
            enrollment.ParticipantId == participantId && enrollment.Status == EnrollmentStatus.Enrolled) == true;

        await groupRepository.RemoveEnrollmentAsync(groupId, participantId, cancellationToken);

        if (wasEnrolled)
        {
            await PromoteFirstWaitlistedAsync(groupId, cancellationToken);
        }

        return await GetDetailsAsync(participantId, cancellationToken);
    }

    private async Task EnrollInternalAsync(Guid participantId, Guid groupId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken)
            ?? throw new ArgumentException("Wybrana grupa nie istnieje.");

        if (group.Enrollments.Any(enrollment => enrollment.ParticipantId == participantId))
        {
            return; // już zapisany - idempotentnie
        }

        var enrolledCount = group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Enrolled);
        var status = group.Capacity is not null && enrolledCount >= group.Capacity
            ? EnrollmentStatus.Waitlisted
            : EnrollmentStatus.Enrolled;

        await groupRepository.AddEnrollmentAsync(
            new GroupEnrollment { GroupId = groupId, ParticipantId = participantId, Status = status },
            cancellationToken);
    }

    private async Task PromoteFirstWaitlistedAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(groupId, cancellationToken);

        if (group?.Capacity is null)
        {
            return;
        }

        var enrolledCount = group.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Enrolled);

        if (enrolledCount >= group.Capacity)
        {
            return;
        }

        var next = group.Enrollments
            .Where(enrollment => enrollment.Status == EnrollmentStatus.Waitlisted)
            .OrderBy(enrollment => enrollment.EnrolledAt)
            .FirstOrDefault();

        if (next is not null)
        {
            await groupRepository.SetEnrollmentStatusAsync(groupId, next.ParticipantId, EnrollmentStatus.Enrolled, cancellationToken);
        }
    }

    private static void ApplyChildFields(
        Participant participant,
        string? phone,
        string? email,
        DateOnly? birthDate,
        string? notes,
        string? guardianName,
        string? guardianPhone,
        string? guardianEmail,
        string? guardianRelation,
        bool consentDataProcessing,
        bool consentImage)
    {
        if (birthDate is { } value && value > DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            throw new ArgumentException("Data urodzenia nie może być w przyszłości.");
        }

        participant.Phone = NullIfEmpty(phone);
        participant.Email = NullIfEmpty(email);
        participant.BirthDate = birthDate;
        participant.Notes = NullIfEmpty(notes);
        participant.GuardianName = NullIfEmpty(guardianName);
        participant.GuardianPhone = NullIfEmpty(guardianPhone);
        participant.GuardianEmail = NullIfEmpty(guardianEmail);
        participant.GuardianRelation = NullIfEmpty(guardianRelation);
        participant.DataProcessingConsentAt = ResolveConsent(participant.DataProcessingConsentAt, consentDataProcessing);
        participant.ImageConsentAt = ResolveConsent(participant.ImageConsentAt, consentImage);
    }

    // Podtrzymana zgoda zachowuje pierwotną datę; nowa dostaje bieżący stempel; cofnięta -> null.
    private static DateTimeOffset? ResolveConsent(DateTimeOffset? existing, bool granted) =>
        granted ? existing ?? DateTimeOffset.UtcNow : null;

    private static bool Matches(Participant participant, string query)
    {
        return Contains(participant.FirstName, query)
            || Contains(participant.LastName, query)
            || Contains(participant.Phone, query)
            || Contains(participant.Email, query)
            || Contains(participant.GuardianName, query)
            || Contains(participant.GuardianPhone, query)
            || Contains(participant.GuardianEmail, query);
    }

    private static bool Contains(string? value, string query) =>
        value is not null && value.Contains(query, StringComparison.OrdinalIgnoreCase);

    private static Dictionary<Guid, List<ParticipantGroupDto>> BuildGroupMap(IReadOnlyList<Group> groups)
    {
        var map = new Dictionary<Guid, List<ParticipantGroupDto>>();

        foreach (var group in groups)
        {
            foreach (var enrollment in group.Enrollments)
            {
                if (!map.TryGetValue(enrollment.ParticipantId, out var list))
                {
                    list = [];
                    map[enrollment.ParticipantId] = list;
                }

                list.Add(new ParticipantGroupDto(group.Id, group.Name));
            }
        }

        return map;
    }

    private static ParticipantSummaryDto ToSummary(
        Participant participant,
        Dictionary<Guid, List<ParticipantGroupDto>> groupsByParticipant,
        bool hasGuardianAccount) =>
        new(
            participant.Id,
            participant.FirstName,
            participant.LastName,
            participant.Phone,
            participant.Email,
            participant.BirthDate,
            participant.GuardianName,
            participant.GuardianPhone,
            participant.IsArchived,
            participant.DataProcessingConsentAt is not null,
            participant.ImageConsentAt is not null,
            groupsByParticipant.TryGetValue(participant.Id, out var groups) ? groups : [],
            participant.GuardianEmail,
            hasGuardianAccount);

    private static ParticipantDetailsDto ToDetails(Participant participant, Dictionary<Guid, List<ParticipantGroupDto>> groupsByParticipant) =>
        new(
            participant.Id,
            participant.FirstName,
            participant.LastName,
            participant.Phone,
            participant.Email,
            participant.BirthDate,
            participant.Notes,
            participant.GuardianName,
            participant.GuardianPhone,
            participant.GuardianEmail,
            participant.GuardianRelation,
            participant.IsArchived,
            participant.ArchivedAt,
            participant.DataProcessingConsentAt,
            participant.ImageConsentAt,
            participant.CreatedAt,
            groupsByParticipant.TryGetValue(participant.Id, out var groups) ? groups : []);

    private static string RequireName(string? value, string label)
    {
        var trimmed = (value ?? string.Empty).Trim();

        if (trimmed.Length == 0)
        {
            throw new ArgumentException($"Uczestnik musi mieć {label}.");
        }

        return trimmed;
    }

    private static string? NullIfEmpty(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}

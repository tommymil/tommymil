using LessonRunner.Application.Auth;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Participants;
using LessonRunner.Domain.Safety;
using LessonRunner.Domain.Support;

namespace LessonRunner.Application.Safety;

public interface ISafetyService
{
    /// <summary>
    /// Rejestr incydentów. Administrator widzi wszystko; instruktor **wyłącznie własne
    /// zgłoszenia** — patrz komentarz przy implementacji.
    /// </summary>
    Task<IncidentBoardDto> GetIncidentsAsync(Guid userId, bool isAdmin, CancellationToken cancellationToken);

    Task<IncidentDto> ReportIncidentAsync(Guid userId, CreateIncidentDto dto, CancellationToken cancellationToken);

    /// <summary>Prowadzenie sprawy — wyłącznie administrator. Zwraca `null` dla nieznanego id.</summary>
    Task<IncidentDto?> UpdateIncidentAsync(Guid id, UpdateIncidentDto dto, CancellationToken cancellationToken);

    Task<SupportBoardDto> GetSupportTicketsAsync(Guid? participantId, CancellationToken cancellationToken);
    Task<SupportTicketDto> ReportSupportTicketAsync(Guid userId, CreateSupportTicketDto dto, CancellationToken cancellationToken);
    Task<SupportTicketDto?> UpdateSupportTicketAsync(Guid id, UpdateSupportTicketDto dto, CancellationToken cancellationToken);
}

public sealed class SafetyService(
    IIncidentRepository incidentRepository,
    ISupportTicketRepository ticketRepository,
    IUserRepository userRepository,
    IParticipantRepository participantRepository,
    IGroupRepository groupRepository) : ISafetyService
{
    public async Task<IncidentBoardDto> GetIncidentsAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var incidents = await incidentRepository.ListAsync(cancellationToken);

        // Instruktor widzi wyłącznie to, co sam zgłosił.
        //
        // Powód jest konkretny: wśród rodzajów incydentu jest „zachowanie osoby prowadzącej”.
        // Rejestr, w którym każdy instruktor czyta zgłoszenia o wszystkich, jest rejestrem,
        // do którego nikt nie zgłosi niczego niewygodnego. Przeglądanie i prowadzenie spraw
        // należy do administracji.
        var visible = isAdmin
            ? incidents
            : incidents.Where(incident => incident.ReportedByUserId == userId).ToList();

        return new IncidentBoardDto(
            await ToDtosAsync(visible, cancellationToken),
            Options<IncidentKind>(kind => kind.Name(), kind => kind.Label()),
            Options<IncidentSeverity>(severity => severity.Name(), severity => severity.Label()),
            Options<IncidentStatus>(status => status.Name(), status => status.Label()));
    }

    public async Task<IncidentDto> ReportIncidentAsync(
        Guid userId,
        CreateIncidentDto dto,
        CancellationToken cancellationToken)
    {
        var description = Required(dto.Description, "Opis incydentu jest wymagany.", 4000);

        var incident = new Incident
        {
            Kind = ParseOr(dto.Kind, IncidentKind.Other),
            Severity = ParseOr(dto.Severity, IncidentSeverity.Medium),
            Status = IncidentStatus.Reported,
            // Data zdarzenia bywa wcześniejsza niż zgłoszenie - instruktor opisuje sprawę
            // po zajęciach, nie w ich trakcie.
            OccurredAt = dto.OccurredAt ?? DateTimeOffset.UtcNow,
            GroupId = dto.GroupId,
            SessionId = dto.SessionId,
            Description = description,
            ReportedByUserId = userId
        };

        incident.SetParticipantIds(dto.ParticipantIds ?? []);
        await incidentRepository.AddAsync(incident, cancellationToken);

        return (await ToDtosAsync([incident], cancellationToken))[0];
    }

    public async Task<IncidentDto?> UpdateIncidentAsync(
        Guid id,
        UpdateIncidentDto dto,
        CancellationToken cancellationToken)
    {
        var incident = await incidentRepository.GetByIdAsync(id, cancellationToken);

        if (incident is null)
        {
            return null;
        }

        var status = ParseOr(dto.Status, incident.Status);

        // Zamknięcie sprawy wymaga opisu rozwiązania. Rejestr, w którym sprawy znikają
        // ze statusem „rozwiązane” i pustym polem, nie jest dowodem na nic.
        var resolution = Trimmed(dto.Resolution, 4000) ?? incident.Resolution;

        if (status.IsClosed() && string.IsNullOrWhiteSpace(resolution))
        {
            throw new ArgumentException("Zamknięcie sprawy wymaga opisania, jak została rozwiązana.");
        }

        incident.Status = status;
        incident.Severity = dto.Severity is null ? incident.Severity : ParseOr(dto.Severity, incident.Severity);
        incident.ActionsTaken = Trimmed(dto.ActionsTaken, 4000) ?? incident.ActionsTaken;
        incident.Resolution = resolution;
        incident.AssignedToUserId = dto.AssignedToUserId ?? incident.AssignedToUserId;
        incident.UpdatedAt = DateTimeOffset.UtcNow;
        incident.ResolvedAt = status.IsClosed() ? incident.ResolvedAt ?? DateTimeOffset.UtcNow : null;

        await incidentRepository.UpdateAsync(incident, cancellationToken);
        return (await ToDtosAsync([incident], cancellationToken))[0];
    }

    public async Task<SupportBoardDto> GetSupportTicketsAsync(
        Guid? participantId,
        CancellationToken cancellationToken)
    {
        var tickets = participantId is Guid id
            ? await ticketRepository.ListByParticipantAsync(id, cancellationToken)
            : await ticketRepository.ListAsync(cancellationToken);

        return new SupportBoardDto(
            await ToTicketDtosAsync(tickets, cancellationToken),
            Options<SupportCategory>(category => category.Name(), category => category.Label()),
            Options<SupportTicketStatus>(status => status.Name(), status => status.Label()));
    }

    public async Task<SupportTicketDto> ReportSupportTicketAsync(
        Guid userId,
        CreateSupportTicketDto dto,
        CancellationToken cancellationToken)
    {
        var ticket = new SupportTicket
        {
            ParticipantId = dto.ParticipantId,
            SessionId = dto.SessionId,
            GroupId = dto.GroupId,
            Category = ParseOr(dto.Category, SupportCategory.Other),
            Status = SupportTicketStatus.Open,
            Description = Required(dto.Description, "Opis problemu jest wymagany.", 4000),
            CostLessonTime = dto.CostLessonTime,
            ReportedByUserId = userId
        };

        await ticketRepository.AddAsync(ticket, cancellationToken);
        return (await ToTicketDtosAsync([ticket], cancellationToken))[0];
    }

    public async Task<SupportTicketDto?> UpdateSupportTicketAsync(
        Guid id,
        UpdateSupportTicketDto dto,
        CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(id, cancellationToken);

        if (ticket is null)
        {
            return null;
        }

        var status = ParseOr(dto.Status, ticket.Status);
        var resolution = Trimmed(dto.Resolution, 4000) ?? ticket.Resolution;

        // Przy zgłoszeniu technicznym opis rozwiązania to **cała wartość rejestru**: przy
        // powtórce tego samego problemu u tego samego dziecka nie zaczynamy od zera.
        if (status == SupportTicketStatus.Resolved && string.IsNullOrWhiteSpace(resolution))
        {
            throw new ArgumentException("Napisz, co pomogło — bez tego zgłoszenie nie przyda się następnym razem.");
        }

        ticket.Status = status;
        ticket.Category = dto.Category is null ? ticket.Category : ParseOr(dto.Category, ticket.Category);
        ticket.Resolution = resolution;
        ticket.CostLessonTime = dto.CostLessonTime ?? ticket.CostLessonTime;
        ticket.UpdatedAt = DateTimeOffset.UtcNow;
        ticket.ResolvedAt = status.IsOpen() ? null : ticket.ResolvedAt ?? DateTimeOffset.UtcNow;

        await ticketRepository.UpdateAsync(ticket, cancellationToken);
        return (await ToTicketDtosAsync([ticket], cancellationToken))[0];
    }

    /// <summary>Nazwy ludzi i grup dociągamy raz na listę, nie po jednym na wiersz.</summary>
    private async Task<(Dictionary<Guid, string> Users, Dictionary<Guid, string> Groups)> LookupsAsync(
        CancellationToken cancellationToken)
    {
        var users = (await userRepository.ListAsync(cancellationToken))
            .ToDictionary(user => user.Id, user => user.DisplayName);
        var groups = (await groupRepository.ListAsync(cancellationToken))
            .ToDictionary(group => group.Id, group => group.Name);

        return (users, groups);
    }

    private async Task<IReadOnlyList<IncidentDto>> ToDtosAsync(
        IReadOnlyList<Incident> incidents,
        CancellationToken cancellationToken)
    {
        var (users, groups) = await LookupsAsync(cancellationToken);
        var participantIds = incidents.SelectMany(incident => incident.ParticipantIdList()).Distinct().ToList();
        var participants = participantIds.Count == 0
            ? []
            : await participantRepository.GetByIdsAsync(participantIds, cancellationToken);
        var names = participants.ToDictionary(
            participant => participant.Id,
            participant => $"{participant.FirstName} {participant.LastName}");

        return incidents
            .Select(incident => new IncidentDto(
                incident.Id,
                incident.Kind.Name(),
                incident.Kind.Label(),
                incident.Severity.Name(),
                incident.Severity.Label(),
                incident.Status.Name(),
                incident.Status.Label(),
                incident.OccurredAt,
                incident.GroupId,
                incident.GroupId is Guid groupId ? groups.GetValueOrDefault(groupId) : null,
                incident.SessionId,
                incident.ParticipantIdList()
                    .Select(id => new IncidentParticipantDto(id, names.GetValueOrDefault(id, "(usunięte dane)")))
                    .ToList(),
                incident.Description,
                incident.ActionsTaken,
                incident.Resolution,
                incident.ReportedByUserId,
                users.GetValueOrDefault(incident.ReportedByUserId, "(nieznany)"),
                incident.AssignedToUserId,
                incident.AssignedToUserId is Guid assignee ? users.GetValueOrDefault(assignee) : null,
                incident.CreatedAt,
                incident.ResolvedAt,
                incident.NeedsImmediateAttention()))
            .ToList();
    }

    private async Task<IReadOnlyList<SupportTicketDto>> ToTicketDtosAsync(
        IReadOnlyList<SupportTicket> tickets,
        CancellationToken cancellationToken)
    {
        var (users, groups) = await LookupsAsync(cancellationToken);
        var participantIds = tickets
            .Where(ticket => ticket.ParticipantId is not null)
            .Select(ticket => ticket.ParticipantId!.Value)
            .Distinct()
            .ToList();
        var participants = participantIds.Count == 0
            ? []
            : await participantRepository.GetByIdsAsync(participantIds, cancellationToken);
        var names = participants.ToDictionary(
            participant => participant.Id,
            participant => $"{participant.FirstName} {participant.LastName}");

        return tickets
            .Select(ticket => new SupportTicketDto(
                ticket.Id,
                ticket.ParticipantId,
                ticket.ParticipantId is Guid participantId ? names.GetValueOrDefault(participantId) : null,
                ticket.SessionId,
                ticket.GroupId,
                ticket.GroupId is Guid groupId ? groups.GetValueOrDefault(groupId) : null,
                ticket.Category.Name(),
                ticket.Category.Label(),
                ticket.Status.Name(),
                ticket.Status.Label(),
                ticket.Description,
                ticket.Resolution,
                ticket.CostLessonTime,
                ticket.ReportedByUserId,
                users.GetValueOrDefault(ticket.ReportedByUserId, "(nieznany)"),
                ticket.CreatedAt,
                ticket.ResolvedAt))
            .ToList();
    }

    private static IReadOnlyList<SafetyOptionDto> Options<TEnum>(
        Func<TEnum, string> name,
        Func<TEnum, string> label) where TEnum : struct, Enum =>
        Enum.GetValues<TEnum>().Select(value => new SafetyOptionDto(name(value), label(value))).ToList();

    private static TEnum ParseOr<TEnum>(string? value, TEnum fallback) where TEnum : struct =>
        Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : fallback;

    private static string Required(string? value, string error, int maxLength)
    {
        var trimmed = value?.Trim();

        return string.IsNullOrEmpty(trimmed)
            ? throw new ArgumentException(error)
            : trimmed[..Math.Min(trimmed.Length, maxLength)];
    }

    private static string? Trimmed(string? value, int maxLength)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed[..Math.Min(trimmed.Length, maxLength)];
    }
}

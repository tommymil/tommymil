using LessonRunner.Application.Safety;
using LessonRunner.Domain.Safety;
using LessonRunner.Domain.Support;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Safety;

internal sealed class EfIncidentRepository(AppDbContext dbContext) : IIncidentRepository
{
    public async Task<IReadOnlyList<Incident>> ListAsync(CancellationToken cancellationToken)
    {
        // SQLite nie sortuje po DateTimeOffset w SQL - materializujemy i porządkujemy
        // po stronie klienta, tak jak reszta repozytoriów w tym projekcie.
        var documents = await dbContext.Incidents.AsNoTracking().ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(incident => incident.OccurredAt)
            .Select(ToDomain)
            .ToList();
    }

    public async Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Incidents.AsNoTracking()
            .FirstOrDefaultAsync(incident => incident.Id == id, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task AddAsync(Incident incident, CancellationToken cancellationToken)
    {
        dbContext.Incidents.Add(ToDocument(incident));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(Incident incident, CancellationToken cancellationToken)
    {
        var document = await dbContext.Incidents.FirstOrDefaultAsync(item => item.Id == incident.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Kind = incident.Kind.ToString();
        document.Severity = incident.Severity.ToString();
        document.Status = incident.Status.ToString();
        document.OccurredAt = incident.OccurredAt;
        document.GroupId = incident.GroupId;
        document.SessionId = incident.SessionId;
        document.ParticipantIds = incident.ParticipantIds;
        document.Description = incident.Description;
        document.ActionsTaken = incident.ActionsTaken;
        document.Resolution = incident.Resolution;
        document.AssignedToUserId = incident.AssignedToUserId;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        document.ResolvedAt = incident.ResolvedAt;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Incident ToDomain(IncidentDocument document) => new()
    {
        Id = document.Id,
        Kind = ParseEnum(document.Kind, IncidentKind.Other),
        Severity = ParseEnum(document.Severity, IncidentSeverity.Medium),
        Status = ParseEnum(document.Status, IncidentStatus.Reported),
        OccurredAt = document.OccurredAt,
        GroupId = document.GroupId,
        SessionId = document.SessionId,
        ParticipantIds = document.ParticipantIds,
        Description = document.Description,
        ActionsTaken = document.ActionsTaken,
        Resolution = document.Resolution,
        ReportedByUserId = document.ReportedByUserId,
        AssignedToUserId = document.AssignedToUserId,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt,
        ResolvedAt = document.ResolvedAt
    };

    private static IncidentDocument ToDocument(Incident incident) => new()
    {
        Id = incident.Id,
        Kind = incident.Kind.ToString(),
        Severity = incident.Severity.ToString(),
        Status = incident.Status.ToString(),
        OccurredAt = incident.OccurredAt,
        GroupId = incident.GroupId,
        SessionId = incident.SessionId,
        ParticipantIds = incident.ParticipantIds,
        Description = incident.Description,
        ActionsTaken = incident.ActionsTaken,
        Resolution = incident.Resolution,
        ReportedByUserId = incident.ReportedByUserId,
        AssignedToUserId = incident.AssignedToUserId,
        CreatedAt = incident.CreatedAt,
        UpdatedAt = incident.UpdatedAt,
        ResolvedAt = incident.ResolvedAt
    };

    private static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct =>
        Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : fallback;
}

internal sealed class EfSupportTicketRepository(AppDbContext dbContext) : ISupportTicketRepository
{
    public async Task<IReadOnlyList<SupportTicket>> ListAsync(CancellationToken cancellationToken)
    {
        var documents = await dbContext.SupportTickets.AsNoTracking().ToListAsync(cancellationToken);
        return Ordered(documents);
    }

    public async Task<IReadOnlyList<SupportTicket>> ListByParticipantAsync(
        Guid participantId,
        CancellationToken cancellationToken)
    {
        // Filtr po dziecku idzie do bazy (tłumaczy się), porządek liczymy w pamięci.
        var documents = await dbContext.SupportTickets
            .AsNoTracking()
            .Where(ticket => ticket.ParticipantId == participantId)
            .ToListAsync(cancellationToken);

        return Ordered(documents);
    }

    public async Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.SupportTickets.AsNoTracking()
            .FirstOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken)
    {
        dbContext.SupportTickets.Add(ToDocument(ticket));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken)
    {
        var document = await dbContext.SupportTickets.FirstOrDefaultAsync(item => item.Id == ticket.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Category = ticket.Category.ToString();
        document.Status = ticket.Status.ToString();
        document.Description = ticket.Description;
        document.Resolution = ticket.Resolution;
        document.CostLessonTime = ticket.CostLessonTime;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        document.ResolvedAt = ticket.ResolvedAt;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IReadOnlyList<SupportTicket> Ordered(IEnumerable<SupportTicketDocument> documents) =>
        documents
            .OrderByDescending(ticket => ticket.CreatedAt)
            .Select(ToDomain)
            .ToList();

    private static SupportTicket ToDomain(SupportTicketDocument document) => new()
    {
        Id = document.Id,
        ParticipantId = document.ParticipantId,
        SessionId = document.SessionId,
        GroupId = document.GroupId,
        Category = ParseEnum(document.Category, SupportCategory.Other),
        Status = ParseEnum(document.Status, SupportTicketStatus.Open),
        Description = document.Description,
        Resolution = document.Resolution,
        CostLessonTime = document.CostLessonTime,
        ReportedByUserId = document.ReportedByUserId,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt,
        ResolvedAt = document.ResolvedAt
    };

    private static SupportTicketDocument ToDocument(SupportTicket ticket) => new()
    {
        Id = ticket.Id,
        ParticipantId = ticket.ParticipantId,
        SessionId = ticket.SessionId,
        GroupId = ticket.GroupId,
        Category = ticket.Category.ToString(),
        Status = ticket.Status.ToString(),
        Description = ticket.Description,
        Resolution = ticket.Resolution,
        CostLessonTime = ticket.CostLessonTime,
        ReportedByUserId = ticket.ReportedByUserId,
        CreatedAt = ticket.CreatedAt,
        UpdatedAt = ticket.UpdatedAt,
        ResolvedAt = ticket.ResolvedAt
    };

    private static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct =>
        Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : fallback;
}

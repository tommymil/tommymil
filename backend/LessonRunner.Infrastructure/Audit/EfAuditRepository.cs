using LessonRunner.Application.Audit;
using LessonRunner.Domain.Audit;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Audit;

internal sealed class EfAuditRepository(AppDbContext dbContext) : IAuditRepository
{
    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken)
    {
        dbContext.AuditLogs.Add(ToDocument(auditLog));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> ListRecentAsync(int limit, CancellationToken cancellationToken)
    {
        // SQLite nie sortuje po DateTimeOffset w SQL - porządkujemy i tniemy limit po stronie klienta.
        var documents = await dbContext.AuditLogs
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(log => log.OccurredAt)
            .Take(limit)
            .Select(ToDomain)
            .ToList();
    }

    private static AuditLog ToDomain(AuditLogDocument document) => new()
    {
        Id = document.Id,
        OccurredAt = document.OccurredAt,
        ActorUserId = document.ActorUserId,
        Action = document.Action,
        EntityType = document.EntityType,
        EntityId = document.EntityId,
        Success = document.Success,
        Details = document.Details
    };

    private static AuditLogDocument ToDocument(AuditLog auditLog) => new()
    {
        Id = auditLog.Id,
        OccurredAt = auditLog.OccurredAt,
        ActorUserId = auditLog.ActorUserId,
        Action = auditLog.Action,
        EntityType = auditLog.EntityType,
        EntityId = auditLog.EntityId,
        Success = auditLog.Success,
        Details = auditLog.Details
    };
}

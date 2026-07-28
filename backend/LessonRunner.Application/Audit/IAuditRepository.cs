using LessonRunner.Domain.Audit;

namespace LessonRunner.Application.Audit;

public interface IAuditRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken);
    Task<IReadOnlyList<AuditLog>> ListRecentAsync(int limit, CancellationToken cancellationToken);
}

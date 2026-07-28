namespace LessonRunner.Application.Audit;

public interface IAuditService
{
    Task RecordAsync(
        Guid? actorUserId,
        string action,
        string entityType,
        string? entityId,
        bool success,
        string? details,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AuditLogDto>> ListRecentAsync(int limit, CancellationToken cancellationToken);

    /// <summary>Dziennik z filtrami - przy reklamacji szuka się po konkretnym obszarze,
    /// a nie przewija stu ostatnich wpisów.</summary>
    Task<IReadOnlyList<AuditLogDto>> SearchAsync(AuditLogQueryDto query, CancellationToken cancellationToken);
}

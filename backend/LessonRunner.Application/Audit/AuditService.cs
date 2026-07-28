using LessonRunner.Domain.Audit;

namespace LessonRunner.Application.Audit;

public sealed class AuditService(IAuditRepository auditRepository) : IAuditService
{
    private const int MaxLimit = 500;

    public Task RecordAsync(
        Guid? actorUserId,
        string action,
        string entityType,
        string? entityId,
        bool success,
        string? details,
        CancellationToken cancellationToken)
    {
        var auditLog = new AuditLog
        {
            ActorUserId = actorUserId,
            Action = Normalize(action, "unknown"),
            // Obszar trzymamy w jednej konwencji: małe litery, tak jak segment ścieżki
            // („users", „groups", „billing"). Jawne wpisy używały kiedyś „User", przez co
            // filtrowanie po obszarze gubiło połowę dziennika.
            EntityType = Normalize(entityType, "unknown").ToLowerInvariant(),
            EntityId = string.IsNullOrWhiteSpace(entityId) ? null : entityId.Trim(),
            Success = success,
            Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim()
        };

        return auditRepository.AddAsync(auditLog, cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLogDto>> ListRecentAsync(int limit, CancellationToken cancellationToken)
    {
        var normalizedLimit = Math.Clamp(limit <= 0 ? 100 : limit, 1, MaxLimit);
        var logs = await auditRepository.ListRecentAsync(normalizedLimit, cancellationToken);
        return logs.Select(log => new AuditLogDto(
            log.Id,
            log.OccurredAt,
            log.ActorUserId,
            log.Action,
            log.EntityType,
            log.EntityId,
            log.Success,
            log.Details)).ToList();
    }

    public async Task<IReadOnlyList<AuditLogDto>> SearchAsync(AuditLogQueryDto query, CancellationToken cancellationToken)
    {
        // Filtrujemy w pamięci na już ograniczonym zbiorze: dziennik czyta wyłącznie admin,
        // a przy tej skali zapytanie po ostatnich N wpisach jest tańsze niż indeksowanie
        // każdej kombinacji filtrów.
        var normalizedLimit = Math.Clamp(query.Limit <= 0 ? 100 : query.Limit, 1, MaxLimit);
        var logs = await auditRepository.ListRecentAsync(MaxLimit, cancellationToken);

        return logs
            .Where(log => string.IsNullOrWhiteSpace(query.EntityType)
                || string.Equals(log.EntityType, query.EntityType.Trim(), StringComparison.OrdinalIgnoreCase))
            .Where(log => string.IsNullOrWhiteSpace(query.Action)
                || log.Action.Contains(query.Action.Trim(), StringComparison.OrdinalIgnoreCase))
            .Where(log => query.Success is null || log.Success == query.Success)
            .Take(normalizedLimit)
            .Select(log => new AuditLogDto(
                log.Id,
                log.OccurredAt,
                log.ActorUserId,
                log.Action,
                log.EntityType,
                log.EntityId,
                log.Success,
                log.Details))
            .ToList();
    }

    private static string Normalize(string value, string fallback)
    {
        var normalized = (value ?? string.Empty).Trim();
        return normalized.Length == 0 ? fallback : normalized;
    }
}

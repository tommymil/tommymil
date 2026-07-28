namespace LessonRunner.Application.Audit;

public sealed record AuditLogDto(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid? ActorUserId,
    string Action,
    string EntityType,
    string? EntityId,
    bool Success,
    string? Details);

public sealed record AuditLogQueryDto(
    int Limit = 100,
    /// <summary>Obszar, np. `groups`, `users`, `billing`.</summary>
    string? EntityType = null,
    /// <summary>Fragment nazwy akcji, np. `Cancel`.</summary>
    string? Action = null,
    /// <summary>`true` = tylko udane, `false` = tylko nieudane, `null` = wszystkie.</summary>
    bool? Success = null);

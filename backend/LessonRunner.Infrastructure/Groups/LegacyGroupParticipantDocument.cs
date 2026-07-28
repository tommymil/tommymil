namespace LessonRunner.Infrastructure.Groups;

/// <summary>
/// Odwzorowanie starej tabeli "GroupParticipants" (uczestnik zaszyty 1:1 w grupie), używane
/// wyłącznie do jednorazowego, idempotentnego backfillu do Participants/GroupEnrollments.
/// Tabela fizycznie zostaje w bazie do czasu osobnej migracji porządkującej.
/// </summary>
internal sealed class LegacyGroupParticipantDocument
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

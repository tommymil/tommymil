using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Participants;

public sealed class Participant : Entity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    /// <summary>Data urodzenia dziecka (wiek liczymy z niej w warstwie prezentacji).</summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>Notatki - np. potrzeby specjalne, alergie, uwagi organizacyjne.</summary>
    public string? Notes { get; set; }

    // Opiekun/rodzic - podstawowy kontakt dla dziecka.
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianEmail { get; set; }

    /// <summary>Relacja opiekuna do dziecka, np. "mama", "tata", "opiekun prawny".</summary>
    public string? GuardianRelation { get; set; }

    /// <summary>Zarchiwizowany = nieaktywny, ale zachowany (nie usuwamy dzieci z historią).</summary>
    public bool IsArchived { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }

    // Zgody RODO - moment udzielenia (null = brak zgody). Trzymamy datę na potrzeby audytu.
    public DateTimeOffset? DataProcessingConsentAt { get; set; }
    public DateTimeOffset? ImageConsentAt { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

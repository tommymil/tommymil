namespace LessonRunner.Application.Participants;

/// <summary>Centralna baza uczestników, niezależna od pojedynczej grupy + zapisy/wypisy do grup.</summary>
public interface IParticipantService
{
    Task<IReadOnlyList<ParticipantSummaryDto>> GetSummariesAsync(string? query, bool includeArchived, CancellationToken cancellationToken);
    Task<ParticipantDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<ParticipantDetailsDto> CreateAsync(CreateParticipantDto dto, CancellationToken cancellationToken);
    Task<ParticipantDetailsDto?> UpdateAsync(Guid id, UpdateParticipantDto dto, CancellationToken cancellationToken);

    /// <summary>Archiwizuje (nieaktywny, ale zachowany) lub przywraca uczestnika.</summary>
    Task<bool> SetArchivedAsync(Guid id, bool archived, CancellationToken cancellationToken);

    /// <summary>RODO - kasuje dane osobowe, zachowując rekord i historię obecności.</summary>
    Task<bool> AnonymizeAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Zapisuje uczestnika do grupy. Idempotentne - jeśli już zapisany, zwraca bieżący stan.</summary>
    Task<ParticipantDetailsDto?> EnrollAsync(Guid participantId, Guid groupId, CancellationToken cancellationToken);

    /// <summary>Wypisuje uczestnika z grupy. Zawsze dozwolone - historia obecności nie jest usuwana.</summary>
    Task<ParticipantDetailsDto?> UnenrollAsync(Guid participantId, Guid groupId, CancellationToken cancellationToken);
}

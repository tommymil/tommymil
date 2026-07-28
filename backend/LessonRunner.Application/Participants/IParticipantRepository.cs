using LessonRunner.Domain.Participants;

namespace LessonRunner.Application.Participants;

public interface IParticipantRepository
{
    Task<IReadOnlyList<Participant>> ListAsync(CancellationToken cancellationToken);
    Task<Participant?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Participant>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
    Task AddAsync(Participant participant, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Participant participant, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

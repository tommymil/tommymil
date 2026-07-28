using LessonRunner.Domain.Parents;

namespace LessonRunner.Application.Parents;

public interface IParentPortalRepository
{
    Task<IReadOnlyList<ParentParticipantLink>> ListAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ParentParticipantLink>> ListByParentAsync(Guid parentUserId, CancellationToken cancellationToken);
    Task AddAsync(ParentParticipantLink link, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid parentUserId, Guid participantId, CancellationToken cancellationToken);
}

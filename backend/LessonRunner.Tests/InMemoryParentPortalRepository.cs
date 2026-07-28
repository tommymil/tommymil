using LessonRunner.Application.Parents;
using LessonRunner.Domain.Parents;

namespace LessonRunner.Tests;

internal sealed class InMemoryParentPortalRepository : IParentPortalRepository
{
    private readonly List<ParentParticipantLink> _links = [];

    public Task<IReadOnlyList<ParentParticipantLink>> ListAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<ParentParticipantLink>>(_links.ToList());

    public Task<IReadOnlyList<ParentParticipantLink>> ListByParentAsync(Guid parentUserId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<ParentParticipantLink>>(_links.Where(link => link.ParentUserId == parentUserId).ToList());

    public Task AddAsync(ParentParticipantLink link, CancellationToken cancellationToken)
    {
        _links.Add(link);
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(Guid parentUserId, Guid participantId, CancellationToken cancellationToken) =>
        Task.FromResult(_links.RemoveAll(link => link.ParentUserId == parentUserId && link.ParticipantId == participantId) > 0);
}

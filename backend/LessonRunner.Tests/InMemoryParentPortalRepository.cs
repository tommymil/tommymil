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

    public Task<IReadOnlyList<ParentParticipantLink>> ListByParticipantsAsync(
        IReadOnlyList<Guid> participantIds,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ParentParticipantLink> result = _links
            .Where(link => participantIds.Contains(link.ParticipantId))
            .ToList();
        return Task.FromResult(result);
    }

    public Task<bool> UpdateAsync(ParentParticipantLink link, CancellationToken cancellationToken)
    {
        var index = _links.FindIndex(item => item.ParentUserId == link.ParentUserId && item.ParticipantId == link.ParticipantId);

        if (index < 0)
        {
            return Task.FromResult(false);
        }

        _links[index] = link;
        return Task.FromResult(true);
    }
}

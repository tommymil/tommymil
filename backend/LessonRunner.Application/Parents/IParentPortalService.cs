namespace LessonRunner.Application.Parents;

public interface IParentPortalService
{
    Task<ParentPortalDto> GetPortalAsync(Guid parentUserId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ParentParticipantLinkDto>> ListLinksAsync(CancellationToken cancellationToken);
    Task<ParentParticipantLinkDto> LinkAsync(Guid parentUserId, Guid participantId, CancellationToken cancellationToken);
    Task<bool> UnlinkAsync(Guid parentUserId, Guid participantId, CancellationToken cancellationToken);
}

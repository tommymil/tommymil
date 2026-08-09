using LessonRunner.Application.Parents;
using LessonRunner.Domain.Parents;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Parents;

internal sealed class EfParentPortalRepository(AppDbContext dbContext) : IParentPortalRepository
{
    public async Task<IReadOnlyList<ParentParticipantLink>> ListAsync(CancellationToken cancellationToken) =>
        (await dbContext.ParentParticipantLinks.AsNoTracking().ToListAsync(cancellationToken))
            .Select(ToDomain)
            .ToList();

    public async Task<IReadOnlyList<ParentParticipantLink>> ListByParentAsync(Guid parentUserId, CancellationToken cancellationToken) =>
        (await dbContext.ParentParticipantLinks
            .AsNoTracking()
            .Where(link => link.ParentUserId == parentUserId)
            .ToListAsync(cancellationToken))
            .Select(ToDomain)
            .ToList();

    public async Task<IReadOnlyList<ParentParticipantLink>> ListByParticipantsAsync(
        IReadOnlyList<Guid> participantIds,
        CancellationToken cancellationToken)
    {
        if (participantIds.Count == 0)
        {
            return [];
        }

        var ids = participantIds.Distinct().ToList();
        return (await dbContext.ParentParticipantLinks
            .AsNoTracking()
            .Where(link => ids.Contains(link.ParticipantId))
            .ToListAsync(cancellationToken))
            .Select(ToDomain)
            .ToList();
    }

    public async Task<bool> UpdateAsync(ParentParticipantLink link, CancellationToken cancellationToken)
    {
        var document = await dbContext.ParentParticipantLinks
            .FirstOrDefaultAsync(item => item.ParentUserId == link.ParentUserId && item.ParticipantId == link.ParticipantId, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Relation = link.Relation;
        document.IsPrimaryContact = link.IsPrimaryContact;
        document.ReceivesNotifications = link.ReceivesNotifications;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddAsync(ParentParticipantLink link, CancellationToken cancellationToken)
    {
        dbContext.ParentParticipantLinks.Add(ToDocument(link));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid parentUserId, Guid participantId, CancellationToken cancellationToken)
    {
        var document = await dbContext.ParentParticipantLinks
            .FirstOrDefaultAsync(link => link.ParentUserId == parentUserId && link.ParticipantId == participantId, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.ParentParticipantLinks.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ParentParticipantLink ToDomain(ParentParticipantLinkDocument document) => new()
    {
        Id = document.Id,
        ParentUserId = document.ParentUserId,
        ParticipantId = document.ParticipantId,
        Relation = document.Relation,
        IsPrimaryContact = document.IsPrimaryContact,
        ReceivesNotifications = document.ReceivesNotifications,
        CreatedAt = document.CreatedAt
    };

    private static ParentParticipantLinkDocument ToDocument(ParentParticipantLink link) => new()
    {
        Id = link.Id,
        ParentUserId = link.ParentUserId,
        ParticipantId = link.ParticipantId,
        Relation = link.Relation,
        IsPrimaryContact = link.IsPrimaryContact,
        ReceivesNotifications = link.ReceivesNotifications,
        CreatedAt = link.CreatedAt
    };
}

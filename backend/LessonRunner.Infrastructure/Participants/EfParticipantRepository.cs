using LessonRunner.Application.Participants;
using LessonRunner.Domain.Participants;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Participants;

internal sealed class EfParticipantRepository(AppDbContext dbContext) : IParticipantRepository
{
    public async Task<IReadOnlyList<Participant>> ListAsync(CancellationToken cancellationToken)
    {
        var documents = await dbContext.Participants
            .AsNoTracking()
            .OrderBy(participant => participant.LastName)
            .ThenBy(participant => participant.FirstName)
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<Participant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Participants
            .AsNoTracking()
            .FirstOrDefaultAsync(participant => participant.Id == id, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task<IReadOnlyList<Participant>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var documents = await dbContext.Participants
            .AsNoTracking()
            .Where(participant => ids.Contains(participant.Id))
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Participant participant, CancellationToken cancellationToken)
    {
        dbContext.Participants.Add(ToDocument(participant));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(Participant participant, CancellationToken cancellationToken)
    {
        var document = await dbContext.Participants
            .FirstOrDefaultAsync(item => item.Id == participant.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.FirstName = participant.FirstName;
        document.LastName = participant.LastName;
        document.Phone = participant.Phone;
        document.Email = participant.Email;
        document.BirthDate = participant.BirthDate;
        document.Notes = participant.Notes;
        document.GuardianName = participant.GuardianName;
        document.GuardianPhone = participant.GuardianPhone;
        document.GuardianEmail = participant.GuardianEmail;
        document.GuardianRelation = participant.GuardianRelation;
        document.IsArchived = participant.IsArchived;
        document.ArchivedAt = participant.ArchivedAt;
        document.DataProcessingConsentAt = participant.DataProcessingConsentAt;
        document.ImageConsentAt = participant.ImageConsentAt;
        document.UpdatedAt = participant.UpdatedAt;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Participants.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.Participants.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Participant ToDomain(ParticipantDocument document) => new()
    {
        Id = document.Id,
        FirstName = document.FirstName,
        LastName = document.LastName,
        Phone = document.Phone,
        Email = document.Email,
        BirthDate = document.BirthDate,
        Notes = document.Notes,
        GuardianName = document.GuardianName,
        GuardianPhone = document.GuardianPhone,
        GuardianEmail = document.GuardianEmail,
        GuardianRelation = document.GuardianRelation,
        IsArchived = document.IsArchived,
        ArchivedAt = document.ArchivedAt,
        DataProcessingConsentAt = document.DataProcessingConsentAt,
        ImageConsentAt = document.ImageConsentAt,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt
    };

    private static ParticipantDocument ToDocument(Participant participant) => new()
    {
        Id = participant.Id,
        FirstName = participant.FirstName,
        LastName = participant.LastName,
        Phone = participant.Phone,
        Email = participant.Email,
        BirthDate = participant.BirthDate,
        Notes = participant.Notes,
        GuardianName = participant.GuardianName,
        GuardianPhone = participant.GuardianPhone,
        GuardianEmail = participant.GuardianEmail,
        GuardianRelation = participant.GuardianRelation,
        IsArchived = participant.IsArchived,
        ArchivedAt = participant.ArchivedAt,
        DataProcessingConsentAt = participant.DataProcessingConsentAt,
        ImageConsentAt = participant.ImageConsentAt,
        CreatedAt = participant.CreatedAt,
        UpdatedAt = participant.UpdatedAt
    };
}

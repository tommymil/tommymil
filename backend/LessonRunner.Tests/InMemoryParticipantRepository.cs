using LessonRunner.Application.Participants;
using LessonRunner.Domain.Participants;

namespace LessonRunner.Tests;

/// <summary>Fake repozytorium uczestników (centralna baza) trzymające rekordy w pamięci, do testów warstwy Application.</summary>
internal sealed class InMemoryParticipantRepository : IParticipantRepository
{
    private readonly List<Participant> _participants = [];

    public Task<IReadOnlyList<Participant>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Participant> result = _participants.ToList();
        return Task.FromResult(result);
    }

    public Task<Participant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_participants.FirstOrDefault(participant => participant.Id == id));
    }

    public Task<IReadOnlyList<Participant>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
    {
        IReadOnlyList<Participant> result = _participants.Where(participant => ids.Contains(participant.Id)).ToList();
        return Task.FromResult(result);
    }

    public Task AddAsync(Participant participant, CancellationToken cancellationToken)
    {
        _participants.Add(participant);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateAsync(Participant participant, CancellationToken cancellationToken)
    {
        var stored = _participants.FirstOrDefault(item => item.Id == participant.Id);

        if (stored is null || ReferenceEquals(stored, participant))
        {
            return Task.FromResult(stored is not null);
        }

        stored.FirstName = participant.FirstName;
        stored.LastName = participant.LastName;
        stored.Phone = participant.Phone;
        stored.Email = participant.Email;
        stored.BirthDate = participant.BirthDate;
        stored.Notes = participant.Notes;
        stored.GuardianName = participant.GuardianName;
        stored.GuardianPhone = participant.GuardianPhone;
        stored.GuardianEmail = participant.GuardianEmail;
        stored.GuardianRelation = participant.GuardianRelation;
        stored.IsArchived = participant.IsArchived;
        stored.ArchivedAt = participant.ArchivedAt;
        stored.DataProcessingConsentAt = participant.DataProcessingConsentAt;
        stored.ImageConsentAt = participant.ImageConsentAt;
        stored.UpdatedAt = participant.UpdatedAt;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_participants.RemoveAll(participant => participant.Id == id) > 0);
    }
}

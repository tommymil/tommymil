using LessonRunner.Domain.Safety;
using LessonRunner.Domain.Support;

namespace LessonRunner.Application.Safety;

public interface IIncidentRepository
{
    Task<IReadOnlyList<Incident>> ListAsync(CancellationToken cancellationToken);
    Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Incident incident, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Incident incident, CancellationToken cancellationToken);
}

public interface ISupportTicketRepository
{
    Task<IReadOnlyList<SupportTicket>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Historia problemów jednego dziecka — właściwa wartość tego rejestru.</summary>
    Task<IReadOnlyList<SupportTicket>> ListByParticipantAsync(Guid participantId, CancellationToken cancellationToken);

    Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken);
}

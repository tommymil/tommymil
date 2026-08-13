using LessonRunner.Domain.Trials;

namespace LessonRunner.Application.Trials;

public interface ITrialRepository
{
    Task<IReadOnlyList<TrialLesson>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Lekcje próbne przypisane do jednego instruktora — jego osobna lista.</summary>
    Task<IReadOnlyList<TrialLesson>> ListByInstructorAsync(Guid instructorId, CancellationToken cancellationToken);

    Task<TrialLesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(TrialLesson trial, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(TrialLesson trial, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

using LessonRunner.Domain.Lessons;

namespace LessonRunner.Application.Lessons;

public interface ILessonRepository
{
    Task<IReadOnlyList<Lesson>> ListAsync(CancellationToken cancellationToken);
    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Lesson lesson, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Lesson lesson, CancellationToken cancellationToken);
    Task SeedAsync(IReadOnlyList<Lesson> lessons, CancellationToken cancellationToken);
}

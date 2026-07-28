using LessonRunner.Domain.Courses;

namespace LessonRunner.Application.Courses;

public interface ICourseRepository
{
    Task<IReadOnlyList<Course>> ListAsync(CancellationToken cancellationToken);
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Course course, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Course course, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

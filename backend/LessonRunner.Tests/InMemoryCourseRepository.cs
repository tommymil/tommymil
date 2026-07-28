using LessonRunner.Application.Courses;
using LessonRunner.Domain.Courses;

namespace LessonRunner.Tests;

internal sealed class InMemoryCourseRepository : ICourseRepository
{
    private readonly Dictionary<Guid, Course> _courses = [];

    public Task<IReadOnlyList<Course>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Course> result = _courses.Values.ToList();
        return Task.FromResult(result);
    }

    public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_courses.TryGetValue(id, out var course) ? course : null);
    }

    public Task AddAsync(Course course, CancellationToken cancellationToken)
    {
        _courses[course.Id] = course;
        return Task.CompletedTask;
    }

    public Task<bool> UpdateAsync(Course course, CancellationToken cancellationToken)
    {
        if (!_courses.ContainsKey(course.Id))
        {
            return Task.FromResult(false);
        }

        _courses[course.Id] = course;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_courses.Remove(id));
    }
}

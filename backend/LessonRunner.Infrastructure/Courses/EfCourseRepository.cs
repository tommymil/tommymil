using LessonRunner.Application.Courses;
using LessonRunner.Domain.Courses;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Courses;

internal sealed class EfCourseRepository(AppDbContext dbContext) : ICourseRepository
{
    public async Task<IReadOnlyList<Course>> ListAsync(CancellationToken cancellationToken)
    {
        var documents = await QueryAggregate()
            .OrderBy(course => course.Subject)
            .ThenBy(course => course.Level)
            .ThenBy(course => course.Name)
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await QueryAggregate().FirstOrDefaultAsync(course => course.Id == id, cancellationToken);
        return document is null ? null : ToDomain(document);
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken)
    {
        dbContext.Courses.Add(ToDocument(course));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(Course course, CancellationToken cancellationToken)
    {
        var document = await dbContext.Courses
            .Include(item => item.Lessons)
            .FirstOrDefaultAsync(item => item.Id == course.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Name = course.Name;
        document.Subject = course.Subject;
        document.Level = course.Level;
        document.Description = course.Description;
        document.UpdatedAt = course.UpdatedAt;
        document.Lessons.Clear();
        document.Lessons.AddRange(course.Lessons.Select(ToDocument));

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Courses.FirstOrDefaultAsync(course => course.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.Courses.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<CourseDocument> QueryAggregate() =>
        dbContext.Courses
            .AsNoTracking()
            .Include(course => course.Lessons);

    private static Course ToDomain(CourseDocument document) => new()
    {
        Id = document.Id,
        Name = document.Name,
        Subject = document.Subject,
        Level = document.Level,
        Description = document.Description,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt,
        Lessons = document.Lessons.Select(ToDomain).ToList()
    };

    private static CourseLesson ToDomain(CourseLessonDocument document) => new()
    {
        Id = document.Id,
        CourseId = document.CourseId,
        LessonId = document.LessonId,
        Order = document.Order
    };

    private static CourseDocument ToDocument(Course course) => new()
    {
        Id = course.Id,
        Name = course.Name,
        Subject = course.Subject,
        Level = course.Level,
        Description = course.Description,
        CreatedAt = course.CreatedAt,
        UpdatedAt = course.UpdatedAt,
        Lessons = course.Lessons.Select(ToDocument).ToList()
    };

    private static CourseLessonDocument ToDocument(CourseLesson lesson) => new()
    {
        Id = lesson.Id,
        CourseId = lesson.CourseId,
        LessonId = lesson.LessonId,
        Order = lesson.Order
    };
}

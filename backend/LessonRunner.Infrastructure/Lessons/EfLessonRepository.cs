using System.Text.Json;
using LessonRunner.Application.Lessons;
using LessonRunner.Domain.Lessons;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Lessons;

internal sealed class EfLessonRepository(AppDbContext dbContext) : ILessonRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<Lesson>> ListAsync(CancellationToken cancellationToken)
    {
        var documents = await dbContext.Lessons
            .AsNoTracking()
            .OrderBy(lesson => lesson.Subject)
            .ThenBy(lesson => lesson.Title)
            .ToListAsync(cancellationToken);

        return documents
            .Select(document => JsonSerializer.Deserialize<Lesson>(document.DocumentJson, JsonOptions))
            .OfType<Lesson>()
            .ToList();
    }

    public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(lesson => lesson.Id == id, cancellationToken);

        return document is null
            ? null
            : JsonSerializer.Deserialize<Lesson>(document.DocumentJson, JsonOptions);
    }

    public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken)
    {
        dbContext.Lessons.Add(ToDocument(lesson));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Lessons.FirstOrDefaultAsync(lesson => lesson.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.Lessons.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateAsync(Lesson lesson, CancellationToken cancellationToken)
    {
        var document = await dbContext.Lessons.FirstOrDefaultAsync(item => item.Id == lesson.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        var updatedDocument = ToDocument(lesson);
        document.Title = updatedDocument.Title;
        document.Subject = updatedDocument.Subject;
        document.Status = updatedDocument.Status;
        document.DocumentJson = updatedDocument.DocumentJson;

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task SeedAsync(IReadOnlyList<Lesson> lessons, CancellationToken cancellationToken)
    {
        if (await dbContext.Lessons.AnyAsync(cancellationToken))
        {
            return;
        }

        foreach (var lesson in lessons)
        {
            dbContext.Lessons.Add(ToDocument(lesson));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static LessonDocument ToDocument(Lesson lesson)
    {
        return new LessonDocument
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Subject = lesson.Subject,
            Status = lesson.Status.ToString(),
            DocumentJson = JsonSerializer.Serialize(lesson, JsonOptions)
        };
    }
}

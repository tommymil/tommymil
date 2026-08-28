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

    public async Task<IReadOnlyDictionary<Guid, string>> ListTitlesAsync(CancellationToken cancellationToken)
    {
        // Projekcja na dwie kolumny - kolumna z dokumentem JSON nie jest w ogóle czytana.
        var titles = await dbContext.Lessons
            .AsNoTracking()
            .Select(lesson => new { lesson.Id, lesson.Title })
            .ToListAsync(cancellationToken);

        return titles.ToDictionary(lesson => lesson.Id, lesson => lesson.Title);
    }

    public async Task<LessonProjectFile?> FindProjectFileByDownloadTokenAsync(
        string token,
        CancellationToken cancellationToken)
    {
        // Klucz siedzi wewnątrz dokumentu JSON, więc bazie zlecamy zawężenie po treści,
        // a deserializujemy dopiero garstkę kandydatów. `LIKE` służy wyłącznie do zawężenia -
        // rozstrzyga porównanie dokładne niżej, więc ewentualne trafienie przypadkowe
        // niczego nie otwiera.
        var pattern = $"%{EscapeForLike(token)}%";
        var documents = await dbContext.Lessons
            .AsNoTracking()
            .Where(lesson => EF.Functions.Like(lesson.DocumentJson, pattern, LikeEscapeCharacter))
            .ToListAsync(cancellationToken);

        return documents
            .Select(document => JsonSerializer.Deserialize<Lesson>(document.DocumentJson, JsonOptions))
            .OfType<Lesson>()
            .SelectMany(lesson => new[] { lesson.ProjectFiles.Starter, lesson.ProjectFiles.Final })
            .OfType<LessonProjectFile>()
            .FirstOrDefault(file => string.Equals(file.DownloadToken, token, StringComparison.Ordinal));
    }

    private const string LikeEscapeCharacter = "\\";

    /// <summary>Klucze pobierania są w base64url, więc zawierają `_` — a to znak wieloznaczny
    /// w `LIKE`. Bez wygaszenia jeden klucz pasowałby do wielu dokumentów.</summary>
    private static string EscapeForLike(string value) => value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("%", "\\%", StringComparison.Ordinal)
        .Replace("_", "\\_", StringComparison.Ordinal);

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

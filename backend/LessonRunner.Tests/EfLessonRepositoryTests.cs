using LessonRunner.Application.Lessons;
using LessonRunner.Infrastructure.Lessons;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Testy round-trip repozytorium EF na bazie SQLite in-memory.
/// Weryfikują, że lekcja serializuje się do dokumentu i wraca bez utraty danych.
/// </summary>
public sealed class EfLessonRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public EfLessonRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new AppDbContext(_options);
        context.Database.EnsureCreated();
    }

    private AppDbContext CreateContext() => new(_options);

    /// <summary>
    /// Trasa `/download/lesson-files/{token}` jest anonimowa i dotąd wczytywała wszystkie
    /// konspekty, żeby znaleźć w nich jeden plik. Zawężenie idzie teraz do bazy, ale klucze
    /// pobierania są w base64url — zawierają `_`, czyli znak wieloznaczny w `LIKE`.
    /// Bez wygaszenia go jeden klucz pasowałby do cudzych dokumentów.
    /// </summary>
    [Fact]
    public async Task FindProjectFileByDownloadToken_MatchesExactly_EvenWithLikeWildcardsInToken()
    {
        var wanted = TestData.ValidLesson().ToTestLesson();
        wanted.Title = "Z plikiem";
        wanted.ProjectFiles.Starter = new LessonRunner.Domain.Lessons.LessonProjectFile
        {
            Label = "Materiał startowy",
            Url = "/uploads/aaa.sb3",
            FileName = "start.sb3",
            ContentType = "application/octet-stream",
            SizeBytes = 10,
            DownloadToken = "a_b-cDEF1234567890abcdefGH"
        };

        var other = TestData.ValidLesson().ToTestLesson();
        other.Title = "Inna lekcja";
        other.ProjectFiles.Final = new LessonRunner.Domain.Lessons.LessonProjectFile
        {
            Label = "Wersja końcowa",
            Url = "/uploads/bbb.sb3",
            FileName = "koniec.sb3",
            ContentType = "application/octet-stream",
            SizeBytes = 20,
            DownloadToken = "aXb-cDEF1234567890abcdefGH"
        };

        await using (var context = CreateContext())
        {
            var repository = new EfLessonRepository(context);
            await repository.AddAsync(wanted, CancellationToken.None);
            await repository.AddAsync(other, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var repository = new EfLessonRepository(context);

            var found = await repository.FindProjectFileByDownloadTokenAsync(
                "a_b-cDEF1234567890abcdefGH", CancellationToken.None);

            // `_` w kluczu nie może zadziałać jak wieloznacznik i trafić w drugą lekcję.
            Assert.NotNull(found);
            Assert.Equal("start.sb3", found!.FileName);

            Assert.Null(await repository.FindProjectFileByDownloadTokenAsync(
                "nieistniejacy-klucz-000000", CancellationToken.None));
        }
    }

    [Fact]
    public async Task AddThenGetById_RoundTripsFullLesson()
    {
        var lesson = TestData.ValidLesson().ToTestLesson();

        await using (var context = CreateContext())
        {
            var repository = new EfLessonRepository(context);
            await repository.AddAsync(lesson, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var repository = new EfLessonRepository(context);
            var loaded = await repository.GetByIdAsync(lesson.Id, CancellationToken.None);

            Assert.NotNull(loaded);
            Assert.Equal(lesson.Title, loaded!.Title);
            Assert.Equal(lesson.Subject, loaded.Subject);
            Assert.Equal(2, loaded.Steps.Count);
            Assert.Equal(["scratch", "gra"], loaded.Tags);
            var firstStep = loaded.Steps.Single(step => step.Order == 1);
            Assert.Equal(2, firstStep.Resources.Count);
            Assert.Single(firstStep.Notes);
            Assert.Single(firstStep.StudentItems);
        }
    }

    [Fact]
    public async Task Update_OverwritesContent()
    {
        var lesson = TestData.ValidLesson().ToTestLesson();
        await using (var context = CreateContext())
        {
            await new EfLessonRepository(context).AddAsync(lesson, CancellationToken.None);
        }

        lesson.Title = "Po edycji";
        await using (var context = CreateContext())
        {
            var ok = await new EfLessonRepository(context).UpdateAsync(lesson, CancellationToken.None);
            Assert.True(ok);
        }

        await using (var context = CreateContext())
        {
            var loaded = await new EfLessonRepository(context).GetByIdAsync(lesson.Id, CancellationToken.None);
            Assert.Equal("Po edycji", loaded!.Title);
        }
    }

    [Fact]
    public async Task Delete_RemovesLesson()
    {
        var lesson = TestData.ValidLesson().ToTestLesson();
        await using (var context = CreateContext())
        {
            await new EfLessonRepository(context).AddAsync(lesson, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var deleted = await new EfLessonRepository(context).DeleteAsync(lesson.Id, CancellationToken.None);
            Assert.True(deleted);
        }

        await using (var context = CreateContext())
        {
            var loaded = await new EfLessonRepository(context).GetByIdAsync(lesson.Id, CancellationToken.None);
            Assert.Null(loaded);
        }
    }

    public void Dispose() => _connection.Dispose();
}

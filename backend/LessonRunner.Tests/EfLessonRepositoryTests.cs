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

    [Fact]
    public async Task Seed_IsIdempotent()
    {
        var seed = new[] { TestData.ValidLesson().ToTestLesson() };

        await using (var context = CreateContext())
        {
            await new EfLessonRepository(context).SeedAsync(seed, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            await new EfLessonRepository(context).SeedAsync(seed, CancellationToken.None);
        }

        await using (var context = CreateContext())
        {
            var all = await new EfLessonRepository(context).ListAsync(CancellationToken.None);
            Assert.Single(all);
        }
    }

    public void Dispose() => _connection.Dispose();
}

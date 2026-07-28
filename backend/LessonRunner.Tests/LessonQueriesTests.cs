using LessonRunner.Application.Lessons;
using Xunit;

namespace LessonRunner.Tests;

public sealed class LessonQueriesTests
{
    [Fact]
    public async Task GetSummariesAsync_OrdersByExplicitOrder()
    {
        var repository = new InMemoryLessonRepository();
        var commands = new LessonCommands(repository, new LessonQueries(repository), new InMemoryGroupRepository());
        await commands.CreateAsync(TestData.ValidLesson(title: "Zebra", subject: "Scratch") with { Order = 3 }, CancellationToken.None);
        await commands.CreateAsync(TestData.ValidLesson(title: "Alfabet", subject: "Scratch") with { Order = 1 }, CancellationToken.None);
        await commands.CreateAsync(TestData.ValidLesson(title: "Cokolwiek", subject: "Python") with { Order = 2 }, CancellationToken.None);

        var queries = new LessonQueries(repository);
        var summaries = await queries.GetSummariesAsync(CancellationToken.None);

        Assert.Equal(
            ["Alfabet", "Cokolwiek", "Zebra"],
            summaries.Select(summary => summary.Title).ToArray());
    }

    [Fact]
    public async Task GetSummariesAsync_AssignsSequentialOrder_WhenNotSpecified()
    {
        var repository = new InMemoryLessonRepository();
        var commands = new LessonCommands(repository, new LessonQueries(repository), new InMemoryGroupRepository());
        await commands.CreateAsync(TestData.ValidLesson(title: "Pierwsza"), CancellationToken.None);
        await commands.CreateAsync(TestData.ValidLesson(title: "Druga"), CancellationToken.None);

        var queries = new LessonQueries(repository);
        var summaries = await queries.GetSummariesAsync(CancellationToken.None);

        Assert.Equal(
            ["Pierwsza", "Druga"],
            summaries.Select(summary => summary.Title).ToArray());
        Assert.Equal([1, 2], summaries.Select(summary => summary.Order).ToArray());
    }

    [Fact]
    public async Task GetSummariesAsync_AssignsOrderPerSubject_StartsFromOneForEach()
    {
        var repository = new InMemoryLessonRepository();
        var commands = new LessonCommands(repository, new LessonQueries(repository), new InMemoryGroupRepository());
        await commands.CreateAsync(TestData.ValidLesson(title: "S1", subject: "Scratch"), CancellationToken.None);
        await commands.CreateAsync(TestData.ValidLesson(title: "S2", subject: "Scratch"), CancellationToken.None);
        await commands.CreateAsync(TestData.ValidLesson(title: "M1", subject: "Minecraft"), CancellationToken.None);
        await commands.CreateAsync(TestData.ValidLesson(title: "M2", subject: "Minecraft"), CancellationToken.None);

        var queries = new LessonQueries(repository);
        var summaries = await queries.GetSummariesAsync(CancellationToken.None);

        Assert.Equal(1, summaries.First(s => s.Title == "S1").Order);
        Assert.Equal(2, summaries.First(s => s.Title == "S2").Order);
        Assert.Equal(1, summaries.First(s => s.Title == "M1").Order);
        Assert.Equal(2, summaries.First(s => s.Title == "M2").Order);
    }

    [Fact]
    public async Task GetSummariesAsync_MapsDurationAndStepCount()
    {
        var repository = new InMemoryLessonRepository();
        var commands = new LessonCommands(repository, new LessonQueries(repository), new InMemoryGroupRepository());
        await commands.CreateAsync(TestData.ValidLesson(), CancellationToken.None);

        var queries = new LessonQueries(repository);
        var summary = Assert.Single(await queries.GetSummariesAsync(CancellationToken.None));

        Assert.Equal(2, summary.StepCount);
        Assert.Equal(15, summary.DurationMinutes);
        Assert.Equal("draft", summary.Status);
    }

    [Fact]
    public async Task GetDetailsAsync_ReturnsNull_WhenMissing()
    {
        var repository = new InMemoryLessonRepository();
        var queries = new LessonQueries(repository);

        var result = await queries.GetDetailsAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }
}

using LessonRunner.Application.Auth;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Participants;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LessonRunner.Tests;

public sealed class DatabaseInitializationTests
{
    [Fact]
    public async Task Startup_CreatesOnlyBootstrapAdmin_WithoutDemoData()
    {
        await using var factory = new ApiFactory();
        using var scope = factory.Services.CreateScope();

        var lessons = scope.ServiceProvider.GetRequiredService<ILessonRepository>();
        var groups = scope.ServiceProvider.GetRequiredService<IGroupRepository>();
        var participants = scope.ServiceProvider.GetRequiredService<IParticipantRepository>();
        var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        Assert.Empty(await lessons.ListAsync(CancellationToken.None));
        Assert.Empty(await groups.ListAsync(CancellationToken.None));
        Assert.Empty(await participants.ListAsync(CancellationToken.None));

        var bootstrapUser = Assert.Single(await users.ListAsync(CancellationToken.None));
        Assert.Equal(ApiFactory.AdminEmail, bootstrapUser.Email);
    }
}

using System.Net;
using LessonRunner.Domain.Users;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Regresje na uprawnienia ról. Te ścieżki przechodzą przez realny pipeline HTTP, bo błędy
/// tego rodzaju (np. rodzic czytający konspekty) nie są widoczne w testach serwisów -
/// polityka autoryzacji jest przypięta do endpointu, nie do klasy serwisu.
/// </summary>
public sealed class ApiAuthorizationTests
{
    [Fact]
    public async Task Lessons_RequireAuthentication()
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/lessons");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Parent_CannotReadLessons()
    {
        await using var factory = new ApiFactory();
        var parent = await factory.CreateClientForAsync(UserRole.Parent, "rodzic@test.local");

        var response = await parent.GetAsync("/api/lessons");

        // Konspekt zawiera scenariusz prowadzenia i notatki instruktora - rodzic nie ma tam czego szukać.
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Parent_CannotReadScheduleOrCalendar()
    {
        await using var factory = new ApiFactory();
        var parent = await factory.CreateClientForAsync(UserRole.Parent, "rodzic2@test.local");

        Assert.Equal(HttpStatusCode.Forbidden, (await parent.GetAsync("/api/schedule")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await parent.GetAsync("/api/calendar")).StatusCode);
    }

    [Fact]
    public async Task Instructor_ReadsLessons_ButCannotAdminister()
    {
        await using var factory = new ApiFactory();
        var instructor = await factory.CreateClientForAsync(UserRole.Instructor, "trener@test.local");

        Assert.Equal(HttpStatusCode.OK, (await instructor.GetAsync("/api/lessons")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await instructor.GetAsync("/api/schedule")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await instructor.GetAsync("/api/users")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await instructor.GetAsync("/api/groups")).StatusCode);
    }

    [Fact]
    public async Task Instructor_CannotOpenParentPortal()
    {
        await using var factory = new ApiFactory();
        var instructor = await factory.CreateClientForAsync(UserRole.Instructor, "trener2@test.local");

        var response = await instructor.GetAsync("/api/parent/portal");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Admin_ReachesAdministrativeEndpoints()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();

        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/lessons")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/users")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/groups")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/dashboard")).StatusCode);
    }

    [Fact]
    public async Task Health_IsAnonymous_AndHidesInternals()
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("Data Source", body, StringComparison.OrdinalIgnoreCase);
    }
}

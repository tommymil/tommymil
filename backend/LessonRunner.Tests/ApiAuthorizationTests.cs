using System.Net;
using System.Net.Http.Json;
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
    public async Task PasswordReset_IsAnonymous_AndDoesNotRevealWhoHasAnAccount()
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateClient();
        await factory.CreateClientForAsync(UserRole.Parent, "istnieje@test.local");

        var known = await client.PostAsJsonAsync("/api/auth/password-reset", new { email = "istnieje@test.local" });
        var unknown = await client.PostAsJsonAsync("/api/auth/password-reset", new { email = "nie-ma@test.local" });

        // Ta sama odpowiedź w obu przypadkach - inaczej formularz „nie pamiętam hasła" byłby
        // wygodnym sprawdzaczem, kto korzysta ze szkoły.
        Assert.Equal(HttpStatusCode.Accepted, known.StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, unknown.StatusCode);
        Assert.Equal(await known.Content.ReadAsStringAsync(), await unknown.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task UnknownResetToken_Returns404_ForAnonymousCaller()
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateClient();

        var describe = await client.GetAsync("/api/auth/password-reset/nieistniejacy-token");
        var confirm = await client.PostAsJsonAsync(
            "/api/auth/password-reset/confirm",
            new { token = "nieistniejacy-token", password = "haslo-testowe-123" });

        Assert.Equal(HttpStatusCode.NotFound, describe.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, confirm.StatusCode);
    }

    [Fact]
    public async Task Invite_IsAdminOnly()
    {
        await using var factory = new ApiFactory();
        var instructor = await factory.CreateClientForAsync(UserRole.Instructor, "trener3@test.local");
        var parent = await factory.CreateClientForAsync(UserRole.Parent, "rodzic3@test.local");
        var target = Guid.NewGuid();

        Assert.Equal(HttpStatusCode.Forbidden, (await instructor.PostAsync($"/api/users/{target}/invite", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await parent.PostAsync($"/api/users/{target}/invite", null)).StatusCode);
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            (await factory.CreateClient().PostAsync($"/api/users/{target}/invite", null)).StatusCode);
    }

    [Fact]
    public async Task AccountWithoutPassword_CannotLogIn_UntilInvitationIsUsed()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();

        var created = await admin.PostAsJsonAsync(
            "/api/users",
            new { email = "bez-hasla@test.local", password = (string?)null, role = "parent" });

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        // Konto istnieje, ale nie ma hasła, które ktokolwiek zna - puste też nie działa.
        var attempt = await factory.CreateClient().PostAsJsonAsync(
            "/api/auth/login",
            new { email = "bez-hasla@test.local", password = "" });

        Assert.Equal(HttpStatusCode.Unauthorized, attempt.StatusCode);
    }

    [Fact]
    public async Task Progress_IsStaffOnly()
    {
        await using var factory = new ApiFactory();
        var parent = await factory.CreateClientForAsync(UserRole.Parent, "rodzic4@test.local");
        var instructor = await factory.CreateClientForAsync(UserRole.Instructor, "trener4@test.local");
        var participant = Guid.NewGuid();

        // Rodzic dostaje swoje dane w portalu, nie w panelu pracowniczym.
        Assert.Equal(HttpStatusCode.Forbidden, (await parent.GetAsync($"/api/progress/participants/{participant}")).StatusCode);
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            (await factory.CreateClient().GetAsync($"/api/progress/participants/{participant}")).StatusCode);

        // Instruktor przechodzi politykę, ale nie zna tego dziecka - 404, nie 403: nie ma prawa
        // wiedzieć, czy takie dziecko w ogóle istnieje.
        Assert.Equal(HttpStatusCode.NotFound, (await instructor.GetAsync($"/api/progress/participants/{participant}")).StatusCode);
    }

    /// <summary>
    /// Wyszukiwarka globalna zwraca listę dzieci i grup, więc rola `Parent` nie ma tu wstępu.
    /// Pole wyszukiwania byłoby najprostszą drogą do listy cudzych dzieci.
    /// </summary>
    [Fact]
    public async Task Search_IsStaffOnly()
    {
        await using var factory = new ApiFactory();
        var parent = await factory.CreateClientForAsync(UserRole.Parent, "rodzic5@test.local");
        var instructor = await factory.CreateClientForAsync(UserRole.Instructor, "trener5@test.local");
        var admin = await factory.CreateAdminClientAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, (await factory.CreateClient().GetAsync("/api/search?q=zosia")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await parent.GetAsync("/api/search?q=zosia")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await instructor.GetAsync("/api/search?q=zosia")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/search?q=zosia")).StatusCode);
    }

    /// <summary>
    /// Zbyt krótka fraza nie może uruchamiać przeglądania całej bazy - przy dwóch znakach
    /// wynik i tak byłby listą wszystkiego.
    /// </summary>
    [Fact]
    public async Task Search_IgnoresTooShortQuery()
    {
        await using var factory = new ApiFactory();
        var admin = await factory.CreateAdminClientAsync();

        var response = await admin.GetAsync("/api/search?q=a");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"results\":[]", body.Replace(" ", ""));
    }

    /// <summary>
    /// Zgody dziecka zmienia wyłącznie powiązany opiekun. Personel ma do tego panel
    /// uczestników, a nie trasę portalu rodzica.
    /// </summary>
    [Fact]
    public async Task ParentConsents_AreParentOnly()
    {
        await using var factory = new ApiFactory();
        var parent = await factory.CreateClientForAsync(UserRole.Parent, "rodzic6@test.local");
        var instructor = await factory.CreateClientForAsync(UserRole.Instructor, "trener6@test.local");
        var payload = new { participantId = Guid.NewGuid(), imageConsent = true };

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            (await factory.CreateClient().PutAsJsonAsync("/api/parent/consents", payload)).StatusCode);
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (await instructor.PutAsJsonAsync("/api/parent/consents", payload)).StatusCode);

        // Rodzic przechodzi politykę, ale nie jest powiązany z tym dzieckiem - 404, nie 403:
        // nie ma prawa wiedzieć, czy cudze dziecko w ogóle istnieje.
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await parent.PutAsJsonAsync("/api/parent/consents", payload)).StatusCode);
    }

    /// <summary>
    /// Znacznik pracy na żywo siedzi na trasie personelu i dodatkowo sprawdza właściciela
    /// terminu - tak samo jak reszta operacji kokpitu.
    /// </summary>
    [Fact]
    public async Task LiveStatus_IsStaffOnly_AndChecksSessionOwner()
    {
        await using var factory = new ApiFactory();
        var parent = await factory.CreateClientForAsync(UserRole.Parent, "rodzic7@test.local");
        var instructor = await factory.CreateClientForAsync(UserRole.Instructor, "trener7@test.local");
        var sessionId = Guid.NewGuid();
        var payload = new { participantId = Guid.NewGuid(), liveStatus = "needshelp" };

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            (await factory.CreateClient().PutAsJsonAsync($"/api/schedule/{sessionId}/live-status", payload)).StatusCode);
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (await parent.PutAsJsonAsync($"/api/schedule/{sessionId}/live-status", payload)).StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await instructor.PutAsJsonAsync($"/api/schedule/{sessionId}/live-status", payload)).StatusCode);
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

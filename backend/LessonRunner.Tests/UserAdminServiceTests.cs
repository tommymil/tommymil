using LessonRunner.Application.Auth;
using LessonRunner.Application.Groups;
using LessonRunner.Infrastructure.Auth;
using Xunit;

namespace LessonRunner.Tests;

public sealed class UserAdminServiceTests
{
    private static UserAdminService BuildService(out InMemoryUserRepository repository)
    {
        repository = new InMemoryUserRepository();
        return new UserAdminService(repository, new Pbkdf2PasswordHasher());
    }

    /// <summary>
    /// Regresja: nierozpoznana rola wpadała cicho na `Instructor`. Literówka w formularzu
    /// zakładała więc rodzicowi konto personelu - z dostępem do konspektów, grafiku,
    /// materiałów i wyszukiwarki po dzieciach.
    /// </summary>
    [Theory]
    [InlineData("wizard")]
    [InlineData("")]
    [InlineData(null)]
    // `Enum.TryParse` przyjmuje też liczby, a dla wartości spoza zakresu zwraca `true`
    // z nieistniejącym elementem. Stąd dodatkowe `Enum.IsDefined` w walidacji.
    [InlineData("99")]
    public async Task CreateAsync_Throws_ForUnknownRole(string? role)
    {
        var service = BuildService(out var repository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(new CreateUserDto("kto@x.pl", "password123", role!), CancellationToken.None));

        Assert.Empty(await repository.ListAsync(CancellationToken.None));
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("Instructor")]
    [InlineData("PARENT")]
    public async Task CreateAsync_AcceptsKnownRoles_RegardlessOfCasing(string role)
    {
        var service = BuildService(out _);

        var user = await service.CreateAsync(new CreateUserDto("kto@x.pl", "password123", role), CancellationToken.None);

        Assert.Equal(role.ToLowerInvariant(), user.Role);
    }

    [Fact]
    public async Task SetRoleAsync_ChangesRole_AndInvalidatesActiveSessions()
    {
        var service = BuildService(out var repository);
        var admin = await service.CreateAsync(new CreateUserDto("admin@x.pl", "password123", "admin"), CancellationToken.None);
        var wrong = await service.CreateAsync(new CreateUserDto("rodzic@x.pl", "password123", "instructor"), CancellationToken.None);
        var stampBefore = (await repository.GetByIdAsync(wrong.Id, CancellationToken.None))!.SecurityStamp;

        Assert.True(await service.SetRoleAsync(wrong.Id, "parent", admin.Id, CancellationToken.None));

        var updated = (await repository.GetByIdAsync(wrong.Id, CancellationToken.None))!;
        Assert.Equal(LessonRunner.Domain.Users.UserRole.Parent, updated.Role);

        // Rola jedzie w tokenie - bez nowego znacznika konto zostałoby personelem do 12 h.
        Assert.NotEqual(stampBefore, updated.SecurityStamp);
    }

    [Fact]
    public async Task SetRoleAsync_Throws_ForUnknownRole()
    {
        var service = BuildService(out _);
        var admin = await service.CreateAsync(new CreateUserDto("admin@x.pl", "password123", "admin"), CancellationToken.None);
        var other = await service.CreateAsync(new CreateUserDto("kto@x.pl", "password123", "parent"), CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SetRoleAsync(other.Id, "wizard", admin.Id, CancellationToken.None));
    }

    [Fact]
    public async Task SetRoleAsync_RefusesToDemoteOwnAccount()
    {
        var service = BuildService(out _);
        await service.CreateAsync(new CreateUserDto("admin1@x.pl", "password123", "admin"), CancellationToken.None);
        var second = await service.CreateAsync(new CreateUserDto("admin2@x.pl", "password123", "admin"), CancellationToken.None);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SetRoleAsync(second.Id, "instructor", second.Id, CancellationToken.None));

        Assert.Contains("własnego konta", error.Message);
    }

    [Fact]
    public async Task SetRoleAsync_RefusesToDemoteLastActiveAdmin()
    {
        var service = BuildService(out var repository);
        var admin = await service.CreateAsync(new CreateUserDto("admin@x.pl", "password123", "admin"), CancellationToken.None);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SetRoleAsync(admin.Id, "instructor", Guid.NewGuid(), CancellationToken.None));

        Assert.Contains("jedyne aktywne konto administratora", error.Message);
        Assert.True(await repository.HasAnyAdminAsync(CancellationToken.None));
    }

    [Fact]
    public async Task SetRoleAsync_ReturnsFalse_ForUnknownAccount()
    {
        var service = BuildService(out _);

        Assert.False(await service.SetRoleAsync(Guid.NewGuid(), "parent", Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task SetActiveAsync_RefusesToDeactivateLastActiveAdmin()
    {
        var service = BuildService(out var repository);
        var admin = await service.CreateAsync(new CreateUserDto("admin@x.pl", "password123", "admin"), CancellationToken.None);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SetActiveAsync(admin.Id, false, null, CancellationToken.None));

        Assert.Contains("jedyne aktywne konto administratora", error.Message);
        Assert.True(await repository.HasAnyAdminAsync(CancellationToken.None));
    }

    [Fact]
    public async Task SetActiveAsync_AllowsDeactivatingAdmin_WhenAnotherAdminStaysActive()
    {
        var service = BuildService(out var repository);
        var first = await service.CreateAsync(new CreateUserDto("admin1@x.pl", "password123", "admin"), CancellationToken.None);
        var second = await service.CreateAsync(new CreateUserDto("admin2@x.pl", "password123", "admin"), CancellationToken.None);

        Assert.True(await service.SetActiveAsync(first.Id, false, second.Id, CancellationToken.None));
        Assert.Equal(1, await repository.CountActiveAdminsAsync(CancellationToken.None));
    }

    [Fact]
    public async Task SetActiveAsync_RefusesSelfDeactivation()
    {
        var service = BuildService(out _);
        await service.CreateAsync(new CreateUserDto("admin1@x.pl", "password123", "admin"), CancellationToken.None);
        var second = await service.CreateAsync(new CreateUserDto("admin2@x.pl", "password123", "admin"), CancellationToken.None);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SetActiveAsync(second.Id, false, second.Id, CancellationToken.None));

        Assert.Contains("własnego konta", error.Message);
    }

    [Fact]
    public async Task HasAnyAdminAsync_IgnoresInactiveAdmins()
    {
        var service = BuildService(out var repository);
        await service.CreateAsync(new CreateUserDto("admin1@x.pl", "password123", "admin"), CancellationToken.None);
        var second = await service.CreateAsync(new CreateUserDto("admin2@x.pl", "password123", "admin"), CancellationToken.None);

        await service.SetActiveAsync(second.Id, false, null, CancellationToken.None);
        Assert.True(await repository.HasAnyAdminAsync(CancellationToken.None));

        // Ostatniego aktywnego admina serwis chroni, ale repozytorium musi widzieć prawdę:
        // gdy wszystkie konta są nieaktywne, bootstrap ma prawo odtworzyć administratora.
        await repository.SetActiveAsync((await service.ListAsync(CancellationToken.None))
            .First(user => user.IsActive && user.Role == "admin").Id, false, CancellationToken.None);

        Assert.False(await repository.HasAnyAdminAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_AddsActiveUser_AndShowsInList()
    {
        var service = BuildService(out _);

        var created = await service.CreateAsync(new CreateUserDto("Nowy@X.PL", "password123", "instructor"), CancellationToken.None);

        Assert.Equal("nowy@x.pl", created.Email);
        Assert.Equal("instructor", created.Role);
        Assert.True(created.IsActive);

        var all = await service.ListAsync(CancellationToken.None);
        Assert.Contains(all, item => item.Id == created.Id);
    }

    [Fact]
    public async Task CreateAsync_StoresProfile_AndBuildsDisplayName()
    {
        var service = BuildService(out _);

        var created = await service.CreateAsync(
            new CreateUserDto("trener@x.pl", "password123", "instructor", FirstName: "Anna", LastName: "Nowak", Phone: "600100200"),
            CancellationToken.None);

        Assert.Equal("Anna", created.FirstName);
        Assert.Equal("Nowak", created.LastName);
        Assert.Equal("Anna Nowak", created.DisplayName);
    }

    [Fact]
    public async Task CreateAsync_DisplayNameFallsBackToEmail_WhenNoProfile()
    {
        var service = BuildService(out _);

        var created = await service.CreateAsync(new CreateUserDto("solo@x.pl", "password123", "instructor"), CancellationToken.None);

        Assert.Equal("solo@x.pl", created.DisplayName);
    }

    [Fact]
    public async Task UpdateProfileAsync_ChangesNameShownInList()
    {
        var service = BuildService(out _);
        var created = await service.CreateAsync(new CreateUserDto("t@x.pl", "password123", "instructor"), CancellationToken.None);

        Assert.True(await service.UpdateProfileAsync(created.Id, new UpdateUserProfileDto("Jan", "Kowalski", "601"), CancellationToken.None));

        var listed = (await service.ListAsync(CancellationToken.None)).Single(item => item.Id == created.Id);
        Assert.Equal("Jan Kowalski", listed.DisplayName);
        Assert.Equal("601", listed.Phone);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateEmail()
    {
        var service = BuildService(out _);
        await service.CreateAsync(new CreateUserDto("dup@x.pl", "password123", "instructor"), CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(new CreateUserDto("DUP@x.pl", "password123", "instructor"), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WithoutPassword_CreatesAccountNobodyCanLogIntoYet()
    {
        var service = BuildService(out var repository);
        var hasher = new Pbkdf2PasswordHasher();

        var created = await service.CreateAsync(new CreateUserDto("rodzic@x.pl", null, "parent"), CancellationToken.None);

        var stored = await repository.GetByIdAsync(created.Id, CancellationToken.None);
        Assert.NotNull(stored);
        Assert.NotEmpty(stored!.PasswordHash);

        // Konto czeka na zaproszenie. Nie ma osobnego stanu „brak hasła” - jest skrót z losowego
        // ciągu, którego nikt nie zna - więc żadne oczywiste hasło nie otworzy tego konta.
        foreach (var guess in new[] { "", " ", "password123", stored.Email })
        {
            Assert.False(hasher.Verify(guess, stored.PasswordHash));
        }
    }

    [Fact]
    public async Task CreateAsync_RejectsShortPassword()
    {
        var service = BuildService(out _);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(new CreateUserDto("ok@x.pl", "short", "instructor"), CancellationToken.None));
    }

    [Fact]
    public async Task DeactivatedUser_CannotLogIn()
    {
        var repository = new InMemoryUserRepository();
        var hasher = new Pbkdf2PasswordHasher();
        var admin = new UserAdminService(repository, hasher);
        var auth = new AuthService(repository, hasher, new FakeTokenService());

        var created = await admin.CreateAsync(new CreateUserDto("t@x.pl", "password123", "instructor"), CancellationToken.None);
        Assert.NotNull(await auth.AuthenticateAsync(new LoginDto("t@x.pl", "password123"), CancellationToken.None));

        await admin.SetActiveAsync(created.Id, false, null, CancellationToken.None);

        Assert.Null(await auth.AuthenticateAsync(new LoginDto("t@x.pl", "password123"), CancellationToken.None));
    }

    [Fact]
    public async Task SetPasswordAsync_LetsUserLogInWithNewPassword()
    {
        var repository = new InMemoryUserRepository();
        var hasher = new Pbkdf2PasswordHasher();
        var admin = new UserAdminService(repository, hasher);
        var auth = new AuthService(repository, hasher, new FakeTokenService());

        var created = await admin.CreateAsync(new CreateUserDto("t@x.pl", "password123", "instructor"), CancellationToken.None);

        await admin.SetPasswordAsync(created.Id, "nowehaslo123", CancellationToken.None);

        Assert.Null(await auth.AuthenticateAsync(new LoginDto("t@x.pl", "password123"), CancellationToken.None));
        Assert.NotNull(await auth.AuthenticateAsync(new LoginDto("t@x.pl", "nowehaslo123"), CancellationToken.None));
    }

    [Fact]
    public async Task SetPasswordAsync_RejectsShortPassword()
    {
        var service = BuildService(out _);
        var created = await service.CreateAsync(new CreateUserDto("t@x.pl", "password123", "instructor"), CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.SetPasswordAsync(created.Id, "short", CancellationToken.None));
    }

    [Fact]
    public async Task DeactivatedInstructor_IsExcludedFromInstructorPicker()
    {
        var repository = new InMemoryUserRepository();
        var admin = new UserAdminService(repository, new Pbkdf2PasswordHasher());
        var groupService = new GroupService(new InMemoryGroupRepository(), new InMemoryLessonRepository(), repository, new InMemoryParticipantRepository());

        var active = await admin.CreateAsync(new CreateUserDto("active@x.pl", "password123", "instructor"), CancellationToken.None);
        var inactive = await admin.CreateAsync(new CreateUserDto("inactive@x.pl", "password123", "instructor"), CancellationToken.None);
        await admin.SetActiveAsync(inactive.Id, false, null, CancellationToken.None);

        var instructors = await groupService.GetInstructorsAsync(CancellationToken.None);

        Assert.Contains(instructors, item => item.Id == active.Id);
        Assert.DoesNotContain(instructors, item => item.Id == inactive.Id);
    }
}

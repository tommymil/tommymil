using LessonRunner.Application.Parents;
using LessonRunner.Domain.Parents;
using LessonRunner.Domain.Participants;
using LessonRunner.Domain.Users;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Jedno miejsce rozstrzygające, do kogo pisać w sprawie dziecka.
///
/// Przed tą zmianą dane opiekuna żyły w dwóch światach: powiadomienia szły na
/// `Participant.GuardianEmail`, a portal działał na kontach `User(Parent)`. Przy dwojgu
/// opiekunów jedno dostawałoby e-maile, a drugie widziało portal.
/// </summary>
public sealed class GuardianDirectoryTests
{
    [Fact]
    public async Task LinkedAccountsWin_OverInlineGuardianData()
    {
        var (directory, users, links) = Build();
        var dziecko = ChildWithConsent(guardianEmail: "stary-adres@example.com");

        var mama = await AddParentAsync(users, "mama@example.com", "Anna Kowalska");
        await links.AddAsync(
            new ParentParticipantLink { ParentUserId = mama.Id, ParticipantId = dziecko.Id, Relation = "mama", IsPrimaryContact = true },
            CancellationToken.None);

        var contacts = await directory.ResolveAsync([dziecko], CancellationToken.None);

        // Konto wygrywa - do kolumny przy dziecku nie sięgamy w ogóle.
        var contact = Assert.Single(contacts);
        Assert.Equal("mama@example.com", contact.Email);
        Assert.Equal("Anna Kowalska", contact.Name);
        Assert.Equal("mama", contact.Relation);
        Assert.True(contact.FromAccount);
    }

    [Fact]
    public async Task BothGuardians_GetTheirOwnContact()
    {
        var (directory, users, links) = Build();
        var dziecko = ChildWithConsent();

        var mama = await AddParentAsync(users, "mama@example.com", "Anna Kowalska");
        var tata = await AddParentAsync(users, "tata@example.com", "Piotr Kowalski");
        await links.AddAsync(
            new ParentParticipantLink { ParentUserId = mama.Id, ParticipantId = dziecko.Id, Relation = "mama", IsPrimaryContact = true },
            CancellationToken.None);
        await links.AddAsync(
            new ParentParticipantLink { ParentUserId = tata.Id, ParticipantId = dziecko.Id, Relation = "tata" },
            CancellationToken.None);

        var contacts = await directory.ResolveAsync([dziecko], CancellationToken.None);

        Assert.Equal(2, contacts.Count);
        Assert.Contains(contacts, contact => contact.Email == "mama@example.com" && contact.IsPrimaryContact);
        Assert.Contains(contacts, contact => contact.Email == "tata@example.com" && !contact.IsPrimaryContact);
    }

    [Fact]
    public async Task GuardianWhoOptedOut_IsNotContacted()
    {
        var (directory, users, links) = Build();
        var dziecko = ChildWithConsent();

        var mama = await AddParentAsync(users, "mama@example.com", "Anna Kowalska");
        var tata = await AddParentAsync(users, "tata@example.com", "Piotr Kowalski");
        await links.AddAsync(
            new ParentParticipantLink { ParentUserId = mama.Id, ParticipantId = dziecko.Id },
            CancellationToken.None);
        await links.AddAsync(
            new ParentParticipantLink { ParentUserId = tata.Id, ParticipantId = dziecko.Id, ReceivesNotifications = false },
            CancellationToken.None);

        var contacts = await directory.ResolveAsync([dziecko], CancellationToken.None);

        Assert.Equal("mama@example.com", Assert.Single(contacts).Email);
    }

    [Fact]
    public async Task FamilyWithoutAccount_FallsBackToInlineData()
    {
        var (directory, _, _) = Build();
        var dziecko = ChildWithConsent(guardianEmail: "opiekun@example.com");

        var contact = Assert.Single(await directory.ResolveAsync([dziecko], CancellationToken.None));

        Assert.Equal("opiekun@example.com", contact.Email);
        Assert.False(contact.FromAccount);
    }

    [Fact]
    public async Task WithoutDataProcessingConsent_NobodyIsContacted()
    {
        var (directory, users, links) = Build();
        var dziecko = new Participant { FirstName = "Ola", LastName = "Bez", GuardianEmail = "opiekun@example.com" };

        var mama = await AddParentAsync(users, "mama@example.com", "Anna Kowalska");
        await links.AddAsync(
            new ParentParticipantLink { ParentUserId = mama.Id, ParticipantId = dziecko.Id },
            CancellationToken.None);

        // Zgoda RODO jest warunkiem wstępnym niezależnie od źródła danych.
        Assert.Empty(await directory.ResolveAsync([dziecko], CancellationToken.None));
    }

    [Fact]
    public async Task DeactivatedParentAccount_IsSkipped()
    {
        var (directory, users, links) = Build();
        var dziecko = ChildWithConsent(guardianEmail: "zapas@example.com");

        var mama = await AddParentAsync(users, "mama@example.com", "Anna Kowalska");
        mama.IsActive = false;
        await links.AddAsync(
            new ParentParticipantLink { ParentUserId = mama.Id, ParticipantId = dziecko.Id },
            CancellationToken.None);

        // Powiązanie istnieje, więc do danych przy dziecku nie wracamy - konto jest źródłem prawdy,
        // a wyłączone konto oznacza, że tego adresu świadomie nie używamy.
        Assert.Empty(await directory.ResolveAsync([dziecko], CancellationToken.None));
    }

    private static (GuardianDirectory Directory, InMemoryUserRepository Users, InMemoryParentPortalRepository Links) Build()
    {
        var users = new InMemoryUserRepository();
        var links = new InMemoryParentPortalRepository();
        return (new GuardianDirectory(links, users), users, links);
    }

    private static Participant ChildWithConsent(string? guardianEmail = null) => new()
    {
        FirstName = "Jan",
        LastName = "Kowalski",
        GuardianEmail = guardianEmail,
        GuardianName = guardianEmail is null ? null : "Opiekun z kartoteki",
        DataProcessingConsentAt = DateTimeOffset.UtcNow
    };

    private static async Task<User> AddParentAsync(InMemoryUserRepository users, string email, string displayName)
    {
        var parts = displayName.Split(' ');
        var parent = new User
        {
            Email = email,
            PasswordHash = "h",
            Role = UserRole.Parent,
            FirstName = parts[0],
            LastName = parts.Length > 1 ? parts[1] : null
        };
        await users.AddAsync(parent, CancellationToken.None);
        return parent;
    }
}

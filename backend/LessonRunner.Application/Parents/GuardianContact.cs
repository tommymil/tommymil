using LessonRunner.Application.Auth;
using LessonRunner.Domain.Participants;

namespace LessonRunner.Application.Parents;

/// <summary>Opiekun, do którego można napisać: adres, nazwa i skąd pochodzą te dane.</summary>
public sealed record GuardianContact(
    Guid ParticipantId,
    string Email,
    string Name,
    string? Relation,
    bool IsPrimaryContact,
    /// <summary>`true` = dane z konta opiekuna, `false` = z kolumn przy dziecku (zapas).</summary>
    bool FromAccount);

public interface IGuardianDirectory
{
    /// <summary>Opiekunowie wskazanych dzieci, do których wolno wysłać powiadomienie.</summary>
    Task<IReadOnlyList<GuardianContact>> ResolveAsync(
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken);
}

/// <summary>
/// Jedno miejsce, które odpowiada na pytanie „do kogo napisać w sprawie tego dziecka”.
///
/// Zasada rozstrzygania jest prosta i celowo bezwarunkowa:
/// **są powiązane konta opiekunów → piszemy do nich i tylko do nich; nie ma żadnego →
/// wracamy do danych wpisanych przy dziecku.**
///
/// Bez tego istniały dwa równoległe światy: powiadomienia szły na `Participant.GuardianEmail`,
/// a portal działał na kontach `User(Parent)`. Przy dwojgu opiekunów jedno z nich dostawałoby
/// e-maile, a drugie widziało portal — i nikt by nie wiedział, dlaczego.
///
/// Zgoda RODO jest warunkiem wstępnym niezależnie od źródła danych.
/// </summary>
public sealed class GuardianDirectory(
    IParentPortalRepository parentRepository,
    IUserRepository userRepository) : IGuardianDirectory
{
    public async Task<IReadOnlyList<GuardianContact>> ResolveAsync(
        IReadOnlyList<Participant> participants,
        CancellationToken cancellationToken)
    {
        var allowed = participants.Where(HasDataProcessingConsent).ToList();

        if (allowed.Count == 0)
        {
            return [];
        }

        var links = await parentRepository.ListByParticipantsAsync(
            allowed.Select(participant => participant.Id).ToList(),
            cancellationToken);

        var linksByParticipant = links
            .GroupBy(link => link.ParticipantId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var accounts = links.Count == 0
            ? []
            : (await userRepository.ListAsync(cancellationToken)).ToDictionary(user => user.Id);

        var contacts = new List<GuardianContact>();

        foreach (var participant in allowed)
        {
            var participantLinks = linksByParticipant.GetValueOrDefault(participant.Id, []);
            var fromAccounts = participantLinks
                .Where(link => link.ReceivesNotifications)
                .Select(link => (link, user: accounts.GetValueOrDefault(link.ParentUserId)))
                .Where(item => item.user is not null && item.user.IsActive && IsEmail(item.user.Email))
                .Select(item => new GuardianContact(
                    participant.Id,
                    item.user!.Email,
                    item.user.DisplayName,
                    item.link.Relation,
                    item.link.IsPrimaryContact,
                    FromAccount: true))
                .ToList();

            if (fromAccounts.Count > 0)
            {
                contacts.AddRange(fromAccounts);
                continue;
            }

            // Rodzina bez konta - korzystamy z danych wpisanych przy dziecku.
            if (participantLinks.Count == 0 && IsEmail(participant.GuardianEmail))
            {
                contacts.Add(new GuardianContact(
                    participant.Id,
                    participant.GuardianEmail!,
                    participant.GuardianName ?? "Opiekun",
                    participant.GuardianRelation,
                    IsPrimaryContact: true,
                    FromAccount: false));
            }
        }

        return contacts;
    }

    private static bool HasDataProcessingConsent(Participant participant) =>
        participant.DataProcessingConsentAt is not null;

    private static bool IsEmail(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value.Contains('@');
}

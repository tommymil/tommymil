using LessonRunner.Domain.Participants;

namespace LessonRunner.Infrastructure.Participants;

/// <summary>Demonstracyjni uczestnicy dla lokalnego developmentu (patrz GroupSeedData).</summary>
internal static class ParticipantSeedData
{
    /// <summary>Dzieci powiązane z demonstracyjnym kontem rodzica (patrz <c>UserSeedData</c>).
    /// Rodzeństwo jest tu celowo: portal rodzica pokazuje przełącznik dziecka dopiero
    /// przy dwójce, więc przy jednym dziecku ten widok nigdy nie był widziany.</summary>
    public static IReadOnlyList<Guid> ChildrenOfSeedParent =>
    [
        Guid.Parse("c1d2e3f4-0001-4a10-9a20-000000000001"),
        Guid.Parse("c1d2e3f4-0004-4a10-9a20-000000000004")
    ];

    public static IReadOnlyList<Participant> Create()
    {
        var consentedAt = DateTimeOffset.UtcNow.AddMonths(-2);

        return
        [
            new Participant
            {
                Id = Guid.Parse("c1d2e3f4-0001-4a10-9a20-000000000001"),
                FirstName = "Zofia",
                LastName = "Kowalska",
                Phone = "600100200",
                Email = "zofia@example.com",
                GuardianName = "Katarzyna Kowalska",
                GuardianPhone = "600500600",
                GuardianEmail = "parent@lessonrunner.local",
                GuardianRelation = "mama",
                DataProcessingConsentAt = consentedAt,
                ImageConsentAt = consentedAt
            },
            new Participant
            {
                Id = Guid.Parse("c1d2e3f4-0004-4a10-9a20-000000000004"),
                FirstName = "Jan",
                LastName = "Kowalski",
                Phone = null,
                Email = "jan@example.com",
                GuardianName = "Katarzyna Kowalska",
                GuardianPhone = "600500600",
                GuardianEmail = "parent@lessonrunner.local",
                GuardianRelation = "mama",
                DataProcessingConsentAt = consentedAt
            },
            new Participant
            {
                Id = Guid.Parse("c1d2e3f4-0002-4a10-9a20-000000000002"),
                FirstName = "Antoni",
                LastName = "Nowak",
                Phone = "600300400",
                Email = "antoni@example.com"
            },
            new Participant
            {
                Id = Guid.Parse("c1d2e3f4-0003-4a10-9a20-000000000003"),
                FirstName = "Maja",
                LastName = "Wiśniewska",
                Phone = null,
                Email = "maja@example.com"
            }
        ];
    }
}

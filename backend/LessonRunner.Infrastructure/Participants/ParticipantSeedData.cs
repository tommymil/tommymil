using LessonRunner.Domain.Participants;

namespace LessonRunner.Infrastructure.Participants;

/// <summary>Demonstracyjni uczestnicy dla lokalnego developmentu (patrz GroupSeedData).</summary>
internal static class ParticipantSeedData
{
    public static IReadOnlyList<Participant> Create()
    {
        return
        [
            new Participant
            {
                Id = Guid.Parse("c1d2e3f4-0001-4a10-9a20-000000000001"),
                FirstName = "Zofia",
                LastName = "Kowalska",
                Phone = "600100200",
                Email = "zofia@example.com"
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

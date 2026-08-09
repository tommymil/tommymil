using System.Globalization;
using System.Text;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Participants;

namespace LessonRunner.Application.Search;

public interface ISearchService
{
    /// <summary>Wyszukiwanie w obrębie tego, co użytkownik ma prawo widzieć.</summary>
    Task<SearchResponseDto> SearchAsync(
        Guid userId,
        bool isAdmin,
        string query,
        CancellationToken cancellationToken);
}

/// <summary>
/// Wyszukiwanie globalne (Ctrl+K).
///
/// Decyzje warte zapamiętania:
/// - **Instruktor przeszukuje wyłącznie swoje grupy** i dzieci do nich zapisane, łącznie
///   z zastępstwami. Bez tego pole wyszukiwania byłoby najprostszą drogą do listy
///   wszystkich dzieci w szkole — a to dane, do których instruktor spoza grupy nie ma prawa.
/// - **Porównujemy bez polskich znaków i bez wielkości liter.** Wpisanie „zosia" ma znaleźć
///   „Zosię"; ta sama normalizacja działa już w parserze konspektów.
/// - **Nie przeszukujemy notatek o dzieciach.** Trafiają tam uwagi o potrzebach specjalnych
///   i sytuacji rodzinnej; wyszukiwarka po nich zamieniłaby te notatki w wyszukiwalny rejestr.
/// - Wyniki są przycięte do 20 pozycji — lista, której nie da się przejrzeć jednym
///   spojrzeniem, przestaje być skrótem.
/// </summary>
public sealed class SearchService(
    IParticipantRepository participantRepository,
    IGroupRepository groupRepository,
    ILessonRepository lessonRepository) : ISearchService
{
    private const int MaxResults = 20;
    private const int MinQueryLength = 2;

    public async Task<SearchResponseDto> SearchAsync(
        Guid userId,
        bool isAdmin,
        string query,
        CancellationToken cancellationToken)
    {
        var needle = Normalize(query);

        if (needle.Length < MinQueryLength)
        {
            return new SearchResponseDto([]);
        }

        var groups = await groupRepository.ListAsync(cancellationToken);

        var visibleGroups = isAdmin
            ? groups
            : groups
                .Where(group => group.InstructorId == userId
                    || group.Sessions.Any(session => session.SubstituteInstructorId == userId))
                .ToList();

        var visibleParticipantIds = visibleGroups
            .SelectMany(group => group.Enrollments.Select(enrollment => enrollment.ParticipantId))
            .ToHashSet();

        var results = new List<SearchResultDto>();

        foreach (var group in visibleGroups.Where(group => Normalize(group.Name).Contains(needle)))
        {
            results.Add(new SearchResultDto(
                nameof(SearchResultKind.Group).ToLowerInvariant(),
                group.Id,
                group.Name,
                $"{group.Enrollments.Count} uczestników · {group.Sessions.Count} terminów",
                $"/admin/groups/{group.Id}"));
        }

        var participants = await participantRepository.ListAsync(cancellationToken);

        foreach (var participant in participants
            .Where(participant => isAdmin || visibleParticipantIds.Contains(participant.Id))
            .Where(participant => !participant.IsArchived))
        {
            var fullName = $"{participant.FirstName} {participant.LastName}";
            var matchesChild = Normalize(fullName).Contains(needle);
            var matchesGuardian = isAdmin && Normalize(participant.GuardianName).Contains(needle);

            if (matchesChild)
            {
                var groupNames = visibleGroups
                    .Where(group => group.Enrollments.Any(enrollment => enrollment.ParticipantId == participant.Id))
                    .Select(group => group.Name)
                    .ToList();

                results.Add(new SearchResultDto(
                    nameof(SearchResultKind.Participant).ToLowerInvariant(),
                    participant.Id,
                    fullName,
                    groupNames.Count == 0 ? "Bez grupy" : string.Join(", ", groupNames),
                    "/admin/participants"));
            }
            else if (matchesGuardian)
            {
                // Opiekuna pokazujemy jako drogę do dziecka - to dziecko jest tu jednostką
                // organizacyjną, a opiekun tylko kontaktem.
                results.Add(new SearchResultDto(
                    nameof(SearchResultKind.Guardian).ToLowerInvariant(),
                    participant.Id,
                    participant.GuardianName ?? "",
                    $"opiekun · {fullName}",
                    "/admin/participants"));
            }
        }

        if (isAdmin)
        {
            var lessons = await lessonRepository.ListAsync(cancellationToken);

            foreach (var lesson in lessons.Where(lesson => Normalize(lesson.Title).Contains(needle)))
            {
                results.Add(new SearchResultDto(
                    nameof(SearchResultKind.Lesson).ToLowerInvariant(),
                    lesson.Id,
                    lesson.Title,
                    lesson.Subject,
                    $"/admin/lessons/{lesson.Id}/edit"));
            }
        }

        return new SearchResponseDto(results.Take(MaxResults).ToList());
    }

    /// <summary>Małe litery bez znaków diakrytycznych - „Zosia" i „zosia" mają się znaleźć tak samo.</summary>
    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        var decomposed = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        // Polskie „ł" nie rozkłada się na literę bazową i znak diakrytyczny.
        return builder.ToString().Normalize(NormalizationForm.FormC).Replace('ł', 'l');
    }
}

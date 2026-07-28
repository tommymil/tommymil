using LessonRunner.Domain.Groups;

namespace LessonRunner.Infrastructure.Groups;

/// <summary>
/// Demonstracyjna grupa dla lokalnego developmentu - tworzona tylko gdy nie ma żadnej grupy
/// i istnieje instruktor oraz przynajmniej jedna lekcja Ready.
/// </summary>
internal static class GroupSeedData
{
    /// <summary>Tworzy demonstracyjną grupę i zapisuje do niej uczestników o podanych Id (patrz <c>ParticipantSeedData</c>).</summary>
    public static Group Create(Guid instructorId, IReadOnlyList<Guid> readyLessonIds, IReadOnlyList<Guid> participantIds)
    {
        var firstSession = NextWeekday(DateTimeOffset.Now, DayOfWeek.Monday, hour: 16);

        var group = new Group
        {
            Name = "Poniedziałkowa grupa Scratch (16:00)",
            InstructorId = instructorId
        };

        group.Enrollments = participantIds
            .Select(participantId => new GroupEnrollment { GroupId = group.Id, ParticipantId = participantId })
            .ToList();

        group.Sessions = readyLessonIds
            .Select((lessonId, index) => new ScheduledSession
            {
                GroupId = group.Id,
                LessonId = lessonId,
                ScheduledAt = firstSession.AddDays(7 * index),
                SequenceNumber = index + 1,
                Status = ScheduledSessionStatus.Planned
            })
            .ToList();

        return group;
    }

    private static DateTimeOffset NextWeekday(DateTimeOffset from, DayOfWeek weekday, int hour)
    {
        var daysAhead = ((int)weekday - (int)from.DayOfWeek + 7) % 7;
        if (daysAhead == 0)
        {
            daysAhead = 7;
        }

        var date = from.Date.AddDays(daysAhead).AddHours(hour);
        return new DateTimeOffset(date, from.Offset);
    }
}

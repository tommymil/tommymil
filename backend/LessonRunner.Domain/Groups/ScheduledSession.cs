using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Groups;

public sealed class ScheduledSession : Entity
{
    public Guid GroupId { get; set; }

    /// <summary>Lekcja przypisana do terminu (przy modelu "1 lekcja = 1 termin").</summary>
    public Guid? LessonId { get; set; }

    public DateTimeOffset ScheduledAt { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? SubstituteInstructorId { get; set; }

    /// <summary>Numer spotkania w obrębie grupy (1..N), do czytelnej kolejności.</summary>
    public int SequenceNumber { get; set; }

    public ScheduledSessionStatus Status { get; set; } = ScheduledSessionStatus.Planned;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>Notatka instruktora wpisywana przy zakończeniu zajęć.</summary>
    public string? InstructorNote { get; set; }

    /// <summary>Link do spotkania dla tego konkretnego terminu. Nadpisuje link grupy -
    /// potrzebne przy zastępstwie (inny pokój prowadzącego), odrabianiu i jednorazowej
    /// zmianie platformy. Puste = używamy linku grupy.</summary>
    public string? MeetingUrl { get; set; }

    /// <summary>Nagranie z zajęć, udostępniane rodzicom po zakończeniu terminu.</summary>
    public string? RecordingUrl { get; set; }

    public List<AttendanceRecord> Attendance { get; set; } = [];
}

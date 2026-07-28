using LessonRunner.Domain.Common;

namespace LessonRunner.Domain.Groups;

public sealed class AttendanceRecord : Entity
{
    public Guid ScheduledSessionId { get; set; }
    public Guid ParticipantId { get; set; }

    /// <summary>Pełny status obecności - źródło prawdy.</summary>
    public AttendanceStatus Status { get; set; } = AttendanceStatus.UnexcusedAbsence;

    /// <summary>
    /// Skrót „był na zajęciach", liczony ze statusu. Zostaje dla frekwencji, eksportów i KPI,
    /// które operują na jednym bicie.
    ///
    /// Ustawienie go na `true` daje status „obecny", a na `false` - „nieobecność niezgłoszona".
    /// Jeżeli chcesz zapisać spóźnienie albo problemy techniczne, ustaw <see cref="Status"/>.
    /// </summary>
    public bool Present
    {
        get => Status.CountsAsPresent();
        set => Status = value ? AttendanceStatus.Present : AttendanceStatus.UnexcusedAbsence;
    }

    /// <summary>Krótka notatka instruktora o tym dziecku na tych zajęciach,
    /// np. „dołączył 15 minut później", „mikrofon nie działał", „odrobić projekt".</summary>
    public string? Note { get; set; }

    /// <summary>Orientacyjny czas dołączenia i opuszczenia zajęć - na razie wpisywany ręcznie.</summary>
    public DateTimeOffset? JoinedAt { get; set; }
    public DateTimeOffset? LeftAt { get; set; }

    public bool MakeupRequired { get; set; }
    public Guid? MakeupSessionId { get; set; }
    public DateTimeOffset MarkedAt { get; set; } = DateTimeOffset.UtcNow;
}

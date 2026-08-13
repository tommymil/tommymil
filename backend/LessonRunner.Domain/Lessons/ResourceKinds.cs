namespace LessonRunner.Domain.Lessons;

public enum StudentItemKind
{
    Text = 0,
    Image = 1
}

public enum LessonResourceKind
{
    Code = 0,
    Link = 1,
    Image = 2,
    File = 3
}

/// <summary>
/// Rodzaje wskazówek dla prowadzącego.
///
/// `Faster` i `Shorter` powstały z rozbicia `Pace`, które mieszało dwie przeciwne sytuacje:
/// „trójka skończyła, daj im coś więcej" i „zostało dziesięć minut, co wolno wyciąć".
/// W trakcie zajęć to są dwa różne pytania i prowadzący zadaje je w różnych momentach.
/// `Pace` zostaje dla wskazówek o samym rytmie lekcji.
/// </summary>
public enum LessonNoteKind
{
    Error = 0,
    Hint = 1,
    Pace = 2,
    Faster = 3,
    Shorter = 4
}

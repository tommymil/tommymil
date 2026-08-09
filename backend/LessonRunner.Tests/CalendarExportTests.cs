using System.Text;
using LessonRunner.Application.Scheduling;
using Xunit;

namespace LessonRunner.Tests;

public sealed class CalendarExportTests
{
    private static string Ics(params CalendarEventDto[] events) =>
        Encoding.UTF8.GetString(CalendarExport.ToIcs(events, "Grafik zajęć"));

    [Fact]
    public void ToIcs_ProducesValidEnvelopeWithEvent()
    {
        var start = new DateTimeOffset(2026, 6, 15, 16, 0, 0, TimeSpan.FromHours(2));
        var ics = Ics(new CalendarEventDto(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Grupa A: Pierwsza gra",
            start,
            90,
            "Opis",
            "Sala 1",
            "https://meet.google.com/abc",
            Cancelled: false));

        Assert.StartsWith("BEGIN:VCALENDAR\r\n", ics);
        Assert.EndsWith("END:VCALENDAR\r\n", ics);
        Assert.Contains("UID:11111111-1111-1111-1111-111111111111@szkola-programowania", ics);

        // Czas zapisujemy w UTC: 16:00 +02:00 to 14:00 UTC, koniec 90 minut później.
        Assert.Contains("DTSTART:20260615T140000Z", ics);
        Assert.Contains("DTEND:20260615T153000Z", ics);
        Assert.Contains("STATUS:CONFIRMED", ics);
        Assert.Contains("URL:https://meet.google.com/abc", ics);
    }

    /// <summary>
    /// Odwołane zajęcia zostają w pliku ze statusem CANCELLED - dzięki temu przy ponownym
    /// imporcie znikają z kalendarza zamiast zostać tam jako duchy.
    /// </summary>
    [Fact]
    public void CancelledSession_IsMarkedCancelled()
    {
        var ics = Ics(new CalendarEventDto(
            Guid.NewGuid(), "Grupa A", DateTimeOffset.UtcNow, 90, null, null, null, Cancelled: true));

        Assert.Contains("STATUS:CANCELLED", ics);
    }

    [Fact]
    public void SpecialCharacters_AreEscapedPerRfc5545()
    {
        var ics = Ics(new CalendarEventDto(
            Guid.NewGuid(),
            "Grupa A; poziom 1, wtorki",
            DateTimeOffset.UtcNow,
            90,
            "Pierwsza linia\nDruga linia",
            null,
            null,
            Cancelled: false));

        Assert.Contains(@"SUMMARY:Grupa A\; poziom 1\, wtorki", ics);
        Assert.Contains(@"Pierwsza linia\nDruga linia", ics);
    }

    /// <summary>
    /// RFC 5545 wymaga zwijania linii po 75 oktetach. Bez tego długie nazwy grup albo linki
    /// potrafią wywrócić import w części kalendarzy.
    /// </summary>
    [Fact]
    public void LongLines_AreFolded_WithoutBreakingMultibyteCharacters()
    {
        var ics = Ics(new CalendarEventDto(
            Guid.NewGuid(),
            string.Concat(Enumerable.Repeat("Zajęcia z programowania dla dzieci ", 5)),
            DateTimeOffset.UtcNow,
            90,
            null,
            null,
            null,
            Cancelled: false));

        foreach (var line in ics.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
        {
            Assert.True(
                Encoding.UTF8.GetByteCount(line) <= 75,
                $"Linia dłuższa niż 75 oktetów: {line}");
        }

        // Zwinięcie nie może rozciąć polskiego znaku na pół - inaczej dostalibyśmy krzaki.
        Assert.Contains("Zaj", ics);
        Assert.DoesNotContain("�", ics);
    }

    [Fact]
    public void EmptySchedule_StillProducesValidCalendar()
    {
        var ics = Ics();

        Assert.Contains("BEGIN:VCALENDAR", ics);
        Assert.Contains("END:VCALENDAR", ics);
        Assert.DoesNotContain("BEGIN:VEVENT", ics);
    }
}

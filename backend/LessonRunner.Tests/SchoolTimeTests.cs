using LessonRunner.Application.Scheduling;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Strażnik formatowania terminów dla człowieka.
///
/// Regresja, przed którą broni: przeglądarka wysyła termin przez `toISOString()`, czyli
/// z offsetem `Z`. Wywołane wprost `DateTimeOffset.ToString("HH:mm")` drukuje wtedy godzinę
/// UTC, więc rodzic dostawał w e-mailu termin o dwie godziny wcześniejszy niż faktyczny.
/// </summary>
public sealed class SchoolTimeTests
{
    /// <summary>Środowisko bez bazy stref czasowych nie ma czego weryfikować.</summary>
    private static bool HasTimeZoneData => SchoolTime.Zone.BaseUtcOffset != TimeSpan.Zero;

    [Fact]
    public void FormatDateTime_RendersSummerInstantInCentralEuropeanSummerTime()
    {
        if (!HasTimeZoneData)
        {
            return;
        }

        var instant = new DateTimeOffset(2026, 7, 1, 16, 0, 0, TimeSpan.Zero);

        Assert.Equal("2026-07-01 18:00", SchoolTime.FormatDateTime(instant));
    }

    [Fact]
    public void FormatDateTime_RendersWinterInstantInCentralEuropeanTime()
    {
        if (!HasTimeZoneData)
        {
            return;
        }

        var instant = new DateTimeOffset(2026, 1, 15, 16, 0, 0, TimeSpan.Zero);

        Assert.Equal("2026-01-15 17:00", SchoolTime.FormatDateTime(instant));
    }

    /// <summary>
    /// Ten sam moment zapisany z różnym offsetem musi dać ten sam tekst. To jest sedno błędu:
    /// terminy generowane cotygodniowo miały offset warszawski, a pierwszy termin serii
    /// zostawał z `Z`, więc jedna seria drukowała się w mailach na dwa sposoby.
    /// </summary>
    [Fact]
    public void FormatDateTime_IgnoresOffsetOfTheStoredValue()
    {
        if (!HasTimeZoneData)
        {
            return;
        }

        var fromBrowser = new DateTimeOffset(2026, 7, 1, 16, 0, 0, TimeSpan.Zero);
        var fromGenerator = new DateTimeOffset(2026, 7, 1, 18, 0, 0, TimeSpan.FromHours(2));

        Assert.Equal(fromBrowser, fromGenerator);
        Assert.Equal(SchoolTime.FormatDateTime(fromGenerator), SchoolTime.FormatDateTime(fromBrowser));
    }

    [Fact]
    public void FormatDateTimeWithZone_AppendsTheOffsetOfTheSchoolClock()
    {
        if (!HasTimeZoneData)
        {
            return;
        }

        var instant = new DateTimeOffset(2026, 7, 1, 16, 0, 0, TimeSpan.Zero);

        Assert.Equal("2026-07-01 18:00 +02:00", SchoolTime.FormatDateTimeWithZone(instant));
    }

    [Fact]
    public void LocalDate_UsesTheSchoolClock_NotUtc()
    {
        if (!HasTimeZoneData)
        {
            return;
        }

        // 22:30 UTC to już następny dzień w Polsce - dzień wolny musi być rozpoznany po polsku.
        var lateEvening = new DateTimeOffset(2026, 7, 1, 22, 30, 0, TimeSpan.Zero);

        Assert.Equal(new DateOnly(2026, 7, 2), SchoolTime.LocalDate(lateEvening));
    }

    [Fact]
    public void FromWallClock_TakesTheOffsetOfTheTargetTime()
    {
        if (!HasTimeZoneData)
        {
            return;
        }

        var summer = SchoolTime.FromWallClock(new DateTime(2026, 7, 1, 18, 0, 0));
        var winter = SchoolTime.FromWallClock(new DateTime(2026, 1, 15, 18, 0, 0));

        Assert.Equal(TimeSpan.FromHours(2), summer.Offset);
        Assert.Equal(TimeSpan.FromHours(1), winter.Offset);
    }
}

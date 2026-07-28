namespace LessonRunner.Application.Scheduling;

public sealed record LocationDto(Guid Id, string Name, string? Description, bool IsActive);

public sealed record UpsertLocationDto(string Name, string? Description, bool IsActive = true);

public sealed record HolidayDto(Guid Id, DateOnly Date, string Name);

public sealed record UpsertHolidayDto(DateOnly Date, string Name);

public sealed record CalendarSessionDto(
    Guid Id,
    Guid GroupId,
    string GroupName,
    Guid InstructorId,
    string InstructorName,
    Guid? LessonId,
    string? LessonTitle,
    DateTimeOffset ScheduledAt,
    int SequenceNumber,
    string Status,
    string StatusLabel,
    Guid? LocationId,
    string? LocationName);

public sealed record CalendarDto(
    IReadOnlyList<CalendarSessionDto> Sessions,
    IReadOnlyList<HolidayDto> Holidays,
    IReadOnlyList<LocationDto> Locations);

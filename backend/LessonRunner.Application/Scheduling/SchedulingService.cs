using LessonRunner.Application.Auth;
using LessonRunner.Application.Groups;
using LessonRunner.Application.Lessons;
using LessonRunner.Domain.Scheduling;
using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Scheduling;

public sealed class SchedulingService(
    ISchedulingRepository schedulingRepository,
    IGroupRepository groupRepository,
    ILessonRepository lessonRepository,
    IUserRepository userRepository) : ISchedulingService
{
    public async Task<IReadOnlyList<LocationDto>> GetLocationsAsync(CancellationToken cancellationToken)
    {
        var locations = await schedulingRepository.ListLocationsAsync(cancellationToken);
        return locations.OrderBy(location => location.Name).Select(ToDto).ToList();
    }

    public async Task<LocationDto> CreateLocationAsync(UpsertLocationDto dto, CancellationToken cancellationToken)
    {
        var location = new Location
        {
            Name = NormalizeName(dto.Name, "Nazwa lokalizacji jest wymagana."),
            Description = NormalizeOptional(dto.Description),
            IsActive = dto.IsActive
        };

        await schedulingRepository.AddLocationAsync(location, cancellationToken);
        return ToDto(location);
    }

    public async Task<LocationDto?> UpdateLocationAsync(Guid id, UpsertLocationDto dto, CancellationToken cancellationToken)
    {
        var location = await schedulingRepository.GetLocationByIdAsync(id, cancellationToken);

        if (location is null)
        {
            return null;
        }

        location.Name = NormalizeName(dto.Name, "Nazwa lokalizacji jest wymagana.");
        location.Description = NormalizeOptional(dto.Description);
        location.IsActive = dto.IsActive;
        location.UpdatedAt = DateTimeOffset.UtcNow;
        await schedulingRepository.UpdateLocationAsync(location, cancellationToken);
        return ToDto(location);
    }

    public Task<bool> DeleteLocationAsync(Guid id, CancellationToken cancellationToken) =>
        schedulingRepository.DeleteLocationAsync(id, cancellationToken);

    public async Task<IReadOnlyList<HolidayDto>> GetHolidaysAsync(CancellationToken cancellationToken)
    {
        var holidays = await schedulingRepository.ListHolidaysAsync(cancellationToken);
        return holidays.OrderBy(holiday => holiday.Date).Select(ToDto).ToList();
    }

    public async Task<HolidayDto> CreateHolidayAsync(UpsertHolidayDto dto, CancellationToken cancellationToken)
    {
        var holiday = new Holiday
        {
            Date = dto.Date,
            Name = NormalizeName(dto.Name, "Nazwa dnia wolnego jest wymagana.")
        };

        await schedulingRepository.AddHolidayAsync(holiday, cancellationToken);
        return ToDto(holiday);
    }

    public async Task<HolidayDto?> UpdateHolidayAsync(Guid id, UpsertHolidayDto dto, CancellationToken cancellationToken)
    {
        var holiday = await schedulingRepository.GetHolidayByIdAsync(id, cancellationToken);

        if (holiday is null)
        {
            return null;
        }

        holiday.Date = dto.Date;
        holiday.Name = NormalizeName(dto.Name, "Nazwa dnia wolnego jest wymagana.");
        await schedulingRepository.UpdateHolidayAsync(holiday, cancellationToken);
        return ToDto(holiday);
    }

    public Task<bool> DeleteHolidayAsync(Guid id, CancellationToken cancellationToken) =>
        schedulingRepository.DeleteHolidayAsync(id, cancellationToken);

    public async Task<CalendarDto> GetCalendarAsync(
        DateTimeOffset? from,
        DateTimeOffset? to,
        Guid? instructorId,
        CancellationToken cancellationToken)
    {
        // Instruktor widzi swoje grupy ORAZ pojedyncze terminy, na których jest zastępstwem -
        // inaczej kalendarz pokazywałby co innego niż grafik (`/api/schedule`), który zastępstwa
        // uwzględnia. Filtr po zastępstwie stosujemy dalej, przy składaniu listy terminów.
        var allGroups = await groupRepository.ListAsync(cancellationToken);
        var groups = instructorId is Guid userId
            ? allGroups
                .Where(group => group.InstructorId == userId
                    || group.Sessions.Any(session => session.SubstituteInstructorId == userId))
                .ToList()
            : allGroups;
        var lessons = await lessonRepository.ListTitlesAsync(cancellationToken);
        var users = await userRepository.ListAsync(cancellationToken);
        var instructorNames = users.ToDictionary(user => user.Id, user => user.DisplayName);
        var locations = await schedulingRepository.ListLocationsAsync(cancellationToken);
        var locationNames = locations.ToDictionary(location => location.Id, location => location.Name);
        var holidays = await schedulingRepository.ListHolidaysAsync(cancellationToken);

        var sessions = groups
            .SelectMany(group => group.Sessions.Select(session => (group, session)))
            // Z cudzej grupy pokazujemy wyłącznie te terminy, na których jest się zastępstwem.
            .Where(item => instructorId is not Guid viewerId
                || item.group.InstructorId == viewerId
                || item.session.SubstituteInstructorId == viewerId)
            .Where(item => from is null || item.session.ScheduledAt >= from.Value)
            .Where(item => to is null || item.session.ScheduledAt <= to.Value)
            .OrderBy(item => item.session.ScheduledAt)
            .ThenBy(item => item.group.Name)
            .Select(item =>
            {
                var locationId = item.session.LocationId ?? item.group.LocationId;
                return new CalendarSessionDto(
                    item.session.Id,
                    item.group.Id,
                    item.group.Name,
                    item.group.InstructorId,
                    instructorNames.GetValueOrDefault(item.group.InstructorId, "(nieznany)"),
                    item.session.LessonId,
                    item.session.LessonId is Guid lessonId ? lessons.GetValueOrDefault(lessonId) : null,
                    item.session.ScheduledAt,
                    item.session.SequenceNumber,
                    GroupMapping.StatusName(item.session.Status),
                    GroupMapping.StatusLabel(item.session.Status),
                    locationId,
                    locationId is Guid id ? locationNames.GetValueOrDefault(id) : null);
            })
            .ToList();

        return new CalendarDto(
            sessions,
            holidays.OrderBy(holiday => holiday.Date).Select(ToDto).ToList(),
            locations.OrderBy(location => location.Name).Select(ToDto).ToList());
    }

    private static LocationDto ToDto(Location location) =>
        new(location.Id, location.Name, location.Description, location.IsActive);

    private static HolidayDto ToDto(Holiday holiday) =>
        new(holiday.Id, holiday.Date, holiday.Name);

    private static string NormalizeName(string? value, string error)
    {
        var normalized = (value ?? string.Empty).Trim();
        return normalized.Length == 0 ? throw new ArgumentException(error) : normalized;
    }

    private static string? NormalizeOptional(string? value)
    {
        var normalized = (value ?? string.Empty).Trim();
        return normalized.Length == 0 ? null : normalized;
    }
}

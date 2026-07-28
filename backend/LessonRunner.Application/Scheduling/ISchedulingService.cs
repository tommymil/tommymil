namespace LessonRunner.Application.Scheduling;

public interface ISchedulingService
{
    Task<IReadOnlyList<LocationDto>> GetLocationsAsync(CancellationToken cancellationToken);
    Task<LocationDto> CreateLocationAsync(UpsertLocationDto dto, CancellationToken cancellationToken);
    Task<LocationDto?> UpdateLocationAsync(Guid id, UpsertLocationDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteLocationAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<HolidayDto>> GetHolidaysAsync(CancellationToken cancellationToken);
    Task<HolidayDto> CreateHolidayAsync(UpsertHolidayDto dto, CancellationToken cancellationToken);
    Task<HolidayDto?> UpdateHolidayAsync(Guid id, UpsertHolidayDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteHolidayAsync(Guid id, CancellationToken cancellationToken);

    Task<CalendarDto> GetCalendarAsync(
        DateTimeOffset? from,
        DateTimeOffset? to,
        Guid? instructorId,
        CancellationToken cancellationToken);
}

using LessonRunner.Domain.Scheduling;

namespace LessonRunner.Application.Scheduling;

public interface ISchedulingRepository
{
    Task<IReadOnlyList<Location>> ListLocationsAsync(CancellationToken cancellationToken);
    Task<Location?> GetLocationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddLocationAsync(Location location, CancellationToken cancellationToken);
    Task<bool> UpdateLocationAsync(Location location, CancellationToken cancellationToken);
    Task<bool> DeleteLocationAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Holiday>> ListHolidaysAsync(CancellationToken cancellationToken);
    Task<Holiday?> GetHolidayByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddHolidayAsync(Holiday holiday, CancellationToken cancellationToken);
    Task<bool> UpdateHolidayAsync(Holiday holiday, CancellationToken cancellationToken);
    Task<bool> DeleteHolidayAsync(Guid id, CancellationToken cancellationToken);
}

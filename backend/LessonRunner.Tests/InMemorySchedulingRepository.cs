using LessonRunner.Application.Scheduling;
using LessonRunner.Domain.Scheduling;

namespace LessonRunner.Tests;

internal sealed class InMemorySchedulingRepository : ISchedulingRepository
{
    private readonly List<Location> _locations = [];
    private readonly List<Holiday> _holidays = [];

    public Task<IReadOnlyList<Location>> ListLocationsAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Location> result = _locations.ToList();
        return Task.FromResult(result);
    }

    public Task<Location?> GetLocationByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_locations.FirstOrDefault(location => location.Id == id));

    public Task AddLocationAsync(Location location, CancellationToken cancellationToken)
    {
        _locations.Add(location);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateLocationAsync(Location location, CancellationToken cancellationToken)
    {
        var stored = _locations.FirstOrDefault(item => item.Id == location.Id);

        if (stored is null)
        {
            return Task.FromResult(false);
        }

        stored.Name = location.Name;
        stored.Description = location.Description;
        stored.IsActive = location.IsActive;
        stored.UpdatedAt = location.UpdatedAt;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteLocationAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_locations.RemoveAll(location => location.Id == id) > 0);

    public Task<IReadOnlyList<Holiday>> ListHolidaysAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Holiday> result = _holidays.ToList();
        return Task.FromResult(result);
    }

    public Task<Holiday?> GetHolidayByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_holidays.FirstOrDefault(holiday => holiday.Id == id));

    public Task AddHolidayAsync(Holiday holiday, CancellationToken cancellationToken)
    {
        _holidays.Add(holiday);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateHolidayAsync(Holiday holiday, CancellationToken cancellationToken)
    {
        var stored = _holidays.FirstOrDefault(item => item.Id == holiday.Id);

        if (stored is null)
        {
            return Task.FromResult(false);
        }

        stored.Date = holiday.Date;
        stored.Name = holiday.Name;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteHolidayAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_holidays.RemoveAll(holiday => holiday.Id == id) > 0);
}

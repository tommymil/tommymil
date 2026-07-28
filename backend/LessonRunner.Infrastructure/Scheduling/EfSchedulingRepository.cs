using LessonRunner.Application.Scheduling;
using LessonRunner.Domain.Scheduling;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Scheduling;

internal sealed class EfSchedulingRepository(AppDbContext dbContext) : ISchedulingRepository
{
    public async Task<IReadOnlyList<Location>> ListLocationsAsync(CancellationToken cancellationToken)
    {
        var documents = await dbContext.Locations
            .AsNoTracking()
            .OrderBy(location => location.Name)
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<Location?> GetLocationByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Locations.AsNoTracking().FirstOrDefaultAsync(location => location.Id == id, cancellationToken);
        return document is null ? null : ToDomain(document);
    }

    public async Task AddLocationAsync(Location location, CancellationToken cancellationToken)
    {
        dbContext.Locations.Add(ToDocument(location));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateLocationAsync(Location location, CancellationToken cancellationToken)
    {
        var document = await dbContext.Locations.FirstOrDefaultAsync(item => item.Id == location.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Name = location.Name;
        document.Description = location.Description;
        document.IsActive = location.IsActive;
        document.UpdatedAt = location.UpdatedAt;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Locations.FirstOrDefaultAsync(location => location.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.Locations.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Holiday>> ListHolidaysAsync(CancellationToken cancellationToken)
    {
        var documents = await dbContext.Holidays
            .AsNoTracking()
            .OrderBy(holiday => holiday.Date)
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<Holiday?> GetHolidayByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Holidays.AsNoTracking().FirstOrDefaultAsync(holiday => holiday.Id == id, cancellationToken);
        return document is null ? null : ToDomain(document);
    }

    public async Task AddHolidayAsync(Holiday holiday, CancellationToken cancellationToken)
    {
        dbContext.Holidays.Add(ToDocument(holiday));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateHolidayAsync(Holiday holiday, CancellationToken cancellationToken)
    {
        var document = await dbContext.Holidays.FirstOrDefaultAsync(item => item.Id == holiday.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Date = holiday.Date;
        document.Name = holiday.Name;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteHolidayAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Holidays.FirstOrDefaultAsync(holiday => holiday.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.Holidays.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Location ToDomain(LocationDocument document) => new()
    {
        Id = document.Id,
        Name = document.Name,
        Description = document.Description,
        IsActive = document.IsActive,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt
    };

    private static Holiday ToDomain(HolidayDocument document) => new()
    {
        Id = document.Id,
        Date = document.Date,
        Name = document.Name,
        CreatedAt = document.CreatedAt
    };

    private static LocationDocument ToDocument(Location location) => new()
    {
        Id = location.Id,
        Name = location.Name,
        Description = location.Description,
        IsActive = location.IsActive,
        CreatedAt = location.CreatedAt,
        UpdatedAt = location.UpdatedAt
    };

    private static HolidayDocument ToDocument(Holiday holiday) => new()
    {
        Id = holiday.Id,
        Date = holiday.Date,
        Name = holiday.Name,
        CreatedAt = holiday.CreatedAt
    };
}

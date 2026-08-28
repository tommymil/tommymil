using LessonRunner.Application.Materials;
using LessonRunner.Domain.Materials;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Materials;

internal sealed class EfMaterialRepository(AppDbContext dbContext) : IMaterialRepository
{
    public async Task<IReadOnlyList<Material>> ListAsync(CancellationToken cancellationToken)
    {
        // SQLite nie tłumaczy `DateTimeOffset` w ORDER BY. Biblioteka jest mała, więc zgodnie
        // z pozostałymi repozytoriami materializujemy wynik i sortujemy po stronie aplikacji.
        var documents = await dbContext.Materials
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(material => material.UpdatedAt)
            .Select(ToDomain)
            .ToList();
    }

    public async Task<Material?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Materials
            .AsNoTracking()
            .FirstOrDefaultAsync(material => material.Id == id, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task AddAsync(Material material, CancellationToken cancellationToken)
    {
        dbContext.Materials.Add(ToDocument(material));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(Material material, CancellationToken cancellationToken)
    {
        var document = await dbContext.Materials
            .FirstOrDefaultAsync(item => item.Id == material.Id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.Title = material.Title;
        document.Description = material.Description;
        document.ResourceUrl = material.ResourceUrl;
        document.FileName = material.FileName;
        document.ContentType = material.ContentType;
        document.SizeBytes = material.SizeBytes;
        document.Visibility = ToStoredVisibility(material.Visibility);
        document.UpdatedAt = material.UpdatedAt;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Materials.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.Materials.Remove(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Material ToDomain(MaterialDocument document) => new()
    {
        Id = document.Id,
        Title = document.Title,
        Description = document.Description,
        ResourceUrl = document.ResourceUrl,
        FileName = document.FileName,
        ContentType = document.ContentType,
        SizeBytes = document.SizeBytes,
        Visibility = string.Equals(document.Visibility, "admin", StringComparison.OrdinalIgnoreCase)
            ? MaterialVisibility.Admin
            : MaterialVisibility.Staff,
        CreatedAt = document.CreatedAt,
        UpdatedAt = document.UpdatedAt
    };

    private static MaterialDocument ToDocument(Material material) => new()
    {
        Id = material.Id,
        Title = material.Title,
        Description = material.Description,
        ResourceUrl = material.ResourceUrl,
        FileName = material.FileName,
        ContentType = material.ContentType,
        SizeBytes = material.SizeBytes,
        Visibility = ToStoredVisibility(material.Visibility),
        CreatedAt = material.CreatedAt,
        UpdatedAt = material.UpdatedAt
    };

    private static string ToStoredVisibility(MaterialVisibility visibility) =>
        visibility == MaterialVisibility.Admin ? "admin" : "staff";
}

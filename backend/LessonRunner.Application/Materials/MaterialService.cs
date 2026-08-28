using LessonRunner.Application.Common;
using LessonRunner.Domain.Materials;

namespace LessonRunner.Application.Materials;

public sealed class MaterialService(IMaterialRepository repository) : IMaterialService
{
    public async Task<IReadOnlyList<MaterialDto>> ListAsync(
        bool includeAdminOnly,
        CancellationToken cancellationToken)
    {
        var materials = await repository.ListAsync(cancellationToken);

        return materials
            .Where(material => includeAdminOnly || material.Visibility == MaterialVisibility.Staff)
            .OrderByDescending(material => material.UpdatedAt)
            .Select(ToDto)
            .ToList();
    }

    public async Task<MaterialDto> CreateAsync(UpsertMaterialDto dto, CancellationToken cancellationToken)
    {
        var values = Validate(dto);
        var material = new Material
        {
            Title = values.Title,
            Description = values.Description,
            ResourceUrl = values.ResourceUrl,
            FileName = values.FileName,
            ContentType = values.ContentType,
            SizeBytes = values.SizeBytes,
            Visibility = values.Visibility
        };

        await repository.AddAsync(material, cancellationToken);
        return ToDto(material);
    }

    public async Task<MaterialDto?> UpdateAsync(
        Guid id,
        UpsertMaterialDto dto,
        CancellationToken cancellationToken)
    {
        var material = await repository.GetByIdAsync(id, cancellationToken);

        if (material is null)
        {
            return null;
        }

        var values = Validate(dto);
        material.Title = values.Title;
        material.Description = values.Description;
        material.ResourceUrl = values.ResourceUrl;
        material.FileName = values.FileName;
        material.ContentType = values.ContentType;
        material.SizeBytes = values.SizeBytes;
        material.Visibility = values.Visibility;
        material.UpdatedAt = DateTimeOffset.UtcNow;

        await repository.UpdateAsync(material, cancellationToken);
        return ToDto(material);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        repository.DeleteAsync(id, cancellationToken);

    private static ValidatedMaterial Validate(UpsertMaterialDto dto)
    {
        var title = dto.Title?.Trim() ?? string.Empty;
        var description = dto.Description?.Trim() ?? string.Empty;
        var resourceUrl = dto.ResourceUrl?.Trim() ?? string.Empty;
        var fileName = NullIfWhiteSpace(dto.FileName);
        var contentType = NullIfWhiteSpace(dto.ContentType);

        if (title.Length == 0)
        {
            throw new ArgumentException("Tytuł materiału jest wymagany.");
        }

        if (title.Length > 200)
        {
            throw new ArgumentException("Tytuł materiału może mieć maksymalnie 200 znaków.");
        }

        if (description.Length > 2000)
        {
            throw new ArgumentException("Opis materiału może mieć maksymalnie 2000 znaków.");
        }

        if (!IsAllowedResourceUrl(resourceUrl))
        {
            throw new ArgumentException("Podaj prawidłowy adres HTTPS/HTTP albo prześlij plik.");
        }

        if (fileName is { Length: > 260 })
        {
            throw new ArgumentException("Nazwa pliku może mieć maksymalnie 260 znaków.");
        }

        if (contentType is { Length: > 160 })
        {
            throw new ArgumentException("Typ pliku może mieć maksymalnie 160 znaków.");
        }

        if (dto.SizeBytes is < 0)
        {
            throw new ArgumentException("Rozmiar pliku nie może być ujemny.");
        }

        var visibility = dto.Visibility?.Trim().ToLowerInvariant() switch
        {
            "admin" => MaterialVisibility.Admin,
            "staff" => MaterialVisibility.Staff,
            _ => throw new ArgumentException("Widoczność musi mieć wartość 'admin' albo 'staff'.")
        };

        return new ValidatedMaterial(
            title,
            description,
            resourceUrl,
            fileName,
            contentType,
            dto.SizeBytes,
            visibility);
    }

    /// <summary>Plik wgrany do systemu (ścieżka `/uploads/...`) albo zewnętrzny adres http(s).</summary>
    private static bool IsAllowedResourceUrl(string value) =>
        value.StartsWith("/uploads/", StringComparison.Ordinal) || WebLink.IsHttpUrl(value);

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static MaterialDto ToDto(Material material) => new(
        material.Id,
        material.Title,
        material.Description,
        material.ResourceUrl,
        material.FileName,
        material.ContentType,
        material.SizeBytes,
        material.Visibility == MaterialVisibility.Admin ? "admin" : "staff",
        material.CreatedAt,
        material.UpdatedAt);

    private sealed record ValidatedMaterial(
        string Title,
        string Description,
        string ResourceUrl,
        string? FileName,
        string? ContentType,
        long? SizeBytes,
        MaterialVisibility Visibility);
}

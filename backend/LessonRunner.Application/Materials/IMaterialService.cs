namespace LessonRunner.Application.Materials;

public interface IMaterialService
{
    Task<IReadOnlyList<MaterialDto>> ListAsync(bool includeAdminOnly, CancellationToken cancellationToken);
    Task<MaterialDto> CreateAsync(UpsertMaterialDto dto, CancellationToken cancellationToken);
    Task<MaterialDto?> UpdateAsync(Guid id, UpsertMaterialDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

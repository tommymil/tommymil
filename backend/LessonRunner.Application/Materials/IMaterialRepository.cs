using LessonRunner.Domain.Materials;

namespace LessonRunner.Application.Materials;

public interface IMaterialRepository
{
    Task<IReadOnlyList<Material>> ListAsync(CancellationToken cancellationToken);
    Task<Material?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Material material, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Material material, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

namespace LessonRunner.Application.Auth;

/// <summary>Administracja kontami (tylko dla admina): lista, tworzenie, aktywacja/dezaktywacja.</summary>
public interface IUserAdminService
{
    Task<IReadOnlyList<UserListItemDto>> ListAsync(CancellationToken cancellationToken);
    Task<UserListItemDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateProfileAsync(Guid id, UpdateUserProfileDto dto, CancellationToken cancellationToken);
    /// <summary>Aktywacja/dezaktywacja konta. <paramref name="actingUserId"/> to admin wykonujący
    /// operację - nie może wyłączyć samego siebie ani ostatniego aktywnego administratora.</summary>
    Task<bool> SetActiveAsync(Guid id, bool isActive, Guid? actingUserId, CancellationToken cancellationToken);
    Task<bool> SetPasswordAsync(Guid id, string password, CancellationToken cancellationToken);
}

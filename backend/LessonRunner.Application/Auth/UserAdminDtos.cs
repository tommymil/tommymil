namespace LessonRunner.Application.Auth;

public sealed record UserListItemDto(
    Guid Id,
    string Email,
    string Role,
    bool IsActive,
    string? FirstName,
    string? LastName,
    string? Phone,
    string DisplayName);

public sealed record CreateUserDto(
    string Email,
    string Password,
    string Role,
    string? FirstName = null,
    string? LastName = null,
    string? Phone = null);

public sealed record UpdateUserProfileDto(string? FirstName, string? LastName, string? Phone);

public sealed record SetPasswordDto(string Password);

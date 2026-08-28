using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;

namespace LessonRunner.Tests;

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.FirstOrDefault(user => user.Email == normalizedEmail));
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.FirstOrDefault(user => user.Id == id));
    }

    public Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<User> result = _users.ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<User>> ListByRoleAsync(UserRole role, CancellationToken cancellationToken)
    {
        IReadOnlyList<User> result = _users.Where(user => user.Role == role && user.IsActive).ToList();
        return Task.FromResult(result);
    }

    public Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var user = _users.FirstOrDefault(item => item.Id == id);

        if (user is null)
        {
            return Task.FromResult(false);
        }

        user.IsActive = isActive;
        return Task.FromResult(true);
    }

    public Task<bool> SetProfileAsync(Guid id, string? firstName, string? lastName, string? phone, CancellationToken cancellationToken)
    {
        var user = _users.FirstOrDefault(item => item.Id == id);

        if (user is null)
        {
            return Task.FromResult(false);
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Phone = phone;
        return Task.FromResult(true);
    }

    public Task<bool> SetPasswordHashAsync(Guid id, string passwordHash, CancellationToken cancellationToken)
    {
        var user = _users.FirstOrDefault(item => item.Id == id);

        if (user is null)
        {
            return Task.FromResult(false);
        }

        user.PasswordHash = passwordHash;
        return Task.FromResult(true);
    }

    public Task<bool> SetRoleAsync(Guid id, UserRole role, CancellationToken cancellationToken)
    {
        var user = _users.FirstOrDefault(item => item.Id == id);

        if (user is null)
        {
            return Task.FromResult(false);
        }

        user.Role = role;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        return Task.FromResult(true);
    }

    public Task<bool> ExistsAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.Any(user => user.Email == normalizedEmail));
    }

    public Task<bool> HasAnyAdminAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.Any(user => user.Role == UserRole.Admin && user.IsActive));
    }

    public Task<int> CountActiveAdminsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.Count(user => user.Role == UserRole.Admin && user.IsActive));
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }

}

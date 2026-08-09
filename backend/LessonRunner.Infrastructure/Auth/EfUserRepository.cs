using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Auth;

internal sealed class EfUserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var document = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        return document is null ? null : ToDomain(document);
    }

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken)
    {
        var documents = await dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Email)
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<User>> ListByRoleAsync(UserRole role, CancellationToken cancellationToken)
    {
        var roleName = role.ToString();
        var documents = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Role == roleName && user.IsActive)
            .OrderBy(user => user.Email)
            .ToListAsync(cancellationToken);

        return documents.Select(ToDomain).ToList();
    }

    public async Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var document = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.IsActive = isActive;

        // Dezaktywacja ma działać od razu, a nie dopiero po wygaśnięciu tokenu.
        if (!isActive)
        {
            document.SecurityStamp = NewSecurityStamp();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetProfileAsync(Guid id, string? firstName, string? lastName, string? phone, CancellationToken cancellationToken)
    {
        var document = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.FirstName = firstName;
        document.LastName = lastName;
        document.Phone = phone;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetPasswordHashAsync(Guid id, string passwordHash, CancellationToken cancellationToken)
    {
        var document = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        if (document is null)
        {
            return false;
        }

        document.PasswordHash = passwordHash;
        // Zmiana lub reset hasła wylogowuje wszystkie pozostałe sesje tego konta.
        document.SecurityStamp = NewSecurityStamp();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string NewSecurityStamp() => Guid.NewGuid().ToString("N");

    public Task<bool> ExistsAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return dbContext.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    /// <summary>Czy istnieje AKTYWNE konto administratora. Nieaktywni admini nie mogą się zalogować,
    /// więc gdyby liczyli się tutaj, dezaktywacja ostatniego admina blokowałaby system na stałe
    /// (bootstrap nie odtworzyłby konta).</summary>
    public async Task<bool> HasAnyAdminAsync(CancellationToken cancellationToken)
    {
        return await CountActiveAdminsAsync(cancellationToken) > 0;
    }

    public Task<int> CountActiveAdminsAsync(CancellationToken cancellationToken)
    {
        var adminRole = UserRole.Admin.ToString();
        return dbContext.Users.CountAsync(user => user.Role == adminRole && user.IsActive, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        dbContext.Users.Add(ToDocument(user));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Dokłada brakujące konta startowe.
    ///
    /// Warunek jest per e-mail, a nie „czy w bazie jest jakikolwiek użytkownik". Przy tym
    /// drugim dopisanie nowego konta do seeda nie docierało do żadnego istniejącego
    /// środowiska deweloperskiego - baza miała już admina, więc seed kończył się na pierwszej
    /// linijce. Istniejących kont nie ruszamy: zmienione hasło ma zostać zmienione.
    /// </summary>
    public async Task SeedAsync(IReadOnlyList<User> users, CancellationToken cancellationToken)
    {
        var existingEmails = await dbContext.Users
            .Select(user => user.Email)
            .ToListAsync(cancellationToken);
        var known = existingEmails.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var added = false;

        foreach (var user in users.Where(user => !known.Contains(user.Email)))
        {
            dbContext.Users.Add(ToDocument(user));
            added = true;
        }

        if (added)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static UserDocument ToDocument(User user)
    {
        return new UserDocument
        {
            Id = user.Id,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            SecurityStamp = user.SecurityStamp,
            CreatedAt = user.CreatedAt
        };
    }

    private static User ToDomain(UserDocument document)
    {
        return new User
        {
            Id = document.Id,
            Email = document.Email,
            PasswordHash = document.PasswordHash,
            Role = Enum.TryParse<UserRole>(document.Role, ignoreCase: true, out var role) ? role : UserRole.Instructor,
            IsActive = document.IsActive,
            FirstName = document.FirstName,
            LastName = document.LastName,
            Phone = document.Phone,
            SecurityStamp = document.SecurityStamp,
            CreatedAt = document.CreatedAt
        };
    }
}

using LessonRunner.Application.Auth;
using LessonRunner.Domain.Users;

namespace LessonRunner.Infrastructure.Auth;

/// <summary>
/// Domyślne konta startowe. Hasła służą wyłącznie do lokalnego developmentu.
/// Przed wdrożeniem produkcyjnym należy je zmienić lub wyłączyć seed.
/// </summary>
internal static class UserSeedData
{
    public static IReadOnlyList<User> Create(IPasswordHasher passwordHasher)
    {
        return
        [
            new User
            {
                Email = "admin@lessonrunner.local",
                PasswordHash = passwordHasher.Hash("admin12345"),
                Role = UserRole.Admin,
                FirstName = "Adam",
                LastName = "Administrator"
            },
            new User
            {
                Email = "instructor@lessonrunner.local",
                PasswordHash = passwordHasher.Hash("teacher12345"),
                Role = UserRole.Instructor,
                FirstName = "Tomasz",
                LastName = "Trener",
                Phone = "600123456"
            }
        ];
    }
}

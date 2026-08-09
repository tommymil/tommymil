namespace LessonRunner.Domain.Users;

/// <summary>
/// Po co wydano token jednorazowy. Rozróżnienie jest istotne, bo oba przepływy kończą się tym
/// samym (ustawieniem hasła), ale mają inny czas ważności i inną treść wiadomości.
/// </summary>
public enum AccountTokenPurpose
{
    /// <summary>Użytkownik sam poprosił o reset — ważny krótko, bo to akcja w toku.</summary>
    PasswordReset = 0,

    /// <summary>Admin zaprosił opiekuna do systemu — ważny długo, bo rodzic zajrzy do skrzynki,
    /// kiedy zajrzy.</summary>
    Invitation = 1
}

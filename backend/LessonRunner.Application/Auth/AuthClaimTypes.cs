namespace LessonRunner.Application.Auth;

/// <summary>Własne nazwy claimów używane w tokenach aplikacji.</summary>
public static class AuthClaimTypes
{
    /// <summary>Znacznik ważności sesji (security stamp). Porównywany z wartością w bazie przy
    /// każdym żądaniu - dzięki temu dezaktywacja konta i zmiana hasła unieważniają token
    /// natychmiast, a nie dopiero po jego wygaśnięciu.</summary>
    public const string SecurityStamp = "sst";
}

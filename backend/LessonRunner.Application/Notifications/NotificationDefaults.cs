using LessonRunner.Domain.Notifications;

namespace LessonRunner.Application.Notifications;

/// <summary>
/// Nadawca wiadomości, zanim ktokolwiek wejdzie w panel powiadomień.
///
/// Ustawienia powiadomień siedzą w bazie i powstają dopiero przy pierwszym zapisie z panelu.
/// Do tego czasu obowiązywał adres wpisany w kodzie — `noreply@lessonrunner.local` — z domeny,
/// która nie istnieje w DNS. Świeżo wdrożona instalacja z `SMTP_MODE=Smtp` wysyłała więc
/// wiadomości, których żaden dostawca nie przyjmie, a jedynym śladem był wpis „failed”
/// w dzienniku wysyłek.
///
/// Dlatego adres nadawcy jest częścią konfiguracji wdrożenia (`Notifications:FromEmail`),
/// tak samo jak klucz podpisu czy dane SMTP — a nie wartością, którą trzeba pamiętać,
/// żeby ręcznie nadpisać po pierwszym uruchomieniu.
/// </summary>
public sealed class NotificationDefaults
{
    public string FromName { get; init; } = new NotificationSettings().FromName;

    public string FromEmail { get; init; } = NotificationSettings.PlaceholderFromEmail;

    /// <summary>Ustawienia dla instalacji, w której nikt jeszcze nic nie zapisał w panelu.</summary>
    public NotificationSettings CreateSettings() => new()
    {
        FromName = FromName,
        FromEmail = FromEmail
    };
}

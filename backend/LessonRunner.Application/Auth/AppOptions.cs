namespace LessonRunner.Application.Auth;

public sealed class AppOptions
{
    public const string SectionName = "App";

    /// <summary>Publiczny adres aplikacji, od którego budujemy linki w e-mailach (reset hasła,
    /// zaproszenia). Backend nie zna adresu, pod którym stoi frontend — za reverse proxy widzi
    /// tylko własny port — więc zgadywanie z żądania dałoby linki prowadzące donikąd albo,
    /// gorzej, adres podstawiony przez atakującego w nagłówku Host.</summary>
    public string PublicOrigin { get; set; } = "http://localhost:8080";
}

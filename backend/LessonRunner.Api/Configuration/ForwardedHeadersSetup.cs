using Microsoft.AspNetCore.HttpOverrides;

namespace LessonRunner.Api.Configuration;

/// <summary>
/// Buduje konfigurację nagłówków X-Forwarded-* dla wdrożeń za reverse proxy.
/// Wydzielone z <c>Program.cs</c>, żeby dało się to sprawdzić testem - skutek złej listy
/// zaufanych sieci widać dopiero pod obciążeniem realnego proxy, czyli za późno.
/// </summary>
public static class ForwardedHeadersSetup
{
    /// <summary>
    /// Pętla zwrotna i zakresy prywatne (RFC 1918). Reverse proxy zawsze stoi pod takim adresem,
    /// a kontener API nie jest wystawiany na zewnątrz (patrz docker-compose.yml). W szczególności
    /// nginx w compose dostaje adres z sieci bridge Dockera, mieszczącej się w 172.16.0.0/12.
    /// </summary>
    public static readonly string[] DefaultTrustedProxyNetworks =
        ["127.0.0.0/8", "::1/128", "10.0.0.0/8", "172.16.0.0/12", "192.168.0.0/16"];

    public const string ConfigurationKey = "Security:TrustedProxyNetworks";

    /// <summary>
    /// Lista zaufanych sieci musi być podana wprost i nie może być pusta. Domyślnie ASP.NET Core
    /// ufa wyłącznie pętli zwrotnej, więc nagłówek od nginxa z sieci bridge byłby po cichu
    /// odrzucany: limiter partycjonowałby po adresie proxy i „10 prób na IP” działałoby jak jeden
    /// limit na całą instalację. Zaufanie wszystkim jest drugą skrajnością - wtedy dowolny klient
    /// obchodzi ten limit, podstawiając cudzy adres w nagłówku.
    /// </summary>
    public static ForwardedHeadersOptions Create(IConfiguration configuration)
    {
        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            // Między aplikacją a klientem stoi dokładnie jedno nasze proxy.
            ForwardLimit = 1
        };

        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();

        var configured = configuration.GetSection(ConfigurationKey).Get<string[]>();
        var networks = configured is { Length: > 0 } ? configured : DefaultTrustedProxyNetworks;

        foreach (var candidate in networks)
        {
            if (!System.Net.IPNetwork.TryParse(candidate, out var network))
            {
                throw new InvalidOperationException(
                    $"{ConfigurationKey} zawiera nieprawidłowy zakres CIDR: '{candidate}'.");
            }

            options.KnownIPNetworks.Add(network);
        }

        return options;
    }
}

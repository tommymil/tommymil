using System.Net;
using LessonRunner.Api.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Zaufanie do nagłówka X-Forwarded-For. Adres klienta decyduje o tym, kogo limituje
/// polityka „auth” (10 nieudanych logowań na 5 minut), więc pomyłka w obie strony jest
/// kosztowna: przy zbyt wąskiej liście nagłówek jest odrzucany i cała szkoła dzieli jeden
/// limit, przy zbyt szerokiej limit obchodzi się samym nagłówkiem.
///
/// Testujemy middleware wprost, a nie przez ApiFactory: TestServer nie ustawia
/// Connection.RemoteIpAddress, a bez kontroli nad adresem połączenia nie da się odróżnić
/// proxy zaufanego od niezaufanego.
/// </summary>
public sealed class ForwardedHeadersTests
{
    private const string DockerBridgeProxy = "172.18.0.5";
    private const string PublicAddress = "198.51.100.7";
    private const string ClientAddress = "203.0.113.10";

    [Fact]
    public void DefaultNetworks_AreAllParsable()
    {
        // Regresja na literówkę w samych stałych: System.Net.IPNetwork odrzuca zapis
        // z ustawionymi bitami hosta (np. „127.0.0.1/8”), a Create rzuca wtedy wyjątkiem
        // przy starcie aplikacji - czyli aplikacja w ogóle nie wstaje na produkcji.
        foreach (var network in ForwardedHeadersSetup.DefaultTrustedProxyNetworks)
        {
            Assert.True(
                System.Net.IPNetwork.TryParse(network, out _),
                $"Domyślna sieć '{network}' nie jest prawidłowym zapisem CIDR.");
        }
    }

    [Fact]
    public void DefaultConfiguration_TrustsDockerBridgeButNotPublicAddresses()
    {
        var options = ForwardedHeadersSetup.Create(EmptyConfiguration());

        // Pusta lista to dokładnie ten błąd, który naprawiamy - domyślne zaufanie samej
        // pętli zwrotnej sprawiało, że nginx z sieci bridge był traktowany jak obcy.
        Assert.NotEmpty(options.KnownIPNetworks);
        Assert.Contains(options.KnownIPNetworks, n => n.Contains(IPAddress.Parse(DockerBridgeProxy)));
        Assert.DoesNotContain(options.KnownIPNetworks, n => n.Contains(IPAddress.Parse(PublicAddress)));
    }

    [Fact]
    public async Task TrustedProxy_ForwardedForBecomesClientAddress()
    {
        var context = await RunMiddlewareAsync(
            remoteIp: DockerBridgeProxy,
            forwardedFor: ClientAddress,
            forwardedProto: "https");

        Assert.Equal(ClientAddress, context.Connection.RemoteIpAddress?.ToString());
        Assert.Equal("https", context.Request.Scheme);
    }

    [Fact]
    public async Task UntrustedProxy_ForwardedForIsIgnored()
    {
        // Bez tego dowolny klient podstawiłby losowy adres w nagłówku i miałby świeży
        // limit logowania przy każdej próbie.
        var context = await RunMiddlewareAsync(
            remoteIp: PublicAddress,
            forwardedFor: ClientAddress,
            forwardedProto: "https");

        Assert.Equal(PublicAddress, context.Connection.RemoteIpAddress?.ToString());
        Assert.Equal("http", context.Request.Scheme);
    }

    [Fact]
    public async Task TrustedProxy_DistinctClientsKeepDistinctAddresses()
    {
        // Sedno sprawy dla limitera: dwie osoby za tym samym proxy nie mogą wyglądać
        // jak jeden adres, bo wtedy dziesięć pomyłek jednej blokuje logowanie pozostałym.
        var first = await RunMiddlewareAsync(DockerBridgeProxy, forwardedFor: "203.0.113.10");
        var second = await RunMiddlewareAsync(DockerBridgeProxy, forwardedFor: "203.0.113.99");

        Assert.NotEqual(
            first.Connection.RemoteIpAddress?.ToString(),
            second.Connection.RemoteIpAddress?.ToString());
    }

    [Fact]
    public void NarrowedConfiguration_ReplacesDefaults()
    {
        var options = ForwardedHeadersSetup.Create(ConfigurationWith("203.0.113.0/24"));

        Assert.Single(options.KnownIPNetworks);
        Assert.Contains(options.KnownIPNetworks, n => n.Contains(IPAddress.Parse(ClientAddress)));
        Assert.DoesNotContain(options.KnownIPNetworks, n => n.Contains(IPAddress.Parse(DockerBridgeProxy)));
    }

    [Fact]
    public void InvalidCidr_FailsFastWithReadableMessage()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => ForwardedHeadersSetup.Create(ConfigurationWith("172.16.0.0/nie-liczba")));

        Assert.Contains(ForwardedHeadersSetup.ConfigurationKey, exception.Message);
        Assert.Contains("172.16.0.0/nie-liczba", exception.Message);
    }

    private static async Task<HttpContext> RunMiddlewareAsync(
        string remoteIp,
        string forwardedFor,
        string? forwardedProto = null)
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(remoteIp);
        context.Request.Scheme = "http";
        context.Request.Headers["X-Forwarded-For"] = forwardedFor;

        if (forwardedProto is not null)
        {
            context.Request.Headers["X-Forwarded-Proto"] = forwardedProto;
        }

        var middleware = new ForwardedHeadersMiddleware(
            _ => Task.CompletedTask,
            NullLoggerFactory.Instance,
            Options.Create(ForwardedHeadersSetup.Create(EmptyConfiguration())));

        await middleware.Invoke(context);

        return context;
    }

    private static IConfiguration EmptyConfiguration() =>
        new ConfigurationBuilder().Build();

    private static IConfiguration ConfigurationWith(params string[] networks)
    {
        var values = networks
            .Select((network, index) =>
                new KeyValuePair<string, string?>(
                    $"{ForwardedHeadersSetup.ConfigurationKey}:{index}",
                    network));

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }
}

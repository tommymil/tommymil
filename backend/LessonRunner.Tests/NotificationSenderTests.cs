using LessonRunner.Application.Notifications;
using LessonRunner.Domain.Notifications;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Strażnik adresu nadawcy.
///
/// Regresja, przed którą broni: świeża instalacja nie ma wiersza ustawień powiadomień, więc
/// obowiązywał adres wpisany w kodzie — z domeny `.local`, której nie ma w DNS. Po przełączeniu
/// na `SMTP_MODE=Smtp` wiadomości do rodziców nie miały prawa dojść, a jedynym śladem był
/// wpis „failed” w dzienniku wysyłek z komunikatem cudzego serwera.
/// </summary>
public sealed class NotificationSenderTests
{
    [Theory]
    [InlineData("noreply@lessonrunner.local")]
    [InlineData("zajecia@szkola.localhost")]
    [InlineData("kontakt@szkola.test")]
    [InlineData("kontakt@szkola.invalid")]
    [InlineData("kontakt@szkola.example")]
    [InlineData("kontakt@example.com")]
    [InlineData("bez-malpy")]
    [InlineData("")]
    [InlineData(null)]
    public void IsUnroutableSenderAddress_RejectsAddressesThatCannotReceiveMail(string? address)
    {
        Assert.True(NotificationSettings.IsUnroutableSenderAddress(address));
    }

    [Theory]
    [InlineData("zajecia@twojadomena.pl")]
    [InlineData("Kontakt@Szkola-Programowania.PL")]
    [InlineData("noreply@localhost.pl")]
    public void IsUnroutableSenderAddress_AcceptsRealDomains(string address)
    {
        Assert.False(NotificationSettings.IsUnroutableSenderAddress(address));
    }

    /// <summary>Adres zastępczy w kodzie musi zostać rozpoznany jako nienadający się do wysyłki.</summary>
    [Fact]
    public void PlaceholderFromEmail_IsRecognisedAsUnroutable()
    {
        Assert.True(NotificationSettings.IsUnroutableSenderAddress(NotificationSettings.PlaceholderFromEmail));
        Assert.Equal(NotificationSettings.PlaceholderFromEmail, new NotificationSettings().FromEmail);
    }

    [Fact]
    public void CreateSettings_UsesTheAddressFromDeploymentConfiguration()
    {
        var defaults = new NotificationDefaults
        {
            FromEmail = "zajecia@twojadomena.pl",
            FromName = "Kodziaki"
        };

        var settings = defaults.CreateSettings();

        Assert.Equal("zajecia@twojadomena.pl", settings.FromEmail);
        Assert.Equal("Kodziaki", settings.FromName);
        Assert.False(NotificationSettings.IsUnroutableSenderAddress(settings.FromEmail));
    }
}

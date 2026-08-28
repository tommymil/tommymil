using LessonRunner.Application.Common;
using Xunit;

namespace LessonRunner.Tests;

/// <summary>
/// Strażnik adresów wpisywanych przez personel, które trafiają do atrybutu `href`.
///
/// Regresja, przed którą broni: link do spotkania lekcji próbnej i link do projektu dziecka
/// nie były sprawdzane wcale. `javascript:...` w tych polach wykonywał się w sesji osoby,
/// która klika — przy projekcie w portalu jest to sesja rodzica.
/// </summary>
public sealed class WebLinkTests
{
    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("JavaScript:alert(1)")]
    [InlineData("data:text/html;base64,PHNjcmlwdD4=")]
    [InlineData("vbscript:msgbox(1)")]
    [InlineData("file:///etc/passwd")]
    [InlineData("//evil.example/x")]
    [InlineData("meet.example/abc")]
    public void Normalize_Rejects_EverythingThatIsNotHttp(string value)
    {
        Assert.Throws<ArgumentException>(() => WebLink.Normalize(value, "zły adres"));
        Assert.False(WebLink.IsHttpUrl(value));
    }

    [Theory]
    [InlineData("https://meet.example/abc")]
    [InlineData("http://meet.example/abc")]
    [InlineData("HTTPS://MEET.EXAMPLE/abc")]
    public void Normalize_Accepts_AbsoluteHttpAddresses(string value)
    {
        Assert.Equal(value, WebLink.Normalize(value, "zły adres"));
        Assert.True(WebLink.IsHttpUrl(value));
    }

    /// <summary>Brak linku jest dozwolony — to nie to samo co link w złym schemacie.</summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Normalize_TreatsEmptyAsNoLink(string? value)
    {
        Assert.Null(WebLink.Normalize(value, "zły adres"));
    }

    [Fact]
    public void Normalize_TrimsSurroundingWhitespace()
    {
        Assert.Equal("https://meet.example/abc", WebLink.Normalize("  https://meet.example/abc  ", "zły adres"));
    }
}

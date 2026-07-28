using LessonRunner.Infrastructure.Auth;
using Xunit;

namespace LessonRunner.Tests;

public sealed class Pbkdf2PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ProducesDifferentOutputsForSamePassword()
    {
        var first = _hasher.Hash("supersecret");
        var second = _hasher.Hash("supersecret");

        // Różne sole => różne hashe, mimo tego samego hasła.
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Verify_ReturnsTrue_ForCorrectPassword()
    {
        var hash = _hasher.Hash("supersecret");

        Assert.True(_hasher.Verify("supersecret", hash));
    }

    [Fact]
    public void Verify_ReturnsFalse_ForWrongPassword()
    {
        var hash = _hasher.Hash("supersecret");

        Assert.False(_hasher.Verify("notsecret", hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("garbage")]
    [InlineData("1.only-two-parts")]
    public void Verify_ReturnsFalse_ForMalformedHash(string malformed)
    {
        Assert.False(_hasher.Verify("supersecret", malformed));
    }
}

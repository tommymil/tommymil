using System.Text;
using LessonRunner.Application.Files;
using LessonRunner.Infrastructure.Files;
using Microsoft.Extensions.Options;
using Xunit;

namespace LessonRunner.Tests;

public sealed class LocalFileStorageTests : IDisposable
{
    private readonly string _root;
    private readonly LocalFileStorage _storage;

    public LocalFileStorageTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "lesson-runner-tests", Guid.NewGuid().ToString("N"));
        var options = Options.Create(new FileStorageOptions { RootPath = _root, RequestPath = "/uploads" });
        _storage = new LocalFileStorage(options);
    }

    [Fact]
    public async Task SaveAsync_WritesFile_AndReturnsPublicUrlWithCorrectExtension()
    {
        var bytes = Encoding.UTF8.GetBytes("fake-png-bytes");
        using var content = new MemoryStream(bytes);

        var stored = await _storage.SaveAsync(content, "diagram.png", "image/png", CancellationToken.None);

        Assert.StartsWith("/uploads/", stored.Url);
        Assert.EndsWith(".png", stored.Url);
        Assert.Equal("diagram.png", stored.FileName);
        Assert.Equal("image/png", stored.ContentType);
        Assert.Equal(bytes.Length, stored.SizeBytes);

        var physicalName = stored.Url["/uploads/".Length..];
        Assert.True(File.Exists(Path.Combine(_root, physicalName)));
    }

    [Fact]
    public async Task SaveAsync_UsesExtensionFromContentType_NotFromFileName()
    {
        using var content = new MemoryStream([1, 2, 3]);

        var stored = await _storage.SaveAsync(content, "evil.exe", "application/pdf", CancellationToken.None);

        Assert.EndsWith(".pdf", stored.Url);
    }

    [Fact]
    public async Task SaveAsync_PreservesAllowedPackageExtension()
    {
        using var content = new MemoryStream([1, 2, 3]);

        var stored = await _storage.SaveAsync(content, "start.sb3", "application/octet-stream", CancellationToken.None);

        Assert.EndsWith(".sb3", stored.Url);
    }

    [Fact]
    public async Task SaveAsync_GeneratesUniqueNames()
    {
        using var first = new MemoryStream([1]);
        using var second = new MemoryStream([2]);

        var a = await _storage.SaveAsync(first, "a.png", "image/png", CancellationToken.None);
        var b = await _storage.SaveAsync(second, "b.png", "image/png", CancellationToken.None);

        Assert.NotEqual(a.Url, b.Url);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }
}

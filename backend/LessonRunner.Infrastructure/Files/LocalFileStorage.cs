using LessonRunner.Application.Files;
using Microsoft.Extensions.Options;

namespace LessonRunner.Infrastructure.Files;

internal sealed class LocalFileStorage(IOptions<FileStorageOptions> options) : IFileStorage
{
    private readonly FileStorageOptions _options = options.Value;

    public async Task<StoredFile> SaveAsync(
        Stream content,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_options.RootPath);

        var extension = FileUploadRules.ExtensionFor(contentType, originalFileName);
        var storedName = $"{Guid.NewGuid():N}{extension}";
        var physicalPath = Path.Combine(_options.RootPath, storedName);

        await using (var target = File.Create(physicalPath))
        {
            await content.CopyToAsync(target, cancellationToken);
        }

        var sizeBytes = new FileInfo(physicalPath).Length;
        var url = $"{_options.RequestPath.TrimEnd('/')}/{storedName}";

        return new StoredFile(url, originalFileName, contentType, sizeBytes);
    }
}

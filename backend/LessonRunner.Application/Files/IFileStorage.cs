namespace LessonRunner.Application.Files;

public sealed record StoredFile(string Url, string FileName, string ContentType, long SizeBytes);

public interface IFileStorage
{
    Task<StoredFile> SaveAsync(Stream content, string originalFileName, string contentType, CancellationToken cancellationToken);
}

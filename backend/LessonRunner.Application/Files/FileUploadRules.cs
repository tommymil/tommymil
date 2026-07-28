namespace LessonRunner.Application.Files;

public static class FileUploadRules
{
    public const long MaxSizeBytes = 10 * 1024 * 1024; // 10 MB

    private static readonly IReadOnlyDictionary<string, string> AllowedContentTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/png"] = ".png",
            ["image/jpeg"] = ".jpg",
            ["image/gif"] = ".gif",
            ["image/webp"] = ".webp",
            ["application/pdf"] = ".pdf"
        };

    private static readonly HashSet<string> AllowedPackageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".sb2",
        ".sb3",
        ".zip",
        ".mcworld",
        ".mctemplate"
    };

    private static readonly HashSet<string> AllowedPackageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "",
        "application/octet-stream",
        "application/zip",
        "application/x-zip-compressed",
        "application/x.scratch.sb3"
    };

    public static bool IsAllowedContentType(string? contentType) =>
        contentType is not null && AllowedContentTypes.ContainsKey(contentType);

    public static bool IsAllowedUpload(string? contentType, string fileName) =>
        IsAllowedContentType(contentType)
        || (AllowedPackageExtensions.Contains(Path.GetExtension(fileName))
            && AllowedPackageContentTypes.Contains(contentType ?? string.Empty));

    public static string ExtensionFor(string contentType, string originalFileName)
    {
        if (AllowedContentTypes.TryGetValue(contentType, out var extension))
        {
            return extension;
        }

        var originalExtension = Path.GetExtension(originalFileName);
        return AllowedPackageExtensions.Contains(originalExtension) ? originalExtension.ToLowerInvariant() : ".bin";
    }

    public static string AllowedSummary()
    {
        var contentTypes = string.Join(", ", AllowedContentTypes.Keys);
        var packageExtensions = string.Join(", ", AllowedPackageExtensions.Order());
        return $"{contentTypes}, projekty: {packageExtensions}";
    }
}

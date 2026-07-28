namespace LessonRunner.Application.Files;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>Fizyczny katalog na pliki (ustawiany przez warstwe Api na podstawie content root).</summary>
    public string RootPath { get; set; } = string.Empty;

    /// <summary>Prefiks ścieżki publicznej, pod którą pliki są serwowane.</summary>
    public string RequestPath { get; set; } = "/uploads";
}

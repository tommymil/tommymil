using LessonRunner.Domain.Lessons;

namespace LessonRunner.Application.Lessons;

public sealed record CreateLessonDto(
    string Title,
    string Subject,
    string Level,
    string Description,
    IReadOnlyList<string> Tags,
    CreateLessonProjectFilesDto? ProjectFiles,
    IReadOnlyList<CreateLessonStepDto> Steps,
    int? Order = null,
    /// <summary>Cel dydaktyczny lekcji — czego dziecko ma się nauczyć.</summary>
    string? Objective = null,
    /// <summary>Po zajęciach dziecko potrafi… (sprawdzalne kryteria).</summary>
    IReadOnlyList<string>? SuccessCriteria = null,
    /// <summary>Co przygotować przed zajęciami.</summary>
    IReadOnlyList<string>? Preparation = null,
    /// <summary>Zadanie domowe albo co pokazać rodzicom.</summary>
    IReadOnlyList<string>? Homework = null);

public sealed record CreateLessonProjectFilesDto(
    CreateLessonProjectFileDto? Starter,
    CreateLessonProjectFileDto? Final);

public sealed record CreateLessonProjectFileDto(
    string Label,
    string Url,
    string FileName,
    string ContentType,
    long SizeBytes,
    string? DownloadToken);

public sealed record CreateLessonStepDto(
    string Type,
    string Title,
    int DurationMinutes,
    IReadOnlyList<string> Script,
    IReadOnlyList<CreateStudentItemDto> StudentItems,
    IReadOnlyList<CreateLessonResourceDto> Resources,
    IReadOnlyList<CreateLessonNoteDto> Notes);

public sealed record CreateStudentItemDto(
    string Kind,
    string? Text,
    string? Caption,
    string? Url);

public sealed record CreateLessonResourceDto(
    string Kind,
    string? Label,
    string? Url,
    string? Code,
    string? Language);

public sealed record CreateLessonNoteDto(
    string Kind,
    string Text);

internal static class CreateLessonDtoMapping
{
    public static Lesson ToLesson(this CreateLessonDto dto, int order, Guid? id = null, LessonStatus status = LessonStatus.Draft)
    {
        return new Lesson
        {
            Id = id ?? Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Subject = dto.Subject.Trim(),
            Level = dto.Level.Trim(),
            Description = dto.Description.Trim(),
            Order = order,
            Status = status,
            Tags = dto.Tags.Select(tag => tag.Trim()).Where(tag => tag.Length > 0).ToList(),
            ProjectFiles = dto.ProjectFiles.ToProjectFiles(),
            Steps = dto.Steps.Select((step, index) => step.ToStep(index + 1)).ToList(),
            Objective = NullIfBlank(dto.Objective),
            SuccessCriteria = CleanList(dto.SuccessCriteria),
            Preparation = CleanList(dto.Preparation),
            Homework = CleanList(dto.Homework)
        };
    }

    private static string? NullIfBlank(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static List<string> CleanList(IReadOnlyList<string>? values) =>
        (values ?? []).Select(value => value.Trim()).Where(value => value.Length > 0).ToList();

    private static LessonProjectFiles ToProjectFiles(this CreateLessonProjectFilesDto? dto)
    {
        return new LessonProjectFiles
        {
            Starter = dto?.Starter.ToProjectFile("Lekcja startowa"),
            Final = dto?.Final.ToProjectFile("Lekcja końcowa")
        };
    }

    private static LessonProjectFile? ToProjectFile(this CreateLessonProjectFileDto? dto, string fallbackLabel)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.Url) || string.IsNullOrWhiteSpace(dto.FileName))
        {
            return null;
        }

        return new LessonProjectFile
        {
            Label = string.IsNullOrWhiteSpace(dto.Label) ? fallbackLabel : dto.Label.Trim(),
            Url = dto.Url.Trim(),
            FileName = dto.FileName.Trim(),
            ContentType = string.IsNullOrWhiteSpace(dto.ContentType) ? "application/octet-stream" : dto.ContentType.Trim(),
            SizeBytes = Math.Max(0, dto.SizeBytes),
            DownloadToken = NormalizeDownloadToken(dto.DownloadToken)
        };
    }

    private static string NormalizeDownloadToken(string? token)
    {
        return token is { Length: >= 24 } && token.All(IsTokenChar)
            ? token
            : Guid.NewGuid().ToString("N");
    }

    private static bool IsTokenChar(char value)
    {
        return char.IsAsciiLetterOrDigit(value) || value is '-' or '_';
    }

    private static LessonStep ToStep(this CreateLessonStepDto dto, int order)
    {
        return new LessonStep
        {
            Order = order,
            Type = ParseEnum(dto.Type, LessonStepType.Concept),
            Title = dto.Title.Trim(),
            DurationMinutes = Math.Max(1, dto.DurationMinutes),
            Script = dto.Script.Select(line => line.Trim()).Where(line => line.Length > 0).ToList(),
            StudentItems = dto.StudentItems.Select(ToStudentItem).ToList(),
            Resources = dto.Resources.Select(ToResource).ToList(),
            Notes = dto.Notes.Select(ToNote).ToList()
        };
    }

    private static StudentItem ToStudentItem(CreateStudentItemDto dto)
    {
        return new StudentItem
        {
            Kind = ParseEnum(dto.Kind, StudentItemKind.Text),
            Text = dto.Text,
            Caption = dto.Caption,
            Url = dto.Url
        };
    }

    private static LessonResource ToResource(CreateLessonResourceDto dto)
    {
        return new LessonResource
        {
            Kind = ParseEnum(dto.Kind, LessonResourceKind.Link),
            Label = dto.Label,
            Url = dto.Url,
            Code = dto.Code,
            Language = dto.Language
        };
    }

    private static LessonNote ToNote(CreateLessonNoteDto dto)
    {
        return new LessonNote
        {
            Kind = ParseEnum(dto.Kind, LessonNoteKind.Hint),
            Text = dto.Text.Trim()
        };
    }

    private static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct
    {
        return Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) ? parsed : fallback;
    }
}

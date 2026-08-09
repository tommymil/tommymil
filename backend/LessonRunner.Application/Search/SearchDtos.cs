namespace LessonRunner.Application.Search;

/// <summary>
/// Rodzaj trafienia. Front używa go do etykiety i ikony; ścieżkę dostaje gotową,
/// żeby nie budować adresów z rozsypanych fragmentów po swojej stronie.
/// </summary>
public enum SearchResultKind
{
    Participant,
    Group,
    Guardian,
    Lesson
}

public sealed record SearchResultDto(
    string Kind,
    Guid Id,
    string Title,
    string? Subtitle,
    string Path);

public sealed record SearchResponseDto(IReadOnlyList<SearchResultDto> Results);

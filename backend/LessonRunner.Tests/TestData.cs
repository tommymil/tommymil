using LessonRunner.Application.Lessons;
using LessonRunner.Domain.Lessons;

namespace LessonRunner.Tests;

internal static class TestData
{
    /// <summary>
    /// Buduje domenową lekcję odpowiadającą <see cref="ValidLesson"/>, na potrzeby testów repozytorium.
    /// </summary>
    public static Lesson ToTestLesson(this CreateLessonDto _)
    {
        return new Lesson
        {
            Title = "Pierwsza gra",
            Subject = "Scratch",
            Level = "Poziom 1",
            Description = "Opis lekcji",
            Status = LessonStatus.Draft,
            Tags = ["scratch", "gra"],
            Steps =
            [
                new LessonStep
                {
                    Order = 1,
                    Type = LessonStepType.Intro,
                    Title = "Wprowadzenie",
                    DurationMinutes = 5,
                    Script = ["Przywitaj się", "Pokaż cel lekcji"],
                    StudentItems =
                    [
                    new StudentItem { Kind = StudentItemKind.Text, Text = "Otwórz Scratch" }
                    ],
                    Resources =
                    [
                        new LessonResource { Kind = LessonResourceKind.Link, Label = "Scratch", Url = "https://scratch.mit.edu" },
                        new LessonResource { Kind = LessonResourceKind.Code, Label = "Ruch", Code = "move 10 steps", Language = "scratch" }
                    ],
                    Notes =
                    [
                        new LessonNote { Kind = LessonNoteKind.Hint, Text = "Sprawdź czy wszyscy mają otwarty edytor" }
                    ]
                },
                new LessonStep
                {
                    Order = 2,
                    Type = LessonStepType.Challenge,
                    Title = "Wyzwanie",
                    DurationMinutes = 10,
                    Script = ["Daj zadanie"]
                }
            ]
        };
    }

    public static CreateLessonDto ValidLesson(
        string title = "Pierwsza gra",
        string subject = "Scratch")
    {
        return new CreateLessonDto(
            Title: title,
            Subject: subject,
            Level: "Poziom 1",
            Description: "Opis lekcji",
            Tags: ["scratch", "gra"],
            ProjectFiles: null,
            Steps:
            [
                new CreateLessonStepDto(
                    Type: "intro",
                    Title: "Wprowadzenie",
                    DurationMinutes: 5,
                    Script: ["Przywitaj się", "Pokaż cel lekcji"],
                    StudentItems:
                    [
                    new CreateStudentItemDto("text", "Otwórz Scratch", null, null)
                    ],
                    Resources:
                    [
                        new CreateLessonResourceDto("link", "Scratch", "https://scratch.mit.edu", null, null),
                        new CreateLessonResourceDto("code", "Ruch", null, "move 10 steps", "scratch")
                    ],
                    Notes:
                    [
                        new CreateLessonNoteDto("hint", "Sprawdź czy wszyscy mają otwarty edytor")
                    ]),
                new CreateLessonStepDto(
                    Type: "challenge",
                    Title: "Wyzwanie",
                    DurationMinutes: 10,
                    Script: ["Daj zadanie"],
                    StudentItems: [],
                    Resources: [],
                    Notes: [])
            ]);
    }
}

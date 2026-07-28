namespace LessonRunner.Application.Groups;

public sealed class SchedulingConflictException(string message) : InvalidOperationException(message);

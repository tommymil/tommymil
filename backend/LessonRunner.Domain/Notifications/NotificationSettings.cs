namespace LessonRunner.Domain.Notifications;

public sealed class NotificationSettings
{
    public bool RemindersEnabled { get; set; } = true;
    public bool AbsenceEnabled { get; set; } = true;
    public int ReminderLeadHours { get; set; } = 24;
    public string FromName { get; set; } = "Szkoła Programowania";
    public string FromEmail { get; set; } = "noreply@lessonrunner.local";
    public string ReminderSubject { get; set; } = "Przypomnienie o zajęciach: {{group}}";
    public string ReminderBody { get; set; } =
        "Dzień dobry,\n\nprzypominamy o zajęciach {{group}}: {{sessionAt}}.\nLekcja: {{lesson}}.\n\nSzkoła Programowania";
    public string AbsenceSubject { get; set; } = "Nieobecność na zajęciach: {{group}}";
    public string AbsenceBody { get; set; } =
        "Dzień dobry,\n\nodnotowaliśmy nieobecność uczestnika {{participant}} na zajęciach {{group}} w dniu {{sessionAt}}.\n\nSzkoła Programowania";
}

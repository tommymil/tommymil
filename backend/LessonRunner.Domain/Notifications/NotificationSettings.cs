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
        "Dzień dobry,\n\nprzypominamy o zajęciach {{group}}: {{sessionAt}}.\nLekcja: {{lesson}}.\nLink do spotkania: {{link}}\n\nSzkoła Programowania";
    public string AbsenceSubject { get; set; } = "Nieobecność na zajęciach: {{group}}";
    public string AbsenceBody { get; set; } =
        "Dzień dobry,\n\nodnotowaliśmy nieobecność uczestnika {{participant}} na zajęciach {{group}} w dniu {{sessionAt}}.\n\nSzkoła Programowania";
}

/// <summary>
/// Pozostałe typy powiadomień z rozdziału 6 dokumentu koncepcyjnego: zmiana terminu,
/// podsumowanie zajęć i przypomnienie o płatności.
///
/// Treść jest **wbudowana, a nie edytowalna w panelu** — ta sama decyzja, co przy e-mailach
/// o dostępie do konta (reset hasła, zaproszenie). Powody:
///
/// - Każdy z tych szablonów niesie zmienne, których nie ma w pozostałych
///   (`{{previousAt}}`, `{{reason}}`, `{{summary}}`, `{{amount}}`, `{{dueDate}}`). Wpuszczenie
///   ich do wspólnego edytora oznaczałoby, że administrator może wstawić `{{amount}}`
///   do przypomnienia o zajęciach i dostać w mailu surowy `{{amount}}`.
/// - Wiadomość o zmianie terminu jest dowodem przy reklamacji „nie dostaliśmy informacji”;
///   szablon zepsuty literówką psuje ten dowód po cichu.
///
/// Wyniesienie ich do ustawień to osobna zmiana: dziewięć kolumn, migracja i walidacja
/// zmiennych per typ. Do tego czasu szkoła zmienia tu tekst i wgrywa nową wersję.
/// </summary>
public static class NotificationTemplates
{
    public const string RescheduleSubject = "Zmiana terminu zajęć: {{group}}";

    public const string RescheduleBody =
        "Dzień dobry,\n\nzajęcia {{group}} zostały przeniesione z {{previousAt}} na {{sessionAt}}.\n"
        + "Powód: {{reason}}\nLink do spotkania: {{link}}\n\nSzkoła Programowania";

    public const string CancelSubject = "Odwołane zajęcia: {{group}}";

    public const string CancelBody =
        "Dzień dobry,\n\nzajęcia {{group}} zaplanowane na {{sessionAt}} zostały odwołane.\n"
        + "Powód: {{reason}}\n\nO ewentualnym odrobieniu lub rekompensacie poinformujemy osobno.\n\n"
        + "Szkoła Programowania";

    public const string SummarySubject = "Podsumowanie zajęć: {{group}}";

    public const string SummaryBody =
        "Dzień dobry,\n\nzajęcia {{group}} ({{lesson}}) z {{sessionAt}} za nami.\n\n{{summary}}\n\n"
        + "Szczegóły i materiały znajdą Państwo w portalu rodzica.\n\nSzkoła Programowania";

    public const string PaymentSubject = "Przypomnienie o płatności: {{group}}";

    public const string PaymentBody =
        "Dzień dobry,\n\nprzypominamy o płatności za zajęcia {{group}}: {{amount}}, termin {{dueDate}}.\n"
        + "Jeśli płatność została już wykonana, prosimy zignorować tę wiadomość.\n\nSzkoła Programowania";
}

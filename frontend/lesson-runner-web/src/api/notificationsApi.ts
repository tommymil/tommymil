import { apiGet, apiPostEmpty, apiPut } from "./client";
import type { NotificationLog, NotificationPreview, NotificationSettings } from "../types/notification";

export function getNotificationSettings(): Promise<NotificationSettings> {
  return apiGet<NotificationSettings>("/api/notifications/settings");
}

export function updateNotificationSettings(settings: NotificationSettings): Promise<NotificationSettings> {
  return apiPut<NotificationSettings, NotificationSettings>("/api/notifications/settings", settings);
}

export function getNotificationLogs(limit = 100): Promise<NotificationLog[]> {
  return apiGet<NotificationLog[]>(`/api/notifications/logs?limit=${limit}`);
}

export function previewNotification(type: "reminder" | "absence"): Promise<NotificationPreview> {
  return apiGet<NotificationPreview>(`/api/notifications/preview/${type}`);
}

export function sendReminderNotifications(): Promise<void> {
  return apiPostEmpty<void>("/api/notifications/send-reminders");
}

/**
 * Przypomnienia o zaległych płatnościach.
 *
 * Świadomie osobny przycisk, a nie przełącznik w ustawieniach: upominanie się o pieniądze
 * to decyzja biznesowa, którą szkoła podejmuje za każdym razem, a nie ustawia raz i zapomina.
 * Odpowiedź niesie liczbę wysłanych wiadomości.
 */
export function sendPaymentReminders(): Promise<{ sent: number }> {
  return apiPostEmpty<{ sent: number }>("/api/notifications/send-payment-reminders");
}

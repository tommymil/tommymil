export type NotificationSettings = {
  remindersEnabled: boolean;
  absenceEnabled: boolean;
  reminderLeadHours: number;
  fromName: string;
  fromEmail: string;
  reminderSubject: string;
  reminderBody: string;
  absenceSubject: string;
  absenceBody: string;
};

export type NotificationLog = {
  id: string;
  createdAt: string;
  sentAt: string | null;
  type: string;
  channel: string;
  recipient: string;
  subject: string;
  status: string;
  dedupeKey: string;
  error: string | null;
};

export type NotificationPreview = {
  type: string;
  subject: string;
  body: string;
};

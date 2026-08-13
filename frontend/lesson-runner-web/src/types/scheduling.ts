export type Location = {
  id: string;
  name: string;
  description: string | null;
  isActive: boolean;
};

export type Holiday = {
  id: string;
  date: string;
  name: string;
};

export type CalendarSession = {
  id: string;
  groupId: string;
  groupName: string;
  instructorId: string;
  instructorName: string;
  lessonId: string | null;
  lessonTitle: string | null;
  scheduledAt: string;
  sequenceNumber: number;
  status: string;
  statusLabel: string;
  locationId: string | null;
  locationName: string | null;
};

export type CalendarData = {
  sessions: CalendarSession[];
  holidays: Holiday[];
  locations: Location[];
};

export type UpsertLocationRequest = {
  name: string;
  description: string | null;
  isActive: boolean;
};

export type UpsertHolidayRequest = {
  date: string;
  name: string;
};

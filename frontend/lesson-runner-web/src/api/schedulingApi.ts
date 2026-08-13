import { apiDelete, apiGet, apiPost, apiPut } from "./client";
import type {
  CalendarData,
  Holiday,
  Location,
  UpsertHolidayRequest,
  UpsertLocationRequest,
} from "../types/scheduling";

export function getLocations(): Promise<Location[]> {
  return apiGet<Location[]>("/api/locations");
}

export function createLocation(request: UpsertLocationRequest): Promise<Location> {
  return apiPost<UpsertLocationRequest, Location>("/api/locations", request);
}

export function updateLocation(id: string, request: UpsertLocationRequest): Promise<Location> {
  return apiPut<UpsertLocationRequest, Location>(`/api/locations/${id}`, request);
}

export function deleteLocation(id: string): Promise<void> {
  return apiDelete(`/api/locations/${id}`);
}

export function getHolidays(): Promise<Holiday[]> {
  return apiGet<Holiday[]>("/api/holidays");
}

export function createHoliday(request: UpsertHolidayRequest): Promise<Holiday> {
  return apiPost<UpsertHolidayRequest, Holiday>("/api/holidays", request);
}

export function updateHoliday(id: string, request: UpsertHolidayRequest): Promise<Holiday> {
  return apiPut<UpsertHolidayRequest, Holiday>(`/api/holidays/${id}`, request);
}

export function deleteHoliday(id: string): Promise<void> {
  return apiDelete(`/api/holidays/${id}`);
}

export function getCalendar(from?: string, to?: string): Promise<CalendarData> {
  const params = new URLSearchParams();

  if (from) {
    params.set("from", from);
  }

  if (to) {
    params.set("to", to);
  }

  const query = params.toString();
  return apiGet<CalendarData>(query ? `/api/calendar?${query}` : "/api/calendar");
}

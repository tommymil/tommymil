import { apiGet } from "./client";
import type { Dashboard } from "../types/dashboard";

export function getDashboard(): Promise<Dashboard> {
  return apiGet<Dashboard>("/api/dashboard");
}

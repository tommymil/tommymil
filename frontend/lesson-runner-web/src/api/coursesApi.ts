import { apiDelete, apiGet, apiPost, apiPut } from "./client";
import type { CourseDetails, CourseSummary, UpsertCourseRequest } from "../types/course";

export function getCourses(): Promise<CourseSummary[]> {
  return apiGet<CourseSummary[]>("/api/courses");
}

export function getCourse(id: string): Promise<CourseDetails> {
  return apiGet<CourseDetails>(`/api/courses/${id}`);
}

export function createCourse(request: UpsertCourseRequest): Promise<CourseDetails> {
  return apiPost<UpsertCourseRequest, CourseDetails>("/api/courses", request);
}

export function updateCourse(id: string, request: UpsertCourseRequest): Promise<CourseDetails> {
  return apiPut<UpsertCourseRequest, CourseDetails>(`/api/courses/${id}`, request);
}

export function deleteCourse(id: string): Promise<void> {
  return apiDelete(`/api/courses/${id}`);
}

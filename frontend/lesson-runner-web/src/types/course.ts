export type CourseLesson = {
  lessonId: string;
  lessonTitle: string;
  subject: string;
  level: string;
  order: number;
};

export type CourseSummary = {
  id: string;
  name: string;
  subject: string;
  level: string;
  description: string;
  lessonCount: number;
};

export type CourseDetails = CourseSummary & {
  lessons: CourseLesson[];
};

export type UpsertCourseRequest = {
  name: string;
  subject: string;
  level: string;
  description: string;
  lessonIds: string[];
};

import { createBrowserRouter } from "react-router-dom";
import { App } from "./App";
import { AdminBillingPage } from "../pages/AdminBillingPage";
import { AdminCoursesPage } from "../pages/AdminCoursesPage";
import { AdminDashboardPage } from "../pages/AdminDashboardPage";
import { AdminGroupsPage } from "../pages/AdminGroupsPage";
import { AdminLibraryPage } from "../pages/AdminLibraryPage";
import { AdminOperationsPage } from "../pages/AdminOperationsPage";
import { AdminNotificationsPage } from "../pages/AdminNotificationsPage";
import { AdminParticipantsPage } from "../pages/AdminParticipantsPage";
import { AdminSafetyPage } from "../pages/AdminSafetyPage";
import { AdminTrialsPage } from "../pages/AdminTrialsPage";
import { AdminUsersPage } from "../pages/AdminUsersPage";
import { CalendarPage } from "../pages/CalendarPage";
import { GroupDetailsPage } from "../pages/GroupDetailsPage";
import { GroupEditorPage } from "../pages/GroupEditorPage";
import { InstructorLessonsPage } from "../pages/InstructorLessonsPage";
import { InstructorSchedulePage } from "../pages/InstructorSchedulePage";
import { InstructorTrialsPage } from "../pages/InstructorTrialsPage";
import { LessonEditorPage } from "../pages/LessonEditorPage";
import { LessonImportPage } from "../pages/LessonImportPage";
import { LoginPage } from "../pages/LoginPage";
import { ParentPortalPage } from "../pages/ParentPortalPage";
import { PresenterPage } from "../pages/PresenterPage";
import { ProfilePage } from "../pages/ProfilePage";
import { ResetPasswordPage } from "../pages/ResetPasswordPage";
import { SetPasswordPage } from "../pages/SetPasswordPage";
import { SessionCockpitPage } from "../pages/SessionCockpitPage";
import { NotFoundPage, RouteErrorPage } from "../pages/FaultPages";
import { RequireAdmin, RequireAuth, RequireParent, RequireStaff } from "../features/auth/RouteGuards";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    // Bez tego błąd renderowania albo nieznany adres kończy się własnym, angielskim
    // ekranem diagnostycznym React Routera - dla rodzica wygląda jak awaria serwera.
    errorElement: <RouteErrorPage />,
    children: [
      { index: true, element: <LoginPage /> },
      // Trasy publiczne: wchodzi się na nie z linku w e-mailu albo z ekranu logowania,
      // więc muszą stać poza wszystkimi strażnikami.
      { path: "reset-password", element: <ResetPasswordPage /> },
      { path: "set-password", element: <SetPasswordPage /> },
      {
        element: <RequireAdmin />,
        children: [
          { path: "admin/dashboard", element: <AdminDashboardPage /> },
          { path: "admin/lessons", element: <AdminLibraryPage /> },
          { path: "admin/lessons/import", element: <LessonImportPage /> },
          { path: "admin/lessons/new", element: <LessonEditorPage /> },
          { path: "admin/lessons/:lessonId/edit", element: <LessonEditorPage /> },
          { path: "admin/courses", element: <AdminCoursesPage /> },
          { path: "admin/billing", element: <AdminBillingPage /> },
          { path: "admin/groups", element: <AdminGroupsPage /> },
          { path: "admin/groups/new", element: <GroupEditorPage /> },
          { path: "admin/groups/:groupId", element: <GroupDetailsPage /> },
          { path: "admin/participants", element: <AdminParticipantsPage /> },
          { path: "admin/trials", element: <AdminTrialsPage /> },
          { path: "admin/users", element: <AdminUsersPage /> },
          { path: "admin/notifications", element: <AdminNotificationsPage /> },
          { path: "admin/safety", element: <AdminSafetyPage /> },
          { path: "admin/operations", element: <AdminOperationsPage /> },
        ],
      },
      {
        element: <RequireAuth />,
        children: [
          { path: "profile", element: <ProfilePage /> },
        ],
      },
      {
        element: <RequireParent />,
        children: [
          { path: "parent/portal", element: <ParentPortalPage /> },
        ],
      },
      {
        element: <RequireStaff />,
        children: [
          { path: "calendar", element: <CalendarPage /> },
          { path: "instructor/lessons", element: <InstructorLessonsPage /> },
          { path: "instructor/schedule", element: <InstructorSchedulePage /> },
          { path: "instructor/trials", element: <InstructorTrialsPage /> },
          { path: "instructor/sessions/:sessionId", element: <SessionCockpitPage /> },
          { path: "presenter/:lessonId", element: <PresenterPage /> },
        ],
      },
      { path: "*", element: <NotFoundPage /> },
    ],
  },
]);

using LessonRunner.Application.Auth;
using LessonRunner.Application.Audit;
using LessonRunner.Application.Billing;
using LessonRunner.Application.Courses;
using LessonRunner.Application.Dashboard;
using LessonRunner.Application.Groups;
using LessonRunner.Application.LessonRuns;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Notifications;
using LessonRunner.Application.Participants;
using LessonRunner.Application.Parents;
using LessonRunner.Application.Operations;
using LessonRunner.Application.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace LessonRunner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILessonCommands, LessonCommands>();
        services.AddScoped<ILessonQueries, LessonQueries>();
        services.AddScoped<ILessonRunSessionService, LessonRunSessionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IBillingService, BillingService>();
        services.AddScoped<IPaymentProvider, ManualPaymentProvider>();
        services.AddScoped<IUserAdminService, UserAdminService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IParticipantService, ParticipantService>();
        services.AddScoped<IParentPortalService, ParentPortalService>();
        services.AddScoped<ISchedulingService, SchedulingService>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}

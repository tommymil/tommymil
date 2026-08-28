using LessonRunner.Application.Auth;
using LessonRunner.Application.Audit;
using LessonRunner.Application.Billing;
using LessonRunner.Application.Courses;
using LessonRunner.Application.Dashboard;
using LessonRunner.Application.Groups;
using LessonRunner.Application.LessonRuns;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Materials;
using LessonRunner.Application.Notifications;
using LessonRunner.Application.Participants;
using LessonRunner.Application.Parents;
using LessonRunner.Application.Operations;
using LessonRunner.Application.Progress;
using LessonRunner.Application.Safety;
using LessonRunner.Application.Trials;
using LessonRunner.Application.Scheduling;
using LessonRunner.Application.Search;
using Microsoft.Extensions.DependencyInjection;

namespace LessonRunner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILessonCommands, LessonCommands>();
        services.AddScoped<ILessonQueries, LessonQueries>();
        services.AddScoped<ILessonRunSessionService, LessonRunSessionService>();
        services.AddScoped<IMaterialService, MaterialService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IBillingService, BillingService>();
        services.AddScoped<IPaymentProvider, ManualPaymentProvider>();
        services.AddScoped<IUserAdminService, UserAdminService>();
        services.AddScoped<IAccountTokenService, AccountTokenService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IParticipantService, ParticipantService>();
        services.AddScoped<IParentPortalService, ParentPortalService>();
        services.AddScoped<IGuardianDirectory, GuardianDirectory>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<ISchedulingService, SchedulingService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<ISafetyService, SafetyService>();
        services.AddScoped<ITrialService, TrialService>();

        return services;
    }
}

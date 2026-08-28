using LessonRunner.Application.Auth;
using LessonRunner.Application.Audit;
using LessonRunner.Application.Billing;
using LessonRunner.Application.Courses;
using LessonRunner.Application.Files;
using LessonRunner.Application.Groups;
using LessonRunner.Application.LessonRuns;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Materials;
using LessonRunner.Application.Operations;
using LessonRunner.Application.Notifications;
using LessonRunner.Application.Participants;
using LessonRunner.Application.Parents;
using LessonRunner.Application.Progress;
using LessonRunner.Application.Safety;
using LessonRunner.Application.Trials;
using LessonRunner.Application.Scheduling;
using LessonRunner.Infrastructure.Auth;
using LessonRunner.Infrastructure.Audit;
using LessonRunner.Infrastructure.Billing;
using LessonRunner.Infrastructure.Courses;
using LessonRunner.Infrastructure.Files;
using LessonRunner.Infrastructure.Groups;
using LessonRunner.Infrastructure.LessonRuns;
using LessonRunner.Infrastructure.Lessons;
using LessonRunner.Infrastructure.Materials;
using LessonRunner.Infrastructure.Notifications;
using LessonRunner.Infrastructure.Operations;
using LessonRunner.Infrastructure.Participants;
using LessonRunner.Infrastructure.Parents;
using LessonRunner.Infrastructure.Persistence;
using LessonRunner.Infrastructure.Progress;
using LessonRunner.Infrastructure.Safety;
using LessonRunner.Infrastructure.Scheduling;
using LessonRunner.Infrastructure.Trials;
using LessonRunner.Domain.Notifications;
using LessonRunner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LessonRunner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=lesson-runner.db";

        services.AddDbContext<AppDbContext>(options =>
        {
            if (ShouldUseSqlite(connectionString))
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseNpgsql(connectionString);
            }
        });
        services.AddScoped<ILessonRepository, EfLessonRepository>();
        services.AddScoped<ILessonRunSessionRepository, EfLessonRunSessionRepository>();
        services.AddScoped<IMaterialRepository, EfMaterialRepository>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IAccountTokenRepository, EfAccountTokenRepository>();
        services.AddScoped<IGroupRepository, EfGroupRepository>();
        services.AddScoped<IParticipantRepository, EfParticipantRepository>();
        services.AddScoped<IParentPortalRepository, EfParentPortalRepository>();
        services.AddScoped<IProgressRepository, EfProgressRepository>();
        services.AddScoped<ICourseRepository, EfCourseRepository>();
        services.AddScoped<ISchedulingRepository, EfSchedulingRepository>();
        services.AddScoped<IAuditRepository, EfAuditRepository>();
        services.AddScoped<IBillingRepository, EfBillingRepository>();
        services.AddScoped<INotificationRepository, EfNotificationRepository>();
        services.AddScoped<IIncidentRepository, EfIncidentRepository>();
        services.AddScoped<ITrialRepository, EfTrialRepository>();
        services.AddScoped<ISupportTicketRepository, EfSupportTicketRepository>();
        services.Configure<SmtpOptions>(options =>
        {
            options.Mode = configuration[$"{SmtpOptions.SectionName}:Mode"] ?? "Log";
            options.Host = configuration[$"{SmtpOptions.SectionName}:Host"] ?? "";
            options.Port = int.TryParse(configuration[$"{SmtpOptions.SectionName}:Port"], out var port) ? port : 587;
            options.Username = configuration[$"{SmtpOptions.SectionName}:Username"] ?? "";
            options.Password = configuration[$"{SmtpOptions.SectionName}:Password"] ?? "";
            options.EnableSsl = !string.Equals(configuration[$"{SmtpOptions.SectionName}:EnableSsl"], "false", StringComparison.OrdinalIgnoreCase);
        });
        services.Configure<NotificationWorkerOptions>(options =>
        {
            options.IntervalMinutes = int.TryParse(configuration["Notifications:ReminderIntervalMinutes"], out var minutes) ? minutes : 30;
        });

        // Nadawca dla instalacji, w której nikt jeszcze nie zapisał ustawień powiadomień.
        // Bez tego obowiązywał adres z domeny `.local`, której nie ma w DNS - patrz NotificationDefaults.
        var settingsDefaults = new NotificationSettings();
        services.AddSingleton(new NotificationDefaults
        {
            FromName = configuration["Notifications:FromName"] is { Length: > 0 } fromName
                ? fromName
                : settingsDefaults.FromName,
            FromEmail = configuration["Notifications:FromEmail"] is { Length: > 0 } fromEmail
                ? fromEmail
                : settingsDefaults.FromEmail
        });
        services.AddScoped<IEmailSender>(provider =>
        {
            var smtp = provider.GetRequiredService<IOptions<SmtpOptions>>().Value;
            return string.Equals(smtp.Mode, "Smtp", StringComparison.OrdinalIgnoreCase)
                ? ActivatorUtilities.CreateInstance<SmtpEmailSender>(provider)
                : ActivatorUtilities.CreateInstance<LogEmailSender>(provider);
        });
        services.AddHostedService<NotificationReminderWorker>();
        services.Configure<BackupOptions>(options =>
        {
            options.RootPath = configuration[$"{BackupOptions.SectionName}:RootPath"] ?? "backups";
        });
        services.AddScoped<IBackupService, LocalBackupService>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtOptions = new JwtOptions
        {
            Issuer = jwtSection["Issuer"] ?? "LessonRunner",
            Audience = jwtSection["Audience"] ?? "LessonRunnerClients",
            SigningKey = jwtSection["SigningKey"] ?? string.Empty,
            ExpiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var expiryMinutes) ? expiryMinutes : 120
        };
        services.AddSingleton(Options.Create(jwtOptions));
        services.AddSingleton<ITokenService, JwtTokenService>();

        // Adres, od którego budujemy linki w e-mailach. Świadomie z konfiguracji, nie z nagłówka
        // Host żądania - ten podlega podmianie przez klienta, a link w mailu prowadziłby wtedy
        // pod adres atakującego.
        services.AddSingleton(new AppOptions
        {
            PublicOrigin = configuration[$"{AppOptions.SectionName}:PublicOrigin"] is { Length: > 0 } origin
                ? origin
                : new AppOptions().PublicOrigin
        });

        services.AddSingleton<IFileStorage, LocalFileStorage>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(
        this IServiceProvider serviceProvider,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (dbContext.Database.IsSqlite())
        {
            await LegacyParticipantBackfill.RunAsync(dbContext, cancellationToken);
        }

        await BootstrapAdminAsync(scope.ServiceProvider, configuration, cancellationToken);
    }

    private static bool ShouldUseSqlite(string connectionString)
    {
        return connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
            || connectionString.EndsWith(".db", StringComparison.OrdinalIgnoreCase)
            || connectionString.StartsWith("Filename=", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task BootstrapAdminAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

        if (await userRepository.HasAnyAdminAsync(cancellationToken))
        {
            return;
        }

        var email = configuration["BOOTSTRAP_ADMIN_EMAIL"];
        var password = configuration["BOOTSTRAP_ADMIN_PASSWORD"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Brak konta administratora. Ustaw BOOTSTRAP_ADMIN_EMAIL i BOOTSTRAP_ADMIN_PASSWORD dla pierwszego uruchomienia.");
        }

        if (!email.Contains('@'))
        {
            throw new InvalidOperationException("BOOTSTRAP_ADMIN_EMAIL musi być prawidłowym adresem e-mail.");
        }

        if (password.Length < 8)
        {
            throw new InvalidOperationException("BOOTSTRAP_ADMIN_PASSWORD musi mieć co najmniej 8 znaków.");
        }

        var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();
        await userRepository.AddAsync(
            new User
            {
                Email = User.NormalizeEmail(email),
                PasswordHash = passwordHasher.Hash(password),
                Role = UserRole.Admin
            },
            cancellationToken);
    }

}

using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using LessonRunner.Application;
using LessonRunner.Application.Auth;
using LessonRunner.Application.Audit;
using LessonRunner.Application.Billing;
using LessonRunner.Application.Courses;
using LessonRunner.Application.Dashboard;
using LessonRunner.Application.Files;
using LessonRunner.Application.Groups;
using LessonRunner.Application.LessonRuns;
using LessonRunner.Application.Lessons;
using LessonRunner.Application.Notifications;
using LessonRunner.Application.Operations;
using LessonRunner.Application.Participants;
using LessonRunner.Application.Parents;
using LessonRunner.Application.Progress;
using LessonRunner.Application.Safety;
using LessonRunner.Application.Scheduling;
using LessonRunner.Application.Search;
using LessonRunner.Api.Auditing;
using LessonRunner.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddCors(options =>
{
    // Puste wpisy odfiltrowujemy: w compose `Cors__AllowedOrigins__0` bywa pustym stringiem,
    // gdy frontend i API stoją za tym samym originem (nginx proxuje /api) i CORS nie jest
    // w ogóle potrzebny. Pusta lista = żaden zewnętrzny origin nie przejdzie.
    var allowedOrigins = (builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
        .Where(origin => !string.IsNullOrWhiteSpace(origin))
        .Select(origin => origin.Trim().TrimEnd('/'))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
{
    throw new InvalidOperationException(
        "Brak klucza Jwt:SigningKey. Ustaw go w appsettings.Development.json, appsettings.Production.json lub zmiennej środowiskowej Jwt__SigningKey.");
}

if (Encoding.UTF8.GetByteCount(jwtOptions.SigningKey) < 32)
{
    throw new InvalidOperationException("Jwt:SigningKey musi miec co najmniej 32 bajty.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        options.Events = new JwtBearerEvents
        {
            // Sam podpis tokenu nie wystarczy: konto mogło zostać w międzyczasie wyłączone,
            // a hasło zmienione. Bez tej kontroli skradziony lub cofnięty token działałby
            // do końca swojej ważności, czyli nawet 12 godzin.
            OnTokenValidated = async context =>
            {
                var principal = context.Principal;
                var rawUserId = principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!Guid.TryParse(rawUserId, out var userId))
                {
                    context.Fail("Token bez identyfikatora użytkownika.");
                    return;
                }

                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var user = await userRepository.GetByIdAsync(userId, context.HttpContext.RequestAborted);

                if (user is null || !user.IsActive)
                {
                    context.Fail("Konto nie istnieje lub zostało wyłączone.");
                    return;
                }

                // Pusty znacznik = konto sprzed migracji `AddUserSecurityStamp`. Nie wylogowujemy
                // takich sesji siłowo; znacznik uzupełni się przy najbliższej zmianie hasła.
                if (!string.IsNullOrEmpty(user.SecurityStamp)
                    && !string.Equals(
                        principal?.FindFirstValue(AuthClaimTypes.SecurityStamp),
                        user.SecurityStamp,
                        StringComparison.Ordinal))
                {
                    context.Fail("Sesja została unieważniona.");
                }
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(nameof(LessonRunner.Domain.Users.UserRole.Admin)));
    options.AddPolicy("ParentOnly", policy => policy.RequireRole(nameof(LessonRunner.Domain.Users.UserRole.Parent)));

    // Personel szkoły: konspekty, grafik i kalendarz. Rodzic ma własny portal i nie może
    // czytać treści lekcji (scenariusz prowadzenia, notatki i wskazówki instruktora).
    options.AddPolicy("StaffOnly", policy => policy.RequireRole(
        nameof(LessonRunner.Domain.Users.UserRole.Admin),
        nameof(LessonRunner.Domain.Users.UserRole.Instructor)));
});

// Ochrona przed atakiem słownikowym na logowanie i reset hasła. Partycjonujemy po adresie IP;
// za reverse proxy pamiętaj o przekazywaniu X-Forwarded-For (UseForwardedHeaders).
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
        }

        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { error = "Zbyt wiele prób. Spróbuj ponownie za chwilę." },
            cancellationToken);
    };

    options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(5),
            QueueLimit = 0
        }));
});

var uploadsRoot = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
builder.Services.Configure<FileStorageOptions>(options =>
{
    options.RootPath = uploadsRoot;
    options.RequestPath = "/uploads";
});

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.Services.InitializeDatabaseAsync(app.Configuration, app.Environment.IsDevelopment(), app.Lifetime.ApplicationStopping);

if (!app.Environment.IsDevelopment())
{
    // Za reverse proxy (nginx w docker compose) prawdziwy adres klienta i schemat przychodzą
    // w nagłówkach X-Forwarded-*. Bez tego rate limiter widziałby jeden adres IP proxy
    // dla wszystkich użytkowników i blokowałby całą szkołę po kilku próbach logowania.
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    // TLS zwykle kończy się na proxy, a kontener API mówi po HTTP. Wymuszanie HTTPS
    // wewnątrz aplikacji dawałoby wtedy pętlę przekierowań, dlatego jest to opcja włączana
    // świadomie (Security__ForceHttps=true) - dla wdrożeń, gdzie API stoi bezpośrednio na TLS.
    if (app.Configuration.GetValue("Security:ForceHttps", false))
    {
        app.UseHsts();
        app.UseHttpsRedirection();
    }
}

app.UseCors("Frontend");

Directory.CreateDirectory(uploadsRoot);

// Pliki mają nazwy GUID i są serwowane statycznie, bo trafiają do <img src> w edytorze
// i prezenterze, a znacznik obrazu nie wyśle nagłówka Authorization.
// Świadomie przyjęte ryzyko: kto zna pełny adres, ten pobierze plik.
// Dlatego: (1) nie cache'ujemy w proxy ani na dysku przeglądarki,
// (2) nie serwujemy nieznanych rozszerzeń, (3) materiały kierowane do rodziców idą
// osobną, autoryzowaną ścieżką (`/download/lesson-files/{token}`), a nie tym katalogiem.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsRoot),
    RequestPath = "/uploads",
    ServeUnknownFileTypes = false,
    OnPrepareResponse = context =>
    {
        context.Context.Response.Headers.CacheControl = "private, no-store";
        context.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    }
});

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", async (
    AppDbContext dbContext,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) =>
{
    var databaseHealthy = false;

    try
    {
        databaseHealthy = await dbContext.Database.CanConnectAsync(cancellationToken);
    }
    catch (Exception ex)
    {
        // Endpoint jest anonimowy, więc na zewnątrz nie wychodzi ani connection string,
        // ani nazwa hosta bazy - szczegóły trafiają wyłącznie do logu.
        loggerFactory.CreateLogger("Health").LogError(ex, "Health check bazy danych nie powiódł się.");
    }

    var status = databaseHealthy ? "Healthy" : "Unhealthy";
    var payload = new
    {
        status,
        checks = new[]
        {
            new
            {
                name = "database",
                status,
                description = databaseHealthy ? "Database connection is available." : "Database connection failed."
            }
        }
    };

    return databaseHealthy ? Results.Ok(payload) : Results.Json(payload, statusCode: StatusCodes.Status503ServiceUnavailable);
})
.AllowAnonymous()
.WithTags("Health")
.WithName("Health");

app.MapGet("/download/lesson-files/{token}", async (
    string token,
    ILessonRepository lessonRepository,
    IOptions<FileStorageOptions> storageOptions,
    CancellationToken cancellationToken) =>
{
    if (!IsDownloadToken(token))
    {
        return Results.NotFound();
    }

    var lessons = await lessonRepository.ListAsync(cancellationToken);
    var file = lessons
        .SelectMany(GetProjectFiles)
        .FirstOrDefault(item => string.Equals(item.DownloadToken, token, StringComparison.Ordinal));

    if (file is null)
    {
        return Results.NotFound();
    }

    var path = ResolveStoredFilePath(file.Url, storageOptions.Value);

    return path is null || !File.Exists(path)
        ? Results.NotFound()
        : Results.File(path, file.ContentType, file.FileName);
})
.WithTags("Files")
.WithName("DownloadLessonFile");

// Pobranie wersji projektu dziecka. Ten sam kompromis, co przy plikach lekcji: link do pobrania
// nie niesie nagłówka `Authorization`, więc uprawnienie siedzi w nieodgadywalnym kluczu w adresie.
app.MapGet("/download/project-files/{token}", async (
    string token,
    IProgressRepository progressRepository,
    IOptions<FileStorageOptions> storageOptions,
    CancellationToken cancellationToken) =>
{
    if (!IsDownloadToken(token))
    {
        return Results.NotFound();
    }

    var submission = await progressRepository.GetSubmissionByTokenAsync(token, cancellationToken);

    if (submission?.FileUrl is null)
    {
        return Results.NotFound();
    }

    var path = ResolveStoredFilePath(submission.FileUrl, storageOptions.Value);

    return path is null || !File.Exists(path)
        ? Results.NotFound()
        : Results.File(path, submission.ContentType ?? "application/octet-stream", submission.FileName);
})
.WithTags("Files")
.WithName("DownloadProjectFile");

var lessons = app.MapGroup("/api/lessons").WithTags("Lessons").RequireAuthorization("StaffOnly").AddEndpointFilter<AuditEndpointFilter>();

lessons.MapGet("/", async (ILessonQueries lessonQueries, CancellationToken cancellationToken) =>
{
    var summaries = await lessonQueries.GetSummariesAsync(cancellationToken);
    return Results.Ok(summaries);
})
.WithName("GetLessons");

lessons.MapGet("/{id:guid}", async (Guid id, ILessonQueries lessonQueries, CancellationToken cancellationToken) =>
{
    var lesson = await lessonQueries.GetDetailsAsync(id, cancellationToken);
    return lesson is null ? Results.NotFound() : Results.Ok(lesson);
})
.WithName("GetLessonById");

lessons.MapPost("/", async (CreateLessonDto dto, ILessonCommands lessonCommands, CancellationToken cancellationToken) =>
{
    try
    {
        var lesson = await lessonCommands.CreateAsync(dto, cancellationToken);
        return Results.Created($"/api/lessons/{lesson.Id}", lesson);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.RequireAuthorization("AdminOnly")
.WithName("CreateLesson");

lessons.MapPut("/{id:guid}", async (Guid id, CreateLessonDto dto, ILessonCommands lessonCommands, CancellationToken cancellationToken) =>
{
    try
    {
        var lesson = await lessonCommands.UpdateAsync(id, dto, cancellationToken);
        return lesson is null ? Results.NotFound() : Results.Ok(lesson);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.RequireAuthorization("AdminOnly")
.WithName("UpdateLesson");

lessons.MapPost("/{id:guid}/send-to-review", async (Guid id, ILessonCommands lessonCommands, CancellationToken cancellationToken) =>
{
    var lesson = await lessonCommands.SendToReviewAsync(id, cancellationToken);
    return lesson is null ? Results.NotFound() : Results.Ok(lesson);
})
.RequireAuthorization("AdminOnly")
.WithName("SendLessonToReview");

lessons.MapPost("/{id:guid}/publish", async (Guid id, ILessonCommands lessonCommands, CancellationToken cancellationToken) =>
{
    var lesson = await lessonCommands.PublishAsync(id, cancellationToken);
    return lesson is null ? Results.NotFound() : Results.Ok(lesson);
})
.RequireAuthorization("AdminOnly")
.WithName("PublishLesson");

lessons.MapDelete("/{id:guid}", async (Guid id, ILessonCommands lessonCommands, CancellationToken cancellationToken) =>
{
    try
    {
        var deleted = await lessonCommands.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
})
.RequireAuthorization("AdminOnly")
.WithName("DeleteLesson");

lessons.MapPost("/{id:guid}/run-session", async (
    Guid id,
    Guid? scheduledSessionId,
    ClaimsPrincipal principal,
    ILessonRunSessionService sessionService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var session = await sessionService.GetOrCreateAsync(id, userId.Value, scheduledSessionId ?? Guid.Empty, cancellationToken);
    return session is null ? Results.NotFound() : Results.Ok(session);
})
.WithName("GetOrCreateLessonRunSession");

lessons.MapPut("/{id:guid}/run-session", async (
    Guid id,
    Guid? scheduledSessionId,
    UpdateLessonRunSessionDto dto,
    ClaimsPrincipal principal,
    ILessonRunSessionService sessionService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var session = await sessionService.UpdateAsync(id, userId.Value, scheduledSessionId ?? Guid.Empty, dto, cancellationToken);
    return session is null ? Results.NotFound() : Results.Ok(session);
})
.WithName("UpdateLessonRunSession");

// --- Grupy (administracja) ---
var groups = app.MapGroup("/api/groups").WithTags("Groups").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

groups.MapGet("/", async (IGroupService groupService, CancellationToken cancellationToken) =>
    Results.Ok(await groupService.GetSummariesAsync(cancellationToken)))
.WithName("GetGroups");

groups.MapGet("/{id:guid}", async (Guid id, IGroupService groupService, CancellationToken cancellationToken) =>
{
    var group = await groupService.GetDetailsAsync(id, cancellationToken);
    return group is null ? Results.NotFound() : Results.Ok(group);
})
.WithName("GetGroupById");

groups.MapPost("/", async (CreateGroupDto dto, IGroupService groupService, CancellationToken cancellationToken) =>
{
    try
    {
        var group = await groupService.CreateAsync(dto, cancellationToken);
        return Results.Created($"/api/groups/{group.Id}", group);
    }
    catch (SchedulingConflictException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateGroup");

groups.MapPut("/{id:guid}", async (Guid id, UpdateGroupDto dto, IGroupService groupService, CancellationToken cancellationToken) =>
{
    try
    {
        var group = await groupService.UpdateAsync(id, dto, cancellationToken);
        return group is null ? Results.NotFound() : Results.Ok(group);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdateGroup");

groups.MapDelete("/{id:guid}", async (Guid id, IGroupService groupService, CancellationToken cancellationToken) =>
    await groupService.DeleteAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteGroup");

groups.MapPost("/{id:guid}/sessions", async (
    Guid id,
    AddSessionDto dto,
    ClaimsPrincipal principal,
    IGroupService groupService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var session = await groupService.AddSessionAsync(id, dto, GetCurrentUserId(principal), cancellationToken);
        return session is null ? Results.NotFound() : Results.Ok(session);
    }
    catch (SchedulingConflictException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("AddSession");

groups.MapPost("/{id:guid}/sessions/{sessionId:guid}/cancel", async (
    Guid id,
    Guid sessionId,
    CancelSessionDto? dto,
    ClaimsPrincipal principal,
    IGroupService groupService,
    CancellationToken cancellationToken) =>
{
    var session = await groupService.CancelSessionAsync(id, sessionId, dto, GetCurrentUserId(principal), cancellationToken);
    return session is null ? Results.NotFound() : Results.Ok(session);
})
.WithName("CancelSession");

groups.MapPost("/{id:guid}/sessions/{sessionId:guid}/reschedule", async (
    Guid id,
    Guid sessionId,
    RescheduleSessionDto dto,
    ClaimsPrincipal principal,
    IGroupService groupService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var session = await groupService.RescheduleSessionAsync(id, sessionId, dto, GetCurrentUserId(principal), cancellationToken);
        return session is null ? Results.NotFound() : Results.Ok(session);
    }
    catch (SchedulingConflictException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("RescheduleSession");

groups.MapPost("/{id:guid}/sessions/{sessionId:guid}/substitute", async (
    Guid id,
    Guid sessionId,
    SetSubstituteInstructorDto dto,
    ClaimsPrincipal principal,
    IGroupService groupService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var session = await groupService.SetSubstituteInstructorAsync(id, sessionId, dto, GetCurrentUserId(principal), cancellationToken);
        return session is null ? Results.NotFound() : Results.Ok(session);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("SetSessionSubstituteInstructor");

groups.MapPut("/{id:guid}/sessions/{sessionId:guid}/links", async (
    Guid id,
    Guid sessionId,
    UpdateSessionLinksDto dto,
    ClaimsPrincipal principal,
    IGroupService groupService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var session = await groupService.SetSessionLinksAsync(id, sessionId, dto, GetCurrentUserId(principal), cancellationToken);
        return session is null ? Results.NotFound() : Results.Ok(session);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("SetSessionLinks");

groups.MapPut("/{id:guid}/sessions/{sessionId:guid}/status", async (
    Guid id,
    Guid sessionId,
    SetSessionStatusDto dto,
    ClaimsPrincipal principal,
    IGroupService groupService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var session = await groupService.SetSessionStatusAsync(id, sessionId, dto, GetCurrentUserId(principal), cancellationToken);
        return session is null ? Results.NotFound() : Results.Ok(session);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("SetSessionStatus");

groups.MapGet("/session-statuses", (IGroupService groupService) =>
    Results.Ok(groupService.GetSessionStatusOptions()))
.WithName("GetSessionStatusOptions");

groups.MapGet("/{id:guid}/sessions/history", async (Guid id, IGroupService groupService, CancellationToken cancellationToken) =>
{
    var history = await groupService.GetSessionHistoryAsync(id, cancellationToken);
    return history is null ? Results.NotFound() : Results.Ok(history);
})
.WithName("GetGroupSessionHistory");

groups.MapGet("/{id:guid}/attendance", async (Guid id, IGroupService groupService, CancellationToken cancellationToken) =>
{
    var summary = await groupService.GetAttendanceSummaryAsync(id, cancellationToken);
    return summary is null ? Results.NotFound() : Results.Ok(summary);
})
.WithName("GetGroupAttendanceSummary");

groups.MapGet("/{id:guid}/attendance/export", async (
    Guid id,
    string? format,
    IGroupService groupService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var export = await groupService.ExportAttendanceSummaryAsync(id, format, cancellationToken);
        return export is null ? Results.NotFound() : Results.File(export.Content, export.ContentType, export.FileName);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("ExportGroupAttendanceSummary");

groups.MapGet("/{id:guid}/sessions/{sessionId:guid}/attendance/export", async (
    Guid id,
    Guid sessionId,
    string? format,
    IGroupService groupService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var export = await groupService.ExportSessionAttendanceAsync(id, sessionId, format, cancellationToken);
        return export is null ? Results.NotFound() : Results.File(export.Content, export.ContentType, export.FileName);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("ExportSessionAttendance");

// --- Kursy / programy jako szablony grup ---
var courses = app.MapGroup("/api/courses").WithTags("Courses").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

courses.MapGet("/", async (ICourseService courseService, CancellationToken cancellationToken) =>
    Results.Ok(await courseService.GetSummariesAsync(cancellationToken)))
.WithName("GetCourses");

courses.MapGet("/{id:guid}", async (Guid id, ICourseService courseService, CancellationToken cancellationToken) =>
{
    var course = await courseService.GetDetailsAsync(id, cancellationToken);
    return course is null ? Results.NotFound() : Results.Ok(course);
})
.WithName("GetCourseById");

courses.MapPost("/", async (UpsertCourseDto dto, ICourseService courseService, CancellationToken cancellationToken) =>
{
    try
    {
        var course = await courseService.CreateAsync(dto, cancellationToken);
        return Results.Created($"/api/courses/{course.Id}", course);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateCourse");

courses.MapPut("/{id:guid}", async (Guid id, UpsertCourseDto dto, ICourseService courseService, CancellationToken cancellationToken) =>
{
    try
    {
        var course = await courseService.UpdateAsync(id, dto, cancellationToken);
        return course is null ? Results.NotFound() : Results.Ok(course);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdateCourse");

courses.MapDelete("/{id:guid}", async (Guid id, ICourseService courseService, CancellationToken cancellationToken) =>
    await courseService.DeleteAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteCourse");

// --- Uczestnicy (centralna baza, niezależna od pojedynczej grupy) ---
var participants = app.MapGroup("/api/participants").WithTags("Participants").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

participants.MapGet("/", async (string? q, bool? includeArchived, IParticipantService participantService, CancellationToken cancellationToken) =>
    Results.Ok(await participantService.GetSummariesAsync(q, includeArchived ?? false, cancellationToken)))
.WithName("GetParticipants");

participants.MapGet("/{id:guid}", async (Guid id, IParticipantService participantService, CancellationToken cancellationToken) =>
{
    var participant = await participantService.GetDetailsAsync(id, cancellationToken);
    return participant is null ? Results.NotFound() : Results.Ok(participant);
})
.WithName("GetParticipantById");

participants.MapPost("/", async (CreateParticipantDto dto, IParticipantService participantService, CancellationToken cancellationToken) =>
{
    try
    {
        var participant = await participantService.CreateAsync(dto, cancellationToken);
        return Results.Created($"/api/participants/{participant.Id}", participant);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateParticipant");

participants.MapPut("/{id:guid}", async (Guid id, UpdateParticipantDto dto, IParticipantService participantService, CancellationToken cancellationToken) =>
{
    try
    {
        var participant = await participantService.UpdateAsync(id, dto, cancellationToken);
        return participant is null ? Results.NotFound() : Results.Ok(participant);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdateParticipant");

participants.MapDelete("/{id:guid}", async (Guid id, IParticipantService participantService, CancellationToken cancellationToken) =>
{
    try
    {
        return await participantService.DeleteAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound();
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
})
.WithName("DeleteParticipant");

participants.MapPost("/{id:guid}/archive", async (Guid id, IParticipantService participantService, CancellationToken cancellationToken) =>
    await participantService.SetArchivedAsync(id, true, cancellationToken) ? Results.NoContent() : Results.NotFound())
.WithName("ArchiveParticipant");

participants.MapPost("/{id:guid}/restore", async (Guid id, IParticipantService participantService, CancellationToken cancellationToken) =>
    await participantService.SetArchivedAsync(id, false, cancellationToken) ? Results.NoContent() : Results.NotFound())
.WithName("RestoreParticipant");

participants.MapPost("/{id:guid}/anonymize", async (Guid id, IParticipantService participantService, CancellationToken cancellationToken) =>
    await participantService.AnonymizeAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound())
.WithName("AnonymizeParticipant");

participants.MapPut("/{id:guid}/groups/{groupId:guid}", async (
    Guid id,
    Guid groupId,
    IParticipantService participantService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var participant = await participantService.EnrollAsync(id, groupId, cancellationToken);
        return participant is null ? Results.NotFound() : Results.Ok(participant);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("EnrollParticipant");

participants.MapDelete("/{id:guid}/groups/{groupId:guid}", async (
    Guid id,
    Guid groupId,
    IParticipantService participantService,
    CancellationToken cancellationToken) =>
{
    var participant = await participantService.UnenrollAsync(id, groupId, cancellationToken);
    return participant is null ? Results.NotFound() : Results.Ok(participant);
})
.WithName("UnenrollParticipant");

// --- Konta użytkowników (administracja) ---
var users = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

users.MapGet("/", async (IUserAdminService userAdminService, CancellationToken cancellationToken) =>
    Results.Ok(await userAdminService.ListAsync(cancellationToken)))
.WithName("GetUsers");

users.MapGet("/instructors", async (IGroupService groupService, CancellationToken cancellationToken) =>
    Results.Ok(await groupService.GetInstructorsAsync(cancellationToken)))
.WithName("GetInstructors");

users.MapPost("/", async (
    CreateUserDto dto,
    ClaimsPrincipal principal,
    IUserAdminService userAdminService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var user = await userAdminService.CreateAsync(dto, cancellationToken);
        await auditService.RecordAsync(
            GetCurrentUserId(principal),
            "user.create",
            "users",
            user.Id.ToString(),
            true,
            user.Email,
            cancellationToken);
        return Results.Created($"/api/users/{user.Id}", user);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
})
.SelfAudited()
.WithName("CreateUser");

users.MapPut("/{id:guid}/profile", async (
    Guid id,
    UpdateUserProfileDto dto,
    ClaimsPrincipal principal,
    IUserAdminService userAdminService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    var updated = await userAdminService.UpdateProfileAsync(id, dto, cancellationToken);

    if (updated)
    {
        await auditService.RecordAsync(GetCurrentUserId(principal), "user.profile.update", "users", id.ToString(), true, null, cancellationToken);
    }

    return updated ? Results.NoContent() : Results.NotFound();
})
.SelfAudited()
.WithName("UpdateUserProfile");

users.MapPost("/{id:guid}/activate", async (
    Guid id,
    ClaimsPrincipal principal,
    IUserAdminService userAdminService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    var updated = await userAdminService.SetActiveAsync(id, true, GetCurrentUserId(principal), cancellationToken);

    if (updated)
    {
        await auditService.RecordAsync(GetCurrentUserId(principal), "user.activate", "users", id.ToString(), true, null, cancellationToken);
    }

    return updated ? Results.NoContent() : Results.NotFound();
})
.SelfAudited()
.WithName("ActivateUser");

users.MapPost("/{id:guid}/deactivate", async (
    Guid id,
    ClaimsPrincipal principal,
    IUserAdminService userAdminService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var updated = await userAdminService.SetActiveAsync(id, false, GetCurrentUserId(principal), cancellationToken);

        if (updated)
        {
            await auditService.RecordAsync(GetCurrentUserId(principal), "user.deactivate", "users", id.ToString(), true, null, cancellationToken);
        }

        return updated ? Results.NoContent() : Results.NotFound();
    }
    catch (InvalidOperationException ex)
    {
        await auditService.RecordAsync(GetCurrentUserId(principal), "user.deactivate", "users", id.ToString(), false, ex.Message, cancellationToken);
        return Results.Conflict(new { error = ex.Message });
    }
})
.SelfAudited()
.WithName("DeactivateUser");

users.MapPost("/{id:guid}/password", async (
    Guid id,
    SetPasswordDto dto,
    ClaimsPrincipal principal,
    IUserAdminService userAdminService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var updated = await userAdminService.SetPasswordAsync(id, dto.Password, cancellationToken);

        if (updated)
        {
            await auditService.RecordAsync(GetCurrentUserId(principal), "user.password.set", "users", id.ToString(), true, null, cancellationToken);
        }

        return updated ? Results.NoContent() : Results.NotFound();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.SelfAudited()
.WithName("SetUserPassword");

// Zaproszenie zamiast hasła wymyślonego przez admina: konto dostaje jednorazowy link
// i ustawia hasło samo. Przy trzydziestu rodzicach to różnica między etatem a jednym kliknięciem.
users.MapPost("/{id:guid}/invite", async (
    Guid id,
    ClaimsPrincipal principal,
    IAccountTokenService accountTokenService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var result = await accountTokenService.SendInvitationAsync(id, GetCurrentUserId(principal), cancellationToken);

        if (result is null)
        {
            return Results.NotFound();
        }

        await auditService.RecordAsync(
            GetCurrentUserId(principal),
            "user.invite",
            "users",
            id.ToString(),
            result.Sent,
            result.Error,
            cancellationToken);

        // Token powstał nawet wtedy, gdy poczta nie wyszła - admin musi zobaczyć różnicę,
        // bo inaczej czekałby na rodzica, który nic nie dostał.
        return result.Sent
            ? Results.Ok(result)
            : Results.Json(result, statusCode: StatusCodes.Status502BadGateway);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
})
.SelfAudited()
.WithName("InviteUser");

var audit = app.MapGroup("/api/audit").WithTags("Audit").RequireAuthorization("AdminOnly");

audit.MapGet("/", async (
    int? limit,
    string? entityType,
    string? action,
    bool? success,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
    Results.Ok(await auditService.SearchAsync(
        new AuditLogQueryDto(limit ?? 100, entityType, action, success),
        cancellationToken)))
.WithName("GetAuditLogs");

var dashboard = app.MapGroup("/api/dashboard").WithTags("Dashboard").RequireAuthorization("AdminOnly");

dashboard.MapGet("/", async (IDashboardService dashboardService, CancellationToken cancellationToken) =>
    Results.Ok(await dashboardService.GetAsync(cancellationToken)))
.WithName("GetDashboard");

// --- Grafik instruktora (właściciel sprawdzany w serwisie) ---
var billing = app.MapGroup("/api/billing").WithTags("Billing").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

billing.MapGet("/", async (IBillingService billingService, CancellationToken cancellationToken) =>
    Results.Ok(await billingService.GetOverviewAsync(cancellationToken)))
.WithName("GetBillingOverview");

billing.MapPost("/price-plans", async (
    UpsertPricePlanDto dto,
    IBillingService billingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var pricePlan = await billingService.CreatePricePlanAsync(dto, cancellationToken);
        return Results.Created($"/api/billing/price-plans/{pricePlan.Id}", pricePlan);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreatePricePlan");

billing.MapPut("/price-plans/{id:guid}", async (
    Guid id,
    UpsertPricePlanDto dto,
    IBillingService billingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var pricePlan = await billingService.UpdatePricePlanAsync(id, dto, cancellationToken);
        return pricePlan is null ? Results.NotFound() : Results.Ok(pricePlan);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdatePricePlan");

billing.MapDelete("/price-plans/{id:guid}", async (Guid id, IBillingService billingService, CancellationToken cancellationToken) =>
    await billingService.DeletePricePlanAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound())
.WithName("DeletePricePlan");

billing.MapPost("/enrollments", async (
    CreateBillingEnrollmentDto dto,
    IBillingService billingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var enrollment = await billingService.CreateEnrollmentAsync(dto, cancellationToken);
        return Results.Created($"/api/billing/enrollments/{enrollment.Id}", enrollment);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateBillingEnrollment");

billing.MapGet("/credits", async (Guid? participantId, IBillingService billingService, CancellationToken cancellationToken) =>
    Results.Ok(await billingService.ListCreditsAsync(participantId, cancellationToken)))
.WithName("GetLessonCredits");

billing.MapPost("/credits", async (
    IssueLessonCreditDto dto,
    ClaimsPrincipal principal,
    IBillingService billingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var credit = await billingService.IssueCreditAsync(dto, GetCurrentUserId(principal), cancellationToken);
        return Results.Created($"/api/billing/credits/{credit.Id}", credit);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("IssueLessonCredit");

billing.MapPost("/credits/{id:guid}/use", async (
    Guid id,
    UseLessonCreditDto dto,
    ClaimsPrincipal principal,
    IBillingService billingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var credit = await billingService.UseCreditAsync(id, dto, GetCurrentUserId(principal), cancellationToken);
        return credit is null ? Results.NotFound() : Results.Ok(credit);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UseLessonCredit");

billing.MapPost("/credits/{id:guid}/revoke", async (
    Guid id,
    RevokeLessonCreditDto? dto,
    ClaimsPrincipal principal,
    IBillingService billingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var credit = await billingService.RevokeCreditAsync(id, dto?.Reason, GetCurrentUserId(principal), cancellationToken);
        return credit is null ? Results.NotFound() : Results.Ok(credit);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("RevokeLessonCredit");

billing.MapPost("/invoices", async (
    CreateInvoiceDto dto,
    IBillingService billingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var invoice = await billingService.CreateInvoiceAsync(dto, cancellationToken);
        return Results.Created($"/api/billing/invoices/{invoice.Id}", invoice);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateInvoice");

billing.MapPost("/invoices/{id:guid}/pay", async (
    Guid id,
    RecordManualPaymentDto dto,
    IBillingService billingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var invoice = await billingService.MarkInvoicePaidAsync(id, dto, cancellationToken);
        return invoice is null ? Results.NotFound() : Results.Ok(invoice);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("PayInvoice");

billing.MapPost("/invoices/{id:guid}/cancel", async (
    Guid id,
    IBillingService billingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var invoice = await billingService.CancelInvoiceAsync(id, cancellationToken);
        return invoice is null ? Results.NotFound() : Results.Ok(invoice);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CancelInvoice");

var parentAdmin = app.MapGroup("/api/parent-links").WithTags("ParentLinks").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

parentAdmin.MapGet("/", async (IParentPortalService parentPortalService, CancellationToken cancellationToken) =>
    Results.Ok(await parentPortalService.ListLinksAsync(cancellationToken)))
.WithName("GetParentLinks");

parentAdmin.MapPost("/", async (
    ParentParticipantLinkDto dto,
    IParentPortalService parentPortalService,
    CancellationToken cancellationToken) =>
{
    try
    {
        return Results.Ok(await parentPortalService.LinkAsync(
            dto.ParentUserId,
            dto.ParticipantId,
            dto.Relation,
            dto.IsPrimaryContact,
            dto.ReceivesNotifications,
            cancellationToken));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateParentLink");

parentAdmin.MapDelete("/{parentUserId:guid}/{participantId:guid}", async (
    Guid parentUserId,
    Guid participantId,
    IParentPortalService parentPortalService,
    CancellationToken cancellationToken) =>
    await parentPortalService.UnlinkAsync(parentUserId, participantId, cancellationToken) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteParentLink");

var parentPortal = app.MapGroup("/api/parent").WithTags("ParentPortal").RequireAuthorization("ParentOnly")
    .AddEndpointFilter<AuditEndpointFilter>();

parentPortal.MapGet("/portal", async (
    ClaimsPrincipal principal,
    IParentPortalService parentPortalService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);
    return userId is null
        ? Results.Unauthorized()
        : Results.Ok(await parentPortalService.GetPortalAsync(userId.Value, cancellationToken));
})
.WithName("GetParentPortal");

parentPortal.MapGet("/export.ics", async (
    ClaimsPrincipal principal,
    IParentPortalService parentPortalService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);
    return userId is null
        ? Results.Unauthorized()
        : Results.File(
            await parentPortalService.ExportScheduleIcsAsync(userId.Value, cancellationToken),
            "text/calendar; charset=utf-8",
            "zajecia-dziecka.ics");
})
.WithName("ExportParentScheduleIcs");

parentPortal.MapPost("/sessions/{sessionId:guid}/absence", async (
    Guid sessionId,
    ReportAbsenceDto dto,
    ClaimsPrincipal principal,
    IParentPortalService parentPortalService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var reported = await parentPortalService.ReportAbsenceAsync(userId.Value, sessionId, dto, cancellationToken);

    // Świadomie 404 zamiast 403: rodzic nie ma prawa wiedzieć, czy termin cudzego dziecka istnieje.
    return reported
        ? Results.NoContent()
        : Results.NotFound(new { error = "Nie można zgłosić nieobecności na ten termin." });
})
.WithName("ReportParentAbsence");

parentPortal.MapPut("/consents", async (
    UpdateParentConsentDto dto,
    ClaimsPrincipal principal,
    IParentPortalService parentPortalService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var updated = await parentPortalService.UpdateConsentAsync(userId.Value, dto, cancellationToken);

    // Ta sama zasada co przy nieobecności: 404, a nie 403 - rodzic nie ma prawa wiedzieć,
    // czy cudze dziecko w ogóle istnieje w systemie.
    return updated
        ? Results.NoContent()
        : Results.NotFound(new { error = "Nie można zmienić zgody dla tego dziecka." });
})
.WithName("UpdateParentConsent");

// Wyszukiwanie globalne. Grupa jest tylko do odczytu, więc bez filtra audytu
// (odczytów świadomie nie logujemy - zaśmiecałyby dziennik, nie zmieniając stanu).
// Rola „Parent" nie ma tu wstępu: wyszukiwarka po dzieciach i grupach byłaby
// najprostszą drogą do listy cudzych dzieci.
// --- Incydenty i zgłoszenia techniczne (rozdziały 3 i 8 dokumentu koncepcyjnego) ---
//
// `StaffOnly`, a nie `AdminOnly`: zgłosić incydent musi móc osoba, która go widziała, czyli
// instruktor. Prowadzenie sprawy jest już wyłącznie po stronie administracji i to sprawdzamy
// na poszczególnych trasach. Rola `Parent` nie ma tu wstępu w żadnej formie — rejestr
// incydentów nie jest dokumentem dla rodzica.
var safety = app.MapGroup("/api/safety").WithTags("Safety").RequireAuthorization("StaffOnly")
    .AddEndpointFilter<AuditEndpointFilter>();

safety.MapGet("/incidents", async (
    ClaimsPrincipal principal,
    ISafetyService safetyService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var isAdmin = principal.IsInRole(nameof(LessonRunner.Domain.Users.UserRole.Admin));
    return Results.Ok(await safetyService.GetIncidentsAsync(userId.Value, isAdmin, cancellationToken));
})
.WithName("GetIncidents");

safety.MapPost("/incidents", async (
    CreateIncidentDto dto,
    ClaimsPrincipal principal,
    ISafetyService safetyService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    try
    {
        return Results.Ok(await safetyService.ReportIncidentAsync(userId.Value, dto, cancellationToken));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("ReportIncident");

// Prowadzenie sprawy - wyłącznie administracja. Instruktor zgłasza i widzi własne zgłoszenie,
// ale nie zmienia jego statusu ani nie przypisuje odpowiedzialnych.
safety.MapPut("/incidents/{id:guid}", async (
    Guid id,
    UpdateIncidentDto dto,
    ISafetyService safetyService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var updated = await safetyService.UpdateIncidentAsync(id, dto, cancellationToken);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.RequireAuthorization("AdminOnly")
.WithName("UpdateIncident");

safety.MapGet("/tickets", async (
    Guid? participantId,
    ISafetyService safetyService,
    CancellationToken cancellationToken) =>
    Results.Ok(await safetyService.GetSupportTicketsAsync(participantId, cancellationToken)))
.WithName("GetSupportTickets");

safety.MapPost("/tickets", async (
    CreateSupportTicketDto dto,
    ClaimsPrincipal principal,
    ISafetyService safetyService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    try
    {
        return Results.Ok(await safetyService.ReportSupportTicketAsync(userId.Value, dto, cancellationToken));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("ReportSupportTicket");

// Zgłoszenia techniczne prowadzi też instruktor: to on najczęściej wie, co pomogło,
// i to jego następne zajęcia zależą od tego, czy problem został opisany.
safety.MapPut("/tickets/{id:guid}", async (
    Guid id,
    UpdateSupportTicketDto dto,
    ISafetyService safetyService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var updated = await safetyService.UpdateSupportTicketAsync(id, dto, cancellationToken);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdateSupportTicket");

var searchGroup = app.MapGroup("/api/search").WithTags("Search").RequireAuthorization("StaffOnly");

searchGroup.MapGet("/", async (
    string? q,
    ClaimsPrincipal principal,
    ISearchService searchService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var isAdmin = principal.IsInRole(nameof(LessonRunner.Domain.Users.UserRole.Admin));
    return Results.Ok(await searchService.SearchAsync(userId.Value, isAdmin, q ?? "", cancellationToken));
})
.WithName("Search");

var schedule = app.MapGroup("/api/schedule").WithTags("Schedule").RequireAuthorization("StaffOnly").AddEndpointFilter<AuditEndpointFilter>();

schedule.MapGet("/", async (ClaimsPrincipal principal, ISessionService sessionService, CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);
    return userId is null
        ? Results.Unauthorized()
        : Results.Ok(await sessionService.GetScheduleAsync(userId.Value, cancellationToken));
})
.WithName("GetSchedule");

schedule.MapGet("/export.ics", async (ClaimsPrincipal principal, ISessionService sessionService, CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);
    return userId is null
        ? Results.Unauthorized()
        : Results.File(
            await sessionService.ExportScheduleIcsAsync(userId.Value, cancellationToken),
            "text/calendar; charset=utf-8",
            "grafik-zajec.ics");
})
.WithName("ExportInstructorScheduleIcs");

schedule.MapGet("/{sessionId:guid}", async (
    Guid sessionId,
    ClaimsPrincipal principal,
    ISessionService sessionService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var session = await sessionService.GetSessionAsync(sessionId, userId.Value, cancellationToken);
    return session is null ? Results.NotFound() : Results.Ok(session);
})
.WithName("GetScheduledSession");

schedule.MapPost("/{sessionId:guid}/start", async (
    Guid sessionId,
    ClaimsPrincipal principal,
    ISessionService sessionService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    try
    {
        var attendance = await sessionService.StartAsync(sessionId, userId.Value, cancellationToken);
        return attendance is null ? Results.NotFound() : Results.Ok(attendance);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("StartSession");

schedule.MapGet("/{sessionId:guid}/attendance", async (
    Guid sessionId,
    ClaimsPrincipal principal,
    ISessionService sessionService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var attendance = await sessionService.GetAttendanceAsync(sessionId, userId.Value, cancellationToken);
    return attendance is null ? Results.NotFound() : Results.Ok(attendance);
})
.WithName("GetAttendance");

schedule.MapPut("/{sessionId:guid}/attendance", async (
    Guid sessionId,
    SaveAttendanceDto dto,
    ClaimsPrincipal principal,
    ISessionService sessionService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    try
    {
        var attendance = await sessionService.SaveAttendanceAsync(sessionId, userId.Value, dto, cancellationToken);
        return attendance is null ? Results.NotFound() : Results.Ok(attendance);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("SaveAttendance");

// Znacznik pracy na żywo: wąska operacja na jednym dziecku. W trakcie zajęć instruktor
// klika to co kilkadziesiąt sekund, więc przepychanie przy tym całej listy obecności
// groziłoby nadpisaniem świeżej zmiany danymi sprzed chwili.
schedule.MapPut("/{sessionId:guid}/live-status", async (
    Guid sessionId,
    SetLiveStatusDto dto,
    ClaimsPrincipal principal,
    ISessionService sessionService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    try
    {
        var attendance = await sessionService.SetLiveStatusAsync(sessionId, userId.Value, dto, cancellationToken);
        return attendance is null ? Results.NotFound() : Results.Ok(attendance);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("SetLiveStatus");

schedule.MapPost("/{sessionId:guid}/finish", async (
    Guid sessionId,
    FinishSessionDto dto,
    ClaimsPrincipal principal,
    ISessionService sessionService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    try
    {
        var session = await sessionService.FinishAsync(sessionId, userId.Value, dto, cancellationToken);
        return session is null ? Results.NotFound() : Results.Ok(session);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("FinishSession");

// --- Planowanie: lokalizacje, dni wolne i kalendarz ---
// --- Postępy i projekty dzieci ---
// `StaffOnly`: zapisuje instruktor prowadzący, czyta też admin. Rodzic ma to w swoim portalu
// w wersji przyciętej do tego, co napisano z myślą o nim.
var progress = app.MapGroup("/api/progress").WithTags("Progress").RequireAuthorization("StaffOnly").AddEndpointFilter<AuditEndpointFilter>();

progress.MapGet("/sessions/{sessionId:guid}", async (
    Guid sessionId,
    ClaimsPrincipal principal,
    IProgressService progressService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var result = await progressService.GetSessionProgressAsync(sessionId, userId.Value, cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
})
.WithName("GetSessionProgress");

progress.MapPut("/sessions/{sessionId:guid}", async (
    Guid sessionId,
    SaveSessionProgressDto dto,
    ClaimsPrincipal principal,
    IProgressService progressService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var result = await progressService.SaveSessionProgressAsync(sessionId, userId.Value, dto, cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
})
.WithName("SaveSessionProgress");

progress.MapGet("/participants/{participantId:guid}", async (
    Guid participantId,
    ClaimsPrincipal principal,
    IProgressService progressService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var result = await progressService.GetParticipantProgressAsync(
        participantId, userId.Value, IsAdmin(principal), cancellationToken);

    // 404, nie 403: instruktor spoza grupy nie ma prawa wiedzieć, czy takie dziecko istnieje.
    return result is null ? Results.NotFound() : Results.Ok(result);
})
.WithName("GetParticipantProgress");

progress.MapPost("/projects", async (
    CreateProjectDto dto,
    ClaimsPrincipal principal,
    IProgressService progressService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    try
    {
        var project = await progressService.CreateProjectAsync(dto, userId.Value, IsAdmin(principal), cancellationToken);
        return project is null
            ? Results.NotFound()
            : Results.Created($"/api/progress/projects/{project.Id}", project);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateProject");

progress.MapPost("/projects/{projectId:guid}/submissions", async (
    Guid projectId,
    AddSubmissionDto dto,
    ClaimsPrincipal principal,
    IProgressService progressService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    try
    {
        var project = await progressService.AddSubmissionAsync(projectId, dto, userId.Value, IsAdmin(principal), cancellationToken);
        return project is null ? Results.NotFound() : Results.Ok(project);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("AddProjectSubmission");

progress.MapPut("/projects/{projectId:guid}/submissions/{submissionId:guid}/comment", async (
    Guid projectId,
    Guid submissionId,
    SetSubmissionCommentDto dto,
    ClaimsPrincipal principal,
    IProgressService progressService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var updated = await progressService.SetSubmissionCommentAsync(
        projectId, submissionId, dto.Comment, userId.Value, IsAdmin(principal), cancellationToken);

    return updated ? Results.NoContent() : Results.NotFound();
})
.WithName("SetProjectSubmissionComment");

var locations = app.MapGroup("/api/locations").WithTags("Locations").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

locations.MapGet("/", async (ISchedulingService schedulingService, CancellationToken cancellationToken) =>
    Results.Ok(await schedulingService.GetLocationsAsync(cancellationToken)))
.WithName("GetLocations");

locations.MapPost("/", async (UpsertLocationDto dto, ISchedulingService schedulingService, CancellationToken cancellationToken) =>
{
    try
    {
        var location = await schedulingService.CreateLocationAsync(dto, cancellationToken);
        return Results.Created($"/api/locations/{location.Id}", location);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateLocation");

locations.MapPut("/{id:guid}", async (
    Guid id,
    UpsertLocationDto dto,
    ISchedulingService schedulingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var location = await schedulingService.UpdateLocationAsync(id, dto, cancellationToken);
        return location is null ? Results.NotFound() : Results.Ok(location);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdateLocation");

locations.MapDelete("/{id:guid}", async (Guid id, ISchedulingService schedulingService, CancellationToken cancellationToken) =>
    await schedulingService.DeleteLocationAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteLocation");

var holidays = app.MapGroup("/api/holidays").WithTags("Holidays").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

holidays.MapGet("/", async (ISchedulingService schedulingService, CancellationToken cancellationToken) =>
    Results.Ok(await schedulingService.GetHolidaysAsync(cancellationToken)))
.WithName("GetHolidays");

holidays.MapPost("/", async (UpsertHolidayDto dto, ISchedulingService schedulingService, CancellationToken cancellationToken) =>
{
    try
    {
        var holiday = await schedulingService.CreateHolidayAsync(dto, cancellationToken);
        return Results.Created($"/api/holidays/{holiday.Id}", holiday);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateHoliday");

holidays.MapPut("/{id:guid}", async (
    Guid id,
    UpsertHolidayDto dto,
    ISchedulingService schedulingService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var holiday = await schedulingService.UpdateHolidayAsync(id, dto, cancellationToken);
        return holiday is null ? Results.NotFound() : Results.Ok(holiday);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdateHoliday");

holidays.MapDelete("/{id:guid}", async (Guid id, ISchedulingService schedulingService, CancellationToken cancellationToken) =>
    await schedulingService.DeleteHolidayAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteHoliday");

var calendar = app.MapGroup("/api/calendar").WithTags("Calendar").RequireAuthorization("StaffOnly");

calendar.MapGet("/", async (
    DateTimeOffset? from,
    DateTimeOffset? to,
    ClaimsPrincipal principal,
    ISchedulingService schedulingService,
    CancellationToken cancellationToken) =>
{
    var userId = principal.IsInRole(nameof(LessonRunner.Domain.Users.UserRole.Admin))
        ? null
        : GetCurrentUserId(principal);

    if (!principal.IsInRole(nameof(LessonRunner.Domain.Users.UserRole.Admin)) && userId is null)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(await schedulingService.GetCalendarAsync(from, to, userId, cancellationToken));
})
.WithName("GetCalendar");

var operations = app.MapGroup("/api/operations").WithTags("Operations").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

operations.MapPost("/backup", async (
    ClaimsPrincipal principal,
    IBackupService backupService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    var backup = await backupService.CreateAsync(cancellationToken);
    await auditService.RecordAsync(
        GetCurrentUserId(principal),
        "backup.create",
        "operations",
        backup.FileName,
        true,
        $"{backup.SizeBytes} bytes",
        cancellationToken);
    return Results.Ok(backup);
})
.SelfAudited()
.WithName("CreateBackup");

operations.MapGet("/backups", async (IBackupService backupService, CancellationToken cancellationToken) =>
    Results.Ok(await backupService.ListAsync(cancellationToken)))
.WithName("ListBackups");

var notifications = app.MapGroup("/api/notifications").WithTags("Notifications").RequireAuthorization("AdminOnly").AddEndpointFilter<AuditEndpointFilter>();

notifications.MapGet("/settings", async (INotificationService notificationService, CancellationToken cancellationToken) =>
    Results.Ok(await notificationService.GetSettingsAsync(cancellationToken)))
.WithName("GetNotificationSettings");

notifications.MapPut("/settings", async (
    NotificationSettingsDto dto,
    INotificationService notificationService,
    CancellationToken cancellationToken) =>
{
    try
    {
        return Results.Ok(await notificationService.UpdateSettingsAsync(dto, cancellationToken));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdateNotificationSettings");

notifications.MapGet("/logs", async (int? limit, INotificationService notificationService, CancellationToken cancellationToken) =>
    Results.Ok(await notificationService.ListLogsAsync(limit ?? 100, cancellationToken)))
.WithName("GetNotificationLogs");

notifications.MapGet("/preview/{type}", async (string type, INotificationService notificationService, CancellationToken cancellationToken) =>
    Results.Ok(await notificationService.PreviewAsync(type, cancellationToken)))
.WithName("PreviewNotification");

notifications.MapPost("/send-reminders", async (INotificationService notificationService, CancellationToken cancellationToken) =>
{
    await notificationService.SendUpcomingSessionRemindersAsync(cancellationToken);
    return Results.Accepted();
})
.WithName("SendNotificationReminders");

// Przypomnienia o zaległych płatnościach. Świadomie bez automatu i bez przełącznika
// w ustawieniach: upominanie się o pieniądze to decyzja biznesowa i wizerunkowa, którą
// administrator podejmuje klikając przycisk - i od razu widzi, ile wiadomości wyszło.
notifications.MapPost("/send-payment-reminders", async (
    INotificationService notificationService,
    CancellationToken cancellationToken) =>
{
    var sent = await notificationService.SendPaymentRemindersAsync(cancellationToken);
    return Results.Ok(new { sent });
})
.WithName("SendPaymentReminders");

app.MapPost("/api/files", async (IFormFile file, IFileStorage storage, CancellationToken cancellationToken) =>
{
    if (file.Length == 0)
    {
        return Results.BadRequest(new { error = "Plik jest pusty." });
    }

    if (file.Length > FileUploadRules.MaxSizeBytes)
    {
        return Results.BadRequest(new { error = $"Plik przekracza limit {FileUploadRules.MaxSizeBytes / (1024 * 1024)} MB." });
    }

    if (!FileUploadRules.IsAllowedUpload(file.ContentType, file.FileName))
    {
        return Results.BadRequest(new { error = $"Niedozwolony typ pliku. Dozwolone: {FileUploadRules.AllowedSummary()}." });
    }

    await using var stream = file.OpenReadStream();
    var stored = await storage.SaveAsync(stream, file.FileName, file.ContentType, cancellationToken);

    return Results.Ok(stored);
})
.RequireAuthorization("AdminOnly")
.DisableAntiforgery()
.WithTags("Files")
.WithName("UploadFile");

// Limit nakładamy punktowo na endpointy wrażliwe na zgadywanie hasła. Świadomie NIE obejmujemy
// nim `GET /me`, bo ten leci przy każdym otwarciu aplikacji i kilku pracowników za jednym NAT-em
// wyczerpałoby pulę.
var auth = app.MapGroup("/api/auth").WithTags("Auth");

auth.MapPost("/login", async (LoginDto dto, IAuthService authService, IAuditService auditService, CancellationToken cancellationToken) =>
{
    var result = await authService.AuthenticateAsync(dto, cancellationToken);
    await auditService.RecordAsync(
        result?.User.Id,
        "auth.login",
        "auth",
        result?.User.Id.ToString(),
        result is not null,
        string.IsNullOrWhiteSpace(dto.Email) ? null : UserSafe(dto.Email),
        cancellationToken);
    return result is null
        ? Results.Unauthorized()
        : Results.Ok(result);
})
.RequireRateLimiting("auth")
.WithName("Login");

// --- Reset hasła: trasy publiczne, bez uwierzytelnienia ---
// Wszystkie trzy pod limitem "auth", bo są jedynym miejscem, w którym niezalogowany
// wywołujący dotyka kont.

auth.MapPost("/password-reset", async (
    RequestPasswordResetDto dto,
    IAccountTokenService accountTokenService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    await accountTokenService.RequestPasswordResetAsync(dto, cancellationToken);
    await auditService.RecordAsync(
        null,
        "auth.password.reset.request",
        "auth",
        null,
        true,
        string.IsNullOrWhiteSpace(dto.Email) ? null : UserSafe(dto.Email),
        cancellationToken);

    // Zawsze 202, także dla adresu, którego nie ma w bazie. Odpowiedź zależna od istnienia
    // konta zamieniłaby ten formularz w sprawdzacz, kto korzysta ze szkoły.
    return Results.Accepted();
})
.RequireRateLimiting("auth")
.WithName("RequestPasswordReset");

auth.MapGet("/password-reset/{token}", async (
    string token,
    IAccountTokenService accountTokenService,
    CancellationToken cancellationToken) =>
{
    var info = await accountTokenService.DescribeAsync(token, cancellationToken);
    return info is null ? Results.NotFound() : Results.Ok(info);
})
.RequireRateLimiting("auth")
.WithName("DescribeAccountToken");

auth.MapPost("/password-reset/confirm", async (
    ConfirmPasswordResetDto dto,
    IAccountTokenService accountTokenService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var userId = await accountTokenService.ConfirmPasswordResetAsync(dto, cancellationToken);
        await auditService.RecordAsync(
            userId,
            "auth.password.reset.confirm",
            "auth",
            userId?.ToString(),
            userId is not null,
            null,
            cancellationToken);

        return userId is null
            ? Results.BadRequest(new { error = "Link stracił ważność lub został już użyty. Poproś o nowy." })
            : Results.NoContent();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.RequireRateLimiting("auth")
.WithName("ConfirmPasswordReset");

auth.MapGet("/me", (ClaimsPrincipal principal) =>
{
    var id = principal.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var email = principal.FindFirstValue(ClaimTypes.Email)
        ?? principal.FindFirstValue(JwtRegisteredClaimNames.Email);
    var role = principal.FindFirstValue(ClaimTypes.Role);
    var displayName = principal.FindFirstValue(ClaimTypes.Name);

    if (id is null || email is null || role is null)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new AuthUserDto(Guid.Parse(id), email, role.ToLowerInvariant(), string.IsNullOrWhiteSpace(displayName) ? email : displayName));
})
.RequireAuthorization()
.WithName("CurrentUser");

auth.MapPost("/change-password", async (
    ChangePasswordDto dto,
    ClaimsPrincipal principal,
    IAuthService authService,
    IAuditService auditService,
    CancellationToken cancellationToken) =>
{
    var userId = GetCurrentUserId(principal);

    if (userId is null)
    {
        return Results.Unauthorized();
    }

    try
    {
        var changed = await authService.ChangePasswordAsync(userId.Value, dto, cancellationToken);
        await auditService.RecordAsync(
            userId,
            "auth.password.change",
            "auth",
            userId.Value.ToString(),
            changed,
            null,
            cancellationToken);

        return changed
            ? Results.NoContent()
            : Results.BadRequest(new { error = "Obecne hasło jest nieprawidłowe." });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.RequireAuthorization()
.WithName("ChangeOwnPassword");

app.Run();

static Guid? GetCurrentUserId(ClaimsPrincipal principal)
{
    var id = principal.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

    return Guid.TryParse(id, out var userId) ? userId : null;
}

/// <summary>Czy wywołujący jest administratorem. Polityka `StaffOnly` przepuszcza admina
/// i instruktora, a część odczytów ma dla admina szerszy zakres.</summary>
static bool IsAdmin(ClaimsPrincipal principal) =>
    principal.IsInRole(nameof(LessonRunner.Domain.Users.UserRole.Admin));

static IEnumerable<LessonRunner.Domain.Lessons.LessonProjectFile> GetProjectFiles(LessonRunner.Domain.Lessons.Lesson lesson)
{
    if (lesson.ProjectFiles.Starter is not null)
    {
        yield return lesson.ProjectFiles.Starter;
    }

    if (lesson.ProjectFiles.Final is not null)
    {
        yield return lesson.ProjectFiles.Final;
    }
}

static bool IsDownloadToken(string token)
{
    return token.Length >= 24 && token.All(value => char.IsAsciiLetterOrDigit(value) || value is '-' or '_');
}

static string UserSafe(string value)
{
    var trimmed = value.Trim();
    return trimmed.Length <= 254 ? trimmed : trimmed[..254];
}

static string? ResolveStoredFilePath(string url, FileStorageOptions options)
{
    var requestPath = options.RequestPath.TrimEnd('/');

    if (!url.StartsWith($"{requestPath}/", StringComparison.OrdinalIgnoreCase))
    {
        return null;
    }

    var fileName = Path.GetFileName(url[requestPath.Length..].TrimStart('/'));

    if (string.IsNullOrWhiteSpace(fileName))
    {
        return null;
    }

    var root = Path.GetFullPath(options.RootPath);
    var path = Path.GetFullPath(Path.Combine(root, fileName));
    var requiredPrefix = root.EndsWith(Path.DirectorySeparatorChar)
        ? root
        : $"{root}{Path.DirectorySeparatorChar}";

    return path.StartsWith(requiredPrefix, StringComparison.OrdinalIgnoreCase) ? path : null;
}

public partial class Program;

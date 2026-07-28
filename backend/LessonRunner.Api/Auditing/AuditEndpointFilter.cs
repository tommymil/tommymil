using System.Security.Claims;
using LessonRunner.Application.Audit;

namespace LessonRunner.Api.Auditing;

/// <summary>
/// Znacznik dla endpointów, które zapisują własny, bogatszy wpis audytu
/// (np. logowanie z adresem e-mail, backup z rozmiarem pliku).
/// Filtr automatyczny je pomija, żeby nie dublować wpisów.
/// </summary>
internal sealed class SelfAuditedMetadata;

/// <summary>
/// Automatyczny dziennik zmian: każde żądanie modyfikujące dane trafia do `AuditLogs`.
///
/// Ręczne wołanie `IAuditService` w każdym endpoincie nie skaluje się — przy trzydziestu
/// endpointach ktoś zawsze zapomni, a brak wpisu wychodzi dopiero przy reklamacji.
/// Filtr działa odwrotnie: domyślnie audytuje wszystko, co zmienia stan, a wyjątki
/// trzeba zaznaczyć świadomie.
///
/// Świadomie NIE zapisujemy treści żądania — poszłyby tam hasła
/// (`POST /api/users/{id}/password`, `POST /api/auth/change-password`).
/// </summary>
internal sealed class AuditEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;
        var method = http.Request.Method;

        var readOnly = HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method);
        var endpoint = http.GetEndpoint();
        var selfAudited = endpoint?.Metadata.GetMetadata<SelfAuditedMetadata>() is not null;

        if (readOnly || selfAudited)
        {
            return await next(context);
        }

        object? result;
        var statusCode = StatusCodes.Status200OK;
        string? failureDetails = null;

        try
        {
            result = await next(context);
            statusCode = (result as IStatusCodeHttpResult)?.StatusCode ?? StatusCodes.Status200OK;
        }
        catch (Exception exception)
        {
            // Nieobsłużony wyjątek też jest zdarzeniem wartym zapisania - ale request
            // ma polecieć dalej tak, jakby filtru nie było.
            statusCode = StatusCodes.Status500InternalServerError;
            failureDetails = exception.GetType().Name;
            await RecordAsync(http, method, statusCode, failureDetails, endpoint);
            throw;
        }

        await RecordAsync(http, method, statusCode, failureDetails, endpoint);
        return result;
    }

    private static async Task RecordAsync(
        HttpContext http,
        string method,
        int statusCode,
        string? failureDetails,
        Endpoint? endpoint)
    {
        try
        {
            var auditService = http.RequestServices.GetService<IAuditService>();

            if (auditService is null)
            {
                return;
            }

            // Nazwa endpointu jest czytelniejsza niż wzorzec trasy: "CancelSession"
            // zamiast "POST /api/groups/{id}/sessions/{sessionId}/cancel".
            var action = endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName
                ?? $"{method} {http.Request.Path}";

            await auditService.RecordAsync(
                CurrentUserId(http.User),
                action,
                EntityTypeFromPath(http.Request.Path),
                EntityIdFromRoute(http),
                statusCode is >= 200 and < 400,
                failureDetails is null
                    ? $"{method} {http.Request.Path} → {statusCode}"
                    : $"{method} {http.Request.Path} → {statusCode} ({failureDetails})",
                http.RequestAborted);
        }
        catch
        {
            // Dziennik nie może wywrócić operacji, która się właśnie udała.
        }
    }

    private static Guid? CurrentUserId(ClaimsPrincipal principal)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub");
        return Guid.TryParse(id, out var userId) ? userId : null;
    }

    /// <summary>Drugi segment ścieżki: `/api/groups/...` → `groups`.</summary>
    private static string EntityTypeFromPath(PathString path)
    {
        var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];
        return segments.Length >= 2 ? segments[1] : "unknown";
    }

    /// <summary>Najbardziej szczegółowy identyfikator z trasy - id terminu bije id grupy.</summary>
    private static string? EntityIdFromRoute(HttpContext http)
    {
        foreach (var key in new[] { "sessionId", "participantId", "id" })
        {
            if (http.Request.RouteValues.TryGetValue(key, out var value) && value is not null)
            {
                return value.ToString();
            }
        }

        return null;
    }
}

internal static class AuditEndpointFilterExtensions
{
    /// <summary>Endpoint zapisuje własny wpis audytu - pomiń filtr automatyczny.</summary>
    public static TBuilder SelfAudited<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        builder.WithMetadata(new SelfAuditedMetadata());
        return builder;
    }
}

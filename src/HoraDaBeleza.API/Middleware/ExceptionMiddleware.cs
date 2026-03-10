using System.Net;
using System.Text.Json;
using HoraDaBeleza.Domain.Exceptions;

namespace HoraDaBeleza.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate            _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleAsync(context, ex);
        }
    }

    private static Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, message) = exception switch
        {
            NotFoundException     e => (HttpStatusCode.NotFound,            e.Message),
            UnauthorizedException e => (HttpStatusCode.Unauthorized,        e.Message),
            BusinessException     e => (HttpStatusCode.UnprocessableEntity, e.Message),
            DomainException       e => (HttpStatusCode.BadRequest,          e.Message),
            FluentValidation.ValidationException e
                => (HttpStatusCode.BadRequest, string.Join("; ", e.Errors.Select(x => x.ErrorMessage))),
            _   => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        var body = new
        {
            status  = (int)status,
            error   = message,
            path    = context.Request.Path.Value,
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)status;

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }
}

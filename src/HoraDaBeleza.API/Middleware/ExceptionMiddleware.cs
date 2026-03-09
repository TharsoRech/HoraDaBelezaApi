using System.Net;
using System.Text.Json;
using FluentValidation;
using HoraDaBeleza.Domain.Exceptions;

namespace HoraDaBeleza.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                "Erro de validação.",
                ve.Errors.Select(e => e.ErrorMessage).ToList()),

            NotFoundException nfe => (
                HttpStatusCode.NotFound,
                nfe.Message,
                (List<string>?)null),

            UnauthorizedException ue => (
                HttpStatusCode.Unauthorized,
                ue.Message,
                (List<string>?)null),

            BusinessException be => (
                HttpStatusCode.UnprocessableEntity,
                be.Message,
                (List<string>?)null),

            _ => (
                HttpStatusCode.InternalServerError,
                "Erro interno do servidor.",
                (List<string>?)null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var response = new
        {
            status  = (int)statusCode,
            message,
            errors
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

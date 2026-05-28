using FluentValidation;
using Serilog;
using System.Net;
using System.Text.Json;
using NexaCRM.Domain.Exceptions;

namespace NexaCRM.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
        => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .Select(e => new { field = e.PropertyName, message = e.ErrorMessage });

            var response = new
            {
                status = 400,
                error = "Validation failed",
                details = errors,
                traceId = context.TraceIdentifier
            };

            Log.Warning("Validation failed for {Path}: {@Errors}",
                context.Request.Path, errors);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 400,
                error = ex.Message,
                traceId = context.TraceIdentifier
            };

            Log.Warning("Domain exception at {Path}: {Message}",
                context.Request.Path, ex.Message);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 500,
                error = "An unexpected error occurred.",
                traceId = context.TraceIdentifier
            };

            Log.Error(ex, "Unhandled exception at {Path}", context.Request.Path);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
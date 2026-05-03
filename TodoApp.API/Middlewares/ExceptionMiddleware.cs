using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace TodoApp.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Determine the status code based on the exception type or message
        // For simplicity, we use BadRequest for business logic exceptions or Unauthorized for invalid credentials
        if (exception.Message.Contains("Invalid username or password") || exception.Message.Contains("access denied"))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else if (exception.Message.Contains("not found"))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        var result = JsonSerializer.Serialize(new { message = exception.Message });
        return context.Response.WriteAsync(result);
    }
}

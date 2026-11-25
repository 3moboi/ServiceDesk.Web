using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.Services;

namespace ServiceDesk.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Не найдено", ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Нарушено бизнес-правило", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанное исключение");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Внутренняя ошибка", "При обработке запроса произошла ошибка");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        });
    }
}
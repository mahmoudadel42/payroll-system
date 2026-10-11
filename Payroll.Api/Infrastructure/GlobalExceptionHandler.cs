using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Resources;

namespace Payroll.Api.Infrastructure;

/// <summary>Maps exceptions to RFC 7807 ProblemDetails with localized titles and a stable <c>code</c> extension.</summary>
public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, code, problem) = exception switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest, "validation", Validation(ve)),
            DbUpdateConcurrencyException =>
                (StatusCodes.Status409Conflict, "concurrency", new ProblemDetails { Title = AppMessages.Get("concurrency") }),
            DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
                (StatusCodes.Status409Conflict, "duplicate", new ProblemDetails { Title = AppMessages.Get("duplicate") }),
            BadHttpRequestException bad => (StatusCodes.Status400BadRequest, "badRequest", new ProblemDetails { Title = bad.Message }),
            _ => (StatusCodes.Status500InternalServerError, "server", new ProblemDetails { Title = AppMessages.Get("server") })
        };

        if (status >= 500) logger.LogError(exception, "Unhandled exception");
        else logger.LogInformation("Request failed with {Code}: {Message}", code, exception.Message);

        problem.Status = status;
        problem.Extensions["code"] = code;
        context.Response.StatusCode = status;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problem,
            Exception = exception
        });
    }

    private static ValidationProblemDetails Validation(ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => ToCamelCase(e.PropertyName))
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());
        return new ValidationProblemDetails(errors) { Title = AppMessages.Get("validation") };
    }

    private static string ToCamelCase(string name) =>
        string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];
}

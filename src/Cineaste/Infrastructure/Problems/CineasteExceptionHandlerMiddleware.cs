using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Cineaste.Infrastructure.Problems;

internal sealed class CineasteExceptionHandlerMiddleware(RequestDelegate next)
{
    private const string Problem = nameof(Problem);

    private const string Errors = "errors";
    private const string Properties = "properties";
    private const string Resource = "resource";

    public async Task InvokeAsync(
        HttpContext context,
        IProblemDetailsService problemDetailsService,
        IHostEnvironment env,
        ILogger<CineasteExceptionHandlerMiddleware> logger)
    {
        if (context.Features.Get<IExceptionHandlerFeature>()?.Error is Exception exception)
        {
            var problemDetails = this.CreateProblemDetails(exception);

            if (env.IsDevelopment())
            {
                problemDetails.Extensions.Add("exception", new ExceptionDetails(exception));
            }

            context.Response.StatusCode = problemDetails.Status ?? Status500InternalServerError;

            if (context.Response.StatusCode == Status500InternalServerError)
            {
                logger.LogError(exception, "Unhandled exception");
            }

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = problemDetails
            });
        } else
        {
            await next(context);
        }
    }

    private ProblemDetails CreateProblemDetails(Exception exception) =>
        exception switch
        {
            ValidationException ex => new ProblemDetails
            {
                Type = FormatProblemType(Status400BadRequest),
                Title = ReasonPhrases.GetReasonPhrase(Status400BadRequest),
                Status = Status400BadRequest,
                Detail = !ex.Errors.Any()
                    ? $"{Problem}.ValidationFailed"
                    : $"{Problem}.ValidationFailed.{ex.Errors.First().ErrorCode.Split('.')[0]}",
                Extensions =
                {
                    [Errors] = ex.Errors
                        .GroupBy(error => error.PropertyName, error => error.ErrorCode)
                        .ToDictionary(errors => errors.Key, errors => errors.ToList())
                }
            },
            InvalidInputException ex => new ProblemDetails
            {
                Type = FormatProblemType(Status400BadRequest),
                Title = ReasonPhrases.GetReasonPhrase(Status400BadRequest),
                Status = Status400BadRequest,
                Detail = $"{Problem}.{ex.MessageCode}",
                Extensions = { [Properties] = ex.Properties }
            },
            NotFoundException ex => new ProblemDetails
            {
                Type = FormatProblemType(Status404NotFound),
                Title = ReasonPhrases.GetReasonPhrase(Status404NotFound),
                Status = Status404NotFound,
                Detail = $"{Problem}.{ex.MessageCode}",
                Extensions = { [Resource] = ex.Resource, [Properties] = ex.Properties }
            },
            ConflictException ex => new ProblemDetails
            {
                Type = FormatProblemType(Status409Conflict),
                Title = ReasonPhrases.GetReasonPhrase(Status409Conflict),
                Status = Status409Conflict,
                Detail = $"{Problem}.{ex.MessageCode}",
                Extensions = { [Resource] = ex.Resource, [Properties] = ex.Properties }
            },
            _ => new ProblemDetails
            {
                Type = FormatProblemType(Status500InternalServerError),
                Title = ReasonPhrases.GetReasonPhrase(Status500InternalServerError),
                Status = Status500InternalServerError,
                Detail = $"{Problem}.Unknown"
            }
        };

    private string FormatProblemType(int statusCode) =>
        $"https://httpstatuses.io/{statusCode}";
}

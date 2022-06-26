namespace Cineaste.Server.Infrastructure.Problems;

using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using static StatusCodes;

public static class ExceptionMappingExtensions
{
    private const string Problem = "Problem";

    private const string Errors = "errors";
    private const string Properties = "properties";
    private const string Resource = "resource";

    public static void MapCineasteExceptions(this ProblemDetailsOptions options)
    {
        options.Map<ValidationException>(ex => new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{Status400BadRequest}",
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
        });

        options.Map<BadRequestException>(ex => new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{Status400BadRequest}",
            Title = ReasonPhrases.GetReasonPhrase(Status400BadRequest),
            Status = Status400BadRequest,
            Detail = $"{Problem}.{ex.MessageCode}",
            Extensions = { [Properties] = ex.Properties }
        });

        options.Map<NotFoundException>(ex => new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{Status404NotFound}",
            Title = ReasonPhrases.GetReasonPhrase(Status404NotFound),
            Status = Status404NotFound,
            Detail = $"{Problem}.{ex.MessageCode}",
            Extensions = { [Resource] = ex.Resource, [Properties] = ex.Properties }
        });

        options.Map<ConflictException>(ex => new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{Status409Conflict}",
            Title = ReasonPhrases.GetReasonPhrase(Status409Conflict),
            Status = Status409Conflict,
            Detail = $"{Problem}.{ex.MessageCode}",
            Extensions = { [Resource] = ex.Resource, [Properties] = ex.Properties }
        });
    }
}

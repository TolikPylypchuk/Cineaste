namespace Cineaste.Server.Infrastructure;

using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using static StatusCodes;

public static class ExceptionMappingExtensions
{
    public static void MapCineasteExceptions(this ProblemDetailsOptions options)
    {
        options.Map<ValidationException>(ex => new ProblemDetails
            {
                Type = $"https://httpstatuses.io/{Status400BadRequest}",
                Title = ReasonPhrases.GetReasonPhrase(Status400BadRequest),
                Status = Status400BadRequest,
                Detail = !ex.Errors.Any()
                    ? "Problem.ValidationFailed"
                    : $"Problem.ValidationFailed.{ex.Errors.First().ErrorCode.Split('.')[0]}",
                Extensions =
                {
                    ["errors"] = ex.Errors
                        .GroupBy(error => error.PropertyName, error => error.ErrorCode)
                        .ToDictionary(errors => errors.Key, errors => errors.ToList())
                }
            });

        options.Map<NotFoundException>(ex => new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{Status404NotFound}",
            Title = ReasonPhrases.GetReasonPhrase(Status404NotFound),
            Status = Status404NotFound,
            Detail = $"Problem.{ex.MessageCode}",
            Extensions = { ["resource"] = ex.Resource, ["properties"] = ex.Properties }
        });
    }
}

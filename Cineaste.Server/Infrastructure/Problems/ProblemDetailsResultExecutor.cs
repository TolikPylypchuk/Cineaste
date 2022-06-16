namespace Cineaste.Server.Infrastructure.Problems;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;

public class ProblemDetailsResultExecutor : IActionResultExecutor<ObjectResult>
{
    private readonly JsonSerializerOptions jsonOptions;

    public ProblemDetailsResultExecutor(IOptions<JsonSerializerOptions> jsonOptions) =>
        this.jsonOptions = jsonOptions.Value;

    public virtual Task ExecuteAsync(ActionContext context, ObjectResult result)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(result);

        var executor = Results.Json(result.Value, this.jsonOptions, "application/problem+json", result.StatusCode);
        return executor.ExecuteAsync(context.HttpContext);
    }
}

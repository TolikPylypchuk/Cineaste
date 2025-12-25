using Cineaste.Endpoints;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Cineaste.Problems;

public sealed class ProblemDetailsOperationTransformer(IOptions<JsonOptions> options) : IOpenApiOperationTransformer
{
    private readonly JsonSerializerOptions jsonSerializerOptions = options.Value.SerializerOptions;

    public async Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var problemsByStatus = context.Description
            .ActionDescriptor
            .EndpointMetadata
            .OfType<ProblemEndpointMetadata>()
            .Select(metadata => this.CreateProblemDetails(metadata, context))
            .WhereNotNull()
            .GroupBy(problemDetails => problemDetails.Status)
            .Where(group => group.Key is not null)
            .ToDictionary(group => group.Key ?? 0, group => group.ToList());

        foreach (var (status, problems) in problemsByStatus)
        {
            await this.AddProblemResponse(operation, status, problems, context, cancellationToken);
        }
    }

    private ProblemDetails? CreateProblemDetails(
        ProblemEndpointMetadata metadata,
        OpenApiOperationTransformerContext context)
    {
        var customizer = context.ApplicationServices.GetService<ProblemCustomizer>();

        if (customizer is null)
        {
            return null;
        }

        var problemDetails = new ProblemDetails();
        customizer.CustomizeProblemDetails(problemDetails, metadata.Exception, $"/{context.Description.RelativePath}");

        return problemDetails;
    }

    private async Task AddProblemResponse(
        OpenApiOperation operation,
        int status,
        List<ProblemDetails> problems,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var response = new OpenApiResponse
        {
            Description = ReasonPhrases.GetReasonPhrase(status)
        };

        response.Content ??= new Dictionary<string, OpenApiMediaType>();
        response.Content["application/problem+json"] = new OpenApiMediaType
        {
            Schema = await context.GetOrCreateSchemaAsync(typeof(ProblemDetails), cancellationToken: cancellationToken),
            Examples = this.CreateExamples(problems)
        };

        operation.Responses ??= [];
        operation.Responses[status.ToString()] = response;
    }

    private Dictionary<string, IOpenApiExample> CreateExamples(List<ProblemDetails> problems) =>
        problems.GroupBy(problemDetails => problemDetails.NonNullTitle)
            .Select(group => group.ToList())
            .SelectMany(group => group.Count == 1
                ? group.Select(problem => (Title: problem.NonNullTitle, Problem: problem))
                : group.Select((problem, index) => (Title: $"{problem.NonNullTitle} ({index + 1})", Problem: problem)))
            .ToDictionary(
                titleAndProblem => titleAndProblem.Title,
                titleAndProblem => (IOpenApiExample)new OpenApiExample
                {
                    Summary = titleAndProblem.Title,
                    Value = JsonSerializer.SerializeToNode(titleAndProblem.Problem, this.jsonSerializerOptions)
                });
}

file static class Extensions
{
    extension(ProblemDetails problemDetails)
    {
        public string NonNullTitle =>
            problemDetails.Title ?? String.Empty;
    }
}

using Cineaste.Client.Localization;
using Cineaste.Endpoints;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Cineaste.Problems;

public sealed class ProblemDetailsOperationTransformer(IOptions<JsonOptions> options) : IOpenApiOperationTransformer
{
    private const string ProblemJsonContentType = "application/problem+json";

    private static readonly OpenApiSchema ProblemSchema = new()
    {
        Type = JsonSchemaType.Object,
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["type"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "uri" },
            ["title"] = new OpenApiSchema { Type = JsonSchemaType.String },
            ["status"] = new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32" },
            ["detail"] = new OpenApiSchema { Type = JsonSchemaType.String },
            ["instance"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "uri" },
        },
        AdditionalPropertiesAllowed = true,
        Required = new HashSet<string> { "type", "title", "status", "detail", "instance" }
    };

    private readonly JsonSerializerOptions jsonSerializerOptions = options.Value.SerializerOptions;

    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        context.Description
            .ActionDescriptor
            .EndpointMetadata
            .OfType<ProblemEndpointMetadata>()
            .Select(metadata => this.CreateProblemDetails(metadata, context))
            .WhereNotNull()
            .GroupBy(problemDetails => problemDetails.Status)
            .Where(group => group.Key is not null)
            .ToDictionary(group => group.Key ?? 0, group => group.ToList())
            .ForEach(group => this.AddProblemResponse(operation, group.Key, group.Value));

        return Task.CompletedTask;
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

    private void AddProblemResponse(OpenApiOperation operation, int status, List<ProblemDetails> problemDetails)
    {
        var examples = problemDetails.GroupBy(problemDetails => problemDetails.NonNullTitle)
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

        var response = new OpenApiResponse
        {
            Description = ReasonPhrases.GetReasonPhrase(status)
        };

        response.Content ??= new Dictionary<string, OpenApiMediaType>();
        response.Content[ProblemJsonContentType] = new OpenApiMediaType
        {
            Schema = ProblemSchema,
            Examples = examples
        };

        operation.Responses ??= [];
        operation.Responses[status.ToString()] = response;
    }
}

file static class Extensions
{
    extension(ProblemDetails problemDetails)
    {
        public string NonNullTitle =>
            problemDetails.Title ?? String.Empty;
    }
}

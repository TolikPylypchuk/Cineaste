using System.Text.Json.Serialization;

using Cineaste.Server.Infrastructure;

using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;

using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CineasteDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default"), sql => sql.MigrationsHistoryTable("Migrations")));

builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSingleton<IActionResultExecutor<ObjectResult>, ProblemDetailsResultExecutor>();

builder.Services.AddProblemDetails(options =>
{
    bool isDevelopment = builder.Environment.IsDevelopment();

    options.IncludeExceptionDetails = (ctx, ex) => isDevelopment;
    options.ExceptionDetailsPropertyName = "exception";
    options.ShouldLogUnhandledException = (ctx, ex, details) => ex is not ApplicationException;

    options.Map<NotFoundException>(ex => new ProblemDetails
    {
        Type = $"https://httpstatuses.io/{StatusCodes.Status404NotFound}",
        Title = ReasonPhrases.GetReasonPhrase(StatusCodes.Status404NotFound),
        Status = StatusCodes.Status404NotFound,
        Detail = ex.Message,
        Extensions = { ["resource"] = ex.Resource, ["properties"] = ex.Properties }
    });
});

builder.Services.AddScoped<ICultureExtractor, CultureExtractor>();
builder.Services.AddScoped<IListMapper, ListMapper>();
builder.Services.AddScoped<IListService, ListService>();
builder.Services.AddScoped<TestDataProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseProblemDetails();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UsePathBase(new PathString("/api"));
app.UseRouting();

app.MapCultureRoutes();
app.MapListRoutes();

app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider.GetRequiredService<TestDataProvider>();
    await provider.CreateTestDataIfNeeded();
}

await app.RunAsync();

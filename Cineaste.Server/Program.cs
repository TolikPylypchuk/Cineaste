using System.Text.Json.Serialization;

using Cineaste.Server.Infrastructure.Json;
using Cineaste.Server.Infrastructure.Problems;
using Cineaste.Shared.Validation.Json;

using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CineasteDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default"), sql => sql.MigrationsHistoryTable("Migrations")));

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new IdJsonConverterFactory());
    options.SerializerOptions.Converters.Add(new ValidatedJsonConverterFactory());
});

builder.Services.AddSingleton<IActionResultExecutor<ObjectResult>, ProblemDetailsResultExecutor>();

builder.Services.AddProblemDetails(options =>
{
    bool isDevelopment = builder.Environment.IsDevelopment();

    options.IncludeExceptionDetails = (ctx, ex) => isDevelopment;
    options.ExceptionDetailsPropertyName = "exception";
    options.ShouldLogUnhandledException = (ctx, ex, details) => ex is not CineasteException and not ValidationException;

    options.MapCineasteExceptions();
});

builder.Services.AddScoped<ICultureExtractor, CultureExtractor>();
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

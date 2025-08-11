using System.Text.Json.Serialization;

using AngleSharp;

using Cineaste.Components;
using Cineaste.Infrastructure.Json;
using Cineaste.Infrastructure.OpenApi;
using Cineaste.Infrastructure.Problems;
using Cineaste.Shared.Collections.Json;
using Cineaste.Shared.Validation.Json;

using Microsoft.AspNetCore.Http.Features;

using Scalar.AspNetCore;

using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100L * 1024 * 1024; // 100 MB
});

builder.Services.AddDbContext<CineasteDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default"),
    sql => sql.MigrationsHistoryTable("Migrations")));

static void AddConverters(JsonSerializerOptions options)
{
    options.Converters.Add(new JsonStringEnumConverter());
    options.Converters.Add(new ImmutableValueListConverterFactory());
    options.Converters.Add(new IdJsonConverterFactory());
    options.Converters.Add(new ValidatedJsonConverterFactory());
}

builder.Services.Configure<JsonOptions>(options => AddConverters(options.SerializerOptions));
builder.Services.Configure<CineasteOpenApiOptions>(builder.Configuration.GetSection("OpenApi"));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50L * 1024 * 1024; // 50 MB
});

builder.Services.AddHttpClient();
builder.Services.AddProblemDetails();
builder.Services.AddCineasteOpenApi();
builder.Services.AddCors();

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("OpenApi", policy => policy.Expire(TimeSpan.FromHours(1)));
});

builder.Services.AddScoped<CultureProvider>();
builder.Services.AddScoped<ListService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<SeriesService>();
builder.Services.AddScoped<FranchiseService>();
builder.Services.AddScoped<IPosterProvider, PosterProvider>();

builder.Services.AddSingleton(Configuration.Default.WithDefaultLoader());

builder.Services.AddControllers()
    .AddJsonOptions(options => AddConverters(options.JsonSerializerOptions));

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<TestDataProvider>();
}

var app = builder.Build();

app.UseOutputCache();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}

app.UseCineasteExceptionHandling();

app.UseRouting();
app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().CacheOutput("OpenApi");
    app.MapScalarApiReference(options =>
    {
        options.OperationSorter = OperationSorter.Alpha;
    });
}

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Cineaste.Client._Imports).Assembly);

app.MapFallback("/api/{**path}", () =>
{
    throw new NotFoundException(Cineaste.Resources.Any, "The requested resource was not found");
});

if (builder.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var provider = scope.ServiceProvider.GetRequiredService<TestDataProvider>();
    await provider.CreateTestDataIfNeeded();
}

await app.RunAsync();

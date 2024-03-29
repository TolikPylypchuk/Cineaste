using System.Text.Json.Serialization;

using Cineaste.Server.Infrastructure.Json;
using Cineaste.Server.Infrastructure.Problems;
using Cineaste.Shared.Collections.Json;
using Cineaste.Shared.Validation.Json;

using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CineasteDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default"),
    sql => sql.MigrationsHistoryTable("Migrations")));

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new ImmutableValueListConverterFactory());
    options.SerializerOptions.Converters.Add(new IdJsonConverterFactory());
    options.SerializerOptions.Converters.Add(new ValidatedJsonConverterFactory());
});

builder.Services.AddProblemDetails();

builder.Services.AddScoped<CultureProvider>();
builder.Services.AddScoped<ListService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<SeriesService>();
builder.Services.AddScoped<FranchiseService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<TestDataProvider>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseCineasteExceptionHandling();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapCultureRoutes();
app.MapListRoutes();
app.MapMovieRoutes();
app.MapSeriesRoutes();
app.MapFranchiseRoutes();

app.MapFallbackRoutes();

if (builder.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var provider = scope.ServiceProvider.GetRequiredService<TestDataProvider>();
    await provider.CreateTestDataIfNeeded();
}

await app.RunAsync();

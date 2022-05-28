using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CineasteDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default"), sql => sql.MigrationsHistoryTable("Migrations")));

builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddControllers();

builder.Services.AddScoped<IListService, ListService>();
builder.Services.AddScoped<TestDataProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UsePathBase(new PathString("/api"));
app.UseRouting();

app.MapControllers();

app.MapListRoutes();

app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider.GetRequiredService<TestDataProvider>();
    await provider.CreateTestDataIfNeeded();
}

await app.RunAsync();

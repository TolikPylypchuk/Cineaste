var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CineasteDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default"), sql => sql.MigrationsHistoryTable("Migrations")));

builder.Services.AddControllers();

builder.Services.AddScoped<IListService, ListService>();

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

app.Run();

using System.ComponentModel;
using System.Text.Json.Serialization;

using Cineaste.Application.Services.List;
using Cineaste.Application.Services.Poster;
using Cineaste.Application.Services.User;
using Cineaste.Client;
using Cineaste.Client.BaseUri;
using Cineaste.Client.Navigation;
using Cineaste.Components;
using Cineaste.Core.Converter;
using Cineaste.Json;
using Cineaste.OpenApi;
using Cineaste.Problems;
using Cineaste.Routes;
using Cineaste.Services.BaseUri;
using Cineaste.Shared.Collections.Json;
using Cineaste.Shared.Validation.Json;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using MudBlazor.Services;

using Scalar.AspNetCore;

using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100L * 1024 * 1024; // 100 MB
});

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddControllers()
    .AddJsonOptions(options => AddConverters(options.JsonSerializerOptions));

builder.Services.Configure<JsonOptions>(options => AddConverters(options.SerializerOptions));

static void AddConverters(JsonSerializerOptions options)
{
    options.Converters.Add(new JsonStringEnumConverter());
    options.Converters.Add(new ImmutableValueListConverterFactory());
    options.Converters.Add(new IdJsonConverterFactory());
    options.Converters.Add(new ValidatedJsonConverterFactory());
}

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50L * 1024 * 1024; // 50 MB
});

builder.Services.AddDbContext<CineasteDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default"),
    sql => sql.MigrationsHistoryTable("Migrations")));

builder.Services.AddIdentityCore<CineasteUser>()
    .AddEntityFrameworkStores<CineasteDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies(cookies =>
{
    cookies.ApplicationCookie?.Configure(options =>
    {
        options.Cookie.Name = "Cineaste.Identity";
        options.LoginPath = NavigationPages.LoginPage;
        options.ReturnUrlParameter = NavigationPages.ReturnUrlParameter;
    });
});

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthorization();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "Cineaste.Antiforgery";
});

TypeDescriptor.AddAttributes(typeof(Id<CineasteUser>), new TypeConverterAttribute(typeof(IdConverter<CineasteUser>)));

builder.Services.AddHttpClient();
builder.Services.AddProblemDetails();
builder.Services.AddCineasteOpenApi(builder.Configuration.GetSection("OpenApi"));
builder.Services.AddCors();

var openApiCachePolicy = "OpenApi";

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy(openApiCachePolicy, policy => policy.Expire(TimeSpan.FromHours(1)));
});

builder.Services.AddMudServices();
builder.Services.AddLocalization();

builder.Services.AddCineasteRefitClients();
builder.Services.AddCineasteFluxor();

builder.Services.AddScoped<IPageNavigator, PageNavigator>();
builder.Services.AddSingleton<IBaseUriProvider, ServerBaseUriProvider>();

builder.Services.AddScoped<CultureProvider>();
builder.Services.AddScoped<IDefaultListCreator, DefaultListCreator>();
builder.Services.AddScoped<ListService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<SeriesService>();
builder.Services.AddScoped<FranchiseService>();

builder.Services.AddScoped<IPosterProvider, PosterProvider>();
builder.Services.AddScoped<IHtmlDocumentProvider, HtmlDocumentProvider>();

builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().CacheOutput(openApiCachePolicy);
    app.MapScalarApiReference(options =>
    {
        options.TagSorter = TagSorter.Alpha;
        options.OperationSorter = OperationSorter.Alpha;
    });
}

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Cineaste.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

app.MapFallback("/api/{**path}", (string path) =>
    TypedResults.Problem(
        type: "/problem/not-found",
        statusCode: StatusCodes.Status404NotFound,
        title: ReasonPhrases.GetReasonPhrase(StatusCodes.Status404NotFound),
        detail: "The requested resource was not found",
        instance: $"/api/{path}"));

if (builder.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var provider = scope.ServiceProvider.GetRequiredService<TestDataProvider>();
    await provider.CreateTestDataIfNeeded();
}

await app.RunAsync();

using System.Reflection;
using System.Text.Json.Serialization;

using Cineaste.Shared.Collections.Json;
using Cineaste.Shared.Validation.Json;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNameCaseInsensitive = true;
    options.Converters.Add(new JsonStringEnumConverter());
    options.Converters.Add(new ImmutableValueListConverterFactory());
    options.Converters.Add(new ValidatedJsonConverterFactory());
});

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
var apiBaseAddress = new Uri(baseAddress, "/api");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });

builder.Services.AddMudServices();

builder.Services.AddRefitClient<ICultureApi>(apiBaseAddress);
builder.Services.AddRefitClient<IListApi>(apiBaseAddress);
builder.Services.AddRefitClient<IMovieApi>(apiBaseAddress);
builder.Services.AddRefitClient<ISeriesApi>(apiBaseAddress);
builder.Services.AddRefitClient<IFranchiseApi>(apiBaseAddress);

builder.Services.AddLocalization();

builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(Assembly.GetExecutingAssembly());

#if DEBUG
    options.UseReduxDevTools();
#endif
});

builder.Services.AddScoped<IPageNavigator, PageNavigator>();

var english = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = english;
CultureInfo.DefaultThreadCurrentUICulture = english;

await builder.Build().RunAsync();

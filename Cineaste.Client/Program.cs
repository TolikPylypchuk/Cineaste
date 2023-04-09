using System.Reflection;
using System.Text.Json.Serialization;

using Cineaste.Shared.Collections.Json;
using Cineaste.Shared.Validation.Json;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Fast.Components.FluentUI;

using Radzen;

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

var baseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), "/api");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });

builder.Services.AddRefitClient<ICultureApi>(baseAddress);
builder.Services.AddRefitClient<IListApi>(baseAddress);
builder.Services.AddRefitClient<IMovieApi>(baseAddress);
builder.Services.AddRefitClient<ISeriesApi>(baseAddress);

builder.Services.AddFluentUIComponents();

builder.Services.AddLocalization();

builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(Assembly.GetExecutingAssembly());

#if DEBUG
    options.UseReduxDevTools();
#endif
});

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddScoped<IPageNavigator, PageNavigator>();

var english = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = english;
CultureInfo.DefaultThreadCurrentUICulture = english;

await builder.Build().RunAsync();

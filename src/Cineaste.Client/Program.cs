using System.Text.Json.Serialization;

using Cineaste.Client.BaseUri;
using Cineaste.Shared.Collections.Json;
using Cineaste.Shared.Validation.Json;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNameCaseInsensitive = true;
    options.Converters.Add(new JsonStringEnumConverter());
    options.Converters.Add(new ImmutableValueListConverterFactory());
    options.Converters.Add(new ValidatedJsonConverterFactory());
});

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });

builder.Services.AddSingleton<IBaseUriProvider>(sp => new StaticBaseUriProvider(baseAddress));

builder.Services.AddAuthorizationCore()
    .AddCascadingAuthenticationState()
    .AddAuthenticationStateDeserialization();

builder.Services.AddMudServices();
builder.Services.AddLocalization();

builder.Services.AddCineasteRefitClients();
builder.Services.AddCineasteFluxor();

builder.Services.AddScoped<IPageNavigator, PageNavigator>();

var english = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = english;
CultureInfo.DefaultThreadCurrentUICulture = english;

await builder.Build().RunAsync();

using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNameCaseInsensitive = true;
    options.Converters.Add(new JsonStringEnumConverter());
});

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });

builder.Services.AddRefitClient<ICultureApi>(baseAddress);
builder.Services.AddRefitClient<IListApi>(baseAddress);

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddScoped<IPageNavigator, PageNavigator>();

builder.Services.AddScoped<CreateListPageViewModel>();
builder.Services.AddScoped<HomePageViewModel>();
builder.Services.AddScoped<ListPageViewModel>();

await builder.Build().RunAsync();

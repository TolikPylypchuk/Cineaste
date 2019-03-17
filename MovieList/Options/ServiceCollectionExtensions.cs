using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MovieList.Options
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.json")
            where T : class, new()
            => services
                .Configure<T>(value => section.Bind(value))
                .AddTransient<IWritableOptions<T>>(provider =>
                    new WritableOptions<T>(provider.GetService<IOptionsMonitor<T>>(), section.Key, file));
    }
}

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
                .Configure<T>(value => CopyFromSection(value, section))
                .AddTransient<IWritableOptions<T>>(provider =>
                    new WritableOptions<T>(provider.GetService<IOptionsMonitor<T>>(), section.Key, file));

        private static void CopyFromSection<T>(T value, IConfigurationSection section)
        {
            if (value == null || section == null)
            {
                return;
            }

            var valueToCopy = section.Get<T>();

            if (valueToCopy == null)
            {
                return;
            }

            foreach (var property in value.GetType().GetProperties())
            {
                property.SetValue(value, property.GetValue(valueToCopy));
            }
        }
    }
}

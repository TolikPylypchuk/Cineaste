using System;
using System.Windows;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MovieList.Data;
using MovieList.Services;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.ServiceProvider = new ServiceCollection()
                .AddDbContext<MovieContext>((services, builder) =>
                    builder.UseSqlite($"Data Source={services.GetService<SettingsService>().Settings.DatabasePath}"))
                .AddSingleton<SettingsService>()
                .BuildServiceProvider();
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.

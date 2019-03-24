using System;
using System.Windows;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MovieList.Config;
using MovieList.Data;
using MovieList.Options;
using MovieList.Services;
using MovieList.ViewModels;
using MovieList.Views;

namespace MovieList
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfigurationRoot Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ConfigureServices();

            this.MainWindow = this.ServiceProvider.GetRequiredService<MainWindow>();
            this.MainWindow.Show();
        }

        private void ConfigureServices()
        {
            this.Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            this.ServiceProvider = new ServiceCollection()
                .AddDbContext<MovieContext>(
                    (services, builder) =>
                        builder.ConfigureMovieContext(this.Configuration.GetSection("Config")["DatabasePath"]),
                    ServiceLifetime.Scoped)

                .AddLogging(loggingBuilder => loggingBuilder
                    .AddConfiguration(this.Configuration.GetSection("Logging"))
                    .AddFile(this.Configuration.GetSection("Logging")))

                .ConfigureWritable<Configuration>(this.Configuration.GetSection("Config"))
                .ConfigureWritable<UIConfig>(this.Configuration.GetSection("UI"))
                .ConfigureWritable<LoggingConfig>(this.Configuration.GetSection("Logging"))

                .AddScoped<IMovieListService, MovieListService>()

                .AddTransient<MainViewModel>()
                .AddTransient<MovieListViewModel>()
                .AddTransient<MovieFormViewModel>()
                .AddTransient<SeriesFormViewModel>()
                .AddTransient<MovieSeriesFormViewModel>()
                .AddTransient<SettingsViewModel>()

                .AddSingleton<MainWindow>()
                .AddSingleton(_ => this)

                .BuildServiceProvider();
        }
    }
}

using System;
using System.Windows;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MovieList.Config;
using MovieList.Data;
using MovieList.Options;
using MovieList.Services;
using MovieList.Services.Implementations;
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

            this.Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            this.ConfigureServices(services);
            this.ServiceProvider = services.BuildServiceProvider();

            AppDomain.CurrentDomain.UnhandledException += this.CatchException;

            this.MainWindow = this.ServiceProvider.GetRequiredService<MainWindow>();
            this.MainWindow?.Show();
        }

        private void ConfigureServices(IServiceCollection services)
            => services
                .AddDbContext<MovieContext>(
                    (s, builder) => builder
                        .ConfigureMovieContext(this.Configuration.GetSection("Config")["DatabasePath"])
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors(),
                    ServiceLifetime.Transient)

                .AddLogging(loggingBuilder => loggingBuilder
                    .AddConfiguration(this.Configuration.GetSection("Logging"))
                    .AddFile(this.Configuration.GetSection("Logging")))

                .ConfigureWritable<Configuration>(this.Configuration.GetSection("Config"))
                .ConfigureWritable<UIConfig>(this.Configuration.GetSection("UI"))
                .ConfigureWritable<LoggingConfig>(this.Configuration.GetSection("Logging"))

                .AddSingleton<IDbService, DbService>()
                .AddSingleton<IFileService, FileService>()

                .AddSingleton<MainViewModel>()
                .AddSingleton<MovieListViewModel>()
                .AddSingleton<SidePanelViewModel>()
                .AddSingleton<AddNewViewModel>()
                .AddSingleton<MovieFormViewModel>()
                .AddSingleton<SeriesFormViewModel>()
                .AddSingleton<MovieSeriesFormViewModel>()
                .AddSingleton<SeasonFormViewModel>()
                .AddSingleton<SpecialEpisodeFormViewModel>()
                .AddSingleton<SettingsViewModel>()

                .AddSingleton<MainWindow>()
                .AddSingleton(_ => this.ServiceProvider)
                .AddSingleton(_ => this);

        private void CatchException(object sender, UnhandledExceptionEventArgs e)
            => this.ServiceProvider.GetService<ILogger<App>>().LogError(e.ExceptionObject.ToString());
    }
}

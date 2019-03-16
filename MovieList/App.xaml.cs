using System;
using System.Windows;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MovieList.Config;
using MovieList.Config.Logging;
using MovieList.Data;
using MovieList.Options;
using MovieList.Views;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

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
                .AddDbContext<MovieContext>((services, builder) =>
                    builder.UseSqlite($"Data Source={this.Configuration.GetSection("Config")["DatabasePath"]}"))
                .AddLogging(loggingBuilder => loggingBuilder.AddFile(this.Configuration.GetSection("Logging")))
                .ConfigureWritable<Configuration>(this.Configuration.GetSection("Config"))
                .ConfigureWritable<LoggingConfig>(this.Configuration.GetSection("Logging"))
                .ConfigureWritable<UIConfig>(this.Configuration.GetSection("UI"))
                .AddSingleton(_ => this)
                .AddSingleton<MainWindow>()
                .BuildServiceProvider();
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.

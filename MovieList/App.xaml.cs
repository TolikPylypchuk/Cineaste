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

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfigurationRoot Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
            => this.ConfigureServices();

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
                .AddSingleton(_ => this)
                .BuildServiceProvider();
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized.

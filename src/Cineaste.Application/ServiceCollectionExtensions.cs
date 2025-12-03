using Cineaste.Application.Services;
using Cineaste.Application.Services.List;
using Cineaste.Application.Services.User;

using Microsoft.Extensions.DependencyInjection;

namespace Cineaste.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddScoped<CultureProvider>();
            services.AddScoped<IDefaultListCreator, DefaultListCreator>();
            services.AddScoped<ListService>();
            services.AddScoped<MovieService>();
            services.AddScoped<SeriesService>();
            services.AddScoped<FranchiseService>();

            services.AddScoped<IPosterProvider, PosterProvider>();
            services.AddScoped<IHtmlDocumentProvider, HtmlDocumentProvider>();

            services.AddScoped<IUserRegistrationService, UserRegistrationService>();

            return services;
        }
    }
}

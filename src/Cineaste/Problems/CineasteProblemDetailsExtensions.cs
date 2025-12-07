using Microsoft.AspNetCore.Diagnostics;

namespace Cineaste.Problems;

internal static class CineasteProblemDetailsExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCineasteProblemDetails() =>
            services.AddSingleton<ProblemCustomizer>()
                .AddProblemDetails(options =>
                    options.CustomizeProblemDetails = context =>
                    {
                        var exception = context.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
                        if (exception is not null)
                        {
                            var customizer = context.HttpContext.RequestServices.GetService<ProblemCustomizer>();
                            customizer?.CustomizeProblemDetails(
                                context.ProblemDetails, exception, context.HttpContext.Request.Path);
                        }

                        if (context.ProblemDetails.Status is int statusCode)
                        {
                            context.HttpContext.Response.StatusCode = statusCode;
                        }
                    });
    }
}

namespace Cineaste.Server.Infrastructure.Problems;

using Microsoft.AspNetCore.Builder;

internal static class CineasteExceptionHandlerExtensions
{
    public static IApplicationBuilder UseCineasteExceptionHandling(this IApplicationBuilder app) =>
        app.UseExceptionHandler(exceptionHandlerApp =>
            exceptionHandlerApp.UseMiddleware<CineasteExceptionHandlerMiddleware>());
}

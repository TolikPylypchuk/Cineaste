namespace Cineaste.Infrastructure.Problems;

internal static class CineasteExceptionHandlerExtensions
{
    public static IApplicationBuilder UseCineasteExceptionHandling(this IApplicationBuilder app) =>
        app.UseExceptionHandler(exceptionHandlerApp =>
            exceptionHandlerApp.UseMiddleware<CineasteExceptionHandlerMiddleware>());
}

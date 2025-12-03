namespace Cineaste.Problems;

internal static class CineasteExceptionHandlerExtensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseCineasteExceptionHandling() =>
            app.UseExceptionHandler(exceptionHandlerApp =>
                exceptionHandlerApp.UseMiddleware<CineasteExceptionHandlerMiddleware>());
    }
}

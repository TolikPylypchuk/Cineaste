namespace Cineaste.Client.Services.Api;

[AutoConstructor]
public sealed partial class ApiExecutorFactory : IApiExecutorFactory
{
    private readonly IServiceProvider services;

    public IApiExecutor<TApi> For<TApi>()
        where TApi : notnull =>
        this.services.GetRequiredService<IApiExecutor<TApi>>();
}

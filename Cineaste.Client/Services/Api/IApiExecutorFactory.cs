namespace Cineaste.Client.Services.Api;

public interface IApiExecutorFactory
{
    IApiExecutor<TApi> For<TApi>()
        where TApi : notnull;
}

namespace Cineaste.Client.Services.BaseUri;

public sealed class StaticBaseUriProvider(Uri baseUri) : IBaseUriProvider
{
    public Uri BaseUri { get; } = baseUri;
}

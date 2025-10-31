using Cineaste.Client.BaseUri;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Cineaste.Services.BaseUri;

public sealed class ServerBaseUriProvider(IServer server) : IBaseUriProvider
{
    public Uri BaseUri { get; } =
        new Uri(server.Features.GetRequiredFeature<IServerAddressesFeature>().Addresses.First());
}

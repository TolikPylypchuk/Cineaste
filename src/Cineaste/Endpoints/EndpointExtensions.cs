using static Cineaste.Shared.Validation.PosterContentTypes;

namespace Cineaste.Endpoints;

public static class EndpointExtensions
{
    extension(RouteHandlerBuilder endpoint)
    {
        public RouteHandlerBuilder AcceptsPosterContentTypes() =>
            endpoint.Accepts<byte[]>(ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp);

        public RouteHandlerBuilder ProducesPosterContentTypes(int statusCode = StatusCodes.Status200OK) =>
            endpoint.Produces<byte[]>(
                statusCode, ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp);
    }
}

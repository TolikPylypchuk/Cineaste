using Cineaste.Problems;

using FluentValidation.Results;

using static Cineaste.Shared.Validation.PosterContentTypes;

namespace Cineaste.Endpoints;

public static class EndpointExtensions
{
    private const string ExamplePosterUrl = "https://example.com/poster.png";
    private const string ExampleContentType = "application/json";

    extension(RouteHandlerBuilder endpoint)
    {
        public RouteHandlerBuilder AcceptsPosterContentTypes() =>
            endpoint.Accepts<byte[]>(ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp);

        public RouteHandlerBuilder ProducesPosterContentTypes(int statusCode = StatusCodes.Status200OK) =>
            endpoint.Produces<byte[]>(
                statusCode, ImageApng, ImageAvif, ImageGif, ImageJpeg, ImagePng, ImageSvg, ImageWebp);

        public RouteHandlerBuilder ProducesProblem(Func<Exception> exceptionFunc) =>
            endpoint.WithMetadata(new ProblemEndpointMetadata(exceptionFunc));

        public RouteHandlerBuilder ProducesValidationProblem(string message) =>
            endpoint.ProducesProblem(() => new ValidationException(
                message, [new ValidationFailure { PropertyName = "Property", ErrorCode = "Error.Code" }]));

        public RouteHandlerBuilder ProducesMovieRequestValidationProblem() =>
            endpoint.ProducesValidationProblem("Movie request is invalid");

        public RouteHandlerBuilder ProducesSeriesRequestValidationProblem() =>
            endpoint.ProducesValidationProblem("Series request is invalid");

        public RouteHandlerBuilder ProducesLimitedSeriesRequestValidationProblem() =>
            endpoint.ProducesValidationProblem("Limited series request is invalid");

        public RouteHandlerBuilder ProducesFranchiseRequestValidationProblem() =>
            endpoint.ProducesValidationProblem("Franchise request is invalid");

        public RouteHandlerBuilder ProducesListNotFoundProblem() =>
            endpoint.ProducesProblem(() => new ListNotFoundException(Id.Create<CineasteList>()));

        public RouteHandlerBuilder ProducesMovieNotFoundProblem() =>
            endpoint.ProducesProblem(() => new MovieNotFoundException(Id.Create<Movie>()));

        public RouteHandlerBuilder ProducesSeriesNotFoundProblem() =>
            endpoint.ProducesProblem(() => new SeriesNotFoundException(Id.Create<Series>()));

        public RouteHandlerBuilder ProducesSeasonPartNotFoundProblem() =>
            endpoint.ProducesProblem(() => new SeasonPartNotFoundException(Id.Create<SeasonPart>()));

        public RouteHandlerBuilder ProducesSpecialEpisodeNotFoundProblem() =>
            endpoint.ProducesProblem(() => new SpecialEpisodeNotFoundException(Id.Create<SpecialEpisode>()));

        public RouteHandlerBuilder ProducesLimitedSeriesNotFoundProblem() =>
            endpoint.ProducesProblem(() => new LimitedSeriesNotFoundException(Id.Create<LimitedSeries>()));

        public RouteHandlerBuilder ProducesFranchiseNotFoundProblem() =>
            endpoint.ProducesProblem(() => new FranchiseNotFoundException(Id.Create<Franchise>()));

        public RouteHandlerBuilder ProducesFranchiseItemsNotFoundProblem() =>
            endpoint.ProducesProblem(() => new FranchiseItemsNotFoundException(
                [Id.Create<Movie>()], [Id.Create<Series>()], [Id.Create<Franchise>()]));

        public RouteHandlerBuilder ProducesMovieKindNotFoundProblem() =>
            endpoint.ProducesProblem(() => new MovieKindNotFoundException(Id.Create<MovieKind>()));

        public RouteHandlerBuilder ProducesSeriesKindNotFoundProblem() =>
            endpoint.ProducesProblem(() => new SeriesKindNotFoundException(Id.Create<SeriesKind>()));

        public RouteHandlerBuilder ProducesMoviePosterNotFoundProblem() =>
            endpoint.ProducesProblem(() => new MoviePosterNotFoundException(Id.Create<Movie>()));

        public RouteHandlerBuilder ProducesSeriesPosterNotFoundProblem() =>
            endpoint.ProducesProblem(() => new SeriesPosterNotFoundException(Id.Create<Series>()));

        public RouteHandlerBuilder ProducesSeasonPosterNotFoundProblem() =>
            endpoint.ProducesProblem(() => new SeasonPosterNotFoundException(Id.Create<SeasonPart>()));

        public RouteHandlerBuilder ProducesSpecialEpisodePosterNotFoundProblem() =>
            endpoint.ProducesProblem(() => new SpecialEpisodePosterNotFoundException(Id.Create<SpecialEpisode>()));

        public RouteHandlerBuilder ProducesLimitedSeriesPosterNotFoundProblem() =>
            endpoint.ProducesProblem(() => new LimitedSeriesPosterNotFoundException(Id.Create<LimitedSeries>()));

        public RouteHandlerBuilder ProducesFranchisePosterNotFoundProblem() =>
            endpoint.ProducesProblem(() => new FranchisePosterNotFoundException(Id.Create<Franchise>()));

        public RouteHandlerBuilder ProducesPosterProblems() =>
            endpoint
                .ProducesProblem(() => new PosterFetchException())
                .ProducesProblem(() => new PosterFetchResponseException(
                    ExamplePosterUrl,
                    new(404, new() { ["Content-Type"] = ExampleContentType }, """{ "error": "Not found" }""")))
                .ProducesProblem(() => new UnsupportedPosterTypeException(ExampleContentType, ExamplePosterUrl))
                .ProducesProblem(() => new NoPosterContentTypeException(ExamplePosterUrl))
                .ProducesProblem(() => new NoPosterContentLengthException(ExamplePosterUrl));

        public RouteHandlerBuilder ProducesImdbPosterProblems() =>
            endpoint
                .ProducesPosterProblems()
                .ProducesProblem(() => new ImdbMediaImageNotFoundException("https://imdb.com/media"));
    }
}

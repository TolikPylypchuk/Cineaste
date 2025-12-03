using System.Reflection;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;

using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Cineaste.Problems;

internal sealed class CineasteExceptionHandlerMiddleware(RequestDelegate next)
{
    private const string Problem = nameof(Problem);

    public async Task InvokeAsync(
        HttpContext context,
        IProblemDetailsService problemDetailsService,
        IHostEnvironment env,
        ILogger<CineasteExceptionHandlerMiddleware> logger)
    {
        if (context.Features.Get<IExceptionHandlerFeature>()?.Error is Exception exception)
        {
            var problemDetails = this.CreateProblemDetails(exception);

            if (env.IsDevelopment())
            {
                problemDetails.Extensions.Add(Props.Exception, new ExceptionDetails(exception));
            }

            int status = problemDetails.Status ?? Status500InternalServerError;
            problemDetails.Title ??= ReasonPhrases.GetReasonPhrase(status);
            context.Response.StatusCode = status;

            problemDetails.Detail ??= exception.Message;

            if (context.Response.StatusCode == Status500InternalServerError)
            {
                logger.LogError(exception, "Unhandled exception");
            }

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = problemDetails
            });
        } else
        {
            await next(context);
        }
    }

    private ProblemDetails CreateProblemDetails(Exception exception) =>
        exception switch
        {
            ValidationException ex => new()
            {
                Type = this.CreateProblemType(Types.Validation),
                Title = "Validation has failed",
                Status = Status400BadRequest,
                Extensions =
                {
                    [Props.Errors] = ex.Errors
                        .GroupBy(error => error.PropertyName, error => error.ErrorCode)
                        .ToDictionary(errors => errors.Key, errors => errors.ToList())
                }
            },
            NotFoundException ex => this.CreateProblemDetails(ex),
            PosterException ex => this.CreateProblemDetails(ex),
            _ => this.CreateDefaultProblemDetails()
        };

    private ProblemDetails CreateProblemDetails(PosterException exception) =>
        exception switch
        {
            PosterFetchException => new()
            {
                Type = this.CreateProblemType(Types.Poster),
                Title = "Poster fetch error",
                Status = Status503ServiceUnavailable
            },
            PosterFetchResponseException ex => new()
            {
                Type = this.CreateProblemType(Types.Poster, Types.ErrorResponse),
                Title = "Unsuccessful response when fetching a poster",
                Status = Status503ServiceUnavailable,
                Extensions = { [Props.Response] = ex.Response, [Props.Url] = ex.Url }
            },
            UnsupportedPosterTypeException ex => new()
            {
                Type = this.CreateProblemType(Types.Poster, Types.Type, Types.Unsupported),
                Title = "Unsupported poster type",
                Status = Status415UnsupportedMediaType,
                Extensions = { [Props.ContentType] = ex.ContentType }
            },
            NoPosterContentTypeException ex => new()
            {
                Type = this.CreateProblemType(Types.Poster, Types.Type, Types.Missing),
                Title = "Missing poster content type",
                Status = Status503ServiceUnavailable,
                Extensions = { [Props.Url] = ex.Url }
            },
            NoPosterContentLengthException ex => new()
            {
                Type = this.CreateProblemType(Types.Poster, Types.Length, Types.Missing),
                Title = "Missing poster content length",
                Status = Status503ServiceUnavailable,
                Extensions = { [Props.Url] = ex.Url }
            },
            ImdbMediaImageNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.Poster, Types.ImdbMedia, Types.Image, Types.NotFound),
                Title = "The image URL is not found in the IMDb media page",
                Status = Status503ServiceUnavailable,
                Extensions = { [Props.Url] = ex.Url }
            },
            _ => this.CreateDefaultProblemDetails()
        };

    private ProblemDetails CreateProblemDetails(NotFoundException exception) =>
        exception switch
        {
            ListNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.List),
                Title = "List not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.ListId }
            },
            ListItemNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.ListItem),
                Title = "List item not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.ItemId }
            },
            MovieNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Movie),
                Title = "Movie not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.MovieId }
            },
            MovieKindNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.MovieKind),
                Title = "Movie kind not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.KindId }
            },
            MoviePosterNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.Movie),
                Title = "Movie poster not found",
                Status = Status404NotFound,
                Extensions = { [Props.MovieId] = ex.MovieId }
            },
            SeriesNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Series),
                Title = "Series not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.SeriesId }
            },
            SeriesKindNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.SeriesKind),
                Title = "Series kind not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.KindId }
            },
            SeriesPosterNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.Series),
                Title = "Series kind not found",
                Status = Status404NotFound,
                Extensions = { [Props.SeriesId] = ex.SeriesId }
            },
            PeriodNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Series, Types.Period),
                Title = "Series period not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.PeriodId }
            },
            SeasonPosterNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.Season),
                Title = "Season poster not found",
                Status = Status404NotFound,
                Extensions = { [Props.PeriodId] = ex.PeriodId }
            },
            SpecialEpisodeNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Series, Types.SpecialEpisode),
                Title = "Special episode not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.EpisodeId }
            },
            SpecialEpisodePosterNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.SpecialEpisode),
                Title = "Special episode poster not found",
                Status = Status404NotFound,
                Extensions = { [Props.EpisodeId] = ex.EpisodeId }
            },
            FranchiseNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Franchise),
                Title = "Franchise not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.FranchiseId }
            },
            FranchiseItemNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.FranchiseItem),
                Title = "Franchise item not found",
                Status = Status404NotFound,
                Extensions = { [Props.Id] = ex.ItemId, [Props.ItemType] = ex.ItemType }
            },
            FranchiseItemWithNumberNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.FranchiseItem),
                Title = "Franchise item not found",
                Status = Status404NotFound,
                Extensions = { [Props.FranchiseId] = ex.FranchiseId, [Props.SequenceNumber] = ex.SequenceNumber }
            },
            FranchiseItemsNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.FranchiseItems),
                Title = "Franchise items not found",
                Status = Status404NotFound,
                Extensions =
                {
                    [Props.MovieIds] = ex.MovieIds,
                    [Props.SeriesIds] = ex.SeriesIds,
                    [Props.FranchiseIds] = ex.FranchiseIds
                }
            },
            FranchisePosterNotFoundException ex => new()
            {
                Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.Franchise),
                Title = "Franchise poster not found",
                Status = Status404NotFound,
                Extensions = { [Props.FranchiseId] = ex.FranchiseId }
            },
            _ => this.CreateDefaultProblemDetails()
        };

    private ProblemDetails CreateDefaultProblemDetails() =>
        new()
        {
            Type = this.CreateProblemType(Types.Unhandled),
            Title = "Unhandled problem",
        };

    private string CreateProblemType(params Span<string> types) =>
        $"/problem/{String.Join('/', types)}";
}

file static class Types
{
    public const string ErrorResponse = "error-response";

    public const string Franchise = "franchise";
    public const string FranchiseItem = "franchise-item";
    public const string FranchiseItems = "franchise-items";

    public const string Image = "image";
    public const string ImdbMedia = "imdb-media";

    public const string Length = "length";
    public const string List = "list";
    public const string ListItem = "list-item";

    public const string Missing = "missing";
    public const string Movie = "movie";
    public const string MovieKind = "movie-kind";

    public const string NotFound = "not-found";

    public const string Period = "period";
    public const string Poster = "poster";

    public const string Season = "season";
    public const string Series = "series";
    public const string SeriesKind = "series-kind";
    public const string SpecialEpisode = "special-episode";

    public const string Type = "type";

    public const string Unhandled = "unhandled";
    public const string Unsupported = "unsupported";

    public const string Validation = "validation";
}

file static class Props
{
    public const string ContentType = "contentType";

    public const string EpisodeId = "episodeId";
    public const string Errors = "errors";
    public const string Exception = "exception";

    public const string FranchiseId = "franchiseId";
    public const string FranchiseIds = "franchiseIds";

    public const string Id = "id";
    public const string ItemType = "itemType";

    public const string MovieId = "movieId";
    public const string MovieIds = "movieIds";

    public const string PeriodId = "periodId";
    public const string Properties = "properties";

    public const string Resource = "resource";
    public const string Response = "response";

    public const string SequenceNumber = "sequenceNumber";
    public const string SeriesId = "seriesId";
    public const string SeriesIds = "seriesIds";

    public const string Url = "url";
}

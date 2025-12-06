using Microsoft.AspNetCore.WebUtilities;

using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Cineaste.Problems;

internal class ProblemCustomizer(IHostEnvironment env)
{
    public void CustomizeProblemDetails(ProblemDetails problemDetails, Exception exception)
    {
        switch (exception)
        {
            case ValidationException ex:
                problemDetails.Type = this.CreateProblemType(Types.Validation);
                problemDetails.Title = "Validation has failed";
                problemDetails.Status = Status400BadRequest;
                problemDetails.Extensions[Props.Errors] = ex.Errors
                    .GroupBy(error => error.PropertyName, error => error.ErrorCode)
                    .ToDictionary(errors => errors.Key, errors => errors.ToList());
                break;
            case PosterException ex:
                this.CustomizeProblemDetails(problemDetails, ex);
                break;
            case NotFoundException ex:
                this.CustomizeProblemDetails(problemDetails, ex);
                break;
            case NotImplementedException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotImplemented);
                problemDetails.Title = "Not implemented";
                problemDetails.Status = Status501NotImplemented;
                break;
            default:
                this.CustomizeDefaultProblemDetails(problemDetails);
                break;
        }

        if (env.IsDevelopment())
        {
            problemDetails.Extensions.Add(Props.ExceptionDetails, new ExceptionDetails(exception));
        }

        int status = problemDetails.Status ?? Status500InternalServerError;
        problemDetails.Title ??= ReasonPhrases.GetReasonPhrase(status);

        problemDetails.Detail ??= exception.Message;
    }

    private void CustomizeProblemDetails(ProblemDetails problemDetails, PosterException exception)
    {
        switch (exception)
        {
            case PosterFetchException:
                problemDetails.Type = this.CreateProblemType(Types.Poster);
                problemDetails.Title = "Poster fetch error";
                problemDetails.Status = Status503ServiceUnavailable;
                break;
            case PosterFetchResponseException ex:
                problemDetails.Type = this.CreateProblemType(Types.Poster, Types.ErrorResponse);
                problemDetails.Title = "Unsuccessful response when fetching a poster";
                problemDetails.Status = Status503ServiceUnavailable;
                problemDetails.Extensions[Props.Response] = ex.Response;
                problemDetails.Extensions[Props.Url] = ex.Url;
                break;
            case UnsupportedPosterTypeException ex:
                problemDetails.Type = this.CreateProblemType(Types.Poster, Types.Type, Types.Unsupported);
                problemDetails.Title = "Unsupported poster type";
                problemDetails.Status = Status415UnsupportedMediaType;
                problemDetails.Extensions[Props.ContentType] = ex.ContentType;
                break;
            case NoPosterContentTypeException ex:
                problemDetails.Type = this.CreateProblemType(Types.Poster, Types.Type, Types.Missing);
                problemDetails.Title = "Missing poster content type";
                problemDetails.Status = Status503ServiceUnavailable;
                problemDetails.Extensions[Props.Url] = ex.Url;
                break;
            case NoPosterContentLengthException ex:
                problemDetails.Type = this.CreateProblemType(Types.Poster, Types.Length, Types.Missing);
                problemDetails.Title = "Missing poster content length";
                problemDetails.Status = Status503ServiceUnavailable;
                problemDetails.Extensions[Props.Url] = ex.Url;
                break;
            case ImdbMediaImageNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(
                    Types.Poster, Types.ImdbMedia, Types.Image, Types.NotFound);
                problemDetails.Title = "The image URL is not found in the IMDb media page";
                problemDetails.Status = Status503ServiceUnavailable;
                problemDetails.Extensions[Props.Url] = ex.Url;
                break;
            default:
                this.CustomizeDefaultProblemDetails(problemDetails);
                break;
        }
    }

    private void CustomizeProblemDetails(ProblemDetails problemDetails, NotFoundException exception)
    {
        switch (exception)
        {
            case ListNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.List);
                problemDetails.Title = "List not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.ListId;
                break;
            case ListItemNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.ListItem);
                problemDetails.Title = "List item not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.ItemId;
                break;
            case MovieNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Movie);
                problemDetails.Title = "Movie not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.MovieId;
                break;
            case MovieKindNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.MovieKind);
                problemDetails.Title = "Movie kind not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.KindId;
                break;
            case MoviePosterNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.Movie);
                problemDetails.Title = "Movie poster not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.MovieId] = ex.MovieId;
                break;
            case SeriesNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Series);
                problemDetails.Title = "Series not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.SeriesId;
                break;
            case SeriesKindNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.SeriesKind);
                problemDetails.Title = "Series kind not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.KindId;
                break;
            case SeriesPosterNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.Series);
                problemDetails.Title = "Series poster not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.SeriesId] = ex.SeriesId;
                break;
            case PeriodNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Series, Types.Period);
                problemDetails.Title = "Series period not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.PeriodId;
                break;
            case SeasonPosterNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.Season);
                problemDetails.Title = "Season poster not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.PeriodId] = ex.PeriodId;
                break;
            case SpecialEpisodeNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Series, Types.SpecialEpisode);
                problemDetails.Title = "Special episode not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.EpisodeId;
                break;
            case SpecialEpisodePosterNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.SpecialEpisode);
                problemDetails.Title = "Special episode poster not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.EpisodeId] = ex.EpisodeId;
                break;
            case FranchiseNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Franchise);
                problemDetails.Title = "Franchise not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.FranchiseId;
                break;
            case FranchiseItemNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.FranchiseItem);
                problemDetails.Title = "Franchise item not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.Id] = ex.ItemId;
                problemDetails.Extensions[Props.ItemType] = ex.ItemType;
                break;
            case FranchiseItemWithNumberNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.FranchiseItem);
                problemDetails.Title = "Franchise item not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.FranchiseId] = ex.FranchiseId;
                problemDetails.Extensions[Props.SequenceNumber] = ex.SequenceNumber;
                break;
            case FranchiseItemsNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.FranchiseItems);
                problemDetails.Title = "Franchise items not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.MovieIds] = ex.MovieIds;
                problemDetails.Extensions[Props.SeriesIds] = ex.SeriesIds;
                problemDetails.Extensions[Props.FranchiseIds] = ex.FranchiseIds;
                break;
            case FranchisePosterNotFoundException ex:
                problemDetails.Type = this.CreateProblemType(Types.NotFound, Types.Poster, Types.Franchise);
                problemDetails.Title = "Franchise poster not found";
                problemDetails.Status = Status404NotFound;
                problemDetails.Extensions[Props.FranchiseId] = ex.FranchiseId;
                break;
            default:
                this.CustomizeDefaultProblemDetails(problemDetails);
                break;
        }
    }

    private void CustomizeDefaultProblemDetails(ProblemDetails problemDetails)
    {
        problemDetails.Type = this.CreateProblemType(Types.Unhandled);
        problemDetails.Title = "Unhandled problem";
    }

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
    public const string NotImplemented = "not-implemented";

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
    public const string ExceptionDetails = "exceptionDetails";

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

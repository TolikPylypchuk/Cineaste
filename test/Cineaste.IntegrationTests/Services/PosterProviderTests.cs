using System.Net;

using AngleSharp;

using Cineaste.Models;
using Cineaste.Shared.Models.Poster;

using RichardSzalay.MockHttp;

namespace Cineaste.Services;

public sealed class PosterProviderTests(DataFixture data, ITestOutputHelper output)
{
    private readonly ILogger<PosterProvider> logger = XUnitLogger.Create<PosterProvider>(output);

    [Fact(DisplayName = "FetchPoster should fetch a poster from a URL")]
    public async Task FetchPosterShouldFetchPosterFromUrl()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var context = Substitute.For<IBrowsingContext>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), context, this.logger);

        var request = this.CreatePosterUrlRequest();

        var expectedContent = data.CreatePosterContent();

        this.SetUpHttp(mockHttp, request.Value.Url, HttpStatusCode.OK, expectedContent);

        // Act

        var actualContent = await provider.FetchPoster(request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(expectedContent.Type, actualContent.Type);
        Assert.Equal(expectedContent.Length, actualContent.Length);

        var expectedData = new byte[expectedContent.Length];
        expectedContent.GetStream().ReadExactly(expectedData);

        var actualData = new byte[actualContent.Length];
        expectedContent.GetStream().ReadExactly(actualData);

        Assert.Equal(expectedData, actualData);
    }

    [Fact(DisplayName = "FetchPoster should throw is response is not successful")]
    public async Task FetchPosterShouldThrowIfResponseNotSuccessful()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var context = Substitute.For<IBrowsingContext>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), context, this.logger);

        var request = this.CreatePosterUrlRequest();

        this.SetUpHttp(mockHttp, request.Value.Url, HttpStatusCode.NotFound);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<PosterFetchException>(() => provider.FetchPoster(
            request, TestContext.Current.CancellationToken));

        Assert.Equal("Poster.Fetch.UnsuccessfulResponse", exception.MessageCode);
    }

    [Fact(DisplayName = "FetchPoster should throw is response has no content type")]
    public async Task FetchPosterShouldThrowIfResponseHasNoContentType()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var context = Substitute.For<IBrowsingContext>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), context, this.logger);

        var request = this.CreatePosterUrlRequest();

        this.SetUpHttp(mockHttp, request.Value.Url, HttpStatusCode.OK, contentType: null);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<PosterFetchException>(() => provider.FetchPoster(
            request, TestContext.Current.CancellationToken));

        Assert.Equal("Poster.Fetch.NoContentType", exception.MessageCode);
    }

    [Fact(DisplayName = "FetchPoster should throw is response has no content length")]
    public async Task FetchPosterShouldThrowIfResponseHasNoContentLength()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var context = Substitute.For<IBrowsingContext>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), context, this.logger);

        var request = this.CreatePosterUrlRequest();

        this.SetUpHttp(
            mockHttp, request.Value.Url, HttpStatusCode.OK, contentType: DataFixture.PosterType, contentLength: null);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<PosterFetchException>(() => provider.FetchPoster(
            request, TestContext.Current.CancellationToken));

        Assert.Equal("Poster.Fetch.NoContentLength", exception.MessageCode);
    }

    [Theory(DisplayName = "FetchPoster should throw is response has invalid content type")]
    [InlineData("application/json")]
    [InlineData("application/octet-stream")]
    [InlineData("text/html")]
    [InlineData("text/plain")]
    public async Task FetchPosterShouldThrowIfResponseHasInvalidContentType(string contentType)
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var context = Substitute.For<IBrowsingContext>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), context, this.logger);

        var request = this.CreatePosterUrlRequest();

        this.SetUpHttp(
            mockHttp,
            request.Value.Url,
            HttpStatusCode.OK,
            contentType: contentType,
            contentLength: DataFixture.PosterData.Length);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<UnsupportedPosterTypeException>(() => provider.FetchPoster(
            request, TestContext.Current.CancellationToken));

        Assert.Equal("Poster.ContentType.Unsupported", exception.MessageCode);
        Assert.Equal(contentType, exception.Properties["contentType"]);
    }

    private void SetUpHttp(
        MockHttpMessageHandler mockHttp,
        string url,
        HttpStatusCode statusCode,
        StreamableContent content) =>
        this.SetUpHttp(mockHttp, url, statusCode, content.GetStream(), content.Type, content.Length);

    private void SetUpHttp(
        MockHttpMessageHandler mockHttp,
        string url,
        HttpStatusCode statusCode,
        Stream? stream = null,
        string? contentType = null,
        long? contentLength = null)
    {
        var content = new StreamContent(stream is not null && contentType is not null && contentLength is not null
            ? stream
            : Stream.Null);

        content.Headers.ContentType = contentType is not null ? new(contentType) : null;
        content.Headers.ContentLength = contentLength;

        mockHttp.When(HttpMethod.Get, url)
            .Respond(statusCode, content);
    }

    private Validated<PosterUrlRequest> CreatePosterUrlRequest() =>
        new PosterUrlRequest("https://tolik.io").Validated();
}

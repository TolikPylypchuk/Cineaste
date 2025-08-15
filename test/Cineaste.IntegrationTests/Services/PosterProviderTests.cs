using System.Net;

using AngleSharp.Dom;

using Cineaste.Models;
using Cineaste.Services.Poster;
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
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterUrlRequest();

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
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterUrlRequest();

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
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterUrlRequest();

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
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterUrlRequest();

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
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterUrlRequest();

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

    [Fact(DisplayName = "FetchPoster should throw is HttpClient cancels")]
    public async Task FetchPosterShouldThrowIfHttpClientCancels()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterUrlRequest();

        var expectedException = new OperationCanceledException();
        this.SetUpHttpException(mockHttp, request.Value.Url, expectedException);

        // Act + Assert

        var actualException = await Assert.ThrowsAsync<OperationCanceledException>(() => provider.FetchPoster(
            request, TestContext.Current.CancellationToken));

        Assert.Same(expectedException, actualException);
    }

    [Fact(DisplayName = "FetchPoster should throw is HttpClient throws")]
    public async Task FetchPosterShouldThrowIfHttpClientThrows()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterUrlRequest();

        var expectedException = new InvalidOperationException();
        this.SetUpHttpException(mockHttp, request.Value.Url, expectedException);

        // Act + Assert

        var actualException = await Assert.ThrowsAsync<PosterFetchException>(() => provider.FetchPoster(
            request, TestContext.Current.CancellationToken));

        Assert.Equal("Poster.Fetch.Error", actualException.MessageCode);
        Assert.Same(expectedException, actualException.InnerException);
    }

    [Fact(DisplayName = "GetPosterUrl should get a poster URL")]
    public async Task GetPosterUrlShouldGetPosterUrl()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterImdbMediaRequest();
        var expectedUrl = data.CreatePosterUrlRequest().Value.Url;

        var document = this.MockDocument(html, request);

        var image = Substitute.For<IElement>();
        document.QuerySelector(DataFixture.ImdbImageSelector).Returns(image);

        image.LocalName.Returns(DataFixture.Img);
        image.GetAttribute(DataFixture.Src).Returns(expectedUrl);

        // Act

        var actualUrl = await provider.GetPosterUrl(request, TestContext.Current.CancellationToken);

        // Assert

        Assert.Equal(expectedUrl, actualUrl);
    }

    [Fact(DisplayName = "GetPosterUrl should throw if the image element is not found")]
    public async Task GetPosterUrlShouldThrowIfImageElementNotFound()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterImdbMediaRequest();
        var expectedUrl = data.CreatePosterUrlRequest().Value.Url;

        var document = this.MockDocument(html, request);

        document.QuerySelector(DataFixture.ImdbImageSelector).Returns((IElement?)null);

        // Act

        var exception = await Assert.ThrowsAsync<PosterFetchException>(() => provider.GetPosterUrl(
            request, TestContext.Current.CancellationToken));

        // Assert

        Assert.Equal("Poster.Fetch.Imdb.Media.PosterNotFound", exception.MessageCode);
        Assert.Equal(request.Value.Url, exception.Properties["url"]);
    }

    [Fact(DisplayName = "GetPosterUrl should throw if the element is not an image")]
    public async Task GetPosterUrlShouldThrowIfNotImageElement()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterImdbMediaRequest();
        var expectedUrl = data.CreatePosterUrlRequest().Value.Url;

        var document = this.MockDocument(html, request);

        var image = Substitute.For<IElement>();
        document.QuerySelector(DataFixture.ImdbImageSelector).Returns(image);

        image.LocalName.Returns("p");

        // Act

        var exception = await Assert.ThrowsAsync<PosterFetchException>(() => provider.GetPosterUrl(
            request, TestContext.Current.CancellationToken));

        // Assert

        Assert.Equal("Poster.Fetch.Imdb.Media.PosterNotFound", exception.MessageCode);
        Assert.Equal(request.Value.Url, exception.Properties["url"]);
    }

    [Fact(DisplayName = "GetPosterUrl should throw if the image has no source")]
    public async Task GetPosterUrlShouldThrowIfImageHasNoSource()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterImdbMediaRequest();
        var expectedUrl = data.CreatePosterUrlRequest().Value.Url;

        var document = this.MockDocument(html, request);

        var image = Substitute.For<IElement>();
        document.QuerySelector(DataFixture.ImdbImageSelector).Returns(image);

        image.LocalName.Returns(DataFixture.Img);
        image.GetAttribute(DataFixture.Src).Returns((string?)null);

        // Act

        var exception = await Assert.ThrowsAsync<PosterFetchException>(() => provider.GetPosterUrl(
            request, TestContext.Current.CancellationToken));

        // Assert

        Assert.Equal("Poster.Fetch.Imdb.Media.PosterNotFound", exception.MessageCode);
        Assert.Equal(request.Value.Url, exception.Properties["url"]);
    }

    [Fact(DisplayName = "GetPosterUrl should throw is the HTML provider cancels")]
    public async Task GetPosterUrlShouldThrowIfHtmlProviderCancels()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterImdbMediaRequest();

        var expectedException = new OperationCanceledException();
        html.GetDocument(request.Value.Url, TestContext.Current.CancellationToken).ThrowsAsync(expectedException);

        // Act + Assert

        var actualException = await Assert.ThrowsAsync<OperationCanceledException>(() => provider.GetPosterUrl(
            request, TestContext.Current.CancellationToken));

        Assert.Same(expectedException, actualException);
    }

    [Fact(DisplayName = "GetPosterUrl should throw is the HTML provider throws")]
    public async Task GetPosterUrlShouldThrowIfHtmlProviderThrows()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterImdbMediaRequest();

        var expectedException = new InvalidOperationException();
        html.GetDocument(request.Value.Url, TestContext.Current.CancellationToken).ThrowsAsync(expectedException);

        // Act + Assert

        var actualException = await Assert.ThrowsAsync<PosterFetchException>(() => provider.GetPosterUrl(
            request, TestContext.Current.CancellationToken));

        Assert.Equal("Poster.Fetch.Error", actualException.MessageCode);
        Assert.Same(expectedException, actualException.InnerException);
    }

    [Fact(DisplayName = "FetchPoster should fetch a poster from IMDb media")]
    public async Task FetchPosterShouldFetchPosterFromImdbMedia()
    {
        // Arrange

        var mockHttp = new MockHttpMessageHandler();
        var html = Substitute.For<IHtmlDocumentProvider>();

        var provider = new PosterProvider(mockHttp.ToHttpClient(), html, this.logger);

        var request = data.CreatePosterImdbMediaRequest();

        var expectedContent = data.CreatePosterContent();
        var expectedUrl = data.CreatePosterUrlRequest().Value.Url;

        this.SetUpHttp(mockHttp, expectedUrl, HttpStatusCode.OK, expectedContent);

        var document = this.MockDocument(html, request);

        var image = Substitute.For<IElement>();
        document.QuerySelector(DataFixture.ImdbImageSelector).Returns(image);

        image.LocalName.Returns(DataFixture.Img);
        image.GetAttribute(DataFixture.Src).Returns(expectedUrl);

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

    private void SetUpHttpException(
        MockHttpMessageHandler mockHttp,
        string url,
        Exception exception) =>
        mockHttp.When(HttpMethod.Get, url)
            .Respond(req =>
            {
                throw exception;
            });

    private IDocument MockDocument(IHtmlDocumentProvider html, Validated<PosterImdbMediaRequest> request)
    {
        var document = Substitute.For<IDocument>();

        html.GetDocument(request.Value.Url, TestContext.Current.CancellationToken)
            .Returns(Task.FromResult(document));

        return document;
    }
}

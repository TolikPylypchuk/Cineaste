using AngleSharp;
using AngleSharp.Dom;

using IConfiguration = AngleSharp.IConfiguration;

namespace Cineaste.Application.Services.Poster;

public interface IHtmlDocumentProvider
{
    Task<IDocument> GetDocument(string url, CancellationToken token = default);
}

[ExcludeFromCodeCoverage]
public class HtmlDocumentProvider : IHtmlDocumentProvider
{
    private readonly IConfiguration config = Configuration.Default.WithDefaultLoader();

    public Task<IDocument> GetDocument(string url, CancellationToken token = default)
    {
        using var context = BrowsingContext.New(config);
        return context.OpenAsync(url, token);
    }
}

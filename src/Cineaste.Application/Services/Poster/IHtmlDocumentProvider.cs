using AngleSharp.Dom;

namespace Cineaste.Application.Services.Poster;

public interface IHtmlDocumentProvider
{
    Task<IDocument> GetDocument(string url, CancellationToken token = default);
}

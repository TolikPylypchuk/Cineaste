namespace Cineaste.Data.Services;

public interface IListService
{
    DataList GetList(IEnumerable<Kind> kinds, IEnumerable<Tag> tags);
}

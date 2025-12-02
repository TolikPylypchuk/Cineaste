namespace Cineaste.Application.Services;

public static class AssertUtils
{
    public static void AssertTitles<TEntity>(TitledEntity<TEntity> entity, ITitledModel model)
        where TEntity : TitledEntity<TEntity>
    {
        AssertTitles(entity.AllTitles, original: false, model.Titles);
        AssertTitles(entity.AllTitles, original: true, model.OriginalTitles);
    }

    public static void AssertTitles<TEntity>(ITitledRequest request, TitledEntity<TEntity> entity)
        where TEntity : TitledEntity<TEntity>
    {
        AssertTitles(request.Titles, entity.AllTitles, original: false);
        AssertTitles(request.OriginalTitles, entity.AllTitles, original: true);
    }

    public static void AssertTitles(ITitledRequest request, ITitledModel model)
    {
        AssertTitles(request.Titles, model.Titles);
        AssertTitles(request.OriginalTitles, model.OriginalTitles);
    }

    private static void AssertTitles(
        IEnumerable<Title> expectedTitles,
        bool original,
        IEnumerable<TitleModel> actualTitles) =>
        Assert.Equal(
            from title in expectedTitles
            where title.IsOriginal == original
            orderby title.SequenceNumber
            select title.Name,
            from title in actualTitles
            orderby title.SequenceNumber
            select title.Name);

    private static void AssertTitles(
        IEnumerable<TitleRequest> expectedTitles,
        IEnumerable<Title> actualTitles,
        bool original) =>
        Assert.Equal(
            from title in expectedTitles
            orderby title.SequenceNumber
            select title.Name,
            from title in actualTitles
            where title.IsOriginal == original
            orderby title.SequenceNumber
            select title.Name);

    private static void AssertTitles(
        IEnumerable<TitleRequest> expectedTitles,
        IEnumerable<TitleModel> actualTitles) =>
        Assert.Equal(
            from title in expectedTitles
            orderby title.SequenceNumber
            select title.Name,
            from title in actualTitles
            orderby title.SequenceNumber
            select title.Name);
}

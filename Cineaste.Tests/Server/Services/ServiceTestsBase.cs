namespace Cineaste.Server.Services;

using System.Runtime.CompilerServices;

using Cineaste.Core.Domain;
using Cineaste.Persistence;
using Cineaste.Shared.Models;
using Cineaste.Shared.Models.Shared;

using Microsoft.EntityFrameworkCore;

public abstract class ServiceTestsBase
{
    protected CineasteDbContext CreateInMemoryDb([CallerMemberName] string dbName = "")
    {
        var options = new DbContextOptionsBuilder<CineasteDbContext>()
            .UseInMemoryDatabase(databaseName: this.GetType().Name + "." + dbName)
            .Options;

        var context = new CineasteDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    protected void AssertTitles<TEntity>(TitledEntity<TEntity> entity, ITitledModel model)
        where TEntity : TitledEntity<TEntity>
    {
        this.AssertTitles(entity.Titles, original: false, model.Titles);
        this.AssertTitles(entity.Titles, original: true, model.OriginalTitles);
    }

    protected void AssertTitles<TEntity>(ITitledRequest request, TitledEntity<TEntity> entity)
        where TEntity : TitledEntity<TEntity>
    {
        this.AssertTitles(request.Titles, entity.Titles, original: false);
        this.AssertTitles(request.OriginalTitles, entity.Titles, original: true);
    }

    protected void AssertTitles(ITitledRequest request, ITitledModel model)
    {
        this.AssertTitles(request.Titles, model.Titles);
        this.AssertTitles(request.OriginalTitles, model.OriginalTitles);
    }

    private void AssertTitles(
        IEnumerable<Title> expectedTitles,
        bool original,
        IEnumerable<TitleModel> actualTitles) =>
        Assert.Equal(
            from title in expectedTitles
            where title.IsOriginal == original
            orderby title.Priority
            select title.Name,
            from title in actualTitles
            orderby title.Priority
            select title.Name);

    private void AssertTitles(
        IEnumerable<TitleRequest> expectedTitles,
        IEnumerable<Title> actualTitles,
        bool original) =>
        Assert.Equal(
            from title in expectedTitles
            orderby title.Priority
            select title.Name,
            from title in actualTitles
            where title.IsOriginal == original
            orderby title.Priority
            select title.Name);

    private void AssertTitles(
        IEnumerable<TitleRequest> expectedTitles,
        IEnumerable<TitleModel> actualTitles) =>
        Assert.Equal(
            from title in expectedTitles
            orderby title.Priority
            select title.Name,
            from title in actualTitles
            orderby title.Priority
            select title.Name);
}

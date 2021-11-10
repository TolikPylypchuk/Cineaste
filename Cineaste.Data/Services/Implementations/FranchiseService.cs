namespace Cineaste.Data.Services.Implementations;

internal class FranchiseService : EntityServiceBase<Franchise>
{
    public FranchiseService(string file)
        : base(file)
    { }

    protected override void Insert(Franchise franchise, IDbConnection connection, IDbTransaction transaction)
    {
        franchise.Id = (int)connection.Insert(franchise, transaction);

        foreach (var title in franchise.Titles)
        {
            title.FranchiseId = franchise.Id;
            title.Id = (int)connection.Insert(title, transaction);
        }

        this.InsertEntries(franchise.Entries, franchise.Id, connection, transaction);

        if (franchise.Entry != null)
        {
            var entry = franchise.Entry;
            entry.FranchiseId = franchise.Id;
            entry.ParentFranchiseId = entry.ParentFranchise.Id;
            entry.Id = (int)connection.Insert(entry, transaction);
            entry.ParentFranchise.Entries.Add(entry);

            this.UpdateMergedDisplayNumbers(franchise);
        }
    }

    protected override void Update(Franchise franchise, IDbConnection connection, IDbTransaction transaction)
    {
        connection.Update(franchise, transaction);

        if (franchise.Entry != null)
        {
            connection.Update(franchise.Entry, transaction);
            this.UpdateMergedDisplayNumbers(franchise);
        }

        var updater = new DependentEntityUpdater(connection, transaction);

        updater.UpdateDependentEntities(
            franchise,
            franchise.Titles,
            title => title.FranchiseId,
            (title, franchiseId) => title.FranchiseId = franchiseId);

        this.InsertEntries(
            franchise.Entries.Where(entry => entry.Id == default), franchise.Id, connection, transaction);

        updater.UpdateDependentEntities(
            franchise,
            franchise.Entries,
            entry => entry.ParentFranchiseId,
            (entry, franchiseId) => entry.ParentFranchiseId = franchiseId);
    }

    protected override void Delete(Franchise franchise, IDbConnection connection, IDbTransaction transaction)
    {
        connection.DeleteAsync(franchise.Titles, transaction);
        connection.DeleteAsync(franchise.Entries, transaction);

        foreach (var entry in franchise.Entries)
        {
            if (entry.Movie != null)
            {
                entry.Movie.Entry = null;
            } else if (entry.Series != null)
            {
                entry.Series.Entry = null;
            } else if (entry.Franchise != null)
            {
                entry.Franchise.Entry = null;
            }
        }

        if (franchise.Entry != null)
        {
            this.DeleteFranchiseEntry(franchise.Entry, connection, transaction);
        }

        connection.Delete(franchise, transaction);
    }

    private void InsertEntries(
        IEnumerable<FranchiseEntry> entries,
        int franchiseId,
        IDbConnection connection,
        IDbTransaction transaction)
    {
        foreach (var entry in entries)
        {
            entry.ParentFranchiseId = franchiseId;

            if (entry.Movie != null)
            {
                entry.MovieId = entry.Movie.Id;
            } else if (entry.Series != null)
            {
                entry.SeriesId = entry.Series.Id;
            } else if (entry.Franchise != null)
            {
                entry.FranchiseId = entry.Franchise.Id;
            }

            entry.Id = (int)connection.Insert(entry, transaction);

            if (entry.Movie != null)
            {
                entry.Movie.Entry = entry;
            } else if (entry.Series != null)
            {
                entry.Series.Entry = entry;
            } else if (entry.Franchise != null)
            {
                entry.Franchise.Entry = entry;
            }
        }
    }
}

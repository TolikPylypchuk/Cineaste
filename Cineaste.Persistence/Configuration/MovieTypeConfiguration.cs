namespace Cineaste.Persistence.Configuration;

internal class MovieTypeConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> movie)
    {
        movie.HasStronglyTypedId();
        movie.HasCheckConstraint("CH_Movies_YearPositive", "Year > 0");

        movie.OwnsMany(
            m => m.Titles,
            title =>
            {
                title.ToTable("MovieTitles");
                title.Property(t => t.Name);
                title.Property(t => t.Priority);
                title.Property(t => t.IsOriginal);

                title.HasCheckConstraint("CH_MovieTitles_NameNonEmpty", "Name <> ''");
                title.HasCheckConstraint("CH_MovieTitles_PriorityPositive", "Priority > 0");
            })
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        movie.OwnsOne(
            m => m.Poster,
            poster => poster.Property(p => p.RawData).HasColumnName(nameof(Movie.Poster)));

        movie.Navigation(m => m.Poster).AutoInclude(false);

        movie.Ignore(m => m.OwnerList);
        movie.Ignore(m => m.Kind);
        movie.Ignore(m => m.Tags);
        movie.Ignore(m => m.FranchiseItem);
    }
}

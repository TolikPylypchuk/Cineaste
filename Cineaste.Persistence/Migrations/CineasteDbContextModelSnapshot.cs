﻿// <auto-generated />
using System;
using Cineaste.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cineaste.Persistence.Migrations
{
    [DbContext(typeof(CineasteDbContext))]
    partial class CineasteDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Cineaste.Core.Domain.CineasteList", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Handle")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Handle")
                        .IsUnique();

                    b.ToTable("Lists", t =>
                        {
                            t.HasCheckConstraint("CH_Lists_HandleNotEmpty", "Handle <> ''");

                            t.HasCheckConstraint("CH_Lists_NameNotEmpty", "Name <> ''");
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Franchise", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("ContinueNumbering")
                        .HasColumnType("bit");

                    b.Property<Guid?>("FranchiseItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsLooselyConnected")
                        .HasColumnType("bit");

                    b.Property<Guid>("ListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("ShowTitles")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("FranchiseItemId")
                        .IsUnique()
                        .HasFilter("[FranchiseItemId] IS NOT NULL");

                    b.HasIndex("ListId");

                    b.ToTable("Franchises");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.FranchiseItem", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ParentFranchiseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SequenceNumber")
                        .HasColumnType("int");

                    b.Property<bool>("ShouldDisplayNumber")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ParentFranchiseId");

                    b.ToTable("FranchiseItems", t =>
                        {
                            t.HasCheckConstraint("CH_FranchiseItems_SequenceNumberPositive", "SequenceNumber > 0");
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.ListConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DefaultSeasonOriginalTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DefaultSeasonTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ListId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ListId")
                        .IsUnique();

                    b.ToTable("ListConfigurations");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Movie", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("FranchiseItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImdbId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsReleased")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWatched")
                        .HasColumnType("bit");

                    b.Property<Guid>("KindId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RottenTomatoesId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FranchiseItemId")
                        .IsUnique()
                        .HasFilter("[FranchiseItemId] IS NOT NULL");

                    b.HasIndex("KindId");

                    b.HasIndex("ListId");

                    b.ToTable("Movies", t =>
                        {
                            t.HasCheckConstraint("CH_Movies_YearPositive", "Year > 0");
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.MovieKind", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NotReleasedColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NotWatchedColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WatchedColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ListId");

                    b.HasIndex("Name", "ListId")
                        .IsUnique();

                    b.ToTable("MovieKinds", t =>
                        {
                            t.HasCheckConstraint("CH_MovieKinds_NameNotEmpty", "Name <> ''");
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Period", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EndMonth")
                        .HasColumnType("int");

                    b.Property<int>("EndYear")
                        .HasColumnType("int");

                    b.Property<int>("EpisodeCount")
                        .HasColumnType("int");

                    b.Property<bool>("IsSingleDayRelease")
                        .HasColumnType("bit");

                    b.Property<string>("RottenTomatoesId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("StartMonth")
                        .HasColumnType("int");

                    b.Property<int>("StartYear")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SeasonId");

                    b.ToTable("Periods", t =>
                        {
                            t.HasCheckConstraint("CH_Periods_EndMonthValid", "StartMonth >= 1 AND StartMonth <= 12");

                            t.HasCheckConstraint("CH_Periods_EndYearPositive", "EndYear > 0");

                            t.HasCheckConstraint("CH_Periods_EpisodeCountPositive", "EndYear > 0");

                            t.HasCheckConstraint("CH_Periods_PeriodValid", "DATEFROMPARTS(StartYear, StartMonth, 1) <= DATEFROMPARTS(EndYear, EndMonth, 1)");

                            t.HasCheckConstraint("CH_Periods_StartMonthValid", "StartMonth >= 1 AND StartMonth <= 12");

                            t.HasCheckConstraint("CH_Periods_StartYearPositive", "StartYear > 0");
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Season", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Channel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReleaseStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SequenceNumber")
                        .HasColumnType("int");

                    b.Property<Guid?>("SeriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("WatchStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SeriesId");

                    b.ToTable("Seasons", t =>
                        {
                            t.HasCheckConstraint("CH_Seasons_ChannelNotEmpty", "Channel <> ''");

                            t.HasCheckConstraint("CH_Seasons_SequenceNumberPositive", "SequenceNumber > 0");
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Series", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("FranchiseItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImdbId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("KindId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ReleaseStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RottenTomatoesId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WatchStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FranchiseItemId")
                        .IsUnique()
                        .HasFilter("[FranchiseItemId] IS NOT NULL");

                    b.HasIndex("KindId");

                    b.HasIndex("ListId");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.SeriesKind", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NotReleasedColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NotWatchedColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WatchedColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ListId");

                    b.HasIndex("Name", "ListId")
                        .IsUnique();

                    b.ToTable("SeriesKinds", t =>
                        {
                            t.HasCheckConstraint("CH_SeriesKinds_NameNotEmpty", "Name <> ''");
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.SpecialEpisode", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Channel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsReleased")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWatched")
                        .HasColumnType("bit");

                    b.Property<int>("Month")
                        .HasColumnType("int");

                    b.Property<string>("RottenTomatoesId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SequenceNumber")
                        .HasColumnType("int");

                    b.Property<Guid?>("SeriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SeriesId");

                    b.ToTable("SpecialEpisodes", t =>
                        {
                            t.HasCheckConstraint("CH_SpecialEpisodes_ChannelNotEmpty", "Channel <> ''");

                            t.HasCheckConstraint("CH_SpecialEpisodes_MonthValid", "Month >= 1 AND Month <= 12");

                            t.HasCheckConstraint("CH_SpecialEpisodes_SequenceNumberPositive", "SequenceNumber > 0");

                            t.HasCheckConstraint("CH_SpecialEpisodes_YearPositive", "Year > 0");
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsApplicableToMovies")
                        .HasColumnType("bit");

                    b.Property<bool>("IsApplicableToSeries")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ListId");

                    b.HasIndex("Name", "Category")
                        .IsUnique();

                    b.ToTable("Tags", t =>
                        {
                            t.HasCheckConstraint("CH_Tag_CategoryNotEmpty", "Category <> ''");

                            t.HasCheckConstraint("CH_Tag_NameNotEmpty", "Name <> ''");
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.TagImplication", b =>
                {
                    b.Property<Guid>("ImpliedTagId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ImplyingTagId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ImpliedTagId", "ImplyingTagId");

                    b.HasIndex("ImplyingTagId");

                    b.ToTable("TagImplications", (string)null);
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Franchise", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.FranchiseItem", "FranchiseItem")
                        .WithOne("Franchise")
                        .HasForeignKey("Cineaste.Core.Domain.Franchise", "FranchiseItemId");

                    b.HasOne("Cineaste.Core.Domain.CineasteList", null)
                        .WithMany("Franchises")
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Cineaste.Core.Domain.Poster", "Poster", b1 =>
                        {
                            b1.Property<Guid>("FranchiseId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<byte[]>("RawData")
                                .IsRequired()
                                .HasColumnType("varbinary(max)")
                                .HasColumnName("Poster");

                            b1.HasKey("FranchiseId");

                            b1.ToTable("Franchises");

                            b1.WithOwner()
                                .HasForeignKey("FranchiseId");
                        });

                    b.OwnsMany("Cineaste.Core.Domain.Title", "Titles", b1 =>
                        {
                            b1.Property<Guid>("FranchiseId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("FranchiseId", "Id");

                            b1.ToTable("FranchiseTitles", null, t =>
                                {
                                    t.HasCheckConstraint("CH_FranchiseTitles_NameNotEmpty", "Name <> ''");

                                    t.HasCheckConstraint("CH_FranchiseTitles_PriorityPositive", "Priority > 0");
                                });

                            b1.WithOwner()
                                .HasForeignKey("FranchiseId");
                        });

                    b.Navigation("FranchiseItem");

                    b.Navigation("Poster");

                    b.Navigation("Titles");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.FranchiseItem", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.Franchise", "ParentFranchise")
                        .WithMany("Children")
                        .HasForeignKey("ParentFranchiseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentFranchise");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.ListConfiguration", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.CineasteList", null)
                        .WithOne("Configuration")
                        .HasForeignKey("Cineaste.Core.Domain.ListConfiguration", "ListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Cineaste.Core.Domain.ListSortingConfiguration", "SortingConfiguration", b1 =>
                        {
                            b1.Property<Guid>("ListConfigurationId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("DefaultFirstSortDirection")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("DefaultFirstSortDirection");

                            b1.Property<string>("DefaultFirstSortOrder")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("DefaultFirstSortOrder");

                            b1.Property<string>("DefaultSecondSortDirection")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("DefaultSecondSortDirection");

                            b1.Property<string>("DefaultSecondSortOrder")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("DefaultSecondSortOrder");

                            b1.HasKey("ListConfigurationId");

                            b1.ToTable("ListConfigurations");

                            b1.WithOwner()
                                .HasForeignKey("ListConfigurationId");
                        });

                    b.Navigation("SortingConfiguration")
                        .IsRequired();
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Movie", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.FranchiseItem", "FranchiseItem")
                        .WithOne("Movie")
                        .HasForeignKey("Cineaste.Core.Domain.Movie", "FranchiseItemId");

                    b.HasOne("Cineaste.Core.Domain.MovieKind", "Kind")
                        .WithMany()
                        .HasForeignKey("KindId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Cineaste.Core.Domain.CineasteList", null)
                        .WithMany("Movies")
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Cineaste.Core.Domain.Poster", "Poster", b1 =>
                        {
                            b1.Property<Guid>("MovieId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<byte[]>("RawData")
                                .IsRequired()
                                .HasColumnType("varbinary(max)")
                                .HasColumnName("Poster");

                            b1.HasKey("MovieId");

                            b1.ToTable("Movies");

                            b1.WithOwner()
                                .HasForeignKey("MovieId");
                        });

                    b.OwnsMany("Cineaste.Core.Domain.TagContainer", "Tags", b1 =>
                        {
                            b1.Property<Guid>("MovieId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<Guid>("TagId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("MovieId", "Id");

                            b1.HasIndex("TagId");

                            b1.ToTable("MovieTags", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("MovieId");

                            b1.HasOne("Cineaste.Core.Domain.Tag", "Tag")
                                .WithMany()
                                .HasForeignKey("TagId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.Navigation("Tag");
                        });

                    b.OwnsMany("Cineaste.Core.Domain.Title", "Titles", b1 =>
                        {
                            b1.Property<Guid>("MovieId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("MovieId", "Id");

                            b1.ToTable("MovieTitles", null, t =>
                                {
                                    t.HasCheckConstraint("CH_MovieTitles_NameNotEmpty", "Name <> ''");

                                    t.HasCheckConstraint("CH_MovieTitles_PriorityPositive", "Priority > 0");
                                });

                            b1.WithOwner()
                                .HasForeignKey("MovieId");
                        });

                    b.Navigation("FranchiseItem");

                    b.Navigation("Kind");

                    b.Navigation("Poster");

                    b.Navigation("Tags");

                    b.Navigation("Titles");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.MovieKind", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.CineasteList", null)
                        .WithMany("MovieKinds")
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Period", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.Season", null)
                        .WithMany("Periods")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("Cineaste.Core.Domain.Poster", "Poster", b1 =>
                        {
                            b1.Property<Guid>("PeriodId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<byte[]>("RawData")
                                .IsRequired()
                                .HasColumnType("varbinary(max)")
                                .HasColumnName("Poster");

                            b1.HasKey("PeriodId");

                            b1.ToTable("Periods");

                            b1.WithOwner()
                                .HasForeignKey("PeriodId");
                        });

                    b.Navigation("Poster");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Season", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.Series", null)
                        .WithMany("Seasons")
                        .HasForeignKey("SeriesId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsMany("Cineaste.Core.Domain.Title", "Titles", b1 =>
                        {
                            b1.Property<Guid>("SeasonId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("SeasonId", "Id");

                            b1.ToTable("SeasonTitles", null, t =>
                                {
                                    t.HasCheckConstraint("CH_SeasonTitles_NameNotEmpty", "Name <> ''");

                                    t.HasCheckConstraint("CH_SeasonTitles_PriorityPositive", "Priority > 0");
                                });

                            b1.WithOwner()
                                .HasForeignKey("SeasonId");
                        });

                    b.Navigation("Titles");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Series", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.FranchiseItem", "FranchiseItem")
                        .WithOne("Series")
                        .HasForeignKey("Cineaste.Core.Domain.Series", "FranchiseItemId");

                    b.HasOne("Cineaste.Core.Domain.SeriesKind", "Kind")
                        .WithMany()
                        .HasForeignKey("KindId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Cineaste.Core.Domain.CineasteList", null)
                        .WithMany("Series")
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Cineaste.Core.Domain.Poster", "Poster", b1 =>
                        {
                            b1.Property<Guid>("SeriesId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<byte[]>("RawData")
                                .IsRequired()
                                .HasColumnType("varbinary(max)")
                                .HasColumnName("Poster");

                            b1.HasKey("SeriesId");

                            b1.ToTable("Series");

                            b1.WithOwner()
                                .HasForeignKey("SeriesId");
                        });

                    b.OwnsMany("Cineaste.Core.Domain.TagContainer", "Tags", b1 =>
                        {
                            b1.Property<Guid>("SeriesId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<Guid>("TagId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("SeriesId", "Id");

                            b1.HasIndex("TagId");

                            b1.ToTable("SeriesTags", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("SeriesId");

                            b1.HasOne("Cineaste.Core.Domain.Tag", "Tag")
                                .WithMany()
                                .HasForeignKey("TagId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.Navigation("Tag");
                        });

                    b.OwnsMany("Cineaste.Core.Domain.Title", "Titles", b1 =>
                        {
                            b1.Property<Guid>("SeriesId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("SeriesId", "Id");

                            b1.ToTable("SeriesTitles", null, t =>
                                {
                                    t.HasCheckConstraint("CH_SeriesTitles_NameNotEmpty", "Name <> ''");

                                    t.HasCheckConstraint("CH_SeriesTitles_PriorityPositive", "Priority > 0");
                                });

                            b1.WithOwner()
                                .HasForeignKey("SeriesId");
                        });

                    b.Navigation("FranchiseItem");

                    b.Navigation("Kind");

                    b.Navigation("Poster");

                    b.Navigation("Tags");

                    b.Navigation("Titles");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.SeriesKind", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.CineasteList", null)
                        .WithMany("SeriesKinds")
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Cineaste.Core.Domain.SpecialEpisode", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.Series", null)
                        .WithMany("SpecialEpisodes")
                        .HasForeignKey("SeriesId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("Cineaste.Core.Domain.Poster", "Poster", b1 =>
                        {
                            b1.Property<Guid>("SpecialEpisodeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<byte[]>("RawData")
                                .IsRequired()
                                .HasColumnType("varbinary(max)")
                                .HasColumnName("Poster");

                            b1.HasKey("SpecialEpisodeId");

                            b1.ToTable("SpecialEpisodes");

                            b1.WithOwner()
                                .HasForeignKey("SpecialEpisodeId");
                        });

                    b.OwnsMany("Cineaste.Core.Domain.Title", "Titles", b1 =>
                        {
                            b1.Property<Guid>("SpecialEpisodeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("SpecialEpisodeId", "Id");

                            b1.ToTable("SpecialEpisodeTitles", null, t =>
                                {
                                    t.HasCheckConstraint("CH_SpecialEpisodeTitles_NameNotEmpty", "Name <> ''");

                                    t.HasCheckConstraint("CH_SpecialEpisodeTitles_PriorityPositive", "Priority > 0");
                                });

                            b1.WithOwner()
                                .HasForeignKey("SpecialEpisodeId");
                        });

                    b.Navigation("Poster");

                    b.Navigation("Titles");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Tag", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.CineasteList", null)
                        .WithMany("Tags")
                        .HasForeignKey("ListId");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.TagImplication", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.Tag", null)
                        .WithMany()
                        .HasForeignKey("ImpliedTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cineaste.Core.Domain.Tag", null)
                        .WithMany()
                        .HasForeignKey("ImplyingTagId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Cineaste.Core.Domain.CineasteList", b =>
                {
                    b.Navigation("Configuration")
                        .IsRequired();

                    b.Navigation("Franchises");

                    b.Navigation("MovieKinds");

                    b.Navigation("Movies");

                    b.Navigation("Series");

                    b.Navigation("SeriesKinds");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Franchise", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.FranchiseItem", b =>
                {
                    b.Navigation("Franchise");

                    b.Navigation("Movie");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Season", b =>
                {
                    b.Navigation("Periods");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Series", b =>
                {
                    b.Navigation("Seasons");

                    b.Navigation("SpecialEpisodes");
                });
#pragma warning restore 612, 618
        }
    }
}

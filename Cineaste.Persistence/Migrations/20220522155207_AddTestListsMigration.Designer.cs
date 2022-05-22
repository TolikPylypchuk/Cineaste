﻿// <auto-generated />
using System;
using Cineaste.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cineaste.Persistence.Migrations
{
    [DbContext(typeof(CineasteDbContext))]
    [Migration("20220522155207_AddTestListsMigration")]
    partial class AddTestListsMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Cineaste.Core.Domain.CineasteList", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Lists");

                    b.HasCheckConstraint("CH_Lists_NameNotEmpty", "Name <> ''");

                    b.HasData(
                        new
                        {
                            Id = new Guid("f93ab0d0-189b-4806-850f-e129bd5af30a"),
                            Name = "Test List 1"
                        },
                        new
                        {
                            Id = new Guid("3770d3f7-47b3-4865-9819-11268c9b965f"),
                            Name = "Test List 2"
                        },
                        new
                        {
                            Id = new Guid("04071a9e-17ed-4aa0-855e-7c89ed16921b"),
                            Name = "Test List 3"
                        },
                        new
                        {
                            Id = new Guid("45db8370-09d6-43bf-80e5-6818175189d6"),
                            Name = "Test List 4"
                        },
                        new
                        {
                            Id = new Guid("c7c2dc17-dbb0-4506-989a-30a7904fd5ba"),
                            Name = "Test List 5"
                        });
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Franchise", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FranchiseItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsLooselyConnected")
                        .HasColumnType("bit");

                    b.Property<Guid>("ListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("MergeDisplayNumbers")
                        .HasColumnType("bit");

                    b.Property<bool>("ShowTitles")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("FranchiseItemId")
                        .IsUnique();

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

                    b.ToTable("FranchiseItems");

                    b.HasCheckConstraint("CH_FranchiseItems_SequenceNumberPositive", "SequenceNumber > 0");
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

                    b.Property<Guid>("FranchiseItemId")
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

                    b.Property<string>("RottenTomatoesLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FranchiseItemId")
                        .IsUnique();

                    b.HasIndex("KindId");

                    b.HasIndex("ListId");

                    b.ToTable("Movies");

                    b.HasCheckConstraint("CH_Movies_YearPositive", "Year > 0");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.MovieKind", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ListId")
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

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("MovieKinds");

                    b.HasCheckConstraint("CH_MovieKinds_NameNotEmpty", "Name <> ''");
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

                    b.Property<string>("RottenTomatoesLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SeasonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("StartMonth")
                        .HasColumnType("int");

                    b.Property<int>("StartYear")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SeasonId");

                    b.ToTable("Periods");

                    b.HasCheckConstraint("CH_Periods_EndMonthValid", "StartMonth >= 1 AND StartMonth <= 12");

                    b.HasCheckConstraint("CH_Periods_EndYearPositive", "EndYear > 0");

                    b.HasCheckConstraint("CH_Periods_EpisodeCountPositive", "EndYear > 0");

                    b.HasCheckConstraint("CH_Periods_PeriodValid", "DATEFROMPARTS(StartYear, StartMonth, 1) <= DATEFROMPARTS(EndYear, EndMonth, 1)");

                    b.HasCheckConstraint("CH_Periods_StartMonthValid", "StartMonth >= 1 AND StartMonth <= 12");

                    b.HasCheckConstraint("CH_Periods_StartYearPositive", "StartYear > 0");
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

                    b.ToTable("Seasons");

                    b.HasCheckConstraint("CH_Seasons_ChannelNotEmpty", "Channel <> ''");

                    b.HasCheckConstraint("CH_Seasons_SequenceNumberPositive", "SequenceNumber > 0");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Series", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FranchiseItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImdbId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsMiniseries")
                        .HasColumnType("bit");

                    b.Property<Guid>("KindId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ListId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ReleaseStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RottenTomatoesLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WatchStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FranchiseItemId")
                        .IsUnique();

                    b.HasIndex("KindId");

                    b.HasIndex("ListId");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.SeriesKind", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ListId")
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

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("SeriesKinds");

                    b.HasCheckConstraint("CH_SeriesKinds_NameNotEmpty", "Name <> ''");
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

                    b.Property<string>("RottenTomatoesLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SequenceNumber")
                        .HasColumnType("int");

                    b.Property<Guid?>("SeriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SeriesId");

                    b.ToTable("SpecialEpisodes");

                    b.HasCheckConstraint("CH_SpecialEpisodes_ChannelNotEmpty", "Channel <> ''");

                    b.HasCheckConstraint("CH_SpecialEpisodes_MonthValid", "Month >= 1 AND Month <= 12");

                    b.HasCheckConstraint("CH_SpecialEpisodes_SequenceNumberPositive", "SequenceNumber > 0");

                    b.HasCheckConstraint("CH_SpecialEpisodes_YearPositive", "Year > 0");
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

                    b.ToTable("Tags");

                    b.HasCheckConstraint("CH_Tag_CategoryNotEmpty", "Category <> ''");

                    b.HasCheckConstraint("CH_Tag_NameNotEmpty", "Name <> ''");
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
                        .WithOne()
                        .HasForeignKey("Cineaste.Core.Domain.Franchise", "FranchiseItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("FranchiseId", "Id");

                            b1.ToTable("FranchiseTitles", (string)null);

                            b1.HasCheckConstraint("CH_FranchiseTitles_NameNotEmpty", "Name <> ''");

                            b1.HasCheckConstraint("CH_FranchiseTitles_PriorityPositive", "Priority > 0");

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
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ParentFranchise");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.ListConfiguration", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.CineasteList", "List")
                        .WithOne()
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

                    b.Navigation("List");

                    b.Navigation("SortingConfiguration")
                        .IsRequired();
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Movie", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.FranchiseItem", "FranchiseItem")
                        .WithOne()
                        .HasForeignKey("Cineaste.Core.Domain.Movie", "FranchiseItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cineaste.Core.Domain.MovieKind", "Kind")
                        .WithMany()
                        .HasForeignKey("KindId")
                        .OnDelete(DeleteBehavior.Cascade)
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

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

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

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("MovieId", "Id");

                            b1.ToTable("MovieTitles", (string)null);

                            b1.HasCheckConstraint("CH_MovieTitles_NameNotEmpty", "Name <> ''");

                            b1.HasCheckConstraint("CH_MovieTitles_PriorityPositive", "Priority > 0");

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
                        .HasForeignKey("ListId");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Period", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.Season", null)
                        .WithMany("Periods")
                        .HasForeignKey("SeasonId");

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
                        .HasForeignKey("SeriesId");

                    b.OwnsMany("Cineaste.Core.Domain.Title", "Titles", b1 =>
                        {
                            b1.Property<Guid>("SeasonId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("SeasonId", "Id");

                            b1.ToTable("SeasonTitles", (string)null);

                            b1.HasCheckConstraint("CH_SeasonTitles_NameNotEmpty", "Name <> ''");

                            b1.HasCheckConstraint("CH_SeasonTitles_PriorityPositive", "Priority > 0");

                            b1.WithOwner()
                                .HasForeignKey("SeasonId");
                        });

                    b.Navigation("Titles");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Series", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.FranchiseItem", "FranchiseItem")
                        .WithOne()
                        .HasForeignKey("Cineaste.Core.Domain.Series", "FranchiseItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cineaste.Core.Domain.SeriesKind", "Kind")
                        .WithMany()
                        .HasForeignKey("KindId")
                        .OnDelete(DeleteBehavior.Cascade)
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

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

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

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("SeriesId", "Id");

                            b1.ToTable("SeriesTitles", (string)null);

                            b1.HasCheckConstraint("CH_SeriesTitles_NameNotEmpty", "Name <> ''");

                            b1.HasCheckConstraint("CH_SeriesTitles_PriorityPositive", "Priority > 0");

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
                        .HasForeignKey("ListId");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.SpecialEpisode", b =>
                {
                    b.HasOne("Cineaste.Core.Domain.Series", null)
                        .WithMany("SpecialEpisodes")
                        .HasForeignKey("SeriesId");

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

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<bool>("IsOriginal")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Priority")
                                .HasColumnType("int");

                            b1.HasKey("SpecialEpisodeId", "Id");

                            b1.ToTable("SpecialEpisodeTitles", (string)null);

                            b1.HasCheckConstraint("CH_SpecialEpisodeTitles_NameNotEmpty", "Name <> ''");

                            b1.HasCheckConstraint("CH_SpecialEpisodeTitles_PriorityPositive", "Priority > 0");

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

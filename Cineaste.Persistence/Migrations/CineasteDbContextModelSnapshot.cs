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
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Cineaste.Core.Domain.Movie", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImdbId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsReleased")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWatched")
                        .HasColumnType("bit");

                    b.Property<string>("RottenTomatoesLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Movies");

                    b.HasCheckConstraint("CH_Movies_YearPositive", "Year > 0");
                });

            modelBuilder.Entity("Cineaste.Core.Domain.Movie", b =>
                {
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

                            b1.HasCheckConstraint("CH_MovieTitles_NameNonEmpty", "Name <> ''");

                            b1.HasCheckConstraint("CH_MovieTitles_PriorityPositive", "Priority > 0");

                            b1.WithOwner()
                                .HasForeignKey("MovieId");
                        });

                    b.Navigation("Poster");

                    b.Navigation("Titles");
                });
#pragma warning restore 612, 618
        }
    }
}

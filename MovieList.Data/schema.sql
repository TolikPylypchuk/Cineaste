CREATE TABLE "Kinds" (
    "Id" INTEGER PRIMARY KEY,
    "Name" TEXT UNIQUE NOT NULL,
    "ColorForWatchedMovie" TEXT NOT NULL,
    "ColorForWatchedSeries" TEXT NOT NULL,
    "ColorForNotWatchedMovie" TEXT NOT NULL,
    "ColorForNotWatchedSeries" TEXT NOT NULL,
    "ColorForNotReleasedMovie" TEXT NOT NULL,
    "ColorForNotReleasedSeries" TEXT NOT NULL
);

CREATE TABLE "Movies" (
    "Id" INTEGER PRIMARY KEY,
    "Year" INTEGER NOT NULL,
    "IsWatched" INTEGER(1) NOT NULL,
    "IsReleased" INTEGER(1) NOT NULL,
    "ImdbLink" TEXT,
    "RottenTomatoesLink" TEXT,
    "PosterUrl" TEXT,
    "KindId" INTEGER NOT NULL,

    FOREIGN KEY ("KindId")
        REFERENCES "Kinds" ("Id"),

    CHECK ("IsWatched" = 0 OR "IsReleased" = 1)
);

CREATE INDEX "Idx_Movies_KindId" ON "Movies" ("KindId");

CREATE TABLE "Series" (
    "Id" INTEGER PRIMARY KEY,
    "IsMiniseries" INTEGER(1) NOT NULL,
    "IsAnthology" INTEGER(1) NOT NULL,
    "WatchStatus" INTEGER NOT NULL,
    "ReleaseStatus" INTEGER NOT NULL,
    "ImdbLink" TEXT,
    "RottenTomatoesLink" TEXT,
    "PosterUrl" TEXT,
    "KindId" INTEGER NOT NULL,

    FOREIGN KEY ("KindId")
        REFERENCES "Kinds" ("Id")
);

CREATE INDEX "Idx_Series_KindId" ON "Series" ("KindId");

CREATE TABLE "Seasons" (
    "Id" INTEGER PRIMARY KEY,
    "WatchStatus" INTEGER NOT NULL,
    "ReleaseStatus" INTEGER NOT NULL,
    "Channel" TEXT NOT NULL,
    "SequenceNumber" INTEGER NOT NULL,
    "SeriesId" INTEGER NOT NULL,

    FOREIGN KEY ("SeriesId")
        REFERENCES "Series" ("Id")
        ON DELETE CASCADE
);

CREATE INDEX "Idx_Seasons_SeriesId" ON "Seasons" ("SeriesId");

CREATE TABLE "Periods" (
    "Id" INTEGER PRIMARY KEY,
    "StartMonth" INTEGER NOT NULL,
    "StartYear" INTEGER NOT NULL,
    "EndMonth" INTEGER NOT NULL,
    "EndYear" INTEGER NOT NULL,
    "IsSingleDayRelease" INTEGER(1) NOT NULL,
    "NumberOfEpisodes" INTEGER NOT NULL,
    "RottenTomatoesLink" TEXT,
    "PosterUrl" TEXT,
    "SeasonId" INTEGER NOT NULL,

    FOREIGN KEY ("SeasonId")
        REFERENCES "Seasons" ("Id")
        ON DELETE CASCADE
);

CREATE INDEX "Idx_Periods_SeasonId" ON "Periods" ("SeasonId");

CREATE TABLE "SpecialEpisodes" (
    "Id" INTEGER PRIMARY KEY,
    "Month" INTEGER NOT NULL,
    "Year" INTEGER NOT NULL,
    "IsWatched" INTEGER(1) NOT NULL,
    "IsReleased" INTEGER(1) NOT NULL,
    "Channel" TEXT NOT NULL,
    "SequenceNumber" INTEGER NOT NULL,
    "RottenTomatoesLink" TEXT,
    "PosterUrl" TEXT,
    "SeriesId" INTEGER NOT NULL,

    FOREIGN KEY ("SeriesId")
        REFERENCES "Series" ("Id")
        ON DELETE CASCADE,

    CHECK ("IsWatched" = 0 OR "IsReleased" = 1)
);

CREATE INDEX "Idx_SpecialEpisodes_SeriesId" ON "SpecialEpisodes" ("SeriesId");

CREATE TABLE "Franchises" (
    "Id" INTEGER PRIMARY KEY,
    "ShowTitles" INTEGER(1) NOT NULL,
    "IsLooselyConnected" INTEGER(1) NOT NULL,
    "MergeDisplayNumbers" INTEGER(1) NOT NULL DEFAULT 0,
    "PosterUrl" TEXT
);

CREATE TABLE "FranchiseEntries" (
    "Id" INTEGER PRIMARY KEY,
    "SequenceNumber" INTEGER NOT NULL,
    "DisplayNumber" INTEGER,
    "MovieId" INTEGER,
    "SeriesId" INTEGER,
    "FranchiseId" INTEGER,
    "ParentFranchiseId" INTEGER NOT NULL,

    FOREIGN KEY ("MovieId")
        REFERENCES "Movies" ("Id")
        ON DELETE CASCADE,

    FOREIGN KEY ("SeriesId")
        REFERENCES "Series" ("Id")
        ON DELETE CASCADE,

    FOREIGN KEY ("FranchiseId")
        REFERENCES "Franchises" ("Id")
        ON DELETE CASCADE,

    FOREIGN KEY ("ParentFranchiseId")
        REFERENCES "Franchises" ("Id")
        ON DELETE CASCADE
);

CREATE INDEX "Idx_FranchiseEntries_MovieId" ON "FranchiseEntries" ("MovieId");
CREATE INDEX "Idx_FranchiseEntries_SeriesId" ON "FranchiseEntries" ("SeriesId");
CREATE INDEX "Idx_FranchiseEntries_FranchiseId" ON "FranchiseEntries" ("FranchiseId");
CREATE INDEX "Idx_FranchiseEntries_ParentFranchiseId" ON "FranchiseEntries" ("ParentFranchiseId");

CREATE TABLE "Titles" (
    "Id" INTEGER PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Priority" INTEGER NOT NULL DEFAULT 1,
    "IsOriginal" INTEGER(1) NOT NULL,
    "MovieId" INTEGER,
    "SeriesId" INTEGER,
    "SeasonId" INTEGER,
    "SpecialEpisodeId" INTEGER,
    "FranchiseId" INTEGER,

    FOREIGN KEY ("MovieId")
        REFERENCES "Movies" ("Id")
        ON DELETE CASCADE,

    FOREIGN KEY ("SeriesId")
        REFERENCES "Series" ("Id")
        ON DELETE CASCADE,

    FOREIGN KEY ("SeasonId")
        REFERENCES "Seasons" ("Id")
        ON DELETE CASCADE,
        
    FOREIGN KEY ("SpecialEpisodeId")
        REFERENCES "SpecialEpisodes" ("Id")
        ON DELETE CASCADE,
        
    FOREIGN KEY ("FranchiseId")
        REFERENCES "Franchises" ("Id")
        ON DELETE CASCADE,

    CHECK ("Priority" >= 1 AND "Priority" <= 10)
);

CREATE INDEX "Idx_Titles_MovieId" ON "Titles" ("MovieId");
CREATE INDEX "Idx_Titles_SeriesId" ON "Titles" ("SeriesId");
CREATE INDEX "Idx_Titles_SeasonId" ON "Titles" ("SeasonId");
CREATE INDEX "Idx_Titles_SpecialEpisodeId" ON "Titles" ("SpecialEpisodeId");
CREATE INDEX "Idx_Titles_FranchiseId" ON "Titles" ("FranchiseId");

CREATE TABLE "Tags" (
    "Id" INTEGER PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Color" TEXT NOT NULL,
    "IsApplicableToMovies" INTEGER(1) NOT NULL,
    "IsApplicableToSeries" INTEGER(1) NOT NULL,

    UNIQUE ("Name", "Category")
);

CREATE TABLE "TagImplications" (
    "Id" INTEGER PRIMARY KEY,
    "PremiseId" INTEGER,
    "ConsequenceId" INTEGER,
    
    FOREIGN KEY ("PremiseId")
        REFERENCES "Tags" ("Id")
        ON DELETE CASCADE,
        
    FOREIGN KEY ("ConsequenceId")
        REFERENCES "Tags" ("Id")
        ON DELETE CASCADE,

    CHECK ("PremiseId" <> "ConsequenceId")
);

CREATE TABLE "MovieTags" (
    "Id" INTEGER PRIMARY KEY,
    "MovieId" INTEGER,
    "TagId" INTEGER,
    
    FOREIGN KEY ("MovieId")
        REFERENCES "Movies" ("Id")
        ON DELETE CASCADE,
        
    FOREIGN KEY ("TagId")
        REFERENCES "Tags" ("Id")
        ON DELETE CASCADE
);

CREATE TABLE "SeriesTags" (
    "Id" INTEGER PRIMARY KEY,
    "SeriesId" INTEGER,
    "TagId" INTEGER,
    
    FOREIGN KEY ("SeriesId")
        REFERENCES "Series" ("Id")
        ON DELETE CASCADE,
        
    FOREIGN KEY ("TagId")
        REFERENCES "Tags" ("Id")
        ON DELETE CASCADE
);

CREATE TABLE "Settings" (
    "Id" INTEGER PRIMARY KEY,
    "Key" TEXT UNIQUE NOT NULL,
    "Value" TEXT NOT NULL
);

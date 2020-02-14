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
    "PosterUrl" TEXT,
    "SeriesId" INTEGER NOT NULL,

    FOREIGN KEY ("SeriesId")
        REFERENCES "Series" ("Id")
        ON DELETE CASCADE,

    CHECK ("IsWatched" = 0 OR "IsReleased" = 1)
);

CREATE INDEX "Idx_SpecialEpisodes_SeriesId" ON "SpecialEpisodes" ("SeriesId");

CREATE TABLE "MovieSeries" (
    "Id" INTEGER PRIMARY KEY,
    "ShowTitles" INTEGER(1) NOT NULL,
    "IsLooselyConnected" INTEGER(1) NOT NULL,
    "MergeDisplayNumbers" INTEGER(1) NOT NULL DEFAULT 0,
    "PosterUrl" TEXT
);

CREATE TABLE "MovieSeriesEntries" (
    "Id" INTEGER PRIMARY KEY,
    "SequenceNumber" INTEGER NOT NULL,
    "DisplayNumber" INTEGER,
    "MovieId" INTEGER,
    "SeriesId" INTEGER,
    "MovieSeriesId" INTEGER,
    "ParentSeriesId" INTEGER NOT NULL,

    FOREIGN KEY ("MovieId")
        REFERENCES "Movies" ("Id")
        ON DELETE CASCADE,

    FOREIGN KEY ("SeriesId")
        REFERENCES "Series" ("Id")
        ON DELETE CASCADE,

    FOREIGN KEY ("MovieSeriesId")
        REFERENCES "MovieSeries" ("Id")
        ON DELETE CASCADE,

    FOREIGN KEY ("ParentSeriesId")
        REFERENCES "MovieSeries" ("Id")
        ON DELETE CASCADE
);

CREATE INDEX "Idx_MovieSeriesEntries_MovieId" ON "MovieSeriesEntries" ("MovieId");
CREATE INDEX "Idx_MovieSeriesEntries_SeriesId" ON "MovieSeriesEntries" ("SeriesId");
CREATE INDEX "Idx_MovieSeriesEntries_MovieSeriesId" ON "MovieSeriesEntries" ("MovieSeriesId");
CREATE INDEX "Idx_MovieSeriesEntries_ParentSeriesId" ON "MovieSeriesEntries" ("ParentSeriesId");

CREATE TABLE "Titles" (
    "Id" INTEGER PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Priority" INTEGER NOT NULL DEFAULT 1,
    "IsOriginal" INTEGER(1) NOT NULL,
    "MovieId" INTEGER,
    "SeriesId" INTEGER,
    "SeasonId" INTEGER,
    "SpecialEpisodeId" INTEGER,
    "MovieSeriesId" INTEGER,

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
        
    FOREIGN KEY ("MovieSeriesId")
        REFERENCES "MovieSeries" ("Id")
        ON DELETE CASCADE,

    CHECK ("Priority" >= 1 AND "Priority" <= 10)
);

CREATE INDEX "Idx_Titles_MovieId" ON "Titles" ("MovieId");
CREATE INDEX "Idx_Titles_SeriesId" ON "Titles" ("SeriesId");
CREATE INDEX "Idx_Titles_SeasonId" ON "Titles" ("SeasonId");
CREATE INDEX "Idx_Titles_SpecialEpisodeId" ON "Titles" ("SpecialEpisodeId");
CREATE INDEX "Idx_Titles_MovieSeriesId" ON "Titles" ("MovieSeriesId");

CREATE TABLE "Settings" (
    "Id" INTEGER PRIMARY KEY,
    "Key" TEXT UNIQUE NOT NULL,
    "Value" TEXT NOT NULL
);

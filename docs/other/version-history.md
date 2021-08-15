# Version History

This article provides info on what's new in every version of the Cineaste app.

## Version 0.3

This version adds no new features. The app itself was renamed to Cineaste. The UI was completely rewritten to use Avalonia instead of WPF, so now the app can be cross-platform \(it was built and tested only on Windows 10 though\). Also, it now uses Fluent UI instead of Material UI, and a dark theme was added.

![](../.gitbook/assets/v0.3-screen-movie.png)

There are no breaking changes to the list files, except that now their extension should be _.cnl_ instead of _.mlist_. The preferences file contains breaking changes though, since the current theme \(light or dark\) was added to the preferences.

## Version 0.2

This version adds searching, filtering, and sorting capabilities to the app. Also, tags can now be added to movies and series to make searching/filtering easier. This release is still very incomplete and far from being stable.

![](../.gitbook/assets/v0.2-screen-movie.png)

The following breaking changes were added to the list files:

* The `Tags`, `MovieTags`, and `SeriesTags` tables were added;
* The `IsAnthology` column was removed from series - if you want this info, implement it yourself through tags;
* The following keys were added to the `Settings` table: `list.sort.order.1`, `list.sort.order.2`, `list.sort.direction.1`, and `list.sort.direction.2`;
* The following keys were changed in the `Settings` table: `season.title.default` -&gt; `list.season.title.default`, `season.title.default-original` -&gt; `list.season.title.default-original`.

## Version 0.1

This is the first usable version of the app, albeit very incomplete. It provides the most basic functionality:

* Creating/opening list files;
* Adding/editing/removing movies, series, and franchises;
* Editing file settings, including kinds;
* Editing app preferences.

![](../.gitbook/assets/v0.1-screen-movie.png)

## Ancient History

In the beginning, the app looked and felt very differently. There could be only a sinlge list file, and the path to it was specified in the preferences. There were no tabs. And the UI was in Ukrainian, instead of English.

Afterwards the app was completely rewritten, but this version can be found under the `old` tag on GitHub.

![](../.gitbook/assets/old-screen-1.png)

![](../.gitbook/assets/old-screen-2.png)


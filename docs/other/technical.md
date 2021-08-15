# Technical Stuff

Cineaste is written with [C\#](https://github.com/dotnet/csharplang) and [.NET 5](https://github.com/dotnet/runtime). Following is the list of technologies used in this app and some other technical aspects.

## UI

This app is a desktop app. I've contemplated writing a web app, but decided against it, partly because it's easier, and because it makes more sense to store lists in files on users' machines instead of some sort of centralized database.

The UI is written using [Avalonia](https://avaloniaui.net), and styled with [Fluent Design](https://www.microsoft.com/design/fluent) using [FluentAvalonia](https://github.com/amwx/FluentAvalonia). This means that the app can be cross-platform, but it was built and tested only on Windows 10.

At the beginning of the app's developement, I didn't use any libraries for styling at all, thinking I will write styles myself. That proved to be a horrible idea, and I started using [HandyControl](https://github.com/HandyOrg/HandyControl) and then switched to my own fork of the library \(which I've since deleted completely, so some very old commits cannot even be built anymore\). I have since decided to completely rewrite the app, and to use Material Design for UI before switching to Fluent Design. You can find this old version under the [old git tag](https://github.com/TolikPylypchuk/MovieList/releases/tag/old).

## Core Logic

I'm using [ReactiveUI](https://www.reactiveui.net) as the MVVM framework for the core logic, and I must say that it's a real pleasure to use this framework. Also, [Splat](https://github.com/reactiveui/splat) is used for service location.

In the beginning I rolled my own MVVM stuff, but it quickly became bloated and extremely unwieldy in code, and the performance became horrible. So I've scrapped everyhing, and started from scratch using ReactiveUI \(and changed the UI library along the way\).

## Data Access

This app uses [SQLite](https://www.sqlite.org/index.html) for data access. The _.cnl_ files are [actually SQLite databases](https://www.sqlite.org/appfileformat.html) which you can open in e.g. [DB Browser for SQLite](https://sqlitebrowser.org) and browse like any other relational database, although looking through data is quite inconvenient there.

The file make-up is quite straightforward. Just look at table definitions in DB Browser for SQLite or another app to see their structure and relations.

The only table which should be elaborated on is the `Settings` table. It contains key-value pairs for some general file settings which don't fit into other places. Currently each file contains nine such key-value pairs:

* List name \(`list.name`\);
* List version \(`list.version`\);
* The default season title \(`list.season.title.default`\);
* The default season original title \(`list.season.title.default-original`\);
* The language used for sorting titles \(`list.culture`\);
* The default first sorting order \(`list.sort.order.1`\);
* The default second sorting order \(`list.sort.order.2`\);
* The default first sorting direction \(`list.sort.direction.1`\);
* The default second sorting direction \(`list.sort.direction.2`\).

The app ignores any other rows in this table and will not be able to read the file if any one of the aforementioned rows are missing.

The list version is currently always set to 1 and will be incremented every time a new version of the app introduces breaking changes to the file format. Note that the 0.x versions are pre-release versions and they will definitely introduce breaking changes to files, but won't increment its version. The file version will be incremented after the app reaches version 1.0. I haven't yet figured out what the strategy for dealing with multiple file versions will be. All breaking changes will be documented, but if you're using this app before version 1.0, you have to figure out how to migrate files on your own.

## Preferences

Preferences are stored in the user's app data folder, under the _Cineaste_ directory. Also, the cache of downloaded posters is stored in the user's local app data folder \(also under the _Cineaste_ directory\). If you want to uninstall this app completely, you should delete these folders manually.

This app uses [Akavache](https://github.com/reactiveui/Akavache) as the engine for storing settings and cache.

## Logging

This app uses a plain-text log file to log the stuff it's doing and errors it encounters along the way. [Serilog](https://serilog.net) is used as the logging library. The location of the log file, as well as the minimum log level can be set in preferences, in the advanced section. The default location of the log file is in the user's app data folder. Here's the list of supported log levels \(from lowest to highest\):

* _Verbose_: log everything \(this level is not used directly and setting it is the same as setting _Debug_ as the minimum level\);
* _Debug_: log the details of the app's internal workflow;
* _Info_: log general information;
* _Warning_: log warnings about unwanted but manageable events;
* _Error_: log errors that the app cannot fix itself;
* _Fatal_: log unforeseen and unknown errors which make the app crash.

The default log level is _Warning_ and you should rarely set it to a lower setting.

Currently, the maximum log file size is 10 MB. If it gets bigger, it will be rolled over. The last 2 rolled files are kept, while older ones are deleted.

## Docs

[GitBook](https://www.gitbook.com) is used for docs since it's quite versatile and can host docs for multiple app versions.

Previously these articles were built using [Jekyll](https://jekyllrb.com) and [GitHub Pages](https://pages.github.com), and used the [Minimal Mistakes theme](https://mmistakes.github.io/minimal-mistakes). The first version of the docs used the [Just the Docs theme](https://pmarsceill.github.io/just-the-docs) but it doesn't support multiple versions of the docs.

## Building from Source

If you want to build your own version of this app, you simply need .NET 5. You can look at the `global.json` file to see the exact version of .NET that this app needs. That's it, there are no other prerequisites.

 To build the app for Windows run the `Publish-App` script to create a zipped app which you can then extract to anywhere. There are not scripts available for other platforms, but you can look at how `Publish-App` works and do something similar.


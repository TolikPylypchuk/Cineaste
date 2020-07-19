---
layout: default
title: Technical Stuff
nav_order: 5
permalink: technical
---

# Technical Stuff

The Movie List app is written with [C#](https://github.com/dotnet/csharplang) and
[.NET Core 3.1](https://github.com/dotnet/core). Following is the list of technologies used in this app and some other
technical aspects.

## UI

This app is a desktop app. I've contemplated writing a web-app, but decided against it, partly because it's easier,
and because it makes more sense to store lists in files on user's machines instead of some sort of centralized database.

The UI is written using [WPF](https://github.com/dotnet/wpf), and styled with
[Material Design](https://material.io/design) using
[Material Design in XAML](http://materialdesigninxaml.net/) and
[Material Design Extensions](https://spiegelp.github.io/MaterialDesignExtensions).

I'm currently thinking about switching to [Avalonia](https://avaloniaui.net) or
[WinUI 3](https://github.com/microsoft/microsoft-ui-xaml) in the future, because while WPF is powerful, these frameworks
are the hot stuff and are more active. Also, they will support Fluent Design, which I think is more natural for Windows
compared to Material Design. If I migrate to Avalonia, I may even make this app cross-platform, even though it's highly
unlikely.

At the beginning of the app's developement, I didn't use any libraries for styling at all, thinking I will write styles
myself. That proved to be a horrible idea, and I started using [HandyControl](https://github.com/HandyOrg/HandyControl)
and then switched to my own fork of the library (which I've since deleted completely, so some very old commits cannot
even be built anymore). I have since decided to completely rewrite the app, and to use Material Design for UI.

## Core Logic

I'm using [ReactiveUI](https://www.reactiveui.net) as the MVVM framework for the core logic, and I must say that it's a
real pleasure to use this framework.

In the beginning I rolled my own MVVM stuff, but it quickly became bloated and extremely unwieldy in code, and the
performance became horrible. So I've scrapped everyhing, and started from scratch using ReactiveUI (and changed the UI
library along the way).

## Data Access

This app uses [SQLite](https://www.sqlite.org/index.html) for data access. The .mlist files are
[actually SQLite databases](https://www.sqlite.org/appfileformat.html) which you can open in e.g.
[DB Browser for SQLite](https://sqlitebrowser.org) and browse like any other relational database, although looking
through data is quite inconvenient there.

The file make-up is quite straightforward. Just look at table definitions in DB Browser for SQLite or another app to see
their structure and relations.

The only table which should be elaborated on is the 'Settings' table. It contains key-value pairs for some general file
settings which don't fit into other places. Currently each file contains five such key-value pairs:

- List name (list.name)
- List version (list.version)
- The default season title (season.title.default)
- The default season original title (season.title.default-original)
- The language used for sorting titles (list.culture)

The app ignores any other rows in this table and will not be able to read the file if any one of the aforementioned rows
are missing.

The list version is currently always set to 1 and will be incremented every time a new version of the app introduces
breaking changes to the file format. Note that the 0.x versions are pre-release versions and they will definitely
introduce breaking changes to files, but won't increment its version. The file version will be incremented after the
app reaches version 1.0. I haven't yet figured out what the strategy for dealing with multiple file version will be.
All breaking changes will be documented, but if you're using this app before version 1.0, you have to figure out how to
migrate files on your own.

## Preferences

Preferences are stored in the user's app data folder, under the 'MovieList' directory. Also, the cache of downloaded
posters is stored in the user's local app data folder (also under the 'MovieList' directory). If you want to uninstall
this app completely, you should delete these folders manually.

This app uses [Akavache](https://github.com/reactiveui/Akavache) as the engine for storing settings and cache.

## Logging

This app uses a plain-text log file to log the stuff it's doing and errors it encounters along the way. The location
of the log file, as well as the minimum log level can be set in preferences, in the advanced section. The default
location of the log file is in the user's app data folder. Here's the list of supported log levels (from lowest to
highest):

- Verbose: log everything (this level is not used directly and setting it is the same as setting 'Debug' as the
minimum level)
- Debug: log the details of the app's internal workflow
- Info: log general information
- Warning: log warnings about unwanted but manageable events
- Error: log errors that the app cannot fix itself
- Fatal: log unforeseen and unknown errors which make the app crash

The default log level is Warning and you should rarely set it to a lower setting.

Currently, the file is never trimmed or rolled over, so if you choose a low log level, and use this app a lot, the size
of the log file may become quite big. If it does, don't hesitate to delete it - the app certainly won't miss it - it
won't even notice that the file is gone. A future version will set a max size of the log file so that this doesn't
happen.

## Docs

These articles are built using [Jekyll](https://jekyllrb.com) and [GitHub Pages](https://pages.github.com), and use the
[Just the Docs](https://pmarsceill.github.io/just-the-docs) theme.

## Building from Source

If you want to build your own version of this app, you simply need .NET Core 3.1. That's it, there are no special
prerequisites.

If you also want to build the docs locally, you need Jekyll.

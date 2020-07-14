---
layout: default
title: List Files
nav_order: 3
permalink: list-files
---

# List Files

Here's what the app looks like when you create a new file:

![Screen with new file](/assets/images/screen-new-file.png)

The files are contain three main sections: the list itself, stats, and settings. You can switch between them using the
left-side bar.

When you open a file, you can close it via the 'close' button in the tab's header, through the File > Close menu item,
or by pressing Ctrl+W.

## The List Section

Here's what the list section looks like when it contains entries:

![Screen with list](/assets/images/screen-list.png)

### List Structure

The list items can be of three types:

- Movies
- Series
- Franchises

The items are sorted alphabetically by title and then by year if their title is the same. Future versions will include
alternative sorting options.

Franchises are used to group related items together and without them some items would be scattered across the list when
it would make more sense to have them appear together.

The list has four columns:

- Sequence number to show the item's number in a franchise
- The title (in your native language)
- The original title
- Year or years

The majority of popular contemporary movies are in English, so if your native language is English, then the title
will often be the same as the original title.

### Kinds

Every item can be of a certain kind. Kinds are used to color-code items in the list. Every kind has six
colors for various item types and states:

- Movie which has already been watched
- Movie which hasn't been watched yet
- Movie which hasn't been released yet
- Series which has already been watched or is being watched
- Series which hasn't been watched yet
- Series which hasn't been released yet

There are no separate colors for franchises - they get the color of their first entry.

By default, every file contains three kinds (although it can be configured):

- Live-action: movies are black, series are blue
- Animation: movies are green, series are light-blue
- Documentary: both movies and series are purple

The default kinds have all non-watched items as red and all non-released items as dark red.

### Adding Items to the List

You can add new items to the list by clicking the buttons in the right-side panel, which will open a form for editing
the item. If you want to edit an existing item, click on it, and the form for this item will be opened. You can't create
franchises directly because they are not supposed to be separate list items. Rather, you create them through previously
created items.

You can learn more about editing list items in the next article.

## The Stats Section

Statistics are not implemented yet, so the respective left-side bar item is disabled.

## The Settings Section

Here's what the settings for a file look like:

![Screen with settings](/assets/images/screen-settings.png)

You can open the file settings by clicking the left-side bar item, through the File > Settings menu item, or by
pressing Ctrl+P.

The settings consist of the two main sections: main settings and kind settings.

### Main File Settings

You can edit the following file attributes in the settings:

- List name
- Title language
- Default season title
- Default season original title

You can name your list however you like and it doesn't have to be the same as the name of the file. The list name
appears in the tab header and in the recent files list on the home page.

Title language is used to correctly sort titles. While original titles can be in any language, titles will usually be
in your native language, and sorting rules may be different for different languages.

Default season title and original title are used to generate titles for newly created seasons for series. The # symbol
will be replaced with the actual season number. For example, if the default season title is 'Season #' and you create
two seasons, their default names will be 'Season 1' and 'Season 2' respectively. Same for season original titles. You
can learn more about the season form in the next article.

### Kind Settings

Here you can edit existing kinds, delete them, or add new ones.

A single kind setting consists of a name and six colors for the aforementioned item types. You can edit colors by
clicking on them and by editing the color in the modal window.

![Screen with color modal](/assets/images/screen-settings-color.png)

You can select a color in the color picker or enter its HEX color value. Mind that the HEX value field uses ARGB instead
of RGB! You can add 'FF' in the beginning to convert an RGB value into an ARGB value.

You can have as many kinds as you like (but more than zero). You can't delete kinds that have items of that kind in the
list. If you want to delete a kind, you should manually edit its every item to have a different kind.

---
layout: default
title: The Movie Form
nav_order: 1
permalink: movie-form
parent: Forms
---

# The Movie Form

The movie form is the simplest and most straightforward of the forms. Here's how it looks:

![Screen with movie form](/assets/images/screen-movie-form.png)

The form consists of the following sections:

- Header which contains the first title, the 'close' button, and a few other buttons if the movie is a part of a
franchise (which will be discussed in the article on franchises)
- Poster and links, if present
- The form fields
- Buttons for various actions

## Form Fields

The movie form contains the following fields:

- Check-box which indicates whether you watched the movie
- Check-box which indicates whether the movie is released
- Titles (at least one required)
- Original titles (at least one required)
- Year (required)
- Kind (required)
- IMDb link
- Rotten Tomatoes link
- Poster URL

The 'released' check-box is enabled only if the movie's year is the same as the current year. If the movie's year is
before the current year, it's obvious that the movie is already released. Conversely, if the movie's year is after the
current year, it's obvious that the movie was not released yet.

If you check the 'watched' check-box, and the 'released' check-box is unchecked, it becomes checked as well. It doesn't
make sense that you can watch an unreleased movie (yes, there are exceptions, but those are rare).

You can add titles with the 'Add title' and 'Add original title' buttons. If there are multiple titles, you can delete
them by pressing the 'delete' button on the right side of the title field. You can't change the order of the titles.
If you decide that the second title should become the first and the first - second, you should change them manually.

The minimum value for the year is 1850 and maximum value is 2100. The reason that the movie form includes only a year,
and not a full release date is because the movie can have multiple release dates: premieres are on different dates in
different markets, and movies can be shown at festivals ahead of their release.

You can select any kind from the list of kinds which you specify in the file's settings.

Links and the poster URL are not required, and indeed not every movie is guaranteed to have an entry in IMDb or RT
(although I have yet to find a movie which doesn't have an entry in IMDb).

## Actions

The movie form has several action buttons:

- Delete the movie
- Create a franchise
- Other franchise actions which are discussed in the article on franchises

You can execute these actions only when the form doesn't contain changes.

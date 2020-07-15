---
layout: default
title: The Series Form
nav_order: 2
permalink: series-form
parent: Forms
---

# The Series Form

This is a big one. And it will take much more effort to add a series than it is to add a movie. Here's how a series
form looks:

![Screen with series form](/assets/images/screen-series-form.png)

- Header which contains the first title, the 'close' button, and a few other buttons if the series is a part of a
franchise (which will be discussed in the article on franchises)
- Poster (or logo) and links, if present
- Main form fields
- The list of components: seasons and special episodes
- Additional fields
- Buttons for various actions

## Form Fields

The series form contains the following fields:

- Titles (at least one required)
- Original titles (at least one required)
- Watch status (required)
- Release status (required)
- Check-box which indicates whether the series is an anthology
- Kind (required)
- IMDb link
- Rotten Tomatoes link
- Poster URL

All the same rules apply to titles as with the movie form.

Since watching the series is more of a continuous process (unless you binge it), its watch status is more than just
'watched' or 'not watched'. Here are the possible watch statuses:

- Not watched, if you haven't started watching the series yet
- Watching, if you are currenly watching the series or waiting for new seasons
- Watched, if you watched the series in its entirety
- Stopped watching, if you desided that the series is not worth your time anymore

Like watch status, there's more to the release status than simply 'released' or 'not released'. Here's the possible
release statuses:

- Not started
- Running
- Finished, if the series finished naturally
- Cancelled, if the series was axed by the studio
- Don't know/don't care, if you stopped watching the series and you don't want to track it in the app anymore

You can specify that a series is an anthology series if its seasons or even individual episodes have separate
storylines.

You can select any kind from the list of kinds which you specify in the file's settings.

Links and the poster URL are not required, and indeed not every series is guaranteed to have an entry in IMDb or RT
(although, like with movies, I have yet to find a series which doesn't have an entry in IMDb). Instead of a poster,
you can put a URL of a series' logo, which I do for most series.

## Series Components

Series can (and should) contain one or more components. A series component is either a season or a special episode
which in the context of this app means an episode which was aired or released separately from others, even if
the studio doesn't designate it as 'special'. You can add componentes through the 'Add season' or 'Add special episode'
buttons. When you press one of them, a respective form is opened. You can't save a series unless it contains at least
one component.

## Other Actions

The series form has several other action buttons:

- Delete the series
- Convert to miniseries
- Create a franchise
- Other franchise actions which are discussed in the article on franchises

You can convert a series to a miniseries. A miniseries is a one-season series without special episodes which is designed
to be limited in nature. Miniseries have separate, simplified forms. You can convert a series to a miniseries only if
it contains one season or none at all, and no special episodes.

You can execute these actions only when the form doesn't contain changes.

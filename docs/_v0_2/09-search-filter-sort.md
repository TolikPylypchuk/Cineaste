---
title: Searching, Filtering, Sorting
permalink: /v0.2/search-filter-sort
---

## Searching

You can search for certain items in the list. To do that, specify the filters, and press 'Find next' or 'Find
previous'. The found items are colored light purple, and the currently selected found item is colored a slightly darker
shade of purple. Here's how it looks:

![Screen with search]({{ '/assets/v0.2/images/screen-search.png' | absolute_url }})

When the search is activated, the list of all found items will appear in the search panel, and you can click on them to
jump to those items. To stop searching, click on the 'Stop searching' button. Afterwards, if you click on the 'Find
next' or 'Find previous' button again, the search will start from the top. If you want to stop searching and clear the
filters, then click on the 'Clear search' button.

Here's the list of all possible search filters:

- Title - searches by title (if an item has multiple titles, they are all considered):

  - Title is - searches for items whose title contains the specified input
  - Title starts with - searches for items whose title starts with the specified input
  - Title ends with - searches for items whose title ends with the specified input

- Year - searches by year:

  - Year is - searches for items of the specified year
  - Year less than - searches for items before the specified year
  - Year greater than - searches for items after the specified year
  - Year between - searches for items between the specified years (inclusive)

- Kind is - searches for items of the specified kind

- Tags - searches by tags:

  - Tags include - searches for items which have all of the specified tags
  - Tags exclude - searches for items which have none of the specified tags
  - Tags have category - searches for items which have any tags of the specified category

- Standalone - searches for items which don't belong to a franchise

- Movies only - searches only for movies

- Watched (movies only) - searches for movies which are watched

- Released (movies only) - searches for movies which are released

- Series only - searches only for series

- Watch status is (series only) - searches for series which have the specified watch status

- Release status is (series only) - searches for series which have the specified release status

- Channel (series only) - searches for series which have the specified channel:

  - Channel is - searches for series whose channel name equals the specified input
  - Channel starts with - searches for series whose channel name starts with the specified input
  - Channel ends with - searches for series whose channel name ends with the specified input

- Number of seasons (series only) - searches for series by the number of seasons:

  - Number of seasons is - searches for series which have the specified number of seasons
  - Number of seasons less than - searches for series which have less seasons than the specified number
  - Number of seasons greater than - searches for series which have more seasons than the specified number
  - Number of seasons between - searches for series whose number of seasons is between the specified numbers (inclusive)

- Number of episodes (series only) - searches for series by the number of episodes:

  - Number of episodes is - searches for series which have the specified number of episodes
  - Number of episodes less than - searches for series which have less episodes than the specified number
  - Number of episodes greater than - searches for series which have more episodes than the specified number
  - Number of episodes between - searches for series whose number of episodes is between the specified numbers
  (inclusive)

- Miniseries (series only) - searches for miniseries

You can negate the filters by checking the 'negate' check-box. Remember that it can mean a different thing than what
you think. For example, if you negate the 'Watched (movies only)' filter, it will search for series as well, because
it will search for all items which don't satisfy the 'Watched (movies only)' filter, and that includes series. When
a filter is negated, then a red strip is added on the left side.

The search filters can be composed in several ways. If you click on the right-side three-dot menu, the following
options will appear:

- Make composite (and)
- Make composite (or)
- Remove filter

If you make the filter composite, then three buttons will appear:

- Add filter (the 'plus' button)
- Change the filter composition (the 'cog' button)
- Simplify (the 'arrows' button)

If a composite filter contains only one filter, you can make it a simple filter again by pressing the 'simplify' button.
You can add filters to the composite filter by pressing the 'add filter' button.

The nature of the composite filter depends on the composition type of which there are two: 'and' and 'or'. If the
composition is 'and' then all filters must be positive for the composite filter to be positive. If the composition is
'or' then at least one filter must be positive for the composite filter to be positive. The 'and' composition is shown
with a green strip on the left side, and the 'or' composition is shown with a blue strip on the left side. You can
change the filter's composition by clicking the 'change composition' button.

Here's an example: in order to search for non-watched movies, you should compose two filters: the negated 'Watched
(movies only)' and 'Movies only', and set the composition to 'and' because both filters must be positive.

If you want to remove a filter, then you can click on the 'remove filter' in the three-dot menu.

You can create complex hierarchies of filters with multiple layers and different composition types.

## Filtering

Filtering is very similar to searching. As you saw previously, when searching for items you already use various kinds of
filters. Filtering the list uses the same principles, but has one big difference - it doesn't highlight the found items.
Instead, it shows only the found items. All other items are removed from the list completely. Here's how it looks:

![Screen with filter]({{ '/assets/v0.2/images/screen-filter.png' | absolute_url }})

When you have specified the filters, you can click on the 'Apply filter' button to apply them. Afterwards, you can click
on the 'Clear filter' button to return back to the full list.

You can combine searching and filtering, for example by filtering by some general conditions, and then searching with
more specific conditions in the filtered list.

## Sorting

You can specify how the list is sorted. There are a couple options, but they are limited. Here's how the sorting panel
looks:

![Screen with sort]({{ '/assets/v0.2/images/screen-sort.png' | absolute_url }})

Here are the possible sorting orders:

- By title
- By original title
- By title (simple)
- By original title (simple)
- By year

Sorting by title takes franchises into account, while simple sorting by title doesn't. You can specify the sorting
direction as well - ascending or descending.

There are two levels of sorting. The second level is used when the first level is the same (like having the same title).
You can't add more levels since two levels is enough.

By default the first level of sorting is by title, and the second level is by year. The defaults can be changed in the
file's settings, and the default settings for new files can be changed in the preferences.

Sorting by title is available only as the first level, because it's complex and it doesn't make sense to make it
second-level. Thus only the three last options are available for the second-level sorting.

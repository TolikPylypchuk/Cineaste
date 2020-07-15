---
layout: default
title: Forms
nav_order: 4
permalink: forms
has_children: true
---

# Forms

You can add, edit, and delete list items through forms which appear on the right side of the main section. There are five
types of forms:

- Movie
- Series
- Season (part of the series form)
- Special episode (part of the series form)
- Franchise

All list items can contain posters. None of them are stored in the list file however. Only their URLs are stored - the
posters themselves are fetched from the internet when the form is opened and cached locally for 5 minutes.

## Saving the Forms

Forms can be in two basic states: changed and unchaged. If the form is changed, you can cancel all changes and return
the form to how it was when you opened it. If the form is changed and valid, you can save it to write the changes to
the file. If you close an unsaved form, all changes are discarded. You can't perform certain actions when the form
contains changes, e.g. you can't delete it, or create a franchise.

You can also save a file through the File > Save menu item, or by pressing Ctrl+S.

If you look at the big picture, you'll see that this app lets you write only tiny changes to the file at a time, unlike
many other file-based apps. For example, you can't spend half an hour inputting data into the app and save all those
changes at once (and you really shouldn't do that in other apps as well), because you can save only one form at a time.

The Save As functionality works differently than in other apps. Normally you can make some changes to
a file, save it as another file (and maybe another file type) and the original will be unchagned - you can
even revert all changes so that the original file remains the same. In this app however, Save As works only on when
all changes are already saved in the original file. This is because the app doesn't let you change a lot without saving,
so there is no reason to use Save As to save a copy of the file and have the original unchanged. You shouldn't change
the file extension when saving as either. This app works only with the .mlist file extension.

You can also save a file as another file through the File > Save As... menu item, or by pressing Ctrl+Shift+S.

## Integration with IMDb and Rotten Tomatoes

This version of the app has minimal integration with IMDb and RT. You can enter links to the item on those sites and
that's it for now. A future version will show the IMDb score and RT scores inline so you don't have to open the sites
to view them.

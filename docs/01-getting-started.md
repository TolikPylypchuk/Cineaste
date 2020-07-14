---
layout: default
title: Getting Started
nav_order: 2
permalink: getting-started
---

# Getting Started

## How to Get the App

The Movie List app works only on Windows (and only tested on Windows 10). Currently the only way to get this app is to
build it from source, but version 0.1 will be released shortly.

Once built, extract it to anywhere and run it. On the first run it will create the necessary configuration which is
stored in the user's app data folder.

## Files and Tabs

The app works with files of the .mlist extension. An .mlist file represents a list of movies, series, and franchises,
and some auxiliary settings. If you want Windows to bind the file extension to this app, you'll have to do it manually
for now. All files are opened in separate tabs.

This is a single-instance app. You can't open multiple windows in this app. If you open it when it's already opened, it
will simply show you the existing window. If you open a file when the app is already running, it will open the file in
a new tab.

## The Home Page

![Screen with home page](/assets/images/screen-home-page.png)

The app's home page contains several features for working with files.

Firstly, you can create a list file. When you click on the 'Create a list' button, a 'Save file' dialog is opened.
Enter the file name and select the location, click save, and an empty list file will be created and opened in a new
tab. This is also available through the menu item File > New, or by pressing Ctrl+N.

Secondly, you can open an existing list file. There are several ways to do that. You can click the 'Open a list' button
and select the file in the 'Open file' dialog. Or you can use the menu item File > Open, or press Ctrl+O. Also, you can
drop a file into any area inside the app window to open a new file. Lastly, you can open the file through the File
Explorer if you had Windows bind the file extension to this app.

Thirdly, you can view and open any files that you've recently opened. Simply double-click on any 'Recently opened files'
entry to open it. You can also go to File > Open Recent... > {file name} to open a recent file. To remove a file from
the recently opened files list, click on the check-box on the left side of the entry, and click the 'Remove from list'
button.

## Other Menu Items

You can learn about other menu items in the following sections.

The following menu items don't belong to specific features:

- File > Exit (or press Alt+F4) to exit the app (obviously)
- Help > About (or press F1) to view some basic info about the app

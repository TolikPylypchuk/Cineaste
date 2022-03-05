# Preferences

The final feature covered in these docs is the app-wide preferences. You can open them through the _Edit > Preferences_ menu item, or by pressing `Ctrl+Shift+P`, and close them by clicking the _close_ button in the tab header, through the _File > Close_ menu item, or by pressing `Ctrl+W`.

Preferences consist of two sections:

* Default settings;
* Other preferences.

Preferences are stored in your user's app data folder, under the _Cineaste_ directory. If you decide to uninstall the app, you can delete this directory as well, if you don't wish to keep these preferences.

## Default Settings

Default settings are the settings that new files will get when they are created. They are almost identical to the settings you can find if you open a file's settings. The only difference is that you can't set a default list name. Here's how they look:

![](<../.gitbook/assets/v0.3-screen-preferences-1 (1).png>)

## Other Preferences

This section contains only a few things:

* The app's theme;
* A check-box which indicates whether to show recent files in the home page;
* Logging preferences.

Here's how they look:

![](<../.gitbook/assets/v0.3-screen-preferences-2 (1).png>)

You can set the app's theme to be light or dark. You can't set it to automatically be the same as the rest of your system though.

If you uncheck the check-box to show recent files, the app will clear the recent files list, and won't track which files were opened.

You probably shouldn't touch the logging preferences. They control the app's logging level and log location. You can read more in the article on technical stuff if you so wish. Unlike other preferences, if you change these settings, they will take effect after restarting the app.

## Dark Theme

All previous screenshots in the docs used the light theme. Here's how the dark theme looks:

![](<../.gitbook/assets/v0.3-screen-dark (1).png>)

You may have noticed that the list item colors are completely different here. You can't set separate colors of the light and dark theme - the app does it itself. It inverts the lightness of the kind colors, meaning that black becomes white, dark red becomes light red, light green becomes dark green and so on.

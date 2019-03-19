# EpisodeRenamer
I'm tired of renaming my TV episode files. This will automate the process for me.
## Description
Many of the files I download have a format like "**name.of.tv.show.s01e07.blah.blah.blah**". 

But, I'm also very particular about naming my files so they follow this format: **Name of TV show - 1.07 - Name of Episode**

This application retrieves info from an external database, stores them in a local database, and renames my files.

Solution is in .NET Core with a Console app as a front end. Stores TV show & episode info in a local Sqlite DB. Retrieves information from thetvdb.com
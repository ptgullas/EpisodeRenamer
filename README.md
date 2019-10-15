# EpisodeRenamer
I'm tired of renaming my TV episode files. This will automate the process for me.
## Description
Many of the files I download have a format like "**name.of.tv.show.s01e07.blah.blah.blah**". 

But, I'm also very particular about naming my files so they follow this format: **Name of TV show - 1.07 - Name of Episode**

This application retrieves info from an external database, stores them in a local database, and renames my files.

Solution is in .NET Core with a Console app as a front end. Stores TV show & episode info in a local Sqlite DB. Retrieves information from thetvdb.com
## Notes
There are a lot of duplicate commits because I used BFG to clean up files from my history, neglected to delete old commits, then ended up pushing the old history back.

Update: fixed this. Ran gitk --all (The Git graphical history viewer). Found there were 2 sets of commits from between 2 dates. I reset the master branch to the end of the first set (did a Mixed reset). 

Made a new commit, then did:
git push origin master --force

This removed the duplicate commits from GitHub.
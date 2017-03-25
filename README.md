# TVRenamer
Version 0.8

The purpose of this program is to automatically rename and organize TV Show and Movie files and folders on your computer.
Current Features:

1. Automatically remove "junk text" that commonly appear within file names of video files
2. Automatically combine single episodes into folders labeled "Season X" and organize those folders into a folder labeled "Show_Name"
3. The season number and title of the series is automatically detected
4. Allow for editing of .mkv file meta-tags so that the title of the episode does not retain the junk text of the original file name.

Coming Soon:

1. Click and Drag abilities for moving files into new folders within the TreeView UI.
2. If editing an mkv tag results in an error, have the error displayed at the end once all other tags are processed.
3. Automatically edit/remove mkv tags when pressing "Apply All"


Planned Features:

1. Automatically look up title names for episodes using online resources such as IMDB or wikipedia
2. Edit/remove meta tags of .mp4, .avi, and other common video file types
3. Set a "Video folder" location to send video files to. For example, automatically send files from Downloads folder to Videos.


Current Issues:

1. The program may incorrectly assume an episode formatted as "Ep ##" inside a folder named "Ep ##" is supposed to be Season ##
2. The auto-file combiner (put episodes into season folders) does not work unless it is a single file in a single folder


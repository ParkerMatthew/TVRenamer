# TVRenamer
Version 0.8

The purpose of this program is to automatically rename and organize TV Show and Movie files and folders on your computer.
Current Features:
1. Automatically remove "junk text" that commonly appear within file names of video files
2. Automatically combine single episodes into folders labeled "Season X" and organize those folders into a folder labeled "Show_Name"
  a. The season number and title of the series is automatically detected
3. Allow for editing of .mkv file meta-tags so that the title of the episode does not retain the junk text of the original file name.

Planned Features:
1. Automatically look up title names for episodes using online resources such as IMDB or wikipedia
2. Quick title meta-tag editing of .mp4, .avi, and other common video file types
3. Click and Drag abilities for moving files into new folders withint he TreeView UI.
4. If editing an mkv tag results in an error, have the error displayed at the end once all other tags are processed.

Current Issues:
1. Renaming and deleting folders and files at the same time will sometimes cause errors to occur. This is because The folders are renamed before the video files in some cases.
2. Double clicking a folder to edit the name causes the folder to become collapsed in the UI. This is a standard feature of the default TreeView component.

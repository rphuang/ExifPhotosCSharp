# Exif Photos
This is an EXIF Metadata Viewer for Android.
ExifPhotos provides flexible UI and extensible metadata extraction.
* Flexible - customize your own list of tags to display
* Extensible - customize your own metadata mapping to extract
Currently, ExifPhotos uses MetadataExtractor to extract EXIF from file. The data quality of MetadataExtractor is not as good as ExifTool but it is good enough for using in Android to view most important EXIF info.

# Getting Started
1. Get code from https://github.com/rphuang/LibsCSharp Assume that the code is under <parent>\LibsCSharp
2. Get ExifPhotos code. Assume that the code is under <parent>\ExifPhotosCSharp.
3. Build ExifPhotos solution
4. Create docs folder in Android phone and copy exifphotossettings.txt and TagMappings.csv to the docs folder.
5. Deploy

# Configuration
## exifphotossettings.txt file
* NumberOfGrids - number of grids (number of tag-value columns) in UI. should be 1 for phone screen.
* GridWidthRequest - the requested width or each grid
* ImageHeightRequest - the requested image height
* ImageWidthRequest - the requested image width
* IncludeSubFolder - include sub folders when pick a folder
* TagMappingFile - custom tag mapping files with comma separated file names or full pathnames. See next for details.
* Group - defines the tags to be displayed in the grid. The Index property defines which grid (all 0 for the single grid). The Tags property defines comma separated tag names.

## Custom Tag Mappings
The TagMappings.csv contains some examples of custom mappings.
* One line per mapping with format Name,DirectoryName,TagName where
    * Name is the display tag name and the name used in the dictionary
    * DirectoryName is the directory name from MetadataExtractor
    * TagName is the tag name from MetadataExtractor
* Comment line starts with #
* Empty lines are ignored

# Issues & Future Development
* Swipe (left right) gesture is only available on the grid. Not sure why it doesn't work on Image but work on the grid's stack.
* ~~Develop UWP version with special file access code~~ It makes no sense to support UWP since the System.IO cannot be used in UWP.
* Leverage ExifTool (at least on Windows version)
* Open page (PickFolderPage) needs a re-write 


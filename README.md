# Brook's Crappy Photo Library Utilities
Quick &amp; Dirty Console apps I use to manage my photo library after getting fed up with other tools.

Seriously don't use these, destroy your pictures, and then be mad at me.  The only reason this repo is public is because I don't want to pay for it :P

I warned you.

Example Usage Scenario

* In-law hands me disc full of pictures
* `mono PicImport.exe /media/usb /media/raid10/photos`
* This copies all the pics off the disc into /media/raid10/photos/<year>/<month>/<filename>
* If there is a file with the same name it will append _1, _2, _3 etc to the filename to avoid collisions, assuming the files have a different length (they are different).  If they are not different (same length) it will simply skip it.


Example Usage Scenario
* Plex camera upload syncs a bunch of pictures from an iPhone to a local folder.  Those pictures may or may not have been previously imported.
* `mono PicDeduplicate.exe /media/raid10/photos move /media/raid10/duplicates`
* Moves any duplicate files out of the tree in /media/raid10/photos to /media/raid10/duplicates (excludes all subdirs in destination).  Duplicates are detected using filename (to lower) and file size.  No hashing is done, so if you happen to have files of the same name AND length it will happily treat them as duplicates.  you've been warned.
* you can simply remove the duplicates with `mono PicDuplicates.exe /Media/raid10/photos remove` if you like to live dangerously.

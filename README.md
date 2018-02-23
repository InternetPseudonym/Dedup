# Dedup
Deduplicator for arbitrary files (remove identical duplicates)
This software will remove every file in a given directory which is already present within in the very same directory - hence remove exact duplicates.
Duplicate-detection is done via the cryptographic algorithm "[MD5](https://en.wikipedia.org/wiki/MD5)", false-positives should be astronomically rare.

Currently, this app has no features other than detection and file removal - once the progressbar is through all of the duplicates will have been removed automatically; you just need to wait.

It is designed with large filesets in mind, even tens or hundreds of thousands of files should be no problem.<br>
_(In fact even millions of files because a [red-black tree](https://en.wikipedia.org/wiki/Red%E2%80%93black_tree) memory algorithm is used)_
If you ever happen to close the app while its running : dont worry, no harm will be done and it will start over next time you run the app.

Keep in mind : Duplicates will be removed AUTOMATICALLY, **this programm will delete files**

# [DOWNLOAD](releases/0.1)

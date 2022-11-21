# N2HR
NES2 Header program to fix old iNES and Archaic headers.

Uses the nes20db.xml file to compare your .nes files against the database.
Both PRG and CHR parts of the files are checked for: Size, CRC32, SHA1 and SUM16 (when applicable)

This program does NOT overwrite your existing rom files, unless pointed into the same directory where the original files reside.
By default this program will try to create a "Working" and "Defective" folder in the same folder where this program will run from.
Use the "Settings" button to point this program to a "Working" and "Defective" folder, and choose whether you want to preserve your sub folder structure if present.

If you put the nes20db.xml file in the same folder as this program it will try to load it automatically, else you can always point to a folder where the database file resides using the "XML DataFile" button. (provided the file is correct, and layout of the file hasn't changed over time.)

Just point this program to your rom folder using the "ROM Folder" button and hit that "Convert Headers" button!

On a test system it took about 30 seconds for 1210 files spread over 70+ folders.

This program requires .NET5.0 to be installed on the system where it's running from.

If there are any issues or suggestions drop me a line and i'll have a look.

The nes20db.xml data file can be found at: http://forums.nesdev.com/viewtopic.php?f=3&t=19940&p=248905&hilit=Ines+2+script#p248905

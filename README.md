# Rexer
Simple tool for regex renaming of files.

## Help

This tool allows renaming of files using regex expressions!  

The flags you need to provide are:  
-f <folder> -> folder to scan for files  
-p <pattern> -> regex pattern to find  
-r <replace> -> replace string for renaming  
  
For testing a pattern you can use:  
-p <pattern> -> regex pattern to test  
  
You can see the regex groups which would be extracted this way.  
Furthermore you can use the captured groups in the <replace> like "g{id, [optional] padding}"  
Please note that the group indices start with zero and that padding works for numbers only.  

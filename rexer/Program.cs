using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace rexer
{

    public class RenameInfo
    {
        public string OriginalName { get; set; }

        public string NewName { get; set; }

        public string FileFolderPath { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            // Change Exception language to english
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string[] expectedArgs = {"-f","-p", "-r", "-h", "-e"};

            Dictionary<string, string> argsDict;

            try
            {
                argsDict = GetArgValues(expectedArgs, args);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("For help use the -h argument!");
                return;
            }

            if (argsDict.ContainsKey("-h"))
            {
                HelpFunction();
                return;
            }
            try
            {
                string path;

                if (argsDict.ContainsKey("-f"))
                {
                    path = argsDict["-f"];
                }
                else
                {
                    path = Directory.GetCurrentDirectory();
                }

                if (argsDict.ContainsKey("-p") && argsDict.ContainsKey("-r"))
                {
                    PatternReplaceFunction(argsDict, path);
                }
                else if (argsDict.ContainsKey("-p"))
                {
                    CheckPatternFunction(argsDict, path);
                }
                else
                {
                    Console.WriteLine("Not enough argument have been provided. Use the -h argument for help.");
                }
            }catch(IOException ex)
            {
                Console.WriteLine($"Could not access files with following exception: \"{ex.Message}\"");
            }catch(Exception ex)
            {
                Console.WriteLine($"Performing the operation encountered the following exception: \"{ex.Message}\"");
            }

        }

        private static void CheckPatternFunction(Dictionary<string, string> argsDict, string path)
        {
            var pattern = argsDict["-p"];
            var files = IOAccess.GetFilesInPath(path);

            foreach (var file in files)
            {

                var fileInfo = new FileInfo(file);
                var matches = Regex.Match(fileInfo.Name, pattern);

                var groups = matches.Groups.Cast<Group>().Where(g => g.Success).ToList();

                if (matches.Success)
                {
                    var groupString = string.Join(", ", groups);
                    Console.WriteLine($"{fileInfo.Name} -> Groups: {groupString}");
                }
            }
        }

        private static void PatternReplaceFunction(Dictionary<string, string> argsDict, string path)
        {
            var pattern = argsDict["-p"];
            var replace = argsDict["-r"];

            var files = IOAccess.GetFilesInPath(path);

            var matchedNames = new List<RenameInfo>();


            foreach(var file in files)
            {
                var fileInfo = new FileInfo(file);
                var matches = Regex.Match(fileInfo.Name, pattern);

                var groups = matches.Groups.Cast<Group>().Where(g => g.Success).ToArray();

                if (matches.Success)
                {
                    var newName = Replacer.Replace(file, replace, groups);

                    matchedNames.Add(new RenameInfo() { OriginalName = fileInfo.Name, NewName = newName, FileFolderPath = fileInfo.DirectoryName });
                }
            };

            if (argsDict.ContainsKey("-e"))
            {
                matchedNames.ForEach(nm =>
                {       
                    var oldPath = Path.Combine(nm.FileFolderPath, nm.OriginalName);
                    var newPath = Path.Combine(nm.FileFolderPath, nm.NewName);
                    if (File.Exists(newPath))
                    {
                        Console.WriteLine($"renaming {nm.OriginalName} -> {nm.NewName} failed because a file with this name already exists!");
                    }
                    else
                    {
                        File.Move(oldPath, newPath);
                        Console.WriteLine($"renaming {nm.OriginalName} -> {nm.NewName}");
                    }
                });
            }
            else
            {
                matchedNames.ForEach(nm =>
                {
                    Console.WriteLine($"{nm.OriginalName} -> {nm.NewName}");
                });

            }
        }

        private static void HelpFunction()
        {
            var infoString = "This tool allows renaming of files using regex expressions!\n"
                + "The flags you need to provide are: \n\n"
                + "-f <folder> -> folder to scan for files\n"
                + "-p <pattern> -> regex pattern to find\n"
                + "-r <replace> -> replace string for renaming\n\n"
                + "The flag you need to provide to actually rename the files is:\n\n"
                + "-e <execute> -> not only shows but executes the renaming\n\n"
                + "For testing a pattern you can use:\n\n"
                + "-p <pattern> -> regex pattern to test\n\n"
                + "You can see the regex groups which would be extracted this way.\n"
                + "Furthermore you can use the captured groups in the <replace> like \"g{id, [optional] padding}\".\n"
                + "Please note that the group indices start with zero and that padding works for numbers only.\n"
                + "\n";


            Console.WriteLine(infoString);
            return;
        }

        static Dictionary<string, string> GetArgValues(string[] exptectedArgs, string[] args)
        {
            var argDict = new Dictionary<string, string>();
            var argNumber = args.Count();
            for (int i = 0; i < argNumber; i++)
            {
                var arg = args[i];
                var isExpectedArg = exptectedArgs.Where(s1 => s1.ToLower() == arg.ToLower()).Count() > 0;

                if (isExpectedArg)
                {
                    if (argDict.ContainsKey(arg))
                    {
                        throw new Exception($"The argument \"{arg}\" was provided twice!");
                    }

                    if (i + 1 < argNumber)
                    {
                        argDict.Add(arg.ToLower(), args[i + 1]);
                    }
                    else
                    {
                        argDict.Add(arg.ToLower(), string.Empty);
                    }
                }
            }
            return argDict;
        }
    }
}

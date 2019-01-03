using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace rexer
{

    public struct GroupReplacer
    {
        public string OptionsString { get; set; }

        public string[] Options { get; set; }
    }

    public static class Replacer
    {

        private static Regex GroupMatchExpression { get; set; } = new Regex(@"g{(\s*\d\s*|\d\s*,\s*\d)}");

        public static string Replace(string input, string replacer, Group[] groups)
        {
            var groupReplacers = FindGroupReplacers(replacer);

            foreach(var groupReplacer in groupReplacers)
            {
                
                var nOptions = groupReplacer.Options.Select(o => int.Parse(o)).ToArray();
                if (nOptions.Length == 1)
                {
                    var group = groups[nOptions[0]];
                    var groupString = group.Value;

                    replacer = GroupMatchExpression.Replace(replacer, groupString, 1);
                }
                else if(nOptions.Length == 2)
                {
                    try
                    {
                        var group = groups[nOptions[0]];
                        int groupNumber = int.Parse(group.Value);
                        replacer = GroupMatchExpression.Replace(replacer, groupNumber.ToString($"D{nOptions[1]}"), 1);
                    }
                    catch (FormatException ex)
                    {
                        throw new Exception($"The group with number {nOptions[0]} you selected does not only contain a number. Thus padding is not possible.", ex);
                    }
                    catch
                    {
                        throw new Exception($"Error replacing! Does a group with index {nOptions[0]} exist?");
                    }
                }
                else
                {
                    throw new Exception("You need to provide a group id!");
                }
            }


            return replacer;
        }

        private static List<GroupReplacer> FindGroupReplacers(string replacer)
        {
            var paddings = new List<GroupReplacer>();
            var matches = GroupMatchExpression.Matches(replacer);

            foreach(Match match in matches)
            {
                var optionsString = match.Groups[1].Value;
                optionsString = Regex.Replace(optionsString, @"\s+", "");
                var options = optionsString.Split(',');
                paddings.Add(new GroupReplacer() { OptionsString = optionsString, Options = options });

            }
            return paddings;
        }
    }
}

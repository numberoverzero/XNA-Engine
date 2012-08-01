using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
namespace Engine.Utility
{
    /// <summary>
    /// Adds a few handy methods for dealing with strings and regex
    /// </summary>
    public static class StringAndRegexExtensions
    {
        /// <summary>
        /// Returns a Dictionary&lt;CapturingGroupName, CapturedGroupValue&gt;
        /// of the regex of the input string
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> MatchNamedCaptures(this Regex regex, string input)
        {
            var namedCaptureDictionary = new Dictionary<string, string>();
            GroupCollection groups = regex.Match(input).Groups;
            string[] groupNames = regex.GetGroupNames();
            foreach (string groupName in groupNames)
                if (groups[groupName].Captures.Count > 0)
                    namedCaptureDictionary.Add(groupName, groups[groupName].Value);
            return namedCaptureDictionary;
        }

        /// <summary>
        /// Checks if the regex has a named group for the given input string
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="input"></param>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public static bool HasNamedCapture(this Regex regex, string input, string groupname)
        {
            return regex.Match(input).Groups[groupname].Success;
        }

        /// <summary>
        /// If the regex has a named group for the given input string, returns the value of that groupname.
        /// Otherwise, returns null.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="input"></param>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public static string GetNamedCapture(this Regex regex, string input, string groupname)
        {
            var groups = regex.Match(input).Groups;
            return groups[groupname].Value;
        }

        /// <summary>
        /// Generator that yields each line of a file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadLines(this string file)
        {
            string line;
            using (var reader = File.OpenText(file))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Takes an enumerable and returns the string such that each element is printed,
        /// separated by the given separator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join<T>(this string separator, IEnumerable<T> enumerable)
        {
            return enumerable.PrettyPrint(separator);
        }
    }
}

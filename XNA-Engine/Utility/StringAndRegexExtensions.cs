using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Engine.Utility
{
    /// <summary>
    ///   Adds a few handy methods for dealing with strings and regex
    /// </summary>
    public static class StringAndRegexExtensions
    {
        private const string dflt_time_fmt = "HH:mm:ff ";
        private static readonly UnicodeEncoding _uni = new UnicodeEncoding();

        /// <summary>
        ///   Returns a Dictionary&lt;CapturingGroupName, CapturedGroupValue&gt;
        ///   of the regex of the input string
        /// </summary>
        /// <param name="regex"> </param>
        /// <param name="input"> </param>
        /// <returns> </returns>
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
        ///   Checks if the regex has a named group for the given input string
        /// </summary>
        /// <param name="regex"> </param>
        /// <param name="input"> </param>
        /// <param name="groupname"> </param>
        /// <returns> </returns>
        public static bool HasNamedCapture(this Regex regex, string input, string groupname)
        {
            return regex.Match(input).Groups[groupname].Success;
        }

        /// <summary>
        ///   If the regex has a named group for the given input string, returns the value of that groupname.
        ///   Otherwise, returns null.
        /// </summary>
        /// <param name="regex"> </param>
        /// <param name="input"> </param>
        /// <param name="groupname"> </param>
        /// <returns> </returns>
        public static string GetNamedCapture(this Regex regex, string input, string groupname)
        {
            var groups = regex.Match(input).Groups;
            return groups[groupname].Value;
        }

        /// <summary>
        ///   Generator that yields each line of a file
        /// </summary>
        /// <param name="file"> </param>
        /// <returns> </returns>
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
        ///   Takes an enumerable and returns the string such that each element is printed,
        ///   separated by the given separator.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="enumerable"> </param>
        /// <param name="separator"> </param>
        /// <returns> </returns>
        public static string Join<T>(this string separator, IEnumerable<T> enumerable)
        {
            return String.Join(separator, enumerable);
        }

        /// <summary>
        ///   Removes any occurrences of the character sequence trim from the end of the string.
        ///   Uses repeated substring and assignment - do not use for heavy lifting.
        /// </summary>
        /// <param name="source"> </param>
        /// <param name="trim"> </param>
        /// <returns> </returns>
        public static string TrimEnd(this string source, string trim)
        {
            while (source.EndsWith(trim))
            {
                source = source.Substring(0, source.Length - trim.Length);
            }
            return source;
        }

        /// <summary>
        ///   Returns the formatted string.  Lowercase because there's already a static method with this signature
        /// </summary>
        /// <param name="str"> </param>
        /// <param name="args"> </param>
        /// <returns> </returns>
        public static string format(this string str, params object[] args)
        {
            return String.Format(str, args);
        }

        /// <summary>
        ///   Returns the string prepended with the time in the format:
        ///   "HH:mm:ffff "
        /// </summary>
        /// <param name="str"> </param>
        /// <param name="time_fmt"> </param>
        /// <returns> </returns>
        public static string Timestamped(this string str, string time_fmt = null)
        {
            if (time_fmt == null) time_fmt = dflt_time_fmt;
            return DateTime.Now.ToString(time_fmt) + str;
        }

        /// <summary>
        ///   Gets the byte count of the string in the given encoding
        /// </summary>
        /// <param name="str"> </param>
        /// <param name="encoding"> </param>
        /// <returns> </returns>
        public static int ByteCount(this string str, Encoding encoding)
        {
            return encoding.GetByteCount(str);
        }

        /// <summary>
        ///   Returns the substring from the beginning until the first occurance of the character c.
        ///   Returns the whole string if that character isn't found.
        /// </summary>
        public static string Until(this string str, char c)
        {
            if (String.IsNullOrEmpty(str)) return str;
            var index = str.IndexOf(c);
            return index < 0 ? str : str.Substring(0, index);
        }

        public static void AppendToFile(this string msg, string filename)
        {
            using (var file = File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                file.Write(_uni.GetBytes(msg), 0, _uni.GetByteCount(msg));
            }
        }

        public static void AppendLineToFile(this string msg, string filename)
        {
            AppendToFile(msg + "\r\n", filename);
        }

        public static IEnumerable<string> ReadLines(this StreamReader reader)
        {
            string line = null;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        public static void WriteLine(this FileStream stream, string msg)
        {
            msg = msg + "\r\n";
            stream.Write(_uni.GetBytes(msg), 0, _uni.GetByteCount(msg));
        }

        public static string WithTermChar(this string str, char terminatingChar = '\0')
        {
            return str + terminatingChar;
        }

        public static string[] Split(this string str, char c, int count)
        {
            return str.Split(new[] {c});
        }

        public static string[] Trim(this string[] array)
        {
            return array.ToList().Mutate(str => str.Trim()).ToArray();
        }

        public static int ToInt(this string str)
        {
            return Int32.Parse(str);
        }

        public static bool ToBool(this string str)
        {
            return Boolean.Parse(str);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Engine.Utility;

namespace Engine.FileHandlers
{
    /// <summary>
    ///   Handles config file read/write for an extremely basic spec
    /// </summary>
    public class ConfigParser
    {
        private const String HeaderPattern = @"\[(?<header>[^\]]*)\]";

        private readonly char _delim;
        private readonly string _filename;
        private readonly bool _trimWhitespace;
        private Dictionary<string, Dictionary<string, string>> _map;

        public ConfigParser(string filename, char delim = '=', bool trimWhitespace = true)
        {
            Clear();
            _filename = filename;
            _trimWhitespace = trimWhitespace;
            _delim = delim;
            LoadFromFile();
        }

        public string this[string header, string key]
        {
            get { return HasSetting(header, key) ? _map[header][key] : null; }
            set
            {
                if (!HasSection(header)) _map[header] = new Dictionary<string, string>();
                _map[header][key] = value;
            }
        }

        /// <summary>
        /// Shortcut for setting/getting sectionless values
        /// </summary>
        public string this[string key]
        {
            get { return this["", key]; }
            set { this["", key] = value; }
        }

        public void Clear()
        {
            _map = new Dictionary<string, Dictionary<string, string>>();
        }

        public string Get(string header, string key, string defaultStr)
        {
            return this[header, key] ?? defaultStr;
        }

        /// <summary>
        /// Returns the options in a specific section
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetSection(string header)
        {
            return _map.ContainsKey(header) ? _map[header] : null;
        }

        /// <summary>
        /// Returns the sections defined in the config
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Sections { get { return _map.Keys; } }

        public void LoadFromFile()
        {
            Clear();
            var headerRegex = new Regex(HeaderPattern);
            var _buffer = new FullFileBuffer(_filename);

            var header = "";
            foreach (var line in _buffer)
            {
                if (headerRegex.HasNamedCapture(line, "header"))
                    header = headerRegex.GetNamedCapture(line, "header");
                else
                {
                    var keyvalue = line.Split(_delim, 2);
                    
                    if (header == null) continue;
                    if (keyvalue.Length != 2) continue; // Poorly formed line
                    
                    if (_trimWhitespace) keyvalue = keyvalue.Trim();
                    this[header, keyvalue[0]] = keyvalue[1];
                }
            }
        }

        public void SaveToFile()
        {
            var buffer = new FullFileBuffer(_filename);
            if (_map.ContainsKey("")) WriteSection("", buffer); // Settings without a section
            _map.Keys.Where(header => header != "").Each(header => WriteSection(header, buffer));
            buffer.SaveToFile();
        }

        private void WriteSection(string header, FullFileBuffer buffer)
        {
            if (header == null) return;
            if(!String.IsNullOrEmpty(header))
                buffer.Append("[{0}]".format(header));
            var section = _map[header];
            if(section == null) return;
            foreach (var key in section.Keys)
            {
                var value = section[key];
                buffer.Append("{0}{1}{2}".format(key, _delim, value));
            }
            buffer.Append("");
        }

        public bool HasSection(string header)
        {
            return _map.ContainsKey(header);
        }

        public bool HasSetting(string header, string key)
        {
            return HasSection(header) && _map[header].ContainsKey(key);
        }
    }
}
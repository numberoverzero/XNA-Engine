using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Engine.Utility;

namespace Engine.FileHandlers
{
    public class FullFileBuffer : IEnumerable<string>
    {
        private List<string> _lines;

        public FullFileBuffer(string filename)
        {
            Filename = filename;
            LoadFromFile();
        }

        public string Filename { get; set; }

        public void LoadFromFile()
        {
            _lines = File.ReadAllLines(Filename).ToList();
        }

        public void SaveToFile()
        {
            using (var file = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                foreach (var line in _lines) file.WriteLine(line);
            }
        }

        public string ReadLine(int lineno)
        {
            return lineno >= _lines.Count ? null : _lines[lineno];
        }

        public void WriteLine(string line, int lineno)
        {
            while (_lines.Count <= lineno)
                _lines.Add("\r\n");
            _lines[lineno] = line;
        }

        public int LineMatching(string pattern)
        {
            for (var i = 0; i < _lines.Count; i++)
            {
                var line = _lines[i];
                if (Regex.IsMatch(line, pattern))
                    return i;
            }
            return -1;
        }

        public IEnumerable<int> LinesMatching(string pattern)
        {
            for (var i = 0; i < _lines.Count; i++)
            {
                var line = _lines[i];
                if (Regex.IsMatch(line, pattern))
                    yield return i;
            }
        }

        public void Append(string line)
        {
            WriteLine(line, _lines.Count);
        }

        public void Clear()
        {
            _lines.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>) _lines).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
using System;
using Engine.Utility;

namespace Engine.FileHandlers
{
    /// <summary>
    ///   See <see cref="ILog" />
    /// </summary>
    public class FileLog : Log
    {
        /// <summary>
        ///   The file to log to.
        /// </summary>
        public string Filename;

        private Frequency _frequency;

        /// <summary>
        ///   Create a log file at the given location,
        ///   with a given expected frequency of logging
        /// </summary>
        /// <param name="filename"> </param>
        /// <param name="frequency"> </param>
        public FileLog(string filename, Frequency frequency)
        {
            Filename = filename;
            this._frequency = frequency;
        }

        protected override void WriteMsg(string msg)
        {
            if(!String.IsNullOrEmpty(Filename))
                msg.AppendLineToFile(Filename);
        }
    }
}
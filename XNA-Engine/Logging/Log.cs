using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.DataStructures;
using Engine.Utility;

namespace Engine.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class Log
    {
        UnicodeEncoding uniEncoding = new UnicodeEncoding();
        const string fmt = "{0}::{1}";
        string filename;
        FileStream _log;
        FileStream log
        {
            get
            {
                if (_log == null || !_log.CanWrite)
                {
                    _log.Close();
                    _log = File.Open(filename, FileMode.Append);
                }
                return _log;
            }
        }
        Frequency frequency;
        DoubleBuffer<string> buffer;
        int buffsize = 50;

        /// <summary>
        /// Create a log file at the given location,
        /// with a given expected frequency of logging
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="frequency"></param>
        public Log(string filename, Frequency frequency) 
        {
            this.filename = filename;
            this.frequency = frequency;
            buffer = new DoubleBuffer<string>();
        }

        /// <summary>
        /// Write any pending messages to disk
        /// </summary>
        public void Flush()
        {
            // Flip the buffer, grabbing all the messages from the back and
            // still allowing writes
            buffer.Flip();
            foreach (var msg in buffer.Front)
                log.Write(uniEncoding.GetBytes(msg),
                    0, uniEncoding.GetByteCount(msg));
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="msg"></param>
        public void Error(string msg)
        {
            WriteMsg(msg, Level.Error);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="msg"></param>
        public void Warn(string msg)
        {
            WriteMsg(msg, Level.Warning);
        }

        /// <summary>
        /// Log an info message
        /// </summary>
        /// <param name="msg"></param>
        public void Info(string msg)
        {
            WriteMsg(msg, Level.Info);
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="msg"></param>
        public void Debug(string msg)
        {
            WriteMsg(msg, Level.Debug);
        }

        void WriteMsg(string msg, Level level)
        {
            string prefix = "";
            switch (level)
            {
                case Level.Error:
                    prefix = "ERRO";
                    break;
                case Level.Warning:
                    prefix = "WARN";
                    break;
                case Level.Info:
                    prefix = "INFO";
                    break;
                case Level.Debug:
                    prefix = "DEBG";
                    break;
            }

            buffer.Push(fmt.format(prefix, msg));
            if (buffer.BackBufferSize >= buffsize) Flush();
        }
    }
}

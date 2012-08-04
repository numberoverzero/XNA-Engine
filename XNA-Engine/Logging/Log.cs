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
    /// See <see cref="ILog"/>
    /// </summary>
    public class Log : ILog
    {
        UnicodeEncoding uniEncoding = new UnicodeEncoding();
        const string fmt = "{0:s} {1}::{2}";
        /// <summary>
        /// The file to log to.
        /// </summary>
        public string Filename;
        FileStream _log;
        FileStream log
        {
            get
            {
                if (String.IsNullOrEmpty(Filename))
                    return null;
                if (_log == null || !_log.CanWrite)
                {
                    _log.Close();
                    _log = File.Open(Filename, FileMode.Append);
                }
                return _log;
            }
        }
        Frequency frequency;
        List<string> msgs;
        int buffsize = 50;

        /// <summary>
        /// Create a log file at the given location,
        /// with a given expected frequency of logging
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="frequency"></param>
        public Log(string filename, Frequency frequency) 
        {
            this.Filename = filename;
            this.frequency = frequency;
            msgs = new List<string>();
        }

        /// <summary>
        /// Write any pending messages to disk
        /// </summary>
        public virtual void Flush()
        {
            // Flip the buffer, grabbing all the messages from the back and
            // still allowing writes
            var msgsCopy = msgs.ToArray();
            msgs.Clear();
            if (log == null || !log.CanWrite) return;
            foreach (var msg in msgsCopy)
                log.Write(uniEncoding.GetBytes(msg),
                    0, uniEncoding.GetByteCount(msg));
            log.Close();
        }

        /// <summary>
        /// See <see cref="ILog.Error"/>
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Error(string msg)
        {
            WriteMsg(msg, Level.Error);
        }

        /// <summary>
        /// See <see cref="ILog.Warn"/>
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Warn(string msg)
        {
            WriteMsg(msg, Level.Warning);
        }

        /// <summary>
        /// See <see cref="ILog.Info"/>
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Info(string msg)
        {
            WriteMsg(msg, Level.Info);
        }

        /// <summary>
        /// See <see cref="ILog.Debug"/>
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Debug(string msg)
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

            msgs.Add(fmt.format(DateTime.Now, prefix, msg));
            if (msgs.Count >= buffsize) Flush();
        }
    }

    /// <summary>
    /// Used for injecting text into another log message
    /// </summary>
    public abstract class LogLine : ILog
    {
        ILog log;
        internal LogLine(ILog log) { this.log = log;}

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Error(string msg)
        {
            InjectMsg(log.Error, msg);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Warn(string msg)
        {
            InjectMsg(log.Warn, msg);
        }

        /// <summary>
        /// Log an info message
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Info(string msg)
        {
            InjectMsg(log.Info, msg);
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Debug(string msg)
        {
            InjectMsg(log.Debug, msg);
        }

        /// <summary>
        /// Called by Error/Warn/Info/Debug, used for injecting into the message
        /// </summary>
        /// <param name="func"></param>
        /// <param name="msg"></param>
        protected abstract void InjectMsg(Action<string> func, string msg);
    }
}

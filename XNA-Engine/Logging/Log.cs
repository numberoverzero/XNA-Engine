using System;
using Engine.Utility;

namespace Engine.Logging
{
    /// <summary>
    ///   See <see cref="ILog" />
    /// </summary>
    public class Log : ILog
    {
        private const string Fmt = "{0:s} {1}::{2}";

        /// <summary>
        ///   The file to log to.
        /// </summary>
        public string Filename;

        private Frequency frequency;

        /// <summary>
        ///   Create a log file at the given location,
        ///   with a given expected frequency of logging
        /// </summary>
        /// <param name="filename"> </param>
        /// <param name="frequency"> </param>
        public Log(string filename, Frequency frequency)
        {
            Filename = filename;
            this.frequency = frequency;
            Debug("Log:Initialized");
        }

        #region ILog Members

        /// <summary>
        ///   See <see cref="ILog.Error" />
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Error(string msg)
        {
            WriteMsg(msg, Level.Error);
        }

        /// <summary>
        ///   See <see cref="ILog.Warn" />
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Warn(string msg)
        {
            WriteMsg(msg, Level.Warning);
        }

        /// <summary>
        ///   See <see cref="ILog.Info" />
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Info(string msg)
        {
            WriteMsg(msg, Level.Info);
        }

        /// <summary>
        ///   See <see cref="ILog.Debug" />
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Debug(string msg)
        {
            WriteMsg(msg, Level.Debug);
        }

        #endregion

        /// <summary>
        ///   Write any pending messages to disk
        /// </summary>
        public virtual void Flush()
        {
        }

        private void WriteMsg(string msg, Level level)
        {
            if (String.IsNullOrEmpty(msg)) return;
            var prefix = "";
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
            try
            {
                LogWrite(Fmt.format(DateTime.Now, prefix, msg));
            }
            catch
            {
            }
        }

        private void LogWrite(string msg)
        {
            if (String.IsNullOrEmpty(Filename)) return;
            msg.AppendLineToFile(Filename);
        }
    }

    /// <summary>
    ///   Used for injecting text into another log message
    /// </summary>
    public abstract class LogLine : ILog
    {
        private readonly ILog log;

        internal LogLine(ILog log)
        {
            this.log = log;
        }

        #region ILog Members

        /// <summary>
        ///   Log an error message
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Error(string msg)
        {
            InjectMsg(log.Error, msg);
        }

        /// <summary>
        ///   Log a warning message
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Warn(string msg)
        {
            InjectMsg(log.Warn, msg);
        }

        /// <summary>
        ///   Log an info message
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Info(string msg)
        {
            InjectMsg(log.Info, msg);
        }

        /// <summary>
        ///   Log a debug message
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Debug(string msg)
        {
            InjectMsg(log.Debug, msg);
        }

        #endregion

        /// <summary>
        ///   Called by Error/Warn/Info/Debug, used for injecting into the message
        /// </summary>
        /// <param name="func"> </param>
        /// <param name="msg"> </param>
        protected abstract void InjectMsg(Action<string> func, string msg);
    }
}
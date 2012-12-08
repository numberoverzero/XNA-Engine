using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Utility;

namespace Engine.FileHandlers
{
    public abstract class Log : ILog
    {
        private const string Fmt = "{0:s} {1}::{2}";
        protected List<ILog> Mirrors;
 
        public Log()
        {
            Mirrors = new List<ILog>();
        }

        /// <summary>
        ///   See <see cref="ILog.Error" />
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Error(string msg)
        {
            WriteMsgWithLevel(msg, Level.Error);
        }

        /// <summary>
        ///   See <see cref="ILog.Warn" />
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Warn(string msg)
        {
            WriteMsgWithLevel(msg, Level.Warning);
        }

        /// <summary>
        ///   See <see cref="ILog.Info" />
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Info(string msg)
        {
            WriteMsgWithLevel(msg, Level.Info);
        }

        /// <summary>
        ///   See <see cref="ILog.Debug" />
        /// </summary>
        /// <param name="msg"> </param>
        public virtual void Debug(string msg)
        {
            WriteMsgWithLevel(msg, Level.Debug);
        }

        private void WriteMsgWithLevel(string msg, Level level)
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
            WriteLine(Fmt.format(DateTime.Now, prefix, msg));
        }

        public void WriteLine(string msg)
        {
            WriteMsg(msg);
            foreach (var mirror in Mirrors)
                mirror.WriteLine(msg);
        }

        protected abstract void WriteMsg(string msg);

        public void AddMirror(ILog other)
        {
            Mirrors.Add(other);
        }

        public void RemoveMirror(ILog other)
        {
            Mirrors.Remove(other);
        }
    }
}

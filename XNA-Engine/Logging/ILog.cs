using System;
namespace Engine.FileHandlers
{
    /// <summary>
    /// Logging interface
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="msg"></param>
        void Debug(string msg);
        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="msg"></param>
        void Error(string msg);
        /// <summary>
        /// Log an info message
        /// </summary>
        /// <param name="msg"></param>
        void Info(string msg);
        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="msg"></param>
        void Warn(string msg);
    }
}

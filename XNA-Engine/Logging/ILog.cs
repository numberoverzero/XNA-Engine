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

        void WriteLine(string msg);

        /// <summary>
        /// <para>Add an ILog as a mirror of this one.</para>
        /// <para>Any message which this log receives, the other will as well.</para>
        /// </summary>
        /// <param name="other"></param>
        void AddMirror(ILog other);

        /// <summary>
        /// <para>Remove a mirror.</para>
        /// </summary>
        /// <param name="other"></param>
        void RemoveMirror(ILog other);
    }
}

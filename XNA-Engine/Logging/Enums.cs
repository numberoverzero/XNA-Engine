using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Logging
{
    /// <summary>
    /// The expected rate of message writing.
    /// Can be used to determine how to batch writes
    /// </summary>
    public enum Frequency
    {
        /// <summary>
        /// Unknown or random frequency
        /// </summary>
        None,
        
        /// <summary>
        /// Rarely writing
        /// </summary>
        Low,

        /// <summary>
        /// Writing constantly
        /// </summary>
        Moderate,

        /// <summary>
        /// Multiple inputs writing constantly
        /// </summary>
        High,

        /// <summary>
        /// Infrequent, high-bandwidth writing
        /// </summary>
        Burst
    }

    /// <summary>
    /// A log message's severity level
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// An error occured
        /// </summary>
        Error,

        /// <summary>
        /// A warning
        /// </summary>
        Warning,

        /// <summary>
        /// An informational message
        /// </summary>
        Info,

        /// <summary>
        /// Debug information
        /// </summary>
        Debug
    }
}

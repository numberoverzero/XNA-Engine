using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Events
{
    /// <summary>
    /// The arguments of a GameEvent
    /// </summary>
    public class GameEventArgs
    {
        /// <summary>
        /// Message describing the event or providing additional metadata
        /// </summary>
        public string msg { get; protected set; }

        /// <summary>
        /// Empty GameEvent
        /// </summary>
        public GameEventArgs() : this("") { }
        /// <summary>
        /// GameEvent with a given message
        /// </summary>
        /// <param name="msg"></param>
        public GameEventArgs(string msg)
        {
            this.msg = msg;
        }

    }
}

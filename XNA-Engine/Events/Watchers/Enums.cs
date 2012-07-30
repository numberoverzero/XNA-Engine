using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Events.EventWatchers
{
    /// <summary>
    /// Describes at which point in an event's lifetime a watcher would want to be notified
    /// of the event.
    /// </summary>
    public enum EventWatchTiming
    {
        /// <summary>
        /// The watcher doesn't want to know about events
        /// </summary>
        None,
        
        /// <summary>
        /// When the event is created
        /// </summary>
        OnCreate,
        
        /// <summary>
        /// When the event is fired by an event manager
        /// </summary>
        OnFire,
        
        /// <summary>
        /// The watcher wants to be notified at any time.
        /// (This will either notifiy at every stage, or the first, depending on implementation
        /// </summary>
        Any
    }
}

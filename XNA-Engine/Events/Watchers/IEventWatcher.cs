using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Events.EventWatchers
{
    /// <summary>
    /// Watches events and reacts to them.
    /// 
    /// Can respond to events when they are created, when they're fired, when they die, etc...
    /// </summary>
    public interface IEventWatcher
    {
        /// <summary>
        /// The time in an even'ts life which the watcher is interested in
        /// </summary>
        /// <returns></returns>
        EventWatchTiming GetWatchTiming();
        /// <summary>
        /// Inspect an event and possibly react to it.
        /// </summary>
        /// <param name="gameEvent"></param>
        void InspectEvent(GameEvent gameEvent);
    }
}

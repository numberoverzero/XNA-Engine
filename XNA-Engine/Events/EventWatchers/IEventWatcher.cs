using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Events.EventWatchers
{
    public interface IEventWatcher
    {
        EventWatchTiming GetWatchTiming();
        void InspectEvent(GameEvent gameEvent);
    }
}

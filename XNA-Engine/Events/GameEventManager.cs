using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events.EventWatchers;

namespace Engine.Events
{
    public class GameEventManager
    {
        #region Singleton

        static GameEventManager globalGameEventManager;
        public static GameEventManager GlobalEventManager
        {
            get
            {
                if (globalGameEventManager == null)
                    globalGameEventManager = new GameEventManager();
                return globalGameEventManager;
            }
        }

        #endregion

        #region Fields

        protected bool isUpdating;
        protected List<GameEvent> events;
        protected List<GameEvent> queuedEvents;
        protected List<GameEvent> completedEvents;
        protected List<IEventWatcher> watchers;

        #endregion

        #region Initialization

        public GameEventManager()
        {
            isUpdating = false;
            events = new List<GameEvent>();
            queuedEvents = new List<GameEvent>();
            completedEvents = new List<GameEvent>();
            watchers = new List<IEventWatcher>();
        }

        #endregion

        #region Add/Remove Events

        public virtual void AddEvent(GameEvent gameEvent) {
            List<GameEvent> eventList = isUpdating ? queuedEvents : events;
            eventList.Add(gameEvent);
            CheckWatchers(gameEvent, EventWatchTiming.OnCreate);
        }

        public virtual void RemoveEvent(GameEvent gameEvent)
        {
            events.Remove(gameEvent);
        }

        #endregion

        #region Add/Remove Watchers

        public virtual void AddWatcher(IEventWatcher watcher) {
            watchers.Add(watcher);
        }

        public virtual void RemoveWatcher(IEventWatcher watcher) {
            watchers.Remove(watcher);
        }

        #endregion

        public void Update(float dt) {
            isUpdating = true;
            foreach (var gameEvent in events) {
                gameEvent.Update(dt);
                if (gameEvent.HasFired) {
                    completedEvents.Add(gameEvent);
                    CheckWatchers(gameEvent, EventWatchTiming.OnFire);
                }
            }
            isUpdating = false;

            RemoveCompletedEvents();
            if (queuedEvents.Count() > 0) {
                AddQueuedEvents();
                //Call update with no change in time, to give added events that are 'instant' a chance to fire
                Update(0);
            }
        }

        #region private Helpers

        private void CheckWatchers(GameEvent gameEvent, EventWatchTiming timing)
        {
            EventWatchTiming watcherTiming;
            foreach (var watcher in watchers)
            {
                watcherTiming = watcher.GetWatchTiming();
                if ((watcherTiming == timing) || (watcherTiming == EventWatchTiming.Any))
                    watcher.InspectEvent(gameEvent);
            }
        }

        private void RemoveCompletedEvents() {
            foreach (var gameEvent in completedEvents)
                events.Remove(gameEvent);
            completedEvents.Clear();
        }

        private void AddQueuedEvents() {
            foreach (var gameEvent in queuedEvents)
                events.Add(gameEvent);
            queuedEvents.Clear();
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events.EventWatchers;

namespace Engine.Events
{
    /// <summary>
    /// Manages and dispatches game events, and
    /// notifies any interested watchers
    /// </summary>
    public class GameEventManager
    {
        #region Singleton

        static GameEventManager globalGameEventManager;
        /// <summary>
        /// A default GameEventManager.  Its use is discouraged
        /// except for those things which are guaranteed to only ever
        /// happen in one context (such as game state changes between menus, etc)
        /// </summary>
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

        /// <summary>
        /// Used to make sure we don't try to add events to the queue as we're processing it.
        /// </summary>
        protected bool isUpdating;
        /// <summary>
        /// Events tracked by the Manager
        /// </summary>
        protected List<GameEvent> events;
        /// <summary>
        /// Events added during the update loop that should be processed
        /// </summary>
        protected List<GameEvent> queuedEvents;
        /// <summary>
        /// Events that have fired and can be removed from the manager at the end of the update loop
        /// </summary>
        protected List<GameEvent> completedEvents;
        /// <summary>
        /// Watchers that should be notified of different event states (Create, Fire)
        /// </summary>
        protected List<IEventWatcher> watchers;

        #endregion

        #region Initialization

        /// <summary>
        /// Create a default GameEventManager
        /// </summary>
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

        /// <summary>
        /// Add an event to the manager.  If called during an update loop, the event will be added to the list of
        /// queued events, and will be updated with a time delta of 0 at the end of the current loop.
        /// </summary>
        /// <param name="gameEvent"></param>
        public virtual void AddEvent(GameEvent gameEvent) {
            List<GameEvent> eventList = isUpdating ? queuedEvents : events;
            eventList.Add(gameEvent);
            CheckWatchers(gameEvent, EventWatchTiming.OnCreate);
        }

        /// <summary>
        /// Remove an event from the Manager
        /// </summary>
        /// <param name="gameEvent"></param>
        public virtual void RemoveEvent(GameEvent gameEvent)
        {
            events.Remove(gameEvent);
        }

        #endregion

        #region Add/Remove Watchers

        /// <summary>
        /// Add a watcher to the manager
        /// </summary>
        /// <param name="watcher"></param>
        public virtual void AddWatcher(IEventWatcher watcher) {
            watchers.Add(watcher);
        }

        /// <summary>
        /// Remove a watcher from the manager
        /// </summary>
        /// <param name="watcher"></param>
        public virtual void RemoveWatcher(IEventWatcher watcher) {
            watchers.Remove(watcher);
        }

        #endregion

        /// <summary>
        /// Update the manager, updating any tracked events, firing any that need to, and removing fired events
        /// </summary>
        /// <param name="dt"></param>
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

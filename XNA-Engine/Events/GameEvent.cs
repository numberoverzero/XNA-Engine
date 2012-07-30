using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Events
{
    /// <summary>
    /// An event fired relating to some change in game state.
    /// Can be used for signaling changes in state, units,
    /// environment, etc
    /// </summary>
    public class GameEvent
    {
        /// <summary>
        /// The firing GameObject (may be null)
        /// </summary>
        public GameObject src;
        /// <summary>
        /// The receiving GameObject (may be null)
        /// </summary>
        public GameObject dst;
        /// <summary>
        /// Any arguments related to the event
        /// </summary>
        public GameEventArgs args;

        float timeToSend;
        
        /// <summary>
        /// Whether or not the event has been fired
        /// (at least once)
        /// </summary>
        public bool HasFired { get; protected set; }

        /// <summary>
        /// Construct a GameEvent that will be fired after timeToSend has elapsed
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <param name="args"></param>
        /// <param name="timeToSend"></param>
        public GameEvent(GameObject src, GameObject dst, GameEventArgs args, float timeToSend)
        {
            this.src = src;
            this.dst = dst;
            this.args = args;
            this.timeToSend = timeToSend;
            HasFired = timeToSend < 0 ? true : false;
        }

        /// <summary>
        /// Update the GameEvent, and fire it if the elapsed time since creation
        /// is >= timeToSend
        /// </summary>
        /// <param name="dt"></param>
        public void Update(float dt)
        {
            timeToSend -= dt;
            if (!HasFired && timeToSend <= 0)
                Fire();
        }

        private void Fire()
        {
            HasFired = true;
            if (dst != null)
                dst.OnGameEvent(src, args);
        }
    }
}

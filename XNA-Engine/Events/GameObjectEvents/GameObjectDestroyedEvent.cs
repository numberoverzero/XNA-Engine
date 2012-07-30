using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Events.GameObjectEvents
{
    /// <summary>
    /// Event when a GameObject is destroyed
    /// </summary>
    public class GameObjectDestroyedEvent : GameEvent
    {
        /// <summary>
        /// The GameObject destroyed
        /// </summary>
        public GameObject GameObject { get; protected set; }
        /// <summary>
        /// Construct an event when a GameObject is destroyed.
        /// It is not required that object destruction fire an event
        /// </summary>
        /// <param name="src"></param>
        public GameObjectDestroyedEvent(GameObject src)
            :base(src, null, null, 0)
        {
            GameObject = src;
        }
    }
}

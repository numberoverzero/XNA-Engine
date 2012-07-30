using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Events.GameObjectEvents
{
    /// <summary>
    /// Event when a GameObject is created
    /// </summary>
    public class GameObjectCreatedEvent : GameEvent
    {
        /// <summary>
        /// The GameObject created
        /// </summary>
        public GameObject GameObject { get; protected set; }
        /// <summary>
        /// Construct an event when a GameObject is created.
        /// It is not required that object creation fire an event
        /// </summary>
        /// <param name="src"></param>
        public GameObjectCreatedEvent(GameObject src)
            :base(src, null, null, 0)
        {
            GameObject = src;
        }
    }
}

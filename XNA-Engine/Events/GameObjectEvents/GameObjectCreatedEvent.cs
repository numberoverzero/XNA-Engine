using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Events.GameObjectEvents
{
    public class GameObjectCreatedEvent : GameEvent
    {
        public GameObject GameObject { get; protected set; }
        public GameObjectCreatedEvent(GameObject src)
            :base(src, null, null, 0)
        {
            GameObject = src;
        }
    }
}

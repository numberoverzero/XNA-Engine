using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Events
{
    public class GameEvent
    {
        public GameObject src, dst;
        public GameEventArgs args;
        float timeToSend;
        public bool HasFired { get; protected set; }

        public GameEvent(GameObject src, GameObject dst, GameEventArgs args, float timeToSend)
        {
            this.src = src;
            this.dst = dst;
            this.args = args;
            this.timeToSend = timeToSend;
            HasFired = timeToSend < 0 ? true : false;
        }

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

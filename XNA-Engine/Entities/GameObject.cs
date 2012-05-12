using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities.Components;
using Microsoft.Xna.Framework;
using Engine.Entities.Behaviors;

namespace Engine.Entities
{
    public class GameObject
    {
        public PhysicsComponent PhysicsComponent { get; protected set; }

        public List<IBehavior> Behaviors { get; protected set; }

        public GameObject()
        {
            PhysicsComponent = new PhysicsComponent();
            Behaviors = new List<IBehavior>();
        }

        public virtual void AddBehavior(IBehavior behavior)
        {
            Behaviors.Add(behavior);
        }

        public virtual void RemoveBehavior(IBehavior behavior)
        {
            Behaviors.Remove(behavior);
        }

        public virtual void Update(float dt)
        {
            UpdateBehaviors(dt);
        }

        protected virtual void UpdateBehaviors(float dt)
        {
            foreach (var behavior in Behaviors)
            {
                behavior.Update(dt);
                if(behavior.MeetsCriteria(this))
                    behavior.Apply(this);
            }
        }

    }
}

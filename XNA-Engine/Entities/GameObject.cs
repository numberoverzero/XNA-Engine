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
        #region Fields

        public PhysicsComponent PhysicsComponent { get; protected set; }
        public List<IBehavior> Behaviors { get; protected set; }
        public bool Active { get; protected set; }
        public float Timescale { get; set; }
        public int Health { get; set; }

        #endregion

        #region Initialization

        public GameObject()
        {
            PhysicsComponent = new PhysicsComponent();
            Behaviors = new List<IBehavior>();
        }

        public GameObject(GameObject other)
        {
            PhysicsComponent = new PhysicsComponent(other.PhysicsComponent);
            Behaviors = new List<IBehavior>(other.Behaviors);

            Active = other.Active;
            Timescale = other.Timescale;
            Health = other.Health;
        }

        #endregion

        #region Behaviors

        public virtual void AddBehavior(IBehavior behavior)
        {
            Behaviors.Add(behavior);
        }

        public virtual void RemoveBehavior(IBehavior behavior)
        {
            Behaviors.Remove(behavior);
        }

        protected virtual void UpdateBehaviors(float dt)
        {
            foreach (var behavior in Behaviors)
            {
                behavior.Update(dt);
                if (behavior.MeetsCriteria(this))
                    behavior.Apply(this);
            }
        }

        #endregion

        public virtual void Update(float dt)
        {
            UpdateBehaviors(dt);
        }
    }
}

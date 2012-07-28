using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Utility;

namespace Engine.Entities.Behaviors
{
    /// <summary>
    /// Always updates the PhysicsComponent's rotation to align with the object's velocity
    /// </summary>
    public class PrimitiveSteeringBehavior : IBehavior
    {
        bool isEnabled = true;

       void IBehavior.Apply(GameObject gameObject)
        {
            gameObject.PhysicsComponent.Rotation = gameObject.PhysicsComponent.Velocity.AsAngle();
        }

        void IBehavior.Destroy()
        {
            ((IBehavior)this).Enabled = false;
        }

        bool IBehavior.MeetsCriteria(GameObject gameObject)
        {
            return ((IBehavior)this).Enabled;
        }

        void IBehavior.Update(float dt) { }


        bool IBehavior.Enabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }
    }
}

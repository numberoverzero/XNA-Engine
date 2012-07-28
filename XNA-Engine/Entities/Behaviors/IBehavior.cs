using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Entities.Behaviors
{
    /// <summary>
    /// A behavior which can be applied to a GameObject if another GameObject meets the
    /// Behavior's criteria.  Note that the GameObjects being checked and applied can be the same.
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// Apply the Behavior to a GameObject.
        /// This method assumes that the behavior's criteria have been met
        /// </summary>
        /// <param name="gameObject"></param>
        void Apply(GameObject gameObject);
        /// <summary>
        /// Destroy the Behavior
        /// </summary>
        void Destroy();
        /// <summary>
        /// Set/Get the state of the Behavior.
        /// Enabled should be checked before applying a Behavior,
        /// but such a check isn't strictly required.
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// Whether or not a given GameObject meets this Behavior's criteria
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        bool MeetsCriteria(GameObject gameObject);
        /// <summary>
        /// Updates any temporal data the Behavior tracks, which can be used when
        /// checking critera or applying the behavior.  Behaviors do not necessarily
        /// use this information.
        /// </summary>
        /// <param name="dt"></param>
        void Update(float dt);
    }
}

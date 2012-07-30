using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Utility;

namespace Engine.Entities.Components
{
    /// <summary>
    /// Handles all physics interactions of a GameObject
    /// - or a piece or collection of (a) GameObject(s).
    /// </summary>
    public class PhysicsComponent
    {
        #region Fields

        const float DEFAULT_ACCEL_DECAY = 0.0f;
        /// <summary>
        /// Dimensions of the AABB of the component
        /// </summary>
        public Vector2 Dimensions { get; protected set; }
        /// <summary>
        /// Position of the component (depending on implementation,
        /// can be center or corner of component)
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// Velocity of the component
        /// </summary>
        public Vector2 Velocity { get; set; }
        /// <summary>
        /// Acceleration of the component
        /// </summary>
        public Vector2 Acceleration { get; set; }
        /// <summary>
        /// Percent of acceleration that decays per second
        /// </summary>
        public Vector2 DecayAcceleration { get; protected set; }
        /// <summary>
        /// Mass of the component
        /// (objects are usually treated as point masses)
        /// </summary>
        public float Mass { get; protected set; }
        /// <summary>
        /// Maximum magnitude of the component
        /// </summary>
        public float MaxSpeed { get; protected set; }
        /// <summary>
        /// Rotation (counter-clockwise from +x) of the component
        /// </summary>
        public double Rotation { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Create a default Phyics Component
        /// </summary>
        public PhysicsComponent()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Acceleration = new Vector2(DEFAULT_ACCEL_DECAY, DEFAULT_ACCEL_DECAY);
            Dimensions = Vector2.Zero;
            DecayAcceleration = Vector2.Zero;
            Rotation = 0;
            Mass = 1;
            MaxSpeed = float.MaxValue;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public PhysicsComponent(PhysicsComponent other)
        {
            Position = other.Position.Copy();
            Velocity = other.Velocity.Copy();
            Acceleration = other.Acceleration.Copy();
            Dimensions = other.Dimensions.Copy();
            DecayAcceleration = other.DecayAcceleration.Copy();
            Rotation = other.Rotation;
            Mass = other.Mass;
            MaxSpeed = other.MaxSpeed;
        }

        #endregion
    }
}

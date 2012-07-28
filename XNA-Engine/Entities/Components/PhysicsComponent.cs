using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.DataStructures;

namespace Engine.Entities.Components
{
    public class PhysicsComponent
    {
        #region Fields

        const float DEFAULT_ACCEL_DECAY = 0.0f;
        
        public Vector2 Dimensions { get; protected set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 DecayAcceleration { get; protected set; }
        public float Mass { get; protected set; }
        public float MaxSpeed { get; protected set; }
        public double Rotation { get; set; }

        #endregion

        #region Initialization

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

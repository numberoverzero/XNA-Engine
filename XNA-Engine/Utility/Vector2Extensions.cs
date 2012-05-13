using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Engine.Utility
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Returns the angle of the vector, counter-clockwise from +x
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static double AsAngle(this Vector2 direction)
        {
            return Math.Atan2(direction.Y, direction.X);
        }

        /// <summary>
        /// Returns a copy of the vector
        /// </summary>
        /// <param name="other">The vector to copy</param>
        /// <returns></returns>
        public static Vector2 Copy(this Vector2 other)
        {
            return new Vector2(other.X, other.Y);
        }

        /// <summary>
        /// Returns a Vector2 of the given vector rotated about an origin, by an angle theta (in raidans)
        /// </summary>
        /// <param name="vector">The vector to rotate</param>
        /// <param name="origin">The point that the given vector is rotated about</param>
        /// <param name="theta">The angle (in radians) to rotate by</param>
        /// <returns></returns>
        public static Vector2 RotateAbout(this Vector2 vector, Vector2 origin, float theta)
        {
            Vector2 outvec = Vector2.Zero;
            outvec.X = origin.X + (float)(Math.Cos(theta) * (vector.X - origin.X) - Math.Sin(theta) * (vector.Y - origin.Y));
            outvec.Y = origin.Y + (float)(Math.Sin(theta) * (vector.X - origin.X) + Math.Cos(theta) * (vector.Y - origin.Y));
            return outvec;
        }
    }
}

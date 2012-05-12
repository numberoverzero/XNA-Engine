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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Engine.Utility
{
    /// <summary>
    /// Provides iteration over the values of an enumeration
    /// </summary>
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    /// <summary>
    /// Variables useful for enumeration and compatability
    /// across various systems
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Provides an in-order [1-4] list of PlayerIndex for loops
        /// </summary>
        public static readonly PlayerIndex[] Players = new PlayerIndex[] { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Engine
{
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Entities
{
    /// <summary>
    /// Describes the hostility relation between GameObject(s)
    /// </summary>
    public enum Hostility
    {
        /// <summary>
        /// There is no defined relation between the object(s)
        /// </summary>
        None,
        /// <summary>
        /// All relations apply between the object(s)
        /// </summary>
        Any,
        /// <summary>
        /// The object(s) are player controlled or can be considered part of the player
        /// </summary>
        Player,
        /// <summary>
        /// The object(s) are friendly with each other
        /// </summary>
        Friend,
        /// <summary>
        /// The object(s) are neutral with each other
        /// </summary>
        Neutral,
        /// <summary>
        /// The object(s) are unfriendly with each other
        /// </summary>
        Unfriendly
    }
}

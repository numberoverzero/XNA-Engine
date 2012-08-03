using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
    /// <summary>
    /// A Binding for input that can be checked for activity, using states for the keyboard, mouse, and gamepads.
    /// </summary>
    public interface IBinding
    {
        /// <summary>
        /// True if the InputBinding is active in the given FrameState of the given InputManager
        /// </summary>
        /// <param name="manager">The manager keeping track of current/previous input states</param>
        /// <param name="player">Player to check binding on</param>
        /// <param name="state">Current or Previous frame</param>
        bool IsActive(DefaultInputManager manager, PlayerIndex player, FrameState state);

        /// <summary>
        /// Returns the list of modifiers necessary to be active before the binding is considered "active"
        /// </summary>
        IBinding[] Modifiers {get;}
    }
}

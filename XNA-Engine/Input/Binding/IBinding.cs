using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <param name="state">Current or Previous frame</param>
        /// <param name="manager">The manager keeping track of current/previous input states</param>
        /// <remarks>
        /// At first I wasn't comfortable with passing the entire InputManager around, but on the plus side,
        /// we can now easily mock up input.  Woohoo!
        /// </remarks>
        bool IsActive(InputManager manager, FrameState state);

        /// <summary>
        /// True if the binding (without modifiers) is active.  In general, one should check if a binding is active through IsActive.
        /// Subclasses of InputBinding should override IsRawBindingActive, which is called from IsActive.
        /// </summary>
        /// <param name="keyState">KeybardState to check binding against</param>
        /// <param name="gamepadState">GamePadState to check binding against</param>
        /// <param name="mouseState">MouseState to check binding against</param>
        /// <param name="settings">Settings to use when checking thresholds, etc</param>
        bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings);

        /// <summary>
        /// Returns the list of modifiers necessary to be active before the binding is considered "active"
        /// </summary>
        IBinding[] Modifiers {get;}
    }
}

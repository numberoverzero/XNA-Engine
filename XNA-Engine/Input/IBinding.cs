using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
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
        /// <returns></returns>
        bool IsActive(InputManager manager, FrameState state);

        /// <summary>
        /// True if the binding (without modifiers) is active.  In general, one should check if a binding is active through IsActive.
        /// Subclasses of InputBinding should override IsRawBindingActive, which is called from IsActive.
        /// </summary>
        /// <param name="keyState">KeybardState to check binding against</param>
        /// <param name="gamepadState">GamePadState to check binding against</param>
        /// <param name="mouseState">MouseState to check binding against</param>
        /// <param name="settings">Settings to use when checking thresholds, etc</param>
        /// <returns></returns>
        bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings);

        /// <summary>
        /// Checks if the modifiers that this binding requires 
        /// match the active state of the modifiers in the given KeyboardState
        /// </summary>
        /// <remarks>
        /// I suspect it should be possible to streamline this in someway while still keeping the system easily extendable.
        /// We're O(n) and we can reasonably assume n is smaller than 10, but even still, it looks ugly.
        /// </remarks>
        /// <param name="keyState">The KeyboardState to check modifiers against</param>
        /// <returns>True if only all required modifiers are active</returns>
        bool AreExactModifiersActive(KeyboardState keyState);
    }
}

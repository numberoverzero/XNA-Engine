#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Engine.Input
{
    /// <summary>
    /// Supports checking if a particular input is active,
    /// given states for keyboards, mice, and gamepads.
    /// </summary>
    public class InputBinding : IBinding
    {
        /// <summary>
        /// Any Modifiers required for this binding to be considered 'active'
        /// </summary>
        public IBinding[] Modifiers { get; private set; }

        #region Initialiation

        /// <summary>
        /// Initialize an InputBinding with an optional list of required modifiers
        /// </summary>
        /// <param name="modifiers">Optional modifiers- Ctrl, Alt, Shift</param>
        /// 
        public InputBinding(params IBinding[] modifiers)
        {
            Modifiers = new IBinding[modifiers.Length];
            Array.Copy(modifiers, Modifiers, modifiers.Length);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public InputBinding(InputBinding other) : this(other.Modifiers) { }

        #endregion

        #region IsActive Methods

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
        public bool IsActive(InputManager manager, FrameState state)
        {
            KeyboardState keyState = state == FrameState.Current ? manager.CurrentKeyboardState : manager.PreviousKeyboardState;
            GamePadState gamepadState = state == FrameState.Current ? manager.CurrentGamePadState : manager.PreviousGamePadState;
            MouseState mouseState = state == FrameState.Current ? manager.CurrentMouseState : manager.PreviousMouseState;
            return IsRawBindingActive(keyState, gamepadState, mouseState, manager.Settings);
        }

        /// <summary>
        /// True if the binding (without modifiers) is active
        /// </summary>
        /// <param name="keyState"></param>
        /// <param name="gamepadState"></param>
        /// <param name="mouseState"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public virtual bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            return false;
        }

        #endregion
    }
}

#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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
        /// Any modifiers required for this binding to be considered 'active'
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
        /// <param name="manager">The manager keeping track of current/previous input states</param>
        /// <param name="player">Player to check binding on</param>
        /// <param name="state">Current or Previous frame</param>
        /// <returns>True if the binding is active</returns>
        public bool IsActive(DefaultInputManager manager, PlayerIndex player, FrameState state)
        {
            var keyState = state == FrameState.Current ? manager.CurrentKeys : manager.PreviousKeys;
            GamePadState gamepadState = state == FrameState.Current ? manager.CurrentGamePadStates[player] : manager.PreviousGamePadStates[player];
            MouseState mouseState = state == FrameState.Current ? manager.CurrentMouseState : manager.PreviousMouseState;
            return IsRawBindingActive(keyState, gamepadState, mouseState, manager.Settings);
        }

        /// <summary>
        /// True if the binding (without modifiers) is active
        /// </summary>
        protected virtual bool IsRawBindingActive(ISet<Keys> keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            return false;
        }

        #endregion
    }
}

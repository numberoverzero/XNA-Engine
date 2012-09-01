using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input.Devices
{
    /// <summary>
    ///   Tracks keyboard state
    /// </summary>
    public class KeyboardDevice : InputDevice
    {
        /// <summary>
        ///   KeyboardState for the previous frame
        /// </summary>
        public KeyboardState PreviousKeyboardState { get; protected set; }

        /// <summary>
        ///   KeyboardState for the current frame
        /// </summary>
        public KeyboardState CurrentKeyboardState { get; protected set; }

        #region InputDevice Members

        /// <summary>
        ///   See <see cref="InputDevice.Update" />
        /// </summary>
        public void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
        }

        /// <summary>
        ///   See <see cref="InputDevice.GetDeviceSnapshot" />
        /// </summary>
        public InputSnapshot GetDeviceSnapshot(PlayerIndex player, FrameState frameState)
        {
            var keyboardState = frameState == FrameState.Current ? CurrentKeyboardState : PreviousKeyboardState;
            return new InputSnapshot(keyboardState, null, null, null);
        }

        #endregion
    }

    /// <summary>
    ///   Tracks mouse state
    /// </summary>
    public class MouseDevice : InputDevice
    {
        /// <summary>
        ///   MouseState for the previous frame
        /// </summary>
        public MouseState PreviousMouseState { get; protected set; }

        /// <summary>
        ///   MouseState for the current frame
        /// </summary>
        public MouseState CurrentMouseState { get; protected set; }

        #region InputDevice Members

        /// <summary>
        ///   See <see cref="InputDevice.Update" />
        /// </summary>
        public void Update()
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        /// <summary>
        ///   See <see cref="InputDevice.GetDeviceSnapshot" />
        /// </summary>
        public InputSnapshot GetDeviceSnapshot(PlayerIndex player, FrameState frameState)
        {
            var mouseState = frameState == FrameState.Current ? CurrentMouseState : PreviousMouseState;
            return new InputSnapshot(null, null, mouseState, null);
        }

        #endregion
    }

    /// <summary>
    ///   Tracks GamePad states
    /// </summary>
    public class GamePadDevice : InputDevice
    {
        /// <summary>
        ///   GamePadState for the previous frame
        /// </summary>
        public Dictionary<PlayerIndex, GamePadState> PreviousGamePadStates { get; protected set; }

        /// <summary>
        ///   GamePadState for the current frame
        /// </summary>
        public Dictionary<PlayerIndex, GamePadState> CurrentGamePadStates { get; protected set; }

        #region InputDevice Members

        /// <summary>
        ///   See <see cref="InputDevice.Update" />
        /// </summary>
        public void Update()
        {
            foreach (var player in Globals.Players)
            {
                PreviousGamePadStates[player] = CurrentGamePadStates[player];
                CurrentGamePadStates[player] = GamePad.GetState(player);
            }
        }

        /// <summary>
        ///   See <see cref="InputDevice.GetDeviceSnapshot" />
        /// </summary>
        public InputSnapshot GetDeviceSnapshot(PlayerIndex player, FrameState frameState)
        {
            var gamePadState = frameState == FrameState.Current
                                   ? CurrentGamePadStates[player]
                                   : PreviousGamePadStates[player];
            return new InputSnapshot(null, gamePadState, null, null);
        }

        #endregion
    }
}
#region Using Statements

using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Engine.Input
{
    public partial class InputManager{
        #region Fields

        private KeyboardState LastKeyboardState;
        private KeyboardState CurrentKeyboardState;

        private GamePadState LastGamePadState;
        private GamePadState CurrentGamePadState;

        private MouseState LastMouseState;
        private MouseState CurrentMouseState;

        private Dictionary<String, InputBinding> keybindings;
        public InputSettings Settings;

        #endregion

        #region Initialization

        public InputManager()
        {
            Settings = new InputSettings(0,0);
            keybindings = new Dictionary<string, InputBinding>();
        }
        public InputManager(InputManager input)
        {
            LastKeyboardState = input.LastKeyboardState;
            CurrentKeyboardState = input.CurrentKeyboardState;

            LastGamePadState = input.LastGamePadState;
            CurrentGamePadState = input.CurrentGamePadState;

            LastMouseState = input.LastMouseState;
            CurrentMouseState = input.CurrentMouseState;

            Settings = new InputSettings(input.Settings.TriggerThreshold, input.Settings.ThumbstickThreshold);

            keybindings = new Dictionary<string, InputBinding>(input.keybindings);

        }

        #endregion

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        /// <remarks>
        /// This should be called at the beginning of your update loop, so that game logic
        /// uses latest values.
        /// Calling update at the end of update loop will have those keys processed
        /// in the next frame.
        /// </remarks>
        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastGamePadState = CurrentGamePadState;
            LastMouseState = CurrentMouseState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            CurrentMouseState = Mouse.GetState();
        }

        #region Add/Remove/Has KeyBindings

        #region AddKeyBinding Methods

        private void AddKeyBinding(string bindingName, InputBinding inputBinding)
        {
            // Make sure there isn't already a biding with that name
            RemoveKeyBindings(bindingName);
            keybindings.Add(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params Modifier[] modifiers)
        {
            InputBinding inputBinding = InputBinding.CreateBinding(thumbstickDirection, thumbstick, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, MouseButton mouseButton, params Modifier[] modifiers)
        {
            InputBinding inputBinding = InputBinding.CreateBinding(mouseButton, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Thumbstick thumbstick, params Modifier[] modifiers)
        {
            InputBinding inputBinding = InputBinding.CreateBinding(thumbstick, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Trigger trigger, params Modifier[] modifiers)
        {
            InputBinding inputBinding = InputBinding.CreateBinding(trigger, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Buttons button, params Modifier[] modifiers)
        {
            InputBinding inputBinding = InputBinding.CreateBinding(button, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Keys key, params Modifier[] modifiers)
        {
            InputBinding inputBinding = InputBinding.CreateBinding(key, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }

        #endregion

        /// <summary>
        /// Removes the binding associated with the specified key
        /// </summary>
        /// <param name="key">The name of the keybinding to remove</param>
        public void RemoveKeyBindings(params string[] keys)
        {
            foreach(var key in keys)
                if(HasKeyBinding(key))
                    keybindings.Remove(key);
        }

        /// <summary>
        /// Returns true if the input has a binding associated with a key
        /// </summary>
        /// <param name="key">The name of the keybinding to check for</param>
        /// <returns></returns>
        public bool HasKeyBinding(string key)
        {
            return keybindings.ContainsKey(key);
        }

        #endregion

        #region Query Single KeyBinding State

        /// <summary>
        /// Returns if the keybinding associated with the string key is active in the specified frame.
        /// Active can mean pressed for buttons, or above threshold for thumbsticks/triggers
        /// </summary>
        /// <param name="key">The string that the keybinding was stored under</param>
        /// <param name="state">The frame to inspect for the press- the current frame or the previous frame</param>
        /// <returns></returns>
        public bool IsActive(string key, FrameState state = FrameState.Current)
        {
            if (HasKeyBinding(key))
                return keybindings[key].IsActive(state, this);
            return false;
        }

        /// <summary>
        /// Returns if the keybinding associated with the string key was pressed this frame,
        /// but not last.  To register on key up, use IsKeyBindingNewRelease
        /// </summary>
        /// <param name="key">The string that the keybinding was stored under</param>
        /// <returns></returns>
        public bool IsPressed(string key)
        {
            return IsActive(key, FrameState.Current) && !IsActive(key, FrameState.Previous);
        }

        /// <summary>
        /// Returns if the keybinding associated with the string key was pressed last frame,
        /// but not this frame (s.t. it was released in this frame).  
        /// To register on key down, use IsKeyBindingNewPress
        /// </summary>
        /// <param name="key">The string that the keybinding was stored under</param>
        /// <returns></returns>
        public bool IsReleased(string key)
        {
            return IsActive(key, FrameState.Previous) && !IsActive(key, FrameState.Current);
        }

        #endregion

        #region Query Multiple KeyBindings State

        public bool AnyActive(params string[] keys){
            foreach (var key in keys)
                if (IsActive(key))
                    return true;
            return false;
        }

        public bool AllActive(params string[] keys){
            foreach (var key in keys)
                if (!IsActive(key))
                    return false;
            return true;
        }

        public bool AnyPressed(params string[] keys){
            foreach (var key in keys)
                if (IsPressed(key))
                    return true;
            return false;
        }

        public bool AllPressed(params string[] keys){
            foreach (var key in keys)
                if (!IsPressed(key))
                    return false;
            return true;
        }

        public bool AnyReleased(params string[] keys)
        {
            foreach (var key in keys)
                if (IsReleased(key))
                    return true;
            return false;
        }

        public bool AllReleased(params string[] keys)
        {
            foreach (var key in keys)
                if (!IsReleased(key))
                    return false;
            return true;
        }

        #endregion

        #region Mouse Position

        /// <summary>
        /// Get the position of the mouse in the CURRENT frame.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePos()
        {
            return GetMousePos(FrameState.Current);
        }

        /// <summary>
        /// Get the position of the mouse in the specified frame.
        /// </summary>
        /// <param name="state">The frame to inspect for the position- the current frame or the previous frame</param>
        /// <returns></returns>
        public Vector2 GetMousePos(FrameState state)
        {
            MouseState mouseState = state == FrameState.Current ? CurrentMouseState : LastMouseState;
            return new Vector2(mouseState.X, mouseState.Y);
        }

        #endregion

        
        
    }
}

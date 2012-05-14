#region Using Statements

using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Engine.Input
{
    public class InputManager{
        #region Fields

        public KeyboardState LastKeyboardState { get; protected set; }
        public KeyboardState CurrentKeyboardState { get; protected set; }

        public GamePadState LastGamePadState { get; protected set; }
        public GamePadState CurrentGamePadState { get; protected set; }

        public MouseState LastMouseState { get; protected set; }
        public MouseState CurrentMouseState { get; protected set; }

        public Dictionary<String, InputBinding> Bindings { get; protected set; }
        public InputSettings Settings { get; private set; }

        #endregion

        #region Initialization

        public InputManager()
        {
            Settings = new InputSettings(0,0);
            Bindings = new Dictionary<string, InputBinding>();
        }
        public InputManager(InputManager input)
        {
            LastKeyboardState = input.LastKeyboardState;
            CurrentKeyboardState = input.CurrentKeyboardState;

            LastGamePadState = input.LastGamePadState;
            CurrentGamePadState = input.CurrentGamePadState;

            LastMouseState = input.LastMouseState;
            CurrentMouseState = input.CurrentMouseState;

            Settings = new InputSettings(input.Settings);
            Bindings = new Dictionary<string, InputBinding>(input.Bindings);

        }

        #endregion

        #region AddKeyBinding Methods

        public void AddKeyBinding(string bindingName, InputBinding inputBinding)
        {
            // Make sure there isn't already a biding with that name
            RemoveKeyBindings(bindingName);
            Bindings.Add(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new ThumbstickDirectionInputBinding(thumbstickDirection, thumbstick, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, MouseButton mouseButton, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new MouseInputBinding(mouseButton, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Thumbstick thumbstick, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new ThumbstickInputBinding(thumbstick, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Trigger trigger, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new TriggerInputBinding(trigger, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Buttons button, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new ButtonInputBinding(button, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Keys key, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new KeyInputBinding(key, modifiers);
            AddKeyBinding(bindingName, inputBinding);
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

        /// <summary>
        /// Removes the bindings associated with the specified keys
        /// </summary>
        /// <param name="keys">The names of the keybindings to remove</param>
        public void RemoveKeyBindings(params string[] keys)
        {
            foreach(var key in keys)
                if(HasKeyBinding(key))
                    Bindings.Remove(key);
        }

        /// <summary>
        /// Returns true if the input has a binding associated with a key
        /// </summary>
        /// <param name="key">The name of the keybinding to check for</param>
        /// <returns></returns>
        public bool HasKeyBinding(string key)
        {
            return Bindings.ContainsKey(key);
        }

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
                return Bindings[key].IsActive(this, state);
            return false;
        }

        /// <summary>
        /// Returns if the keybinding associated with the string key was pressed this frame,
        /// but not last frame (s.t. it was pressed for the first time in this frame).
        /// To register on key up, use IsReleased
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
        /// To register on key down, use IsPressed
        /// </summary>
        /// <param name="key">The string that the keybinding was stored under</param>
        /// <returns></returns>
        public bool IsReleased(string key)
        {
            return IsActive(key, FrameState.Previous) && !IsActive(key, FrameState.Current);
        }

        #endregion

        #region Query Multiple KeyBindings State

        /// <summary>
        /// Returns true if any of the keybindings associated with the given keys are active in
        /// the current frame.
        /// </summary>
        /// <param name="keys">The strings that the keybindings were stored under</param>
        /// <returns></returns>
        public bool AnyActive(params string[] keys){
            foreach (var key in keys)
                if (IsActive(key))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true if all of the keybindings associated with the given keys are active in
        /// the current frame.
        /// </summary>
        /// <param name="keys">The strings that the keybindings were stored under</param>
        /// <returns></returns>
        public bool AllActive(params string[] keys){
            foreach (var key in keys)
                if (!IsActive(key))
                    return false;
            return true;
        }

        /// <summary>
        /// Returns true if any of the keybindings associated with the given keys were first pressed
        /// in the current frame (and not in the last).
        /// </summary>
        /// <param name="keys">The strings that the keybindings were stored under</param>
        /// <returns></returns>
        public bool AnyPressed(params string[] keys){
            foreach (var key in keys)
                if (IsPressed(key))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true if all of the keybindings associated with the given keys were first pressed
        /// in the current frame (and not in the last).
        /// </summary>
        /// <param name="keys">The strings that the keybindings were stored under</param>
        /// <returns></returns>
        public bool AllPressed(params string[] keys){
            foreach (var key in keys)
                if (!IsPressed(key))
                    return false;
            return true;
        }

        /// <summary>
        /// Returns true if any of the keybindings associated with the given keys were first released
        /// in the current frame (and not in the last).
        /// </summary>
        /// <param name="keys">The strings that the keybindings were stored under</param>
        /// <returns></returns>
        public bool AnyReleased(params string[] keys)
        {
            foreach (var key in keys)
                if (IsReleased(key))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true if all of the keybindings associated with the given keys were first released
        /// in the current frame (and not in the last).
        /// </summary>
        /// <param name="keys">The strings that the keybindings were stored under</param>
        /// <returns></returns>
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
        /// Get the position of the mouse in the specified frame.
        /// (Default frame is the current frame)
        /// </summary>
        /// <param name="state">The frame to inspect for the position- the current frame or the previous frame</param>
        /// <returns></returns>
        public Vector2 GetMousePos(FrameState state = FrameState.Current)
        {
            MouseState mouseState = state == FrameState.Current ? CurrentMouseState : LastMouseState;
            return new Vector2(mouseState.X, mouseState.Y);
        }

        #endregion
    }
}

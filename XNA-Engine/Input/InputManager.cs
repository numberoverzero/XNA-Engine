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

        /// <summary>
        /// KeyboardState for the previous frame
        /// </summary>
        public KeyboardState PreviousKeyboardState { get; protected set; }
        /// <summary>
        /// KeyboardState for the current frame
        /// </summary>
        public KeyboardState CurrentKeyboardState { get; protected set; }

        /// <summary>
        /// GamePadState for the previous frame
        /// </summary>
        public GamePadState PreviousGamePadState { get; protected set; }
        /// <summary>
        /// GamePadState for the current frame
        /// </summary>
        public GamePadState CurrentGamePadState { get; protected set; }

        /// <summary>
        /// MouseState for the previous frame
        /// </summary>
        public MouseState PreviousMouseState { get; protected set; }
        /// <summary>
        /// MouseState for the current frame
        /// </summary>
        public MouseState CurrentMouseState { get; protected set; }

        /// <summary>
        /// The Bindings being tracked by the Manager
        /// </summary>
        public Dictionary<String, IBinding> Bindings { get; protected set; }

        /// <summary>
        /// The InputSettings for this InputManager (trigger thresholds, etc)
        /// </summary>
        public InputSettings Settings { get; private set; }

        #endregion

        #region Initialization

        public InputManager()
        {
            Settings = new InputSettings(0,0);
            Bindings = new Dictionary<string, IBinding>();
        }
        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="input"></param>
        public InputManager(InputManager input)
        {
            PreviousKeyboardState = input.PreviousKeyboardState;
            CurrentKeyboardState = input.CurrentKeyboardState;

            PreviousGamePadState = input.PreviousGamePadState;
            CurrentGamePadState = input.CurrentGamePadState;

            PreviousMouseState = input.PreviousMouseState;
            CurrentMouseState = input.CurrentMouseState;

            Settings = new InputSettings(input.Settings);
            Bindings = new Dictionary<string, IBinding>(input.Bindings);

        }

        #endregion

        #region Binding Update Add/Remove/Contains

        #region AddBinding Methods

        /// <summary>
        /// Add a binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="binding"></param>
        public void AddBinding(string bindingName, IBinding binding)
        {
            // Make sure there isn't already a biding with that name
            RemoveBindings(bindingName);
            Bindings.Add(bindingName, binding);
        }
        /// <summary>
        /// Add a ThumbstickDirection binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="thumbstickDirection"></param>
        /// <param name="thumbstick"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new ThumbstickDirectionInputBinding(thumbstickDirection, thumbstick, modifiers);
            AddBinding(bindingName, inputBinding);
        }
        /// <summary>
        /// Add a MouseButton binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="mouseButton"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, MouseButton mouseButton, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new MouseInputBinding(mouseButton, modifiers);
            AddBinding(bindingName, inputBinding);
        }
        /// <summary>
        /// Add a Thumbstick binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="thumbstick"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, Thumbstick thumbstick, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new ThumbstickInputBinding(thumbstick, modifiers);
            AddBinding(bindingName, inputBinding);
        }
        /// <summary>
        /// Add a Trigger binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="trigger"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, Trigger trigger, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new TriggerInputBinding(trigger, modifiers);
            AddBinding(bindingName, inputBinding);
        }
        /// <summary>
        /// Add a Button binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="button"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, Buttons button, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new ButtonInputBinding(button, modifiers);
            AddBinding(bindingName, inputBinding);
        }
        /// <summary>
        /// Add a key binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, Keys key, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new KeyInputBinding(key, modifiers);
            AddBinding(bindingName, inputBinding);
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
        public virtual void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            PreviousGamePadState = CurrentGamePadState;
            PreviousMouseState = CurrentMouseState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            CurrentMouseState = Mouse.GetState();
        }

        /// <summary>
        /// Removes the binding associated with the specified key
        /// </summary>
        /// <param name="key">The name of the keybinding to remove</param>
        public virtual void RemoveBinding(string key)
        {
            if (HasBinding(key))
                Bindings.Remove(key);
        }

        /// <summary>
        /// Removes the bindings associated with the specified keys
        /// </summary>
        /// <param name="keys">The names of the keybindings to remove</param>
        public virtual void RemoveBindings(params string[] keys)
        {
            foreach (var key in keys)
                RemoveBinding(key);
        }

        /// <summary>
        /// Returns true if the input has a binding associated with a key
        /// </summary>
        /// <param name="key">The name of the keybinding to check for</param>
        /// <returns></returns>
        public virtual bool HasBinding(string key)
        {
            return Bindings.ContainsKey(key);
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
        public virtual bool IsActive(string key, FrameState state = FrameState.Current)
        {
            if (HasBinding(key))
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
        public virtual bool IsPressed(string key)
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
        public virtual bool IsReleased(string key)
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
        public virtual bool AnyActive(params string[] keys){
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
        public virtual bool AllActive(params string[] keys){
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
        public virtual bool AnyPressed(params string[] keys){
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
        public virtual bool AllPressed(params string[] keys){
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
        public virtual bool AnyReleased(params string[] keys)
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
        public virtual bool AllReleased(params string[] keys)
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
        public virtual Vector2 GetMousePos(FrameState state = FrameState.Current)
        {
            MouseState mouseState = state == FrameState.Current ? CurrentMouseState : PreviousMouseState;
            return new Vector2(mouseState.X, mouseState.Y);
        }

        #endregion
    }
}

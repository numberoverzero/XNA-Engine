#region Using Statements

using System;
using System.Linq;
using System.Collections.Generic;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Engine.Input
{
    public class InputManager{
        #region Fields

        /// <summary>
        /// The Bindings being tracked by the Manager
        /// </summary>
        public Dictionary<String, IBinding> Bindings { get; protected set; }

        /// <summary>
        /// The InputSettings for this InputManager (trigger thresholds, etc)
        /// </summary>
        public InputSettings Settings { get; private set; }

        /// <summary>
        /// A unique set of modifiers of the bindings this manager tracks.
        /// Keeps track of how many bindings use this modifier; 
        ///     stops checking for modifiers once no bindings use that modifier
        /// </summary>
        public CountedSet<IBinding> Modifiers { get; protected set; }

        #region Previous/Current States

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

        #endregion

        #region State Monitoring

        protected bool monitorKeyboard;
        /// <summary>
        /// Enable/Disable grabbing keyboard state when updating the manager.
        /// Disable for performance when you know the user can't use a keyboard, or no bindings will need the state of the keyboard.
        /// </summary>
        public bool MonitorKeyboard
        {
            get { return monitorKeyboard; }
            set
            {
                monitorKeyboard = value;
                if (value)
                {
                    PreviousKeyboardState = new KeyboardState();
                    CurrentKeyboardState = new KeyboardState();
                }
            }
        }

        protected bool monitorGamePad;
        /// <summary>
        /// Enable/Disable grabbing gamepad state when updating the manager.
        /// Disable for performance when you know the user can't use a gamepad, or no bindings will need the state of the gamepad.
        /// </summary>
        public bool MonitorGamePad
        {
            get { return monitorGamePad; }
            set
            {
                monitorGamePad = value;
                if (value)
                {
                    PreviousGamePadState = new GamePadState();
                    CurrentGamePadState = new GamePadState();
                }
            }
        }

        protected bool monitorMouse;
        /// <summary>
        /// Enable/Disable grabbing mouse state when updating the manager.
        /// Disable for performance when you know the user can't use a mouse, or no bindings will need the state of the mouse.
        /// </summary>
        public bool MonitorMouse
        {
            get { return monitorMouse; }
            set
            {
                monitorMouse = value;
                if (value)
                {
                    PreviousMouseState = new MouseState();
                    CurrentMouseState = new MouseState();
                }
            }
        }
        #endregion

        #endregion

        #region Initialization

        public InputManager()
        {
            Settings = new InputSettings(0,0);
            Bindings = new Dictionary<string, IBinding>();
            Modifiers = new CountedSet<IBinding>();
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
            Modifiers = new CountedSet<IBinding>(input.Modifiers);

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
            Bindings[bindingName] = binding;
            foreach (var modifier in binding.Modifiers)
                Modifiers.Add(modifier);
        }
        /// <summary>
        /// Add a ThumbstickDirection binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="thumbstickDirection"></param>
        /// <param name="thumbstick"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params IBinding[] modifiers)
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
        public void AddBinding(string bindingName, MouseButton mouseButton, params IBinding[] modifiers)
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
        public void AddBinding(string bindingName, Thumbstick thumbstick, params IBinding[] modifiers)
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
        public void AddBinding(string bindingName, Trigger trigger, params IBinding[] modifiers)
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
        public void AddBinding(string bindingName, Buttons button, params IBinding[] modifiers)
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
        public void AddBinding(string bindingName, Keys key, params IBinding[] modifiers)
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
            if (MonitorKeyboard)
            {
                PreviousKeyboardState = CurrentKeyboardState;
                CurrentKeyboardState = Keyboard.GetState();
            }

            if (MonitorGamePad)
            {
                PreviousGamePadState = CurrentGamePadState;
                CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            }

            if (MonitorMouse)
            {
                PreviousMouseState = CurrentMouseState;
                CurrentMouseState = Mouse.GetState();
            }
        }

        /// <summary>
        /// Removes the binding associated with the specified key
        /// </summary>
        /// <param name="key">The name of the keybinding to remove</param>
        public virtual void RemoveBinding(string key)
        {
            if (HasBinding(key))
            {
                foreach (var modifier in Bindings[key].Modifiers)
                    Modifiers.Remove(modifier); 
                Bindings.Remove(key);
            }
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
                return Bindings[key].IsActive(this, state) && AreExactModifiersActive(key, state);
            return false;
        }

        /// <summary>
        /// Checks that only modifiers for that key are active and no other modifiers
        /// </summary>
        /// <param name="key">The string that the keybinding was stored under</param>
        /// <param name="state">The frame to inspect for the press- the current frame or the previous frame</param>
        /// <returns></returns>
        protected virtual bool AreExactModifiersActive(string key, FrameState state)
        {
            IBinding binding = Bindings[key];
            bool modifierActive;
            bool keyTracksModifier;
            foreach (var trackedModifier in Modifiers)
            {
                modifierActive = trackedModifier.IsActive(this, state);
                keyTracksModifier = binding.Modifiers.Contains(trackedModifier);
                if (modifierActive != keyTracksModifier)
                    return false;
            }

            // Only the modifiers that the key cares about were active, and no others.
            return true;
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
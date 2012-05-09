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
    

        public float ThumbstickThreshold = 0;
        public float TriggerThreshold = 0;

        #endregion

        #region Initialization

        public InputManager()
        {
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

            TriggerThreshold = input.TriggerThreshold;
            ThumbstickThreshold = input.ThumbstickThreshold;

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
            InputBinding inputBinding = new InputBinding();
            inputBinding.SetBinding(thumbstickDirection, thumbstick, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, MouseButton mouseButton, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new InputBinding();
            inputBinding.SetBinding(mouseButton, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Thumbstick thumbstick, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new InputBinding();
            inputBinding.SetBinding(thumbstick, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Trigger trigger, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new InputBinding();
            inputBinding.SetBinding(trigger, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Buttons button, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new InputBinding();
            inputBinding.SetBinding(button, modifiers);
            AddKeyBinding(bindingName, inputBinding);
        }
        public void AddKeyBinding(string bindingName, Keys key, params Modifier[] modifiers)
        {
            InputBinding inputBinding = new InputBinding();
            inputBinding.SetBinding(key, modifiers);
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
        public bool IsKeyBindingActive(string key, FrameState state = FrameState.Current)
        {
            return HasKeyBinding(key) && keybindings[key].IsActive(state, this);
        }

        /// <summary>
        /// Returns if the keybinding associated with the string key was pressed this frame,
        /// but not last.  To register on key up, use IsKeyBindingNewRelease
        /// </summary>
        /// <param name="key">The string that the keybinding was stored under</param>
        /// <returns></returns>
        public bool IsKeyBindingPress(string key)
        {
            return IsKeyBindingActive(key, FrameState.Current) && !IsKeyBindingActive(key, FrameState.Previous);
        }

        /// <summary>
        /// Returns if the keybinding associated with the string key was pressed last frame,
        /// but not this frame (s.t. it was released in this frame).  
        /// To register on key down, use IsKeyBindingNewPress
        /// </summary>
        /// <param name="key">The string that the keybinding was stored under</param>
        /// <returns></returns>
        public bool IsKeyBindingRelease(string key)
        {
            return IsKeyBindingActive(key, FrameState.Previous) && !IsKeyBindingActive(key, FrameState.Current);
        }

        #endregion

        #region Query Multiple KeyBindings State

        public bool AnyKeyBindsActive(params string[] keys){
            foreach (var key in keys)
                if (IsKeyBindingActive(key))
                    return true;
            return false;
        }

        public bool AllKeyBindsActive(params string[] keys){
            foreach (var key in keys)
                if (!IsKeyBindingActive(key))
                    return false;
            return true;
        }

        public bool AnyKeyBindsPress(params string[] keys){
            foreach (var key in keys)
                if (IsKeyBindingPress(key))
                    return true;
            return false;
        }

        public bool AllKeyBindsPress(params string[] keys){
            foreach (var key in keys)
                if (!IsKeyBindingPress(key))
                    return false;
            return true;
        }

        public bool AnyKeyBindsRelease(params string[] keys)
        {
            foreach (var key in keys)
                if (IsKeyBindingRelease(key))
                    return true;
            return false;
        }

        public bool AllKeyBindsRelease(params string[] keys)
        {
            foreach (var key in keys)
                if (!IsKeyBindingRelease(key))
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

        /// <summary>
        /// A single binding, wrapper for Thumbsticks, keys, Buttons, etc
        /// </summary>
        private class InputBinding
        {
            #region Fields

            /// <summary>
            /// The type of binding (Key, Trigger, Thumbstick, etc) for this input association
            /// </summary>
            public BindingType BindingType { get; private set; }

            public ThumbstickDirection ThumbstickDirection { get; private set; }
            public MouseButton MouseButton { get; private set; }
            public Thumbstick Thumbstick { get; private set; }
            public Trigger Trigger { get; private set; }
            public Buttons Button { get; private set; }
            public Keys Key { get; private set; }

            public Modifier[] Modifiers { get; private set; }

            #endregion

            #region Initialiation

            /// <summary>
            /// Initialize an InputBinding with no BindingType
            /// </summary>
            public InputBinding() : this(BindingType.None) { }
            /// <summary>
            /// Initialize an InputBinding with the given BindingType and an optional list of required modifiers
            /// </summary>
            /// <param name="type">Type of binding (Key, Trigger, Thumbstick, etc)</param>
            /// <param name="modifiers">Optional modifiers- Ctrl, Alt, Shift</param>
            /// 
            public InputBinding(BindingType type, params Modifier[] modifiers)
            {
                this.BindingType = type;
                ThumbstickDirection = ThumbstickDirection.None;
                MouseButton = MouseButton.None;
                Thumbstick = Thumbstick.None;
                Trigger = Trigger.None;
                Key = Keys.None;
                Button = Buttons.BigButton;

                SetModifiers(modifiers);
            }

            #endregion

            #region SetBinding Methods

            public void SetBinding(ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params Modifier[] modifiers)
            {
                ClearBindings();
                this.ThumbstickDirection = thumbstickDirection;
                this.Thumbstick = thumbstick;
                this.BindingType = BindingType.ThumbstickDirection;
                SetModifiers(modifiers);
            }
            public void SetBinding(MouseButton mouseButton, params Modifier[] modifiers)
            {
                ClearBindings();
                this.MouseButton = mouseButton;
                this.BindingType = BindingType.MouseButton;
                SetModifiers(modifiers);
            }
            public void SetBinding(Thumbstick thumbstick, params Modifier[] modifiers)
            {
                ClearBindings();
                this.Thumbstick = thumbstick;
                this.BindingType = BindingType.Thumbstick;
                SetModifiers(modifiers);
            }
            public void SetBinding(Trigger trigger, params Modifier[] modifiers)
            {
                ClearBindings();
                this.Trigger = trigger;
                this.BindingType = BindingType.Trigger;
                SetModifiers(modifiers);
            }
            public void SetBinding(Buttons button, params Modifier[] modifiers)
            {
                ClearBindings();
                this.Button = button;
                this.BindingType = BindingType.Button;
                SetModifiers(modifiers);
            }
            public void SetBinding(Keys key, params Modifier[] modifiers)
            {
                ClearBindings();
                this.Key = key;
                this.BindingType = BindingType.Key;
                SetModifiers(modifiers);
            }

            #endregion

            public bool IsActive(FrameState state, InputManager inputManager)
            {
                KeyboardState keyState = state == FrameState.Current ? inputManager.CurrentKeyboardState : inputManager.LastKeyboardState;
                GamePadState gamepadState = state == FrameState.Current ? inputManager.CurrentGamePadState : inputManager.LastGamePadState;
                MouseState mouseState = state == FrameState.Current ? inputManager.CurrentMouseState : inputManager.LastMouseState;
                bool isActive = false;
                switch (BindingType)
                {
                    case BindingType.ThumbstickDirection:
                        Vector2 gamepadThumbstick = Thumbstick == Thumbstick.Left ? gamepadState.ThumbSticks.Left : gamepadState.ThumbSticks.Right;
                        switch (ThumbstickDirection){
                            case ThumbstickDirection.Up:
                                isActive = (gamepadThumbstick.Y >= inputManager.ThumbstickThreshold);
                                break;
                            case ThumbstickDirection.Down:
                                isActive = (gamepadThumbstick.Y <= -inputManager.ThumbstickThreshold);
                                break;
                            case ThumbstickDirection.Left:
                                isActive = (gamepadThumbstick.X <= -inputManager.ThumbstickThreshold);
                                break;
                            case ThumbstickDirection.Right:
                                isActive = (gamepadThumbstick.X >= inputManager.ThumbstickThreshold);
                                break;
                            default:
                                break;
                        }
                        break;
                    case BindingType.MouseButton:
                        ButtonState buttonState;
                        switch(MouseButton){
                            case MouseButton.Left:
                                buttonState = mouseState.LeftButton;
                                break;
                            case MouseButton.Right:
                                buttonState = mouseState.RightButton;
                                break;
                            case MouseButton.Middle:
                                buttonState = mouseState.MiddleButton;
                                break;
                            default:
                                buttonState = ButtonState.Released;
                                break;
                        }
                        isActive = buttonState == ButtonState.Pressed;
                        break;
                    case BindingType.Thumbstick:
                        Vector2 gamepadThumbstickMag = Thumbstick == Thumbstick.Left ? gamepadState.ThumbSticks.Left : gamepadState.ThumbSticks.Right;
                        isActive = gamepadThumbstickMag.Length() >= inputManager.ThumbstickThreshold;
                        break;
                    case BindingType.Button:
                        isActive = gamepadState.IsButtonDown(Button);
                        break;
                    case BindingType.Key:
                        isActive = keyState.IsKeyDown(Key);
                        break;
                    case BindingType.Trigger:
                        float triggerMag = Trigger == Trigger.Left ? gamepadState.Triggers.Left : gamepadState.Triggers.Right;
                        isActive = triggerMag >= inputManager.TriggerThreshold;
                        break;
                    case BindingType.None:
                    default:
                        break;
                }
                return isActive && ModifiersMatch(keyState);
            }

            #region Private Helpers

            private void ClearBindings()
            {
                BindingType = BindingType.None;
            }
            private void SetModifiers(params Modifier[] modifiers)
            {
                Modifiers = new Modifier[modifiers.Length];
                Array.Copy(modifiers, Modifiers, modifiers.Length);
            }

            /// <summary>
            /// Checks if the modifiers that this binding requires 
            /// match the active state of the modifiers in the given KeyboardState
            /// </summary>
            /// <param name="keyState">The KeyboardState to check modifiers against</param>
            /// <returns>True if only all required modifiers are active</returns>
            private bool ModifiersMatch(KeyboardState keyState)
            {
                return (Modifier.Alt.IsActive(keyState) == Modifiers.Contains(Modifier.Alt) &&
                        Modifier.Ctrl.IsActive(keyState) == Modifiers.Contains(Modifier.Ctrl) &&
                        Modifier.Shift.IsActive(keyState) == Modifiers.Contains(Modifier.Shift));
            }

            #endregion
        }
        
    }
}

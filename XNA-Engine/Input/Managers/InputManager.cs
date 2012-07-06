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
    // TODO Add multiplayer support for GamePadState.
    // TODO Update every single comment
    public class InputManager : IInputManager{
        #region Fields

        /// <summary>
        /// The Bindings being tracked by the Manager
        /// </summary>
        public DefaultMultiKeyDict<String, PlayerIndex, List<IBinding>> Bindings { get; protected set; }

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
            Bindings = new DefaultMultiKeyDict<String, PlayerIndex, List<IBinding>>();
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
            Bindings = new DefaultMultiKeyDict<String, PlayerIndex, List<IBinding>>(input.Bindings);
            Modifiers = new CountedSet<IBinding>(input.Modifiers);

        }

        #endregion

        /// <summary>
        /// Get the position of the mouse in the specified frame.
        /// (Default frame is the current frame)
        /// </summary>
        /// <param name="state">The frame to inspect for the position- the current frame or the previous frame</param>
        /// <returns></returns>
        public virtual Vector2 GetMousePosition(FrameState state)
        {
            MouseState mouseState = state == FrameState.Current ? CurrentMouseState : PreviousMouseState;
            return new Vector2(mouseState.X, mouseState.Y);
        }

        #region Binding Mutation

        /// <summary>
        /// Add a binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="binding"></param>
        public void AddBinding(string bindingName, IBinding binding, PlayerIndex player)
        {
            var bindings = Bindings[bindingName, player];
            if (bindings.Contains(binding))
                return;

            bindings.Add(binding);
            foreach (var modifier in binding.Modifiers)
                Modifiers.Add(modifier);
        }

        /// <summary>
        /// Removes the binding associated with the specified key
        /// </summary>
        /// <param name="bindingName">The name of the keybinding to remove</param>
        public virtual void RemoveBinding(string bindingName, int index, PlayerIndex player)
        {
            if (!ContainsBinding(bindingName, player))
                return;
            
            var bindings = Bindings[bindingName, player];

            if (index < 0 || index >= bindings.Count)
                return;

            var binding = bindings[index];
            
            foreach (var modifier in binding.Modifiers)
                    Modifiers.Remove(modifier); 
            bindings.RemoveAt(index);
        }

        public virtual void RemoveBinding(string bindingName, IBinding binding, PlayerIndex player)
        {
            if (!ContainsBinding(bindingName, player))
                return;

            var bindings = Bindings[bindingName, player];

            if (bindings.Contains(binding))
                bindings.Remove(binding);
            
            foreach (var modifier in binding.Modifiers)
                Modifiers.Remove(modifier);
        }

        /// <summary>
        /// Returns true if the input has a binding associated with a key
        /// </summary>
        /// <param name="bindingName">The name of the keybinding to check for</param>
        /// <returns></returns>
        public virtual bool ContainsBinding(string bindingName, PlayerIndex player)
        {
            return Bindings[bindingName, player].Count > 0;
        }

        public virtual void ClearBinding(string bindingName, PlayerIndex player)
        {
            // Make sure we clean up any modifiers
            var old_bindings = new List<IBinding>(Bindings[bindingName, player]);
            foreach (var binding in old_bindings)
                RemoveBinding(bindingName, binding, player);
            Bindings[bindingName, player] = new List<IBinding>();
        }

        public virtual void ClearAllBindings()
        {
            Bindings.Clear();
            Modifiers.Clear();
        }

        #endregion

        #region Query Single KeyBinding State

        /// <summary>
        /// Returns if the keybinding associated with the string key is active in the specified frame.
        /// Active can mean pressed for buttons, or above threshold for thumbsticks/triggers
        /// </summary>
        /// <param name="bindingName">The string that the keybinding was stored under</param>
        /// <param name="state">The frame to inspect for the press- the current frame or the previous frame</param>
        /// <returns></returns>
        public virtual bool IsActive(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player))
                return false;
            var bindings = Bindings[bindingName, player];
            foreach (var binding in bindings)
                if (binding.IsActive(this, state) && IsModifiersActive(binding, state))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks that only modifiers for that key are active and no other modifiers
        /// </summary>
        /// <param name="key">The string that the keybinding was stored under</param>
        /// <param name="state">The frame to inspect for the press- the current frame or the previous frame</param>
        /// <returns></returns>
        protected virtual bool IsModifiersActive(IBinding binding, FrameState state)
        {
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
        /// <param name="bindingName">The string that the keybinding was stored under</param>
        /// <returns></returns>
        public virtual bool IsPressed(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Current) && !IsActive(bindingName, player, FrameState.Previous);
        }

        /// <summary>
        /// Returns if the keybinding associated with the string key was pressed last frame,
        /// but not this frame (s.t. it was released in this frame).  
        /// To register on key down, use IsPressed
        /// </summary>
        /// <param name="bindingName">The string that the keybinding was stored under</param>
        /// <returns></returns>
        public virtual bool IsReleased(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Previous) && !IsActive(bindingName, player, FrameState.Current);
        }

        public List<IBinding> GetCurrentBindings(string bindingName, PlayerIndex player)
        {
            return Bindings[bindingName, player];
        }

        #endregion

        #region Manager Query

        /// <summary>
        /// Returns a list of bindings that currently use the specified IBinding
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<string> BindingsUsing(IBinding binding, PlayerIndex player = PlayerIndex.One)
        {
            List<string> binds = new List<string>();

            List<IBinding> bindingGroup;
            foreach (string bindingGroupKey in Bindings.Keys)
            {
                bindingGroup = Bindings[bindingGroupKey, player];
                if (bindingGroup.Contains(binding))
                    binds.Add(bindingGroupKey);
            }

            return binds;
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


        public IEnumerable<IBinding> GetModifiers
        {
            get { return Modifiers; }
        }
    }
}
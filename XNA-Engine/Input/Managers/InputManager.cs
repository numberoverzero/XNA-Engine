﻿#region Using Statements

using System;
using System.Linq;
using System.Collections.Generic;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Engine.Input
{
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
        /// For enumerating through player indices
        /// </summary>
        public static readonly PlayerIndex[] Players = new PlayerIndex[4] { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };

        /// <summary>
        /// GamePadState for the previous frame
        /// </summary>
        public Dictionary<PlayerIndex, GamePadState> PreviousGamePadStates { get; protected set; }
        /// <summary>
        /// GamePadState for the current frame
        /// </summary>
        public Dictionary<PlayerIndex, GamePadState> CurrentGamePadStates { get; protected set; }

        /// <summary>
        /// MouseState for the previous frame
        /// </summary>
        public MouseState PreviousMouseState { get; protected set; }
        /// <summary>
        /// MouseState for the current frame
        /// </summary>
        public MouseState CurrentMouseState { get; protected set; }

        #endregion

        #region Device Polling

        protected bool isPollingKeyboard;
        /// <summary>
        /// Enable/Disable grabbing keyboard state when updating the manager.
        /// Disable for performance when you know the user can't use a keyboard, or no bindings will need the state of the keyboard.
        /// </summary>
        public bool IsPollingKeyboard
        {
            get { return isPollingKeyboard; }
            set
            {
                isPollingKeyboard = value;
                if (value)
                {
                    PreviousKeyboardState = new KeyboardState();
                    CurrentKeyboardState = new KeyboardState();
                }
            }
        }

        protected bool isPollingGamePads;
        /// <summary>
        /// Enable/Disable grabbing gamepad state when updating the manager.
        /// Disable for performance when you know the user can't use a gamepad, or no bindings will need the state of the gamepad.
        /// </summary>
        public bool IsPollingGamePads
        {
            get { return isPollingGamePads; }
            set
            {
                isPollingGamePads = value;
                if (value)
                {
                    PreviousGamePadStates = new Dictionary<PlayerIndex,GamePadState>();
                    CurrentGamePadStates = new Dictionary<PlayerIndex, GamePadState>();
                    foreach (var player in Players)
                    {
                        PreviousGamePadStates[player] = new GamePadState();
                        CurrentGamePadStates[player] = new GamePadState();
                    }
                }
            }
        }

        protected bool isPollingMouse;
        /// <summary>
        /// Enable/Disable grabbing mouse state when updating the manager.
        /// Disable for performance when you know the user can't use a mouse, or no bindings will need the state of the mouse.
        /// </summary>
        public bool IsPollingMouse
        {
            get { return isPollingMouse; }
            set
            {
                isPollingMouse = value;
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

        /// <summary>
        /// Create an empty InputManager.  By default, polls all devices.
        /// </summary>
        public InputManager()
        {
            Settings = new InputSettings(0,0);
            Bindings = new DefaultMultiKeyDict<String, PlayerIndex, List<IBinding>>();
            Modifiers = new CountedSet<IBinding>();
            IsPollingKeyboard = IsPollingGamePads = IsPollingMouse = true;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="input"></param>
        public InputManager(InputManager input)
        {
            PreviousKeyboardState = input.PreviousKeyboardState;
            CurrentKeyboardState = input.CurrentKeyboardState;

            PreviousGamePadStates = new Dictionary<PlayerIndex, GamePadState>(input.PreviousGamePadStates);
            CurrentGamePadStates = new Dictionary<PlayerIndex, GamePadState>(input.CurrentGamePadStates);
            

            PreviousMouseState = input.PreviousMouseState;
            CurrentMouseState = input.CurrentMouseState;

            Settings = new InputSettings(input.Settings);
            Bindings = new DefaultMultiKeyDict<String, PlayerIndex, List<IBinding>>(input.Bindings);
            Modifiers = new CountedSet<IBinding>(input.Modifiers);

            IsPollingGamePads = input.IsPollingGamePads;
            IsPollingKeyboard = input.IsPollingKeyboard;
            IsPollingMouse = input.IsPollingMouse;

        }

        #endregion

        /// <summary>
        /// Get the position of the mouse in the specified frame.
        /// </summary>
        /// <param name="state">The frame to inspect for the position- the current frame or the previous frame</param>
        /// <returns>The position of the mouse in screen space</returns>
        public virtual Vector2 GetMousePosition(FrameState state)
        {
            MouseState mouseState = state == FrameState.Current ? CurrentMouseState : PreviousMouseState;
            return new Vector2(mouseState.X, mouseState.Y);
        }

        #region Binding Mutation

        /// <summary>
        /// Add a binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName">The string used to query the binding state</param>
        /// <param name="binding">The binding to associate with the bindingName</param>
        /// <param name="player">The player to add the binding for</param>
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
        /// Remove a binding from the InputManager.  This removes a binding by its index against a bindingName.
        /// For the binding {"jump": [Binding{Keys.Space}, Binding{Buttons.A}, Binding{Keys.W}]} the command
        /// RemoveBinding("jump", 1, PlayerIndex.One) removes the Buttons.A binding for "jump".
        /// This is useful when you know the index of the binding in its list of bindings
        /// </summary>
        /// <param name="bindingName">The string used to query the binding state</param>
        /// <param name="index">The index of the binding in the list of bindings associated with the bindingName</param>
        /// <param name="player">The player the binding is being removed for</param>
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

        /// <summary>
        /// Remove a binding from the InputManager.  Removes the exact binding from the relation.
        /// This can be used when you don't know the binding's index in its list of bindings.
        /// </summary>
        /// <param name="bindingName">The string used to query the binding state</param>
        /// <param name="binding">The binding to remove from the association with the bindingName</param>
        /// <param name="player">The player the binding is being removed for</param>
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
        /// Check if the manager has a binding associated with a bindingName for a player
        /// </summary>
        /// <param name="bindingName">The name of the binding to check for</param>
        /// <param name="player">The player to check the binding for</param>
        /// <returns>True if there are bindings associated with the bindingName for the given player</returns>
        public virtual bool ContainsBinding(string bindingName, PlayerIndex player)
        {
            return Bindings[bindingName, player].Count > 0;
        }

        /// <summary>
        /// Clears all bindings associated with the given bindingName for a particular player
        /// </summary>
        /// <param name="bindingName">The name of the binding to clear</param>
        /// <param name="player">The player to clear the binding for</param>
        public virtual void ClearBinding(string bindingName, PlayerIndex player)
        {
            // Make sure we clean up any modifiers
            var old_bindings = new List<IBinding>(Bindings[bindingName, player]);
            foreach (var binding in old_bindings)
                RemoveBinding(bindingName, binding, player);
            Bindings[bindingName, player] = new List<IBinding>();
        }

        /// <summary>
        /// Clears all bindings for all players
        /// </summary>
        public virtual void ClearAllBindings()
        {
            Bindings.Clear();
            Modifiers.Clear();
        }

        #endregion

        #region Query Single KeyBinding State

        /// <summary>
        /// Checks if any of the bindings associated with the bindingName for a given player in a given FrameState is active.
        /// </summary>
        /// <param name="bindingName">The name of the binding to query for active state</param>
        /// <param name="player">The player to check the binding's activity for</param>
        /// <param name="state">The FrameState in which to check for activity</param>
        /// <returns>True if any of the bindings associated with the bindingName for a given player in a given FrameState is active.</returns>
        public virtual bool IsActive(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player))
                return false;
            var bindings = Bindings[bindingName, player];
            foreach (var binding in bindings)
                if (binding.IsActive(this, player, state) && IsModifiersActive(binding, player, state))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks that only modifiers for that key are active and no other modifiers
        /// </summary>
        /// <param name="binding">The binding to check modifiers of</param>
        /// <param name="player">The player to check the binding's modifiers of</param>
        /// <param name="state">The FrameState in which to check modifiers</param>
        /// <returns>True if no tracked modifiers except those required for the binding are active</returns>
        protected virtual bool IsModifiersActive(IBinding binding, PlayerIndex player, FrameState state)
        {
            bool modifierActive;
            bool keyTracksModifier;
            foreach (var trackedModifier in Modifiers)
            {
                modifierActive = trackedModifier.IsActive(this, player, state);
                keyTracksModifier = binding.Modifiers.Contains(trackedModifier);
                if (modifierActive != keyTracksModifier)
                    return false;
            }

            // Only the modifiers that the key cares about were active, and no others.
            return true;
        }

        /// <summary>
        /// Checks if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName">The name of the binding to query for active state</param>
        /// <param name="player">The player to check the binding's activity for</param>
        /// <returns>True if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous).</returns>
        public virtual bool IsPressed(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Current) && !IsActive(bindingName, player, FrameState.Previous);
        }

        /// <summary>
        /// Checks if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName">The name of the binding to query for active state</param>
        /// <param name="player">The player to check the binding's activity for</param>
        /// <returns>True if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous).</returns>
        public virtual bool IsReleased(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Previous) && !IsActive(bindingName, player, FrameState.Current);
        }

        /// <summary>
        /// Gets the list of bindings associated with a particular bindingName for a given player
        /// </summary>
        /// <param name="bindingName">The bindingName associated with the list of Bindings</param>
        /// <param name="player">The player to get the list of bindings for</param>
        /// <returns>Returns a copy of the Bindings associated with the bindingName for a givem player</returns>
        public List<IBinding> GetCurrentBindings(string bindingName, PlayerIndex player)
        {
            return new List<IBinding>(Bindings[bindingName, player]);
        }

        #endregion

        #region Manager Query

        /// <summary>
        /// Used to get a list of strings that map to the given binding for a given player.
        /// This is useful when you want to unbind a key from current bindings and remap to a new binding:
        /// You can present a dialog such as "{key} is currently mapped to {List of Bindings using {key}}.  Are you sure you want to remap {key} to {New binding}?"
        /// </summary>
        /// <param name="binding">The binding to search for in the InputManager</param>
        /// <param name="player">The player to search for bindings on</param>
        /// <returns>A list of the bindingNames that, for a given player, track the given binding as a possible input</returns>
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
        /// Reads the latest state of the keyboard, mouse, and gamepad. (If polling is enabled for these devices)
        /// </summary>
        /// <remarks>
        /// This should be called at the beginning of your update loop, so that game logic
        /// uses latest values.
        /// Calling update at the end of update loop will have those keys processed
        /// in the next frame.
        /// </remarks>
        public virtual void Update()
        {
            if (IsPollingKeyboard)
            {
                PreviousKeyboardState = CurrentKeyboardState;
                CurrentKeyboardState = Keyboard.GetState();
            }

            if (IsPollingGamePads)
            {
                foreach (var player in Players)
                {
                    PreviousGamePadStates[player] = CurrentGamePadStates[player];
                    CurrentGamePadStates[player] = GamePad.GetState(player);
                }
            }

            if (IsPollingMouse)
            {
                PreviousMouseState = CurrentMouseState;
                CurrentMouseState = Mouse.GetState();
            }
        }

        /// <summary>
        /// All the modifiers currently being tracked.
        /// </summary>
        public IEnumerable<IBinding> GetModifiers
        {
            get { return Modifiers; }
        }
    }
}
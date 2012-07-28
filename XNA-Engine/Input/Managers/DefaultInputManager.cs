#region Using Statements

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine.Utility;

#endregion

namespace Engine.Input
{
    /// <summary>
    /// An InputManager that supports programmatic binding presses
    /// </summary>
    public class DefaultInputManager : InputManager, EventInput.IKeyboardSubscriber{
        #region Fields

        /// <summary>
        /// The Bindings being tracked by the Manager
        /// </summary>
        public MultiKeyDict<String, PlayerIndex, List<IBinding>> Bindings { get; protected set; }

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

        /// <summary>
        /// The per-frame text buffer.
        /// </summary>
        protected DoubleBuffer<char> BufferedText;

        #region Previous/Current States

        /// <summary>
        /// Keys pressed in the previous frame
        /// </summary>
        public ISet<Keys> PreviousKeys { get; protected set; }

        /// <summary>
        /// Keys pressed in the current frame
        /// </summary>
        public ISet<Keys> CurrentKeys { get; protected set; }

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

        /// <summary>
        /// The default InputManager does not use keyboard polling
        /// </summary>
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
                isPollingKeyboard = false;
                throw new NotSupportedException("DefaultInputManagers do not support keyboard polling.");
            }
        }

        /// <summary>
        /// True when the InputManager is polling gamepads
        /// </summary>
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

        /// <summary>
        /// True when the InputManager is polling the mouse
        /// </summary>
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

        #region Programmatic Key Injection

        /// <summary>
        /// bindings pressed in the previous frame
        /// </summary>
        public ISet<string> PreviousInjectedPresses { get; protected set; }

        /// <summary>
        /// bindings pressed in the current frame
        /// </summary>
        public ISet<string> CurrentInjectedPresses { get; protected set; }

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Create an empty InputManager.  By default, polls all devices.
        /// </summary>
        public DefaultInputManager()
        {
            Settings = new InputSettings(0,0);
            Bindings = new MultiKeyDict<String, PlayerIndex, List<IBinding>>();
            Modifiers = new CountedSet<IBinding>();
            PreviousInjectedPresses = new HashSet<string>();
            CurrentInjectedPresses = new HashSet<string>();
            BufferedText = new DoubleBuffer<char>();
            PreviousKeys = new HashSet<Keys>();
            CurrentKeys = new HashSet<Keys>();
            IsPollingGamePads = IsPollingMouse = true;
            isPollingKeyboard = false;
            EventInput.KeyboardDispatcher.RegisterListener(this);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="input"></param>
        public DefaultInputManager(DefaultInputManager input)
        {
            PreviousKeys = new HashSet<Keys>(input.PreviousKeys);
            CurrentKeys = new HashSet<Keys>(input.CurrentKeys);

            PreviousGamePadStates = new Dictionary<PlayerIndex, GamePadState>(input.PreviousGamePadStates);
            CurrentGamePadStates = new Dictionary<PlayerIndex, GamePadState>(input.CurrentGamePadStates);
            

            PreviousMouseState = input.PreviousMouseState;
            CurrentMouseState = input.CurrentMouseState;

            Settings = new InputSettings(input.Settings);
            Bindings = new MultiKeyDict<String, PlayerIndex, List<IBinding>>(input.Bindings);
            Modifiers = new CountedSet<IBinding>(input.Modifiers);

            IsPollingGamePads = input.IsPollingGamePads;
            isPollingKeyboard = input.isPollingKeyboard;
            IsPollingMouse = input.IsPollingMouse;

            EventInput.KeyboardDispatcher.RegisterListener(this);
        }

        #endregion

        #region Static Initialization

        static bool initialized = false;
        /// <summary>
        /// Initialize InputManager dependencies (For event-driven input)
        /// </summary>
        /// <param name="window"></param>
        public static void Initialize(GameWindow window)
        {
            if (!initialized)
            {
                EventInput.KeyboardDispatcher.Initialize(window);
                initialized = true;
            }
        }

        #endregion

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

            PreviousInjectedPresses.Remove(bindingName);
            CurrentInjectedPresses.Remove(bindingName);
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
            int index = bindings.IndexOf(binding);
            RemoveBinding(bindingName, index, player);
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
            
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            bool isInjected = injectedPresses.Contains(bindingName);
            if (isInjected)
                return true;
            
            var bindings = Bindings[bindingName, player];
            foreach (var binding in bindings)
                if (binding.IsActive(this, player, state) && IsModifiersActive(binding, player, state))
                    return true;
            
            return false;
        }

        /// <summary>
        /// Checks if all (and only all) of the modifiers associated with a binding for a given player were active in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName">The name of the binding whose modifiers are being checked</param>
        /// <param name="player">The player to check the binding's modifiers for</param>
        /// <param name="state">The FrameState in which to check the modifiers</param>
        /// <returns>True if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous).</returns>
        protected virtual bool IsModifiersActive(IBinding bindingName, PlayerIndex player, FrameState state)
        {
            bool modifierActive;
            bool keyTracksModifier;
            foreach (var trackedModifier in Modifiers)
            {
                modifierActive = trackedModifier.IsActive(this, player, state);
                keyTracksModifier = bindingName.Modifiers.Contains(trackedModifier);
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

        #region Programmatic Binding Injection

        /// <summary>
        /// "Press" a key in a given frame.
        /// Cannot press a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="bindingName">The binding to press</param>
        /// <param name="player">The player to press the binding for</param>
        /// <param name="state">The frame to press it in</param>
        public void Press(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player)) return;
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            injectedPresses.Add(bindingName);
        }

        /// <summary>
        /// "Release" a key in a given frame.
        /// Cannot release a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="bindingName">The binding to release</param>
        /// <param name="player">The player to release the binding for</param>
        /// <param name="state">The frame to release it in</param>
        public void Release(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player)) return;
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            injectedPresses.Remove(bindingName);
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

        #region IKeyboardSubscriber Interface

        /// <summary>
        /// Handle a single character of input
        /// </summary>
        /// <param name="inputChar"></param>
        public void ReceiveTextInput(char inputChar)
        {
            BufferedText.Push(inputChar);
        }

        /// <summary>
        /// Handle a string of input
        /// </summary>
        /// <param name="text"></param>
        public void ReceiveTextInput(string text)
        {
            foreach (char c in text)
                BufferedText.Push(c);
        }

        /// <summary>
        /// Handle a special command
        /// </summary>
        /// <param name="command"></param>
        public void ReceiveCommandInput(char command)
        {
            BufferedText.Push(command);
        }

        /// <summary>
        /// Handle a Key input
        /// </summary>
        /// <param name="key"></param>
        public void ReceiveSpecialInput(Keys key)
        {
            CurrentKeys.Add(key);
        }

        /// <summary>
        /// Does this Subscriber have the (possibly exclusive) focus
        /// </summary>
        public bool Selected { get; set; }

        #endregion

        /// <summary>
        /// The buffered text input since the last frame.  This is cleared per frame,
        /// regardless of whether it has been read.
        /// </summary>
        public List<char> GetBufferedText()
        {
            return BufferedText.Front;
        }

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

        /// <summary>
        /// All the modifiers currently being tracked.
        /// </summary>
        public IEnumerable<IBinding> GetModifiers
        {
            get { return Modifiers; }
        }

        /// <summary>
        /// Reads the latest state of the keyboard, mouse, and gamepad. (If polling is enabled for these devices)
        /// </summary>
        /// <remarks>
        /// This should be called at the end of your update loop, so that game logic
        /// uses latest values.
        /// Calling update at the beginning of the update loop will clear current buffers (if any) which
        /// means you will not be able to read the most recent input.
        /// </remarks>
        public virtual void Update()
        {
            BufferedText.Flip();

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

            PreviousKeys = CurrentKeys;
            CurrentKeys = new HashSet<Keys>();

            PreviousInjectedPresses = CurrentInjectedPresses;
            CurrentInjectedPresses = new HashSet<string>();
        }
    }
}
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

        public void AddBinding(string bindingName, IBinding binding, PlayerIndex player)
        {
            var bindings = Bindings[bindingName, player];
            if (bindings.Contains(binding))
                return;

            bindings.Add(binding);
            foreach (var modifier in binding.Modifiers)
                Modifiers.Add(modifier);
        }

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

        public virtual void RemoveBinding(string bindingName, IBinding binding, PlayerIndex player)
        {
            if (!ContainsBinding(bindingName, player))
                return;

            var bindings = Bindings[bindingName, player];
            int index = bindings.IndexOf(binding);
            RemoveBinding(bindingName, index, player);
        }

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

        public virtual bool IsPressed(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Current) && !IsActive(bindingName, player, FrameState.Previous);
        }

        public virtual bool IsReleased(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Previous) && !IsActive(bindingName, player, FrameState.Current);
        }

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
        /// <param name="state">The frame to release it in</param>
        public void Release(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player)) return;
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            injectedPresses.Remove(bindingName);
        }

        #endregion

        #region Manager Query

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

        public void RecieveTextInput(char inputChar)
        {
            BufferedText.Push(inputChar);
        }

        public void RecieveTextInput(string text)
        {
            foreach (char c in text)
                BufferedText.Push(c);
        }

        public void RecieveCommandInput(char command)
        {
            BufferedText.Push(command);
        }

        public void RecieveSpecialInput(Keys key)
        {
            CurrentKeys.Add(key);
        }

        public bool Selected { get; set; }

        #endregion

        public List<char> GetBufferedText()
        {
            return BufferedText.Front;
        }

        public virtual Vector2 GetMousePosition(FrameState state)
        {
            MouseState mouseState = state == FrameState.Current ? CurrentMouseState : PreviousMouseState;
            return new Vector2(mouseState.X, mouseState.Y);
        }

        public IEnumerable<IBinding> GetModifiers
        {
            get { return Modifiers; }
        }

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
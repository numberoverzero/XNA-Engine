﻿using System.Collections.Generic;
using System.Linq;
using Engine.DataStructures;
using Engine.Input.Devices;
using Engine.Input.EventInput;
using Engine.Mathematics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input.Managers
{
    /// <summary>
    ///     Manages all types of InputBindings - mouse, keyboard, gamepad.
    /// </summary>
    public class FullInputManager : InputManager, IKeyboardSubscriber
    {
        private static bool initialized;
        private readonly DoubleBuffer<char> BufferedText;
        private readonly TypedInputManager _gamePadManager;
        private readonly InjectableInputManager _injectableManager;
        private readonly TypedInputManager _keyboardManager, _mouseManager;

        private readonly List<TypedInputManager> _typedManagers;

        private ModifierCheckType _modifierCheckType;

        /// <summary>
        ///     Constructor
        /// </summary>
        public FullInputManager()
        {
            ModifierCheckType = ModifierCheckType.Strict;
            BufferedText = new DoubleBuffer<char>();
            _gamePadManager = new TypedInputManager(new GamePadDevice());
            _keyboardManager = new TypedInputManager(new KeyboardDevice());
            _mouseManager = new TypedInputManager(new MouseDevice());
            _injectableManager = new InjectableInputManager();
            _typedManagers = new List<TypedInputManager> {_gamePadManager, _keyboardManager, _mouseManager};
        }

        /// <summary>
        ///     How the manager checks modifiers
        /// </summary>
        public ModifierCheckType ModifierCheckType
        {
            get { return _modifierCheckType; }
            set
            {
                _modifierCheckType = value;
                foreach (var t in _typedManagers) t.Settings.ModifierCheckType = value;
            }
        }

        #region IKeyboardSubscriber Members

        /// <summary>
        ///     Handle a single character of input
        /// </summary>
        /// <param name="inputChar"> </param>
        public void ReceiveTextInput(char inputChar)
        {
            BufferedText.Push(inputChar);
        }

        /// <summary>
        ///     Handle a string of input
        /// </summary>
        /// <param name="text"> </param>
        public void ReceiveTextInput(string text)
        {
            foreach (char c in text)
                BufferedText.Push(c);
        }

        /// <summary>
        ///     Handle a special command
        /// </summary>
        /// <param name="command"> </param>
        public void ReceiveCommandInput(char command)
        {
            BufferedText.Push(command);
        }

        /// <summary>
        ///     Handle a Key input
        /// </summary>
        /// <param name="key"> </param>
        public void ReceiveSpecialInput(Keys key)
        {
        }

        /// <summary>
        ///     Does this Subscriber have the (possibly exclusive) focus
        /// </summary>
        public bool Selected { get; set; }

        #endregion

        #region InputManager Members

        private int _continuousCheckFrame;
        private int _framesPerContinuousCheck;

        protected int ContinuousCheckFrame
        {
            get { return _continuousCheckFrame; }
            set { _continuousCheckFrame = Basics.Mod(value, FramesPerContinuousCheck); }
        }

        public int FramesPerContinuousCheck
        {
            get { return _framesPerContinuousCheck; }
            set
            {
                _framesPerContinuousCheck = value;
                ContinuousCheckFrame = 0;
            }
        }

        /// <summary>
        ///     All the modifiers currently being tracked.
        /// </summary>
        public IEnumerable<InputBinding> GetModifiers
        {
            get
            {
                var collection = new CountedCollection<InputBinding>();
                foreach (var t in _typedManagers) collection.Merge((CountedCollection<InputBinding>) t.GetModifiers);
                return collection;
            }
        }

        public bool AddBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            bool added = false;

            if (binding is KeyBinding)
                added = _keyboardManager.AddBinding(bindingName, binding, player);
            else if (binding is MouseBinding)
                added = _mouseManager.AddBinding(bindingName, binding, player);
            else if (binding is ThumbstickDirectionBinding ||
                     binding is ThumbstickBinding ||
                     binding is ButtonBinding ||
                     binding is TriggerBinding)
                added = _gamePadManager.AddBinding(bindingName, binding, player);

            if (added) _injectableManager.AddBinding(bindingName, binding, player);

            return added;
        }

        public void RemoveBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            foreach (var t in _typedManagers) t.RemoveBinding(bindingName, binding, player);
        }

        public bool ContainsBinding(string bindingName, PlayerIndex player)
        {
            return _injectableManager.ContainsBinding(bindingName, player) ||
                   _typedManagers.Any(t => t.ContainsBinding(bindingName, player));
        }

        public void ClearBinding(string bindingName, PlayerIndex player)
        {
            foreach (var t in _typedManagers) t.ClearBinding(bindingName, player);
            _injectableManager.ClearBinding(bindingName, player);
        }

        public void ClearAllBindings()
        {
            foreach (var t in _typedManagers) t.ClearAllBindings();
            _injectableManager.ClearAllBindings();
        }


        public bool IsActive(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player))
                return false;

            return _injectableManager.IsActive(bindingName, player, state) ||
                   _typedManagers.Any(t => t.IsActive(bindingName, player, state));
        }

        public bool IsContinuousActive(string bindingName, PlayerIndex player, FrameState state)
        {
            var offset = state == FrameState.Current ? 0 : -1;
            var actualCheck = Basics.Mod(ContinuousCheckFrame + offset, FramesPerContinuousCheck);
            return actualCheck == 0 && IsActive(bindingName, player, state);
        }

        public List<InputBinding> GetCurrentBindings(string bindingName, PlayerIndex player)
        {
            var kb = _keyboardManager.GetCurrentBindings(bindingName, player);
            var mb = _mouseManager.GetCurrentBindings(bindingName, player);
            var gpb = _gamePadManager.GetCurrentBindings(bindingName, player);
            return kb.Union(mb).Union(gpb).ToList();
        }

        public List<string> BindingsUsing(InputBinding binding, PlayerIndex player)
        {
            var kb = _keyboardManager.BindingsUsing(binding, player);
            var mb = _mouseManager.BindingsUsing(binding, player);
            var gpb = _gamePadManager.BindingsUsing(binding, player);
            return kb.Union(mb).Union(gpb).ToList();
        }

        public void Update()
        {
            foreach (var t in _typedManagers) t.Update();
            _injectableManager.Update();
            ContinuousCheckFrame++;
            BufferedText.Flip();
        }

        public bool ContainsBinding(InputBinding binding, PlayerIndex player)
        {
            return _typedManagers.Any(t => t.ContainsBinding(binding, player));
        }

        #endregion

        /// <summary>
        ///     "Press" a key in a given frame.
        ///     Cannot press a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="bindingName"> The binding to press </param>
        /// <param name="player"> The player to press the binding for </param>
        /// <param name="state"> The frame to press it in </param>
        public void Press(string bindingName, PlayerIndex player, FrameState state)
        {
            _injectableManager.Press(bindingName, player, state);
        }

        /// <summary>
        ///     "Release" a key in a given frame.
        ///     Cannot release a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="bindingName"> The binding to release </param>
        /// <param name="player"> The player to release the binding for </param>
        /// <param name="state"> The frame to release it in </param>
        public void Release(string bindingName, PlayerIndex player, FrameState state)
        {
            _injectableManager.Release(bindingName, player, state);
        }

        /// <summary>
        ///     Initialize InputManager dependencies (For event-driven input)
        /// </summary>
        /// <param name="window"> </param>
        public static void Initialize(GameWindow window)
        {
            if (!initialized)
            {
                KeyboardDispatcher.Initialize(window);
                initialized = true;
            }
        }

        /// <summary>
        ///     The buffered text input since the last frame.  This is cleared per frame,
        ///     regardless of whether it has been read.
        /// </summary>
        public List<char> GetBufferedText()
        {
            return BufferedText.Front;
        }

        /// <summary>
        ///     Get the position of the mouse in the specified frame.
        /// </summary>
        /// <param name="state"> The frame to inspect for the position- the current frame or the previous frame </param>
        /// <returns> The position of the mouse in screen space </returns>
        public Vector2 GetMousePosition(FrameState state)
        {
            var mouseDevice = (MouseDevice) _mouseManager.Device;
            var mouseState = state == FrameState.Current
                                 ? mouseDevice.CurrentMouseState
                                 : mouseDevice.PreviousMouseState;
            return new Vector2(mouseState.X, mouseState.Y);
        }
    }
}
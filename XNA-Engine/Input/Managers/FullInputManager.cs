using System;
using System.Collections.Generic;
using System.Linq;
using Engine.DataStructures;
using Engine.Input.Devices;
using Microsoft.Xna.Framework;

namespace Engine.Input.Managers
{
    /// <summary>
    ///   Manages all types of InputBindings - mouse, keyboard, gamepad.
    /// </summary>
    public class FullInputManager : InputManager
    {
        private readonly TypedInputManager<GamePadDevice> _gamePadManager;
        private readonly InjectableInputManager _injectableManager;
        private readonly TypedInputManager<KeyboardDevice> _keyboardManager;
        private readonly TypedInputManager<MouseDevice> _mouseManager;
        private ModifierCheckType _modifierCheckType;

        /// <summary>
        ///   Constructor
        /// </summary>
        public FullInputManager()
        {
            ModifierCheckType = ModifierCheckType.Strict;
            _gamePadManager = new TypedInputManager<GamePadDevice>();
            _keyboardManager = new TypedInputManager<KeyboardDevice>();
            _mouseManager = new TypedInputManager<MouseDevice>();
            _injectableManager = new InjectableInputManager();
        }

        /// <summary>
        ///   How the manager checks modifiers
        /// </summary>
        public ModifierCheckType ModifierCheckType
        {
            get { return _modifierCheckType; }
            set
            {
                _modifierCheckType = value;
                _gamePadManager.Settings.ModifierCheckType = value;
                _keyboardManager.Settings.ModifierCheckType = value;
                _mouseManager.Settings.ModifierCheckType = value;
            }
        }

        #region InputManager Members

        public IEnumerable<InputBinding> GetModifiers
        {
            get
            {
                var collection = new CountedCollection<InputBinding>();
                collection.Merge((CountedCollection<InputBinding>) _gamePadManager.GetModifiers);
                collection.Merge((CountedCollection<InputBinding>) _keyboardManager.GetModifiers);
                collection.Merge((CountedCollection<InputBinding>) _mouseManager.GetModifiers);
                collection.Merge((CountedCollection<InputBinding>) _injectableManager.GetModifiers);
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
            _keyboardManager.RemoveBinding(bindingName, binding, player);
            _mouseManager.RemoveBinding(bindingName, binding, player);
            _gamePadManager.RemoveBinding(bindingName, binding, player);
            _injectableManager.RemoveBinding(bindingName, binding, player);
        }

        public bool ContainsBinding(string bindingName, PlayerIndex player)
        {
            return _injectableManager.ContainsBinding(bindingName, player) ||
                   _keyboardManager.ContainsBinding(bindingName, player) ||
                   _mouseManager.ContainsBinding(bindingName, player) ||
                   _gamePadManager.ContainsBinding(bindingName, player);
        }

        public void ClearBinding(string bindingName, PlayerIndex player)
        {
            _keyboardManager.ClearBinding(bindingName, player);
            _mouseManager.ClearBinding(bindingName, player);
            _gamePadManager.ClearBinding(bindingName, player);
            _injectableManager.ClearBinding(bindingName, player);
        }

        public void ClearAllBindings()
        {
            _keyboardManager.ClearAllBindings();
            _mouseManager.ClearAllBindings();
            _gamePadManager.ClearAllBindings();
            _injectableManager.ClearAllBindings();
        }

        public bool IsActive(string bindingName, PlayerIndex player, FrameState state)
        {
            throw new NotImplementedException();
        }

        public bool IsPressed(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Current) &&
                   !IsActive(bindingName, player, FrameState.Previous);
        }

        public bool IsReleased(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Previous) &&
                   !IsActive(bindingName, player, FrameState.Current);
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
            _gamePadManager.Update();
            _keyboardManager.Update();
            _mouseManager.Update();
            _injectableManager.Update();
        }

        #endregion

        /// <summary>
        ///   The buffered text input since the last frame.  This is cleared per frame,
        ///   regardless of whether it has been read.
        /// </summary>
        public List<char> GetBufferedText()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Get the position of the mouse in the specified frame.
        /// </summary>
        /// <param name="state"> The frame to inspect for the position- the current frame or the previous frame </param>
        /// <returns> The position of the mouse in screen space </returns>
        public Vector2 GetMousePosition(FrameState state)
        {
            var mouseDevice = _mouseManager.Device;
            var mouseState = state == FrameState.Current
                                 ? mouseDevice.CurrentMouseState
                                 : mouseDevice.PreviousMouseState;
            return new Vector2(mouseState.X, mouseState.Y);
        }
    }
}
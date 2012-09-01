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
        private readonly TypedInputManager<GamePadDevice> gamePadManager;
        private readonly InjectableInputManager injectableManager;
        private readonly TypedInputManager<KeyboardDevice> keyboardManager;
        private readonly TypedInputManager<MouseDevice> mouseManager;
        private ModifierCheckType _modifierCheckType;

        /// <summary>
        /// How the manager checks modifiers
        /// </summary>
        public ModifierCheckType ModifierCheckType
        {
            get { return _modifierCheckType; }
            set 
            { 
                _modifierCheckType = value;
                gamePadManager.Settings.ModifierCheckType = value;
                keyboardManager.Settings.ModifierCheckType = value;
                mouseManager.Settings.ModifierCheckType = value;
            }
        }

        /// <summary>
        ///   Constructor
        /// </summary>
        public FullInputManager()
        {
            ModifierCheckType = ModifierCheckType.Strict;
            gamePadManager = new TypedInputManager<GamePadDevice>();
            keyboardManager = new TypedInputManager<KeyboardDevice>();
            mouseManager = new TypedInputManager<MouseDevice>();
            injectableManager = new InjectableInputManager();
        }

        #region InputManager Members

        public IEnumerable<InputBinding> GetModifiers
        {
            get
            {
                var collection = new CountedCollection<InputBinding>();
                collection.Merge((CountedCollection<InputBinding>) gamePadManager.GetModifiers);
                collection.Merge((CountedCollection<InputBinding>) keyboardManager.GetModifiers);
                collection.Merge((CountedCollection<InputBinding>) mouseManager.GetModifiers);
                collection.Merge((CountedCollection<InputBinding>) injectableManager.GetModifiers);
                return collection;
            }
        }

        public bool AddBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            bool added = false;

            if (binding is KeyBinding)
                added = keyboardManager.AddBinding(bindingName, binding, player);
            else if (binding is MouseBinding)
                added = mouseManager.AddBinding(bindingName, binding, player);
            else if (binding is ThumbstickDirectionBinding ||
                     binding is ThumbstickBinding ||
                     binding is ButtonBinding ||
                     binding is TriggerBinding)
                added = gamePadManager.AddBinding(bindingName, binding, player);

            if (added) injectableManager.AddBinding(bindingName, binding, player);

            return added;
        }

        public void RemoveBinding(string bindingName, int index, PlayerIndex player)
        {
            keyboardManager.RemoveBinding(bindingName, index, player);
            mouseManager.RemoveBinding(bindingName, index, player);
            gamePadManager.RemoveBinding(bindingName, index, player);
            injectableManager.RemoveBinding(bindingName, index, player);
        }

        public void RemoveBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            keyboardManager.RemoveBinding(bindingName, binding, player);
            mouseManager.RemoveBinding(bindingName, binding, player);
            gamePadManager.RemoveBinding(bindingName, binding, player);
            injectableManager.RemoveBinding(bindingName, binding, player);
        }

        public bool ContainsBinding(string bindingName, PlayerIndex player)
        {
            return injectableManager.ContainsBinding(bindingName, player) ||
                   keyboardManager.ContainsBinding(bindingName, player) ||
                   mouseManager.ContainsBinding(bindingName, player) ||
                   gamePadManager.ContainsBinding(bindingName, player);
        }

        public void ClearBinding(string bindingName, PlayerIndex player)
        {
            keyboardManager.ClearBinding(bindingName, player);
            mouseManager.ClearBinding(bindingName, player);
            gamePadManager.ClearBinding(bindingName, player);
            injectableManager.ClearBinding(bindingName, player);
        }

        public void ClearAllBindings()
        {
            keyboardManager.ClearAllBindings();
            mouseManager.ClearAllBindings();
            gamePadManager.ClearAllBindings();
            injectableManager.ClearAllBindings();
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
            var kb = keyboardManager.GetCurrentBindings(bindingName, player);
            var mb = mouseManager.GetCurrentBindings(bindingName, player);
            var gpb = gamePadManager.GetCurrentBindings(bindingName, player);
            return kb.Union(mb).Union(gpb).ToList();
        }

        public List<string> BindingsUsing(InputBinding binding, PlayerIndex player)
        {
            var kb = keyboardManager.BindingsUsing(binding, player);
            var mb = mouseManager.BindingsUsing(binding, player);
            var gpb = gamePadManager.BindingsUsing(binding, player);
            return kb.Union(mb).Union(gpb).ToList();
        }

        public void Update()
        {
            gamePadManager.Update();
            keyboardManager.Update();
            mouseManager.Update();
            injectableManager.Update();
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
            var mouseDevice = mouseManager.Device;
            var mouseState = state == FrameState.Current
                                 ? mouseDevice.CurrentMouseState
                                 : mouseDevice.PreviousMouseState;
            return new Vector2(mouseState.X, mouseState.Y);
        }
    }
}
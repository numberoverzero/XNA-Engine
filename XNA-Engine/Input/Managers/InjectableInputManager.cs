using System;
using System.Collections.Generic;
using Engine.DataStructures;
using Microsoft.Xna.Framework;

namespace Engine.Input
{
    /// <summary>
    ///   Can inject presses
    /// </summary>
    public class InjectableInputManager : InputManager
    {
        /// <summary>
        ///   Programmatically injected binding presses
        /// </summary>
        protected CycleBuffer<FrameState, PlayerIndex, string> InjectedPressedKeys;

        /// <summary>
        ///   Keys that the manager "contains"
        /// </summary>
        protected DefaultDict<PlayerIndex, List<string>> PressableKeys;

        /// <summary>
        ///   Constructor
        /// </summary>
        public InjectableInputManager()
        {
            PressableKeys = new DefaultDict<PlayerIndex, List<string>>();
            InjectedPressedKeys = new CycleBuffer<FrameState, PlayerIndex, string>(FrameState.Current,
                                                                                   FrameState.Previous);
        }

        /// <summary>
        ///   Copy Constructor
        /// </summary>
        /// <param name="input"> </param>
        public InjectableInputManager(InjectableInputManager input)
        {
            InjectedPressedKeys = new CycleBuffer<FrameState, PlayerIndex, string>(input.InjectedPressedKeys);
        }

        #region InputManager Members

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
            throw new NotImplementedException();
        }

        /// <summary>
        ///   See <see cref="InputManager.AddBinding" />
        /// </summary>
        public bool AddBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            PressableKeys[player].Add(bindingName);
            return true;
        }

        /// <summary>
        ///   See <see cref="InputManager.RemoveBinding(string, int, PlayerIndex)" />
        /// </summary>
        public void RemoveBinding(string bindingName, int index, PlayerIndex player)
        {
            RemoveBinding(bindingName, player);
        }

        /// <summary>
        ///   See <see cref="InputManager.RemoveBinding(string, InputBinding, PlayerIndex)" />
        /// </summary>
        public void RemoveBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            RemoveBinding(bindingName, player);
        }

        /// <summary>
        ///   See <see cref="InputManager.ContainsBinding" />
        /// </summary>
        public bool ContainsBinding(string bindingName, PlayerIndex player)
        {
            return PressableKeys[player].Contains(bindingName);
        }

        /// <summary>
        ///   See <see cref="InputManager.ClearBinding" />
        /// </summary>
        public void ClearBinding(string bindingName, PlayerIndex player)
        {
            PressableKeys[player].Remove(bindingName);
        }

        /// <summary>
        ///   See <see cref="InputManager.ClearAllBindings" />
        /// </summary>
        public void ClearAllBindings()
        {
            PressableKeys = new DefaultDict<PlayerIndex, List<string>>();
        }

        /// <summary>
        ///   See <see cref="InputManager.IsActive" />
        /// </summary>
        public bool IsActive(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player)) return false;
            var injectedPresses = InjectedPressedKeys[state, player];
            bool isInjected = injectedPresses.Contains(bindingName);
            return isInjected;
        }

        /// <summary>
        ///   See <see cref="InputManager.IsPressed" />
        /// </summary>
        public bool IsPressed(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Current) &&
                   !IsActive(bindingName, player, FrameState.Previous);
        }

        /// <summary>
        ///   See <see cref="InputManager.IsReleased" />
        /// </summary>
        public bool IsReleased(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Previous) &&
                   !IsActive(bindingName, player, FrameState.Current);
        }

        /// <summary>
        ///   See <see cref="InputManager.GetCurrentBindings" />
        /// </summary>
        public List<InputBinding> GetCurrentBindings(string bindingName, PlayerIndex player)
        {
            throw new NotImplementedException(
                "Injectable input does not track the InputBindings associated with a string");
        }

        /// <summary>
        ///   See <see cref="InputManager.BindingsUsing" />
        /// </summary>
        public List<string> BindingsUsing(InputBinding binding, PlayerIndex player)
        {
            throw new NotImplementedException(
                "Injectable input does not track the InputBindings associated with a string");
        }

        /// <summary>
        ///   See <see cref="InputManager.Update" />
        /// </summary>
        public void Update()
        {
            InjectedPressedKeys.Cycle();
        }

        /// <summary>
        ///   See <see cref="InputManager.GetModifiers" />
        /// </summary>
        public IEnumerable<InputBinding> GetModifiers
        {
            get
            {
                throw new NotImplementedException(
                    "Injectable input does not track the modifiers associated with InputBindings");
            }
        }

        #endregion

        /// <summary>
        ///   "Press" a key in a given frame.
        ///   Cannot press a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="bindingName"> The binding to press </param>
        /// <param name="player"> The player to press the binding for </param>
        /// <param name="state"> The frame to press it in </param>
        public void Press(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player)) return;
            InjectedPressedKeys[state, player].Add(bindingName);
        }

        /// <summary>
        ///   "Release" a key in a given frame.
        ///   Cannot release a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="bindingName"> The binding to release </param>
        /// <param name="player"> The player to release the binding for </param>
        /// <param name="state"> The frame to release it in </param>
        public void Release(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player)) return;
            InjectedPressedKeys[state, player].Remove(bindingName);
        }

        private void RemoveBinding(string bindingName, PlayerIndex player)
        {
            PressableKeys[player].Remove(bindingName);
            InjectedPressedKeys[FrameState.Current, player].Remove(bindingName);
            InjectedPressedKeys[FrameState.Previous, player].Remove(bindingName);
        }
    }
}
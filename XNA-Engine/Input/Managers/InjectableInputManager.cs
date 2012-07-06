#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Engine.Input
{
    /// <summary>
    /// Can force keys to press or release using 
    /// Press(string key) and Release(string key)
    /// </summary>
    public class InjectableInputManager : InputManager
    {
        /// <summary>
        /// bindings pressed in the previous frame
        /// </summary>
        public HashSet<string> PreviousInjectedPresses { get; protected set; }
        
        /// <summary>
        /// bindings pressed in the current frame
        /// </summary>
        public HashSet<string> CurrentInjectedPresses { get; protected set; }

        public InjectableInputManager()
            : base()
        {
            PreviousInjectedPresses = new HashSet<string>();
            CurrentInjectedPresses = new HashSet<string>();
        }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        /// <remarks>
        /// This should be called at the beginning of your update loop, so that game logic
        /// uses latest values.
        /// Calling update at the end of update loop will have those keys processed
        /// in the next frame.
        /// </remarks>
        public override void Update()
        {
            PreviousInjectedPresses = new HashSet<string>(CurrentInjectedPresses);
            CurrentInjectedPresses.Clear();
            base.Update();
        }

        /// <summary>
        /// Returns if the keybinding associated with the string key is active in the specified frame.
        /// Active can mean pressed for buttons, or above threshold for thumbsticks/triggers
        /// </summary>
        /// <param name="key">The string that the keybinding was stored under</param>
        /// <param name="state">The frame to inspect for the press- the current frame or the previous frame</param>
        /// <returns></returns>
        public override bool IsActive(string key, PlayerIndex player, FrameState state)
        {
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            bool isInjected = injectedPresses.Contains(key);
            return isInjected || base.IsActive(key, player, state);
        }

        /// <summary>
        /// "Press" a key in a given frame.
        /// Cannot press a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="key">The binding to press</param>
        /// <param name="state">The frame to press it in</param>
        public void Press(string key, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(key, player)) return;
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            injectedPresses.Add(key);
        }

        /// <summary>
        /// "Release" a key in a given frame.
        /// Cannot release a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="key">The binding to release</param>
        /// <param name="state">The frame to release it in</param>
        public void Release(string key, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(key, player)) return;
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            injectedPresses.Remove(key);
        }

        /// <summary>
        /// Removes the binding associated with the specified key
        /// </summary>
        /// <param name="key">The name of the keybinding to remove</param>
        public override void RemoveBinding(string key, int index, PlayerIndex player)
        {
            PreviousInjectedPresses.Remove(key);
            CurrentInjectedPresses.Remove(key);
            base.RemoveBinding(key, index, player);
        }
    }
}

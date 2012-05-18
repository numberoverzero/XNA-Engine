using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Input
{
    /// <summary>
    /// Can force keys to press or release using 
    /// Press(string key) and Release(string key)
    /// </summary>
    public class InjectableInputManager : InputManager
    {
        public HashSet<string> PreviousInjectedPresses { get; protected set; }
        public HashSet<string> CurrentInjectedPresses { get; protected set; }

        public InjectableInputManager()
            : base()
        {
            PreviousInjectedPresses = new HashSet<string>();
            CurrentInjectedPresses = new HashSet<string>();
        }

        public override void Update()
        {
            PreviousInjectedPresses = new HashSet<string>(CurrentInjectedPresses);
            CurrentInjectedPresses.Clear();

            base.Update();
        }

        public override bool IsActive(string key, FrameState state = FrameState.Current)
        {
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            bool isInjected = injectedPresses.Contains(key);
            return isInjected || base.IsActive(key, state);
        }

        /// <summary>
        /// "Press" a key in a given frame.
        /// Cannot press a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="key">The binding to press</param>
        /// <param name="state">The frame to press it in</param>
        public void Press(string key, FrameState state = FrameState.Current)
        {
            if (!HasBinding(key)) return;
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            injectedPresses.Add(key);
        }

        /// <summary>
        /// "Release" a key in a given frame.
        /// Cannot release a binding unless it has been added to the InputManager
        /// </summary>
        /// <param name="key">The binding to release</param>
        /// <param name="state">The frame to release it in</param>
        public void Release(string key, FrameState state = FrameState.Current)
        {
            if (!HasBinding(key)) return;
            var injectedPresses = state == FrameState.Current ? CurrentInjectedPresses : PreviousInjectedPresses;
            injectedPresses.Remove(key);
        }

        public override void RemoveBinding(string key)
        {
            PreviousInjectedPresses.Remove(key);
            CurrentInjectedPresses.Remove(key);
            base.RemoveBinding(key);
        }
    }
}

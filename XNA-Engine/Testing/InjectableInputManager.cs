using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Input;

namespace Engine.Testing
{
    public class InjectableInputManager : InputManager
    {
        public HashSet<string> lastInjectedPresses { get; protected set; }
        public HashSet<string> currentInjectedPresses { get; protected set; }

        public InjectableInputManager()
            : base()
        {
            lastInjectedPresses = new HashSet<string>();
            currentInjectedPresses = new HashSet<string>();
        }

        public override void Update()
        {
            lastInjectedPresses = new HashSet<string>(currentInjectedPresses);
            currentInjectedPresses.Clear();

            base.Update();
        }

        public override bool IsActive(string key, FrameState state = FrameState.Current)
        {
            var injectedPresses = state == FrameState.Current ? currentInjectedPresses : lastInjectedPresses;
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
            var injectedPresses = state == FrameState.Current ? currentInjectedPresses : lastInjectedPresses;
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
            var injectedPresses = state == FrameState.Current ? currentInjectedPresses : lastInjectedPresses;
            injectedPresses.Remove(key);
        }

        public override void RemoveBinding(string key)
        {
            lastInjectedPresses.Remove(key);
            currentInjectedPresses.Remove(key);
            base.RemoveBinding(key);
        }
    }
}

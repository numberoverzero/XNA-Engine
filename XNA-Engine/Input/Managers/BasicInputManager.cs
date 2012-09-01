﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Engine.Input
{
    /// <summary>
    /// An InjectableInputManager that has shortcut methods for 
    /// adding basic input controls (thumbstickDirection, button, etc)
    /// </summary>
    public class BasicInputManager : DefaultInputManager
    {
        /// <summary>
        /// Add a ThumbstickDirection binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="player"></param>
        /// <param name="thumbstickDirection"></param>
        /// <param name="thumbstick"></param>
        /// <param name="modifiers"></param>
        public bool AddBinding(string bindingName, PlayerIndex player, ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new ThumbstickDirectionBinding(thumbstickDirection, thumbstick, modifiers);
            return AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a MouseButton binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="player"></param>
        /// <param name="mouseButton"></param>
        /// <param name="modifiers"></param>
        public bool AddBinding(string bindingName, PlayerIndex player, MouseButton mouseButton, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new MouseBinding(mouseButton, modifiers);
            return AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a Thumbstick binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="player"></param>
        /// <param name="thumbstick"></param>
        /// <param name="modifiers"></param>
        public bool AddBinding(string bindingName, PlayerIndex player, Thumbstick thumbstick, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new ThumbstickBinding(thumbstick, modifiers);
            return AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a Trigger binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="player"></param>
        /// <param name="trigger"></param>
        /// <param name="modifiers"></param>
        public bool AddBinding(string bindingName, PlayerIndex player, Trigger trigger, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new TriggerBinding(trigger, modifiers);
            return AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a Button binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="player"></param>
        /// <param name="button"></param>
        /// <param name="modifiers"></param>
        public bool AddBinding(string bindingName, PlayerIndex player, Buttons button, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new ButtonBinding(button, modifiers);
            return AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a key binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="player"></param>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        public bool AddBinding(string bindingName, PlayerIndex player, Keys key, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new KeyBinding(key, modifiers);
            return AddBinding(bindingName, inputBinding, player);
        }
    }
}

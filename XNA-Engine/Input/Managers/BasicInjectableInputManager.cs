using System;
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
    public class BasicInjectableInputManager : InjectableInputManager
    {
        /// <summary>
        /// Add a ThumbstickDirection binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="thumbstickDirection"></param>
        /// <param name="thumbstick"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, PlayerIndex player, ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params IBinding[] modifiers)
        {
            InputBinding inputBinding = new ThumbstickDirectionInputBinding(thumbstickDirection, thumbstick, modifiers);
            AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a MouseButton binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="mouseButton"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, PlayerIndex player, MouseButton mouseButton, params IBinding[] modifiers)
        {
            InputBinding inputBinding = new MouseInputBinding(mouseButton, modifiers);
            AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a Thumbstick binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="thumbstick"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, PlayerIndex player, Thumbstick thumbstick, params IBinding[] modifiers)
        {
            InputBinding inputBinding = new ThumbstickInputBinding(thumbstick, modifiers);
            AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a Trigger binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="trigger"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, PlayerIndex player, Trigger trigger, params IBinding[] modifiers)
        {
            InputBinding inputBinding = new TriggerInputBinding(trigger, modifiers);
            AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a Button binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="button"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, PlayerIndex player, Buttons button, params IBinding[] modifiers)
        {
            InputBinding inputBinding = new ButtonInputBinding(button, modifiers);
            AddBinding(bindingName, inputBinding, player);
        }
        /// <summary>
        /// Add a key binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"></param>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        public void AddBinding(string bindingName, PlayerIndex player, Keys key, params IBinding[] modifiers)
        {
            InputBinding inputBinding = new KeyInputBinding(key, modifiers);
            AddBinding(bindingName, inputBinding, player);
        }
    }
}

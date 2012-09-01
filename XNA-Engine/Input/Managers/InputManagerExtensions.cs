using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
    /// <summary>
    ///   Shortcut methods for adding various specific input bindings
    /// </summary>
    public static class InputManagerExtensions
    {
        /// <summary>
        ///   Add a ThumbstickDirection binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName,
                                      PlayerIndex player, ThumbstickDirection thumbstickDirection,
                                      Thumbstick thumbstick, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new ThumbstickDirectionBinding(thumbstickDirection, thumbstick, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///   Add a MouseButton binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player,
                                      MouseButton mouseButton, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new MouseBinding(mouseButton, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///   Add a Thumbstick binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player,
                                      Thumbstick thumbstick, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new ThumbstickBinding(thumbstick, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///   Add a Trigger binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player,
                                      Trigger trigger, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new TriggerBinding(trigger, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///   Add a Button binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player,
                                      Buttons button, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new ButtonBinding(button, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///   Add a key binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player, Keys key,
                                      params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new KeyBinding(key, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }
    }
}
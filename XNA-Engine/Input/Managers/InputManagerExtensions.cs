using Engine.Input.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input.Managers.AddBindings
{
    /// <summary>
    ///     Shortcut methods for adding various specific input bindings
    /// </summary>
    public static class InputManagerAddBindingExtensions
    {
        /// <summary>
        ///     Add a ThumbstickDirection binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName,
                                      PlayerIndex player, ThumbstickDirection thumbstickDirection,
                                      Thumbstick thumbstick, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new ThumbstickDirectionBinding(thumbstickDirection, thumbstick, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///     Add a MouseButton binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player,
                                      MouseButton mouseButton, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new MouseBinding(mouseButton, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///     Add a Thumbstick binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player,
                                      Thumbstick thumbstick, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new ThumbstickBinding(thumbstick, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///     Add a Trigger binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player,
                                      Trigger trigger, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new TriggerBinding(trigger, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///     Add a Button binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player,
                                      Buttons button, params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new ButtonBinding(button, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///     Add a key binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        public static bool AddBinding(this InputManager inputManager, string bindingName, PlayerIndex player, Keys key,
                                      params InputBinding[] modifiers)
        {
            DefaultBinding inputBinding = new KeyBinding(key, modifiers);
            return inputManager.AddBinding(bindingName, inputBinding, player);
        }

        /// <summary>
        ///     Checks if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous). </returns>
        public static bool IsPressed(this InputManager inputManager, string bindingName, PlayerIndex player)
        {
            return inputManager.IsActive(bindingName, player, FrameState.Current) &&
                   !inputManager.IsActive(bindingName, player, FrameState.Previous);
        }

        /// <summary>
        ///     Checks if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous). </returns>
        public static bool IsReleased(this InputManager inputManager, string bindingName, PlayerIndex player)
        {
            return inputManager.IsActive(bindingName, player, FrameState.Previous) &&
                   !inputManager.IsActive(bindingName, player, FrameState.Current);
        }
    }
}

namespace Engine.Input.Managers.SinglePlayer
{
    public static class InputManagerSinglePlayerExtensions
    {
        public static bool IsActive(this InputManager inputManager, string bindingName, FrameState frameState)
        {
            return inputManager.IsActive(bindingName, PlayerIndex.One, frameState);
        }

        public static bool IsContinuousActive(this InputManager inputManager, string bindingName, FrameState frameState)
        {
            return inputManager.IsContinuousActive(bindingName, PlayerIndex.One, frameState);
        }

        public static bool IsPressed(this InputManager inputManager, string bindingName)
        {
            return inputManager.IsPressed(bindingName, PlayerIndex.One);
        }

        public static bool IsReleased(this InputManager inputManager, string bindingName)
        {
            return inputManager.IsReleased(bindingName, PlayerIndex.One);
        }
    }
}

namespace Engine.Input
{
    public static class InputManagerExtensions
    {
        /// <summary>
        ///     Checks if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous). </returns>
        public static bool IsPressed(this InputManager inputManager, string bindingName, PlayerIndex player)
        {
            return inputManager.IsActive(bindingName, player, FrameState.Current) &&
                   !inputManager.IsActive(bindingName, player, FrameState.Previous);
        }

        /// <summary>
        ///     Checks if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous). </returns>
        public static bool IsReleased(this InputManager inputManager, string bindingName, PlayerIndex player)
        {
            return inputManager.IsActive(bindingName, player, FrameState.Previous) &&
                   !inputManager.IsActive(bindingName, player, FrameState.Current);
        }
    }
}
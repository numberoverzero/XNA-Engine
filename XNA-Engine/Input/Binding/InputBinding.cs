using System.Collections.Generic;

namespace Engine.Input
{
    /// <summary>
    ///   A Binding for input that can be checked for activity, using states for the keyboard, mouse, and gamepads.
    /// </summary>
    public interface InputBinding
    {
        /// <summary>
        ///   Returns the list of modifiers necessary to be active before the binding is considered "active"
        /// </summary>
        List<InputBinding> Modifiers { get; set; }

        /// <summary>
        ///   True if the InputBinding is active in the given FrameState of the given InputManager
        /// </summary>
        bool IsActive(InputSnapshot inputSnapshot);

        /// <summary>
        ///   Compares this Binding to another, and returns whether they are the same (with/without modifiers)
        /// </summary>
        bool IsEqual(InputBinding other, bool includeModifiers = false);
    }
}
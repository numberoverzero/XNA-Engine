using System.Collections.Generic;

namespace Engine.Input.Managers
{
    /// <summary>
    ///     Allows input querying and offers hooks for event-driven keyboard input
    /// </summary>
    public interface InputManager
    {
        /// <summary>
        ///     How many frames elapse between each check for a "continuous" action.
        ///     The inverse of this would be repeat rate - the number of times
        ///     the button triggers when held down.
        /// </summary>
        int FramesPerContinuousCheck { get; set; }

        /// <summary>
        ///     All the modifiers currently being tracked.
        /// </summary>
        IEnumerable<InputBinding> GetModifiers { get; }

        /// <summary>
        ///     Add a binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"> The string used to query the binding state </param>
        /// <param name="binding"> The binding to associate with the bindingName </param>
        /// <returns> true if the binding was added </returns>
        bool AddBinding(string bindingName, InputBinding binding);

        /// <summary>
        ///     Remove a binding from the InputManager.  Removes the exact binding from the relation.
        ///     This can be used when you don't know the binding's index in its list of bindings.
        /// </summary>
        /// <param name="bindingName"> The string used to query the binding state </param>
        /// <param name="binding"> The binding to remove from the association with the bindingName </param>
        void RemoveBinding(string bindingName, InputBinding binding);

        /// <summary>
        ///     Check if the manager has a binding associated with a bindingName
        /// </summary>
        /// <param name="bindingName"> The name of the binding to check for </param>
        /// <returns> True if there are bindings associated with the bindingName</returns>
        bool ContainsBinding(string bindingName);

        /// <summary>
        ///     Check if the manager has a specific binding
        /// </summary>
        /// <param name="binding"> The binding to check for </param>
        /// <returns> True if the binding exists </returns>
        bool ContainsBinding(InputBinding binding);

        /// <summary>
        ///     Clears all bindings associated with the given bindingName
        /// </summary>
        /// <param name="bindingName"> The name of the binding to clear </param>
        void ClearBinding(string bindingName);

        /// <summary>
        ///     Clears all bindings
        /// </summary>
        void ClearAllBindings();

        /// <summary>
        ///     Checks if any of the bindings associated with the bindingNamein a given FrameState is active.
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="state"> The FrameState in which to check for activity </param>
        /// <returns> True if any of the bindings associated with the bindingName in a given FrameState is active. </returns>
        bool IsActive(string bindingName, FrameState state);

        /// <summary>
        ///     Checks if any of the bindings associated with the bindingNamein a given FrameState is active,
        ///     as determined by the "FramesPerContinuousCheck" variable.
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="state"> The FrameState in which to check for activity </param>
        /// <returns> True if any of the bindings associated with the bindingName in a given FrameState is active. </returns>
        bool IsContinuousActive(string bindingName, FrameState state);

        /// <summary>
        ///     Checks if any of the bindings associated with the bindingName was pressed in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <returns> True if any of the bindings associated with the bindingName was pressed in the current FrameState (and not in the previous). </returns>
        bool IsPressed(string bindingName);

        /// <summary>
        ///     Checks if any of the bindings associated with the bindingName was released in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <returns> True if any of the bindings associated with the bindingName was released in the current FrameState (and not in the previous). </returns>
        bool IsReleased(string bindingName);

        /// <summary>
        ///     Gets the list of bindings associated with a particular bindingName
        /// </summary>
        /// <param name="bindingName"> The bindingName associated with the list of Bindings </param>
        /// <returns> Returns a copy of the Bindings associated with the bindingName </returns>
        List<InputBinding> GetCurrentBindings(string bindingName);

        /// <summary>
        ///     Used to get a list of strings that map to the given binding.
        ///     This is useful when you want to unbind a key from current bindings and remap to a new binding:
        ///     You can present a dialog such as "{key} is currently mapped to {List of Bindings using {key}}.  Are you sure you want to remap {key} to {New binding}?"
        /// </summary>
        /// <param name="binding"> The binding to search for in the InputManager </param>
        /// <returns> A list of the bindingNames that track the given binding as a possible input </returns>
        List<string> BindingsUsing(InputBinding binding);

        /// <summary>
        ///     Reads the latest state of the keyboard, mouse, and gamepad. (If polling is enabled for these devices)
        /// </summary>
        /// <remarks>
        ///     This should be called at the end of your update loop, so that game logic
        ///     uses latest values.
        ///     Calling update at the beginning of the update loop will clear current buffers (if any) which
        ///     means you will not be able to read the most recent input.
        /// </remarks>
        void Update();
    }
}
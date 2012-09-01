#region Using Statements

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace Engine.Input
{
    /// <summary>
    ///   Allows input querying and offers hooks for event-driven keyboard input
    /// </summary>
    public interface InputManager
    {
        /// <summary>
        ///   All the modifiers currently being tracked.
        /// </summary>
        IEnumerable<InputBinding> GetModifiers { get; }

        /// <summary>
        ///   The buffered text input since the last frame.  This is cleared per frame,
        ///   regardless of whether it has been read.
        /// </summary>
        List<char> GetBufferedText();

        /// <summary>
        ///   Get the position of the mouse in the specified frame.
        /// </summary>
        /// <param name="state"> The frame to inspect for the position- the current frame or the previous frame </param>
        /// <returns> The position of the mouse in screen space </returns>
        Vector2 GetMousePosition(FrameState state);

        /// <summary>
        ///   Add a binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"> The string used to query the binding state </param>
        /// <param name="binding"> The binding to associate with the bindingName </param>
        /// <param name="player"> The player to add the binding for </param>
        /// <returns> true if the binding was added </returns>
        bool AddBinding(string bindingName, InputBinding binding, PlayerIndex player);

        /// <summary>
        ///   Remove a binding from the InputManager.  This removes a binding by its index against a bindingName.
        ///   For the binding {"jump": [Binding{Keys.Space}, Binding{Buttons.A}, Binding{Keys.W}]} the command
        ///   RemoveBinding("jump", 1, PlayerIndex.One) removes the Buttons.A binding for "jump".
        ///   This is useful when you know the index of the binding in its list of bindings
        /// </summary>
        /// <param name="bindingName"> The string used to query the binding state </param>
        /// <param name="index"> The index of the binding in the list of bindings associated with the bindingName </param>
        /// <param name="player"> The player the binding is being removed for </param>
        void RemoveBinding(string bindingName, int index, PlayerIndex player);

        /// <summary>
        ///   Remove a binding from the InputManager.  Removes the exact binding from the relation.
        ///   This can be used when you don't know the binding's index in its list of bindings.
        /// </summary>
        /// <param name="bindingName"> The string used to query the binding state </param>
        /// <param name="binding"> The binding to remove from the association with the bindingName </param>
        /// <param name="player"> The player the binding is being removed for </param>
        void RemoveBinding(string bindingName, InputBinding binding, PlayerIndex player);

        /// <summary>
        ///   Check if the manager has a binding associated with a bindingName for a player
        /// </summary>
        /// <param name="bindingName"> The name of the binding to check for </param>
        /// <param name="player"> The player to check the binding for </param>
        /// <returns> True if there are bindings associated with the bindingName for the given player </returns>
        bool ContainsBinding(string bindingName, PlayerIndex player);

        /// <summary>
        ///   Clears all bindings associated with the given bindingName for a particular player
        /// </summary>
        /// <param name="bindingName"> The name of the binding to clear </param>
        /// <param name="player"> The player to clear the binding for </param>
        void ClearBinding(string bindingName, PlayerIndex player);

        /// <summary>
        ///   Clears all bindings for all players
        /// </summary>
        void ClearAllBindings();

        /// <summary>
        ///   Checks if any of the bindings associated with the bindingName for a given player in a given FrameState is active.
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <param name="state"> The FrameState in which to check for activity </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player in a given FrameState is active. </returns>
        bool IsActive(string bindingName, PlayerIndex player, FrameState state);

        /// <summary>
        ///   Checks if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous). </returns>
        bool IsPressed(string bindingName, PlayerIndex player);

        /// <summary>
        ///   Checks if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous). </returns>
        bool IsReleased(string bindingName, PlayerIndex player);

        /// <summary>
        ///   Gets the list of bindings associated with a particular bindingName for a given player
        /// </summary>
        /// <param name="bindingName"> The bindingName associated with the list of Bindings </param>
        /// <param name="player"> The player to get the list of bindings for </param>
        /// <returns> Returns a copy of the Bindings associated with the bindingName for a givem player </returns>
        List<InputBinding> GetCurrentBindings(string bindingName, PlayerIndex player);

        /// <summary>
        ///   Used to get a list of strings that map to the given binding for a given player.
        ///   This is useful when you want to unbind a key from current bindings and remap to a new binding:
        ///   You can present a dialog such as "{key} is currently mapped to {List of Bindings using {key}}.  Are you sure you want to remap {key} to {New binding}?"
        /// </summary>
        /// <param name="binding"> The binding to search for in the InputManager </param>
        /// <param name="player"> The player to search for bindings on </param>
        /// <returns> A list of the bindingNames that, for a given player, track the given binding as a possible input </returns>
        List<string> BindingsUsing(InputBinding binding, PlayerIndex player);

        /// <summary>
        ///   Reads the latest state of the keyboard, mouse, and gamepad. (If polling is enabled for these devices)
        /// </summary>
        /// <remarks>
        ///   This should be called at the end of your update loop, so that game logic
        ///   uses latest values.
        ///   Calling update at the beginning of the update loop will clear current buffers (if any) which
        ///   means you will not be able to read the most recent input.
        /// </remarks>
        void Update();
    }
}
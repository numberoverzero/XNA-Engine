using System;
using System.Collections.Generic;
using System.Linq;
using Engine.DataStructures;
using Engine.Input.Devices;
using Microsoft.Xna.Framework;

namespace Engine.Input.Managers
{
    /// <summary>
    ///   Manages bindings of keys
    /// </summary>
    public abstract class TypedInputManager<TInputBinding, TInputDevice> : InputManager where TInputBinding : class, InputBinding where TInputDevice : class, InputDevice, new()
    {
        /// <summary>
        ///   Constructor
        /// </summary>
        public TypedInputManager()
        {
            Bindings = new MultiKeyObjDict<string, PlayerIndex, List<InputBinding>>();
            Modifiers = new CountedCollection<InputBinding>();
            Device = new TInputDevice();
        }

        /// <summary>
        /// The device that this manager relies on
        /// </summary>
        protected TInputDevice Device { get; set; }

        /// <summary>
        ///   The Bindings being tracked by the Manager
        /// </summary>
        protected MultiKeyObjDict<String, PlayerIndex, List<InputBinding>> Bindings { get; set; }

        /// <summary>
        ///   The InputSettings for this InputManager (trigger thresholds, etc)
        /// </summary>
        protected InputSettings Settings { get; set; }

        /// <summary>
        ///   A unique set of modifiers of the bindings this manager tracks.
        ///   Keeps track of how many bindings use this modifier; 
        ///   stops checking for modifiers once no bindings use that modifier
        /// </summary>
        public ICollection<InputBinding> Modifiers { get; protected set; }

        /// <summary>
        ///   Enable/Disable grabbing device state when updating the manager.
        ///   Disable for performance when you know the user can't use the device, or no bindings will need the state of the device.
        /// </summary>
        public bool IsPolling { get; set; }

        #region InputManager Members

        /// <summary>
        ///   All the modifiers currently being tracked.
        /// </summary>
        public IEnumerable<InputBinding> GetModifiers
        {
            get { return Modifiers; }
        }

        /// <summary>
        ///   The buffered text input since the last frame.  This is cleared per frame,
        ///   regardless of whether it has been read.
        /// </summary>
        public List<char> GetBufferedText()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Get the position of the mouse in the specified frame.
        /// </summary>
        /// <param name="state"> The frame to inspect for the position- the current frame or the previous frame </param>
        /// <returns> The position of the mouse in screen space </returns>
        public Vector2 GetMousePosition(FrameState state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Add a binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"> The string used to query the binding state </param>
        /// <param name="binding"> The binding to associate with the bindingName </param>
        /// <param name="player"> The player to add the binding for </param>
        public bool AddBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            var tBinding = binding as TInputBinding;
            if (tBinding == null) 
                return false;
            
            var bindings = Bindings[bindingName, player];
            if (bindings.Contains(binding))
                return true;

            bindings.Add(binding);
            foreach (var modifier in binding.Modifiers)
                Modifiers.Add(modifier);
            return true;
        }

        /// <summary>
        ///   Remove a binding from the InputManager.  This removes a binding by its index against a bindingName.
        ///   For the binding {"jump": [Binding{Keys.Space}, Binding{Buttons.A}, Binding{Keys.W}]} the command
        ///   RemoveBinding("jump", 1, PlayerIndex.One) removes the Buttons.A binding for "jump".
        ///   This is useful when you know the index of the binding in its list of bindings
        /// </summary>
        /// <param name="bindingName"> The string used to query the binding state </param>
        /// <param name="index"> The index of the binding in the list of bindings associated with the bindingName </param>
        /// <param name="player"> The player the binding is being removed for </param>
        public virtual void RemoveBinding(string bindingName, int index, PlayerIndex player)
        {
            if (!ContainsBinding(bindingName, player))
                return;

            var bindings = Bindings[bindingName, player];

            if (index < 0 || index >= bindings.Count)
                return;

            var binding = bindings[index];

            foreach (var modifier in binding.Modifiers)
                Modifiers.Remove(modifier);
            bindings.RemoveAt(index);
        }

        /// <summary>
        ///   Remove a binding from the InputManager.  Removes the exact binding from the relation.
        ///   This can be used when you don't know the binding's index in its list of bindings.
        /// </summary>
        /// <param name="bindingName"> The string used to query the binding state </param>
        /// <param name="binding"> The binding to remove from the association with the bindingName </param>
        /// <param name="player"> The player the binding is being removed for </param>
        public virtual void RemoveBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            if (!ContainsBinding(bindingName, player))
                return;

            var bindings = Bindings[bindingName, player];
            int index = bindings.IndexOf(binding);
            RemoveBinding(bindingName, index, player);
        }

        /// <summary>
        ///   Check if the manager has a binding associated with a bindingName for a player
        /// </summary>
        /// <param name="bindingName"> The name of the binding to check for </param>
        /// <param name="player"> The player to check the binding for </param>
        /// <returns> True if there are bindings associated with the bindingName for the given player </returns>
        public virtual bool ContainsBinding(string bindingName, PlayerIndex player)
        {
            return Bindings[bindingName, player].Count > 0;
        }

        /// <summary>
        ///   Clears all bindings associated with the given bindingName for a particular player
        /// </summary>
        /// <param name="bindingName"> The name of the binding to clear </param>
        /// <param name="player"> The player to clear the binding for </param>
        public virtual void ClearBinding(string bindingName, PlayerIndex player)
        {
            // Make sure we clean up any modifiers
            var oldBindings = new List<InputBinding>(Bindings[bindingName, player]);
            foreach (var binding in oldBindings)
                RemoveBinding(bindingName, binding, player);
            Bindings[bindingName, player] = new List<InputBinding>();
        }

        /// <summary>
        ///   Clears all bindings for all players
        /// </summary>
        public virtual void ClearAllBindings()
        {
            Bindings.Clear();
            Modifiers.Clear();
        }

        /// <summary>
        ///   Checks if any of the bindings associated with the bindingName for a given player in a given FrameState is active.
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <param name="state"> The FrameState in which to check for activity </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player in a given FrameState is active. </returns>
        public virtual bool IsActive(string bindingName, PlayerIndex player, FrameState state)
        {
            var bindings = Bindings[bindingName, player];

            var inputSnapshot = Device.GetDeviceSnapshot(player, state);

            return bindings.Any(binding => binding.IsActive(inputSnapshot) && IsModifiersActive(binding, inputSnapshot));
        }

        /// <summary>
        ///   Checks if sufficient modifiers are active, as defined by the ModifierCheckType in Settings
        /// </summary>
        protected virtual bool IsModifiersActive(InputBinding inputBinding, InputSnapshot inputSnapshot)
        {
            return false;
        }

        /// <summary>
        ///   Checks if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player was pressed in the current FrameState (and not in the previous). </returns>
        public virtual bool IsPressed(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Current) &&
                   !IsActive(bindingName, player, FrameState.Previous);
        }

        /// <summary>
        ///   Checks if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous).
        /// </summary>
        /// <param name="bindingName"> The name of the binding to query for active state </param>
        /// <param name="player"> The player to check the binding's activity for </param>
        /// <returns> True if any of the bindings associated with the bindingName for a given player was released in the current FrameState (and not in the previous). </returns>
        public virtual bool IsReleased(string bindingName, PlayerIndex player)
        {
            return IsActive(bindingName, player, FrameState.Previous) &&
                   !IsActive(bindingName, player, FrameState.Current);
        }

        /// <summary>
        ///   Gets the list of bindings associated with a particular bindingName for a given player
        /// </summary>
        /// <param name="bindingName"> The bindingName associated with the list of Bindings </param>
        /// <param name="player"> The player to get the list of bindings for </param>
        /// <returns> Returns a copy of the Bindings associated with the bindingName for a givem player </returns>
        public List<InputBinding> GetCurrentBindings(string bindingName, PlayerIndex player)
        {
            return new List<InputBinding>(Bindings[bindingName, player]);
        }

        /// <summary>
        ///   Used to get a list of strings that map to the given binding for a given player.
        ///   This is useful when you want to unbind a key from current bindings and remap to a new binding:
        ///   You can present a dialog such as "{key} is currently mapped to {List of Bindings using {key}}.  Are you sure you want to remap {key} to {New binding}?"
        /// </summary>
        /// <param name="binding"> The binding to search for in the InputManager </param>
        /// <param name="player"> The player to search for bindings on </param>
        /// <returns> A list of the bindingNames that, for a given player, track the given binding as a possible input </returns>
        public List<string> BindingsUsing(InputBinding binding, PlayerIndex player = PlayerIndex.One)
        {
            return (from bindingGroupKey in Bindings.Keys
                    let bindingGroup = Bindings[bindingGroupKey, player]
                    where bindingGroup.Contains(binding)
                    select bindingGroupKey).ToList();
        }

        /// <summary>
        ///   Reads the latest state of the keyboard, mouse, and gamepad. (If polling is enabled for these devices)
        /// </summary>
        /// <remarks>
        ///   This should be called at the end of your update loop, so that game logic
        ///   uses latest values.
        ///   Calling update at the beginning of the update loop will clear current buffers (if any) which
        ///   means you will not be able to read the most recent input.
        /// </remarks>
        public void Update()
        {
            if(IsPolling) Device.Update();
        }

        #endregion
    }
}
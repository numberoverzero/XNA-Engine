using System;
using System.Collections.Generic;
using System.Linq;
using Engine.DataStructures;
using Engine.Input.Devices;
using Engine.Utility;
using Microsoft.Xna.Framework;

namespace Engine.Input.Managers
{
    /// <summary>
    ///   Manages bindings of keys
    /// </summary>
    public class TypedInputManager : InputManager
    {
        /// <summary>
        ///   Constructor
        /// </summary>
        public TypedInputManager(InputDevice inputDevice)
        {
            Settings = new InputSettings(0, 0, ModifierCheckType.Strict);
            Bindings = new MultiKeyObjDict<string, PlayerIndex, List<InputBinding>>();
            Modifiers = new CountedCollection<InputBinding>();
            Device = inputDevice;
            IsPolling = true;
        }

        /// <summary>
        ///   Copy Constructor
        /// </summary>
        /// <param name="inputManager"> </param>
        public TypedInputManager(TypedInputManager inputManager)
        {
            Settings = inputManager.Settings;
            Bindings = new MultiKeyObjDict<string, PlayerIndex, List<InputBinding>>(inputManager.Bindings);
            Modifiers = new CountedCollection<InputBinding>(inputManager.Modifiers);
            Device = inputManager.Device;
            IsPolling = inputManager.IsPolling;
        }

        /// <summary>
        ///   The device that this manager relies on
        /// </summary>
        public InputDevice Device { get; protected set; }

        /// <summary>
        ///   The Bindings being tracked by the Manager
        /// </summary>
        protected MultiKeyObjDict<String, PlayerIndex, List<InputBinding>> Bindings { get; set; }

        /// <summary>
        ///   The InputSettings for this InputManager (trigger thresholds, etc)
        /// </summary>
        public InputSettings Settings { get; set; }

        /// <summary>
        ///   A unique set of modifiers of the bindings this manager tracks.
        ///   Keeps track of how many bindings use this modifier; 
        ///   stops checking for modifiers once no bindings use that modifier
        /// </summary>
        public CountedCollection<InputBinding> Modifiers { get; protected set; }

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
        ///   Add a binding that can be checked for state (Pressed, Released, Active)
        /// </summary>
        /// <param name="bindingName"> The string used to query the binding state </param>
        /// <param name="binding"> The binding to associate with the bindingName </param>
        /// <param name="player"> The player to add the binding for </param>
        public bool AddBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            var bindings = Bindings[bindingName, player];
            if (bindings.Contains(binding))
                return true;

            bindings.Add(binding);
            foreach (var modifier in binding.Modifiers)
                Modifiers.Add(modifier);
            return true;
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

        public bool ContainsBinding(InputBinding binding, PlayerIndex player)
        {
            return Bindings.Keys.Any(bindingName => Bindings[bindingName, player].Contains(binding));
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
            if (!ContainsBinding(bindingName, player)) return false;
            var bindings = Bindings[bindingName, player];

            var inputSnapshot = Device.GetDeviceSnapshot(player, state).Merge(InputSnapshot.With(Settings));

            return
                bindings.Any(
                    binding => binding.IsActive(inputSnapshot) && IsModifiersActive(binding, player, inputSnapshot));
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
            if (IsPolling) Device.Update();
        }

        #endregion

        /// <summary>
        ///   Checks if sufficient modifiers are active, as defined by the ModifierCheckType in Settings
        /// </summary>
        protected virtual bool IsModifiersActive(InputBinding inputBinding, PlayerIndex player,
                                                 InputSnapshot inputSnapshot)
        {
            if (inputSnapshot.InputSettings == null) return false;

            Func<InputBinding, PlayerIndex, InputSnapshot, bool> isActive = null;

            var checkType = inputSnapshot.InputSettings.ModifierCheckType;
            switch (checkType)
            {
                case ModifierCheckType.Strict:
                    isActive = IsStrictModifiersActive;
                    break;
                case ModifierCheckType.Smart:
                    isActive = IsSmartModifiersActive;
                    break;
            }
            return isActive != null && isActive(inputBinding, player, inputSnapshot);
        }

        /// <summary>
        ///   Checks if all (and only all) of the modifiers associated with a binding for a given player were active in the current FrameState (and not in the previous).
        /// </summary>
        protected virtual bool IsStrictModifiersActive(InputBinding inputBinding, PlayerIndex player,
                                                       InputSnapshot inputSnapshot)
        {
            return !(from trackedModifier in Modifiers
                     let modifierActive = trackedModifier.IsActive(inputSnapshot)
                     let keyTracksModifier = inputBinding.Modifiers.Contains(trackedModifier)
                     where modifierActive != keyTracksModifier
                     select modifierActive).Any();
        }

        /// <summary>
        ///   Checks if all (and only all) of the modifiers associated with a binding for a given player were active in the current FrameState (and not in the previous).
        /// </summary>
        protected virtual bool IsSmartModifiersActive(InputBinding inputBinding, PlayerIndex player,
                                                      InputSnapshot inputSnapshot)
        {
            throw new NotImplementedException("Smart modifiers are not supported yet.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Engine.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input.Managers
{
    /// <summary>
    ///   Only supports keyboard input, only supports one binding per name.
    ///   So the binding "jump" cannot be assigned to both Keys.Up and Keys.Space
    /// </summary>
    public class OptimizedKeyboardManager : InputManager
    {
        private static readonly List<ModifierKey> modifiers =
            new List<ModifierKey> {ModifierKey.Alt, ModifierKey.Ctrl, ModifierKey.Shift};

        private readonly DefaultDict<ModifierKey, BidirectionalDict<string, KeyBinding>> bindings;
        private readonly BidirectionalDict<string, KeyBinding> exactBindings;
        private readonly BidirectionalDict<string, KeyBinding> noModifiers;

        private List<ModifierKey> _cachedPressedModifiers;
        private InputSnapshot _previousSnapshot, _currentSnapshot;
        private KeyboardState _current;
        private bool _dirty = true;
        private KeyboardState _previous;

        /// <summary>
        ///   Constructor
        /// </summary>
        public OptimizedKeyboardManager()
        {
            ModifierCheckType = ModifierCheckType.Smart;
            bindings = new DefaultDict<ModifierKey, BidirectionalDict<string, KeyBinding>>();
            exactBindings = new BidirectionalDict<string, KeyBinding>();
            noModifiers = new BidirectionalDict<string, KeyBinding>();
        }

        public ModifierCheckType ModifierCheckType { get; set; }

        private List<ModifierKey> PressedModifiers
        {
            get
            {
                if (_dirty)
                {
                    UpdateCachedValues();
                }
                return _cachedPressedModifiers;
            }
        }

        #region InputManager Members

        public IEnumerable<InputBinding> GetModifiers
        {
            get { return new List<InputBinding>(modifiers); }
        }

        public bool AddBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            var rawBinding = CoerceRawInputBinding(binding, false);
            if (rawBinding == null) return false;
            var exactBinding = CoerceRawInputBinding(binding, true);

            //Make sure we don't have the binding lingering around in any of the modifiers
            if (ContainsBinding(bindingName, player)) ClearBinding(bindingName, player);

            //Strip modifiers off for storage in specific modifiers dict

            if (exactBinding.Modifiers.Count == 0)
                noModifiers.Add(exactBinding, bindingName);
            else
                foreach (var mod in exactBinding.Modifiers)
                    bindings[(ModifierKey) mod].Add(rawBinding, bindingName);

            //Copy of the binding where we only save proper modifiers
            exactBindings.Add(bindingName, CoerceRawInputBinding(binding, true));

            return true;
        }

        public void RemoveBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            ClearBinding(bindingName, player);
        }

        public bool ContainsBinding(string bindingName, PlayerIndex player)
        {
            return exactBindings.Contains(bindingName);
        }

        public bool ContainsBinding(InputBinding binding, PlayerIndex player)
        {
            var cbinding = CoerceRawInputBinding(binding, true);
            return cbinding != null && exactBindings.Contains(cbinding);
        }

        public void ClearBinding(string bindingName, PlayerIndex player)
        {
            //We can ignore the actual binding, since we have a 1:1 requirement, and just use the name.
            foreach (var mod in modifiers) bindings[mod].Remove(bindingName);
            exactBindings.Remove(bindingName);
            noModifiers.Remove(bindingName);
        }

        public void ClearAllBindings()
        {
            foreach (var mod in modifiers) bindings[mod].Clear();
            exactBindings.Clear();
            noModifiers.Clear();
        }

        public bool IsActive(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player)) return false;
            if(_dirty) UpdateCachedValues();

            var snapshot = state == FrameState.Current ? _currentSnapshot : _previousSnapshot;
            Func<string, InputSnapshot, bool> isActive = null;
            switch (ModifierCheckType)
            {
                case ModifierCheckType.Strict:
                    isActive = IsStrictActive;
                    break;
                case ModifierCheckType.Smart:
                    isActive = IsSmartActive;
                    break;
            }
            return isActive != null && isActive(bindingName, snapshot);
        }

        public List<InputBinding> GetCurrentBindings(string bindingName, PlayerIndex player)
        {
            //Max 1 binding, if any
            return ContainsBinding(bindingName, player)
                       ? new List<InputBinding> {exactBindings[bindingName]}
                       : new List<InputBinding>();
        }

        public List<string> BindingsUsing(InputBinding binding, PlayerIndex player)
        {
            var bindingsUsing = new List<string>();
            var cbinding = exactBindings[CoerceRawInputBinding(binding, true)];
            if (cbinding != null) bindingsUsing.Add(cbinding);
            return bindingsUsing;
        }

        public void Update()
        {
            _previous = _current;
            _current = Keyboard.GetState();

            // Update pressed modifiers
            UpdateCachedValues();
        }

        #endregion

        /// <summary>
        ///   Ensures only Ctrl/Alt/Shift modifiers are listed, returns null for non-Key InputBindings
        /// </summary>
        private static KeyBinding CoerceRawInputBinding(InputBinding binding, bool includeModifiers)
        {
            var keyBinding = binding as KeyBinding;
            if (keyBinding == null) return null;
            var coercedBinding = new KeyBinding(keyBinding.Key);
            if (!includeModifiers) return coercedBinding;
            foreach (var mod in modifiers.Where(mod => binding.Modifiers.Contains(mod)))
                coercedBinding.Modifiers.Add(mod);
            return coercedBinding;
        }

        private bool IsStrictActive(string bindingName, InputSnapshot snapshot)
        {
            return (modifiers.Any(mod => PressedModifiers.Contains(mod) != bindings[mod].Contains(bindingName)));
        }

        private void UpdateCachedValues()
        {
            _cachedPressedModifiers = modifiers.Where(mod => mod.IsActive(_current)).ToList();
            _currentSnapshot = InputSnapshot.With(_current);
            _previousSnapshot = InputSnapshot.With(_previous);
            _dirty = false;
        }

        private bool IsSmartActive(string bindingName, InputSnapshot snapshot)
        {
            // Quick check exact conditions met
            if (IsStrictActive(bindingName, snapshot)) return true;

            var binding = exactBindings[bindingName];
            var baseBinding = new KeyBinding(binding.Key);

            // The key can't be pressed if any of its required modifiers aren't also pressed.
            if (binding.Modifiers.Any(mod => !PressedModifiers.Contains(mod))) return false;

            // We're now at a point where at least the modifiers necessary to trigger the binding are active.
            // We want to make sure that the additional modifiers pressed couldn't be interpreted as OTHER bindings we know.
            // For example - we are looking at a binding for Shift + Space, and we want to know if Ctrl + Shift + Space could mean Shift + Space.
            // For that to be the case, we would need to know that there was no binding for Space that also used Ctrl. 
            // If there is, a binding for Space that uses Ctrl, then Ctrl + Shift + Space is ambiguous and we can't say that Shift + Space is pressed.

            // Generate a list of pressed modifiers which the binding doesn't care about.
            var significantModifiers = new List<ModifierKey>(PressedModifiers);
            foreach (var mod in binding.Modifiers)
                significantModifiers.Remove((ModifierKey) mod);

            return significantModifiers.All(mod => !bindings[mod].Contains(baseBinding));
        }

        public void LoadBindings(string filename)
        {
        }

        public void SaveBindings(string filename)
        {

        }
    }
}
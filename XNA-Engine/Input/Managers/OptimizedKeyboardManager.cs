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
        private readonly BidirectionalDict<string, KeyBinding> noModifiers;
        private List<ModifierKey> _cachedPressedModifiers;
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
            noModifiers = new BidirectionalDict<string, KeyBinding>();
        }

        public ModifierCheckType ModifierCheckType { get; set; }

        private List<ModifierKey> PressedModifiers
        {
            get
            {
                if (_dirty)
                {
                    _cachedPressedModifiers = modifiers.Where(mod => mod.IsActive(_current)).ToList();
                    _dirty = false;
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
            var kbinding = binding as KeyBinding;
            if (kbinding == null) return false;

            //Make sure we don't have the binding lingering around in any of the modifiers
            if (ContainsBinding(bindingName, player)) ClearBinding(bindingName, player);

            //Strip modifiers off for storage in specific modifiers dict
            var rawBinding = new KeyBinding(kbinding.Key);

            if (binding.Modifiers.Count == 0)
                noModifiers.Add(rawBinding, bindingName);
            else
                foreach (var mod in modifiers.Where(mod => binding.Modifiers.Contains(mod)))
                    bindings[mod].Add(rawBinding, bindingName);
            return true;
        }

        public void RemoveBinding(string bindingName, InputBinding binding, PlayerIndex player)
        {
            //We can ignore the actual binding, since we have a 1:1 requirement, and just use the name.
            noModifiers.Remove(bindingName);
            foreach (var mod in modifiers)
                bindings[mod].Remove(bindingName);
        }

        public bool ContainsBinding(string bindingName, PlayerIndex player)
        {
            return noModifiers.Contains(bindingName) ||
                   (modifiers.Any(mod => bindings[mod].Contains(bindingName)));
        }

        public bool ContainsBinding(InputBinding binding, PlayerIndex player)
        {
            var kbinding = binding as KeyBinding;
            if (kbinding == null) return false;

            //Strip modifiers off for storage in specific modifiers dict
            var rawBinding = new KeyBinding(kbinding.Key);

            return noModifiers.Contains(rawBinding) ||
                   (modifiers.Any(mod => bindings[mod].Contains(rawBinding)));
        }

        public void ClearBinding(string bindingName, PlayerIndex player)
        {
            foreach (var mod in modifiers) bindings[mod].Remove(bindingName);
            noModifiers.Remove(bindingName);
        }

        public void ClearAllBindings()
        {
            foreach (var mod in modifiers) bindings[mod].Clear();
            noModifiers.Clear();
        }

        public bool IsActive(string bindingName, PlayerIndex player, FrameState state)
        {
            if (!ContainsBinding(bindingName, player)) return false;
            var snapshot = InputSnapshot.With(state == FrameState.Current ? _current : _previous);
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
            var binding = GetExactBinding(bindingName);
            var cbindings = new List<InputBinding>();
            if (binding != null) cbindings.Add(binding);
            return cbindings;
        }

        public List<string> BindingsUsing(InputBinding binding, PlayerIndex player)
        {
            if (!ContainsBinding(binding, player)) return new List<string>();

            var rawBinding = new KeyBinding(((KeyBinding) binding).Key);
            string bindingName = null;
            if (noModifiers.Contains(rawBinding))
                bindingName = noModifiers[rawBinding];
            else
            {
                foreach (var mod in modifiers.Where(mod => bindings[mod].Contains(rawBinding)))
                    bindingName = bindings[mod][rawBinding];
            }

            return string.IsNullOrEmpty(bindingName) ? new List<string>() : new List<string> {bindingName};
        }

        public void Update()
        {
            _previous = _current;
            _current = Keyboard.GetState();

            // Update pressed modifiers
            _dirty = true;
        }

        #endregion

        private KeyBinding GetExactBinding(string bindingName)
        {
            //We can use PlayerIndex.One here because this class has no concept of more than one player.
            if (!ContainsBinding(bindingName, PlayerIndex.One)) return null;

            //Gather all the modifiers on this binding together
            var bindingModifiers = modifiers.Where(mod => bindings[mod].Contains(bindingName)).ToList();

            var rawBinding = noModifiers.Contains(bindingName)
                                 ? noModifiers[bindingName]
                                 : bindings[bindingModifiers[0]][bindingName];

            var iBindingModifiers = bindingModifiers.Cast<InputBinding>().ToArray();
            return new KeyBinding(rawBinding.Key, iBindingModifiers);
        }

        private bool IsStrictActive(string bindingName, InputSnapshot snapshot)
        {
            return (modifiers.Any(mod => PressedModifiers.Contains(mod) != bindings[mod].Contains(bindingName)));
        }

        private bool IsSmartActive(string bindingName, InputSnapshot snapshot)
        {
            // Quick check exact conditions met
            if (IsStrictActive(bindingName, snapshot)) return true;

            var binding = GetExactBinding(bindingName);
            var baseBinding = new KeyBinding(binding.Key);

            // The key can't be pressed if any of its required modifiers aren't also pressed.
            if (binding.Modifiers.Any(mod => !PressedModifiers.Contains(mod))) return false;

            // We're now at a point where at least the modifiers necessary to trigger the binding are active.
            // We want to make sure that the additional modifiers pressed couldn't be interpreted as OTHER bindings we know.
            // For example - we are looking at a binding for Shift + Space, and we want to know if Ctrl + Shift + Space could mean Shift + Space.
            // For that to be the case, we would need to know that there was no binding for Space that also used Ctrl. 
            // If there is, a binding for Space that uses Ctrl, then Ctrl + Shift + Space is ambiguous and we can't say that Shift + Space is pressed.

            var significantModifiers = new List<ModifierKey>(PressedModifiers);
            foreach (var mod in binding.Modifiers)
                significantModifiers.Remove((ModifierKey) mod);


            return significantModifiers.All(mod => !bindings[mod].Contains(baseBinding));
        }
    }
}
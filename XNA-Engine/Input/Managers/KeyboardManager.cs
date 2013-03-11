using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.DataStructures;
using Engine.FileHandlers;
using Engine.Utility;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input.Managers
{
    /// <summary>
    ///     Only supports keyboard input, only supports one binding per name.
    ///     So the binding "jump" cannot be assigned to both Keys.Up and Keys.Space
    /// </summary>
    public class KeyboardManager : InputManager
    {
        private readonly BidirectionalDict<string, KeyBinding> _bindings;
        private readonly Dictionary<string, int> _repeatingBindingHistory; 
        private InputSnapshot _current;
        private InputSnapshot _previous;
        private FullFileBuffer _bindingsFile;
        private int _frame;

        public KeyboardManager()
        {
            _frame = 0;
            _bindings = new BidirectionalDict<string, KeyBinding>();
            _previous = InputSnapshot.With(Keyboard.GetState());
            _current = InputSnapshot.With(Keyboard.GetState());
            _repeatingBindingHistory = new Dictionary<string, int>();
        }

        public bool AddBinding(string bindingName, InputBinding binding)
        {
            var keyBinding = AsKeyBinding(binding);
            if (keyBinding == null) return false;

            // Clear existing bindings
            if (ContainsBinding(bindingName)) ClearBinding(bindingName);

            _bindings.Add(bindingName, keyBinding);

            return true;
        }

        public void RemoveBinding(string bindingName, InputBinding binding)
        {
            // KeyboardManager has a 1:1 name:binding relationship, so remove and clear are identical
            ClearBinding(bindingName);
        }

        public bool ContainsBinding(string bindingName)
        {
            return _bindings.Contains(bindingName);
        }

        public bool ContainsBinding(InputBinding binding)
        {
            var keyBinding = AsKeyBinding(binding);
            return keyBinding != null && _bindings.Contains(keyBinding);
        }

        public void ClearBinding(string bindingName)
        {
            _bindings.Remove(bindingName);
        }

        public void ClearAllBindings()
        {
            _bindings.Clear();
        }

        public bool IsActive(string bindingName, FrameState state)
        {
            if (!ContainsBinding(bindingName)) return false;
            var binding = _bindings[bindingName]; 
            var snapshot = state == FrameState.Current ? _current : _previous;
            return binding.IsActive(snapshot) && ModifierKey.Values.All(modifier => binding.Modifiers.Contains(modifier) == modifier.IsActive(snapshot));
        }

        public bool IsRepeating(string bindingName, FrameState state, int minFramesToRepeat)
        {
            var offset = state == FrameState.Current ? 0 : -1;
            var isActive = IsActive(bindingName, state);
            var lastSeenFrame = _repeatingBindingHistory.GetValueOrDefault(bindingName, -1);
            
            if (lastSeenFrame < 0 && isActive)
            {
                    _repeatingBindingHistory[bindingName] = _frame + offset;
                    return true;
            }
            if (_frame + offset >= lastSeenFrame + minFramesToRepeat && isActive)
            {
                    _repeatingBindingHistory[bindingName] = _frame + offset;
                    return true;
            }
            return false;
        }

        public bool IsPressed(string bindingName)
        {
            return IsActive(bindingName, FrameState.Current) &&
                   !IsActive(bindingName, FrameState.Previous);
        }

        public bool IsReleased(string bindingName)
        {
            return IsActive(bindingName, FrameState.Previous) &&
                   !IsActive(bindingName, FrameState.Current);
        }

        public List<InputBinding> GetCurrentBindings(string bindingName)
        {
            //Max 1 binding, if any
            return ContainsBinding(bindingName)
                       ? new List<InputBinding> {_bindings[bindingName]}
                       : new List<InputBinding>();
        }

        public List<string> BindingsUsing(InputBinding binding)
        {
            var bindingsUsing = new List<string>();
            var keyBinding = AsKeyBinding(binding);
            if (keyBinding == null) return null;

            var cbinding = _bindings[AsKeyBinding(binding)];
            if (cbinding != null) bindingsUsing.Add(cbinding);
            return bindingsUsing;
        }

        public void Update()
        {
            _previous = InputSnapshot.With(_current.KeyboardState);
            _current = InputSnapshot.With(Keyboard.GetState());
            _frame++;
        }

        /// <summary>
        ///     Ensures only Ctrl/Alt/Shift modifiers are listed, returns null for non-Key InputBindings
        /// </summary>
        private static KeyBinding AsKeyBinding(InputBinding binding)
        {
            var keyBinding = binding as KeyBinding;
            if (keyBinding == null) return null;
            var coercedBinding = new KeyBinding(keyBinding.Key);
            foreach (var mod in ModifierKey.Values.Where(mod => binding.Modifiers.Contains(mod)))
                coercedBinding.Modifiers.Add(mod);
            return coercedBinding;
        }

        public void LoadBindings(string filename)
        {
            ClearAllBindings();
            foreach (var line in new StreamReader(filename).ReadLines())
            {
                string bindingName;
                var key = DeserializeBinding(line, out bindingName);
                if (key != null) AddBinding(bindingName, key);
            }
            _bindingsFile = new FullFileBuffer(filename);
        }

        public void SaveBindings(string filename)
        {
            if (_bindingsFile == null)
                _bindingsFile = new FullFileBuffer(filename);
            else
                _bindingsFile.Filename = filename;

            const string pattern = "^{0}";
            foreach (var bindingName in _bindings.GetValuesType1())
            {
                var binding = SerializeBiding(bindingName);

                //Replace existing line instead of duplicating declaration
                var lineno = _bindingsFile.LineMatching(pattern.format(bindingName));
                if (lineno < 0)
                    _bindingsFile.Append(binding);
                else
                    _bindingsFile.WriteLine(binding, lineno);
            }
            _bindingsFile.SaveToFile();
        }

        private string SerializeBiding(string bindingName)
        {
            if (!ContainsBinding(bindingName)) return null;
            var sb = new StringBuilder();
            sb.Append(bindingName);
            var binding = _bindings[bindingName];
            sb.Append(" {0}".format(binding.Key));
            foreach (var mod in binding.Modifiers) sb.Append(" {0}".format(mod));
            return sb.ToString();
        }

        private static KeyBinding DeserializeBinding(string keybindString, out string bindingName)
        {
            bindingName = null;
            keybindString = keybindString.Until('#');
            if (String.IsNullOrEmpty(keybindString)) return null;
            var pieces = keybindString.Split(' ');
            if (pieces.Length < 2) return null;

            bindingName = pieces[0];
            Keys key;
            var parsed = Enum.TryParse(pieces[1], out key);
            if (!parsed) return null;

            var binding = new KeyBinding(key);
            if (pieces.Length == 2) return binding;

            for (var i = 2; i < pieces.Length; i++)
            {
                ModifierKey mkey = null;
                switch (pieces[i])
                {
                    case "Ctrl":
                        mkey = ModifierKey.Ctrl;
                        break;
                    case "Shift":
                        mkey = ModifierKey.Shift;
                        break;
                    case "Alt":
                        mkey = ModifierKey.Alt;
                        break;
                }
                if (mkey != null) binding.Modifiers.Add(mkey);
            }
            return binding;
        }
    }
}
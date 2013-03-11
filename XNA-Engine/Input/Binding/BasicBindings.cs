#region Using Statements

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Engine.Input
{
    /// <summary>
    ///     See <see cref="InputBinding" />
    /// </summary>
    public abstract class DefaultBinding : InputBinding
    {
        /// <summary>
        ///     Initialize an InputBinding with an optional list of required modifiers
        /// </summary>
        /// <param name="modifiers"> Optional modifiers- Ctrl, Alt, Shift </param>
        protected DefaultBinding(params InputBinding[] modifiers)
        {
            Modifiers = new List<InputBinding>(modifiers);
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        /// <param name="other"> </param>
        public DefaultBinding(DefaultBinding other) : this(other.Modifiers.ToArray())
        {
        }

        /// <summary>
        ///     Any modifiers required for this binding to be considered 'active'
        /// </summary>
        public List<InputBinding> Modifiers { get; set; }

        /// <summary>
        ///     Compares this Binding to another, and returns whether they are the same (with/without modifiers)
        /// </summary>
        public virtual bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            return !includeModifiers || Modifiers.SequenceEqual(other.Modifiers);
        }

        /// <summary>
        ///     See <see cref="InputBinding.IsActive" />
        /// </summary>
        public abstract bool IsActive(InputSnapshot inputSnapshot);
    }

    public class ThumbstickDirectionBinding : DefaultBinding
    {
        public ThumbstickDirectionBinding(ThumbstickDirection thumbstickDirection, Thumbstick thumbstick,
                                          params InputBinding[] modifiers)
            : base(modifiers)
        {
            Thumbstick = thumbstick;
            Direction = thumbstickDirection;
        }

        public Thumbstick Thumbstick { get; protected set; }

        public ThumbstickDirection Direction { get; protected set; }

        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var tdb = other as ThumbstickDirectionBinding;
            if (tdb == null) return false;
            return tdb.Thumbstick == Thumbstick &&
                   tdb.Direction == Direction &&
                   base.IsEqual(other, includeModifiers);
        }

        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            if (!inputSnapshot.GamePadState.HasValue)
                return false;

            var gamePadState = inputSnapshot.GamePadState.Value;
            var settings = inputSnapshot.InputSettings;

            Vector2 gamepadThumbstick = Thumbstick == Thumbstick.Left
                                            ? gamePadState.ThumbSticks.Left
                                            : gamePadState.ThumbSticks.Right;
            bool isActive = false;
            switch (Direction)
            {
                case ThumbstickDirection.Up:
                    isActive = (gamepadThumbstick.Y >= settings.ThumbstickThreshold);
                    break;
                case ThumbstickDirection.Down:
                    isActive = (gamepadThumbstick.Y <= -settings.ThumbstickThreshold);
                    break;
                case ThumbstickDirection.Left:
                    isActive = (gamepadThumbstick.X <= -settings.ThumbstickThreshold);
                    break;
                case ThumbstickDirection.Right:
                    isActive = (gamepadThumbstick.X >= settings.ThumbstickThreshold);
                    break;
                default:
                    break;
            }
            return isActive;
        }
    }

    public class MouseBinding : DefaultBinding
    {
        public MouseBinding(MouseButton mouseButton, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Button = mouseButton;
        }

        public MouseButton Button { get; protected set; }

        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var mb = other as MouseBinding;
            if (mb == null) return false;
            return mb.Button == Button && base.IsEqual(other, includeModifiers);
        }

        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            if (!inputSnapshot.MouseState.HasValue)
                return false;
            var mouseState = inputSnapshot.MouseState.Value;
            ButtonState buttonState;
            switch (Button)
            {
                case MouseButton.Left:
                    buttonState = mouseState.LeftButton;
                    break;
                case MouseButton.Right:
                    buttonState = mouseState.RightButton;
                    break;
                case MouseButton.Middle:
                    buttonState = mouseState.MiddleButton;
                    break;
                default:
                    buttonState = ButtonState.Released;
                    break;
            }
            return buttonState == ButtonState.Pressed;
        }
    }

    public class ThumbstickBinding : DefaultBinding
    {
        public ThumbstickBinding(Thumbstick thumbstick, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Thumbstick = thumbstick;
        }

        public Thumbstick Thumbstick { get; protected set; }

        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var tb = other as ThumbstickBinding;
            if (tb == null) return false;
            return tb.Thumbstick == Thumbstick && base.IsEqual(other, includeModifiers);
        }

        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            if (!inputSnapshot.GamePadState.HasValue)
                return false;
            var gamePadState = inputSnapshot.GamePadState.Value;
            Vector2 gamepadThumbstickMag = Thumbstick == Thumbstick.Left
                                               ? gamePadState.ThumbSticks.Left
                                               : gamePadState.ThumbSticks.Right;
            return gamepadThumbstickMag.Length() >= inputSnapshot.InputSettings.ThumbstickThreshold;
        }
    }

    public class ButtonBinding : DefaultBinding
    {
        public ButtonBinding(Buttons button, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Button = button;
        }

        public Buttons Button { get; protected set; }

        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var bb = other as ButtonBinding;
            if (bb == null) return false;
            return bb.Button == Button && base.IsEqual(other, includeModifiers);
        }

        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            if (!inputSnapshot.GamePadState.HasValue)
                return false;
            return inputSnapshot.GamePadState.Value.IsButtonDown(Button);
        }
    }

    public class KeyBinding : DefaultBinding
    {
        public KeyBinding(Keys key, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Key = key;
        }

        public Keys Key { get; protected set; }

        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var kb = other as KeyBinding;
            if (kb == null) return false;
            return kb.Key == Key && base.IsEqual(other, includeModifiers);
        }

        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            return inputSnapshot.KeyboardState.HasValue && inputSnapshot.KeyboardState.Value.IsKeyDown(Key);
        }

        public override string ToString()
        {
            return Key.ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as KeyBinding;
            return other != null && other.Key == Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    /// <summary>
    ///     Used to capture both left and right modifiers for special keys
    /// </summary>
    public class ModifierKey : KeyBinding
    {
        private static ModifierKey _ctrl;
        private static ModifierKey _alt;
        private static ModifierKey _shift;
        private readonly Keys _key1;
        private readonly Keys _key2;
        private readonly string _name;

        public static readonly List<ModifierKey> Values =
            new List<ModifierKey> { Alt, Ctrl, Shift };

        private ModifierKey(Keys key1, Keys key2, string name)
            : base(Keys.None)
        {
            this._key1 = key1;
            this._key2 = key2;
            this._name = name;
        }

        public static ModifierKey Ctrl
        {
            get { return _ctrl ?? (_ctrl = new ModifierKey(Keys.LeftControl, Keys.RightControl, "Ctrl")); }
        }

        public static ModifierKey Alt
        {
            get { return _alt ?? (_alt = new ModifierKey(Keys.LeftAlt, Keys.RightAlt, "Alt")); }
        }

        public static ModifierKey Shift
        {
            get { return _shift ?? (_shift = new ModifierKey(Keys.LeftShift, Keys.RightShift, "Shift")); }
        }

        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            return inputSnapshot.KeyboardState.HasValue && IsActive(inputSnapshot.KeyboardState.Value);
        }

        public bool IsActive(KeyboardState keyboardState)
        {
            return keyboardState.IsKeyDown(_key1) ||
                   keyboardState.IsKeyDown(_key2);
        }

        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var kb = other as KeyBinding;
            if (kb == null) return false;
            if (includeModifiers && other.Modifiers.Count > 0) return false;
            return (kb.Key == _key1 || kb.Key == _key2);
        }

        public override string ToString()
        {
            return _name;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ModifierKey;
            return other != null && other._key1 == _key1 && other._key2 == _key2;
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }

    public class TriggerBinding : DefaultBinding
    {
        public TriggerBinding(Trigger trigger, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Trigger = trigger;
        }

        public Trigger Trigger { get; protected set; }

        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var tb = other as TriggerBinding;
            if (tb == null) return false;
            return tb.Trigger == Trigger && base.IsEqual(other, includeModifiers);
        }

        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            if (!inputSnapshot.GamePadState.HasValue)
                return false;
            var gamePadState = inputSnapshot.GamePadState.Value;
            var triggerMag = Trigger == Trigger.Left ? gamePadState.Triggers.Left : gamePadState.Triggers.Right;
            return triggerMag >= inputSnapshot.InputSettings.TriggerThreshold;
        }
    }
}
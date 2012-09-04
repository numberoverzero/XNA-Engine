#region Using Statements

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Engine.Input
{
    /// <summary>
    ///   See <see cref="InputBinding" />
    /// </summary>
    public abstract class DefaultBinding : InputBinding
    {
        #region InputBinding Members

        /// <summary>
        ///   Any modifiers required for this binding to be considered 'active'
        /// </summary>
        public List<InputBinding> Modifiers { get; set; }

        /// <summary>
        ///   Compares this Binding to another, and returns whether they are the same (with/without modifiers)
        /// </summary>
        public virtual bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            return !includeModifiers || Modifiers.SequenceEqual(other.Modifiers);
        }

        /// <summary>
        ///   See <see cref="InputBinding.IsActive" />
        /// </summary>
        public abstract bool IsActive(InputSnapshot inputSnapshot);

        #endregion

        #region Initialiation

        /// <summary>
        ///   Initialize an InputBinding with an optional list of required modifiers
        /// </summary>
        /// <param name="modifiers"> Optional modifiers- Ctrl, Alt, Shift </param>
        public DefaultBinding(params InputBinding[] modifiers)
        {
            Modifiers = new List<InputBinding>(modifiers);
        }

        /// <summary>
        ///   Copy Constructor
        /// </summary>
        /// <param name="other"> </param>
        public DefaultBinding(DefaultBinding other) : this(other.Modifiers.ToArray())
        {
        }

        #endregion
    }

    /// <summary>
    ///   An InputBinding that checks if a certain ThumbstickDirection is active
    ///   (beyond some threshold as defined in an InputManager's InputSettings)
    /// </summary>
    public class ThumbstickDirectionBinding : DefaultBinding
    {
        /// <summary>
        ///   An InputBinding wrapper for a ThumbstickDirection
        /// </summary>
        /// <param name="thumbstickDirection"> </param>
        /// <param name="thumbstick"> </param>
        /// <param name="modifiers"> </param>
        public ThumbstickDirectionBinding(ThumbstickDirection thumbstickDirection, Thumbstick thumbstick,
                                          params InputBinding[] modifiers)
            : base(modifiers)
        {
            Thumbstick = thumbstick;
            Direction = thumbstickDirection;
        }

        /// <summary>
        ///   The Thumbstick (left or right) which is checked for activity past a threshold
        /// </summary>
        public Thumbstick Thumbstick { get; protected set; }

        /// <summary>
        ///   The Thumbstick Direction (up/down/left/right) which is checked for activity past a threshold
        /// </summary>
        public ThumbstickDirection Direction { get; protected set; }

        /// <summary>
        ///   Compares this Binding to another, and returns whether they are the same (with/without modifiers)
        /// </summary>
        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var tdb = other as ThumbstickDirectionBinding;
            if (tdb == null) return false;
            return tdb.Thumbstick == Thumbstick &&
                   tdb.Direction == Direction &&
                   base.IsEqual(other, includeModifiers);
        }

        /// <summary>
        ///   True if the  ThumbstickDirection of the specified Thumbstick is past the settings threshold
        /// </summary>
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

    /// <summary>
    ///   An InputBinding wrapper for a Thumbstick
    /// </summary>
    public class MouseBinding : DefaultBinding
    {
        /// <summary>
        ///   An InputBinding wrapper for a MouseButton
        /// </summary>
        /// <param name="mouseButton"> </param>
        /// <param name="modifiers"> </param>
        public MouseBinding(MouseButton mouseButton, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Button = mouseButton;
        }

        /// <summary>
        ///   The mouse button which is checked for activity
        /// </summary>
        public MouseButton Button { get; protected set; }

        /// <summary>
        ///   Compares this Binding to another, and returns whether they are the same (with/without modifiers)
        /// </summary>
        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var mb = other as MouseBinding;
            if (mb == null) return false;
            return mb.Button == Button && base.IsEqual(other, includeModifiers);
        }

        /// <summary>
        ///   True if the Mouse button (without modifiers) is pressed
        /// </summary>
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

    /// <summary>
    ///   An InputBinding that checks if a certain Thumbstick is active
    ///   (beyond some threshold as defined in an InputManager's InputSettings)
    ///   (direction doesn't matter)
    /// </summary>
    public class ThumbstickBinding : DefaultBinding
    {
        /// <summary>
        ///   An InputBinding wrapper for a Thumbstick
        /// </summary>
        /// <param name="thumbstick"> </param>
        /// <param name="modifiers"> </param>
        public ThumbstickBinding(Thumbstick thumbstick, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Thumbstick = thumbstick;
        }

        /// <summary>
        ///   The Thumbstick (left or right) which is checked for activity past a threshold
        /// </summary>
        public Thumbstick Thumbstick { get; protected set; }

        /// <summary>
        ///   Compares this Binding to another, and returns whether they are the same (with/without modifiers)
        /// </summary>
        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var tb = other as ThumbstickBinding;
            if (tb == null) return false;
            return tb.Thumbstick == Thumbstick && base.IsEqual(other, includeModifiers);
        }

        /// <summary>
        ///   True if the thumbstick (without modifiers) is past the settings threshold
        /// </summary>
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

    /// <summary>
    ///   An InputBinding wrapper for a Button
    /// </summary>
    public class ButtonBinding : DefaultBinding
    {
        /// <summary>
        ///   An InputBinding wrapper for a Button
        /// </summary>
        /// <param name="button"> </param>
        /// <param name="modifiers"> </param>
        public ButtonBinding(Buttons button, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Button = button;
        }

        /// <summary>
        ///   The Button which is checked for activity
        /// </summary>
        public Buttons Button { get; protected set; }

        /// <summary>
        ///   Compares this Binding to another, and returns whether they are the same (with/without modifiers)
        /// </summary>
        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var bb = other as ButtonBinding;
            if (bb == null) return false;
            return bb.Button == Button && base.IsEqual(other, includeModifiers);
        }

        /// <summary>
        ///   True if the Button (without modifiers) is pressed
        /// </summary>
        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            if (!inputSnapshot.GamePadState.HasValue)
                return false;
            return inputSnapshot.GamePadState.Value.IsButtonDown(Button);
        }
    }

    /// <summary>
    ///   An InputBinding wrapper for a keyboard Key
    /// </summary>
    public class KeyBinding : DefaultBinding
    {
        /// <summary>
        ///   InputBinding wrapper for a keyboard Key
        /// </summary>
        /// <param name="key"> </param>
        /// <param name="modifiers"> </param>
        public KeyBinding(Keys key, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Key = key;
        }

        /// <summary>
        ///   The Key which is checked for activity
        /// </summary>
        public Keys Key { get; protected set; }

        /// <summary>
        ///   Compares this Binding to another, and returns whether they are the same (with/without modifiers)
        /// </summary>
        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var kb = other as KeyBinding;
            if (kb == null) return false;
            return kb.Key == Key && base.IsEqual(other, includeModifiers);
        }

        /// <summary>
        ///   True if the key (without modifiers) is pressed
        /// </summary>
        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            if (!inputSnapshot.KeyboardState.HasValue)
                return false;
            return inputSnapshot.KeyboardState.Value.IsKeyDown(Key);
        }

        public override string ToString()
        {
            return Key.ToString();
        }
    }

    /// <summary>
    ///   Custom KeyBinding for double-keyed modifiers
    /// </summary>
    public class ModifierKey : KeyBinding
    {
        private static ModifierKey _Ctrl;
        private static ModifierKey _Alt;
        private static ModifierKey _Shift;
        private readonly Keys key1;
        private readonly Keys key2;
        private readonly string name;

        private ModifierKey(Keys key1, Keys key2, string name)
            : base(Keys.None)
        {
            this.key1 = key1;
            this.key2 = key2;
            this.name = name;
        }

        /// <summary>
        ///   Control key
        /// </summary>
        public static ModifierKey Ctrl
        {
            get { return _Ctrl ?? (_Ctrl = new ModifierKey(Keys.LeftControl, Keys.RightControl, "Ctrl")); }
        }

        /// <summary>
        ///   Alt key
        /// </summary>
        public static ModifierKey Alt
        {
            get { return _Alt ?? (_Alt = new ModifierKey(Keys.LeftAlt, Keys.RightAlt, "Alt")); }
        }

        /// <summary>
        ///   Shift key
        /// </summary>
        public static ModifierKey Shift
        {
            get { return _Shift ?? (_Shift = new ModifierKey(Keys.LeftShift, Keys.RightShift, "Shift")); }
        }

        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            return inputSnapshot.KeyboardState.HasValue && IsActive(inputSnapshot.KeyboardState.Value);
        }

        public bool IsActive(KeyboardState keyboardState)
        {
            return keyboardState.IsKeyDown(key1) ||
                   keyboardState.IsKeyDown(key2);
        }

        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var kb = other as KeyBinding;
            if (kb == null) return false;
            if (includeModifiers && other.Modifiers.Count > 0) return false;
            return (kb.Key == key1 || kb.Key == key2);
        }

        public override string ToString()
        {
            return name;
        }
    }

    /// <summary>
    ///   An InputBinding that checks if a certain Trigger is active
    ///   (beyond some threshold as defined in an InputManager's InputSettings)
    /// </summary>
    public class TriggerBinding : DefaultBinding
    {
        /// <summary>
        ///   InputBinding wrapper for GamePad triggers
        /// </summary>
        /// <param name="trigger"> </param>
        /// <param name="modifiers"> </param>
        public TriggerBinding(Trigger trigger, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Trigger = trigger;
        }

        /// <summary>
        ///   The left or right trigger
        /// </summary>
        public Trigger Trigger { get; protected set; }

        /// <summary>
        ///   Compares this Binding to another, and returns whether they are the same (with/without modifiers)
        /// </summary>
        public override bool IsEqual(InputBinding other, bool includeModifiers = false)
        {
            var tb = other as TriggerBinding;
            if (tb == null) return false;
            return tb.Trigger == Trigger && base.IsEqual(other, includeModifiers);
        }

        /// <summary>
        ///   True if the trigger (without modifiers) is past the settings threshold
        /// </summary>
        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            if (!inputSnapshot.GamePadState.HasValue)
                return false;
            var gamePadState = inputSnapshot.GamePadState.Value;
            float triggerMag = Trigger == Trigger.Left ? gamePadState.Triggers.Left : gamePadState.Triggers.Right;
            return triggerMag >= inputSnapshot.InputSettings.TriggerThreshold;
        }
    }
}
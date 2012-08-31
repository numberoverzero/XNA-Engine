#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Engine.Input
{
    /// <summary>
    /// See <see cref="InputBinding"/>
    /// </summary>
    public abstract class DefaultInputBinding : InputBinding
    {
        /// <summary>
        /// Any modifiers required for this binding to be considered 'active'
        /// </summary>
        public InputBinding[] Modifiers { get; private set; }

        #region Initialiation

        /// <summary>
        /// Initialize an InputBinding with an optional list of required modifiers
        /// </summary>
        /// <param name="modifiers">Optional modifiers- Ctrl, Alt, Shift</param>
        /// 
        public DefaultInputBinding(params InputBinding[] modifiers)
        {
            Modifiers = new InputBinding[modifiers.Length];
            Array.Copy(modifiers, Modifiers, modifiers.Length);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public DefaultInputBinding(DefaultInputBinding other) : this(other.Modifiers) { }

        #endregion

        /// <summary>
        /// See <see cref="InputBinding.IsActive"/>
        /// </summary>
        public abstract bool IsActive(InputSnapshot inputSnapshot);
    }

    /// <summary>
    /// An InputBinding that checks if a certain ThumbstickDirection is active
    /// (beyond some threshold as defined in an InputManager's InputSettings)
    /// </summary>
    public class ThumbstickDirectionInputBinding : DefaultInputBinding
    {
        /// <summary>
        /// The Thumbstick (left or right) which is checked for activity past a threshold
        /// </summary>
        public Thumbstick Thumbstick { get; protected set; }
        
        /// <summary>
        /// The Thumbstick Direction (up/down/left/right) which is checked for activity past a threshold
        /// </summary>
        public ThumbstickDirection Direction { get; protected set; }

        /// <summary>
        /// An InputBinding wrapper for a ThumbstickDirection
        /// </summary>
        /// <param name="thumbstickDirection"></param>
        /// <param name="thumbstick"></param>
        /// <param name="modifiers"></param>
        public ThumbstickDirectionInputBinding(ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params InputBinding[] modifiers)
            : base(modifiers)
        {
            this.Thumbstick = thumbstick;
            Direction = thumbstickDirection;
        }

        /// <summary>
        /// True if the  ThumbstickDirection of the specified Thumbstick is past the settings threshold
        /// </summary>
        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            var settings = inputSnapshot.InputSettings;
            var gamePadState = inputSnapshot.GamePadState;
            Vector2 gamepadThumbstick = Thumbstick == Thumbstick.Left ? gamePadState.ThumbSticks.Left : gamePadState.ThumbSticks.Right;
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
    /// An InputBinding wrapper for a Thumbstick
    /// </summary>
    public class MouseInputBinding : DefaultInputBinding
    {
        /// <summary>
        /// The mouse button which is checked for activity
        /// </summary>
        public MouseButton Button { get; protected set; }

        /// <summary>
        /// An InputBinding wrapper for a MouseButton
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <param name="modifiers"></param>
        public MouseInputBinding(MouseButton mouseButton, params InputBinding[] modifiers)
            : base(modifiers)
        {
            Button = mouseButton;
        }

        /// <summary>
        /// True if the Mouse button (without modifiers) is pressed
        /// </summary>
        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            var mouseState = inputSnapshot.MouseState;
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
    /// An InputBinding that checks if a certain Thumbstick is active
    /// (beyond some threshold as defined in an InputManager's InputSettings)
    /// (direction doesn't matter)
    /// </summary>
    public class ThumbstickInputBinding : DefaultInputBinding
    {
        /// <summary>
        /// The Thumbstick (left or right) which is checked for activity past a threshold
        /// </summary>
        public Thumbstick Thumbstick { get; protected set; }

        /// <summary>
        /// An InputBinding wrapper for a Thumbstick
        /// </summary>
        /// <param name="thumbstick"></param>
        /// <param name="modifiers"></param>
        public ThumbstickInputBinding(Thumbstick thumbstick, params InputBinding[] modifiers)
            : base(modifiers)
        {
            this.Thumbstick = thumbstick;
        }

        /// <summary>
        /// True if the thumbstick (without modifiers) is past the settings threshold
        /// </summary>
        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            var gamePadState = inputSnapshot.GamePadState;
            Vector2 gamepadThumbstickMag = Thumbstick == Thumbstick.Left ? gamePadState.ThumbSticks.Left : gamePadState.ThumbSticks.Right;
            return gamepadThumbstickMag.Length() >= inputSnapshot.InputSettings.ThumbstickThreshold;
        }
    }

    /// <summary>
    /// An InputBinding wrapper for a Button
    /// </summary>
    public class ButtonInputBinding : DefaultInputBinding
    {
        /// <summary>
        /// The Button which is checked for activity
        /// </summary>
        public Buttons Button { get; protected set; }

        /// <summary>
        /// An InputBinding wrapper for a Button
        /// </summary>
        /// <param name="button"></param>
        /// <param name="modifiers"></param>
        public ButtonInputBinding(Buttons button, params InputBinding[] modifiers)
            : base(modifiers)
        {
            this.Button = button;
        }

        /// <summary>
        /// True if the Button (without modifiers) is pressed
        /// </summary>
        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            return inputSnapshot.GamePadState.IsButtonDown(Button);
        }
    }

    /// <summary>
    /// An InputBinding wrapper for a keyboard Key
    /// </summary>
    public class KeyInputBinding : DefaultInputBinding
    {
        /// <summary>
        /// The Key which is checked for activity
        /// </summary>
        public Keys Key { get; protected set; }
        
        /// <summary>
        /// InputBinding wrapper for a keyboard Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        public KeyInputBinding(Keys key, params InputBinding[] modifiers)
            : base(modifiers)
        {
            this.Key = key;
        }

        /// <summary>
        /// True if the key (without modifiers) is pressed
        /// </summary>
        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            return inputSnapshot.KeyboardState.IsKeyDown(Key);
        }
    }

    /// <summary>
    /// An InputBinding that checks if a certain Trigger is active
    /// (beyond some threshold as defined in an InputManager's InputSettings)
    /// </summary>
    public class TriggerInputBinding : DefaultInputBinding
    {
        /// <summary>
        /// The left or right trigger
        /// </summary>
        public Trigger Trigger { get; protected set; }

        /// <summary>
        /// InputBinding wrapper for GamePad triggers
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="modifiers"></param>
        public TriggerInputBinding(Trigger trigger, params InputBinding[] modifiers)
            : base(modifiers)
        {
            this.Trigger = trigger;
        }

        /// <summary>
        /// True if the trigger (without modifiers) is past the settings threshold
        /// </summary>
        public override bool IsActive(InputSnapshot inputSnapshot)
        {
            var gamePadState = inputSnapshot.GamePadState;
            float triggerMag = Trigger == Trigger.Left ? gamePadState.Triggers.Left : gamePadState.Triggers.Right;
            return triggerMag >= inputSnapshot.InputSettings.TriggerThreshold;
        }
    }
}

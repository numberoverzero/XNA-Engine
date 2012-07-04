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
    public class ThumbstickDirectionInputBinding : InputBinding
    {
        public Thumbstick Thumbstick { get; protected set; }
        public ThumbstickDirection Direction { get; protected set; }

        public ThumbstickDirectionInputBinding(ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params IBinding[] modifiers)
            : base(modifiers)
        {
            this.Thumbstick = thumbstick;
            Direction = thumbstickDirection;
        }

        protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            Vector2 gamepadThumbstick = Thumbstick == Thumbstick.Left ? gamepadState.ThumbSticks.Left : gamepadState.ThumbSticks.Right;
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

    public class MouseInputBinding : InputBinding
    {
        public MouseButton Button { get; protected set; }

        public MouseInputBinding(MouseButton mouseButton, params IBinding[] modifiers)
            : base(modifiers)
        {
            Button = mouseButton;
        }

        protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
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

    public class ThumbstickInputBinding : InputBinding
    {
        public Thumbstick Thumbstick { get; protected set; }
        public ThumbstickInputBinding(Thumbstick thumbstick, params IBinding[] modifiers)
            : base(modifiers)
        {
            this.Thumbstick = thumbstick;
        }

        protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            Vector2 gamepadThumbstickMag = Thumbstick == Thumbstick.Left ? gamepadState.ThumbSticks.Left : gamepadState.ThumbSticks.Right;
            return gamepadThumbstickMag.Length() >= settings.ThumbstickThreshold;
        }
    }

    public class ButtonInputBinding : InputBinding
    {
        public Buttons Button { get; protected set; }
        public ButtonInputBinding(Buttons button, params IBinding[] modifiers)
            : base(modifiers)
        {
            this.Button = button;
        }

        protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            return gamepadState.IsButtonDown(Button);
        }
    }

    public class KeyInputBinding : InputBinding
    {
        public Keys Key { get; protected set; }
        public KeyInputBinding(Keys key, params IBinding[] modifiers)
            : base(modifiers)
        {
            this.Key = key;
        }

        protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            return keyState.IsKeyDown(Key);
        }
    }

    public class TriggerInputBinding : InputBinding
    {
        public Trigger Trigger { get; protected set; }
        public TriggerInputBinding(Trigger trigger, params IBinding[] modifiers)
            : base(modifiers)
        {
            this.Trigger = trigger;
        }

        protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            float triggerMag = Trigger == Trigger.Left ? gamepadState.Triggers.Left : gamepadState.Triggers.Right;
            return triggerMag >= settings.TriggerThreshold;
        }
    }
}

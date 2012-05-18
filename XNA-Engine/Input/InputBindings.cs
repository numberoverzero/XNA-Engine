using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
    
    public class InputBinding : IBinding
    {
        /// <summary>
        /// Any Modifiers required for this binding to be considered 'active'
        /// </summary>
        public Modifier[] Modifiers {get;protected set;}

        #region Initialiation

        /// <summary>
        /// Initialize an InputBinding with an optional list of required modifiers
        /// </summary>
        /// <param name="modifiers">Optional modifiers- Ctrl, Alt, Shift</param>
        /// 
        public InputBinding(params Modifier[] modifiers)
        {
            Modifiers = new Modifier[modifiers.Length];
            Array.Copy(modifiers, Modifiers, modifiers.Length);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public InputBinding(InputBinding other) :this(other.Modifiers) { }

        #endregion

        #region IsActive Methods

        /// <summary>
        /// True if the InputBinding is active in the given FrameState of the given InputManager
        /// </summary>
        /// <param name="state">Current or Previous frame</param>
        /// <param name="manager">The manager keeping track of current/previous input states</param>
        /// <remarks>
        /// At first I wasn't comfortable with passing the entire InputManager around, but on the plus side,
        /// we can now easily mock up input.  Woohoo!
        /// </remarks>
        /// <returns></returns>
        public bool IsActive(InputManager manager, FrameState state)
        {
            KeyboardState keyState = state == FrameState.Current ? manager.CurrentKeyboardState : manager.PreviousKeyboardState;
            GamePadState gamepadState = state == FrameState.Current ? manager.CurrentGamePadState : manager.PreviousGamePadState;
            MouseState mouseState = state == FrameState.Current ? manager.CurrentMouseState : manager.PreviousMouseState;
            return IsRawBindingActive(keyState, gamepadState, mouseState, manager.Settings) && AreExactModifiersActive(keyState);
        }

        /// <summary>
        /// True if the binding (without modifiers) is active
        /// </summary>
        /// <param name="keyState"></param>
        /// <param name="gamepadState"></param>
        /// <param name="mouseState"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public virtual bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            return false;
        }

        /// <summary>
        /// True if every modifier in the binding's Modifiers is active, and only those modifiers are
        /// </summary>
        /// <example>
        /// If we have modifiers Ctrl, Shift and only Shift is active, we return false.
        /// </example>
        /// <example>
        /// If we have modifiers Ctrl, Shift and Ctrl, Shift and Alt are active, we return false.
        /// </example>
        /// <param name="keyState"></param>
        /// <returns></returns>
        public bool AreExactModifiersActive(KeyboardState keyState)
        {
            return (Modifier.Alt.IsActive(keyState) == Modifiers.Contains(Modifier.Alt) &&
                    Modifier.Ctrl.IsActive(keyState) == Modifiers.Contains(Modifier.Ctrl) &&
                    Modifier.Shift.IsActive(keyState) == Modifiers.Contains(Modifier.Shift));
        }

        #endregion
    }

    public class ThumbstickDirectionInputBinding : InputBinding
    {
        public Thumbstick Thumbstick { get; protected set; }
        public ThumbstickDirection Direction { get; protected set; }

        public ThumbstickDirectionInputBinding(ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params Modifier[] modifiers)
            : base(modifiers)
        {
            this.Thumbstick = thumbstick;
            Direction = thumbstickDirection;
        }

        public override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
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

        public MouseInputBinding(MouseButton mouseButton, params Modifier[] modifiers)
            : base(modifiers)
        {
            Button = mouseButton;
        }

        public override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
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
        public ThumbstickInputBinding(Thumbstick thumbstick, params Modifier[] modifiers)
            : base(modifiers)
        {
            this.Thumbstick = thumbstick;
        }

        public override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            Vector2 gamepadThumbstickMag = Thumbstick == Thumbstick.Left ? gamepadState.ThumbSticks.Left : gamepadState.ThumbSticks.Right;
            return gamepadThumbstickMag.Length() >= settings.ThumbstickThreshold;
        }
    }

    public class ButtonInputBinding : InputBinding
    {
        public Buttons Button { get; protected set; }
        public ButtonInputBinding(Buttons button, params Modifier[] modifiers)
            : base(modifiers)
        {
            this.Button = button;
        }

        public override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            return gamepadState.IsButtonDown(Button);
        }
    }

    public class KeyInputBinding : InputBinding
    {
        public Keys Key { get; protected set; }
        public KeyInputBinding(Keys key, params Modifier[] modifiers)
            : base(modifiers)
        {
            this.Key = key;
        }

        public override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            return keyState.IsKeyDown(Key);
        }
    }

    public class TriggerInputBinding : InputBinding
    {
        public Trigger Trigger { get; protected set; }
        public TriggerInputBinding(Trigger trigger, params Modifier[] modifiers)
            : base(modifiers)
        {
            this.Trigger = trigger;
        }

        public override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            float triggerMag = Trigger == Trigger.Left ? gamepadState.Triggers.Left : gamepadState.Triggers.Right;
            return triggerMag >= settings.TriggerThreshold;
        }
    }
}

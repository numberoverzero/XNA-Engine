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
            KeyboardState keyState = state == FrameState.Current ? manager.CurrentKeyboardState : manager.LastKeyboardState;
            GamePadState gamepadState = state == FrameState.Current ? manager.CurrentGamePadState : manager.LastGamePadState;
            MouseState mouseState = state == FrameState.Current ? manager.CurrentMouseState : manager.LastMouseState;
            return IsRawBindingActive(keyState, gamepadState, mouseState, manager.Settings) && AreExactModifiersActive(keyState);

        }

        /// <summary>
        /// True if the binding (without modifiers) is active.  In general, one should check if a binding is active through IsActive.
        /// Subclasses of InputBinding should override IsRawBindingActive, which is called from IsActive.
        /// </summary>
        /// <param name="keyState">KeybardState to check binding against</param>
        /// <param name="gamepadState">GamePadState to check binding against</param>
        /// <param name="mouseState">MouseState to check binding against</param>
        /// <param name="settings">Settings to use when checking thresholds, etc</param>
        /// <returns></returns>
        public virtual bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
        {
            return false;
        }

        /// <summary>
        /// Checks if the modifiers that this binding requires 
        /// match the active state of the modifiers in the given KeyboardState
        /// </summary>
        /// <remarks>
        /// I suspect it should be possible to streamline this in someway while still keeping the system easily extendable.
        /// We're O(n) and we can reasonably assume n is smaller than 10, but even still, it looks ugly.
        /// </remarks>
        /// <param name="keyState">The KeyboardState to check modifiers against</param>
        /// <returns>True if only all required modifiers are active</returns>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
    public partial class InputManager
    {
        private class InputBinding
        {
            private Modifier[] Modifiers;

            #region Initialiation

            /// <summary>
            /// Initialize an InputBinding with an optional list of required modifiers
            /// </summary>
            /// <param name="modifiers">Optional modifiers- Ctrl, Alt, Shift</param>
            /// 
            protected InputBinding(params Modifier[] modifiers)
            {
                Modifiers = new Modifier[modifiers.Length];
                Array.Copy(modifiers, Modifiers, modifiers.Length);
            }

            #endregion

            /// <summary>
            /// True if the InputBinding is active in the given FrameState of the given InputManager
            /// </summary>
            /// <param name="state">Current or Previous frame</param>
            /// <param name="manager">The manager keeping track of current/previous input states</param>
            /// <returns></returns>
            public bool IsActive(FrameState state, InputManager manager)
            {
                KeyboardState keyState = state == FrameState.Current ? manager.CurrentKeyboardState : manager.LastKeyboardState;
                GamePadState gamepadState = state == FrameState.Current ? manager.CurrentGamePadState : manager.LastGamePadState;
                MouseState mouseState = state == FrameState.Current ? manager.CurrentMouseState : manager.LastMouseState;
                return IsRawBindingActive(keyState, gamepadState, mouseState, manager.Settings) && AreExactModifiersActive(keyState);
            }

            protected virtual bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
            {
                return false;
            }

            /// <summary>
            /// Checks if the modifiers that this binding requires 
            /// match the active state of the modifiers in the given KeyboardState
            /// </summary>
            /// <param name="keyState">The KeyboardState to check modifiers against</param>
            /// <returns>True if only all required modifiers are active</returns>
            private bool AreExactModifiersActive(KeyboardState keyState)
            {
                return (Modifier.Alt.IsActive(keyState) == Modifiers.Contains(Modifier.Alt) &&
                        Modifier.Ctrl.IsActive(keyState) == Modifiers.Contains(Modifier.Ctrl) &&
                        Modifier.Shift.IsActive(keyState) == Modifiers.Contains(Modifier.Shift));
            }

            #region Factory

            public static InputBinding CreateBinding(ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params Modifier[] modifiers)
            {
                return new ThumbstickDirectionInputBinding(thumbstickDirection, thumbstick, modifiers);
            }
            
            public static InputBinding CreateBinding(MouseButton mouseButton, params Modifier[] modifiers)
            {
                return new MouseInputBinding(mouseButton, modifiers);
            }

            public static InputBinding CreateBinding(Thumbstick thumbstick, params Modifier[] modifiers)
            {
                return new ThumbstickInputBinding(thumbstick, modifiers);
            }

            public static InputBinding CreateBinding(Buttons button, params Modifier[] modifiers)
            {
                return new ButtonInputBinding(button, modifiers);
            }

            public static InputBinding CreateBinding(Keys key, params Modifier[] modifiers)
            {
                return new KeyInputBinding(key, modifiers);
            }

            public static InputBinding CreateBinding(Trigger trigger, params Modifier[] modifiers)
            {
                return new TriggerInputBinding(trigger, modifiers);
            }

            #endregion

            #region Private Subclasses

            private class ThumbstickDirectionInputBinding : InputBinding
            {
                Thumbstick thumbstick;
                ThumbstickDirection direction;

                public ThumbstickDirectionInputBinding(ThumbstickDirection thumbstickDirection, Thumbstick thumbstick, params Modifier[] modifiers)
                    : base(modifiers)
                {
                    this.thumbstick = thumbstick;
                    direction = thumbstickDirection;
                }

                protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
                {
                    Vector2 gamepadThumbstick = thumbstick == Thumbstick.Left ? gamepadState.ThumbSticks.Left : gamepadState.ThumbSticks.Right;
                    bool isActive = false;
                    switch (direction)
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

            private class MouseInputBinding : InputBinding
            {
                MouseButton button;

                public MouseInputBinding(MouseButton mouseButton, params Modifier[] modifiers)
                    : base(modifiers)
                {
                    button = mouseButton;
                }

                protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
                {
                    ButtonState buttonState;
                    switch (button)
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

            private class ThumbstickInputBinding : InputBinding
            {
                Thumbstick thumbstick;
                public ThumbstickInputBinding(Thumbstick thumbstick, params Modifier[] modifiers)
                    : base(modifiers)
                {
                    this.thumbstick = thumbstick;
                }

                protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
                {
                    Vector2 gamepadThumbstickMag = thumbstick == Thumbstick.Left ? gamepadState.ThumbSticks.Left : gamepadState.ThumbSticks.Right;
                    return gamepadThumbstickMag.Length() >= settings.ThumbstickThreshold;
                }
            }

            private class ButtonInputBinding : InputBinding
            {
                Buttons button;
                public ButtonInputBinding(Buttons button, params Modifier[] modifiers)
                    : base(modifiers)
                {
                    this.button = button;
                }

                protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
                {
                    return gamepadState.IsButtonDown(button);
                }
            }

            private class KeyInputBinding : InputBinding
            {
                Keys key;
                public KeyInputBinding(Keys key, params Modifier[] modifiers)
                    : base(modifiers)
                {
                    this.key = key;
                }

                protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
                {
                    return keyState.IsKeyDown(key);
                }
            }

            private class TriggerInputBinding : InputBinding
            {
                Trigger trigger;
                public TriggerInputBinding(Trigger trigger, params Modifier[] modifiers)
                    : base(modifiers)
                {
                    this.trigger = trigger;
                }

                protected override bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings)
                {
                    float triggerMag = trigger == Trigger.Left ? gamepadState.Triggers.Left : gamepadState.Triggers.Right;
                    return triggerMag >= settings.TriggerThreshold;
                }
            }

            #endregion
        }

        
    }
}

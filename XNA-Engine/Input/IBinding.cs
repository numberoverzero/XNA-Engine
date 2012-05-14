using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
    public interface IBinding
    {
        bool IsActive(InputManager manager, FrameState state);
        bool IsRawBindingActive(KeyboardState keyState, GamePadState gamepadState, MouseState mouseState, InputSettings settings);
        bool AreExactModifiersActive(KeyboardState keyState);
    }
}

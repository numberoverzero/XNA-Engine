using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Input
{
    /// <summary>
    /// Player index, used for gamepad index and such.
    /// A single player game would likely use only PlayerIndex.One.
    /// For games supporting
    /// multiple players on a single keyboard, a custm InputManager implementation can be used
    /// to check overloaded IBindings.
    /// </summary>
    public enum PlayerIndex
    {
        One,
        Two,
        Three,
        Four
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Engine.Input
{
    /// <summary>
    /// Provides singleton modifiers representing Ctrl, Alt, Shift
    /// </summary>
    public class Modifier
    {
        Keys key1, key2;
        private Modifier(Keys key1, Keys key2)
        {
            this.key1 = key1;
            this.key2 = key2;
        }

        public bool IsActive(KeyboardState keyboardState)
        {
            return keyboardState.IsKeyDown(key1) || keyboardState.IsKeyDown(key2);
        }

        #region Modifier Singletons

        private static Modifier ctrl;
        public static Modifier Ctrl
        {
            get
            {
                if (ctrl == null)
                    ctrl = new Modifier(Keys.LeftControl, Keys.RightControl);
                return ctrl;
            }
        }

        private static Modifier alt;
        public static Modifier Alt
        {
            get
            {
                if (alt == null)
                    alt = new Modifier(Keys.LeftAlt, Keys.RightAlt);
                return alt;
            }
        }

        private static Modifier shift;
        public static Modifier Shift
        {
            get
            {
                if (shift == null)
                    shift = new Modifier(Keys.LeftShift, Keys.RightShift);
                return shift;
            }
        }

        #endregion
    }
}

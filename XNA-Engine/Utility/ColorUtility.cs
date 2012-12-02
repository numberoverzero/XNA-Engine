using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using Microsoft.Xna.Framework;

namespace Engine.Utility
{
    public static class ColorUtility
    {
        public static Microsoft.Xna.Framework.Color AsColor(this string color)
        {
            var sdColor = System.Drawing.Color.FromName(color);
            return new Microsoft.Xna.Framework.Color(sdColor.R, sdColor.G, sdColor.B, sdColor.A);
        }
    }
}

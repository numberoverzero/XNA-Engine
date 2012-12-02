using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.UI
{
    public class Frame : Container
    {
        public Dictionary<string, Widget> Names;

        public Frame(string name, float x = 0, float y = 0, float w = 0, float h = 0, bool expandable = true, Alignment? align = null) : base(name, x, y, w, h, expandable, align)
        {
        }
    }
}

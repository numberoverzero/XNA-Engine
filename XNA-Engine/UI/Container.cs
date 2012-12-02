using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI
{
    public class Container : Widget
    {
        public Container(string name, float x = 0, float y = 0, float w = 0, float h = 0, bool expandable = true, Alignment? align = null) : base(name, x, y, w, h, expandable, align)
        {
        }

        protected override void DetermineSize()
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch batch)
        {
            throw new NotImplementedException();
        }
    }
}

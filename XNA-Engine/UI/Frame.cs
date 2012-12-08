using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Input;
using Engine.Rendering;
using Engine.Utility;
using Microsoft.Xna.Framework.Input;

namespace Engine.UI
{
    public class Frame : Container
    {
        public Dictionary<string, Widget> Names;
        public Widget Focus;
        public bool HasFocus
        {
            get { return Focus != null; }
        }
        

        public Frame(string name, float x = 0, float y = 0, float w = 0, float h = 0, bool expandable = true, Alignment? align = null) : base(name, x, y, w, h, expandable, align, null)
        {
            Names = new Dictionary<string, Widget>();
        }

        public override SpriteSheet SpriteSheet
        {
            get { return base.SpriteSheet; }
            set
            {
                base.SpriteSheet = value;
                UpdateLayout();
            }
        }

        public Widget GetElementByName(string name)
        {
            return Names.ContainsKey(name) ? Names[name] : null;
        }

        public override void UpdateLayout()
        {
            Children.Each(c => {c.DetermineSize(); c.ResetSize(c.PreferredSize); });
        }

        public override void OnKeyPress(Keys key, List<ModifierKey> modifiers)
        {
            if (HasFocus) 
                Focus.OnKeyPress(key, modifiers);
            else
                base.OnKeyPress(key, modifiers);
        }

        public override void OnKeyRelease(Keys key, List<ModifierKey> modifiers)
        {
            if (HasFocus)
                Focus.OnKeyRelease(key, modifiers);
            else
                base.OnKeyRelease(key, modifiers);
        }

        public override void OnMousePress(float x, float y, MouseButton button, List<ModifierKey> modifiers)
        {
            if (HasFocus)
                Focus.OnMousePress(x, y, button, modifiers);
            else if (NumberOfChildren > 0 && Children[0].HitTest(x, y))
                Children[0].OnMousePress(x, y, button, modifiers);
            else
            {
                var firstHit = Children.FirstOrDefault(c => c.HitTest(x, y));
                if (firstHit != null)
                {
                    RemoveChild(firstHit);
                    InsertChild(0, firstHit);
                    firstHit.OnMousePress(x, y, button, modifiers);
                }
            }
        }

        public override void OnMouseDrag(float x, float y, float dx, float dy, MouseButton button, List<ModifierKey> modifiers)
        {
            if (HasFocus)
                Focus.OnMouseDrag(x, y, dx, dy, button, modifiers);
            else
                base.OnMouseDrag(x, y, dx, dy, button, modifiers);
        }

        public override void OnMouseRelease(float x, float y, MouseButton button, List<ModifierKey> modifiers)
        {
            if (HasFocus)
                Focus.OnMouseRelease(x, y, button, modifiers);
            else
                base.OnMouseRelease(x, y, button, modifiers);
        }

        public override void OnMouseScroll(float x, float y, float scroll_x, float scroll_y)
        {
            if(HasFocus)
                Focus.OnMouseScroll(x, y, scroll_x, scroll_y);
            else
                base.OnMouseScroll(x, y, scroll_x, scroll_y);
        }

        public override void OnText(string text)
        {
            if (HasFocus)
                Focus.OnText(text);
            else
                base.OnText(text);
        }

        public override void OnTextMotion(int motion, bool select = false)
        {
            if (HasFocus)
                Focus.OnTextMotion(motion, select);
            else
                base.OnTextMotion(motion, select);
        }

    }
}

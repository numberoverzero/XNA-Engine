using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Input;
using Engine.Rendering;
using Engine.Utility;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.UI
{
    public class Container : Widget
    {
        private readonly List<Widget> _children;
        protected IList<Widget> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public virtual int NumberOfChildren
        {
            get { return _children.Count; }
        }

        public Container(string name) : this(name, 0, 0, 0, 0, true, null, null)
        {
        }

        public Container(string name, float x, float y, float w, float h, bool expandable, Alignment? align,
                         IEnumerable<Widget> children) : base(name, x, y, w, h, expandable, align)
        {
            _children = new List<Widget>();
        }

        public override SpriteSheet SpriteSheet
        {
            get { return base.SpriteSheet; }
            set
            {
                base.SpriteSheet = value;
                _children.Each(c => c.SpriteSheet = value);
            }
        }

        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                base.Visible = value;
                _children.Each(c => c.Visible = value);
            }
        }

        public virtual void AddChild(Widget child)
        {
            _children.Add(child);
            child.Parent = this;
            child.SpriteSheet = SpriteSheet;
            FindRoot().UpdateLayout();
            child.UpdateNames();
        }

        public virtual void RemoveChild(Widget child)
        {
            child.RemoveNames();
            _children.Remove(child);
            child.Parent = null;
            FindRoot().UpdateLayout();
        }

        public virtual void InsertChild(int index, Widget child)
        {
            _children.Insert(index, child);
        }

        public virtual void RemoveAllChildren()
        {
            var _copy = new List<Widget>(_children);
            _copy.Each(RemoveChild);
        }

        public override void UpdateNames(string oldName = null)
        {
            base.UpdateNames(oldName);
            _children.Each(c => c.UpdateNames(oldName));
        }

        public override void UpdateGlobalCoords()
        {
            base.UpdateGlobalCoords();
            _children.Each(c => c.UpdateGlobalCoords());
        }

        public override void RemoveNames()
        {
            base.RemoveNames();
            _children.Each(c => c.RemoveNames());
        }

        public override void DetermineSize()
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch batch)
        {
            _children.Each(c => c.Draw(batch));
        }

        public override void OnKeyPress(Keys key, List<ModifierKey> modifiers)
        {
            base.OnKeyPress(key, modifiers);
            _children.Each(c => OnKeyPress(key, modifiers));
        }

        public override void OnKeyRelease(Keys key, List<ModifierKey> modifiers)
        {
            base.OnKeyRelease(key, modifiers);
            _children.Each(c => OnKeyRelease(key, modifiers));
        }

        public override void OnMouseDrag(float x, float y, float dx, float dy, MouseButton button,
                                         List<ModifierKey> modifiers)
        {
            base.OnMouseDrag(x, y, dx, dy, button, modifiers);
            _children.Each(c => OnMouseDrag(x, y, dx, dy, button, modifiers));
        }

        public override void OnMousePress(float x, float y, MouseButton button, List<ModifierKey> modifiers)
        {
            base.OnMousePress(x, y, button, modifiers);
            var b = Bounds;
            _children.Where(c => b.Intersect(c.Bounds).Contains(x, y)).Each(c => OnMousePress(x, y, button, modifiers));
        }

        public override void OnMouseRelease(float x, float y, MouseButton button, List<ModifierKey> modifiers)
        {
            base.OnMouseRelease(x, y, button, modifiers);
            var b = Bounds;
            _children.Where(c => b.Intersect(c.Bounds).Contains(x, y)).Each(c => OnMouseRelease(x, y, button, modifiers));
        }

        public override void OnMouseScroll(float x, float y, float scroll_x, float scroll_y)
        {
            base.OnMouseScroll(x, y, scroll_x, scroll_y);
            var b = Bounds;
            _children.Where(c => b.Intersect(c.Bounds).Contains(x, y)).Each(c => OnMouseScroll(x, y, scroll_x, scroll_y));
        }

        public override void OnText(string text)
        {
            base.OnText(text);
            _children.Each(c => c.OnText(text));
        }

        public override void OnTextMotion(int motion, bool select = false)
        {
            base.OnTextMotion(motion, select);
            _children.Each(c => c.OnTextMotion(motion, select));
        }
    }
}
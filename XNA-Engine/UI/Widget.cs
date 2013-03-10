using System;
using System.Collections.Generic;
using Engine.Input;
using Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.UI
{
    public abstract class Widget
    {
        protected Alignment Alignment;

        protected bool Expandable;
        protected float GX, GY;
        public Widget Parent;
        public Vector2 PreferredSize;
        private float _h;
        private string _name;
        private SpriteSheet _spriteSheet;
        private float _w;
        private float _x;

        private float _y;

        public Widget(string name, float x = 0, float y = 0, float w = 0, float h = 0, bool expandable = true,
                      Alignment? align = null)
        {
            _name = name;
            _x = x;
            _y = y;
            _w = w;
            _h = h;

            PreferredSize = Vector2.Zero;
            Parent = null;
            SpriteSheet = null;
            Visible = true;

            Alignment = align ?? Alignment.Centered;

            Expandable = expandable;
        }

        protected float X
        {
            get { return _x; }
            set
            {
                _x = value;
                FindRoot().UpdateLayout();
            }
        }

        protected float Y
        {
            get { return _y; }
            set
            {
                _y = value;
                FindRoot().UpdateLayout();
            }
        }

        protected float W
        {
            get { return _w; }
            set
            {
                _w = value;
                FindRoot().UpdateLayout();
            }
        }

        protected float H
        {
            get { return _h; }
            set
            {
                _h = value;
                FindRoot().UpdateLayout();
            }
        }

        protected string Name
        {
            get { return _name; }
            set
            {
                var oldName = _name;
                _name = value;
                UpdateNames(oldName);
            }
        }

        public virtual bool Visible { get; set; }

        public virtual bool IsFocus
        {
            get
            {
                var f = FindRoot() as Frame;
                if (this == f) return true;
                if (f == null) return false;
                
                return this == f.Focus;
            }
        }

        public virtual SpriteSheet SpriteSheet
        {
            get { return _spriteSheet; }
            set
            {
                _spriteSheet = value;
            }
        }

        public virtual Rectangle Bounds
        {
            get { return new Rectangle(GX, GY, W, H); }
        }

        protected Widget FindRoot()
        {
            var root = this;
            while (root.Parent != null)
                root = root.Parent;
            return root;
        }

        public virtual void UpdateLayout()
        {
        }

        public virtual void UpdateNames(string oldName = null)
        {
            var frame = FindRoot() as Frame;
            if (frame == null) return;
            if (!String.IsNullOrEmpty(oldName))
                frame.Names.Remove(oldName);
            if (!String.IsNullOrEmpty(Name))
                frame.Names[Name] = this;
        }

        public virtual void RemoveNames()
        {
            var frame = FindRoot() as Frame;
            if (frame == null) return;
            if (!String.IsNullOrEmpty(Name))
                frame.Names.Remove(Name);
        }

        public virtual void UpdateGlobalCoords()
        {
            GX = X;
            GY = Y;
            if (Parent != null)
            {
                GX += Parent.GX;
                GY += Parent.GY;
            }
        }

        public abstract void DetermineSize();

        public virtual void ResetSize(Vector2 size)
        {
            _w = size.X;
            _h = size.Y;
        }

        public virtual bool HitTest(Vector2 pos)
        {
            return Bounds.Contains(pos);
        }
        public virtual bool HitTest(float x, float y)
        {
            return Bounds.Contains(x, y);
        }

        public abstract void Draw(SpriteBatch batch);

        public virtual void OnMousePress(float x, float y, MouseButton button, List<ModifierKey> modifiers)
        {
        }

        public virtual void OnMouseDrag(float x, float y, float dx, float dy, MouseButton button,
                                        List<ModifierKey> modifiers)
        {
        }

        public virtual void OnMouseRelease(float x, float y, MouseButton button, List<ModifierKey> modifiers)
        {
        }

        public virtual void OnMouseScroll(float x, float y, float scroll_x, float scroll_y)
        {
        }

        public virtual void OnKeyPress(Keys key, List<ModifierKey> modifiers)
        {
        }

        public virtual void OnKeyRelease(Keys key, List<ModifierKey> modifiers)
        {
        }

        public virtual void OnText(string text)
        {
        }

        public virtual void OnTextMotion(int motion, bool select = false)
        {
        }
    }
}
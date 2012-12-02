using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI
{
    public abstract class Widget
    {
        private float _x;
        protected float X
        {
            get { return _x; }
            set { 
                _x = value;
                FindRoot().UpdateLayout();
            }
        }

        private float _y;
        protected float Y
        {
            get { return _y; }
            set
            {
                _y = value;
                FindRoot().UpdateLayout();
            }
        }

        private float _w;
        protected float W
        {
            get { return _w; }
            set
            {
                _w = value;
                FindRoot().UpdateLayout();
            }
        }

        private float _h;
        protected float H
        {
            get { return _h; }
            set
            {
                _h = value;
                FindRoot().UpdateLayout();
            }
        }

        private string _name;
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

        protected float GX, GY;

        protected Vector2 PrefSize;

        protected Widget Parent;

        private bool _dirty;

        protected bool Visible;
        protected Alignment Alignment;

        protected bool Expandable;
        private SpriteSheet _spriteSheet;

        protected SpriteSheet SpriteSheet
        {
            get { return _spriteSheet; }
            set
            {
                _spriteSheet = value;
                _dirty = true;
            }
        }

        public Widget(string name, float x=0, float y=0, float w=0, float h=0, bool expandable=true,
            Alignment? align = null)
        {
            _name = name;
            _x = x;
            _y = y;
            _w = w;
            _h = h;

            PrefSize = Vector2.Zero;
            Parent = null;
            SpriteSheet = null;
            _dirty = false;
            Visible = true;

            Alignment = align ?? Alignment.Centered;

            Expandable = expandable;
        }

        protected Widget FindRoot()
        {
            var root = this;
            while (root.Parent != null)
                root = root.Parent;
            return root;
        }

        protected void UpdateLayout()
        {
            _dirty = true;
        }

        protected void UpdateNames(string oldName = null)
        {
            var frame = FindRoot() as Frame;
            if(frame == null) return;
            if (!String.IsNullOrEmpty(oldName))
                frame.Names.Remove(oldName);
            if (!String.IsNullOrEmpty(Name))
                frame.Names[Name] = this;
        }

        protected virtual void RemoveNames()
        {
            var frame = FindRoot() as Frame;
            if (frame == null) return;
            if (!String.IsNullOrEmpty(Name))
                frame.Names.Remove(Name);
        }

        protected virtual void UpdateGlobalCoords()
        {
            GX = X;
            GY = Y;
            if (Parent != null)
            {
                GX += Parent.GX;
                GY += Parent.GY;
            }
        }

        protected abstract void DetermineSize();

        protected virtual void ResetSize(Vector2 size)
        {
            _w = size.X;
            _h = size.Y;
            _dirty = true;
        }

        protected virtual bool HitTest(Vector2 pos)
        {
            return Bounds.Contains(pos);
        }

        protected virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle(GX, GY, W, H);
            }
        }

        public abstract void Draw(SpriteBatch batch);
    }
}

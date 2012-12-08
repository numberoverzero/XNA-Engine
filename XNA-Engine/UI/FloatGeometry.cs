using System;
using Microsoft.Xna.Framework;

namespace Engine.UI
{
    /// <summary>
    ///     Rectangle with float position/dimension
    /// </summary>
    public struct Rectangle
    {
        private readonly float[] _dimensions;

        public Rectangle(Vector2 pos, Vector2 dims) : this(pos.X, pos.Y, dims.X, dims.Y)
        {
        }

        public Rectangle(float x, float y, float w, float h)
        {
            _dimensions = new float[4];
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public Rectangle(Microsoft.Xna.Framework.Rectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
        {
        }

        public Rectangle(Rectangle rect) : this(rect.X, rect.Y, rect.W, rect.H)
        {
        }

        public float X
        {
            get { return _dimensions[0]; }
            set { _dimensions[0] = value; }
        }

        public float Y
        {
            get { return _dimensions[1]; }
            set { _dimensions[1] = value; }
        }

        public float W
        {
            get { return _dimensions[2]; }
            set { _dimensions[2] = value; }
        }

        public float H
        {
            get { return _dimensions[3]; }
            set { _dimensions[3] = value; }
        }

        public Vector2 Pos
        {
            get { return new Vector2(X, Y); }
        }

        public Vector2 Dimensions
        {
            get { return new Vector2(W, H); }
        }

        public static Rectangle Empty
        {
            get { return new Rectangle(0, 0, 0, 0); }
        }

        public bool Contains(Vector2 pos)
        {
            return Contains(pos.X, pos.Y);
        }

        public bool Contains(float x, float y)
        {
            return (x >= X && x <= X + W && y >= Y && y <= Y + H);
        }

        public bool Intersects(Rectangle rect)
        {
            return !(rect.X > X + W ||
                     rect.X + rect.W < X ||
                     rect.Y > Y + H ||
                     rect.Y + rect.H < Y);
        }

        public Rectangle Intersect(Rectangle other)
        {
            var left = Math.Max(X, other.X);
            var right = Math.Min(X + W, other.X + other.W);
            var top = Math.Max(Y, other.Y);
            var bottom = Math.Min(Y + H, other.Y + other.H);
            return left < right && top < bottom
                       ? new Rectangle(left, top, right - left, bottom - top)
                       : Empty;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
    public struct Sprite
    {
        public int H;
        public string Name;
        public Texture2D Texture;
        public int W;
        public int X;
        public int Y;

        public static Sprite NullSprite
        {
            get { return new Sprite(); }
        }

        public Rectangle SrcRect
        {
            get { return new Rectangle(X, Y, W, H); }
        }

        public Vector2 Dimensions
        {
            get { return new Vector2(W, H); }
        }

        public Vector2 SrcPosition
        {
            get { return new Vector2(X, Y); }
        }
    }
}
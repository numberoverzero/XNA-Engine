using Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Utility
{
    public static class SpriteBatchExtensions
    {
        /// <summary>
        ///     Draw with tint color white, origin 0, rotation 0, scale 1, no effects, depth 0
        /// </summary>
        public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position)
        {
            Draw(batch, sprite, position, Color.White);
        }

        /// <summary>
        ///     Draw with origin 0, rotation 0, scale 1, no effects, depth 0
        /// </summary>
        public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, Color color)
        {
            Draw(batch, sprite, position, color, 0, Vector2.Zero, Vector2.One);
        }

        /// <summary>
        ///     Draw with no effects, depth 0
        /// </summary>
        public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, Color color, float rotation,
                                Vector2 origin, Vector2 scale)
        {
            Draw(batch, sprite, position, color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, Color color, float rotation,
                                Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            batch.Draw(sprite.Texture, position, sprite.SrcRect, color, rotation,
                       origin, scale, effects, layerDepth);
        }
    }
}
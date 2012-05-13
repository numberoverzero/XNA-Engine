using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Utility;

namespace Engine.Rendering
{
    public static class BasicShapeRenderer
    {
        private static GraphicsDevice graphicsDevice;
        private static Texture2D pixel1x1;
        private static bool CanDraw
        {
            get { return (graphicsDevice != null) && (pixel1x1 != null); }
        }

        /// <summary>
        /// The BasicShapeRenderer won't actually render until it's been initialized
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (BasicShapeRenderer.graphicsDevice == null)
                BasicShapeRenderer.graphicsDevice = graphicsDevice;
            if (pixel1x1 == null)
                pixel1x1 = ColorTextureGenerator.Create(graphicsDevice, 1, 1, Color.White);
        }

        /// <summary>
        /// Draws a filled square whose center is position
        /// </summary>
        /// <param name="batch">SpriteBatch for drawing</param>
        /// <param name="position">Center of the Square</param>
        /// <param name="color">Color of the square</param>
        /// <param name="width">Square side length</param>
        /// <param name="rotation">Rotation in radians about the center</param>
        public static void DrawSolidSquare(SpriteBatch batch, Vector2 position, Color color, float width, float rotation)
        {
            if (!CanDraw) return;
            var drawPosition = position - new Vector2(width / 2);
            drawPosition = drawPosition.RotateAbout(position, rotation);
            batch.Draw(pixel1x1, drawPosition, null, color, rotation, Vector2.Zero, width, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws a hollow square whose center is position, of width lineWidth (default 1px, does not exceed width)
        /// </summary>
        /// <param name="batch">SpriteBatch for drawing</param>
        /// <param name="position">Center of the Square</param>
        /// <param name="color">Color of the square</param>
        /// <param name="width">Square side length</param>
        /// <param name="rotation">Rotation in radians about the center</param>
        /// <param name="lineWidth">Width in pixels of the line</param>
        public static void DrawSquareOutline(SpriteBatch batch, Vector2 position, Color color, float width, float rotation, float lineWidth = 1)
        {
            if (!CanDraw) return;
            DrawRectangleOutline(batch, position, color, new Vector2(width, width), rotation, lineWidth);
        }

        public static void DrawRectangleOutline(SpriteBatch batch, Vector2 position, Color color, Vector2 dimensions, float rotation, float lineWidth = 1)
        {
            if (!CanDraw) return;
            Vector2 dim2 = dimensions / 2;
            Vector2[] corners = new Vector2[] {position + new Vector2(-dim2.X + lineWidth, -dim2.Y + lineWidth),
                                               position + new Vector2(-dim2.X + lineWidth, dim2.Y - lineWidth),
                                               position + new Vector2(dim2.X - lineWidth, dim2.Y - lineWidth),
                                               position + new Vector2(dim2.X - lineWidth, -dim2.Y + lineWidth),
                                               position + new Vector2(-dim2.X + lineWidth, -dim2.Y + lineWidth)
                                              };

            for (int i = 0; i < 5; i++)
            {
                corners[i] = corners[i].RotateAbout(position, rotation);
            }

            Vector2 scale = new Vector2(0, lineWidth);
            Vector2 segment;
            for (int i = 0; i < 4; i++)
            {
                segment = (corners[i + 1] - corners[i]);
                scale.X = segment.Length() + lineWidth;
                batch.Draw(pixel1x1, corners[i], null, color, (float)segment.AsAngle(), Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }
    }
}

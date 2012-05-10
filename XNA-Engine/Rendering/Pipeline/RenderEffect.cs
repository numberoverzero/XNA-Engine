#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Engine.Rendering.Pipeline
{
    public class RenderEffect : IRenderEffect
    {
        protected GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        public virtual void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
        }
        
        public virtual void UnloadContent()
        {
            graphicsDevice = null;
            spriteBatch = null;
        }

        public virtual void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture) { }

        protected void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, BlendState blendState, Effect effect)
        {
            graphicsDevice.SetRenderTarget(renderTarget);
            DrawFullscreenQuad(texture, blendState, renderTarget.Width, renderTarget.Height, effect);
        }

        protected void DrawFullscreenQuad(Texture2D texture, BlendState blendState, int width, int height, Effect effect)
        {
            spriteBatch.Begin(0, blendState, null, null, null, effect);
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();
        }
        
    }
}

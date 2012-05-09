using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Engine.Rendering.Pipeline
{
    public class RenderEffect : IRenderEffect
    {
        protected GraphicsDevice _graphicsDevice;
        private SpriteBatch _batch;

        public virtual void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _batch = spriteBatch;
        }
        
        public virtual void UnloadContent()
        {
            _graphicsDevice = null;
            _batch = null;
        }

        public virtual void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture) { }

        protected void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, BlendState blendState, Effect effect)
        {
            _graphicsDevice.SetRenderTarget(renderTarget);
            DrawFullscreenQuad(texture, blendState, renderTarget.Width, renderTarget.Height, effect);
        }

        protected void DrawFullscreenQuad(Texture2D texture, BlendState blendState, int width, int height, Effect effect)
        {
            _batch.Begin(0, blendState, null, null, null, effect);
            _batch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            _batch.End();
        }
        
    }
}

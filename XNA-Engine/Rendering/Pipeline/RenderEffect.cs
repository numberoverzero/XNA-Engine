#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Engine.Rendering.Pipeline
{
    /// <summary>
    /// Renders an effect on a preEffect texture, out to a postEffect texture.
    /// preEffect texture is unmodified, unless preEffect texture is postEffect texture
    /// </summary>
    public class RenderEffect : IRenderEffect
    {
        /// <summary>
        /// Used for switching render targets and drawing full-quad effects
        /// </summary>
        protected GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Load any required content the effect requires for rendering
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="spriteBatch"></param>
        public virtual void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
        }

        /// <summary>
        /// Free up resources when you don't need to render bloom effects anymore.
        /// </summary>
        public virtual void UnloadContent()
        {
            graphicsDevice = null;
            spriteBatch = null;
        }

        /// <summary>
        /// Renders the results of the effect on the preEffectTexture to the postEffectTexture.
        /// (preEffectTexture unmodified unless pre == post)
        /// </summary>
        /// <param name="preEffectTexture"></param>
        /// <param name="postEffectTexture"></param>
        public virtual void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture) { }

        /// <summary>
        /// Reset any state the effect might track, such as camera deltas or elapsed game time
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Draw a texture to the renderTarget using a particular BlendState and Effect
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="renderTarget"></param>
        /// <param name="blendState"></param>
        /// <param name="effect"></param>
        protected void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, BlendState blendState, Effect effect)
        {
            graphicsDevice.SetRenderTarget(renderTarget);
            DrawFullscreenQuad(texture, blendState, renderTarget.Width, renderTarget.Height, effect);
        }

        /// <summary>
        /// Draw a texture to a widthxheight section of the current RenderTarget using a particular BlendState and Effect
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="blendState"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="effect"></param>
        protected void DrawFullscreenQuad(Texture2D texture, BlendState blendState, int width, int height, Effect effect)
        {
            spriteBatch.Begin(0, blendState, null, null, null, effect);
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();
        }

        static RenderEffect noop;
        /// <summary>
        /// A RenderEffect which does nothing
        /// </summary>
        public static RenderEffect None
        {
            get
            {
                if (noop == null)
                    noop = new RenderEffect.EmptyRenderEffect();
                return noop;
            }
        }

        private class EmptyRenderEffect : RenderEffect
        {
            public EmptyRenderEffect() { }

            /// <summary>
            /// Renders the results of the effect on the preEffectTexture to the postEffectTexture.
            /// (preEffectTexture unmodified unless pre == post)
            /// </summary>
            /// <param name="preEffectTexture"></param>
            /// <param name="postEffectTexture"></param>
            public override void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture){ }
        }

        
    }
}

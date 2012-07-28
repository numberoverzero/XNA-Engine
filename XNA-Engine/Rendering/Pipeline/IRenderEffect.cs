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
    public interface IRenderEffect
    {
        /// <summary>
        /// Load any required content the effect requires for rendering
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="spriteBatch"></param>
        void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);

        /// <summary>
        /// Free up resources when you don't need to render bloom effects anymore.
        /// </summary>
        void UnloadContent(); 
        
        /// <summary>
        /// Renders the results of the effect on the preEffectTexture to the postEffectTexture.
        /// (preEffectTexture unmodified unless pre == post)
        /// </summary>
        /// <param name="preEffectTexture"></param>
        /// <param name="postEffectTexture"></param>
        void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture);

        /// <summary>
        /// Reset any state the effect might track, such as camera deltas or elapsed game time
        /// </summary>
        void Reset();
    }
}

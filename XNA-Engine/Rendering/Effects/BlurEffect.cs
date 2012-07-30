using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Rendering.Pipeline;
using Engine.Utility;

namespace Engine.Rendering.Effects
{
    /// <summary>
    /// Blurs the movement of objects, appropriately compensating for screen shake (radial) and position changes.
    /// Does not account for scale changes, which will 
    /// </summary>
    public class BlurEffect : RenderEffect
    {
        #region Fields

        private RenderTarget2D combineTarget; 
        private RenderTarget2D previousFrame;
        private Effect reduceAlphaEffect;

        private Vector2 renderDimensions;
        /// <summary>
        /// The percent of the previous frames that decays each frame.
        /// The amount of opacity from previous frames that remains is 1 - AlphaDecay.
        /// <example>
        /// For AlphaDecay = 0.05, 95% of the previous frame is kept.  It will take 14 frames to get to &lt;= 50% opacity
        /// </example>
        /// </summary>
        public float AlphaDecay = 0.003f;

        #endregion

        #region Constructors

        /// <summary>
        /// Default BlurEffect - never decays
        /// </summary>
        public BlurEffect() : this(0) { }

        /// <summary>
        /// BlurEffect which loses alphaDecay% opacity per frame
        /// </summary>
        /// <param name="alphaDecay"></param>
        public BlurEffect(float alphaDecay) : base()
        {
            AlphaDecay = alphaDecay;
        }

        #endregion

        #region Content Management

        /// <summary>
        /// Load any required content the effect requires for rendering
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="spriteBatch"></param>
        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            reduceAlphaEffect = contentManager.Load<Effect>("Effects/Blur/ReduceAlpha");

            renderDimensions = new Vector2(graphicsDevice.PresentationParameters.BackBufferWidth,
                                           graphicsDevice.PresentationParameters.BackBufferHeight);
            
            previousFrame = graphicsDevice.CreateFullscreenRenderTarget(true);
            combineTarget = graphicsDevice.CreateFullscreenRenderTarget(false);

            base.LoadContent(contentManager, graphicsDevice, spriteBatch);
        }

        /// <summary>
        /// Free up resources when you don't need to render bloom effects anymore.
        /// </summary>
        public override void UnloadContent()
        {
            reduceAlphaEffect = null;
            previousFrame = null;
            combineTarget = null;

            base.UnloadContent();
        }

        #endregion

        /// <summary>
        /// This should most likely be called every frame, to update the change in position of the camera.
        /// This will make the blur render w.r.t. the player.  Of course, you could also blur by a normal to
        /// the camera delta or an oscillating angle to the player, to create different effects.
        /// </summary>
        /// <param name="delta">Change in position of the blur effect since the last frame.  
        ///                     Do not scale the offset to texture coordinates, that will be done for you.</param>
        public void SetFrameDelta(Vector2 delta)
        {
            reduceAlphaEffect.Parameters["offsetXY"].SetValue(delta / renderDimensions);
        }

        /// <summary>
        /// Renders the results of the effect on the preEffectTexture to the postEffectTexture.
        /// (preEffectTexture unmodified unless pre == post)
        /// </summary>
        /// <param name="preEffectTexture"></param>
        /// <param name="postEffectTexture"></param>
        public override void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture)
        {
            reduceAlphaEffect.Parameters["factor"].SetValue(AlphaDecay);

            // Pass 1: render the lastFrame onto combineTarget using the reduceAlphaEffect to "fade out" the last draw data.
            
            graphicsDevice.Textures[0] = previousFrame;
            DrawFullscreenQuad(previousFrame, combineTarget, BlendState.Opaque, reduceAlphaEffect);

            // Pass 2: render the contents of this latest frame (preEffectTexture) to the renderTarget1
            DrawFullscreenQuad(preEffectTexture, combineTarget, BlendState.AlphaBlend, null);

            // Pass 3: copy everything from the combineTarget to the lastFrame for the next draw
            DrawFullscreenQuad(combineTarget, previousFrame, BlendState.Opaque, null);

            // Pass 4: copy the combineTarget to the preEffectTexture
            DrawFullscreenQuad(combineTarget, postEffectTexture, BlendState.NonPremultiplied, null);
        }

        /// <summary>
        /// Reset any state the effect might track, such as camera deltas or elapsed game time
        /// </summary>
        public override void Reset()
        {
            graphicsDevice.SetRenderTarget(previousFrame);
            graphicsDevice.Clear(Color.Transparent);
            base.Reset();
        }
    }
}

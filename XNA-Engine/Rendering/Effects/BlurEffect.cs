using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Rendering.Pipeline;

namespace Engine.Rendering.Effects
{
    public class BlurEffect : RenderEffect
    {
        #region Fields

        private RenderTarget2D _combineTarget; 
        private RenderTarget2D _lastFrame;
        private Effect _reduceAlphaEffect;

        private Vector2 renderDimensions;
        public float AlphaDecay = 0.003f;

        #endregion

        #region Constructors


        public BlurEffect() : this(0) { }
        public BlurEffect(float alphaDecay) : base()
        {
            AlphaDecay = alphaDecay;
        }


        #endregion

        #region Content Management


        public override void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _reduceAlphaEffect = contentManager.Load<Effect>("Effects/Blur/ReduceAlpha");

            PresentationParameters pp = graphicsDevice.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat; 
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            renderDimensions = new Vector2(width, height);

            _lastFrame = new RenderTarget2D(graphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.PreserveContents);
            _combineTarget = new RenderTarget2D(graphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            base.LoadContent(contentManager, graphicsDevice, spriteBatch);
        }

        public override void UnloadContent()
        {
            _reduceAlphaEffect = null;
            _lastFrame = null;
            _combineTarget = null;

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
            _reduceAlphaEffect.Parameters["offsetXY"].SetValue(delta / renderDimensions);
        }

        public override void ApplyEffect(RenderTarget2D preEffectTexture, RenderTarget2D postEffectTexture)
        {
            _reduceAlphaEffect.Parameters["factor"].SetValue(AlphaDecay);

            // Pass 1: render the lastFrame onto combineTarget using the reduceAlphaEffect to "fade out" the last draw data.
            
            _graphicsDevice.Textures[0] = _lastFrame;
            DrawFullscreenQuad(_lastFrame, _combineTarget, BlendState.Opaque, _reduceAlphaEffect);

            // Pass 2: render the contents of this latest frame (preEffectTexture) to the renderTarget1
            DrawFullscreenQuad(preEffectTexture, _combineTarget, BlendState.AlphaBlend, null);

            // Pass 3: copy everything from the combineTarget to the lastFrame for the next draw
            DrawFullscreenQuad(_combineTarget, _lastFrame, BlendState.Opaque, null);

            // Pass 4: copy the combineTarget to the preEffectTexture
            DrawFullscreenQuad(_combineTarget, postEffectTexture, BlendState.NonPremultiplied, null);
        }
    }
}
